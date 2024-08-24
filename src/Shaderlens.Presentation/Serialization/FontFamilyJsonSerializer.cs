namespace Shaderlens.Presentation.Serialization
{
    public class FontFamilyJsonSerializer : IJsonSerializer<FontFamily>
    {
        private readonly ValueJsonSerializer<string> serializer;

        public FontFamilyJsonSerializer()
        {
            this.serializer = new ValueJsonSerializer<string>();
        }

        public FontFamily Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return new FontFamily();
            }

            try
            {
                return new FontFamily(this.serializer.Deserialize(source));
            }
            catch (Exception e)
            {
                throw new JsonSourceException($"Failed to deserialize font, a string with font family name is expected", source, e);
            }
        }

        public JsonNode? Serialize(FontFamily value)
        {
            return this.serializer.Serialize(value.ToString());
        }
    }
}