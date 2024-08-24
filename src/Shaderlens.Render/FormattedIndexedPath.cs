namespace Shaderlens.Render
{
    public interface IFormattedIndexedPath
    {
        int StartIndex { get; }
        string SearchPattern { get; }

        string GetPath(int index);
    }

    public static class FormattedIndexedPathExtensions
    {
        public static string GetNextPath(this IFormattedIndexedPath path)
        {
            return path.GetPath(path.StartIndex + 1);
        }
    }

    public partial class FormattedIndexedPath : IFormattedIndexedPath
    {
        public int StartIndex { get; }
        public string SearchPattern { get; }

        private readonly string pathFormat;

        private FormattedIndexedPath(string pathFormat, int startIndex, string searchPattern)
        {
            this.pathFormat = pathFormat;
            this.StartIndex = startIndex;
            this.SearchPattern = searchPattern;
        }

        public string GetPath(int index)
        {
            return String.Format(this.pathFormat, index);
        }

        private static void ParsePath(string projectPath, string framePath, int defaultIndexLength, out string pathFormat, out int startIndex, out string searchPattern)
        {
            var directoryName = Path.GetDirectoryName(framePath) ?? Path.GetPathRoot(framePath)!;
            var projectFileName = Path.GetFileNameWithoutExtension(projectPath);
            var frameFileName = Path.GetFileName(framePath);
            var frameFileNamePrefix = String.Empty;

            if (frameFileName.StartsWith(projectFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                frameFileNamePrefix = frameFileName.Substring(0, projectFileName.Length);
                frameFileName = frameFileName.Substring(projectFileName.Length);
            }

            var match = NumberRegex().Match(frameFileName);
            if (match.Success)
            {
                var valueGroup = match.Groups["value"];
                startIndex = Int32.Parse(valueGroup.Value);

                var prefix = directoryName + "\\" + String.Concat(frameFileNamePrefix, frameFileName.AsSpan(0, valueGroup.Index));
                var suffix = frameFileName.Substring(valueGroup.Index + valueGroup.Length);

                pathFormat = $"{FormatEscape(prefix)}{{0:d{valueGroup.Length}}}{FormatEscape(suffix)}";
                searchPattern = prefix + "*" + suffix;
            }
            else
            {
                var fileName = Path.GetFileNameWithoutExtension(framePath);
                var prefix = directoryName + "\\" + fileName;
                var suffix = Path.GetExtension(framePath);

                startIndex = 0;

                pathFormat = $"{FormatEscape(prefix)}{{0:d{defaultIndexLength}}}{FormatEscape(suffix)}";
                searchPattern = prefix + "*" + suffix;
            }
        }

        public static IFormattedIndexedPath Create(string projectPath, string framePath, int startIndex, int count)
        {
            var defaultIndexLength = (int)Math.Ceiling(Math.Max(
                Math.Log10(Math.Abs(startIndex) + 1),
                Math.Log10(Math.Abs(startIndex + count - 1) + 1)));

            return Create(projectPath, framePath, defaultIndexLength);
        }

        public static IFormattedIndexedPath Create(string projectPath, string framePath, int defaultIndexLength)
        {
            ParsePath(projectPath, framePath, defaultIndexLength, out var pathFormat, out var startIndex, out var searchPattern);
            return new FormattedIndexedPath(pathFormat, startIndex, searchPattern);
        }

        private static string FormatEscape(string value)
        {
            return value.Replace("{", "{{").Replace("}", "}}");
        }

        [GeneratedRegex("(?<value>[0-9]+)[^0-9]*$")]
        private static partial Regex NumberRegex();
    }
}
