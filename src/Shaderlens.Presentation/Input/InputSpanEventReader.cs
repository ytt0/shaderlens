namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanEventReader
    {
        bool TryRead(string value, [MaybeNullWhen(false)] out IInputSpanEvent inputSpanEvent);
    }

    public partial class InputSpanEventReader : IInputSpanEventReader
    {
        private readonly IInputSpanReader reader;

        public InputSpanEventReader(IInputSpanReader reader)
        {
            this.reader = reader;
        }

        public bool TryRead(string value, [MaybeNullWhen(false)] out IInputSpanEvent inputSpanEvent)
        {
            var isStartEvent = true;

            var match = EventTypeRegex().Match(value);
            if (match.Success)
            {
                isStartEvent = match.Groups["type"].Value == "Press";
                value = match.Groups["value"].Value;
            }

            if (reader.TryRead(value, out var inputSpan))
            {
                inputSpanEvent = isStartEvent ? new StartInputSpanEvent(inputSpan) : new EndInputSpanEvent(inputSpan);
                return true;
            }

            inputSpanEvent = null;
            return false;
        }

        [GeneratedRegex("^\\s*(?<type>Press|Release)\\s*\\((?<value>.*)\\)\\s*$")]
        private static partial Regex EventTypeRegex();
    }
}
