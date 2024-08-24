namespace Shaderlens.Presentation.Serialization
{
    public class SizeJsonSerializer : IJsonSerializer<Size>
    {
        public JsonNode? Serialize(Size value)
        {
            return JsonValue.Create($"{(int)value.Width}, {(int)value.Height}");
        }

        public Size Deserialize(JsonNode? source)
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
            if (values.Length == 2 && Double.TryParse(values[0].Trim(), out var width) && Double.TryParse(values[1].Trim(), out var height))
            {
                return new Size(width, height);
            }

            throw new JsonSourceException($"Failed to deserialize size, a string of the format \"width, height\" is expected", source);
        }
    }
}