namespace Shaderlens.Extensions
{
    public static class JsonNodeExtensions
    {
        public static bool TryGetStringValue(this JsonNode jsonNode, [MaybeNullWhen(false)] out string value)
        {
            if (jsonNode.GetValueKind() == JsonValueKind.String)
            {
                value = jsonNode.GetValue<string>();
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetIntValue(this JsonNode jsonNode, out int value)
        {
            if (jsonNode.GetValueKind() == JsonValueKind.Number && !jsonNode.ToJsonString().Contains('.'))
            {
                value = jsonNode.GetValue<int>();
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetPosition(this JsonNode jsonNode, string sourceContent, out long position)
        {
            var options = new JsonReaderOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip, MaxDepth = 10 };
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(sourceContent), options);

            reader.Read();
            return TryGetPathPositionInNode(ref reader, "$", jsonNode.GetPath(), out position);
        }

        private static bool TryGetPathPositionInNode(ref Utf8JsonReader reader, string readerPath, string targetPath, out long targetPosition)
        {
            if (readerPath == targetPath)
            {
                targetPosition = reader.BytesConsumed;
                return true;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                return TryGetPathPositionInObject(ref reader, readerPath, targetPath, out targetPosition);
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return TryGetPathPositionInArray(ref reader, readerPath, targetPath, out targetPosition);
            }

            reader.Read();
            targetPosition = 0;
            return false;
        }

        private static bool TryGetPathPositionInObject(ref Utf8JsonReader reader, string readerPath, string targetPath, out long targetPosition)
        {
            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();
                reader.Read();

                var propertyPath = ContainsSpecialCharacters(propertyName) ? $"{readerPath}['{propertyName}']" : $"{readerPath}.{propertyName}";

                if (TryGetPathPositionInNode(ref reader, propertyPath, targetPath, out targetPosition))
                {
                    return true;
                }
            }

            reader.Read();
            targetPosition = 0;
            return false;
        }

        private static bool TryGetPathPositionInArray(ref Utf8JsonReader reader, string readerPath, string targetPath, out long targetPosition)
        {
            reader.Read();

            var index = 0;
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var itemPath = $"{readerPath}[{index++}]";
                if (TryGetPathPositionInNode(ref reader, itemPath, targetPath, out targetPosition))
                {
                    return true;
                }
            }

            reader.Read();
            targetPosition = 0;
            return false;
        }

        private static bool ContainsSpecialCharacters(this ReadOnlySpan<char> text)
        {
            const string SpecialCharacters = ". '/\"[]()\t\n\r\f\b\\\u0085\u2028\u2029";
            return text.IndexOfAny(SpecialCharacters.AsSpan()) != -1;
        }
    }
}
