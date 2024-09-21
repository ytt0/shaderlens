namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanEvent
    {
        void AddTo(IInputStateBindings target, Action handler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat);
        void WriteTo(IInputSpanEventWriter writer);
    }

    public static class InputSpanEvent
    {
        private class NoneInputSpanEvent : IInputSpanEvent
        {
            public void AddTo(IInputStateBindings target, Action handler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat)
            {
            }

            public void WriteTo(IInputSpanEventWriter writer)
            {
            }
        }

        public static readonly IInputSpanEvent None = new NoneInputSpanEvent();
    }

    public class StartInputSpanEvent : IInputSpanEvent
    {
        private readonly IInputSpan inputSpan;

        public StartInputSpanEvent(IInputSpan inputSpan)
        {
            this.inputSpan = inputSpan;
        }

        public override string ToString()
        {
            return $"Press({this.inputSpan})";
        }

        public void AddTo(IInputStateBindings target, Action handler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat)
        {
            target.Add(this.inputSpan, handler, null, isEnabled, requireSpanStart, allowRepeat);
        }

        public bool MatchStart(IInputState previousState, IInputState state, out int spanMatch, out bool eventMatch)
        {
            var previousMatch = this.inputSpan.Match(previousState);
            var match = this.inputSpan.Match(state);

            if (previousMatch == 0 && match > 0)
            {
                spanMatch = match;
                eventMatch = true;
                return true;
            }

            spanMatch = 0;
            eventMatch = false;
            return false;
        }

        public bool MatchEnd(IInputState previousState, IInputState state, out bool eventMatch)
        {
            var previousMatch = this.inputSpan.Match(previousState);
            var match = this.inputSpan.Match(state);

            if (previousMatch > 0 && match == 0)
            {
                eventMatch = false;
                return true;
            }

            eventMatch = false;
            return false;
        }

        public void WriteTo(IInputSpanEventWriter writer)
        {
            writer.WriteStartEvent(this.inputSpan);
        }
    }

    public class EndInputSpanEvent : IInputSpanEvent
    {
        private readonly IInputSpan inputSpan;

        public EndInputSpanEvent(IInputSpan inputSpan)
        {
            this.inputSpan = inputSpan;
        }

        public override string ToString()
        {
            return $"Release({this.inputSpan})";
        }

        public void AddTo(IInputStateBindings target, Action handler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat)
        {
            target.Add(this.inputSpan, null, handler, isEnabled, requireSpanStart, allowRepeat);
        }

        public void WriteTo(IInputSpanEventWriter writer)
        {
            writer.WriteEndEvent(this.inputSpan);
        }
    }

    public class GlobalInputSpanEvent : IInputSpanEvent
    {
        private readonly IInputSpan inputSpan;

        public GlobalInputSpanEvent(IInputSpan inputSpan)
        {
            this.inputSpan = inputSpan;
        }

        public override string ToString()
        {
            return $"Global({this.inputSpan})";
        }

        public void AddTo(IInputStateBindings target, Action handler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat)
        {
            target.AddGlobal(this.inputSpan, handler, isEnabled, allowRepeat);
        }

        public void WriteTo(IInputSpanEventWriter writer)
        {
            writer.WriteGlobalEvent(this.inputSpan);
        }
    }

    public class AnyInputSpanEvent : IInputSpanEvent
    {
        private readonly IEnumerable<IInputSpanEvent> inputSpanEvents;

        public AnyInputSpanEvent(IEnumerable<IInputSpanEvent> inputSpanEvents)
        {
            this.inputSpanEvents = inputSpanEvents;
        }

        public void AddTo(IInputStateBindings target, Action handler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat)
        {
            foreach (var inputSpanEvent in this.inputSpanEvents)
            {
                inputSpanEvent.AddTo(target, handler, isEnabled, requireSpanStart, allowRepeat);
            }
        }

        public void WriteTo(IInputSpanEventWriter writer)
        {
            foreach (var inputSpanEvent in this.inputSpanEvents)
            {
                inputSpanEvent.WriteTo(writer);
            }
        }
    }

    public class InputSpanEventJsonSerializer : IJsonSerializer<IInputSpanEvent>
    {
        private readonly IInputValueSerializer serializer;
        private readonly InputSpanEventReader reader;

        public InputSpanEventJsonSerializer(IInputValueSerializer serializer, IInputSpanFactory factory)
        {
            this.serializer = serializer;
            this.reader = new InputSpanEventReader(new InputSpanReader(serializer, factory));
        }

        public JsonNode? Serialize(IInputSpanEvent inputSpanEvent)
        {
            if (inputSpanEvent == InputSpanEvent.None)
            {
                return null;
            }

            var writer = new InputSpanEventWriter(this.serializer);
            inputSpanEvent.WriteTo(writer);
            var items = writer.GetResults().Select(result => JsonValue.Create(result)).ToArray();
            return items.Length == 1 ? items.First() : new JsonArray(items);
        }

        public IInputSpanEvent Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return InputSpanEvent.None;
            }

            if (source.GetValueKind() != JsonValueKind.Array)
            {
                return DeserializeValue(source);
            }

            var spans = source.AsArray().OfType<JsonNode>().Select(DeserializeValue).ToArray();

            return spans.Length == 0 ? InputSpanEvent.None :
                spans.Length == 1 ? spans[0] :
                new AnyInputSpanEvent(spans);
        }

        private IInputSpanEvent DeserializeValue(JsonNode source)
        {
            if (source == null)
            {
                return InputSpanEvent.None;
            }

            if (source.GetValueKind() != JsonValueKind.String)
            {
                throw new JsonSourceException($"Failed to deserialize input event value of type {source.GetValueKind()}, a string value is expected", source);
            }

            var value = source.GetValue<string>().Trim();
            if (!this.reader.TryRead(value, out var span))
            {
                throw new JsonSourceException($"Failed to parse input event \"{value}\"", source);
            }

            return span;
        }
    }
}
