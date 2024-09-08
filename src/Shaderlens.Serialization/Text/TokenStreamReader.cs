namespace Shaderlens.Serialization.Text
{
    public interface ITokenStreamReader<T>
    {
        int Position { get; }
        bool IsEmpty { get; }

        bool PeekToken(out Token<T> token);
        Token<T> PopToken();
    }

    public static class TokenStreamReaderExtensions
    {
        public static void ValidateEmpty<T>(this ITokenStreamReader<T> reader)
        {
            if (!reader.IsEmpty)
            {
                var token = reader.PopToken();
                throw new ParseException(token.Start, $"\"{token.Value}\" is unexpected");
            }
        }

        public static void ValidateNonEmpty<T>(this ITokenStreamReader<T> reader)
        {
            if (reader.IsEmpty)
            {
                throw new ParseException(reader.Position, $"stream was terminated unexpectedly");
            }
        }

        public static string MatchToken<T>(this ITokenStreamReader<T> reader, T tokenType)
        {
            reader.ValidateNonEmpty();

            var token = reader.PopToken();
            if (!Equals(token.Type, tokenType))
            {
                throw new ParseException(token.Start, $"\"{token.Value}\" is unexpected");
            }

            return token.Value;
        }

        public static void MatchToken<T>(this ITokenStreamReader<T> reader, T tokenType, string expectedValue)
        {
            var start = reader.Position;
            var value = reader.MatchToken(tokenType);

            if (value != expectedValue)
            {
                throw new ParseException(start, $"\"{expectedValue}\" is expected, \"{value}\" was found");
            }
        }

        public static bool TryMatchToken<T>(this ITokenStreamReader<T> reader, T tokenType, string expectedValue)
        {
            if (reader.PeekToken(tokenType, expectedValue))
            {
                reader.PopToken();
                return true;
            }

            return false;
        }

        public static bool TryMatchToken<T>(this ITokenStreamReader<T> reader, T tokenType, [MaybeNullWhen(false)] out string value)
        {
            if (reader.PeekToken(tokenType, out value))
            {
                reader.PopToken();
                return true;
            }

            value = default;
            return false;
        }

        public static bool PeekToken<T>(this ITokenStreamReader<T> reader, T tokenType)
        {
            return reader.PeekToken(out var token) && Equals(token.Type, tokenType);
        }

        public static bool PeekToken<T>(this ITokenStreamReader<T> reader, T tokenType, string expectedValue)
        {
            return reader.PeekToken(tokenType, out var value) && value == expectedValue;
        }

        public static bool PeekToken<T>(this ITokenStreamReader<T> reader, T tokenType, [MaybeNullWhen(false)] out string value)
        {
            if (reader.PeekToken(out var token) && Equals(token.Type, tokenType))
            {
                value = token.Value;
                return true;
            }

            value = default;
            return false;
        }
    }

    public class TokenStreamReader<T> : ITokenStreamReader<T>
    {
        public int Position { get; private set; }
        public bool IsEmpty { get; private set; }

        private readonly IEnumerator<Token<T>> tokens;

        public TokenStreamReader(IEnumerable<Token<T>> tokens)
        {
            this.tokens = tokens.GetEnumerator();
            this.IsEmpty = !this.tokens.MoveNext();
        }

        public bool PeekToken(out Token<T> token)
        {
            if (!this.IsEmpty)
            {
                token = this.tokens.Current;
                return true;
            }

            token = default;
            return false;
        }

        public Token<T> PopToken()
        {
            if (this.IsEmpty)
            {
                throw new ParseException(0, "Token stream is empty");
            }

            var token = this.tokens.Current;
            this.Position = token.Start + token.Value.Length;
            this.IsEmpty = !this.tokens.MoveNext();
            return token;
        }
    }
}
