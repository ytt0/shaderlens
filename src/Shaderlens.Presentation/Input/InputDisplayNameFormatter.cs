namespace Shaderlens.Presentation.Input
{
    public interface IInputDisplayNameFormatter
    {
        string? GetDisplayName(IInputSpanEvent? inputSpan);
    }

    public static class InputDisplayNameFormatterExtensions
    {
        public static string? GetDisplayName(this IInputDisplayNameFormatter formatter, IInputSpan? inputSpan)
        {
            return formatter.GetDisplayName(inputSpan?.CreateStartEvent());
        }
    }

    public class InputDisplayNameFormatter : IInputDisplayNameFormatter
    {
        private class Writer : IInputSpanEventWriter
        {
            private readonly InputSpanWriter writer;

            public Writer(IInputValueSerializer serializer)
            {
                this.writer = new InputSpanWriter(serializer);
            }

            public string? GetResult()
            {
                return this.writer.GetResults().FirstOrDefault();
            }

            public void WriteStartEvent(IInputSpan inputSpan)
            {
                inputSpan.WriteTo(this.writer);
            }

            public void WriteEndEvent(IInputSpan inputSpan)
            {
                inputSpan.WriteTo(this.writer);
            }

            public void WriteGlobalEvent(IInputSpan inputSpan)
            {
                inputSpan.WriteTo(this.writer);
            }
        }

        private readonly IInputValueSerializer serializer;

        public InputDisplayNameFormatter(IInputValueSerializer serializer)
        {
            this.serializer = serializer;
        }

        public string? GetDisplayName(IInputSpanEvent? inputSpan)
        {
            if (inputSpan == null)
            {
                return null;
            }

            var writer = new Writer(this.serializer);
            inputSpan.WriteTo(writer);
            return writer.GetResult();
        }
    }
}