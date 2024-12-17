namespace Shaderlens.Serialization.Project
{
    public interface IProjectSource
    {
        bool IsFullyLoaded { get; }
        string Path { get; }

        IFileResource<string>? Source { get; }
        IFileResource<string>? Settings { get; }
        IFileResource<string>? ViewSettings { get; }
        IFileResource<string>? UniformsSettings { get; }

        IProjectProgramSource Common { get; }
        IPassCollection<IProjectPass> Passes { get; }
        IEnumerable<IProjectViewerPass> Viewers { get; }

        bool Paused { get; }
        RenderSize? RenderSize { get; }
        bool? Scalable { get; }
    }

    public class ProjectSource : IProjectSource
    {
        public bool IsFullyLoaded { get; }
        public string Path { get; }

        public IFileResource<string>? Source { get; }
        public IFileResource<string>? Settings { get; }
        public IFileResource<string>? ViewSettings { get; }
        public IFileResource<string>? UniformsSettings { get; }

        public IProjectProgramSource Common { get; }
        public IEnumerable<IProjectViewerPass> Viewers { get; }

        public IPassCollection<IProjectPass> Passes { get; }

        public bool Paused { get; }
        public RenderSize? RenderSize { get; }
        public bool? Scalable { get; }

        public ProjectSource(
            bool isFullyLoaded,
            string path,
            IFileResource<string>? source,
            IFileResource<string>? settings,
            IFileResource<string>? viewSettings,
            IFileResource<string>? uniformsSettings,
            IProjectProgramSource common,
            IEnumerable<IProjectViewerPass> viewers,
            IPassCollection<IProjectPass> passes,
            bool paused,
            RenderSize? size,
            bool? scalable)
        {
            this.IsFullyLoaded = isFullyLoaded;
            this.Path = path;
            this.Source = source;
            this.Settings = settings;
            this.ViewSettings = viewSettings;
            this.UniformsSettings = uniformsSettings;
            this.Common = common;
            this.Viewers = viewers;
            this.Passes = passes;
            this.Paused = paused;
            this.RenderSize = size;
            this.Scalable = scalable;
        }
    }

    public partial class ProjectSourceJsonSerializer : IJsonSerializer<IProjectSource>
    {
        private readonly string projectPath;
        private readonly IFileSystem fileSystem;
        private readonly IJsonSerializer<RenderSize> renderSizeSerializer;
        private readonly IJsonSerializer<int> intSerializer;
        private readonly IJsonSerializer<bool> boolSerializer;

        public ProjectSourceJsonSerializer(string projectPath, IFileSystem fileSystem)
        {
            this.projectPath = projectPath;
            this.fileSystem = fileSystem;
            this.renderSizeSerializer = JsonSerializerContext.Create(new RenderSizeJsonSerializer());
            this.intSerializer = JsonSerializerContext.Create(new ValueJsonSerializer<int>());
            this.boolSerializer = JsonSerializerContext.Create(new ValueJsonSerializer<bool>());
        }

        public IProjectSource Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                throw new Exception($"Failed to deserialize project source element, a json object is expected");
            }

            if (source.GetValueKind() != JsonValueKind.Object)
            {
                throw new JsonSourceException($"Failed to deserialize project element of type {source.GetValueKind()}, a json object is expected", source);
            }

            var projectSource = this.fileSystem.ReadText(this.projectPath);
            var projectSettingsSource = GetProjectSettingsSource(source["Settings"]);
            var viewSettingsSource = GetViewSettingsSource(source["ViewSettings"]);
            var uniformsSettingsSource = GetUniformsSettingsSource(source["Uniforms"]);

            var serializer = JsonSerializerContext.Create(new ProjectProgramSourceJsonSerializer("Common", this.fileSystem));
            var commonSourceLocation = serializer.Deserialize(source["Common"]);

            var validator = GetChannelBindingValidator(source);

            ReadProjectPasses(source.AsObject(), validator, out var imagePass, out var bufferPasses);

            var passes = new PassCollection<IProjectPass>(imagePass, bufferPasses);
            var viewers = ReadViewersPasses(source, validator);

            var paused = source["Paused"] != null && this.boolSerializer.Deserialize(source["Paused"]);
            var size = source["RenderSize"] != null ? (RenderSize?)this.renderSizeSerializer.Deserialize(source["RenderSize"]) : null;
            var scalable = source["Scalable"] != null ? (bool?)this.boolSerializer.Deserialize(source["Scalable"]) : null;

            return new ProjectSource(
                true,
                this.projectPath,
                projectSource,
                projectSettingsSource,
                viewSettingsSource,
                uniformsSettingsSource,
                commonSourceLocation,
                viewers,
                passes,
                paused,
                size,
                scalable);
        }

        public JsonNode? Serialize(IProjectSource value)
        {
            throw new NotImplementedException();
        }

        private void ReadProjectPasses(JsonObject source, IChannelBindingValidator validator, out IProjectPass imagePass, out IReadOnlyDictionary<string, IProjectPass> bufferPasses)
        {
            var serializer = JsonSerializerContext.Create(new ProjectPassCollectionJsonSerializer(PassKeyRegex(), validator, this.fileSystem));
            var dictionary = serializer.Deserialize(source);

            if (!dictionary.TryGetValue("Image", out imagePass!))
            {
                throw new JsonSourceException("Failed to deserialize project, Image pass definition is missing", source);
            }

            bufferPasses = dictionary.Where(pair => pair.Key != "Image").ToDictionary();
        }

        private IEnumerable<IProjectViewerPass> ReadViewersPasses(JsonNode source, IChannelBindingValidator validator)
        {
            var viewerElement = source["Viewer"];
            var viewersListElement = source["Viewers"];

            if (viewerElement != null && viewersListElement != null)
            {
                throw new JsonSourceException($"Properties \"Viewer\", and \"Viewers\" (plural), cannot be defined at the same time", viewerElement!);
            }

            if (viewerElement != null)
            {
                return new[] { new ProjectViewerPass("Viewer", ReadViewerProgram("Viewer", viewerElement, validator)) };
            }

            if (viewersListElement != null)
            {
                var viewers = new List<IProjectViewerPass>();

                if (viewersListElement.GetValueKind() == JsonValueKind.Array)
                {
                    var index = 0;
                    foreach (var value in viewersListElement.AsArray())
                    {
                        if (value == null)
                        {
                            throw new JsonSourceException($"A viewer program is expected at index {index}", viewersListElement);
                        }

                        var key = $"Viewer{index}";
                        viewers.Add(new ProjectViewerPass(key, ReadViewerProgram(key, value, validator)));
                        index++;
                    }
                }
                else if (viewersListElement.GetValueKind() == JsonValueKind.Object)
                {
                    foreach (var property in viewersListElement.AsObject())
                    {
                        if (property.Value == null)
                        {
                            throw new JsonSourceException($"A viewer program is expected at {property.Key}", viewersListElement);
                        }

                        viewers.Add(new ProjectViewerPass(property.Key, ReadViewerProgram(property.Key, property.Value, validator)));
                    }
                }
                else
                {
                    throw new JsonSourceException($"Failed to deserialize element of type {viewersListElement.GetValueKind()}, a json object is expected", viewersListElement);
                }

                return viewers;
            }

            return Array.Empty<IProjectViewerPass>();
        }

        private IProjectProgram ReadViewerProgram(string name, JsonNode jsonNode, IChannelBindingValidator validator)
        {
            var serializer = JsonSerializerContext.Create(new ProjectProgramJsonSerializer(name, false, validator, this.fileSystem));
            return serializer.Deserialize(jsonNode);
        }

        private IFileResource<string> GetProjectSettingsSource(JsonNode? jsonNode)
        {
            var value = jsonNode?.GetStringValue();
            return value != null ? this.fileSystem.ReadText(value) :
                this.fileSystem.TryReadText(Path.Combine(".shaderlens", Path.GetFileNameWithoutExtension(this.projectPath) + ".settings.json"));
        }

        private IFileResource<string> GetViewSettingsSource(JsonNode? jsonNode)
        {
            var value = jsonNode?.GetStringValue();
            return value != null ? this.fileSystem.ReadText(value) :
                this.fileSystem.TryReadText(Path.Combine(".shaderlens", Path.GetFileNameWithoutExtension(this.projectPath) + ".view.json"));
        }

        private IFileResource<string> GetUniformsSettingsSource(JsonNode? jsonNode)
        {
            var value = jsonNode?.GetStringValue();
            return value != null ? this.fileSystem.ReadText(value) :
                this.fileSystem.TryReadText(Path.GetFileNameWithoutExtension(this.projectPath) + ".uniforms.json");
        }

        private IChannelBindingValidator GetChannelBindingValidator(JsonNode jsonNode)
        {
            var validator = new ChannelBindingValidator();
            foreach (var property in jsonNode.AsObject().Where(property => PassKeyRegex().IsMatch(property.Key)))
            {
                var outputsSource = property.Value?.GetValueKind() == JsonValueKind.Object ? property.Value["Outputs"] : null;
                var outputs = outputsSource != null ? this.intSerializer.Deserialize(outputsSource) : 1;
                if (outputs < 1 || outputs > 8)
                {
                    throw new JsonSourceException($"Output textures count should be between 1 and 8", outputsSource!);
                }

                validator.AddFramebufferDefinition(property.Key, outputs);
            }

            return validator;
        }

        [GeneratedRegex("^(?<key>Image|(Buffer([0-9]+|[A-Z])))$")]
        private static partial Regex PassKeyRegex();
    }
}
