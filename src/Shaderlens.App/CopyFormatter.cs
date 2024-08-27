namespace Shaderlens
{
    public interface IStringFormatter<T>
    {
        string FormatString(T value);
    }

    public interface ICopyFormatter : IStringFormatter<ICopySource>
    {
        string DisplayName { get; }
        string Format { get; }
    }

    public class CopyFormatter : ICopyFormatter
    {
        public string DisplayName { get; }
        public string Format { get; }

        private string? indexedFormat;

        public CopyFormatter(string displayName, string format)
        {
            this.DisplayName = displayName;
            this.Format = format;
        }

        public string FormatString(ICopySource value)
        {
            var color = new SrgbColor(value.Value).Clamp();
            var linearColor = color.ToLinearRgb();

            var parameters = new Dictionary<string, object>
            {
                { "x", value.Value[0] },
                { "y", value.Value[1] },
                { "z", value.Value[2] },
                { "w", value.Value[3] },
                { "r", Math.Clamp(color.R, 0.0, 1.0) },
                { "g", Math.Clamp(color.G, 0.0, 1.0) },
                { "b", Math.Clamp(color.B, 0.0, 1.0) },
                { "a", Math.Clamp(color.A, 0.0, 1.0) },
                { "R", (int)Math.Clamp(255 * color.R, 0, 255) },
                { "G", (int)Math.Clamp(255 * color.G, 0, 255) },
                { "B", (int)Math.Clamp(255 * color.B, 0, 255) },
                { "A", (int)Math.Clamp(255 * color.A, 0, 255) },
                { "lr", linearColor.R },
                { "lg", linearColor.G },
                { "lb", linearColor.B },
                { "la", linearColor.A },
                { "px", value.Texture != null ? (double)value.X / value.Texture.Width : 0.0 },
                { "py", value.Texture != null ? (double)value.Y / value.Texture.Height : 0.0 },
                { "pX", value.X },
                { "pY", value.Y },
            }.ToArray();

            if (this.indexedFormat == null)
            {
                var keys = parameters.Select(pair => pair.Key).ToArray();

                this.indexedFormat = this.Format;
                for (var i = 0; i < keys.Length; i++)
                {
                    this.indexedFormat = this.indexedFormat.Replace($"{{{keys[i]}}}", i.ToString());
                }
            }

            var values = parameters.Select(pair => pair.Value).ToArray();
            try
            {
                return String.Format(this.indexedFormat, values);
            }
            catch (Exception e)
            {
                throw new Exception($"\"{this.DisplayName}\" format \"{this.Format}\" is invalid", e);
            }
        }
    }

    public class CopyFormatterSerializer : IJsonSerializer<ICopyFormatter>
    {
        public JsonNode? Serialize(ICopyFormatter value)
        {
            return new JsonObject
            {
                { "Name", value.DisplayName},
                { "Format", value.Format },
            };
        }

        public ICopyFormatter Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                throw new Exception("Copy formatter definition is missing, a json object is expected");
            }

            if (source.GetValueKind() != JsonValueKind.Object)
            {
                throw new JsonSourceException($"Failed to deserialize copy formatter, a json object is expected", source);
            }

            if (source["Name"]?.GetValueKind() != JsonValueKind.String)
            {
                throw new JsonSourceException($"Failed to deserialize copy formatter, \"Name\" string property is expected", source);
            }

            if (source["Format"]?.GetValueKind() != JsonValueKind.String)
            {
                throw new JsonSourceException($"Failed to deserialize copy formatter, \"Format\" string property is expected", source);
            }

            return new CopyFormatter(source["Name"]!.GetValue<string>(), source["Format"]!.GetValue<string>());
        }
    }
}
