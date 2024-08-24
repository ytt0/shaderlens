namespace Shaderlens.Presentation.Serialization
{
    public class TransformJsonSerializer : IJsonSerializer<Transform>
    {
        public JsonNode? Serialize(Transform value)
        {
            if (value is ScaleTransform scaleTransform)
            {
                return JsonValue.Create(Math.Round(scaleTransform.ScaleX, 2));
            }

            if (value == Transform.Identity)
            {
                return JsonValue.Create(1.0);
            }

            throw new Exception($"Unexpected Transform type \"{value.GetType().Name}\"");
        }

        public Transform Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return Transform.Identity;
            }

            try
            {
                var scale = source.GetValue<double>();
                return scale == 1.0 ? Transform.Identity : new ScaleTransform(scale, scale);
            }
            catch (Exception e)
            {
                throw new JsonSourceException($"Failed to deserialize Transform value \"{source.ToJsonString()}\"", source, e);
            }
        }
    }
}