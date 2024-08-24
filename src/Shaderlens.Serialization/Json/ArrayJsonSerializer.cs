namespace Shaderlens.Serialization.Json
{
    public class ArrayJsonSerializer<T> : IJsonSerializer<IEnumerable<T>>
    {
        private readonly IJsonSerializer<T> serializer;

        public ArrayJsonSerializer(IJsonSerializer<T> serializer)
        {
            this.serializer = serializer;
        }

        public JsonNode? Serialize(IEnumerable<T> value)
        {
            return new JsonArray(value.Select(this.serializer.Serialize).ToArray());
        }

        public IEnumerable<T> Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return Array.Empty<T>();
            }

            if (source.GetValueKind() != JsonValueKind.Array)
            {
                throw new JsonSourceException($"Failed to deserialize value of type {source.GetValueKind()}, an array is expected", source);
            }

            return source.AsArray().OfType<JsonNode>().Select(this.serializer.Deserialize).ToArray();
        }
    }
}