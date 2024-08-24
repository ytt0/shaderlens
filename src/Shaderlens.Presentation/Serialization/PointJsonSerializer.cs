namespace Shaderlens.Presentation.Serialization
{
    public class PointJsonSerializer : IJsonSerializer<Point>
    {
        public JsonNode? Serialize(Point value)
        {
            return JsonValue.Create($"{(int)value.X}, {(int)value.Y}");
        }

        public Point Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            if (source.GetValueKind() != JsonValueKind.String)
            {
                throw new JsonSourceException($"Failed to deserialize point value of type {source.GetValueKind()}, a string of the format \"x,y\" is expected", source);
            }

            var values = source.GetValue<string>().Split(',');
            if (values.Length == 2 && Double.TryParse(values[0].Trim(), out var x) && Double.TryParse(values[1].Trim(), out var y))
            {
                return new Point(x, y);
            }

            throw new JsonSourceException($"Failed to deserialize point, a string of the format \"x,y\" is expected", source);
        }
    }
}