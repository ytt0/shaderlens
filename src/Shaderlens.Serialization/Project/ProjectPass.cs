namespace Shaderlens.Serialization.Project
{
    public interface IProjectPass
    {
        string Key { get; }
        IProjectProgram Program { get; }
        ViewerPassSelection? DefaultViewer { get; }
        RenderSize? RenderSize { get; }
        bool? Scalable { get; }
    }

    public class ProjectPass : IProjectPass
    {
        public string Key { get; }
        public IProjectProgram Program { get; }
        public ViewerPassSelection? DefaultViewer { get; }
        public RenderSize? RenderSize { get; }
        public bool? Scalable { get; }

        public ProjectPass(string key, IProjectProgram program, ViewerPassSelection? defaultViewer, RenderSize? size, bool? scalable)
        {
            this.Key = key;
            this.Program = program;
            this.DefaultViewer = defaultViewer;
            this.RenderSize = size;
            this.Scalable = scalable;
        }
    }

    public class ProjectPassJsonSerializer : IJsonSerializer<IProjectPass>
    {
        private readonly string key;
        private readonly IProjectPassValidator validator;
        private readonly IFileSystem fileSystem;
        private readonly IJsonSerializer<RenderSize> renderSizeSerializer;
        private readonly IJsonSerializer<bool> boolSerializer;
        private readonly ViewerPassSelectionSerializer viewerPassSelectionSerializer;

        public ProjectPassJsonSerializer(string key, IProjectPassValidator validator, IFileSystem fileSystem)
        {
            this.key = key;
            this.validator = validator;
            this.fileSystem = fileSystem;
            this.renderSizeSerializer = JsonSerializerContext.Create(new RenderSizeJsonSerializer());
            this.boolSerializer = JsonSerializerContext.Create(new ValueJsonSerializer<bool>());
            this.viewerPassSelectionSerializer = new ViewerPassSelectionSerializer();
        }

        public IProjectPass Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                throw new Exception($"{this.key} pass definition is missing");
            }

            if (source.GetValueKind() != JsonValueKind.Object)
            {
                var serializer = JsonSerializerContext.Create(new ProjectProgramJsonSerializer(this.key, true, this.validator, this.fileSystem));
                var program = serializer.Deserialize(source);

                return new ProjectPass(this.key, program, null, null, null);
            }
            else
            {
                var displayName = source["DisplayName"]?.GetStringValue() ?? this.key;

                var serializer = JsonSerializerContext.Create(new ProjectProgramJsonSerializer(displayName!, true, this.validator, this.fileSystem));
                var program = serializer.Deserialize(source);

                var defaultViewer = this.viewerPassSelectionSerializer.Deserialize(source["DefaultViewer"]);

                var size = source["RenderSize"] != null ? (RenderSize?)this.renderSizeSerializer.Deserialize(source["RenderSize"]!) : null;
                var scalable = source["Scalable"] != null ? (bool?)this.boolSerializer.Deserialize(source["Scalable"]!) : null;

                return new ProjectPass(this.key, program, defaultViewer, size, scalable);
            }
        }

        public JsonNode? Serialize(IProjectPass value)
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectPassCollectionJsonSerializer : IJsonSerializer<IReadOnlyDictionary<string, IProjectPass>>
    {
        private readonly Regex passKeyRegex;
        private readonly IProjectPassValidator validator;
        private readonly IFileSystem fileSystem;

        public ProjectPassCollectionJsonSerializer(Regex passKeyRegex, IProjectPassValidator validator, IFileSystem fileSystem)
        {
            this.passKeyRegex = passKeyRegex;
            this.validator = validator;
            this.fileSystem = fileSystem;
        }

        public IReadOnlyDictionary<string, IProjectPass> Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return Dictionary.Empty<string, IProjectPass>();
            }

            if (source.GetValueKind() != JsonValueKind.Object)
            {
                throw new Json.JsonSourceException($"Failed to deserialize element of type {source.GetValueKind()}, a json object is expected", source);
            }

            var passes = new Dictionary<string, IProjectPass>();
            foreach (var property in source.AsObject())
            {
                var match = this.passKeyRegex.Match(property.Key);
                if (match.Success && property.Value != null)
                {
                    var serializer = JsonSerializerContext.Create(new ProjectPassJsonSerializer(property.Key, this.validator, this.fileSystem));
                    passes.Add(property.Key, serializer.Deserialize(property.Value));
                }
            }

            return passes;
        }

        public JsonNode? Serialize(IReadOnlyDictionary<string, IProjectPass> value)
        {
            throw new NotImplementedException();
        }
    }
}
