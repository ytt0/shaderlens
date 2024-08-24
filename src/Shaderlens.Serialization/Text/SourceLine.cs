namespace Shaderlens.Serialization.Text
{
    public readonly struct SourceLine
    {
        public readonly IFileResource<string> Source;
        public readonly string Line;
        public readonly int Index;

        public SourceLine(IFileResource<string> source, string line, int index)
        {
            this.Source = source;
            this.Line = line;
            this.Index = index;
        }

        public override string ToString()
        {
            return $"{this.Source.Key} ({this.Index}): {this.Line}";
        }
    }

    public class SourceLineException : Exception
    {
        public SourceLine SourceLine { get; }

        public SourceLineException(string message, SourceLine sourceLine, Exception? innerException = null) :
            base(message, innerException)
        {
            this.SourceLine = sourceLine;
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
}
