
namespace Shaderlens.Serialization.Text
{
    public interface ISourceLineAnnotationReader
    {
        SourceLine SourceLine { get; }

        bool TryGetValue([MaybeNullWhen(false)] out string value);
        bool TryGetValue(string key, [MaybeNullWhen(false)] out string value);

        void ValidateEmpty();
    }

    public class SourceLineAnnotationReader : ISourceLineAnnotationReader
    {
        public static readonly ISourceLineAnnotationReader Empty = new SourceLineAnnotationReader(SourceLineAnnotation.Empty);

        public SourceLine SourceLine { get { return this.annotation.SourceLine; } }

        private readonly ISourceLineAnnotation annotation;
        private readonly HashSet<string> readKeys;
        private bool readValue;

        public SourceLineAnnotationReader(ISourceLineAnnotation annotation)
        {
            this.annotation = annotation;
            this.readKeys = new HashSet<string>();
        }

        public bool TryGetValue([MaybeNullWhen(false)] out string value)
        {
            this.readValue = true;
            value = this.annotation.Value;
            return value != null;
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            this.readKeys.Add(key);
            return this.annotation.Properties.TryGetValue(key, out value);
        }

        public void ValidateEmpty()
        {
            if (!this.readValue && this.annotation.Value != null)
            {
                throw new SourceLineException($"Unexpected annotation value \"{this.annotation.Value}\"", this.annotation.SourceLine);
            }

            var key = this.annotation.Properties.Keys.Except(this.readKeys).FirstOrDefault();
            if (key != null)
            {
                throw new SourceLineException($"Unexpected annotation property \"{key}\" (\"{this.annotation.Properties[key]}\"), expected properties are: \"{String.Join("\", \"", this.readKeys)}\"", this.annotation.SourceLine);
            }
        }
    }
}
