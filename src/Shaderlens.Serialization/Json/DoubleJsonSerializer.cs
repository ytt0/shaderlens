namespace Shaderlens.Serialization.Json
{
    public class DoubleJsonSerializer : IJsonSerializer<double>
    {
        private readonly int digits;

        public DoubleJsonSerializer(int digits)
        {
            this.digits = digits;
        }

        public JsonNode? Serialize(double value)
        {
            return JsonValue.Create(Math.Round(value, this.digits))!;
        }

        public double Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return default;
            }

            try
            {
                return source.GetValue<double>();
            }
            catch (Exception e)
            {
                throw new JsonSourceException($"Failed to deserialize Double value \"{source.ToJsonString()}\"", source, e);
            }
        }
    }
}