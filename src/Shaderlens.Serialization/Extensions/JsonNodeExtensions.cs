namespace Shaderlens.Serialization.Extensions
{
    public static class JsonNodeExtensions
    {
        public static bool GetBooleanValue(this JsonNode jsonNode)
        {
            var kind = jsonNode.GetValueKind();
            return kind == JsonValueKind.True || (kind == JsonValueKind.False ? false :
                throw new JsonSourceException("A boolean value is expected", jsonNode));
        }

        public static string GetStringValue(this JsonNode jsonNode)
        {
            if (jsonNode.GetValueKind() == JsonValueKind.String)
            {
                return jsonNode.GetValue<string>();
            }

            throw new JsonSourceException("A string value is expected", jsonNode);
        }
    }
}
