namespace Shaderlens.Serialization.Project
{
    public interface IProjectSettings
    {
        ISettingsValue<string?> SaveFramePath { get; }
        ISettingsValue<string?> EditedColor { get; }
        ISettingsValue<int> UniformScroll { get; }
        ISettingsValue<int> UniformDockHeight { get; }
        ISettingsValue<double> UniformColumnRatio { get; }
        ISettingsValue<IEnumerable<ViewerPassSelection>?> ViewerPasses { get; }

        ISettingsValue<RenderSequenceSettings> RenderSequence { get; }

        IProjectSettings CreateMergedSettings(IFileResource<string> projectSettingsFile, IFileResource<string> viewSettingsFile, IFileResource<string> uniformsSettingsFile);

        ISettingsValue<bool> GetGroupExpandedValue(string name, bool defaultValue);
        ISettingsValue<T> GetUniformValue<T>(IJsonSerializer<T> serializer, string name, T defaultValue);

        void Save();
    }

    public class ProjectSettings : IProjectSettings
    {
        public ISettingsValue<string?> SaveFramePath { get; }
        public ISettingsValue<string?> EditedColor { get; }
        public ISettingsValue<int> UniformScroll { get; }
        public ISettingsValue<int> UniformDockHeight { get; }
        public ISettingsValue<double> UniformColumnRatio { get; }
        public ISettingsValue<IEnumerable<ViewerPassSelection>?> ViewerPasses { get; }
        public ISettingsValue<RenderSequenceSettings> RenderSequence { get; }

        private readonly FileResourceKey projectSettingsSourceKey;
        private readonly FileResourceKey viewSettingsSourceKey;
        private readonly FileResourceKey uniformsSettingsSourceKey;

        private readonly JsonSettingsFile projectJsonSettingsFile;
        private readonly JsonSettingsFile viewJsonSettingsFile;
        private readonly JsonSettingsFile uniformsJsonSettingsFile;

        private readonly IReadOnlyDictionary<string, ISettingsValue>? previousViewValues;
        private readonly IReadOnlyDictionary<string, ISettingsValue>? previousUniformsValues;

        private readonly Dictionary<string, ISettingsValue> viewValues;
        private readonly Dictionary<string, ISettingsValue> uniformsValues;

        private readonly List<ISettingsValue> settingsValues;

        private readonly IJsonSerializer<bool> boolSerializer;
        private readonly ValueJsonSerializer<int> intSerializer;
        private readonly ValueJsonSerializer<double> doubleSerializer;
        private readonly IJsonSerializer<string?> stringSerializer;
        private readonly IJsonSerializer<IEnumerable<ViewerPassSelection>?> viewerPassSelectionSerializer;
        private readonly RenderSequenceSettingsSerializer renderSequenceSettingsSerializer;

        public ProjectSettings(IFileResource<string> projectSettingsFile, IFileResource<string> viewSettingsFile, IFileResource<string> uniformsSettingsFile) :
            this(projectSettingsFile, viewSettingsFile, uniformsSettingsFile, null)
        {
        }

        private ProjectSettings(IFileResource<string> projectSettingsFile, IFileResource<string> viewSettingsFile, IFileResource<string> uniformsSettingsFile, ProjectSettings? previousProjectSettings)
        {
            var jsonSerializerOptions = new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };

            this.projectSettingsSourceKey = projectSettingsFile.Key;
            this.viewSettingsSourceKey = viewSettingsFile.Key;
            this.uniformsSettingsSourceKey = uniformsSettingsFile.Key;

            this.projectJsonSettingsFile = CreateJsonSettingsFile(projectSettingsFile, jsonSerializerOptions);
            this.uniformsJsonSettingsFile = CreateJsonSettingsFile(uniformsSettingsFile, jsonSerializerOptions); ;
            this.viewJsonSettingsFile = CreateJsonSettingsFile(viewSettingsFile, jsonSerializerOptions); ;

            this.viewValues = new Dictionary<string, ISettingsValue>();
            this.uniformsValues = new Dictionary<string, ISettingsValue>();

            this.settingsValues = new List<ISettingsValue>();

            this.previousViewValues = previousProjectSettings?.viewValues;
            this.previousUniformsValues = previousProjectSettings?.uniformsValues;

            this.boolSerializer = new ValueJsonSerializer<bool>();
            this.intSerializer = new ValueJsonSerializer<int>();
            this.doubleSerializer = new ValueJsonSerializer<double>();
            this.stringSerializer = new ValueJsonSerializer<string?>();
            this.viewerPassSelectionSerializer = NullableJsonSerializer.Create(new ArrayJsonSerializer<ViewerPassSelection>(new ViewerPassSelectionSerializer()!));
            this.renderSequenceSettingsSerializer = new RenderSequenceSettingsSerializer(RenderSequenceSettings.Default);

            this.SaveFramePath = AddSettingsValue(this.viewJsonSettingsFile.Content, this.stringSerializer, nameof(this.SaveFramePath), null!, previousProjectSettings?.SaveFramePath);
            this.EditedColor = AddSettingsValue(this.viewJsonSettingsFile.Content, this.stringSerializer, nameof(this.EditedColor), null!, previousProjectSettings?.EditedColor);
            this.UniformScroll = AddSettingsValue(this.viewJsonSettingsFile.Content, this.intSerializer, nameof(this.UniformScroll), 0, previousProjectSettings?.UniformScroll);
            this.UniformDockHeight = AddSettingsValue(this.viewJsonSettingsFile.Content, this.intSerializer, nameof(this.UniformDockHeight), 200, previousProjectSettings?.UniformDockHeight);
            this.UniformColumnRatio = AddSettingsValue(this.viewJsonSettingsFile.Content, this.doubleSerializer, nameof(this.UniformColumnRatio), 0.4, previousProjectSettings?.UniformColumnRatio);
            this.ViewerPasses = AddSettingsValue(this.viewJsonSettingsFile.Content, this.viewerPassSelectionSerializer, nameof(this.ViewerPasses), null, previousProjectSettings?.ViewerPasses);

            this.RenderSequence = AddSettingsValue(this.viewJsonSettingsFile.Content, this.renderSequenceSettingsSerializer, nameof(this.RenderSequence), RenderSequenceSettings.Default, previousProjectSettings?.RenderSequence);
        }

        public IProjectSettings CreateMergedSettings(IFileResource<string> projectSettingsFile, IFileResource<string> viewSettingsFile, IFileResource<string> uniformsSettingsFile)
        {
            if (Equals(projectSettingsFile.Key, this.projectSettingsSourceKey) &&
                Equals(viewSettingsFile.Key, this.viewSettingsSourceKey) &&
                Equals(uniformsSettingsFile.Key, this.uniformsSettingsSourceKey))
            {
                return this;
            }

            return new ProjectSettings(projectSettingsFile, viewSettingsFile, uniformsSettingsFile, this);
        }

        public ISettingsValue<bool> GetGroupExpandedValue(string name, bool defaultValue)
        {
            name += ".expanded";
            if (!this.viewValues.TryGetValue(name, out var settingsValue))
            {
                settingsValue = new SettingsValue<bool>(this.viewJsonSettingsFile.Content, this.boolSerializer, name, defaultValue, TryGetSettingsValue<bool>(name, this.previousViewValues));
                this.viewValues.Add(name, settingsValue);
            }

            return (ISettingsValue<bool>)settingsValue;
        }

        public ISettingsValue<T> GetUniformValue<T>(IJsonSerializer<T> serializer, string name, T defaultValue)
        {
            var previousSettingValue = TryGetSettingsValue<T>(name, this.uniformsValues) ?? TryGetSettingsValue<T>(name, this.previousUniformsValues);

            var settingsValue = new SettingsValue<T>(this.uniformsJsonSettingsFile.Content, serializer, name, defaultValue, previousSettingValue);
            this.uniformsValues[name] = settingsValue;
            return settingsValue;
        }

        public void Save()
        {
            foreach (var settingsValue in this.uniformsValues.Values.Concat(this.viewValues.Values).Concat(this.settingsValues))
            {
                settingsValue.SaveValue();
            }

            this.uniformsJsonSettingsFile.Content.ClearUnusedValues();
            this.uniformsJsonSettingsFile.Save();

            this.viewJsonSettingsFile.Content.ClearUnusedValues();
            this.viewJsonSettingsFile.Save();

            this.projectJsonSettingsFile.Content.ClearUnusedValues();
            this.projectJsonSettingsFile.Save();
        }

        private SettingsValue<T> AddSettingsValue<T>(IJsonSettings settings, IJsonSerializer<T> serializer, string name, T defaultValue, ISettingsValue<T>? previousValue)
        {
            var settingsValue = new SettingsValue<T>(settings, serializer, name, defaultValue, previousValue);
            this.settingsValues.Add(settingsValue);
            return settingsValue;
        }

        private static ISettingsValue<T>? TryGetSettingsValue<T>(string name, IReadOnlyDictionary<string, ISettingsValue>? settingValues)
        {
            return settingValues != null && settingValues.TryGetValue(name, out var previousValue) ? previousValue as ISettingsValue<T> : null;
        }

        private static JsonSettingsFile CreateJsonSettingsFile(IFileResource<string> resource, JsonSerializerOptions options)
        {
            return new JsonSettingsFile(DeserializeJsonObject(resource, options), resource.Key.AbsolutePath);
        }

        private static JsonObject DeserializeJsonObject(IFileResource<string> resource, JsonSerializerOptions options)
        {
            try
            {
                return !String.IsNullOrEmpty(resource.Value) ? JsonSerializer.Deserialize<JsonObject>(resource.Value, options)! : new JsonObject();
            }
            catch (JsonException e)
            {
                var message = e.Message.Split("Path:")[0];
                var index = (int)e.LineNumber!.Value;
                var line = resource.Value.SplitLines().ElementAt(index);
                throw new SourceLineException(message, new SourceLine(resource, line, index));
            }
        }
    }
}
