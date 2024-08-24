namespace Shaderlens.Serialization.Text
{
    public interface ILineTransformer
    {
        bool IsGlobal { get; }
        void Transform(ITransformContext context);
    }

    public partial class ReplaceLineTransformer : ILineTransformer
    {
        public bool IsGlobal { get { return false; } }

        private readonly string targetLine;

        public ReplaceLineTransformer(string targetLine)
        {
            this.targetLine = targetLine;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line != null)
            {
                var indentation = IndentationRegex().Match(context.Line).Value;
                context.Line = indentation + this.targetLine.TrimStart();
            }
        }

        [GeneratedRegex("^\\s*")]
        private static partial Regex IndentationRegex();
    }

    public partial class InsertLineTransformer : ILineTransformer
    {
        public bool IsGlobal { get { return false; } }

        private readonly SourceLine sourceLine;

        public InsertLineTransformer(SourceLine sourceLine)
        {
            this.sourceLine = sourceLine;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line == null)
            {
                return;
            }

            context.PrependLine(this.sourceLine);
        }
    }

    public class RemoveLineTransformer : ILineTransformer
    {
        public static readonly ILineTransformer Instance = new RemoveLineTransformer();

        public bool IsGlobal { get { return false; } }

        private RemoveLineTransformer()
        {
        }

        public void Transform(ITransformContext context)
        {
            context.Line = null;
        }
    }

    public class RemoveLineRegexTransformer : ILineTransformer
    {
        public bool IsGlobal { get; }

        private readonly Regex regex;

        public RemoveLineRegexTransformer(string searchPattern, bool isGlobal)
        {
            this.regex = new Regex($"^{searchPattern.TrimStart('^').TrimEnd('$')}$");
            this.IsGlobal = isGlobal;
        }

        public void Transform(ITransformContext context)
        {
            context.Line = context.Line == null || this.regex.IsMatch(context.Line) ? null : context.Line;
        }
    }

    public class ReplaceTransformer : ILineTransformer
    {
        public bool IsGlobal { get; }

        private readonly string source;
        private readonly string target;

        public ReplaceTransformer(string source, string target, bool isGlobal)
        {
            this.source = source;
            this.target = target;
            this.IsGlobal = isGlobal;
        }

        public void Transform(ITransformContext context)
        {
            context.Line = context.Line?.Replace(this.source, this.target);
        }
    }

    public class RemoveTransformer : ILineTransformer
    {
        public bool IsGlobal { get; }

        private readonly string value;

        public RemoveTransformer(string value, bool isGlobal)
        {
            this.value = value;
            this.IsGlobal = isGlobal;
        }

        public void Transform(ITransformContext context)
        {
            context.Line = context.Line?.Replace(this.value, String.Empty);
        }
    }

    public class ReplaceWordTransformer : ILineTransformer
    {
        public bool IsGlobal { get; }

        private readonly Regex replaceRegex;
        private readonly string target;

        public ReplaceWordTransformer(string source, string target, bool isGlobal)
        {
            this.replaceRegex = new Regex($"\\b{Regex.Escape(source)}\\b");
            this.target = target;
            this.IsGlobal = isGlobal;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line != null)
            {
                context.Line = this.replaceRegex.Replace(context.Line, match => this.target);
            }
        }
    }

    public class RemoveWordTransformer : ILineTransformer
    {
        public bool IsGlobal { get; }

        private readonly Regex removeRegex;

        public RemoveWordTransformer(string value, bool isGlobal)
        {
            this.removeRegex = new Regex("(?<prefix>\\s+|\\b)" + Regex.Escape(value) + "(?<suffix>\\s+|\\b)");
            this.IsGlobal = isGlobal;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line == null)
            {
                return;
            }

            context.Line = this.removeRegex.Replace(context.Line, match =>
            {
                var prefix = match.Groups["prefix"].Value;
                var suffix = match.Groups["suffix"].Value;

                if (suffix.Length > 0)
                {
                    suffix = String.Empty;
                }
                else if (prefix.Length > 0)
                {
                    prefix = String.Empty;
                }

                return prefix + suffix;
            });
        }
    }

    public partial class ReplaceRegexTransformer : ILineTransformer
    {
        public bool IsGlobal { get; }

        private readonly Regex regex;
        private readonly string replacePattern;

        public ReplaceRegexTransformer(string searchPattern, string replacePattern, bool isGlobal)
        {
            this.regex = new Regex(searchPattern);
            this.replacePattern = replacePattern;
            this.IsGlobal = isGlobal;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line == null)
            {
                return;
            }

            var matches = this.regex.Matches(context.Line);
            var stringBuilder = new StringBuilder();

            var lastIndex = 0;

            foreach (var lineMatch in matches.Cast<Match>())
            {
                stringBuilder.Append(context.Line.AsSpan(lastIndex, lineMatch.Index - lastIndex));

                stringBuilder.Append(ReplaceGroupRegex().Replace(this.replacePattern, groupMatch =>
                {
                    var groupName = groupMatch.Groups["name"].Value;
                    return Int32.TryParse(groupName, out var groupIndex) ? lineMatch.Groups[groupIndex].Value : lineMatch.Groups[groupName].Value;
                }));

                lastIndex = lineMatch.Index + lineMatch.Length;
            }

            stringBuilder.Append(context.Line.AsSpan(lastIndex, context.Line.Length - lastIndex));

            context.Line = stringBuilder.ToString();
        }

        [GeneratedRegex("\\$\\(?(?<name>\\w+)\\)?")]
        private static partial Regex ReplaceGroupRegex();
    }

    public partial class UniformTransformer : ILineTransformer
    {
        public bool IsGlobal { get { return false; } }

        private readonly ISourceLineAnnotation annotation;

        public UniformTransformer(ISourceLineAnnotation annotation)
        {
            this.annotation = annotation;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line == null)
            {
                return;
            }

            var match = AssignmentRegex().Match(context.Line);

            if (!match.Success)
            {
                return;
            }

            context.PrependLine(this.annotation.SourceLine);
            context.Line = $"{match.Groups["indentation"]}uniform {match.Groups["type"]} {match.Groups["name"]}{match.Groups["value"]};";
        }

        [GeneratedRegex("^(?<indentation>\\s*).*\\b(?<type>\\w+)\\s+(?<name>\\w+)(?<value>\\s*=\\s*.*)?\\s*;")]
        private static partial Regex AssignmentRegex();
    }

    public partial class DefineTransformer : ILineTransformer
    {
        public static readonly ILineTransformer Instance = new DefineTransformer();

        public bool IsGlobal { get { return false; } }

        private DefineTransformer()
        {
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line != null)
            {
                var match = AssignmentRegex().Match(context.Line);
                if (match.Success)
                {
                    context.Line = $"#define {match.Groups["name"].Value} {match.Groups["value"].Value}";
                    return;
                }

                match = LambdaMethodRegex().Match(context.Line);
                if (match.Success)
                {
                    var paramNames = match.Groups["param"].Captures.Cast<Capture>().Select(capture => capture.Value).ToArray();
                    context.Line = $"#define {match.Groups["name"].Value}({String.Join(", ", paramNames)}) {match.Groups["body"].Value}";
                    return;
                }

                throw new Exception($"Line {context.Line} cannot be transformed to a #define directive, a variable declaration or a lambda method (\"expression bodied function\") is expected.");
            }
        }

        [GeneratedRegex(".*\\b(?<name>\\w+)\\s*=\\s*(?<value>.*);")]
        private static partial Regex AssignmentRegex();

        [GeneratedRegex(".*\\b(?<name>\\w+)\\s*\\(\\s*(\\w+\\s+(?<param>\\w+)\\s*,?\\s*)*\\)\\s*=>\\s*(?<body>.*);")]
        private static partial Regex LambdaMethodRegex();
    }

    public class AddPrefixTransformer : ILineTransformer
    {
        public bool IsGlobal { get { return false; } }

        private readonly string prefix;

        public AddPrefixTransformer(string prefix)
        {
            this.prefix = prefix;
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line != null)
            {
                var trimmedLine = context.Line.TrimStart();
                var indentation = context.Line.Substring(0, context.Line.Length - trimmedLine.Length);
                context.Line = $"{indentation}{this.prefix} {trimmedLine}";
            }
        }
    }

    public partial class ScopeTransformer : ILineTransformer
    {
        public bool IsGlobal { get { return true; } }

        private readonly Regex scopeRegex;
        private string? scopeIndentation;

        public ScopeTransformer(string keyword)
        {
            this.scopeRegex = new Regex("\\b" + Regex.Escape(keyword) + "\\b[^;]*;?");
        }

        public void Transform(ITransformContext context)
        {
            if (context.Line == null)
            {
                return;
            }

            var lineIndentation = IndentationRegex().Match(context.Line).Value;

            if (this.scopeIndentation != null)
            {
                if (context.Line == this.scopeIndentation + "{")
                {
                    context.Line = null;
                    return;
                }

                if (context.Line == this.scopeIndentation + "}")
                {
                    this.scopeIndentation = null;
                    context.Line = null;
                    return;
                }

                if (context.Line.StartsWith(this.scopeIndentation))
                {
                    context.Line = this.scopeIndentation + context.Line.Substring(lineIndentation.Length);
                    return;
                }
            }
            else
            {
                var match = this.scopeRegex.Match(context.Line);
                if (match.Success)
                {
                    if (!match.Value.EndsWith(";"))
                    {
                        this.scopeIndentation = lineIndentation;
                    }

                    context.Line = null;
                }
            }
        }

        [GeneratedRegex("^\\s*")]
        private static partial Regex IndentationRegex();
    }

    public partial class ArrayTransformer : ILineTransformer
    {
        public bool IsGlobal { get { return true; } }

        private bool isMatched;

        public void Transform(ITransformContext context)
        {
            if (context.Line == null)
            {
                return;
            }

            var replaceStart = 0;
            var line = context.Line;

            if (!this.isMatched)
            {
                line = ArrayRegex().Replace(line, match =>
                {
                    this.isMatched = true;
                    replaceStart = match.Index;

                    var type = match.Groups["type"].Value;
                    var count = match.Groups["count"].Value;
                    return $"{type}[{count}]";
                });
            }

            if (this.isMatched)
            {
                var replaceEnd = line.IndexOf('}');
                var commentIndex = line.IndexOf("//");
                if (commentIndex != -1 && replaceEnd > commentIndex)
                {
                    replaceEnd = -1;
                }

                this.isMatched = replaceEnd == -1;

                if (replaceEnd < commentIndex)
                {
                    replaceEnd = commentIndex;
                }
                else if (replaceEnd != -1)
                {
                    replaceEnd++;
                }

                var linePrefix = line.Substring(0, replaceStart);
                var lineSuffix = replaceStart < replaceEnd ? line.Substring(Math.Max(commentIndex, replaceEnd)) : String.Empty;

                line = replaceStart < replaceEnd ? line.Substring(replaceStart, replaceEnd - replaceStart) : line.Substring(replaceStart);
                line = line.Replace('{', '(').Replace('}', ')');

                context.Line = linePrefix + line + lineSuffix;
            }
        }

        [GeneratedRegex("\\bnew\\s+(?<type>[\\w]+)\\[(?<count>[0-9]*)\\]")]
        private static partial Regex ArrayRegex();
    }
}
