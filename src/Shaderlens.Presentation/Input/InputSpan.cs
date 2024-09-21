namespace Shaderlens.Presentation.Input
{
    public enum ModifierKey { Alt, Ctrl, Shift, Win }

    public enum MouseScroll { ScrollUp, ScrollDown }

    public interface IInputSpan
    {
        int Match(IInputState state);
        void WriteTo(IInputSpanWriter writer);
    }

    public static class InputSpan
    {
        private class NoneInputSpan : IInputSpan
        {
            public override string ToString()
            {
                return "None";
            }

            public int Match(IInputState state)
            {
                return 0;
            }

            public void WriteTo(IInputSpanWriter writer)
            {
            }
        }

        public static readonly IInputSpan None = new NoneInputSpan();
    }

    public static class InputSpanExtensions
    {
        public static IInputSpanEvent CreateStartEvent(this IInputSpan inputSpan)
        {
            return new StartInputSpanEvent(inputSpan);
        }

        public static IInputSpanEvent CreateEndEvent(this IInputSpan inputSpan)
        {
            return new EndInputSpanEvent(inputSpan);
        }

        public static IInputSpanEvent CreateGlobalEvent(this IInputSpan inputSpan)
        {
            return new GlobalInputSpanEvent(inputSpan);
        }
    }

    public class AllInputSpans : IInputSpan
    {
        private readonly IEnumerable<IInputSpan> inputSpans;

        public AllInputSpans(IEnumerable<IInputSpan> inputSpans)
        {
            this.inputSpans = inputSpans;
        }

        public override string ToString()
        {
            return $"All({String.Join(", ", this.inputSpans.Select(inputSpan => inputSpan.ToString()))})";
        }

        public int Match(IInputState state)
        {
            var matches = this.inputSpans.Select(inputSpan => inputSpan.Match(state)).ToArray();
            return matches.All(match => match > 0) ? matches.Sum() : 0;
        }

        public void WriteTo(IInputSpanWriter writer)
        {
            using (writer.WriteAll())
            {
                foreach (var inputSpan in this.inputSpans)
                {
                    inputSpan.WriteTo(writer);
                }
            }
        }
    }

    public class AnyInputSpan : IInputSpan
    {
        private readonly IEnumerable<IInputSpan> inputSpans;

        public AnyInputSpan(IEnumerable<IInputSpan> inputSpans)
        {
            this.inputSpans = inputSpans;
        }

        public override string ToString()
        {
            return $"Any({String.Join(", ", this.inputSpans.Select(inputSpan => inputSpan.ToString()))})";
        }

        public int Match(IInputState state)
        {
            return this.inputSpans.Max(inputSpan => inputSpan.Match(state));
        }

        public void WriteTo(IInputSpanWriter writer)
        {
            using (writer.WriteAny())
            {
                foreach (var inputSpan in this.inputSpans)
                {
                    inputSpan.WriteTo(writer);
                }
            }
        }
    }

    public class KeyInputSpan : IInputSpan
    {
        private readonly Key key;

        public KeyInputSpan(Key key)
        {
            this.key = key;
        }

        public override string ToString()
        {
            return this.key.ToString();
        }

        public int Match(IInputState state)
        {
            return state.IsKeyDown(this.key) ? 1 : 0;
        }

        public void WriteTo(IInputSpanWriter writer)
        {
            writer.WriteKey(this.key);
        }
    }

    public class ModifierKeyInputSpan : IInputSpan
    {
        private readonly ModifierKey modifierKey;

        public ModifierKeyInputSpan(ModifierKey modifierKey)
        {
            this.modifierKey = modifierKey;
        }

        public override string ToString()
        {
            return this.modifierKey.ToString();
        }

        public int Match(IInputState state)
        {
            return state.IsKeyDown(this.modifierKey) ? 1 : 0;
        }

        public void WriteTo(IInputSpanWriter writer)
        {
            writer.WriteModifierKey(this.modifierKey);
        }
    }

    public class MouseButtonInputSpan : IInputSpan
    {
        private readonly MouseButton button;

        public MouseButtonInputSpan(MouseButton button)
        {
            this.button = button;
        }

        public override string ToString()
        {
            return this.button.ToString();
        }

        public int Match(IInputState state)
        {
            return state.IsMouseButtonDown(this.button) ? 1 : 0;
        }

        public void WriteTo(IInputSpanWriter writer)
        {
            writer.WriteMouseButton(this.button);
        }
    }

    public class MouseScrollInputSpan : IInputSpan
    {
        private readonly MouseScroll direction;

        public MouseScrollInputSpan(MouseScroll direction)
        {
            this.direction = direction;
        }

        public override string ToString()
        {
            return this.direction.ToString();
        }

        public int Match(IInputState state)
        {
            return state.IsMouseScroll(this.direction) ? 1 : 0;
        }

        public void WriteTo(IInputSpanWriter writer)
        {
            writer.WriteMouseScroll(this.direction);
        }
    }

    public class InputSpanJsonSerializer : IJsonSerializer<IInputSpan>
    {
        private readonly IInputValueSerializer serializer;
        private readonly InputSpanReader reader;

        public InputSpanJsonSerializer(IInputValueSerializer serializer, IInputSpanFactory factory)
        {
            this.serializer = serializer;
            this.reader = new InputSpanReader(serializer, factory);
        }

        public JsonNode? Serialize(IInputSpan inputSpan)
        {
            if (inputSpan == InputSpan.None)
            {
                return null;
            }

            var writer = new InputSpanWriter(this.serializer);
            inputSpan.WriteTo(writer);
            var items = writer.GetResults().Select(result => JsonValue.Create(result)).ToArray();
            return items.Length == 1 ? items.First() : new JsonArray(items);
        }

        public IInputSpan Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return InputSpan.None;
            }

            if (source.GetValueKind() != JsonValueKind.Array)
            {
                return DeserializeValue(source);
            }

            var spans = source.AsArray().OfType<JsonNode>().Select(DeserializeValue).ToArray();

            return spans.Length == 0 ? InputSpan.None :
                spans.Length == 1 ? spans[0] :
                new AnyInputSpan(spans);
        }

        private IInputSpan DeserializeValue(JsonNode source)
        {
            if (source == null)
            {
                return InputSpan.None;
            }

            if (source.GetValueKind() != JsonValueKind.String)
            {
                throw new JsonSourceException($"Failed to deserialize input value of type {source.GetValueKind()}, a string value is expected", source);
            }

            var value = source.GetValue<string>().Trim();
            if (!this.reader.TryRead(value, out var span))
            {
                throw new JsonSourceException($"Failed to parse input \"{value}\"", source);
            }

            return span;
        }
    }
}
