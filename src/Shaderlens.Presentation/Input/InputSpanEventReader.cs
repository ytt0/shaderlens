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
            string? type = null;

            var match = EventTypeRegex().Match(value);
            if (match.Success)
            {
                type = match.Groups["type"].Value;
                value = match.Groups["value"].Value;
            }

            if (reader.TryRead(value, out var inputSpan))
            {
                inputSpanEvent =
                    type == "Global" ? new GlobalInputSpanEvent(inputSpan) :
                    type == "Release" ? new EndInputSpanEvent(inputSpan) :
                    new StartInputSpanEvent(inputSpan);
                return true;
            }

            inputSpanEvent = null;
            return false;
        }

        [GeneratedRegex("^\\s*(?<type>Press|Release|Global)\\s*\\((?<value>.*)\\)\\s*$")]
        private static partial Regex EventTypeRegex();
    }
}
