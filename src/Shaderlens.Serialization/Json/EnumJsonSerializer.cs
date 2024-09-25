
namespace Shaderlens.Serialization.Json
{
    public class EnumJsonSerializer<T> : IJsonSerializer<T> where T : Enum
    {
        public JsonNode? Serialize(T value)
        {
            return JsonValue.Create(value.ToString());
        }

        public T Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default!;
            }

            try
            {
                return (T)Enum.Parse(typeof(T), source.GetStringValue());
            }
            catch (Exception e)
            {
                throw new JsonSourceException($"Failed to deserialize {typeof(T).Name} value \"{source.ToJsonString()}\"", source, e);
            }
        }
    }
}
