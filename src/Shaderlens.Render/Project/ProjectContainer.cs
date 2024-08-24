namespace Shaderlens.Render.Project
{
    public interface IProjectLoadListener
    {
        void OnProjectLoadStarted();
        void OnProjectLoadProgress(IProject project);
        void OnProjectLoadCompleted(IProject project);
    }

    public interface IProjectContainer : IDisposable
    {
        void LoadAsync();
    }

    public class ProjectContainer : IProjectContainer
    {
        private readonly IRenderThread renderThread;
        private readonly IDispatcherThread viewThread;
        private readonly IProjectLoader projectLoader;
        private readonly IFileSystem fileSystem;
        private readonly IFileMonitor fileMonitor;
        private readonly IShaderCache shaderCache;
        private readonly IProjectLoadListener listener;
        private readonly IProjectLoadLogger logger;
        private readonly string projectAbsolutePath;
        private readonly CachingProjectResources resources;

        private IProjectSettings? lastProjectSettings;
        private IProjectUniforms? lastProjectUniforms;
        private bool isLoading;
        private bool isDisposed;

        public ProjectContainer(IRenderThread renderThread, IDispatcherThread viewThread, IProjectLoader projectLoader, IFileSystem fileSystem, IFileMonitor fileMonitor, IShaderCache shaderCache, IProjectLoadListener listener, IProjectLoadLogger logger, string projectAbsolutePath, int maxCachedResources)
        {
            this.renderThread = renderThread;
            this.viewThread = viewThread;
            this.projectLoader = projectLoader;
            this.fileSystem = fileSystem;
            this.fileMonitor = fileMonitor;
            this.shaderCache = shaderCache;
            this.listener = listener;
            this.logger = logger;
            this.projectAbsolutePath = projectAbsolutePath;
            this.resources = new CachingProjectResources(new ProjectResources(renderThread.Access), maxCachedResources);
        }

        public void Dispose()
        {
            this.viewThread.VerifyAccess();

            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;
            if (!this.isLoading)
            {
                this.renderThread.Dispatch(this.resources.Dispose);
            }
        }

        public void LoadAsync()
        {
            this.viewThread.VerifyAccess();

            if (this.isLoading)
            {
                return;
            }

            this.isLoading = true;

            this.listener.OnProjectLoadStarted();

            this.logger.LoadStarted();
            this.logger.SetState("Waiting for render thread...");

            this.renderThread.DispatchAsync(() =>
            {
                if (this.viewThread.Dispatch(() => this.isDisposed))
                {
                    this.resources.Dispose();
                    return;
                }

                this.resources.ResetResourcesUsage();
                this.fileMonitor.Clear();

                var project = this.projectLoader.LoadProject(this.projectAbsolutePath, this.resources, this.fileSystem, this.fileMonitor, this.shaderCache, this.lastProjectSettings, this.lastProjectUniforms, this.listener, this.logger);

                if (project.IsFullyLoaded)
                {
                    this.resources.DisposeUnusedResources();
                }

                this.viewThread.DispatchAsync(() =>
                {
                    if (this.isDisposed)
                    {
                        this.renderThread.DispatchAsync(this.resources.Dispose);
                        return;
                    }

                    if (project.Settings != null)
                    {
                        this.lastProjectSettings = project.Settings;
                    }

                    if (project.Uniforms != null)
                    {
                        this.lastProjectUniforms = project.Uniforms;
                    }

                    this.isLoading = false;
                    this.listener.OnProjectLoadCompleted(project);

                    this.logger.LoadCompleted(project.IsFullyLoaded);
                });
            });
        }
    }
}
