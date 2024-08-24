namespace Shaderlens.Serialization.Text
{
    public interface ISourceLineAnnotation
    {
        SourceLine SourceLine { get; }
        string Type { get; }
        string? Value { get; }
        IReadOnlyDictionary<string, string> Properties { get; }
    }

    public class SourceLineAnnotation : ISourceLineAnnotation
    {
        public static readonly ISourceLineAnnotation Empty = new SourceLineAnnotation(default, String.Empty, null, Dictionary.Empty<string, string>());

        public SourceLine SourceLine { get; }
        public string Type { get; }
        public string? Value { get; }
        public IReadOnlyDictionary<string, string> Properties { get; }

        public SourceLineAnnotation(SourceLine sourceLine, string type, string? value, IReadOnlyDictionary<string, string> properties)
        {
            this.SourceLine = sourceLine;
            this.Type = type;
            this.Value = value;
            this.Properties = properties;
        }
    }
}
