namespace Shaderlens.Views.Logger
{
    public partial class ProjectLoadLogger : IProjectLoadLogger
    {
        private class LoggerProgressTrack : FrameworkElement
        {
            private const double IndeterminateTrackMargin = 0.05;
            private const double IndeterminateTrackLength = 0.4;
            private const long IndeterminateDurationTicks = TimeSpan.TicksPerSecond * 4;

            public static readonly DependencyProperty ForegroundProperty = Control.ForegroundProperty.AddOwner(typeof(LoggerProgressTrack), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            public Brush? Foreground
            {
                get { return (Brush?)GetValue(ForegroundProperty); }
                set { SetValue(ForegroundProperty, value); }
            }

            public long StartTime { get; set; }
            private double? progress;

            private readonly ITimeSource timeSource;

            public LoggerProgressTrack(ITimeSource timeSource)
            {
                this.timeSource = timeSource;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                if (this.progress == null)
                {
                    var time = this.timeSource.GetElapsedTime(this.StartTime).Ticks % IndeterminateDurationTicks / ((double)IndeterminateDurationTicks);
                    var offset = -IndeterminateTrackLength - IndeterminateTrackMargin + (1.0 + IndeterminateTrackLength + 2 * IndeterminateTrackMargin) * time;
                    drawingContext.DrawRectangle(this.Foreground, null, new Rect(offset * this.RenderSize.Width, 0, IndeterminateTrackLength * this.RenderSize.Width, this.RenderSize.Height));
                }
                else
                {
                    drawingContext.DrawRectangle(this.Foreground, null, new Rect(0, 0, this.RenderSize.Width * Math.Clamp(this.progress.Value, 0.0, 1.0), this.RenderSize.Height));
                }
            }

            public void SetProgress(double value)
            {
                this.progress = value;
                InvalidateVisual();
            }

            public void SetIndeterminate()
            {
                this.progress = null;
                InvalidateVisual();
            }
        }

        private class ParagraphMarginContainer : Decorator
        {
            protected override Size MeasureOverride(Size constraint)
            {
                base.MeasureOverride(constraint);
                return new Size();
            }

            protected override Size ArrangeOverride(Size arrangeSize)
            {
                if (this.Child != null)
                {
                    var size = this.Child.DesiredSize;
                    this.Child.Arrange(new Rect(-size.Width, 0, size.Width, size.Height));
                }

                return arrangeSize;
            }
        }

        private readonly Decorator container;
        private readonly IApplication application;
        private readonly ITimeSource timeSource;
        private readonly ILoggerResourcesFactory resourcesFactory;
        private readonly IApplicationTheme theme;
        private readonly string rootPath;
        private readonly int contextLinesCount;
        private readonly TimeSpan visibleDelay;
        private readonly bool hideOnSuccess;
        private readonly DispatcherTimer timer;
        private readonly List<SourceLine> failureLines;
        private readonly Run stateHeaderRun;
        private readonly Run stateSeparatorRun;
        private readonly Run stateContentRun;
        private readonly TextBlock timeTextBlock;
        private readonly FlowDocument document;
        private readonly Decorator iconContainer;
        private readonly LoggerProgressTrack progressTrack;
        private readonly Grid headerPanel;
        private readonly StyledRichTextBox documentView;
        private readonly Paragraph failureParagraph;
        private long startTime;
        private bool isRunning;
        private TimeSpan lastElapsedTime;
        private bool lastSucceeded;

        public ProjectLoadLogger(Decorator container, IApplication application, Transform scrollBarTransform, ITimeSource timeSource, ILoggerResourcesFactory resourcesFactory, IApplicationTheme theme, string projectPath, int contextLinesCount, TimeSpan visibleDelay, bool hideOnSuccess)
        {
            this.container = container;
            this.application = application;
            this.timeSource = timeSource;
            this.resourcesFactory = resourcesFactory;
            this.theme = theme;
            this.rootPath = Path.GetDirectoryName(projectPath)!;
            this.contextLinesCount = contextLinesCount;
            this.visibleDelay = visibleDelay;
            this.hideOnSuccess = hideOnSuccess;
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.0 / 60.0), IsEnabled = false };
            this.timer.Tick += OnTimerTick;
            this.failureLines = new List<SourceLine>();

            this.stateHeaderRun = new Run { FontWeight = FontWeights.Bold };
            this.stateSeparatorRun = new Run();
            this.stateContentRun = new Run();
            this.timeTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            this.document = new FlowDocument();

            this.iconContainer = new Decorator { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, -2, 0, 0) };

            var headerTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            headerTextBlock.Inlines.Add(this.stateHeaderRun);
            headerTextBlock.Inlines.Add(this.stateSeparatorRun);
            headerTextBlock.Inlines.Add(this.stateContentRun);

            this.documentView = new StyledRichTextBox(theme)
            {
                Document = this.document,
                ScrollBarTransform = scrollBarTransform,
            }.
            WithReference(Control.ForegroundProperty, theme.Log.ContextForeground).
            WithReference(Control.BackgroundProperty, theme.Log.ContextBackground);

            this.documentView.AddHandler(DataObject.CopyingEvent, new DataObjectCopyingEventHandler((sender, e) =>
            {
                var textBox = (RichTextBox)sender;

                var text = textBox.Selection.Text;
                text = text.Replace(Environment.NewLine + ' ', Environment.NewLine);

                if (text.StartsWith(' '))
                {
                    text = text.Substring(1);
                }

                e.DataObject.SetData(DataFormats.UnicodeText, text, true);
                e.Handled = true;
            }));

            this.progressTrack = new LoggerProgressTrack(this.timeSource)
            {
                Height = 2,
                VerticalAlignment = VerticalAlignment.Top,
            };

            this.headerPanel = new Grid().WithChildren
            (
                this.progressTrack,
                new DockPanel { LastChildFill = true, Margin = new Thickness(2, 2, 8, 0) }.WithChildren
                (
                    this.timeTextBlock.WithDock(Dock.Right),
                    this.iconContainer.WithDock(Dock.Left),
                    headerTextBlock
                )
            );

            container.Child = new DockPanel { LastChildFill = true }.WithChildren
            (
                this.headerPanel.WithDock(Dock.Top),
                this.documentView
            );

            this.failureParagraph = new Paragraph
            {
                Padding = new Thickness(25, 2, 8, 2),
                Margin = new Thickness()
            }.
            WithContentReference(TextElement.FontFamilyProperty, theme.CodeFontFamily).
            WithContentReference(TextElement.ForegroundProperty, theme.Log.FailureDetailsForeground).
            WithContentReference(TextElement.BackgroundProperty, theme.Log.FailureDetailsBackground);
        }

        public void Dispose()
        {
            this.timer.Stop();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            var elapsedTime = this.timeSource.GetElapsedTime(this.startTime);
            this.timeTextBlock.Text = elapsedTime.TotalMinutes < 1.0 ? elapsedTime.TotalSeconds.ToString("0.0s") : elapsedTime.ToString("m':'ss");

            if (this.lastElapsedTime.Ticks > 0)
            {
                this.progressTrack.SetProgress(elapsedTime.TotalSeconds / this.lastElapsedTime.TotalSeconds);
            }
            else
            {
                this.progressTrack.SetIndeterminate();
            }

            if (this.container.Visibility == Visibility.Collapsed && elapsedTime > this.visibleDelay)
            {
                this.container.Visibility = Visibility.Visible;
            }
        }

        public void LoadStarted()
        {
            ValidateNotRunning();
            this.isRunning = true;

            this.container.Visibility = this.lastSucceeded && this.lastElapsedTime.Ticks < 2 * this.visibleDelay.Ticks ? Visibility.Collapsed : Visibility.Visible;

            this.failureLines.Clear();
            this.failureParagraph.Inlines.Clear();
            this.document.Blocks.Clear();
            this.documentView.Visibility = Visibility.Collapsed;

            this.progressTrack.SetProgress(0.0);
            this.progressTrack.WithReference(LoggerProgressTrack.ForegroundProperty, this.theme.Log.ProgressTrack);
            this.headerPanel.WithReference(TextElement.ForegroundProperty, this.theme.Log.HeaderForeground);
            this.headerPanel.WithReference(Panel.BackgroundProperty, this.theme.Log.HeaderBackground);

            this.iconContainer.Child = this.resourcesFactory.CreateProgressIcon();

            this.startTime = this.timeSource.Now;
            this.progressTrack.StartTime = this.startTime;
            this.timer.Start();
        }

        public void SetState(string header, string? content = null)
        {
            ValidateRunning();

            this.stateHeaderRun.Text = header;
            this.stateSeparatorRun.Text = content != null ? " - " : null;
            this.stateContentRun.Text = content;
        }

        public void AddFailure(string message, IFileResource<string>? source = null, int? lineIndex = null)
        {
            ValidateRunning();

            if (source != null && lineIndex != null)
            {
                this.failureLines.Add(new SourceLine(source, String.Empty, lineIndex.Value));

                var span = new Span();
                span.Inlines.Add(CreateResourceReferenceInline(source.Key, lineIndex));
                span.Inlines.Add(new Run(" "));
                span.Inlines.Add(new Run(message));

                AddFailureItem(span);
            }
            else if (source != null)
            {
                var span = new Span();
                span.Inlines.Add(CreateResourceReferenceInline(source.Key));
                span.Inlines.Add(new Run(" "));
                span.Inlines.Add(new Run(message));

                AddFailureItem(span);
            }
            else
            {
                AddFailureItem(new Run(message));
            }
        }

        public void LoadCompleted(bool succeeded)
        {
            ValidateRunning();
            this.lastSucceeded = succeeded;
            this.timer.Stop();

            this.iconContainer.Child = succeeded ? this.resourcesFactory.CreateSuccessIcon() : this.resourcesFactory.CreateFailureIcon();
            this.progressTrack.SetProgress(1.0);

            if (succeeded)
            {
                this.progressTrack.WithReference(LoggerProgressTrack.ForegroundProperty, this.theme.Log.SuccessProgressTrack);
                this.headerPanel.WithReference(TextElement.ForegroundProperty, this.theme.Log.SuccessHeaderForeground);
                this.headerPanel.WithReference(Panel.BackgroundProperty, this.theme.Log.SuccessHeaderBackground);
                this.container.Visibility = this.hideOnSuccess ? Visibility.Collapsed : Visibility.Visible;
                this.lastElapsedTime = this.timeSource.GetElapsedTime(this.startTime);

                SetState("Succeeded");
            }
            else
            {
                this.progressTrack.WithReference(LoggerProgressTrack.ForegroundProperty, this.theme.Log.FailureProgressTrack);
                this.headerPanel.WithReference(TextElement.ForegroundProperty, this.theme.Log.FailureHeaderForeground);
                this.headerPanel.WithReference(Panel.BackgroundProperty, this.theme.Log.FailureHeaderBackground);
                this.container.Visibility = Visibility.Visible;

                SetState("Failed");
                AddFailureLinesContext();
            }

            this.isRunning = false;
        }

        private void ValidateRunning()
        {
            if (!this.isRunning)
            {
                throw new Exception($"{nameof(ProjectLoadLogger)} is not running");
            }
        }

        private void ValidateNotRunning()
        {
            if (this.isRunning)
            {
                throw new Exception($"{nameof(ProjectLoadLogger)} is already running");
            }
        }

        private void AddFailureItem(Inline inline)
        {
            if (this.failureParagraph.Inlines.Count > 0)
            {
                this.failureParagraph.Inlines.Add(new LineBreak());
            }
            else
            {
                this.document.Blocks.Add(this.failureParagraph);
            }

            this.failureParagraph.Inlines.Add(inline);

            this.container.Visibility = Visibility.Visible;
            this.documentView.Visibility = Visibility.Visible;
        }

        private void AddFailureLinesContext()
        {
            if (this.failureLines.Count == 0 || this.contextLinesCount == 0)
            {
                return;
            }

            var sources = this.failureLines.Select(line => line.Source).Distinct().ToArray();

            foreach (var source in sources)
            {
                var isFirstFailure = true;
                string? lastFailureLine = null;
                var lastFailureIndex = -this.contextLinesCount - 1;

                var sourceLines = source.Value.SplitLines().ToArray();

                var headerParagraph = new Paragraph
                {
                    Margin = new Thickness(25, 0, 0, 0),
                };

                headerParagraph.Inlines.Add(new LineBreak());
                headerParagraph.Inlines.Add(CreateResourceReferenceInline(source.Key));

                this.document.Blocks.Add(headerParagraph);

                var contextParagraph = new Paragraph
                {
                    Margin = new Thickness(60, 0, 0, 0),
                    Cursor = Cursors.IBeam,
                }.WithContentReference(TextElement.FontFamilyProperty, this.theme.CodeFontFamily);

                for (var i = 0; i < sourceLines.Length; i++)
                {
                    var sourceLine = sourceLines[i];

                    var isContextLine = i <= lastFailureIndex + this.contextLinesCount;
                    var isFailureLine = this.failureLines.Any(failureLine => failureLine.Index == i && Equals(failureLine.Source.Key, source.Key));

                    if (isFailureLine)
                    {
                        if (i > lastFailureIndex + this.contextLinesCount + this.contextLinesCount + 1 && !isFirstFailure)
                        {
                            contextParagraph.Inlines.Add(new LineBreak());
                            contextParagraph.Inlines.Add(CreateMarginInline(":"));
                            contextParagraph.Inlines.Add("");
                        }

                        if (i > lastFailureIndex + this.contextLinesCount)
                        {
                            for (var j = Math.Max(0, i - this.contextLinesCount); j < i; j++)
                            {
                                if (j >= lastFailureIndex + this.contextLinesCount + 1)
                                {
                                    contextParagraph.Inlines.Add(CreateSourceLineSpan(sourceLines[j], j, false, this.theme.Log));
                                }
                            }
                        }
                    }

                    if (isFailureLine || isContextLine)
                    {
                        contextParagraph.Inlines.Add(CreateSourceLineSpan(sourceLine, i, isFailureLine, this.theme.Log));
                    }

                    if (isFailureLine)
                    {
                        lastFailureIndex = i;
                        lastFailureLine = sourceLine;
                        isFirstFailure = false;
                    }
                }

                this.document.Blocks.Add(contextParagraph);
            }
        }

        private Inline CreateResourceReferenceInline(FileResourceKey key, int? index = null)
        {
            if (key.DisplayName != null)
            {
                return new Run(key.DisplayName) { FontWeight = FontWeights.Bold };
            }

            if (key.AbsolutePath == null)
            {
                throw new ArgumentException($"Invalid {typeof(FileResourceKey)}", nameof(key));
            }

            var relativePath = Path.GetRelativePath(this.rootPath, key.AbsolutePath);

            var span = new Span
            {
                ToolTip = key.AbsolutePath,
                TextDecorations = null,
                Cursor = Cursors.Hand,
            }.WithContentReference(TextElement.ForegroundProperty, this.theme.Log.LinkForeground);

            span.Inlines.Add(new Run(index != null ? $"{relativePath} {index + 1}" : relativePath).WithContentReference(TextElement.FontFamilyProperty, this.theme.CodeFontFamily));
            span.Inlines.Add(new InlineUIContainer(new Border
            {
                Margin = new Thickness(4, 0, 2, 0),
                Child = this.resourcesFactory.CreateLinkIcon()
            }));

            span.MouseDown += (sender, e) => this.application.OpenExternalPath(key.AbsolutePath);

            return span;
        }

        private static Span CreateSourceLineSpan(string sourceLine, int index, bool highlight, ILogTheme theme)
        {
            var span = new Span();

            span.Inlines.Add(new LineBreak());
            span.Inlines.Add(CreateMarginInline((index + 1).ToString()));

            if (highlight)
            {
                var match = LineHighlightRegex().Match(sourceLine);
                span.Inlines.Add(match.Groups["prefix"].Value);
                span.Inlines.Add(new Run(match.Groups["highlight"].Value).
                    WithContentReference(TextElement.ForegroundProperty, theme.ContextHighlightForeground).
                    WithContentReference(TextElement.BackgroundProperty, theme.ContextHighlightBackground));
                span.Inlines.Add(match.Groups["suffix"].Value);
            }
            else
            {
                span.Inlines.Add(sourceLine);
            }

            return span;
        }

        private static InlineUIContainer CreateMarginInline(string text)
        {
            return new InlineUIContainer(new ParagraphMarginContainer { Child = new TextBlock { Text = text, Margin = new Thickness(0, 0, 8, 0), Opacity = 0.5 } }) { BaselineAlignment = BaselineAlignment.Top };
        }

        [GeneratedRegex("^(?<prefix>\\s*)(?<highlight>.*?)(?<suffix>\\s*//($|[^@]).*)?$")]
        private static partial Regex LineHighlightRegex();
    }

    public class DispatcherLogger : IProjectLoadLogger
    {
        private readonly IProjectLoadLogger logger;
        private readonly IDispatcherThread viewThread;

        public DispatcherLogger(IProjectLoadLogger logger, IDispatcherThread viewThread)
        {
            this.logger = logger;
            this.viewThread = viewThread;
        }

        public void Dispose()
        {
            this.viewThread.DispatchAsync(this.logger.Dispose);
        }

        public void LoadStarted()
        {
            this.viewThread.DispatchAsync(this.logger.LoadStarted);
        }

        public void SetState(string header, string? content = null)
        {
            this.viewThread.DispatchAsync(() => this.logger.SetState(header, content));
        }

        public void AddFailure(string message, IFileResource<string>? source = null, int? lineIndex = null)
        {
            this.viewThread.DispatchAsync(() => this.logger.AddFailure(message, source, lineIndex));
        }

        public void LoadCompleted(bool succeeded)
        {
            this.viewThread.DispatchAsync(() => this.logger.LoadCompleted(succeeded));
        }
    }

    public class BackgroundImageLogger : IProjectLoadLogger
    {
        private readonly Decorator container;
        private readonly IApplication application;
        private readonly IProjectLoadLogger logger;
        private readonly ILogTheme theme;
        private bool isLoaded;

        public BackgroundImageLogger(Decorator container, IApplication application, IProjectLoadLogger logger, ILogTheme theme)
        {
            this.container = container;
            this.application = application;
            this.logger = logger;
            this.theme = theme;
        }

        public void Dispose()
        {
            this.logger.Dispose();
        }

        public void LoadStarted()
        {
            this.logger.LoadStarted();

            var texture = this.isLoaded ? this.application.GetViewportBufferTexture() : null;
            if (texture != null)
            {
                var bufferArray = texture.Buffer.ToArray().Select(value => (byte)Math.Clamp((int)Math.Round(255.0 * value), 0, 255)).ToArray();
                bufferArray.FlipBufferRows(texture.Height);
                bufferArray.FlipBgraRgba();
                bufferArray.RemoveAlphaChannel();

                var dpi = VisualTreeHelper.GetDpi(this.container);
                this.container.Child = new Image
                {
                    Source = BitmapSource.Create(texture.Width, texture.Height, dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Bgra32, null, bufferArray, texture.Width * 4).WithFreeze(),
                    Stretch = Stretch.None
                };
            }

            this.container.Visibility = Visibility.Visible;
        }

        public void SetState(string header, string? content = null)
        {
            this.logger.SetState(header, content);
        }

        public void AddFailure(string message, IFileResource<string>? source = null, int? lineIndex = null)
        {
            this.logger.AddFailure(message, source, lineIndex);
        }

        public void LoadCompleted(bool succeeded)
        {
            this.isLoaded = succeeded;

            if (succeeded)
            {
                this.container.Visibility = Visibility.Collapsed;
                this.container.Opacity = 1.0;
            }
            else
            {
                this.container.SetReference(UIElement.OpacityProperty, this.theme.FailureViewportOpacity);
            }

            this.logger.LoadCompleted(succeeded);
        }
    }
}