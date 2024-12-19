namespace Shaderlens.Render
{
    public interface IShaderStatusParser
    {
        void LogCompilationErrors(IProjectLoadLogger logger, IProgramStageSource stageSource, string status);
        void LogLinkingErrors(IProjectLoadLogger logger, IProgramSource programSource, string status);
    }

    public partial class ShaderStatusParser : IShaderStatusParser
    {
        public static readonly IShaderStatusParser Instance = new ShaderStatusParser();

        private static readonly IEnumerable<Regex> StatusLineRegexes = new[] { StatusLineRegex1(), StatusLineRegex2(), StatusLineRegex3() };
        private static readonly IEnumerable<Regex> MessageLineIndexRegexes = new[] { MessageLineIndexRegex1() };

        private ShaderStatusParser()
        {
        }

        public void LogCompilationErrors(IProjectLoadLogger logger, IProgramStageSource stageSource, string status)
        {
            foreach (var line in status.Trim().SplitLines())
            {
                if (TryParseStatusLine(line, stageSource.SourceLines, out var message, out var lineIndex))
                {
                    logger.AddFailure(message, stageSource.SourceLines[lineIndex]);
                }
                else
                {
                    logger.AddFailure(line);
                }
            }
        }

        public void LogLinkingErrors(IProjectLoadLogger logger, IProgramSource programSource, string status)
        {
            SourceLines? sourceLines = null;

            foreach (var line in status.Trim().SplitLines())
            {
                if (String.IsNullOrWhiteSpace(line.Replace("-", "")))
                {
                    continue;
                }

                if (line == "Vertex info")
                {
                    sourceLines = programSource.Vertex?.SourceLines;
                    continue;
                }

                if (line == "Fragment info")
                {
                    sourceLines = programSource.Fragment?.SourceLines;
                    continue;
                }

                if (line == "Compute info")
                {
                    sourceLines = programSource.Compute?.SourceLines;
                    continue;
                }

                if (sourceLines != null && TryParseStatusLine(line, sourceLines, out var message, out var lineIndex))
                {
                    logger.AddFailure(message, sourceLines[lineIndex]);
                }
                else
                {
                    logger.AddFailure(line);
                }
            }
        }

        private bool TryParseStatusLine(string line, SourceLines? sourceLines, [MaybeNullWhen(false)] out string message, out int lineIndex)
        {
            foreach (var regex in StatusLineRegexes)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    lineIndex = Int32.Parse(match.Groups["line"].Value) - 1;
                    message = match.Groups["message"].Value;

                    var messageSourcePath = sourceLines?.ElementAtOrDefault(lineIndex).Source;
                    if (sourceLines != null && messageSourcePath != null)
                    {
                        message = ReplaceMessageLineIndex(messageSourcePath.Value, sourceLines, message);
                    }

                    return true;
                }
            }

            message = null;
            lineIndex = 0;
            return false;
        }

        private string ReplaceMessageLineIndex(string messageSourceLocation, SourceLines sourceLines, string message)
        {
            if (sourceLines != null)
            {
                foreach (var regex in MessageLineIndexRegexes)
                {
                    message = regex.Replace(message, match =>
                    {
                        var lineIndex = Int32.Parse(match.Groups["line"].Value) - 1;
                        var sourceLine = sourceLines.ElementAtOrDefault(lineIndex);
                        return sourceLine.Line == null ? $"at composed line {lineIndex}" :
                            Equals(messageSourceLocation) ? $"at line {sourceLine.Index + 1}" :
                            $"at {sourceLine.Source}:line {sourceLine.Index + 1}";
                    });
                }
            }

            return message;
        }

        // 0(123) : error C1503: undefined variable "MAX_STEPS"
        [GeneratedRegex("^\\s*(?<file>[0-9]+)\\((?<line>[0-9]+)\\)\\s*:\\s*(?<message>.+)$")]
        private static partial Regex StatusLineRegex1();

        // line 123, column 456: error: undefined variable "MAX_STEPS"
        [GeneratedRegex("^\\s*line (?<line>[0-9]+)(,\\s*column [0-9]+)?\\s*:\\s*(?<message>.+)$")]
        private static partial Regex StatusLineRegex2();

        // ERROR: 0:123: error(#143) Undeclared identifier MAX_STEPS
        [GeneratedRegex("^\\s*(ERROR:\\s*)?(?<file>[0-9]+)\\s*:\\s*(?<line>[0-9]+)\\s*:\\s*(?<message>.+)$")]
        private static partial Regex StatusLineRegex3();

        // at 0(123)
        [GeneratedRegex("at \\s*(?<file>[0-9]+)\\((?<line>[0-9]+)\\)")]
        private static partial Regex MessageLineIndexRegex1();
    }
}
