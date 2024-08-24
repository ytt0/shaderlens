namespace Shaderlens.Render.Project
{
    public interface IProjectLoader
    {
        IProject LoadProject(string projectPath, IProjectResources resources, IFileSystem fileSystem, IFileMonitor fileMonitor, IShaderCache shaderCache, IProjectSettings? previousSettings, IProjectUniforms? previousUniforms, IProjectLoadListener listener, IProjectLoadLogger logger);
    }

    public class ProjectLoader : IProjectLoader
    {
        private class LoggingFileSystem : IFileSystem
        {
            private readonly IFileSystem fileSystem;
            private readonly IProjectLoadLogger logger;

            public LoggingFileSystem(IFileSystem fileSystem, IProjectLoadLogger logger)
            {
                this.fileSystem = fileSystem;
                this.logger = logger;
            }

            public FileResourceKey GetFileKey(string path)
            {
                return this.fileSystem.GetFileKey(path);
            }

            public IEnumerable<string> GetSequencePaths(string pattern)
            {
                this.logger.SetState("Listing", pattern);
                return this.fileSystem.GetSequencePaths(pattern);
            }

            public IFileResource<string> ReadText(FileResourceKey key)
            {
                this.logger.SetState("Loading", key.AbsolutePath);
                return this.fileSystem.ReadText(key);
            }

            public IFileResource<byte[]> ReadBytes(FileResourceKey key)
            {
                this.logger.SetState("Loading", key.AbsolutePath);
                return this.fileSystem.ReadBytes(key);
            }
        }

        private readonly IDispatcherThread renderThread;
        private readonly IProjectSourceLoader projectSourceLoader;
        private readonly IHashSource hashSource;
        private readonly ICSharpTransformer csharpTransformer;
        private readonly IPipelineLoader renderPipelineLoader;

        public ProjectLoader(IDispatcherThread renderThread, IProjectSourceLoader projectSourceLoader, IPipelineLoader renderPipelineLoader, IHashSource hashSource, ICSharpTransformer csharpTransformer)
        {
            this.renderThread = renderThread;
            this.projectSourceLoader = projectSourceLoader;
            this.renderPipelineLoader = renderPipelineLoader;
            this.hashSource = hashSource;
            this.csharpTransformer = csharpTransformer;
        }

        public IProject LoadProject(string projectPath, IProjectResources resources, IFileSystem fileSystem, IFileMonitor fileMonitor, IShaderCache shaderCache, IProjectSettings? previousSettings, IProjectUniforms? previousUniforms, IProjectLoadListener listener, IProjectLoadLogger logger)
        {
            IProjectSource? source = null;
            IProjectSettings? settings = null;
            IRenderPipeline? pipeline = null;
            IProjectUniforms? uniforms = null;

            if (!Path.IsPathFullyQualified(projectPath))
            {
                throw new Exception($"Absolute path is expected - {projectPath}");
            }

            var rootPath = Path.GetDirectoryName(projectPath)!;
            var pathResolver = new PathResolver(EnvironmentVariablesSource.Instance, rootPath);

            //fileSystem = new DelayFileSystem(fileSystem, 1);
            fileSystem = new LoggingFileSystem(fileSystem, logger);
            fileSystem = new ProjectLoaderFileSystem(fileSystem, pathResolver, resources, fileMonitor);

            IFileResource<string>? projectResource = null;

            try
            {
                projectResource = fileSystem.ReadText(projectPath);

                logger.SetState("Loading project source");
                source = this.projectSourceLoader.LoadProjectSource(projectResource, fileSystem);

                logger.SetState("Loading project settings");
                settings = LoadProjectSettings(source, previousSettings);

                logger.SetState("Creating uniforms");
                uniforms = CreateUniforms(source, settings, previousSettings, previousUniforms);

                listener.OnProjectLoadProgress(new Project(source, settings, null, uniforms));

                pipeline = this.renderPipelineLoader.LoadPipeline(source, uniforms.Uniforms, resources, shaderCache, fileSystem, logger);
            }
            catch (ProjectLoadException)
            {
            }
            catch (Exception e)
            {
                var sourceLine = (e as SourceLineException)?.SourceLine;
                if (sourceLine != null)
                {
                    logger.AddFailure(e.ToString(), sourceLine.Value);
                }
                else
                {
                    logger.AddFailure(e.ToString());
                }
            }

            if (source == null)
            {
                source = this.projectSourceLoader.CreateUnloadedProjectSource(projectResource, projectResource?.Key ?? fileSystem.GetFileKey(projectPath), fileSystem);
            }

            return new Project(source, settings, pipeline, uniforms);
        }

        private IProjectUniforms CreateUniforms(IProjectSource project, IProjectSettings settings, IProjectSettings? previousSettings, IProjectUniforms? previousUniforms)
        {
            var uniformsBuilder = new ProjectUniformsBuilder(this.renderThread, this.hashSource);

            AddUniformsSources(uniformsBuilder, project.Common);

            foreach (var pass in project.Passes)
            {
                AddUniformsSources(uniformsBuilder, pass.Program.Source);
            }

            foreach (var viewer in project.Viewers)
            {
                AddUniformsSources(uniformsBuilder, viewer.Program.Source);
            }

            if (settings == previousSettings && uniformsBuilder.GetUniformsKey() == previousUniforms?.Key)
            {
                return previousUniforms;
            }

            return uniformsBuilder.CreateProjectUniforms(settings);
        }

        private void AddUniformsSources(ProjectUniformsBuilder target, IProjectProgramSource? programSource)
        {
            if (programSource != null)
            {
                foreach (var source in programSource)
                {
                    target.AddUniforms(GetSourceLines(source));
                }
            }
        }

        private SourceLines GetSourceLines(IFileResource<string> resource)
        {
            var sourceLines = new SourceLines(resource);

            if (Path.GetExtension(resource.Key.AbsolutePath)!.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
            {
                sourceLines = this.csharpTransformer.Transform(sourceLines);
            }

            return sourceLines;
        }

        private static IProjectSettings LoadProjectSettings(IProjectSource project, IProjectSettings? previousSettings)
        {
            return previousSettings != null ?
                previousSettings.CreateMergedSettings(project.Settings!, project.ViewSettings!, project.UniformsSettings!) :
                new ProjectSettings(project.Settings!, project.ViewSettings!, project.UniformsSettings!);
        }
    }
}
