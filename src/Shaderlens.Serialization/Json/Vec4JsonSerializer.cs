namespace Shaderlens.Serialization.Json
{
    public class Vec4JsonSerializer : IJsonSerializer<Vec4>
    {
        private readonly IJsonSerializer<double> serializer;

        public Vec4JsonSerializer() :
            this(new ValueJsonSerializer<double>())
        {
        }

        public Vec4JsonSerializer(int digits) :
            this(new DoubleJsonSerializer(digits))
        {
        }

        public Vec4JsonSerializer(IJsonSerializer<double> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(Vec4 value)
        {
            return new JsonObject
            {
                { "x", this.serializer.Serialize(value.X) },
                { "y", this.serializer.Serialize(value.Y) },
                { "z", this.serializer.Serialize(value.Z) },
                { "w", this.serializer.Serialize(value.W) },
            };
        }

        public Vec4 Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            return new Vec4(
                this.serializer.Deserialize(source["x"]),
                this.serializer.Deserialize(source["y"]),
                this.serializer.Deserialize(source["z"]),
                this.serializer.Deserialize(source["w"]));
        }
    }
}