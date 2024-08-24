namespace Shaderlens.Serialization.Json
{
    public class ValueJsonSerializer<T> : IJsonSerializer<T>
    {
        public JsonNode? Serialize(T value)
        {
            return JsonValue.Create(value)!;
        }

        public T Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default!;
            }

            try
            {
                return source.GetValue<T>();
            }
            catch (Exception e)
            {
                throw new JsonSourceException($"Failed to deserialize {typeof(T).Name} value \"{source.ToJsonString()}\"", source, e);
            }
        }
    }
}