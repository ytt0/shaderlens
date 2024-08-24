namespace Shaderlens.Presentation.Serialization
{
    public class BrushJsonSerializer : IJsonSerializer<Brush>
    {
        private readonly ColorJsonSerializer serializer;

        public BrushJsonSerializer()
        {
            this.serializer = new ColorJsonSerializer();
        }

        public Brush Deserialize(JsonNode? source)
        {
            return new SolidColorBrush(this.serializer.Deserialize(source));
        }

        public JsonNode? Serialize(Brush value)
        {
            if (value is SolidColorBrush solidColorBrush)
            {
                return this.serializer.Serialize(solidColorBrush.Color);
            }

            throw new NotSupportedException($"Unexpected brush type {value.GetType().FullName}");
        }
    }
}