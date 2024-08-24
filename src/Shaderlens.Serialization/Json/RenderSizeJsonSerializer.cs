namespace Shaderlens.Serialization.Json
{
    public class RenderSizeJsonSerializer : IJsonSerializer<RenderSize>
    {
        public JsonNode? Serialize(RenderSize value)
        {
            return JsonValue.Create($"{value.Width}, {value.Height}");
        }

        public RenderSize Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            if (source.GetValueKind() != JsonValueKind.String)
            {
                throw new JsonSourceException($"Failed to deserialize size value of type {source.GetValueKind()}, a string of the format \"width, height\" is expected", source);
            }

            var values = source.GetValue<string>().Split(',');
            if (values.Length == 2 && Int32.TryParse(values[0].Trim(), out var width) && Int32.TryParse(values[1].Trim(), out var height))
            {
                return new RenderSize(width, height);
            }

            throw new JsonSourceException($"Failed to deserialize size, a string of the format \"width, height\" is expected", source);
        }
    }
}