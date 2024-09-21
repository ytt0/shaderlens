namespace Shaderlens.Presentation.Input
{
    using static WinApi;

    public interface IGlobalInputBindings
    {
        void Add(IInputSpan inputSpan, Action handler, Func<bool>? isEnabled, bool allowRepeat);
    }

    public interface IGlobalInputListener
    {
        void GlobalInputReceived(int id);
    }

    public class GlobalInputBindings : IGlobalInputBindings, IGlobalInputListener, IDisposable
    {
        private class EmptyGlobalInputBindings : IGlobalInputBindings
        {
            public void Add(IInputSpan inputSpan, Action? handler, Func<bool>? isEnabled, bool allowRepeat)
            {
            }
        }

        private class HotKeyWriter : IInputSpanWriter
        {
            private class Token : IDisposable
            {
                private readonly HotKeyWriter inputWriter;

                public Token(HotKeyWriter inputWriter)
                {
                    this.inputWriter = inputWriter;
                }

                public void Dispose()
                {
                    this.inputWriter.PopScope();
                }
            }

            private readonly List<HotKey> hotKeys;
            private readonly uint repeatModifier;
            private int anyScopeCount;
            private int allScopeCount;

            private uint scopeKey;
            private uint scopeModifierKeysMask;
            private bool isScopeValid;

            public HotKeyWriter(bool allowRepeat)
            {
                this.hotKeys = new List<HotKey>();
                this.repeatModifier = allowRepeat ? 0u : MOD_NOREPEAT;
            }

            public IEnumerable<HotKey> GetResults()
            {
                if (this.anyScopeCount > 0 || this.allScopeCount > 0)
                {
                    throw new Exception($"{nameof(HotKeyWriter)} scopes are still open");
                }

                return this.hotKeys;
            }

            public void WriteKey(Key key)
            {
                if (this.allScopeCount == 0)
                {
                    if (VirtualKeyMap.TryGetIndex(key, out var keyCode))
                    {
                        this.hotKeys.Add(new HotKey(this.repeatModifier, keyCode));
                    }
                }
                else if (this.scopeKey == 0 && VirtualKeyMap.TryGetIndex(key, out var keyCode))
                {
                    this.scopeKey = keyCode;
                }
                else
                {
                    this.isScopeValid = false;
                }
            }

            public void WriteModifierKey(ModifierKey modifierKey)
            {
                if (this.allScopeCount > 0)
                {
                    this.scopeModifierKeysMask |= GetModifierKeyMask(modifierKey);
                }
            }

            public void WriteMouseButton(MouseButton button)
            {
                if (this.allScopeCount > 0)
                {
                    this.isScopeValid = false;
                }
            }

            public void WriteMouseScroll(MouseScroll direction)
            {
                if (this.allScopeCount > 0)
                {
                    this.isScopeValid = false;
                }
            }

            public IDisposable WriteAny()
            {
                if (this.allScopeCount > 0)
                {
                    throw new Exception("Invalid input structure, \"Any\" scope should not be created inside an \"All\" scope. Input combinations should be flattened. " +
                        "For example, an input such as \"All(Ctrl, Any(A, B))\" should be structured as \"Any(All(Ctrl, A), All(Ctrl, B))\" (serialized as [\"Ctrl A\", \"Ctrl B\"] json array)");
                }

                this.anyScopeCount++;
                return new Token(this);
            }

            public IDisposable WriteAll()
            {
                this.isScopeValid = this.allScopeCount == 0;
                this.allScopeCount++;
                return new Token(this);
            }

            private void PopScope()
            {
                if (this.allScopeCount > 0)
                {
                    this.allScopeCount--;

                    if (this.allScopeCount == 0)
                    {
                        if (this.isScopeValid && this.scopeKey != 0)
                        {
                            this.hotKeys.Add(new HotKey(this.scopeModifierKeysMask | this.repeatModifier, this.scopeKey));
                        }

                        this.scopeKey = 0;
                        this.scopeModifierKeysMask = 0;
                    }
                }
                else if (this.anyScopeCount > 0)
                {
                    this.anyScopeCount--;
                }
                else
                {
                    throw new Exception($"All {nameof(HotKeyWriter)} scopes are already closed");
                }
            }

            private static uint GetModifierKeyMask(ModifierKey modifierKey)
            {
                switch (modifierKey)
                {
                    case ModifierKey.Alt: return MOD_ALT;
                    case ModifierKey.Ctrl: return MOD_CONTROL;
                    case ModifierKey.Shift: return MOD_SHIFT;
                    case ModifierKey.Win: return MOD_WIN;
                    default: throw new NotSupportedException($"Unexpected {nameof(ModifierKey)} \"{modifierKey}\"");
                }
            }
        }

        private record HotKey(uint Modifiers, uint Key);
        private record Binding(HotKey HotKey, List<BindingHandler> Handlers);
        private record BindingHandler(Action Handler, Func<bool>? IsEnabled);

        public static readonly IGlobalInputBindings Empty = new EmptyGlobalInputBindings();

        private readonly Dictionary<int, Binding> bindings;
        private readonly Dictionary<HotKey, int> hotKeyIds;
        private IntPtr handle;
        private int nextId;

        public GlobalInputBindings()
        {
            this.bindings = new Dictionary<int, Binding>();
            this.hotKeyIds = new Dictionary<HotKey, int>();
        }

        public void Dispose()
        {
            if (this.handle != IntPtr.Zero)
            {
                foreach (var key in this.bindings.Keys)
                {
                    UnregisterHotKey(this.handle, key);
                }
            }

            this.bindings.Clear();
            this.handle = IntPtr.Zero;
        }

        public void Add(IInputSpan inputSpan, Action handler, Func<bool>? isEnabled, bool allowRepeat)
        {
            var writer = new HotKeyWriter(allowRepeat);
            inputSpan.WriteTo(writer);

            foreach (var hotKey in writer.GetResults())
            {
                if (!this.hotKeyIds.TryGetValue(hotKey, out var id))
                {
                    id = this.nextId++;
                    this.hotKeyIds.Add(hotKey, id);
                }

                if (!this.bindings.TryGetValue(id, out var binding))
                {
                    binding = new Binding(hotKey, new List<BindingHandler>());
                    this.bindings.Add(id, binding);

                    if (this.handle != IntPtr.Zero)
                    {
                        RegisterHotKey(this.handle, id, hotKey.Modifiers, hotKey.Key);
                    }
                }

                binding.Handlers.Add(new BindingHandler(handler, isEnabled));
            }
        }

        public void GlobalInputReceived(int id)
        {
            if (this.bindings.TryGetValue(id, out var binding))
            {
                foreach (var handler in binding.Handlers)
                {
                    if (handler.IsEnabled == null || handler.IsEnabled())
                    {
                        handler.Handler();
                        return;
                    }
                }
            }
        }

        public void SetTargetWindow(IntPtr handle)
        {
            if (this.handle != IntPtr.Zero)
            {
                throw new Exception("Target window has already been set");
            }

            this.handle = handle;

            foreach (var pair in this.bindings)
            {
                var id = pair.Key;
                var hotKey = pair.Value.HotKey;
                RegisterHotKey(this.handle, id, hotKey.Modifiers, hotKey.Key);
            }
        }
    }
}
