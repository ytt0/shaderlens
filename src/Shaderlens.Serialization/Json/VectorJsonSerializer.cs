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

    public class FixedSizeVectorJsonSerializer<T> : IJsonSerializer<Vector<T>>
    {
        private readonly IJsonSerializer<Vector<T>> serializer;
        private readonly Vector<T> defaultValue;

        public FixedSizeVectorJsonSerializer(IJsonSerializer<Vector<T>> serializer, Vector<T> defaultValue)
        {
            this.serializer = serializer;
            this.defaultValue = defaultValue;
        }

        public JsonNode? Serialize(Vector<T> value)
        {
            return this.serializer.Serialize(SetVectorFixedSize(value, this.defaultValue));
        }

        public Vector<T> Deserialize(JsonNode? source)
        {
            return SetVectorFixedSize(this.serializer.Deserialize(source), this.defaultValue);
        }

        private static Vector<T> SetVectorFixedSize(Vector<T> value, Vector<T> defaultValue)
        {
            if (value.Count != defaultValue.Count)
            {
                value = Vector.Create(value.ToArray().Take(defaultValue.Count).Concat(defaultValue.Skip(value.Count)).ToArray());
            }

            return value;
        }
    }
}