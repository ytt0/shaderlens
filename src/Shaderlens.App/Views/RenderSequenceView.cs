using Microsoft.Win32;

namespace Shaderlens.Views
{
    public interface IRenderSequenceView
    {
        void Show();
        void SetTheme(IApplicationTheme theme);
        void SetProject(string? projectPath, IProjectSource? projectSource, IProjectSettings? projectSettings, IRenderPipeline? pipeline);
    }

    public class RenderSequenceView : IRenderSequenceView
    {
        private class Listener : ISequenceRenderListener
        {
            private readonly RenderSequenceView view;

            public Listener(RenderSequenceView view)
            {
                this.view = view;
            }

            public void SetProgress(int index, ITextureBuffer? textureBuffer)
            {
                this.view.SetProgress(index, textureBuffer);
            }

            public void SetCompleted()
            {
                this.view.SetCompleted();
            }
        }

        private readonly Window window;
        private readonly IRenderThread renderThread;
        private readonly IDispatcherThread viewThread;
        private readonly IApplication application;
        private readonly WindowContainer windowContainer;
        private readonly TextureWriterFactory textureWriterFactory;
        private readonly Decorator settingsViewContainer;
        private readonly Decorator progressViewContainer;
        private readonly Listener listener;
        private readonly RenderSequenceProgressView progressView;
        private readonly RenderSequenceSettingsView settingsView;
        private string? projectPath;
        private IProjectSource? projectSource;
        private IProjectSettings? projectSettings;
        private IRenderPipeline? pipeline;
        private RenderSequenceTask? renderTask;
        private bool reopenOnDialogClose;
        private bool resumePipelineOnComplete;
        private bool setAutoReloadOnComplete;

        public RenderSequenceView(Window window, IRenderThread renderThread, IDispatcherThread viewThread, IApplication application, IApplicationSettings settings, IApplicationTheme theme, SaveFileDialog saveFileDialog)
        {
            this.window = window;
            this.renderThread = renderThread;
            this.viewThread = viewThread;
            this.application = application;

            this.windowContainer = new WindowContainer(window, theme, settings.RenderSequenceWindowState, "Render Sequence");
            this.window.Closing += OnClosing;

            this.textureWriterFactory = new TextureWriterFactory();

            this.settingsView = new RenderSequenceSettingsView(window, theme, this.textureWriterFactory.SupportedExtensions, saveFileDialog);
            this.settingsView.RenderClicked += OnRenderClicked;

            this.progressView = new RenderSequenceProgressView(application, theme);
            this.progressView.BackClicked += OnBackClicked;
            this.progressView.PauseClicked += OnPauseClicked;
            this.progressView.ResumeClicked += OnResumeClicked;

            this.settingsViewContainer = new Decorator { Child = this.settingsView.Content };
            this.progressViewContainer = new Decorator { Child = this.progressView.Content, Visibility = Visibility.Collapsed };

            this.listener = new Listener(this);

            this.windowContainer.SetContent(new Grid().WithChildren(this.settingsViewContainer, this.progressViewContainer));

            this.window.LocationChanged += (sender, e) => this.settingsView.SetDragSensitivity(this.application.GetDragSensitivity(this.window));
        }

        public void Show()
        {
            this.settingsViewContainer.Visibility = Visibility.Visible;
            this.progressViewContainer.Visibility = Visibility.Collapsed;
            this.windowContainer.Show();
        }

        public void SetTheme(IApplicationTheme theme)
        {
            this.windowContainer.SetTheme(theme);
        }

        public void SetProject(string? projectPath, IProjectSource? projectSource, IProjectSettings? projectSettings, IRenderPipeline? pipeline)
        {
            this.projectPath = projectPath;
            this.projectSource = projectSource;
            this.projectSettings = projectSettings;
            this.pipeline = pipeline;

            if (projectSettings != null)
            {
                this.settingsView.SetProject(projectPath, projectSource, projectSettings.RenderSequence.Value);
            }
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            this.renderTask?.Stop();
        }

        private void OnRenderClicked(object? sender, EventArgs e)
        {
            var settings = this.settingsView.GetSettings();

            if (this.projectSettings != null)
            {
                this.projectSettings.RenderSequence.Value = settings;
                this.projectSettings.Save();
            }

            if (this.projectPath != null && this.pipeline != null)
            {
                this.progressView.SetProject(Path.GetDirectoryName(settings.TargetPath)!, settings.StartFrame, settings.FrameCount, settings.Prerender);

                this.settingsViewContainer.Visibility = Visibility.Collapsed;
                this.progressViewContainer.Visibility = Visibility.Visible;

                this.windowContainer.Close();

                this.resumePipelineOnComplete = !this.application.IsPaused;
                this.application.PausePipeline();

                this.setAutoReloadOnComplete = this.application.AutoReload;
                this.application.AutoReload = false;

                this.renderTask = new RenderSequenceTask(this.renderThread, this.viewThread, this.pipeline, this.listener, this.textureWriterFactory, settings, this.projectPath);
                this.renderTask.Start();

                this.reopenOnDialogClose = false;
                this.windowContainer.ShowDialog();

                if (this.reopenOnDialogClose)
                {
                    this.windowContainer.Show();
                }
            }
        }

        private void OnBackClicked(object? sender, EventArgs e)
        {
            this.renderTask?.Stop();

            this.settingsViewContainer.Visibility = Visibility.Visible;
            this.progressViewContainer.Visibility = Visibility.Collapsed;

            this.settingsView.SetProject(this.projectPath, this.projectSource, this.projectSettings!.RenderSequence.Value);
        }

        private void OnPauseClicked(object? sender, EventArgs e)
        {
            this.renderTask?.Pause();
        }

        private void OnResumeClicked(object? sender, EventArgs e)
        {
            this.renderTask?.Resume();
        }

        private void SetProgress(int index, ITextureBuffer? textureBuffer)
        {
            this.progressView.SetProgress(index, textureBuffer);
        }

        private void SetCompleted()
        {
            this.renderTask!.Dispose();
            this.renderTask = null;

            this.application.AutoReload = this.setAutoReloadOnComplete;

            if (this.resumePipelineOnComplete)
            {
                this.application.ResumePipeline();
            }

            this.reopenOnDialogClose = true;
            this.windowContainer.Close();
        }
    }
}
