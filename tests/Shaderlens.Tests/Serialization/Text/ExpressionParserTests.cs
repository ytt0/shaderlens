namespace Shaderlens.Tests.Serialization.Text
{
    [TestClass]
    public class ExpressionParserTests
    {
        [TestMethod]
        [DataRow(" 1 ", 1, DisplayName = "Expression With White Space")]
        [DataRow("+1", 1, DisplayName = "Positive Number")]
        [DataRow("-1", -1, DisplayName = "Negative Number")]
        [DataRow("1.23", 1.23, DisplayName = "Decimal Number")]
        [DataRow("1e2", 1e2, DisplayName = "Exponent Number")]
        [DataRow("1+2", 3, DisplayName = "Add")]
        [DataRow("1-2", -1, DisplayName = "Subtract")]
        [DataRow("2*3", 6, DisplayName = "Multiply 1")]
        [DataRow("2*-3", -6, DisplayName = "Multiply 2")]
        [DataRow("4/2", 2, DisplayName = "Divide")]
        [DataRow("4%3", 1, DisplayName = "Modulo")]
        [DataRow("2^3", 8, DisplayName = "Power 1")]
        [DataRow("2^-3", 0.125, DisplayName = "Power 2")]
        [DataRow("1-2+3", 2, DisplayName = "Addition Subtraction Order")]
        [DataRow("1+3*2", 7, DisplayName = "Multiplication Precedence 1")]
        [DataRow("3*2+1", 7, DisplayName = "Multiplication Precedence 2")]
        [DataRow("1+4/2", 3, DisplayName = "Division Precedence 1")]
        [DataRow("4/2+1", 3, DisplayName = "Division Precedence 2")]
        [DataRow("4/2*3", 6, DisplayName = "Division Precedence 3")]
        [DataRow("(1+3)*2", 8, DisplayName = "Parenthesis Precedence 1")]
        [DataRow("2*(1+3)", 8, DisplayName = "Parenthesis Precedence 2")]
        [DataRow("3^(1+2)", 27, DisplayName = "Parenthesis Precedence 3")]
        [DataRow("(1+2)^4", 81, DisplayName = "Parenthesis Precedence 4")]
        [DataRow("1+3^2", 10, DisplayName = "Exponentiation Precedence 1")]
        [DataRow("3^2+1", 10, DisplayName = "Exponentiation Precedence 2")]
        [DataRow("2^3*4", 32, DisplayName = "Exponentiation Precedence 3")]
        [DataRow("4^3^2", 4096, DisplayName = "Exponentiation Precedence 4")]
        [DataRow("2^3*4^5", 8192, DisplayName = "Exponentiation Precedence 5")]
        [DataRow("PI", Math.PI, DisplayName = "Uppercase Value")]
        [DataRow("EXP(1)", Math.E, DisplayName = "Uppercase Function")]
        [DataRow("e", Math.E, DisplayName = "E Value")]
        [DataRow("phi", 1.618034, DisplayName = "Phi Value")]
        [DataRow("pi", Math.PI, DisplayName = "PI Value")]
        [DataRow("tau", Math.Tau, DisplayName = "Tau Value")]
        [DataRow("abs(-1)", 1.0, DisplayName = "Abs Function")]
        [DataRow("acos(-1)", Math.PI, DisplayName = "Acos Function")]
        [DataRow("acosh((e+1/e)/2)", 1, DisplayName = "Acosh Function")]
        [DataRow("asin(1)", Math.PI / 2, DisplayName = "Asin Function")]
        [DataRow("asinh((e-1/e)/2)", 1, DisplayName = "Asinh Function")]
        [DataRow("atan(1)", Math.PI / 4, DisplayName = "Atan Function")]
        [DataRow("atan2(1,1)", 0.25 * Math.PI, DisplayName = "Atan2 Function 1")]
        [DataRow("atan2(-1,1)", -0.25 * Math.PI, DisplayName = "Atan2 Function 2")]
        [DataRow("atan2(1,-1)", 0.75 * Math.PI, DisplayName = "Atan2 Function 3")]
        [DataRow("atan2(-1,-1)", -0.75 * Math.PI, DisplayName = "Atan2 Function 4")]
        [DataRow("atanh((e-1)/(e+1))", 0.5, DisplayName = "Atanh Function")]
        [DataRow("cbrt(8)", 2, DisplayName = "Cbrt Function")]
        [DataRow("ceil(1.2)", 2, DisplayName = "Ceil Function")]
        [DataRow("clamp(1,2,4)", 2, DisplayName = "Clamp Function 1")]
        [DataRow("clamp(3,2,4)", 3, DisplayName = "Clamp Function 2")]
        [DataRow("clamp(5,2,4)", 4, DisplayName = "Clamp Function 3")]
        [DataRow("cos(pi)", -1, DisplayName = "Cos Function")]
        [DataRow("cosh(1)", 1.543081, DisplayName = "Cosh Function")]
        [DataRow("degrees(pi)", 180, DisplayName = "Degrees Function")]
        [DataRow("exp(1)", Math.E, DisplayName = "Exp Function")]
        [DataRow("exp2(3)", 8, DisplayName = "Exp2 Function")]
        [DataRow("floor(1.2)", 1, DisplayName = "Floor Function 1")]
        [DataRow("floor(-1.2)", -2, DisplayName = "Floor Function 2")]
        [DataRow("fract(1.2)", 0.2, DisplayName = "Fract Function 1")]
        [DataRow("fract(-1.2)", 0.8, DisplayName = "Fract Function 2")]
        [DataRow("inversesqrt(4)", 0.5, DisplayName = "Inversesqrt Function")]
        [DataRow("log(e)", 1, DisplayName = "Log Function")]
        [DataRow("log10(100)", 2, DisplayName = "Log10 Function")]
        [DataRow("log2(8)", 3, DisplayName = "Log2 Function")]
        [DataRow("max(1,2)", 2, DisplayName = "Max Function")]
        [DataRow("min(1,2)", 1, DisplayName = "Min Function")]
        [DataRow("mod(4,3)", 1, DisplayName = "Mod Function")]
        [DataRow("pow(2,3)", 8, DisplayName = "Pow Function")]
        [DataRow("radians(180)", Math.PI, DisplayName = "Radians Function")]
        [DataRow("round(1.4)", 1.0, DisplayName = "Round Function 1")]
        [DataRow("round(1.5)", 2.0, DisplayName = "Round Function 2")]
        [DataRow("round(-1.4)", -1.0, DisplayName = "Round Function 3")]
        [DataRow("sign(1)", 1, DisplayName = "Sign Function 1")]
        [DataRow("sign(-1)", -1, DisplayName = "Sign Function 2")]
        [DataRow("sign(0)", 0, DisplayName = "Sign Function 3")]
        [DataRow("sin(pi/2)", 1, DisplayName = "Sin Function")]
        [DataRow("sinh(1)", 1.175201, DisplayName = "Sinh Function")]
        [DataRow("sqrt(4)", 2, DisplayName = "Sqrt Function")]
        [DataRow("step(1,2)", 1.0, DisplayName = "Step Function 1")]
        [DataRow("step(2,2)", 1.0, DisplayName = "Step Function 2")]
        [DataRow("step(3,2)", 0.0, DisplayName = "Step Function 3")]
        [DataRow("tan(pi/4)", 1, DisplayName = "Tan Function")]
        [DataRow("tanh(1)", 0.761594, DisplayName = "Tanh Function")]
        [DataRow("trunc(1.2)", 1.0, DisplayName = "Trunc Function 1")]
        [DataRow("trunc(-1.2)", -1.0, DisplayName = "Trunc Function 2")]
        public void Parse_WithValidExpression_ExpectedResultIsReturned(string source, double expectedResult)
        {
            // setup
            var parser = ExpressionParser.Instance;

            // test
            var result = parser.Parse(source);
            Assert.AreEqual(Math.Round(expectedResult, 6), Math.Round(result, 6));
        }

        [TestMethod]
        [DataRow("10,000", 2, DisplayName = "Invalid Number Format 1")]
        [DataRow("1.2.3", 0, DisplayName = "Invalid Number Format 2")]
        [DataRow("1 2", 2, DisplayName = "Missing Operator 1")]
        [DataRow("2pi", 1, DisplayName = "Missing Operator 2")]
        [DataRow("1++2", 2, DisplayName = "Multiple Unary Operations 1")]
        [DataRow("1+-2", 2, DisplayName = "Multiple Unary Operations 2")]
        [DataRow("1--2", 2, DisplayName = "Multiple Unary Operations 3")]
        [DataRow("1-+2", 2, DisplayName = "Multiple Unary Operations 4")]
        public void Parse_WithInvalidExpression_ParseExcepionIsThrown(string source, int expectedPosition)
        {
            // setup
            var parser = ExpressionParser.Instance;

            // test
            try
            {
                var result = parser.Parse(source);
                Assert.Fail($"Invalid expression \"{source}\" parsing did not fail, and returned a result of {result}");
            }
            catch (ParseException e)
            {
                Assert.AreEqual(expectedPosition, e.Position);
            }
        }
    }
}
