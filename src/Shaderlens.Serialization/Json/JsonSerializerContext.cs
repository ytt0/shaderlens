namespace Shaderlens.Serialization.Json
{
    public class JsonSerializerContext<T> : IJsonSerializer<T>
    {
        private readonly IJsonSerializer<T> serializer;
        private readonly IFileResource<string>? sourceFile;

        public JsonSerializerContext(IJsonSerializer<T> serializer, IFileResource<string>? sourceFile = null)
        {
            this.serializer = serializer;
            this.sourceFile = sourceFile;
        }

        public JsonNode? Serialize(T value)
        {
            return this.serializer.Serialize(value);
        }

        public T Deserialize(JsonNode? sourceNode)
        {
            try
            {
                return this.serializer.Deserialize(sourceNode);
            }
            catch (SourceLineException)
            {
                throw;
            }
            catch (Exception e)
            {
                var jsonSourceException = e as JsonSourceException;

                if (TryGetSourceLine(jsonSourceException?.SourceNode ?? sourceNode, this.sourceFile, out var sourceLine))
                {
                    throw new SourceLineException(e.ToString(), sourceLine, e);
                }

                if (jsonSourceException == null && sourceNode != null)
                {
                    var message = e is ArgumentException argumentException && argumentException.ParamName == "propertyName" ? e.Message : e.ToString();
                    throw new JsonSourceException(message, sourceNode, e);
                }

                throw;
            }
        }

        private static bool TryGetSourceLine(JsonNode? sourceNode, IFileResource<string>? sourceFile, out SourceLine sourceLine)
        {
            if (sourceFile != null && sourceNode != null && sourceNode.TryGetPosition(sourceFile.Value, out var position))
            {
                sourceFile.Value.GetLinePosition(position, out var lineIndex);
                var line = sourceFile.Value.SplitLines().ElementAt(lineIndex);

                sourceLine = new SourceLine(sourceFile, line, lineIndex);
                return true;
            }

            sourceLine = default;
            return false;
        }
    }

    public static class JsonSerializerContext
    {
        public static IJsonSerializer<T> Create<T>(IJsonSerializer<T> serializer, IFileResource<string>? sourceFile = null)
        {
            return new JsonSerializerContext<T>(serializer, sourceFile);
        }
    }
}