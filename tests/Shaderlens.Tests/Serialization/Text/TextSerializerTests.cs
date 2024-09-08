namespace Shaderlens.Tests.Serialization.Text
{
    [TestClass]
    public class TextSerializerTests
    {
        [TestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void BoolSerialize_Completes_SerializeCorrectly(bool value, string expectedResult)
        {
            // setup
            var serializer = ValueTextSerializer.Bool;

            // test
            var result = serializer.Serialize(value);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(" true ", true, DisplayName = "True With Whitespace")]
        [DataRow(" false ", false, DisplayName = "False With Whitespace")]
        [DataRow("TRUE", true, DisplayName = "True Uppercase")]
        [DataRow("FALSE", false, DisplayName = "False Uppercase")]
        public void BoolDeserialize_WithValidText_DeserializeCorretly(string value, bool expectedResult)
        {
            // setup
            var serializer = ValueTextSerializer.Bool;

            // test
            Assert.IsTrue(serializer.TryDeserialize(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(0, "0")]
        [DataRow(1, "1")]
        [DataRow(-2, "-2")]
        public void IntSerialize_Completes_SerializeCorrectly(int value, string expectedResult)
        {
            // setup
            var serializer = ValueTextSerializer.Int;

            // test
            var result = serializer.Serialize(value);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("1", 1)]
        [DataRow("-2", -2)]
        public void IntDeserialize_WithValidText_DeserializeCorretly(string value, int expectedResult)
        {
            // setup
            var serializer = ValueTextSerializer.Int;

            // test
            Assert.IsTrue(serializer.TryDeserialize(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(1.2, "1.2")]
        [DataRow(-3.4, "-3.4")]
        public void DoubleSerialize_Completes_SerializeCorrectly(double value, string expectedResult)
        {
            // setup
            var serializer = ValueTextSerializer.Double;

            // test
            var result = serializer.Serialize(value);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("1.2", 1.2)]
        [DataRow("-3.4", -3.4)]
        public void DoubleDeserialize_WithValidText_DeserializeCorretly(string value, double expectedResult)
        {
            // setup
            var serializer = ValueTextSerializer.Double;

            // test
            Assert.IsTrue(serializer.TryDeserialize(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(new[] { true, false }, "[true, false]")]
        public void VectorSerialize_WithIntValues_SerializeCorrectly(bool[] value, string expectedResult)
        {
            // setup
            var serializer = new VectorTextSerializer<bool>(ValueTextSerializer.Bool);

            // test
            var result = serializer.Serialize(Vector.Create(value));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(new[] { 0.1, 0.2 }, "[0.1, 0.2]")]
        public void VectorSerialize_WithDoubleValues_SerializeCorrectly(double[] value, string expectedResult)
        {
            // setup
            var serializer = new VectorTextSerializer<double>(ValueTextSerializer.Double);

            // test
            var result = serializer.Serialize(Vector.Create(value));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void FixedSizeVectorDeserialize_WithMatchingSize_DeserializeCorrectly()
        {
            // setup
            var serializer = new FixedSizeVectorTextSerializer<int>(new VectorTextSerializer<int>(ValueTextSerializer.Int), 3);

            // test
            Assert.IsTrue(serializer.TryDeserialize("1, 2, 3", out var result));
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, result.ToArray());
        }

        [TestMethod]
        public void FixedSizeVectorDeserialize_WithNonMatchingSize_DeserializationFails()
        {
            // setup
            var serializer = new FixedSizeVectorTextSerializer<int>(new VectorTextSerializer<int>(ValueTextSerializer.Int), 3);

            // test
            Assert.IsFalse(serializer.TryDeserialize("1, 2", out _));
        }

        [TestMethod]
        [DataRow(0.2, 0.4, 0.6, 0.8, "[0.200, 0.400, 0.600, 0.800]")]
        public void ColorSerialize_Completes_SerializeCorrectly(double valueR, double valueG, double valueB, double valueA, string expectedResult)
        {
            // setup
            var serializer = ColorTextSerializer.Instance;
            var value = new SrgbColor(valueR, valueG, valueB, valueA);

            // test
            var result = serializer.Serialize(value);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("[0.200, 0.400, 0.600, 0.800]", 0.2, 0.4, 0.6, 0.8, DisplayName = "Float Array With Brackets")]
        [DataRow("[0, 1, 2, 3]", 0, 1, 2, 3, DisplayName = "Int Array With Brackets")]
        [DataRow("369", 0.2, 0.4, 0.6, 1.0, DisplayName = "Short Hex Without Alpha")]
        [DataRow("369c", 0.2, 0.4, 0.6, 0.8, DisplayName = "Short Hex")]
        [DataRow("336699", 0.2, 0.4, 0.6, 1.0, DisplayName = "Full Hex Without Alpha")]
        [DataRow("336699cc", 0.2, 0.4, 0.6, 0.8, DisplayName = "Full Hex")]
        [DataRow("#336699cc", 0.2, 0.4, 0.6, 0.8, DisplayName = "Full Hex With Prefix")]
        [DataRow("51, 102, 153", 0.2, 0.4, 0.6, 1.0, DisplayName = "Byte Array Without Alpha")]
        [DataRow("51, 102, 153, 204", 0.2, 0.4, 0.6, 0.8, DisplayName = "Byte Array")]
        [DataRow("0.200, 0.400, 0.600", 0.2, 0.4, 0.6, 1.0, DisplayName = "Float Array Without Alpha")]
        [DataRow("0.200, 0.400, 0.600, 0.800", 0.2, 0.4, 0.6, 0.8, DisplayName = "Float Array")]
        public void ColorDeserialize_WithValidText_DeserializeCorretly(string value, double resultR, double resultG, double resultB, double resultA)
        {
            // setup
            var serializer = ColorTextSerializer.Instance;
            var expectedResult = new SrgbColor(resultR, resultG, resultB, resultA);

            // test
            Assert.IsTrue(serializer.TryDeserialize(value, out var result));
            Assert.AreEqual(expectedResult, result);
        }
    }
}
