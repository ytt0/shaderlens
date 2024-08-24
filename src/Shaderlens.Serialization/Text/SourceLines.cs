namespace Shaderlens.Serialization.Text
{
    public class SourceLines : IEnumerable<SourceLine>
    {
        public static readonly SourceLines Empty = new SourceLines(Array.Empty<SourceLine>());

        public SourceLine this[int index] { get { return this.sourceLines[index]; } }

        public int Count { get { return this.sourceLines.Length; } }

        private readonly SourceLine[] sourceLines;

        public SourceLines(IFileResource<string> resource) :
            this(resource.Value.SplitLines().Select((line, index) => new SourceLine(resource, line, index)).ToArray())
        {
        }

        public SourceLines(IEnumerable<SourceLines> sourcesLines) :
            this(sourcesLines.SelectMany(sourceLines => sourceLines).ToArray())
        {
        }

        public SourceLines(IEnumerable<SourceLine> sourceLines)
        {
            this.sourceLines = sourceLines.ToArray();
        }

        public IEnumerator<SourceLine> GetEnumerator()
        {
            return this.sourceLines.Cast<SourceLine>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class SourceLinesExtensions
    {
        public static string JoinLines(this SourceLines sourceLines)
        {
            return sourceLines.Select(sourceLine => sourceLine.Line).JoinLines();
        }
    }
}
