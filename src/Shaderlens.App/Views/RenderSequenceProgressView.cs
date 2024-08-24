namespace Shaderlens.Views
{
    public interface IRenderSequenceProgressView
    {
        event EventHandler? BackClicked;
        event EventHandler? PauseClicked;
        event EventHandler? ResumeClicked;

        FrameworkElement Content { get; }

        void SetProject(string targetFolderPath, int startFrame, int frameCount, bool prerender);
        void SetProgress(int frameIndex, ITextureBuffer? textureBuffer);
    }

    public class RenderSequenceProgressView : IRenderSequenceProgressView
    {
        private class ProgressTrack : FrameworkElement
        {
            public static readonly DependencyProperty ForegroundProperty = Control.ForegroundProperty.AddOwner(typeof(ProgressTrack), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            public Brush? Foreground
            {
                get { return (Brush?)GetValue(ForegroundProperty); }
                set { SetValue(ForegroundProperty, value); }
            }

            public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(ProgressTrack), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
            public double Progress
            {
                get { return (double)GetValue(ProgressProperty); }
                set { SetValue(ProgressProperty, value); }
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRectangle(this.Foreground, null, new Rect(0, 0, this.RenderSize.Width * Math.Clamp(this.Progress, 0.0, 1.0), this.RenderSize.Height));
            }
        }

        private static readonly Thickness Spacing = new Thickness(10, 5, 10, 5);

        public event EventHandler? BackClicked;
        public event EventHandler? PauseClicked;
        public event EventHandler? ResumeClicked;

        public FrameworkElement Content { get; }

        private readonly IApplication application;
        private readonly IApplicationTheme theme;
        private readonly StyledButton pauseButton;
        private readonly StyledButton resumeButton;
        private readonly StyledButton backButton;
        private readonly StyledButton openFolderButton;
        private readonly ProgressTrack progressTrack;
        private string? targetFolderPath;
        private int startFrame;
        private int endFrame;
        private bool prerender;
        private bool isPaused;
        private int frameIndex;
        private readonly TextBlock stateTextBlock;
        private readonly TextBlock frameTextBlock;
        private readonly TextBlock progressTextBlock;
        private readonly TextBlock failureTextBlock;
        private readonly Border failureContainer;
        private readonly Image image;

        public RenderSequenceProgressView(IApplication application, IApplicationTheme theme)
        {
            this.application = application;
            this.theme = theme;

            this.pauseButton = new StyledButton(theme) { Content = "Pause", Margin = Spacing }.WithHandler(ButtonBase.ClickEvent, OnPauseClicked);
            this.resumeButton = new StyledButton(theme) { Content = "Resume", Margin = Spacing, Visibility = Visibility.Collapsed }.WithHandler(ButtonBase.ClickEvent, OnResumeClicked);
            this.backButton = new StyledButton(theme) { Content = "Back", Margin = Spacing, Visibility = Visibility.Collapsed, VerticalAlignment = VerticalAlignment.Bottom }.WithHandler(ButtonBase.ClickEvent, OnBackClicked);
            this.openFolderButton = new StyledButton(theme) { Content = "Open Folder", IsEnabled = false, Margin = Spacing }.WithHandler(ButtonBase.ClickEvent, OnOpenFolderClicked);

            this.progressTrack = new ProgressTrack { Foreground = Brushes.Yellow, Height = 2, Progress = 0.0 }.WithReference(ProgressTrack.ForegroundProperty, theme.Log.ProgressTrack);

            this.stateTextBlock = new TextBlock { Text = "Initializing...", Margin = Spacing };
            this.frameTextBlock = new TextBlock { Text = "Frame", Margin = Spacing };
            this.progressTextBlock = new TextBlock { Text = "Progress", Margin = Spacing };

            this.failureTextBlock = new TextBlock
            {
                Text = "Error message",
                Margin = Spacing,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            }.
            WithReference(TextElement.ForegroundProperty, theme.Log.FailureHeaderForeground);

            this.failureContainer = new Border
            {
                Child = this.failureTextBlock,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Collapsed
            }.
            WithReference(Border.BackgroundProperty, theme.Log.FailureHeaderBackground);

            this.image = new Image { Stretch = Stretch.Uniform };

            this.Content = new DockPanel { LastChildFill = true }.WithChildren
            (
                this.progressTrack.WithDock(Dock.Top),
                this.failureContainer.WithDock(Dock.Top),
                new Grid { Margin = Spacing }.WithDock(Dock.Right).WithChildren
                (
                    new StackPanel().WithChildren
                    (
                        this.stateTextBlock,
                        this.frameTextBlock,
                        this.progressTextBlock,
                        this.openFolderButton,
                        this.pauseButton,
                        this.resumeButton
                    ),
                    this.backButton
                ),
                this.image
            );
        }

        public void SetProject(string targetFolderPath, int startFrame, int frameCount, bool prerender)
        {
            this.targetFolderPath = targetFolderPath;
            this.startFrame = startFrame;
            this.endFrame = startFrame + frameCount - 1;
            this.prerender = prerender;
            this.stateTextBlock.Text = "Initializing...";
            this.failureContainer.Visibility = Visibility.Collapsed;
            this.progressTrack.Progress = 0;
            this.progressTrack.SetReference(ProgressTrack.ForegroundProperty, this.theme.Log.ProgressTrack);
            this.resumeButton.Visibility = Visibility.Collapsed;
            this.pauseButton.Visibility = Visibility.Visible;
            this.backButton.Visibility = Visibility.Collapsed;
            this.openFolderButton.IsEnabled = true;
            this.isPaused = false;
            this.image.Source = null;
            this.frameTextBlock.Text = "Frame";
            this.progressTextBlock.Text = "Progress";
        }

        public void SetProgress(int frameIndex, ITextureBuffer? textureBuffer)
        {
            if (textureBuffer != null)
            {
                this.image.Source = CreateBitmap(textureBuffer);
            }

            this.frameIndex = frameIndex;

            var renderStartFrame = this.prerender ? 0 : this.startFrame;
            var progress = (double)(frameIndex - renderStartFrame + 1) / (this.endFrame - renderStartFrame + 1);

            this.frameTextBlock.Text = $"Frame {frameIndex:n0}";
            this.progressTextBlock.Text = $"Progress {100.0 * progress:f0}%";

            this.stateTextBlock.Text = GetRenderStateText();

            this.progressTrack.Progress = progress;
            this.progressTrack.SetReference(ProgressTrack.ForegroundProperty, this.theme.Log.ProgressTrack);

            if (frameIndex == this.endFrame)
            {
                this.stateTextBlock.Text = "Completed";
                this.progressTrack.SetReference(ProgressTrack.ForegroundProperty, this.theme.Log.SuccessProgressTrack);
                this.pauseButton.Visibility = Visibility.Collapsed;
                this.backButton.Visibility = Visibility.Visible;
            }
        }

        public void SetFailure(string failure)
        {
            this.stateTextBlock.Text = "Failed";
            this.failureTextBlock.Text = failure;
            this.failureContainer.Visibility = Visibility.Visible;
            this.progressTrack.SetReference(ProgressTrack.ForegroundProperty, this.theme.Log.FailureProgressTrack);
            this.pauseButton.Visibility = Visibility.Collapsed;
            this.resumeButton.Visibility = Visibility.Visible;
            this.backButton.Visibility = Visibility.Visible;
        }

        private void OnPauseClicked(object sender, RoutedEventArgs e)
        {
            this.isPaused = true;

            this.pauseButton.Visibility = Visibility.Collapsed;
            this.resumeButton.Visibility = Visibility.Visible;
            this.backButton.Visibility = Visibility.Visible;

            this.PauseClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnResumeClicked(object sender, RoutedEventArgs e)
        {
            this.isPaused = false;

            this.pauseButton.Visibility = Visibility.Visible;
            this.resumeButton.Visibility = Visibility.Collapsed;
            this.backButton.Visibility = Visibility.Collapsed;

            this.ResumeClicked?.Invoke(this, EventArgs.Empty);

            this.stateTextBlock.Text = GetRenderStateText();
        }

        private void OnOpenFolderClicked(object sender, RoutedEventArgs e)
        {
            this.application.OpenExternalPath(this.targetFolderPath!);
        }

        private void OnBackClicked(object sender, RoutedEventArgs e)
        {
            this.BackClicked?.Invoke(this, EventArgs.Empty);
        }

        private static BitmapSource CreateBitmap(ITextureBuffer buffer)
        {
            var bufferArray = buffer.Buffer.ToArray().Select(value => (byte)Math.Clamp((int)Math.Round(255.0 * value), 0, 255)).ToArray();
            bufferArray.FlipBufferRows(buffer.Height);
            bufferArray.FlipBgraRgba();
            bufferArray.RemoveAlphaChannel();

            var bitmap = BitmapSource.Create(buffer.Width, buffer.Height, 96.0, 96.0, PixelFormats.Bgra32, null, bufferArray, buffer.Width * 4);
            var encoder = new PngBitmapEncoder();
            var stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);

            return bitmap;
        }

        private string GetRenderStateText()
        {
            return this.isPaused ? "Paused" : this.frameIndex < this.startFrame ? "Pre-rendering..." : "Rendering...";
        }
    }
}
