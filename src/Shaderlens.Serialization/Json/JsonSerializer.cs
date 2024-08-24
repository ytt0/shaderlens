namespace Shaderlens.Serialization.Json
{
    public interface IJsonSerializer<T>
    {
        JsonNode? Serialize(T value);
        T Deserialize(JsonNode? source);
    }

    public static class JsonSerializerExtensions
    {
        public static T Deserialize<T>(this IJsonSerializer<T> serializer, JsonNode? source, T defaultValue)
        {
            return source != null ? serializer.Deserialize(source) : defaultValue;
        }

        public static bool TryDeserialize<T>(this IJsonSerializer<T> serializer, JsonNode source, [MaybeNullWhen(false)] out T value)
        {
            try
            {
                value = serializer.Deserialize(source);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }

    public class JsonSourceException : Exception
    {
        public JsonNode SourceNode { get; }

        public JsonSourceException(string message, JsonNode source, Exception? innerException = null) :
            base(message, innerException)
        {
            this.SourceNode = source;
        }

        public override string ToString()
        {
            return this.Message;
        }
    }

    public class NullableJsonSerializer<T> : IJsonSerializer<T?>
    {
        private readonly IJsonSerializer<T> jsonSerializer;

        public NullableJsonSerializer(IJsonSerializer<T> jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public T? Deserialize(JsonNode? source)
        {
            return this.jsonSerializer.Deserialize(source);
        }

        public JsonNode? Serialize(T? value)
        {
            return value != null ? this.jsonSerializer.Serialize(value) : null;
        }
    }

    public static class NullableJsonSerializer
    {
        public static IJsonSerializer<T?> Create<T>(IJsonSerializer<T> jsonSerializer)
        {
            return new NullableJsonSerializer<T>(jsonSerializer);
        }
    }
}