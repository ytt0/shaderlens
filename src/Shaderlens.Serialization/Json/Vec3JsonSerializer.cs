namespace Shaderlens.Serialization.Json
{
    public class Vec3JsonSerializer : IJsonSerializer<Vec3>
    {
        private readonly IJsonSerializer<double> serializer;

        public Vec3JsonSerializer() :
            this(new ValueJsonSerializer<double>())
        {
        }

        public Vec3JsonSerializer(int digits) :
            this(new DoubleJsonSerializer(digits))
        {
        }

        public Vec3JsonSerializer(IJsonSerializer<double> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(Vec3 value)
        {
            return new JsonObject
            {
                { "x", this.serializer.Serialize(value.X) },
                { "y", this.serializer.Serialize(value.Y) },
                { "z", this.serializer.Serialize(value.Z) },
            };
        }

        public Vec3 Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            return new Vec3(
                this.serializer.Deserialize(source["x"]),
                this.serializer.Deserialize(source["y"]),
                this.serializer.Deserialize(source["z"]));
        }
    }
}