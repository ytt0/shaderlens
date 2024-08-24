namespace Shaderlens.Extensions
{
    public static class StringExtensions
    {
        private static readonly string[] LineSeperator = new[] { "\r\n", "\r", "\n" };

        public static string[] SplitLines(this string text)
        {
            return text.Split(LineSeperator, StringSplitOptions.None);
        }

        public static string JoinLines(this IEnumerable<string> lines)
        {
            return String.Join(Environment.NewLine, lines);
        }

        public static string NormalizeDirectorySeparator(this string path)
        {
            return path.Replace('/', '\\');
        }

        public static void GetLinePosition(this string content, long position, out int lineIndex)
        {
            content.GetLinePosition(position, out lineIndex, out _);
        }

        public static void GetLinePosition(this string content, long position, out int lineIndex, out int columnIndex)
        {
            lineIndex = 0;
            columnIndex = 0;

            var i = 0;
            while (i < content.Length && i < position)
            {
                if (content[i] == '\r')
                {
                    if (i + 1 < content.Length && content[i + 1] == '\n')
                    {
                        i++;
                        continue;
                    }

                    lineIndex++;
                    columnIndex = 0;
                }
                else if (content[i] == '\n')
                {
                    lineIndex++;
                    columnIndex = 0;
                }
                else
                {
                    columnIndex++;
                }

                i++;
            }
        }
    }
}
