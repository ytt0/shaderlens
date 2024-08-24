namespace Shaderlens.Serialization.Json
{
    public class Vec2JsonSerializer : IJsonSerializer<Vec2>
    {
        private readonly IJsonSerializer<double> serializer;

        public Vec2JsonSerializer() :
            this(new ValueJsonSerializer<double>())
        {
        }

        public Vec2JsonSerializer(int digits) :
            this(new DoubleJsonSerializer(digits))
        {
        }

        public Vec2JsonSerializer(IJsonSerializer<double> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(Vec2 value)
        {
            return new JsonObject
            {
                { "x", this.serializer.Serialize(value.X) },
                { "y", this.serializer.Serialize(value.Y) },
            };
        }

        public Vec2 Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            return new Vec2(
                this.serializer.Deserialize(source["x"]),
                this.serializer.Deserialize(source["y"]));
        }
    }
}