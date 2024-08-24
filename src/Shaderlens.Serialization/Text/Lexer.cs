namespace Shaderlens.Serialization.Text
{
    public interface ILexer<T>
    {
        IEnumerable<Token<T>> GetTokens(string stream);
    }

    public class ParseException : Exception
    {
        public int Position { get; }
        public string Description { get; }

        public ParseException(int position, string description) :
            base($"{description} (index {position})")
        {
            this.Position = position;
            this.Description = description;
        }

        public override string ToString()
        {
            return this.Message;
        }
    }

    public partial class Lexer<T> : ILexer<T>
    {
        private readonly IEnumerable<ITokenParser<T>> tokenParsers;

        public Lexer(params ITokenParser<T>[] tokenParsers)
        {
            this.tokenParsers = tokenParsers;
        }

        public IEnumerable<Token<T>> GetTokens(string stream)
        {
            var whiteSpaceMatch = WhiteSpaceRegex().Match(stream);
            var start = whiteSpaceMatch.Success ? whiteSpaceMatch.Length : 0;

            while (start < stream.Length)
            {
                var longestToken = default(Token<T>);

                foreach (var tokenDefinition in this.tokenParsers)
                {
                    if (tokenDefinition.TryParse(stream, start, out var token))
                    {
                        if (longestToken.Value == null || longestToken.Value.Length < token.Value.Length)
                        {
                            longestToken = token;
                        }
                    }
                }

                if (longestToken.Value == null)
                {
                    throw new ParseException(start, $"Can't parse \"{stream}\", \"{stream[start]}\" is unexpected");
                }

                start += longestToken.Value.Length;

                yield return longestToken;

                whiteSpaceMatch = WhiteSpaceRegex().Match(stream.Substring(start));
                if (whiteSpaceMatch.Success)
                {
                    start += whiteSpaceMatch.Length;
                }
            }
        }

        [GeneratedRegex(@"^\s+")]
        private static partial Regex WhiteSpaceRegex();
    }
}
