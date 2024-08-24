namespace Shaderlens.Serialization.Json
{
    public class SrgbColorJsonSerializer : IJsonSerializer<SrgbColor>
    {
        private readonly IJsonSerializer<double> serializer;

        public SrgbColorJsonSerializer() :
            this(new ValueJsonSerializer<double>())
        {
        }

        public SrgbColorJsonSerializer(int digits) :
            this(new DoubleJsonSerializer(digits))
        {
        }

        public SrgbColorJsonSerializer(IJsonSerializer<double> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(SrgbColor value)
        {
            return new JsonObject
            {
                { "r", this.serializer.Serialize(value.R) },
                { "g", this.serializer.Serialize(value.G) },
                { "b", this.serializer.Serialize(value.B) },
                { "a", this.serializer.Serialize(value.A) },
            };
        }

        public SrgbColor Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            return new SrgbColor(
                this.serializer.Deserialize(source["r"]),
                this.serializer.Deserialize(source["g"]),
                this.serializer.Deserialize(source["b"]),
                this.serializer.Deserialize(source["a"]));
        }
    }
}