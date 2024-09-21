namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanWriter
    {
        void WriteKey(Key key);
        void WriteModifierKey(ModifierKey modifierKey);
        void WriteMouseButton(MouseButton button);
        void WriteMouseScroll(MouseScroll direction);
        IDisposable WriteAny();
        IDisposable WriteAll();
    }

    public class InputSpanWriter : IInputSpanWriter
    {
        private class Token : IDisposable
        {
            private readonly InputSpanWriter inputWriter;

            public Token(InputSpanWriter inputWriter)
            {
                this.inputWriter = inputWriter;
            }

            public void Dispose()
            {
                this.inputWriter.PopScope();
            }
        }

        private readonly IInputValueSerializer serializer;
        private readonly List<string> inputs;
        private readonly HashSet<Key> scopeKeys;
        private readonly HashSet<ModifierKey> scopeModifierKeys;
        private readonly HashSet<MouseButton> scopeMouseButtons;
        private readonly HashSet<MouseScroll> scopeMouseScrolls;

        private int anyScopeCount;
        private int allScopeCount;

        public InputSpanWriter(IInputValueSerializer serializer)
        {
            this.serializer = serializer;
            this.inputs = new List<string>();
            this.scopeKeys = new HashSet<Key>();
            this.scopeModifierKeys = new HashSet<ModifierKey>();
            this.scopeMouseButtons = new HashSet<MouseButton>();
            this.scopeMouseScrolls = new HashSet<MouseScroll>();
        }

        public IEnumerable<string> GetResults()
        {
            if (this.anyScopeCount > 0 || this.allScopeCount > 0)
            {
                throw new Exception($"{nameof(InputSpanWriter)} scopes are still open");
            }

            return this.inputs;
        }

        public void WriteKey(Key key)
        {
            if (this.allScopeCount > 0)
            {
                this.scopeKeys.Add(key);
            }
            else
            {
                this.inputs.Add(this.serializer.Serialize(key));
            }
        }

        public void WriteModifierKey(ModifierKey modifierKey)
        {
            if (this.allScopeCount > 0)
            {
                this.scopeModifierKeys.Add(modifierKey);
            }
            else
            {
                this.inputs.Add(this.serializer.Serialize(modifierKey));
            }
        }

        public void WriteMouseButton(MouseButton button)
        {
            if (this.allScopeCount > 0)
            {
                this.scopeMouseButtons.Add(button);
            }
            else
            {
                this.inputs.Add(this.serializer.Serialize(button));
            }
        }

        public void WriteMouseScroll(MouseScroll direction)
        {
            if (this.allScopeCount > 0)
            {
                this.scopeMouseScrolls.Add(direction);
            }
            else
            {
                this.inputs.Add(this.serializer.Serialize(direction));
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
                    var inputSegments = new List<string>();
                    inputSegments.AddRange(this.scopeModifierKeys.OrderBy(modifierKey => modifierKey).Select(this.serializer.Serialize));
                    inputSegments.AddRange(this.scopeKeys.OrderBy(key => key).Select(this.serializer.Serialize));
                    inputSegments.AddRange(this.scopeMouseButtons.OrderBy(mouseButton => mouseButton).Select(this.serializer.Serialize));
                    inputSegments.AddRange(this.scopeMouseScrolls.OrderBy(mouseScroll => mouseScroll).Select(this.serializer.Serialize));

                    this.inputs.Add(String.Join(" ", inputSegments));

                    this.scopeKeys.Clear();
                    this.scopeModifierKeys.Clear();
                    this.scopeMouseButtons.Clear();
                    this.scopeMouseScrolls.Clear();
                }
            }
            else if (this.anyScopeCount > 0)
            {
                this.anyScopeCount--;
            }
            else
            {
                throw new Exception($"All {nameof(InputSpanWriter)} scopes are already closed");
            }
        }
    }
}
