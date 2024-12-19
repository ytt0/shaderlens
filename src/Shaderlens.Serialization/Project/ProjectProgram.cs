namespace Shaderlens.Serialization.Project
{
    public enum ProjectProgramType { Fragment, Compute }

    public interface IProjectProgram
    {
        ProjectProgramType Type { get; }
        string DisplayName { get; }
        IProjectProgramSource Source { get; }
        IChannelBindingSource Binding { get; }
        bool IncludeCommonSource { get; }
        Vector<int>? WorkGroupSize { get; }
        Vector<int>? WorkGroups { get; }
    }

    public class ProjectProgram : IProjectProgram
    {
        public ProjectProgramType Type { get; }
        public string DisplayName { get; }
        public IProjectProgramSource Source { get; }
        public IChannelBindingSource Binding { get; }
        public bool IncludeCommonSource { get; }
        public Vector<int>? WorkGroupSize { get; }
        public Vector<int>? WorkGroups { get; }

        public ProjectProgram(ProjectProgramType type, string displayName, IProjectProgramSource source, IChannelBindingSource binding, bool includeCommonSource, Vector<int>? workGroupSize, Vector<int>? workGroups)
        {
            this.Type = type;
            this.DisplayName = displayName;
            this.Source = source;
            this.Binding = binding;
            this.IncludeCommonSource = includeCommonSource;
            this.WorkGroupSize = workGroupSize;
            this.WorkGroups = workGroups;
        }
    }

    public class ProjectProgramJsonSerializer : IJsonSerializer<IProjectProgram>
    {
        private readonly string passName;
        private readonly bool includeCommonSource;
        private readonly IChannelBindingValidator validator;
        private readonly IFileSystem fileSystem;
        private readonly DisplayNameFormatter displayNameFormatter;
        private readonly IJsonSerializer<Vector<int>> vectorSerializer;

        public ProjectProgramJsonSerializer(string passName, bool includeCommonSource, IChannelBindingValidator validator, IFileSystem fileSystem)
        {
            this.passName = passName;
            this.includeCommonSource = includeCommonSource;
            this.validator = validator;
            this.fileSystem = fileSystem;
            this.displayNameFormatter = new DisplayNameFormatter();
            this.vectorSerializer = JsonSerializerContext.Create(new FixedSizeVectorJsonSerializer<int>(new TextVectorJsonSerializer<int>(ValueTextSerializer.Int), Vector.Create(3, 1)));
        }

        public IProjectProgram Deserialize(JsonNode? sourceNode)
        {
            if (sourceNode == null)
            {
                throw new Exception($"{this.passName} pass definition is missing");
            }

            var sourceSerializer = JsonSerializerContext.Create(new ProjectProgramSourceJsonSerializer(this.passName, this.fileSystem));
            var source = sourceSerializer.Deserialize(sourceNode);

            var type = ProjectProgramType.Fragment;
            var bindings = ChannelBindingSource.Empty;
            var displayName = TryGetDisplayName(source) ?? this.passName;
            var includeCommonSource = this.includeCommonSource;
            Vector<int>? workGroupSize = null;
            Vector<int>? workGroups = null;

            if (sourceNode.GetValueKind() == JsonValueKind.Object)
            {
                type = DeserializeType(sourceNode);
                var bindingSerializer = JsonSerializerContext.Create(new ChannelBindingSourceCollectionJsonSerializer(this.validator, this.fileSystem));
                bindings = bindingSerializer.Deserialize(sourceNode.AsObject());
                displayName = sourceNode["DisplayName"]?.GetStringValue() ?? displayName;
                includeCommonSource = sourceNode["IncludeCommon"]?.GetBooleanValue() ?? includeCommonSource;
                workGroupSize = sourceNode["WorkGroupSize"] != null ? DeserializeComputeSize(sourceNode["WorkGroupSize"]!) : workGroupSize;
                workGroups = sourceNode["WorkGroups"] != null ? DeserializeComputeSize(sourceNode["WorkGroups"]!) : workGroups;
            }

            return new ProjectProgram(type, displayName, source, bindings, includeCommonSource, workGroupSize, workGroups);
        }

        public JsonNode? Serialize(IProjectProgram value)
        {
            throw new NotImplementedException();
        }

        private Vector<int> DeserializeComputeSize(JsonNode jsonNode)
        {
            var vector = this.vectorSerializer.Deserialize(jsonNode);

            if (vector.Any(i => i < 1))
            {
                throw new JsonException($"Size {vector} is invalid, expected size should be at least (1, 1, 1)");
            }

            return vector;
        }

        private string? TryGetDisplayName(IProjectProgramSource source)
        {
            var path = source.FirstOrDefault()?.Key.AbsolutePath;
            return path != null ? this.displayNameFormatter.GetDisplayName(Path.GetFileNameWithoutExtension(path)) : null;
        }

        private static ProjectProgramType DeserializeType(JsonNode sourceNode)
        {
            var value = sourceNode["Type"]?.GetStringValue();
            switch (value)
            {
                case null:
                case "Fragment": return ProjectProgramType.Fragment;
                case "Compute": return ProjectProgramType.Compute;
                default: throw new JsonSourceException($"Invalid type \"{value}\", expected types are: Fragment (default), and Compute", sourceNode["Type"]!);
            }
        }
    }
}
