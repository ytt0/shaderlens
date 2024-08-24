namespace Shaderlens.Render.Project
{
    public interface IProjectSourceLoader
    {
        IProjectSource LoadProjectSource(IFileResource<string> projectResource, IFileSystem fileSystem);
        IProjectSource CreateUnloadedProjectSource(IFileResource<string>? projectResource, FileResourceKey projectKey, IFileSystem fileSystem);
    }

    public class ProjectSourceLoader : IProjectSourceLoader
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ProjectSourceLoader()
        {
            this.jsonSerializerOptions = new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };
        }

        public IProjectSource LoadProjectSource(IFileResource<string> projectResource, IFileSystem fileSystem)
        {
            return IsImageOnlyProject(projectResource.Key) ? LoadImageOnlyProject(projectResource, fileSystem) : LoadProject(projectResource, fileSystem);
        }

        public IProjectSource CreateUnloadedProjectSource(IFileResource<string>? projectResource, FileResourceKey projectKey, IFileSystem fileSystem)
        {
            IPassCollection<IProjectPass> passes;

            if (IsImageOnlyProject(projectKey))
            {
                var imageProgramSource = new ProjectProgramSource(projectResource != null ? new[] { projectResource } : new[] { new FileResource<string>(projectKey, String.Empty) });
                var imageProgram = new ProjectPass("Image", new ProjectProgram("Image", imageProgramSource, ChannelBindingSource.Empty, false), null, null, null);

                projectResource = null;
                passes = new PassCollection<IProjectPass>(imageProgram);
            }
            else
            {
                projectResource = new FileResource<string>(projectKey, String.Empty);
                passes = PassCollection.Empty<IProjectPass>();
            }

            return new ProjectSource(false, projectKey.AbsolutePath, projectResource,
                GetDefaultProjectSettingsSource(projectKey.AbsolutePath, fileSystem),
                GetDefaultViewSettingsSource(projectKey.AbsolutePath, fileSystem),
                GetDefaultUniformsSettingsSource(projectKey.AbsolutePath, fileSystem),
                ProjectProgramSource.Empty, Array.Empty<IProjectViewerPass>(), passes, false, null, null);
        }

        private IProjectSource LoadProject(IFileResource<string> projectResource, IFileSystem fileSystem)
        {
            var serializer = JsonSerializerContext.Create(new ProjectSourceJsonSerializer(projectResource.Key.AbsolutePath, fileSystem), projectResource);

            return serializer.Deserialize(DeserializeJsonObject(projectResource));
        }

        private static ProjectSource LoadImageOnlyProject(IFileResource<string> projectResource, IFileSystem fileSystem)
        {
            var imageProgramSource = new ProjectProgramSource(new[] { projectResource });
            var imageProgram = new ProjectPass("Image", new ProjectProgram("Image", imageProgramSource, ChannelBindingSource.Empty, false), null, null, null);

            return new ProjectSource(false, projectResource.Key.AbsolutePath, null,
                GetDefaultProjectSettingsSource(projectResource.Key.AbsolutePath, fileSystem),
                GetDefaultViewSettingsSource(projectResource.Key.AbsolutePath, fileSystem),
                GetDefaultUniformsSettingsSource(projectResource.Key.AbsolutePath, fileSystem),
                ProjectProgramSource.Empty, Array.Empty<IProjectViewerPass>(), new PassCollection<IProjectPass>(imageProgram), false, null, null);
        }

        private JsonObject DeserializeJsonObject(IFileResource<string> resource)
        {
            try
            {
                return !String.IsNullOrEmpty(resource.Value) ? JsonSerializer.Deserialize<JsonObject>(resource.Value, this.jsonSerializerOptions)! : new JsonObject();
            }
            catch (JsonException e)
            {
                var message = e.Message.Split("Path:")[0];
                var index = (int)e.LineNumber!.Value;
                var line = resource.Value.SplitLines().ElementAt(index);
                throw new SourceLineException(message, new SourceLine(resource, line, index));
            }
        }

        private static IFileResource<string> GetDefaultProjectSettingsSource(string projectPath, IFileSystem fileSystem)
        {
            return fileSystem.TryReadText(Path.Combine(".shaderlens", Path.GetFileNameWithoutExtension(projectPath) + ".settings.json"));
        }

        private static IFileResource<string> GetDefaultViewSettingsSource(string projectPath, IFileSystem fileSystem)
        {
            return fileSystem.TryReadText(Path.Combine(".shaderlens", Path.GetFileNameWithoutExtension(projectPath) + ".view.json"));
        }

        private static IFileResource<string> GetDefaultUniformsSettingsSource(string projectPath, IFileSystem fileSystem)
        {
            return fileSystem.TryReadText(Path.GetFileNameWithoutExtension(projectPath) + ".uniforms.json");
        }

        private static bool IsImageOnlyProject(FileResourceKey projectKey)
        {
            var projectFileExtension = Path.GetExtension(projectKey.AbsolutePath).ToLowerInvariant();
            return projectFileExtension == ".glsl" || projectFileExtension == ".cs";
        }
    }
}
