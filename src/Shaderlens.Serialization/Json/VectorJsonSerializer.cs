namespace Shaderlens.Serialization.Json
{
    public class VectorJsonSerializer : IJsonSerializer<Vector<double>>
    {
        private readonly IJsonSerializer<double> serializer;

        public VectorJsonSerializer() :
            this(new ValueJsonSerializer<double>())
        {
        }

        public VectorJsonSerializer(int digits) :
            this(new DoubleJsonSerializer(digits))
        {
        }

        public VectorJsonSerializer(IJsonSerializer<double> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(Vector<double> value)
        {
            return new JsonArray(value.Select(this.serializer.Serialize).ToArray());
        }

        public Vector<double> Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default!;
            }

            if (source.GetValueKind() != JsonValueKind.Array)
            {
                throw new JsonSourceException($"Failed to deserialize value of type {source.GetValueKind()}, an array is expected", source);
            }

            return Vector.Create(source.AsArray().OfType<JsonNode>().Select(item => this.serializer.Deserialize(item)).ToArray());
        }
    }
}