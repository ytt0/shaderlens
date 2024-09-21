namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanEventWriter
    {
        void WriteStartEvent(IInputSpan inputSpan);
        void WriteEndEvent(IInputSpan inputSpan);
    }

    public class InputSpanEventWriter : IInputSpanEventWriter
    {
        private readonly IInputValueSerializer serializer;
        private readonly List<string> results;

        public InputSpanEventWriter(IInputValueSerializer serializer)
        {
            this.serializer = serializer;
            this.results = new List<string>();
        }

        public IEnumerable<string> GetResults()
        {
            return this.results;
        }

        public void WriteStartEvent(IInputSpan inputSpan)
        {
            var writer = new InputSpanWriter(serializer);
            inputSpan.WriteTo(writer);
            this.results.AddRange(writer.GetResults());
        }

        public void WriteEndEvent(IInputSpan inputSpan)
        {
            var writer = new InputSpanWriter(serializer);
            inputSpan.WriteTo(writer);
            this.results.AddRange(writer.GetResults().Select(result => $"Release({result})"));
        }
    }
}
