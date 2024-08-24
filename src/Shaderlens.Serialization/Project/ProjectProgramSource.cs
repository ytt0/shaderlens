namespace Shaderlens.Serialization.Project
{
    public interface IProjectProgramSource : IEnumerable<IFileResource<string>>
    {
        IEnumerable<IFileResource<string>> Items { get; }
    }

    public class ProjectProgramSource : IProjectProgramSource
    {
        public static readonly IProjectProgramSource Empty = new ProjectProgramSource(Array.Empty<IFileResource<string>>());

        public IEnumerable<IFileResource<string>> Items { get; }

        public ProjectProgramSource(IEnumerable<IFileResource<string>> items)
        {
            this.Items = items;
        }

        public IEnumerator<IFileResource<string>> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ProjectProgramSourceJsonSerializer : IJsonSerializer<IProjectProgramSource>
    {
        private readonly IFileSystem fileSystem;
        private readonly string name;

        public ProjectProgramSourceJsonSerializer(string name, IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            this.name = name;
        }

        public IProjectProgramSource Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return ProjectProgramSource.Empty;
            }

            if (source.GetValueKind() != JsonValueKind.Object)
            {
                return new ProjectProgramSource(ReadSources(source));
            }

            if (source["Source"] == null)
            {
                throw new JsonSourceException($"{this.name} \"Source\" property is missing", source);
            }

            var sources = ReadSources(source["Source"]!);

            return new ProjectProgramSource(sources);
        }

        public JsonNode? Serialize(IProjectProgramSource value)
        {
            throw new NotImplementedException();
        }

        private IFileResource<string>[] ReadSources(JsonNode jsonNode)
        {
            if (jsonNode.TryGetStringValue(out var value) == true)
            {
                return new[] { this.fileSystem.ReadText(value, jsonNode) };
            }

            if (jsonNode is JsonArray)
            {
                return jsonNode.AsArray().Select(item => this.fileSystem.ReadText(item!.GetValue<string>()!, item)).ToArray();
            }

            throw new JsonSourceException($"Failed to deserialize source value of type {jsonNode.GetValueKind()}, a string value, or a string array is expected", jsonNode);
        }
    }
}
