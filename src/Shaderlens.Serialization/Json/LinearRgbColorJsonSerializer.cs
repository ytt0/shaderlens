namespace Shaderlens.Serialization.Json
{
    public class LinearRgbColorJsonSerializer : IJsonSerializer<LinearRgbColor>
    {
        private readonly IJsonSerializer<double> serializer;

        public LinearRgbColorJsonSerializer() :
            this(new ValueJsonSerializer<double>())
        {
        }

        public LinearRgbColorJsonSerializer(int digits) :
            this(new DoubleJsonSerializer(digits))
        {
        }

        public LinearRgbColorJsonSerializer(IJsonSerializer<double> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(LinearRgbColor value)
        {
            return new JsonObject
            {
                { "r", this.serializer.Serialize(value.R) },
                { "g", this.serializer.Serialize(value.G) },
                { "b", this.serializer.Serialize(value.B) },
                { "a", this.serializer.Serialize(value.A) },
            };
        }

        public LinearRgbColor Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            return new LinearRgbColor(
                this.serializer.Deserialize(source["r"]),
                this.serializer.Deserialize(source["g"]),
                this.serializer.Deserialize(source["b"]),
                this.serializer.Deserialize(source["a"]));
        }
    }
}