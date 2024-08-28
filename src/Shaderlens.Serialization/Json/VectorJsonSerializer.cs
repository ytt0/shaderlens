namespace Shaderlens.Serialization.Json
{
    public class VectorJsonSerializer<T> : IJsonSerializer<Vector<T>>
    {
        private readonly IJsonSerializer<T> serializer;

        public VectorJsonSerializer() :
            this(new ValueJsonSerializer<T>())
        {
        }

        public VectorJsonSerializer(IJsonSerializer<T> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(Vector<T> value)
        {
            return new JsonArray(value.Select(this.serializer.Serialize).ToArray());
        }

        public Vector<T> Deserialize(JsonNode? source)
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

    public static class VectorJsonSerializer
    {
        public static readonly VectorJsonSerializer<bool> Bool = new VectorJsonSerializer<bool>();
        public static readonly VectorJsonSerializer<double> Double = new VectorJsonSerializer<double>();
        public static readonly VectorJsonSerializer<int> Int = new VectorJsonSerializer<int>();
        public static readonly VectorJsonSerializer<uint> UInt = new VectorJsonSerializer<uint>();
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