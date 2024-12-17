namespace Shaderlens.Serialization.Project
{
    public interface IProjectProgram
    {
        string DisplayName { get; }
        IProjectProgramSource Source { get; }
        IChannelBindingSource Binding { get; }
        bool IncludeCommonSource { get; }
    }

    public class ProjectProgram : IProjectProgram
    {
        public string DisplayName { get; }
        public IProjectProgramSource Source { get; }
        public IChannelBindingSource Binding { get; }
        public bool IncludeCommonSource { get; }

        public ProjectProgram(string displayName, IProjectProgramSource source, IChannelBindingSource binding, bool includeCommonSource)
        {
            this.DisplayName = displayName;
            this.Source = source;
            this.Binding = binding;
            this.IncludeCommonSource = includeCommonSource;
        }
    }

    public class ProjectProgramJsonSerializer : IJsonSerializer<IProjectProgram>
    {
        private readonly string passName;
        private readonly bool includeCommonSource;
        private readonly IChannelBindingValidator validator;
        private readonly IFileSystem fileSystem;
        private readonly DisplayNameFormatter displayNameFormatter;

        public ProjectProgramJsonSerializer(string passName, bool includeCommonSource, IChannelBindingValidator validator, IFileSystem fileSystem)
        {
            this.passName = passName;
            this.includeCommonSource = includeCommonSource;
            this.validator = validator;
            this.fileSystem = fileSystem;
            this.displayNameFormatter = new DisplayNameFormatter();
        }

        public IProjectProgram Deserialize(JsonNode? sourceNode)
        {
            if (sourceNode == null)
            {
                throw new Exception($"{this.passName} pass definition is missing");
            }

            var sourceSerializer = JsonSerializerContext.Create(new ProjectProgramSourceJsonSerializer(this.passName, this.fileSystem));
            var source = sourceSerializer.Deserialize(sourceNode);

            var bindings = ChannelBindingSource.Empty;
            var displayName = TryGetDisplayName(source) ?? this.passName;
            var includeCommonSource = this.includeCommonSource;

            if (sourceNode.GetValueKind() == JsonValueKind.Object)
            {
                var bindingSerializer = JsonSerializerContext.Create(new ChannelBindingSourceCollectionJsonSerializer(this.validator, this.fileSystem));
                bindings = bindingSerializer.Deserialize(sourceNode.AsObject());
                displayName = sourceNode["DisplayName"]?.GetStringValue() ?? displayName;
                includeCommonSource = sourceNode["IncludeCommon"]?.GetBooleanValue() ?? includeCommonSource;
            }

            return new ProjectProgram(displayName, source, bindings, includeCommonSource);
        }

        public JsonNode? Serialize(IProjectProgram value)
        {
            throw new NotImplementedException();
        }

        private string? TryGetDisplayName(IProjectProgramSource source)
        {
            var path = source.FirstOrDefault()?.Key.AbsolutePath;
            return path != null ? this.displayNameFormatter.GetDisplayName(Path.GetFileNameWithoutExtension(path)) : null;
        }
    }
}
