namespace Shaderlens.Serialization.Text
{
    public interface ITokenParser<T>
    {
        bool TryParse(string stream, int start, out Token<T> token);
    }

    public class RegexTokenParser<T> : ITokenParser<T>
    {
        private readonly T tokenType;
        private readonly Regex regex;

        public RegexTokenParser(T tokenType, Regex regex)
        {
            this.tokenType = tokenType;
            this.regex = regex;
        }

        public bool TryParse(string stream, int start, out Token<T> token)
        {
            var match = this.regex.Match(stream.Substring(start));
            if (match.Success && match.Index == 0)
            {
                token = new Token<T>(this.tokenType, match.Value, start);
                return true;
            }

            token = default;
            return false;
        }
    }

    public class TerminalTokenParser<T> : ITokenParser<T>
    {
        private readonly T tokenType;
        private readonly IEnumerable<char> terminals;

        public TerminalTokenParser(T tokenType, IEnumerable<char> terminals)
        {
            this.tokenType = tokenType;
            this.terminals = terminals;
        }

        public bool TryParse(string stream, int start, out Token<T> token)
        {
            if (this.terminals.Contains(stream[start]))
            {
                token = new Token<T>(this.tokenType, stream[start].ToString(), start);
                return true;
            }

            token = default;
            return false;
        }
    }

    public class KeywordTokenParser<T> : ITokenParser<T>
    {
        private readonly T tokenType;
        private readonly string keyword;

        public KeywordTokenParser(T tokenType, string keyword)
        {
            this.tokenType = tokenType;
            this.keyword = keyword;
        }

        public bool TryParse(string stream, int start, out Token<T> token)
        {
            if (start + this.keyword.Length <= stream.Length)
            {
                for (var i = 0; i < this.keyword.Length; i++)
                {
                    if (this.keyword[i] != stream[start + i])
                    {
                        token = default;
                        return false;
                    }
                }

                token = new Token<T>(this.tokenType, this.keyword, start);
                return true;
            }

            token = default;
            return false;
        }
    }
}
