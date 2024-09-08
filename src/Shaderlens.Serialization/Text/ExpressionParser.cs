namespace Shaderlens.Serialization.Text
{
    public interface IExpressionParser
    {
        double Parse(string source);
    }

    public partial class ExpressionParser : IExpressionParser
    {
        private enum TokenType { Terminal, Identifier, Number }
        private enum Precedence { None, AddSubtract, MultiplyDivide, Exponent }

        private const double Phi = 1.61803398874989490253;

        public static readonly IExpressionParser Instance = new ExpressionParser();

        private static readonly Lazy<Lexer<TokenType>> Lexer = new Lazy<Lexer<TokenType>>(() => new Lexer<TokenType>
        (
            new TerminalTokenParser<TokenType>(TokenType.Terminal, "+-*/%^(),"),
            new RegexTokenParser<TokenType>(TokenType.Number, NumberRegex()),
            new RegexTokenParser<TokenType>(TokenType.Identifier, IdentifierRegex())
        ));

        private ExpressionParser()
        {
        }

        public double Parse(string source)
        {
            var tokens = new TokenStreamReader<TokenType>(Lexer.Value.GetTokens(source.ToLower()));

            var value = MatchExpression(tokens);
            tokens.ValidateEmpty();

            return value;
        }

        private static double MatchExpression(TokenStreamReader<TokenType> tokens, Precedence precedence = Precedence.None, bool allowUnary = true)
        {
            double value;

            if (allowUnary)
            {
                if (tokens.TryMatchToken(TokenType.Terminal, "+"))
                {
                    return MatchExpression(tokens, precedence, false);
                }

                if (tokens.TryMatchToken(TokenType.Terminal, "-"))
                {
                    return -MatchExpression(tokens, precedence, false);
                }
            }

            if (tokens.TryMatchToken(TokenType.Terminal, "("))
            {
                value = MatchExpression(tokens);
                tokens.MatchToken(TokenType.Terminal, ")");
            }
            else if (tokens.PeekToken(TokenType.Identifier))
            {
                value = MatchIdentifier(tokens);
            }
            else
            {
                var token = tokens.PopToken();
                if (token.Type != TokenType.Number)
                {
                    throw new ParseException(token.Start, $"\"{token.Value}\" is unexpected, a number is expected");
                }

                if (!Double.TryParse(token.Value, out value))
                {
                    throw new ParseException(token.Start, $"\"{token.Value}\" number format is incorrect");
                }
            }

            while (true)
            {
                if (precedence < Precedence.AddSubtract)
                {
                    if (tokens.TryMatchToken(TokenType.Terminal, "+"))
                    {
                        value += MatchExpression(tokens, Precedence.AddSubtract, false);
                        continue;
                    }

                    if (tokens.TryMatchToken(TokenType.Terminal, "-"))
                    {
                        value -= MatchExpression(tokens, Precedence.AddSubtract, false);
                        continue;
                    }
                }

                if (precedence < Precedence.MultiplyDivide)
                {
                    if (tokens.TryMatchToken(TokenType.Terminal, "*"))
                    {
                        value *= MatchExpression(tokens, Precedence.MultiplyDivide);
                        continue;
                    }

                    if (tokens.TryMatchToken(TokenType.Terminal, "/"))
                    {
                        value /= MatchExpression(tokens, Precedence.MultiplyDivide);
                        continue;
                    }

                    if (tokens.TryMatchToken(TokenType.Terminal, "%"))
                    {
                        value %= MatchExpression(tokens, Precedence.MultiplyDivide);
                        continue;
                    }
                }

                if (precedence < Precedence.Exponent)
                {
                    if (tokens.TryMatchToken(TokenType.Terminal, "^"))
                    {
                        value = Math.Pow(value, MatchExpression(tokens, Precedence.Exponent));
                        continue;
                    }
                }

                break;
            }

            return value;
        }

        private static double MatchIdentifier(TokenStreamReader<TokenType> tokens)
        {
            double value, value1, value2, value3;

            var token = tokens.PopToken();

            if (token.Type == TokenType.Identifier)
            {
                var name = token.Value.ToLowerInvariant();
                switch (name)
                {
                    case "e":
                        return Math.E;

                    case "phi":
                        return Phi;

                    case "pi":
                        return Math.PI;

                    case "tau":
                        return Math.Tau;

                    case "abs":
                        return Math.Abs(MatchParameter(tokens));

                    case "acos":
                        return Math.Acos(MatchParameter(tokens));

                    case "acosh":
                        return Math.Acosh(MatchParameter(tokens));

                    case "asin":
                        return Math.Asin(MatchParameter(tokens));

                    case "asinh":
                        return Math.Asinh(MatchParameter(tokens));

                    case "atan":
                        return Math.Atan(MatchParameter(tokens));

                    case "atan2":
                        MatchParameters(tokens, out value1, out value2);
                        return Math.Atan2(value1, value2);

                    case "atanh":
                        return Math.Atanh(MatchParameter(tokens));

                    case "cbrt":
                        return Math.Cbrt(MatchParameter(tokens));

                    case "ceil":
                        return Math.Ceiling(MatchParameter(tokens));

                    case "clamp":
                        MatchParameters(tokens, out value1, out value2, out value3);
                        return Math.Clamp(value1, value2, value3);

                    case "cos":
                        return Math.Cos(MatchParameter(tokens));

                    case "cosh":
                        return Math.Cosh(MatchParameter(tokens));

                    case "degrees":
                        return MatchParameter(tokens) * 180.0 / Math.PI;

                    case "exp":
                        return Math.Exp(MatchParameter(tokens));

                    case "exp2":
                        return Math.Pow(2.0, MatchParameter(tokens));

                    case "floor":
                        return Math.Floor(MatchParameter(tokens));

                    case "fract":
                        value = MatchParameter(tokens);
                        return value - Math.Floor(value);

                    case "inversesqrt":
                        return 1.0 / Math.Sqrt(MatchParameter(tokens));

                    case "log":
                        return Math.Log(MatchParameter(tokens));

                    case "log2":
                        return Math.Log2(MatchParameter(tokens));

                    case "log10":
                        return Math.Log10(MatchParameter(tokens));

                    case "round":
                        return Math.Round(MatchParameter(tokens));

                    case "max":
                        MatchParameters(tokens, out value1, out value2);
                        return Math.Max(value1, value2);

                    case "min":
                        MatchParameters(tokens, out value1, out value2);
                        return Math.Min(value1, value2);

                    case "mod":
                        MatchParameters(tokens, out value1, out value2);
                        return value1 % value2;

                    case "pow":
                        MatchParameters(tokens, out value1, out value2);
                        return Math.Pow(value1, value2);

                    case "radians":
                        return MatchParameter(tokens) * Math.PI / 180.0;

                    case "sign":
                        return Math.Sign(MatchParameter(tokens));

                    case "sin":
                        return Math.Sin(MatchParameter(tokens));

                    case "sinh":
                        return Math.Sinh(MatchParameter(tokens));

                    case "sqrt":
                        return Math.Sqrt(MatchParameter(tokens));

                    case "step":
                        MatchParameters(tokens, out value1, out value2);
                        return value1 > value2 ? 0.0 : 1.0;

                    case "tan":
                        return Math.Tan(MatchParameter(tokens));

                    case "tanh":
                        return Math.Tanh(MatchParameter(tokens));

                    case "trunc":
                        return Math.Truncate(MatchParameter(tokens));
                }
            }

            throw new ParseException(token.Start, $"\"{token.Value}\" is unexpected, a function or a constant name is expected");
        }

        private static double MatchParameter(TokenStreamReader<TokenType> tokens)
        {
            tokens.MatchToken(TokenType.Terminal, "(");
            var value = MatchExpression(tokens);
            tokens.MatchToken(TokenType.Terminal, ")");
            return value;
        }

        private static void MatchParameters(TokenStreamReader<TokenType> tokens, out double value1, out double value2)
        {
            tokens.MatchToken(TokenType.Terminal, "(");
            value1 = MatchExpression(tokens);
            tokens.MatchToken(TokenType.Terminal, ",");
            value2 = MatchExpression(tokens);
            tokens.MatchToken(TokenType.Terminal, ")");
        }

        private static void MatchParameters(TokenStreamReader<TokenType> tokens, out double value1, out double value2, out double value3)
        {
            tokens.MatchToken(TokenType.Terminal, "(");
            value1 = MatchExpression(tokens);
            tokens.MatchToken(TokenType.Terminal, ",");
            value2 = MatchExpression(tokens);
            tokens.MatchToken(TokenType.Terminal, ",");
            value3 = MatchExpression(tokens);
            tokens.MatchToken(TokenType.Terminal, ")");
        }

        private const string numberPattern = "(([0-9]+([.][0-9.]*)?)|([.][0-9.]+))";
        [GeneratedRegex($"^{numberPattern}(e[-+]?{numberPattern})?")]
        private static partial Regex NumberRegex();

        [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z_0-9]*")]
        private static partial Regex IdentifierRegex();
    }
}
