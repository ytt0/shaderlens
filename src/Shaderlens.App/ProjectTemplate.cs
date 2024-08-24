namespace Shaderlens
{
    public interface IProjectTemplateSelectionViewBuilder
    {
        void AddTemplate(IProjectTemplate template, string displayName, string? description, int index);
    }

    public interface IProjectTemplateParametersViewBuilder
    {
        void SetProjectName(string name);
        void SetCreateDirectory(bool createDirectory);
        void AddParameter(string replaceSource, string value);
        void AddParameter(string replaceSource, string displayName, string defaultValue);
    }

    public interface IProjectTemplate
    {
        IEnumerable<string> GetTemplateRelativePaths(string targetProjectName, IReadOnlyDictionary<string, string> parameters);
        string CreateProject(string targetRootPath, string targetProjectName, IReadOnlyDictionary<string, string> parameters);

        void AddTemplateSelectionView(IProjectTemplateSelectionViewBuilder builder);
        void AddTemplateParametersView(IProjectTemplateParametersViewBuilder builder);
    }

    public interface IProjectTemplateLoader
    {
        IProjectTemplate Load(string rootPath);
    }

    public class ProjectTemplateException : Exception
    {
        public ProjectTemplateException(string message, Exception? innerException = null) :
            base(message, innerException)
        {
        }

        public override string ToString()
        {
            return this.Message;
        }
    }

    public class ProjectTemplateLoader : IProjectTemplateLoader
    {
        private interface IParameter
        {
            void AddParameterView(IProjectTemplateParametersViewBuilder builder);
        }

        private class StringParameter : IParameter
        {
            private readonly string replaceSource;
            private readonly string displayName;
            private readonly string defaultValue;

            public StringParameter(string replaceSource, string displayName, string defaultValue)
            {
                this.replaceSource = replaceSource;
                this.displayName = displayName;
                this.defaultValue = defaultValue;
            }

            public void AddParameterView(IProjectTemplateParametersViewBuilder builder)
            {
                builder.AddParameter(this.replaceSource, this.displayName, this.defaultValue);
            }
        }

        private class GuidParameter : IParameter
        {
            private readonly string replaceSource;

            public GuidParameter(string replaceSource)
            {
                this.replaceSource = replaceSource;
            }

            public void AddParameterView(IProjectTemplateParametersViewBuilder builder)
            {
                builder.AddParameter(this.replaceSource, Guid.NewGuid().ToString().ToUpper());
            }
        }

        private class ApplicationPathParameter : IParameter
        {
            private readonly string replaceSource;

            public ApplicationPathParameter(string replaceSource)
            {
                this.replaceSource = replaceSource;
            }

            public void AddParameterView(IProjectTemplateParametersViewBuilder builder)
            {
                builder.AddParameter(this.replaceSource, Path.ChangeExtension(Assembly.GetEntryAssembly()!.Location, "exe"));
            }
        }

        private class CSharpEnvironmentPathParameter : IParameter
        {
            private readonly string replaceSource;

            public CSharpEnvironmentPathParameter(string replaceSource)
            {
                this.replaceSource = replaceSource;
            }

            public void AddParameterView(IProjectTemplateParametersViewBuilder builder)
            {
                builder.AddParameter(this.replaceSource, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Resources", "CSharpEnvironment"));
            }
        }

        private class Template : IProjectTemplate
        {
            private readonly string displayName;
            private readonly string? description;
            private readonly int index;
            private readonly string defaultProjectName;
            private readonly bool defaultCreateDirectory;
            private readonly IEnumerable<IParameter> parameters;
            private readonly string projectNameReplace;
            private readonly string sourceRootPath;
            private readonly string sourceProjectFilePath;
            private readonly IEnumerable<string> sourceFilesPaths;
            private readonly HashSet<string> replaceContentExtensions;

            public Template(string displayName, string? description, int index, string projectName, string projectNameReplace, bool createDirectory, IEnumerable<IParameter> parameters, string rootPath, string projectFilePath, IEnumerable<string> contentFilePaths, IEnumerable<string> replaceContentExtensions)
            {
                this.displayName = displayName;
                this.description = description;
                this.index = index;
                this.defaultProjectName = projectName;
                this.defaultCreateDirectory = createDirectory;
                this.parameters = parameters;

                this.projectNameReplace = projectNameReplace;
                this.sourceRootPath = rootPath;
                this.sourceProjectFilePath = projectFilePath;
                this.sourceFilesPaths = new[] { projectFilePath }.Concat(contentFilePaths).ToArray();
                this.replaceContentExtensions = new HashSet<string>(replaceContentExtensions);
            }

            public IEnumerable<string> GetTemplateRelativePaths(string targetProjectName, IReadOnlyDictionary<string, string> parameters)
            {
                return this.sourceFilesPaths.Select(sourcePath => GetTargetPath(sourcePath, targetProjectName, parameters)).ToArray();
            }

            public string CreateProject(string targetRootPath, string targetProjectName, IReadOnlyDictionary<string, string> parameters)
            {
                var mapping = this.sourceFilesPaths.Select(sourcePath => new KeyValuePair<string, string>(sourcePath, Path.GetFullPath(Path.Combine(targetRootPath, GetTargetPath(sourcePath, targetProjectName, parameters))))).ToDictionary();

                foreach (var targetPath in mapping.Values)
                {
                    if (!Path.IsPathFullyQualified(targetPath))
                    {
                        throw new Exception($"Failed to create project, file path format is invalid {targetPath}");
                    }

                    if (!Path.GetFullPath(targetPath).StartsWith(Path.GetFullPath(targetRootPath).TrimEnd('\\') + '\\'))
                    {
                        throw new Exception($"Failed to create project, file {targetPath} is expected to be under {targetRootPath}");
                    }

                    if (Path.Exists(targetPath))
                    {
                        throw new Exception($"Failed to create project, file already exists {targetPath}");
                    }
                }

                foreach (var targetDirectory in mapping.Values.Select(Path.GetDirectoryName).Distinct())
                {
                    Directory.CreateDirectory(targetDirectory!);
                }

                foreach (var pair in mapping)
                {
                    var sourcePath = pair.Key;
                    var targetPath = pair.Value;

                    var fileExtension = Path.GetExtension(sourcePath).TrimStart('.').ToLowerInvariant();
                    if (this.replaceContentExtensions.Contains(fileExtension))
                    {
                        var content = File.ReadAllText(sourcePath);
                        content = SetParametersValues(content, targetProjectName, EscapeValues(parameters, fileExtension));
                        File.WriteAllText(targetPath, content);
                    }
                    else
                    {
                        File.Copy(sourcePath, targetPath, false);
                    }
                }

                return mapping[this.sourceProjectFilePath];
            }

            public void AddTemplateSelectionView(IProjectTemplateSelectionViewBuilder builder)
            {
                builder.AddTemplate(this, this.displayName, this.description, this.index);
            }

            public void AddTemplateParametersView(IProjectTemplateParametersViewBuilder builder)
            {
                builder.SetProjectName(this.defaultProjectName);
                builder.SetCreateDirectory(this.defaultCreateDirectory);

                foreach (var parameter in this.parameters)
                {
                    parameter.AddParameterView(builder);
                }
            }

            private string GetTargetPath(string sourcePath, string targetProjectName, IReadOnlyDictionary<string, string> parameters)
            {
                var sourceRelativePath = Path.GetRelativePath(this.sourceRootPath, sourcePath);
                var targetRelativePath = SetParametersValues(sourceRelativePath, targetProjectName, parameters);
                return targetRelativePath;
            }

            private string SetParametersValues(string source, string targetProjectName, IReadOnlyDictionary<string, string> parameters)
            {
                var target = source;

                target = target.Replace(this.projectNameReplace, targetProjectName);

                foreach (var pair in parameters)
                {
                    target = target.Replace(pair.Key, pair.Value);
                }

                return target;
            }

            private static IReadOnlyDictionary<string, string> EscapeValues(IReadOnlyDictionary<string, string> parameters, string fileExtension)
            {
                switch (fileExtension)
                {
                    case "json": return parameters.Select(pair => new KeyValuePair<string, string>(pair.Key, JsonSerializer.Serialize(pair.Value).Trim('"'))).ToDictionary();
                }

                return parameters;
            }
        }

        private const string TemplateFileName = "Template.json";
        private static readonly IEnumerable<string> DefaultContentExtensions = new[] { "json" };
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };

        public IProjectTemplate Load(string rootPath)
        {
            if (!Path.IsPathFullyQualified(rootPath))
            {
                throw new Exception("An absolute path is expected");
            }

            var templateFilePath = Path.Combine(rootPath, TemplateFileName);

            try
            {
                if (!File.Exists(templateFilePath))
                {
                    throw new ProjectTemplateException($"Template definition file not found");
                }

                var source = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(templateFilePath), SerializerOptions);

                var displayName = ReadStringProperty(source, "DisplayName");
                var description = TryReadStringProperty(source, "Description");
                var projectName = ReadStringProperty(source, "ProjectName");
                var projectNameReplace = ReadStringProperty(source, "ProjectNameReplace");
                var projectFile = ReadStringProperty(source, "ProjectFile");

                var createDirectory = source?["CreateDirectory"]?.GetValueKind() != JsonValueKind.False;

                if (source?["Index"]?.TryGetIntValue(out var index) != true)
                {
                    index = Int32.MaxValue;
                }

                var projectFilePath = Path.Combine(rootPath, projectFile!);
                if (!File.Exists(projectFilePath))
                {
                    throw new ProjectTemplateException($"Project file is not found at {projectFilePath}");
                }

                var replaceContentExtensions = source?["ReplaceContentExtensions"]?.GetValueKind() == JsonValueKind.Array ?
                    source["ReplaceContentExtensions"]!.AsArray().Select(item => item?.GetValueKind() == JsonValueKind.String ? item.GetValue<string>().TrimStart('.').ToLowerInvariant() : null).OfType<string>().ToArray() :
                    DefaultContentExtensions;

                if (source?["FileCount"]?.TryGetIntValue(out var fileCount) != true)
                {
                    fileCount = 100;
                }

                var contentPaths = Directory.GetFiles(rootPath, "*", new EnumerationOptions { RecurseSubdirectories = true, MaxRecursionDepth = 3 });
                contentPaths = contentPaths.Where(path => path != templateFilePath && path != projectFilePath).ToArray();

                if (contentPaths.Length > fileCount)
                {
                    throw new ProjectTemplateException($"Template contains {contentPaths.Length} files, the limit is {fileCount} files, set the \"FileCount\" property to allow more files");
                }

                var parameters = ReadParameters(source?["Parameters"]);

                return new Template(displayName, description, index, projectName, projectNameReplace, createDirectory, parameters, rootPath, projectFilePath, contentPaths, replaceContentExtensions);
            }
            catch (ProjectTemplateException e)
            {
                throw new ProjectTemplateException($"Failed to load project template at:{Environment.NewLine}{templateFilePath}{Environment.NewLine}{e.Message}", e);
            }
            catch (JsonException e)
            {
                throw new ProjectTemplateException($"Failed to load project template at:{Environment.NewLine}{templateFilePath}{Environment.NewLine}Template deserialization failed:{Environment.NewLine}{e.Message}", e);
            }
        }

        private static IParameter[] ReadParameters(JsonNode? source)
        {
            if (source == null)
            {
                return Array.Empty<IParameter>();
            }

            if (source.GetValueKind() != JsonValueKind.Array)
            {
                throw new ProjectTemplateException("Template \"Parameters\" value is expected to be an array");
            }

            return source.AsArray().Select(ReadParameter).ToArray();
        }

        private static IParameter ReadParameter(JsonNode? source, int index)
        {
            if (source?["Type"] == null || !source["Type"]!.TryGetStringValue(out var type))
            {
                type = null;
            }

            switch (type)
            {
                case "ApplicationPath":
                    return ReadApplicationPathParameter(source, index);
                case "CSharpEnvironmentPath":
                    return ReadCSharpEnvironmentPathParameter(source, index);
                case "Guid":
                    return ReadGuidParameter(source, index);
                case null:
                case "String":
                    return ReadStringParameter(source, index);
            }

            throw new ProjectTemplateException($"Template parameter type \"{type}\" is not supported, supported types are \"String\", \"Guid\", \"ApplicationPath\", \"CSharpEnvironmentPath\"");
        }

        private static GuidParameter ReadGuidParameter(JsonNode? source, int index)
        {
            if (source?["Replace"]?.TryGetStringValue(out var replaceSource) != true)
            {
                throw new ProjectTemplateException($"Template parameter {index} \"Replace\" property is missing");
            }

            return new GuidParameter(replaceSource!);
        }

        private static ApplicationPathParameter ReadApplicationPathParameter(JsonNode? source, int index)
        {
            if (source?["Replace"]?.TryGetStringValue(out var replaceSource) != true)
            {
                throw new ProjectTemplateException($"Template parameter {index} \"Replace\" property is missing");
            }

            return new ApplicationPathParameter(replaceSource!);
        }

        private static CSharpEnvironmentPathParameter ReadCSharpEnvironmentPathParameter(JsonNode? source, int index)
        {
            if (source?["Replace"]?.TryGetStringValue(out var replaceSource) != true)
            {
                throw new ProjectTemplateException($"Template parameter {index} \"Replace\" property is missing");
            }

            return new CSharpEnvironmentPathParameter(replaceSource!);
        }

        private static StringParameter ReadStringParameter(JsonNode? source, int index)
        {
            if (source?["DisplayName"]?.TryGetStringValue(out var displayName) != true)
            {
                throw new ProjectTemplateException($"Template parameter {index} \"DisplayName\" property is missing");
            }

            if (source?["Value"]?.TryGetStringValue(out var defaultValue) != true)
            {
                throw new ProjectTemplateException($"Template parameter {index} ({displayName}) \"Value\" property is missing");
            }

            if (source?["Replace"]?.TryGetStringValue(out var replaceSource) != true)
            {
                throw new ProjectTemplateException($"Template parameter {index} ({displayName}) \"Replace\" property is missing");
            }

            return new StringParameter(replaceSource!, displayName!, defaultValue!);
        }

        private static string ReadStringProperty(JsonNode? source, string propertyName)
        {
            if (source?[propertyName]?.TryGetStringValue(out var result) != true)
            {
                throw new ProjectTemplateException($"Template \"{propertyName}\" property is missing");
            }

            return result!;
        }

        private static string? TryReadStringProperty(JsonNode? source, string propertyName)
        {
            if (source?[propertyName]?.TryGetStringValue(out var result) != true)
            {
                result = null;
            }

            return result;
        }
    }
}
