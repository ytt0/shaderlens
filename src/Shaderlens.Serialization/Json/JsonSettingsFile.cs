namespace Shaderlens.Serialization.Json
{
    public interface IJsonSettingsFile
    {
        string Path { get; }
        IJsonSettings Content { get; }
        void Save();
    }

    public class JsonSettingsFile : IJsonSettingsFile
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public string Path { get; }
        public IJsonSettings Content { get; }

        private readonly JsonObject element;
        private string serializedContent;

        public JsonSettingsFile(JsonObject element, string path)
        {
            this.element = element;
            this.Path = path;
            this.Content = new JsonSettings(element);
            this.serializedContent = SerializeElement(element);
        }

        public void Save()
        {
            var serializedContent = SerializeElement(this.element);

            if (this.serializedContent != serializedContent)
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(this.Path)!);
                this.serializedContent = serializedContent;
                File.WriteAllText(this.Path, serializedContent);
            }
        }

        public static JsonSettingsFile Load(string path)
        {
            var element = TryLoadElement(path) ?? new JsonObject();
            return new JsonSettingsFile(element, path);
        }

        private static JsonObject? TryLoadElement(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(path), Options);
                }
            }
            catch
            {
            }

            return null;
        }

        private static string SerializeElement(JsonObject element)
        {
            return JsonSerializer.Serialize(element, Options);
        }
    }
}
