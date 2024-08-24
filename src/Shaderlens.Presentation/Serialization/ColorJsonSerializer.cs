namespace Shaderlens.Presentation.Serialization
{
    public class ColorJsonSerializer : IJsonSerializer<Color>
    {
        public JsonNode? Serialize(Color value)
        {
            if (value.A == 255)
            {
                var target = (uint)(value.R << 16) | (uint)(value.G << 8) | value.B;
                return JsonValue.Create(target.ToString("x6"));
            }
            else
            {
                var target = (uint)(value.R << 24) | (uint)(value.G << 16) | (uint)(value.B << 8) | value.A;
                return JsonValue.Create(target.ToString("x8"));
            }
        }

        public Color Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            if (source.GetValueKind() == JsonValueKind.String)
            {
                var stringValue = source.GetValue<string>();

                if (stringValue.Length == 6 && UInt32.TryParse(stringValue, System.Globalization.NumberStyles.HexNumber, null, out var hexValue))
                {
                    var r = (byte)(hexValue >> 16 & 0xff);
                    var g = (byte)(hexValue >> 8 & 0xff);
                    var b = (byte)(hexValue & 0xff);
                    return Color.FromRgb(r, g, b);
                }

                if (stringValue.Length == 8 && UInt32.TryParse(stringValue, System.Globalization.NumberStyles.HexNumber, null, out hexValue))
                {
                    var r = (byte)(hexValue >> 24);
                    var g = (byte)(hexValue >> 16 & 0xff);
                    var b = (byte)(hexValue >> 8 & 0xff);
                    var a = (byte)(hexValue & 0xff);
                    return Color.FromArgb(a, r, g, b);
                }
            }

            throw new JsonSourceException($"Failed to deserialize color, a rgb or rgba hex value is expected (\"000000-ffffff\" or \"00000000-ffffffff\")", source);
        }
    }
}