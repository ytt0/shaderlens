namespace Shaderlens.Serialization.Text
{
    public interface ITransformContext
    {
        SourceLine SourceLine { get; }
        string? Line { get; set; }

        void PrependLine(SourceLine line);
    }

    public partial class TransformContext : ITransformContext
    {
        public SourceLine SourceLine { get; }
        public string? Line { get; set; }

        private readonly List<SourceLine> prependedLines;

        public TransformContext(SourceLine sourceLine)
        {
            this.SourceLine = sourceLine;
            this.Line = sourceLine.Line;

            this.prependedLines = new List<SourceLine>();
        }

        public void PrependLine(SourceLine line)
        {
            this.prependedLines.Add(line);
        }

        public void Commit(List<SourceLine> target)
        {
            var indentation = this.Line != null ? IndentationRegex().Match(this.Line).Value : null;

            foreach (var line in this.prependedLines)
            {
                var indentedLine = indentation != null ? new SourceLine(line.Source, indentation + line.Line.TrimStart(), line.Index) : line;
                target.Add(indentedLine);
            }

            if (this.Line != null)
            {
                target.Add(new SourceLine(this.SourceLine.Source, this.Line, this.SourceLine.Index));
            }
        }

        [GeneratedRegex("^\\s*")]
        private static partial Regex IndentationRegex();
    }
}
