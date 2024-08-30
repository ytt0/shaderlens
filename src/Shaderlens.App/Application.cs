using Microsoft.Win32;

namespace Shaderlens
{
    using static WinApi;

    public interface IApplication
    {
        string? ProjectPath { get; }
        bool IsPartiallyLoaded { get; }
        bool IsFullyLoaded { get; }
        bool IsPaused { get; }

        int FrameRate { get; set; }
        double Speed { get; set; }
        int RenderDownscale { get; set; }
        int ViewerBufferIndex { get; set; }
        int ViewerBuffersCount { get; }
        ViewerPassSelection ViewerPass { get; set; }

        bool AlwaysOnTop { get; set; }
        bool AutoReload { get; set; }
        bool RestartOnAutoReload { get; set; }
        bool ClearStateOnRestart { get; set; }
        bool PauseOnInactivity { get; set; }
        bool RenderInputEventsWhenPaused { get; set; }
        bool WrapShaderInputCursor { get; set; }
        bool EnableShaderCache { get; set; }
        bool DarkTheme { get; set; }

        CopySelection? CopySelection { get; }

        IEnumerable<string> PinnedProjects { get; }
        IEnumerable<string> RecentProjects { get; }
        string SettingsPath { get; }
        string InputsPath { get; }
        string? ThemePath { get; }
        IEnumerable<ICopyFormatter> CopyFormatters { get; }
        IClipboard Clipboard { get; }
        string? SaveFramePath { get; }
        Exception? UnhandledException { get; }
        string? RenderInformation { get; }

        void Start(IntPtr viewportViewHandle);
        void SetKeyState(int index, bool isDown);
        void ClearKeysDownState();
        void SetViewportSize(int width, int height);
        void SetMouseState(bool isDown);
        void StepPipeline();
        void RestartPipeline();
        void PausePipeline();
        void ResumePipeline();
        void RenderFrame();
        void RenderViewerFrame();
        void CreateProject();
        void OpenProject();
        void OpenProject(string projectPath);
        void SaveProject();
        void ReloadProject();
        void ShowStartPage();
        void ShowViewport();
        void ToggleUniformsView();
        void CopyRepeat(ICopySource copySource);
        void CopyValue(ICopySource copySource, ICopyFormatter copyFormatter);
        void CopyFrame(ICopySource copySource, bool includeAlphaChannel);
        void ExportFrame();
        void ExportFrameRepeat();
        void ExportSequence();
        void SetProjectChanged();
        void SetViewerTransform(double scale, double offsetX, double offsetY);
        void Help();
        bool ShowProjectCloseDialog();
        void OpenExternalPath(string filePath);
        ICopySource GetCopySource();
        ITextureBuffer? GetSelectedBufferTexture();
        ITextureBuffer? GetViewportBufferTexture();
        IEnumerable<IProjectTemplate> GetProjectTemplates();
        double GetDragSensitivity(Window window);
    }

    public class Application : IApplication
    {
        private class ProjectLoadListener : IProjectLoadListener
        {
            private readonly Application application;

            public ProjectLoadListener(Application application)
            {
                this.application = application;
            }

            public void OnProjectLoadStarted()
            {
                this.application.OnProjectLoadStarted();
            }

            public void OnProjectLoadProgress(IProject project)
            {
                this.application.viewThread.DispatchAsync(() => this.application.OnProjectLoadProgress(project));
            }

            public void OnProjectLoadCompleted(IProject project)
            {
                this.application.OnProjectLoadCompleted(project);
            }
        }

        private const string ProjectFileDialogGuid = "79ef45b6-8c8f-49a4-8f4d-94d37b0d9543";
        private const string ProjectFolderDialogGuid = "6b90ccd2-ab5c-4e1e-a426-5e3519148d51";
        private const string FrameDialogGuid = "eedc5601-3586-4a68-b792-109ff237082f";

        public string? ProjectPath { get { return this.project?.Source.Path; } }
        public bool IsPartiallyLoaded { get { return this.project != null; } }
        public bool IsFullyLoaded { get { return this.project?.IsFullyLoaded == true; } }
        public bool IsPaused { get; private set; }

        private int frameRate;
        public int FrameRate
        {
            get { return this.frameRate; }
            set
            {
                if (this.frameRate != value)
                {
                    this.frameRate = value;
                    this.renderThread.SetFrameRate(this.frameRate);
                }
            }
        }

        private double speed;
        public double Speed
        {
            get { return this.speed; }
            set
            {
                if (this.speed != value)
                {
                    this.speed = value;
                    this.frameIndexSource.SetSpeed(this.speed);
                }
            }
        }

        private int renderDownscale;
        public int RenderDownscale
        {
            get { return this.renderDownscale; }
            set
            {
                if (this.renderDownscale != value)
                {
                    this.renderDownscale = value;
                    this.pipeline?.SetRenderDownscale(this.renderDownscale);
                    this.viewportView.SetViewerBufferSize(this.pipeline?.ViewerBufferWidth ?? 0, this.pipeline?.ViewerBufferHeight ?? 0);
                    this.renderThread.RenderFrame();
                }
            }
        }

        private int viewerBufferIndex;
        public int ViewerBufferIndex
        {
            get { return this.viewerBufferIndex; }
            set
            {
                if (this.viewerBufferIndex != value)
                {
                    this.viewerBufferIndex = Math.Clamp(value, 0, this.ViewerBuffersCount - 1);
                    this.pipeline?.SetViewerBufferIndex(this.viewerBufferIndex);
                    this.pipeline?.SetViewerPass(this.viewerPasses![this.ViewerBufferIndex]);
                    this.viewportView.SetViewerBufferSize(this.pipeline?.ViewerBufferWidth ?? 0, this.pipeline?.ViewerBufferHeight ?? 0);
                    this.renderThread.RenderViewerFrame();
                }
            }
        }

        public int ViewerBuffersCount { get; private set; }

        private ViewerPassSelection[]? viewerPasses;
        public ViewerPassSelection ViewerPass
        {
            get { return this.viewerPasses != null ? this.viewerPasses[this.ViewerBufferIndex] : this.settings.DefaultViewerPass; }
            set
            {
                if (this.viewerPasses != null && !Equals(this.viewerPasses[this.viewerBufferIndex], value))
                {
                    this.viewerPasses[this.ViewerBufferIndex] = value;

                    if (this.projectSettings != null)
                    {
                        this.projectSettings.ViewerPasses.Value = this.viewerPasses.ToArray();
                    }

                    this.pipeline?.SetViewerPass(this.viewerPasses[this.ViewerBufferIndex]);
                    this.renderThread.RenderViewerFrame();
                }
            }
        }

        public bool AlwaysOnTop
        {
            get { return this.settings.AlwaysOnTop; }
            set
            {
                this.settings.AlwaysOnTop = value;
                this.viewportWindow.Topmost = this.AlwaysOnTop && this.project != null;
            }
        }

        public bool AutoReload
        {
            get { return this.settings.AutoReload; }
            set { this.settings.AutoReload = value; }
        }

        public bool RestartOnAutoReload
        {
            get { return this.settings.RestartOnAutoReload; }
            set { this.settings.RestartOnAutoReload = value; }
        }

        public bool ClearStateOnRestart
        {
            get { return this.settings.ClearStateOnRestart; }
            set { this.settings.ClearStateOnRestart = value; }
        }

        public bool PauseOnInactivity
        {
            get { return this.settings.PauseOnInactivity; }
            set
            {
                this.settings.PauseOnInactivity = value;
                SetInactivityTimer();
            }
        }

        public bool RenderInputEventsWhenPaused
        {
            get { return this.settings.RenderInputEventsWhenPaused; }
            set { this.settings.RenderInputEventsWhenPaused = value; }
        }

        public bool WrapShaderInputCursor
        {
            get { return this.settings.WrapShaderInputCursor; }
            set { this.settings.WrapShaderInputCursor = value; }
        }

        public bool EnableShaderCache
        {
            get { return this.settings.EnableCache; }
            set { this.settings.EnableCache = value; }
        }

        public bool DarkTheme
        {
            get { return this.settings.DarkTheme; }
            set
            {
                this.settings.DarkTheme = value;
                this.theme = value ? this.darkTheme : this.lightTheme;

                this.startPageView?.SetTheme(this.theme);
                this.viewportView.SetTheme(this.theme);
                this.uniformsView.SetTheme(this.theme);

                this.projectCreationView?.SetTheme(this.theme);
                this.renderSequenceView?.SetTheme(this.theme);
            }
        }

        public CopySelection? CopySelection { get; private set; }

        public IEnumerable<string> PinnedProjects
        {
            get { return this.settings.PinnedProjects; }
        }

        public IEnumerable<string> RecentProjects
        {
            get { return this.settings.RecentProjects; }
        }

        public string SettingsPath { get; }
        public string InputsPath { get; }
        public string? ThemePath { get { return this.DarkTheme ? this.darkTheme.Path : this.lightTheme.Path; } }

        public IClipboard Clipboard { get; }

        public IEnumerable<ICopyFormatter> CopyFormatters { get { return this.settings.CopyFormatters; } }

        public string? SaveFramePath { get; private set; }

        public Exception? UnhandledException { get; private set; }
        public string? RenderInformation { get; private set; }

        private readonly System.Windows.Application application;

        private readonly IApplicationInputs inputs;
        private readonly IApplicationTheme darkTheme;
        private readonly IApplicationTheme lightTheme;
        private readonly IApplicationCommands commands;
        private readonly ViewDispatcherThread viewThread;
        private readonly DispatcherThread storageThread;
        private readonly ViewportView viewportView;
        private readonly UniformsView uniformsView;

        private readonly IHashSource hashSource;
        private readonly ICSharpTransformer csharpTransformer;
        private readonly TextureWriterFactory textureWriterFactory;
        private readonly ProjectLoadListener loadListener;
        private readonly IShaderStatusParser shaderStatusParser;
        private readonly FrameIndexSource frameIndexSource;
        private readonly FileSystem fileSystem;
        private readonly FileMonitor fileMonitor;
        private readonly ProgramSourceFactory programSourceFactory;
        private readonly IFrameRateCounter counter;
        private readonly DispatcherTimer inactivityTimer;
        private readonly Window viewportWindow;
        private readonly Window uniformsWindow;
        private readonly OpenFileDialog projectOpenFileDialog;
        private readonly OpenFolderDialog projectOpenFolderDialog;
        private readonly SaveFileDialog frameSaveFileDialog;
        private readonly IApplicationSettings settings;
        private readonly long stepIncrement;

        private Window? startPageWindow;
        private StartPageView? startPageView;
        private Window? projectCreationWindow;
        private ProjectCreationView? projectCreationView;
        private Window? exportWindow;
        private RenderSequenceView? renderSequenceView;
        [AllowNull]
        private RenderThread renderThread;
        private ShaderCompiler? shaderCompiler;
        private IProjectLoader? projectLoader;
        private TextureReaderFactory? textureReaderFactory;
        private KeyboardTextureResource? keyboardTextureResource;
        private QuadVertexArrayResource? vertexArray;
        private MouseStateSource? mouseStateSource;

        private ProjectContainer? projectContainer;
        private IProjectSettings? projectSettings;
        private IProjectUniforms? projectUniforms;
        private IRenderPipeline? pipeline;

        private IApplicationTheme theme;
        private IProject? project;
        private string? projectPath;
        private string? projectName;
        private int viewportWidth;
        private int viewportHeight;
        private bool resumeOnLoadCompletes;
        private uint lastInputTimestamp;
        private DateTime lastInputTime;
        private DateTime lastVisibleTime;
        private IEnumerable<IProjectTemplate>? projectTempates;
        private bool isProjectChanged;
        private bool isProjectChangeConfirmed;
        private long lastStepTime;

        public Application(System.Windows.Application application, IApplicationSettings settings, IApplicationInputs inputs, IApplicationTheme darkTheme, IApplicationTheme lightTheme)
        {
            this.application = application;
            this.settings = settings;
            this.inputs = inputs;
            this.darkTheme = darkTheme;
            this.lightTheme = lightTheme;

            this.theme = this.settings.DarkTheme ? this.darkTheme : this.lightTheme;
            this.commands = new ApplicationCommands(this, inputs);

            this.viewThread = new ViewDispatcherThread(this.application.Dispatcher);
            this.storageThread = new DispatcherThread("storage");

            this.hashSource = Sha256HashSource.Instance;
            this.shaderStatusParser = ShaderStatusParser.Instance;
            this.frameIndexSource = new FrameIndexSource();

            this.fileSystem = new FileSystem();
            this.fileMonitor = new FileMonitor(this.viewThread, this.fileSystem);
            this.fileMonitor.Changed += OnProjectFileChanged;
            this.programSourceFactory = new ProgramSourceFactory(this.hashSource,
                CreateSourceLines(this.settings.VertexHeader + Environment.NewLine, nameof(this.settings.VertexHeader) + " (settings)"),
                CreateSourceLines(this.settings.FragmentHeader + Environment.NewLine, nameof(this.settings.FragmentHeader) + " (settings)"));
            this.csharpTransformer = CSharpTransformer.Instance;
            this.textureWriterFactory = new TextureWriterFactory();

            this.loadListener = new ProjectLoadListener(this);

            this.viewportWindow = new Window();
            this.uniformsWindow = new Window { ShowInTaskbar = false };
            this.uniformsWindow.Closing += OnWindowClosing;

            this.projectOpenFileDialog = new OpenFileDialog
            {
                ClientGuid = new Guid(ProjectFileDialogGuid),
                AddToRecent = false,
                DefaultExt = ".json",
                Filter = String.Join("|",
                    FormatFilterItem("All Supported Files", "json", "glsl", "cs"),
                    FormatFilterItem("Project Files", "json"),
                    FormatFilterItem("GLSL Files", "glsl"),
                    FormatFilterItem("C# Files", "cs")),
                FilterIndex = this.settings.ProjectOpenFileDialogFilterIndex,
            };

            this.projectOpenFileDialog.FileOk += (sender, e) => this.settings.ProjectOpenFileDialogFilterIndex = this.projectOpenFileDialog.FilterIndex;

            this.projectOpenFolderDialog = new OpenFolderDialog
            {
                AddToRecent = false,
                ClientGuid = new Guid(ProjectFolderDialogGuid),
            };

            this.frameSaveFileDialog = new SaveFileDialog
            {
                ClientGuid = new Guid(FrameDialogGuid),
                AddToRecent = false,
                DefaultExt = ".png",
                Filter = String.Join("|",
                    FormatFilterItem("All Supported Files", "png", "jpg", "bmp", "tiff"),
                    FormatFilterItem("PNG Files", "png"),
                    FormatFilterItem("JPEG Files", "jpg"),
                    FormatFilterItem("Bitmap Files", "bmp"),
                    FormatFilterItem("TIFF Files", "tiff")),
                FilterIndex = this.settings.FrameSaveFileDialogFilterIndex,
            };

            this.frameSaveFileDialog.FileOk += (sender, e) => this.settings.FrameSaveFileDialogFilterIndex = this.frameSaveFileDialog.FilterIndex;

            this.application.DispatcherUnhandledException += (sender, e) =>
            {
                this.UnhandledException = e.Exception;
                application.Shutdown();
                e.Handled = true;
            };

            application.Exit += OnExit;

            this.viewportWindow.SourceInitialized += (sender, e) =>
            {
                this.application.MainWindow = this.viewportWindow;
                this.uniformsWindow.Owner = this.viewportWindow;
            };

            this.viewportView = new ViewportView(this.viewportWindow, this, this.settings, this.inputs, this.commands, this.theme, this.viewThread);
            this.uniformsView = new UniformsView(this.uniformsWindow, this, this.settings, this.commands, this.theme);

            this.counter = new FrameRateCounter(this.viewportView.FrameRateTarget, TimeSpan.FromSeconds(0.5));

            this.inactivityTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            this.inactivityTimer.Tick += (sender, e) => OnInactivityTimerTick();

            this.stepIncrement = TimeSpan.FromSeconds(this.settings.StepIncrementSeconds).Ticks;

            this.renderDownscale = 1;
            this.frameRate = 1;
            this.speed = 1;
            this.viewerBufferIndex = -1;
            this.viewerPasses = Array.Empty<ViewerPassSelection>();

            this.SettingsPath = settings.Path!;
            this.InputsPath = inputs.Path;

            this.Clipboard = new Shaderlens.Presentation.Clipboard();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            if (this.UnhandledException == null)
            {
                this.settings.Save();
            }

            this.renderThread?.Stop();
            this.renderThread?.Dispose();
            this.storageThread.Stop();
            this.storageThread.Dispose();
        }

        public void Start(IntPtr viewportViewHandle)
        {
            this.renderThread = new RenderThread(viewportViewHandle, this.frameIndexSource, this.counter, "render");
            this.renderThread.Start();

            this.RenderInformation = this.renderThread.RenderInformation;

            this.renderThread.Dispatch(() =>
            {
                this.keyboardTextureResource = new KeyboardTextureResource(this.renderThread.Access);
                this.vertexArray = new QuadVertexArrayResource(this.renderThread.Access);
                this.mouseStateSource = new MouseStateSource(this.renderThread.Access, this.viewportView.MousePositionSource);
            });

            this.shaderCompiler = new ShaderCompiler(this.renderThread.Access, this.shaderStatusParser);
            this.textureReaderFactory = new TextureReaderFactory(this.renderThread.Access);

            var viewerSourceLines = new ViewerSourceLines(this.settings.OverlayGridVisibleScale, this.settings.OverlayValueVisibleScale, this.settings.OverlayFontScale);

            var pipelineLoader = new PipelineLoader(this.renderThread.Access, this.programSourceFactory, this.shaderCompiler, this.csharpTransformer, this.textureReaderFactory, this.keyboardTextureResource!, this.mouseStateSource!, viewerSourceLines, this.vertexArray!);
            var projectSourceLoader = new ProjectSourceLoader();
            this.projectLoader = new ProjectLoader(this.renderThread, projectSourceLoader, pipelineLoader, this.hashSource, this.csharpTransformer);

            this.storageThread.Start();
        }

        public void SetKeyState(int index, bool isDown)
        {
            this.keyboardTextureResource?.SetKeyState(index, isDown);
            if (this.IsPaused && this.RenderInputEventsWhenPaused)
            {
                StepPipeline();
            }
        }

        public void ClearKeysDownState()
        {
            this.keyboardTextureResource?.ClearKeysDownState();
        }

        public void SetViewportSize(int width, int height)
        {
            this.viewportWidth = width;
            this.viewportHeight = height;
            if (this.pipeline != null)
            {
                this.pipeline.SetViewportSize(this.viewportWidth, this.viewportHeight);
                this.viewportView.SetViewerBufferSize(this.pipeline.ViewerBufferWidth, this.pipeline.ViewerBufferHeight);
            }
        }

        public void SetViewerTransform(double scale, double offsetX, double offsetY)
        {
            this.pipeline?.SetViewerTransform(scale, offsetX, offsetY);
            this.renderThread?.RenderViewerFrame();
        }

        public void SetMouseState(bool isDown)
        {
            this.pipeline?.SetMouseState(isDown);
            if (this.IsPaused && this.RenderInputEventsWhenPaused)
            {
                StepPipeline();
            }
        }

        public void StepPipeline()
        {
            var now = Stopwatch.GetTimestamp();
            this.frameIndexSource.IncrementTime(Math.Min(now - this.lastStepTime, this.stepIncrement));
            this.lastStepTime = now;

            this.renderThread.RenderFrame();
        }

        public void RestartPipeline()
        {
            this.frameIndexSource.Restart();

            if (this.ClearStateOnRestart)
            {
                this.pipeline?.ClearState();
                this.frameIndexSource.Restart();
                ResetViewerState();
            }

            this.renderThread.RenderFrame();
        }

        public void PausePipeline()
        {
            this.renderThread.SetFrameRate(1);
            this.renderThread.PausePipeline();
            this.frameIndexSource.PauseTime();
            this.IsPaused = true;
            SetInactivityTimer();
        }

        public void ResumePipeline()
        {
            this.renderThread.SetFrameRate(this.frameRate);
            this.renderThread.ResumePipeline();
            this.frameIndexSource.ResumeTime();
            this.IsPaused = false;
            SetInactivityTimer();
        }

        public void RenderFrame()
        {
            this.renderThread?.RenderFrame();
        }

        public void RenderViewerFrame()
        {
            this.renderThread?.RenderViewerFrame();
        }

        public void CreateProject()
        {
            if (!ShowProjectCloseDialog())
            {
                return;
            }

            if (this.projectCreationView == null)
            {
                var templates = GetProjectTemplates();
                this.projectCreationWindow = new Window { Owner = this.viewportWindow, ShowInTaskbar = false };
                this.projectCreationWindow.Closing += OnWindowClosing;

                this.projectCreationView = new ProjectCreationView(this.projectCreationWindow, this, this.settings, this.theme, this.projectOpenFolderDialog, templates, this.settings.CreateProjectTemplateIndex, this.settings.CreateProjectPath);
            }

            this.projectCreationView.Show();
        }

        public void ReloadProject()
        {
            if (this.projectPath != null)
            {
                OpenProject(this.projectPath);
            }
        }

        public void OpenProject()
        {
            if (ShowProjectCloseDialog() && this.projectOpenFileDialog.ShowDialog(this.viewportWindow, this.settings.OpenProjectPath) == true)
            {
                var projectPath = this.projectOpenFileDialog.FileName;
                this.settings.OpenProjectPath = projectPath;
                OpenProject(projectPath);
            }
        }

        public void OpenProject(string projectPath)
        {
            if (!ShowProjectCloseDialog())
            {
                return;
            }

            projectPath = Path.GetFullPath(projectPath);

            this.project = null;
            this.projectPath = projectPath;
            this.projectName = Path.GetFileNameWithoutExtension(projectPath);

            this.viewportView.SetProjectName(this.projectName);
            this.uniformsView.SetProjectName(this.projectName);

            this.viewportView.SetProjectChangeState(false);
            this.uniformsView.SetProjectChangeState(false);
            this.isProjectChanged = false;
            this.isProjectChangeConfirmed = false;

            this.viewportView.Show();

            this.projectContainer?.Dispose();

            ResetViewerState();

            this.SaveFramePath = null;

            this.settings.RecentProjects = new[] { this.projectPath }.Concat(this.settings.RecentProjects).Distinct().ToArray();

            PausePipeline();
            this.frameIndexSource.Restart();

            var fileShaderCache = new FileSystemShaderCache(Path.Combine(Path.GetDirectoryName(projectPath)!, ".shaderlens", "cache"));
            var shaderCache = new ShaderCacheCollection(new MemoryShaderCache(), this.EnableShaderCache ? fileShaderCache : new ReadOnlyShaderCache(fileShaderCache));

            var logger = this.viewportView.CreateLogger(projectPath);

            this.projectSettings = null;
            this.projectUniforms = null;
            this.pipeline = null;
            this.viewerPasses = null;
            this.uniformsView.ClearContent();
            this.projectContainer = new ProjectContainer(this.renderThread, this.viewThread, this.projectLoader!, this.fileSystem, this.fileMonitor, shaderCache, this.loadListener, logger, this.projectPath, this.settings.MemoryCachedResources);
            this.projectContainer.LoadAsync();
        }

        public void CopyRepeat(ICopySource copySource)
        {
            if (this.CopySelection != null)
            {
                if (this.CopySelection.CopyFormatter != null)
                {
                    CopyValue(copySource, this.CopySelection.CopyFormatter);
                }
                else
                {
                    CopyFrame(copySource, this.CopySelection == CopySelection.CopyFrameWithAlpha);
                }
            }
        }

        public void CopyValue(ICopySource copySource, ICopyFormatter copyFormatter)
        {
            var value = copyFormatter.FormatString(copySource);
            this.Clipboard.SetText(value);

            this.CopySelection = new CopySelection(copyFormatter);
        }

        public void CopyFrame(ICopySource copySource, bool includeAlphaChannel)
        {
            var texture = copySource.Texture;
            if (texture != null)
            {
                this.Clipboard.SetImage(texture.Buffer, texture.Width, texture.Height, includeAlphaChannel);
            }

            this.CopySelection = includeAlphaChannel ? CopySelection.CopyFrameWithAlpha : CopySelection.CopyFrame;
        }

        public void ExportFrame()
        {
            if (!this.IsFullyLoaded)
            {
                return;
            }

            var path = this.projectSettings!.SaveFramePath.Value != null ?
                FormattedIndexedPath.Create(this.projectPath!, this.projectSettings.SaveFramePath.Value, 3).GetNextPath() :
                Path.Combine(Path.GetDirectoryName(this.projectPath!)!, $"{Path.GetFileNameWithoutExtension(this.projectPath)}-001.png");

            if (this.frameSaveFileDialog.ShowDialog(this.viewportWindow, path, true) == true)
            {
                SaveFrame(this.frameSaveFileDialog.FileName);
            }
        }

        public void ExportFrameRepeat()
        {
            if (String.IsNullOrEmpty(this.SaveFramePath))
            {
                ExportFrame();
                return;
            }

            SaveFrame(this.SaveFramePath);
        }

        private void SaveFrame(string path)
        {
            // save frame
            var texture = GetSelectedBufferTexture();
            var extension = Path.GetExtension(path);

            if (texture != null)
            {
                this.projectSettings!.SaveFramePath.Value = path;
                this.SaveFramePath = FormattedIndexedPath.Create(this.projectPath!, path, 3).GetNextPath();

                this.storageThread.DispatchAsync(() =>
                {
                    try
                    {
                        var textureWriter = this.textureWriterFactory.Create(extension);

                        using (var fileStream = File.OpenWrite(path))
                        {
                            textureWriter.Write(texture, fileStream);
                        }
                    }
                    catch (Exception e)
                    {
                        this.viewThread.DispatchAsync(() => MessageBox.Show(this.viewportWindow, $"Failed to save \"{path}\"\r\n{e.Message}", "Save", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                });
            }
        }

        public void ExportSequence()
        {
            if (this.renderSequenceView == null)
            {
                this.exportWindow = new Window { Owner = this.viewportWindow, ShowInTaskbar = false };
                this.exportWindow.Closing += OnWindowClosing;

                this.renderSequenceView = new RenderSequenceView(this.exportWindow, this.renderThread, this.viewThread, this, this.settings, this.theme, this.frameSaveFileDialog);
                this.renderSequenceView.SetProject(this.projectPath, this.project?.Source, this.projectSettings, this.pipeline);
            }

            this.renderSequenceView.Show();
        }

        public void SetProjectChanged()
        {
            this.viewportView.SetProjectChangeState(true);
            this.uniformsView.SetProjectChangeState(true);
            this.isProjectChanged = true;
            this.isProjectChangeConfirmed = false;
        }

        public void SaveProject()
        {
            if (this.projectSettings == null)
            {
                return;
            }

            this.projectSettings.Save();

            SetFileMonitorKey(this.project!.Source.Settings?.Key.AbsolutePath);
            SetFileMonitorKey(this.project.Source.UniformsSettings?.Key.AbsolutePath);
            SetFileMonitorKey(this.project.Source.ViewSettings?.Key.AbsolutePath);
            this.viewportView.SetProjectChangeState(false);
            this.uniformsView.SetProjectChangeState(false);
            this.isProjectChanged = false;
            this.isProjectChangeConfirmed = false;
        }

        public void ShowStartPage()
        {
            if (this.startPageView == null)
            {
                this.startPageWindow = new Window();
                this.startPageWindow.Closing += OnWindowClosing;

                this.startPageView = new StartPageView(this.startPageWindow, this, this.settings, this.theme, this.projectOpenFileDialog, this.projectOpenFolderDialog);
            }

            this.startPageWindow!.Owner = this.viewportWindow.IsVisible ? this.viewportWindow : null;
            this.startPageWindow.ShowInTaskbar = !this.viewportWindow.IsVisible;
            this.startPageView.Show();
        }

        public void ShowViewport()
        {
            this.viewportView.Show();

            if (this.settings.UniformsOpened)
            {
                this.uniformsView.Show();
            }
        }

        public void ToggleUniformsView()
        {
            if (this.uniformsView.IsVisible)
            {
                this.uniformsView.Hide();
                this.viewportView.Show();
            }
            else
            {
                this.uniformsView.Show();
            }
        }

        public void Help()
        {
            Process.Start(new ProcessStartInfo("explorer", "https://ytt0.github.io/shaderlens"));
        }

        public bool ShowProjectCloseDialog()
        {
            if (this.isProjectChanged && !this.isProjectChangeConfirmed && this.settings.ConfirmSaveOnClose && this.UnhandledException == null)
            {
                switch (MessageBox.Show(this.viewportWindow, $"There are unsaved changes in {this.projectName}.\r\nDo you want to save these changes?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Cancel:
                        return false;

                    case MessageBoxResult.Yes:
                        SaveProject();
                        return true;

                    case MessageBoxResult.No:
                        this.isProjectChangeConfirmed = true;
                        return true;
                }
            }

            return true;
        }

        public void OpenExternalPath(string filePath)
        {
            if (Path.Exists(filePath)) // make sure it's a file or a directory, and not an unintended command
            {
                Process.Start(new ProcessStartInfo("explorer", '"' + filePath.Trim('"') + '"'));
            }
        }

        public ICopySource GetCopySource()
        {
            this.viewportView.MousePositionSource.GetPosition(out var x, out var y);
            return new CopySource(GetSelectedBufferTexture(), (int)x, (int)y);
        }

        public ITextureBuffer? GetSelectedBufferTexture()
        {
            var pipeline = this.pipeline;
            var bufferIndex = this.viewerBufferIndex;

            return pipeline == null ? null : this.renderThread.Dispatch(() =>
            {
                pipeline.GetTextureSize(bufferIndex, out var width, out var height);
                var buffer = new float[width * height * 4];

                pipeline.GetTexture(bufferIndex, buffer);
                return new TextureBuffer(buffer, width, height);
            });
        }

        public ITextureBuffer? GetViewportBufferTexture()
        {
            this.viewThread.VerifyAccess();

            if (this.pipeline == null)
            {
                return null;
            }

            var width = this.viewportWidth;
            var height = this.viewportHeight;
            var buffer = new float[width * height * 4];

            this.renderThread.Dispatch(() => this.pipeline.GetViewerTexture(buffer));

            return new TextureBuffer(buffer, width, height);
        }

        public IEnumerable<IProjectTemplate> GetProjectTemplates()
        {
            if (this.projectTempates == null)
            {
                var loader = new ProjectTemplateLoader();
                var tempates = new List<IProjectTemplate>();
                var templatesRootPath = Path.GetFullPath(this.settings.ProjectTemplatesPath, Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!);

                var showWarning = true;

                foreach (var templatePath in Directory.GetDirectories(templatesRootPath))
                {
                    try
                    {
                        tempates.Add(loader.Load(templatePath));
                    }
                    catch (ProjectTemplateException e)
                    {
                        if (showWarning)
                        {
                            MessageBox.Show(this.viewportWindow, e.Message, "Project Template Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                            showWarning = false;
                        }
                    }
                }

                this.projectTempates = tempates;
            }

            return this.projectTempates;
        }

        public double GetDragSensitivity(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            return 1000.0 * this.settings.TextBoxDragSensitivity / GetMonitorScale(handle);
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            if (!this.viewportWindow.IsVisible)
            {
                this.application.Shutdown();
                return;
            }

            ((Window)sender!).Hide();
            this.viewportView.Show();
            this.application.Dispatcher.InvokeAsync(this.viewportWindow.Activate, DispatcherPriority.SystemIdle);
            e.Cancel = true;
        }

        private void OnProjectLoadStarted()
        {
            this.resumeOnLoadCompletes = !this.IsPaused;
            PausePipeline();
        }

        private void OnProjectLoadProgress(IProject project)
        {
            this.projectSettings = project.Settings ?? this.projectSettings;

            if (project.Settings != null && project.Uniforms != null && this.projectUniforms != project.Uniforms)
            {
                this.projectUniforms = project.Uniforms;
                this.uniformsView.SetContent(this.projectUniforms.Uniforms, project.Settings);
            }

            this.viewportView.SetProject(project);
        }

        private void OnProjectLoadCompleted(IProject project)
        {
            this.viewThread.VerifyAccess();

            var isAutoReload = this.project != null;
            if (!isAutoReload)
            {
                this.resumeOnLoadCompletes = !project.Source.Paused;
            }

            this.project = project;
            this.projectSettings = project.Settings ?? this.projectSettings;
            this.pipeline = project.Pipeline;

            if (project.Settings != null && project.Uniforms != null && this.projectUniforms != project.Uniforms)
            {
                this.projectUniforms = project.Uniforms;
                this.uniformsView.SetContent(this.projectUniforms.Uniforms, project.Settings);
            }

            this.ViewerBuffersCount = project.Source.Passes.Count;
            this.viewerBufferIndex = this.ViewerBuffersCount - 1;
            this.viewerPasses = GetProjectViewerPasses(project.Source, this.projectSettings, this.settings.DefaultViewerPass);

            if (this.pipeline != null)
            {
                this.pipeline.SetViewportSize(this.viewportWidth, this.viewportHeight);
                this.pipeline.SetRenderDownscale(this.renderDownscale);
                this.pipeline.SetViewerBufferIndex(this.ViewerBufferIndex);
                this.pipeline.SetViewerPass(this.ViewerPass);
                this.viewportView.SetViewerBufferSize(this.pipeline.ViewerBufferWidth, this.pipeline.ViewerBufferHeight);

                this.renderThread.SetPipeline(this.pipeline);
            }
            else
            {
                this.renderThread.ClearPipeline();
            }

            this.viewportView.SetProject(project);
            this.renderSequenceView?.SetProject(this.projectPath, this.project?.Source, this.projectSettings, this.pipeline);

            if (!isAutoReload || this.RestartOnAutoReload)
            {
                RestartPipeline();
            }

            if (this.resumeOnLoadCompletes)
            {
                ResumePipeline();
            }
            else
            {
                RenderFrame();
            }

            SetInactivityTimer();
        }

        private void OnProjectFileChanged(object? sender, EventArgs e)
        {
            this.viewThread.VerifyAccess();

            if (this.AutoReload)
            {
                this.projectContainer?.LoadAsync();
            }
        }

        private void OnInactivityTimerTick()
        {
            var lastInputInfo = new LASTINPUTINFO { cbSize = Marshal.SizeOf<LASTINPUTINFO>() };
            GetLastInputInfo(ref lastInputInfo);

            var now = DateTime.Now;
            if (this.viewportView.IsVisible())
            {
                this.lastVisibleTime = now;
            }

            if (this.lastInputTimestamp != lastInputInfo.dwTime)
            {
                this.lastInputTime = now;
                this.lastInputTimestamp = lastInputInfo.dwTime;
            }

            var maxInactivityTime = TimeSpan.FromSeconds(this.settings.InactivityPauseSeconds);

            if (now - this.lastVisibleTime > maxInactivityTime || now - this.lastInputTime > maxInactivityTime)
            {
                PausePipeline();
            }

            SetInactivityTimer();
        }

        private void SetInactivityTimer()
        {
            var isEnabled = this.PauseOnInactivity && this.IsFullyLoaded && !this.IsPaused;

            if (this.inactivityTimer.IsEnabled != isEnabled)
            {
                var now = DateTime.Now;
                this.lastInputTime = now;
                this.lastVisibleTime = now;
                this.inactivityTimer.IsEnabled = isEnabled;
            }
        }

        private void SetFileMonitorKey(string? path)
        {
            if (path != null)
            {
                this.fileMonitor.Set(this.fileSystem.GetFileKey(path));
            }
        }

        private void ResetViewerState()
        {
            this.renderDownscale = 1;
            this.frameRate = 1;
            this.speed = 1;
            this.viewerBufferIndex = this.ViewerBuffersCount - 1;
            this.viewportView.ResetViewerTransform();
        }

        private static ViewerPassSelection[] GetProjectViewerPasses(IProjectSource projectSource, IProjectSettings? projectSettings, ViewerPassSelection defaultViewerPass)
        {
            return Enumerable.Range(0, projectSource.Passes.Count).Select(index => projectSettings?.ViewerPasses.Value?.ElementAtOrDefault(index) ?? projectSource.Passes.ElementAtOrDefault(index)?.DefaultViewer ?? defaultViewerPass).ToArray();
        }

        private static SourceLines CreateSourceLines(string content, string displayName)
        {
            var resourceKey = new FileResourceKey(displayName);
            return new SourceLines(content.SplitLines().Select((line, index) => new SourceLine(new FileResource<string>(resourceKey, content), line, index)).ToArray());
        }

        private static double GetMonitorScale(IntPtr handle)
        {
            var monitorHandle = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);
            var monitorInfo = new MONITORINFO { Size = Marshal.SizeOf(typeof(MONITORINFO)) };
            var monitorSize = GetMonitorInfo(monitorHandle, ref monitorInfo) ? Math.Min(monitorInfo.Monitor.Right - monitorInfo.Monitor.Left, monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top) : 1000.0;
            return Math.Max(1000.0, monitorSize);
        }

        private static string FormatFilterItem(string header, params string[] extensions)
        {
            var formattedExtensions = String.Join(";", extensions.Select(extension => "*." + extension));
            return $"{header} ({formattedExtensions})|{formattedExtensions}";
        }
    }
}
