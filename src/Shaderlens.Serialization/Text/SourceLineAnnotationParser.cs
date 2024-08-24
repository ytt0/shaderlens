namespace Shaderlens.Serialization.Text
{
    public interface ISourceLineAnnotationParser
    {
        ISourceLineAnnotation Parse(SourceLine sourceLine);
    }

    public partial class SourceLineAnnotationParser : ISourceLineAnnotationParser
    {
        private enum TokenType { Prefix, Terminal, Identifier, Number, String }

        private static readonly Lazy<Lexer<TokenType>> Lexer = new Lazy<Lexer<TokenType>>(() => new Lexer<TokenType>
        (
            new KeywordTokenParser<TokenType>(TokenType.Prefix, "//@"),
            new TerminalTokenParser<TokenType>(TokenType.Terminal, ":,()"),
            new RegexTokenParser<TokenType>(TokenType.Number, NumberRegex()),
            new RegexTokenParser<TokenType>(TokenType.Identifier, IdentifierRegex()),
            new RegexTokenParser<TokenType>(TokenType.String, StringRegex())
        ));

        private readonly bool allowLineEndTerminatedValue;

        public SourceLineAnnotationParser(bool allowLineEndTerminatedValue)
        {
            this.allowLineEndTerminatedValue = allowLineEndTerminatedValue;
        }

        // [identifier]':'([line-value]|[value])
        // [identifier](','[identifier]':'[value])*
        public ISourceLineAnnotation Parse(SourceLine sourceLine)
        {
            string? lineValue = null;
            var lineProperties = new Dictionary<string, string>();

            var tokens = new TokenStreamReader<TokenType>(Lexer.Value.GetTokens(sourceLine.Line));

            tokens.MatchToken(TokenType.Prefix);

            var lineType = tokens.MatchToken(TokenType.Identifier);
            if (tokens.PeekToken(TokenType.Terminal, ":"))
            {
                if (this.allowLineEndTerminatedValue)
                {
                    lineValue = sourceLine.Line.Substring(tokens.Position + 1).TrimStart();
                    return new SourceLineAnnotation(sourceLine, lineType, lineValue, lineProperties);
                }

                tokens.PopToken();
                lineValue = MatchValue(tokens);
            }

            while (!tokens.IsEmpty)
            {
                tokens.MatchToken(TokenType.Terminal, ",");
                var propertyName = tokens.MatchToken(TokenType.Identifier);
                tokens.MatchToken(TokenType.Terminal, ":");
                var propertyValue = MatchValue(tokens);

                lineProperties.Add(propertyName, propertyValue);
            }

            tokens.ValidateEmpty();

            return new SourceLineAnnotation(sourceLine, lineType, lineValue, lineProperties);
        }

        private static string MatchValue(ITokenStreamReader<TokenType> tokens)
        {
            if (tokens.PeekToken(TokenType.String))
            {
                var stringValue = tokens.MatchToken(TokenType.String);
                stringValue = stringValue.Substring(1, stringValue.Length - 2);
                stringValue = StringEscapeRegex().Replace(stringValue, match => GetEscapeSequence(match.Value[1]));
                return stringValue;
            }

            if (tokens.PeekToken(TokenType.Number))
            {
                var position = tokens.Position;
                var numberValue = tokens.MatchToken(TokenType.Number);
                return ValidateNumber(numberValue) ? numberValue : throw new ParseException(position, $"Can't parse number \"{numberValue}\"");
            }

            var identifier = tokens.MatchToken(TokenType.Identifier);
            if (!tokens.TryMatchToken(TokenType.Terminal, "("))
            {
                return identifier;
            }

            var value = tokens.MatchToken(TokenType.Number);
            while (tokens.TryMatchToken(TokenType.Terminal, ","))
            {
                value += "," + tokens.MatchToken(TokenType.Number);
            }

            tokens.MatchToken(TokenType.Terminal, ")");

            return $"{identifier}({value})";
        }

        private static string GetEscapeSequence(char code)
        {
            switch (code)
            {
                case 'r': return "\r";
                case 'n': return "\n";
                case '"': return "\"";
                case '\\': return "\\";
                default: return code.ToString();
            }
        }

        private static bool ValidateNumber(string value)
        {
            if (value.Contains('.'))
            {
                return Double.TryParse(value, out var _);
            }

            return Int32.TryParse(value, out var _);
        }

        [GeneratedRegex("\\\\.")]
        private static partial Regex StringEscapeRegex();

        [GeneratedRegex(@"^[+-]?(([0-9]+[.][0-9]*)|([.]?[0-9]+))")]
        private static partial Regex NumberRegex();

        [GeneratedRegex(@"^[a-zA-Z_][\w-]*")]
        private static partial Regex IdentifierRegex();

        [GeneratedRegex(@"^""(([^""\\]|\\""|\\\\|\\r|\\n)+)""")]
        private static partial Regex StringRegex();
    }
}
