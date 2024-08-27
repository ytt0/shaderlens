namespace Shaderlens.Tests.Serialization.Glsl
{
    [TestClass]
    public class GlslValueParserTests
    {
        [TestMethod]
        [DataRow("true", true)]
        [DataRow("false", false)]
        [DataRow(" true ", true, DisplayName = "Value With Whitespace")]
        public void BoolParse_WithValidValue_ValueIsParsed(string value, bool expectedResult)
        {
            // setup
            var serializer = GlslValueParser.Bool;

            // test
            Assert.IsTrue(serializer.TryParse(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void BoolParse_WithInvalidValue_ValueIsNotParsed()
        {
            // setup
            var serializer = GlslValueParser.Bool;

            // test
            Assert.IsFalse(serializer.TryParse("1", out _));
        }

        [TestMethod]
        [DataRow("0", 0)]
        [DataRow("-1", -1)]
        [DataRow(" 2 ", 2, DisplayName = "Value With Whitespace")]
        public void IntParse_WithValidValue_ValueIsParsed(string value, int expectedResult)
        {
            // setup
            var serializer = GlslValueParser.Int;

            // test
            Assert.IsTrue(serializer.TryParse(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void IntParse_WithInvalidValue_ValueIsNotParsed()
        {
            // setup
            var serializer = GlslValueParser.Int;

            // test
            Assert.IsFalse(serializer.TryParse("true", out _)); // true is an invalid boolean value
        }

        [TestMethod]
        [DataRow("0", 0u)]
        [DataRow("1", 1u)]
        [DataRow(" 2 ", 2u, DisplayName = "Value With Whitespace")]
        public void UIntParse_WithValidValue_ValueIsParsed(string value, uint expectedResult)
        {
            // setup
            var serializer = GlslValueParser.UInt;

            // test
            Assert.IsTrue(serializer.TryParse(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void UIntParse_WithInvalidValue_ValueIsNotParsed()
        {
            // setup
            var serializer = GlslValueParser.UInt;

            // test
            Assert.IsFalse(serializer.TryParse("true", out _)); // true is an invalid uint value
        }

        [TestMethod]
        public void VectorParse_WithValidValue_ValueIsParsed()
        {
            // setup
            var serializer = new VectorGlslValueParser<int>(GlslValueParser.Int, "vec2", 2);
            var expectedResult = Vector.Create(new[] { 1, 2 });

            // test
            Assert.IsTrue(serializer.TryParse("vec2(1, 2)", out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void VectorParse_WithSingleArgument_ValueIsParsed()
        {
            // setup
            var serializer = new VectorGlslValueParser<int>(GlslValueParser.Int, "vec2", 2);
            var expectedResult = Vector.Create(new[] { 1, 1 });

            // test
            Assert.IsTrue(serializer.TryParse("vec2(1)", out var result)); // both arguments have the same value
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void VectorParse_WithSingleArgumentWithoutType_ValueIsParsed()
        {
            // setup
            var serializer = new VectorGlslValueParser<int>(GlslValueParser.Int, "vec2", 2);
            var expectedResult = Vector.Create(new[] { 1, 1 });

            // test
            Assert.IsTrue(serializer.TryParse("1", out var result)); // value without type
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void VectorParse_WithInvalidType_ValueIsNotParsed()
        {
            // setup
            var serializer = new VectorGlslValueParser<int>(GlslValueParser.Int, "vec2", 2);

            // test
            Assert.IsFalse(serializer.TryParse("vec3(1, 2, 3)", out _)); // vec3 cannot be parsed as vec2
        }

        [TestMethod]
        public void VectorParse_WithInvalidArgumentsNumber_ValueIsNotParsed()
        {
            // setup
            var serializer = new VectorGlslValueParser<int>(GlslValueParser.Int, "vec2", 2);

            // test
            Assert.IsFalse(serializer.TryParse("vec2(1, 2, 3)", out _)); // vec2 expects 1 or 2 arguments
        }
    }
}