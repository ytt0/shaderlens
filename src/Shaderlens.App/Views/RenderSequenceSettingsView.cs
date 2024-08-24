using Microsoft.Win32;

namespace Shaderlens.Views
{
    public interface IRenderSequenceSettingsView
    {
        event EventHandler? RenderClicked;

        FrameworkElement Content { get; }

        void SetProject(string? projectPath, IEnumerable<string> buffersDisplayNames, RenderSequenceSettings settings);
        void SetDragSensitivity(double dragSensitivity);
        RenderSequenceSettings GetSettings();
    }

    public class RenderSequenceSettingsView : IRenderSequenceSettingsView
    {
        private class PropertyPanel : FrameworkElement
        {
            private const double HeaderMaxWidth = 120.0;
            private const double HeaderRelativeSize = 0.6;

            protected override int VisualChildrenCount { get { return 2; } }

            private readonly FrameworkElement headerElement;
            private readonly FrameworkElement valueElement;

            public PropertyPanel(FrameworkElement headerElement, FrameworkElement valueElement)
            {
                this.headerElement = headerElement;
                this.valueElement = valueElement;
                AddVisualChild(headerElement);
                AddVisualChild(valueElement);
            }

            protected override Visual GetVisualChild(int index)
            {
                return index == 0 ? this.headerElement : index == 1 ? this.valueElement : throw new ArgumentOutOfRangeException(nameof(index));
            }

            protected override Size MeasureOverride(Size availableSize)
            {
                var headerWidth = Math.Min(availableSize.Width * HeaderRelativeSize, HeaderMaxWidth);
                var valueWidth = availableSize.Width - headerWidth;

                this.headerElement.Measure(new Size(headerWidth, availableSize.Height));
                this.valueElement.Measure(new Size(valueWidth, availableSize.Height));

                return new Size(availableSize.Width, Math.Max(this.headerElement.DesiredSize.Height, this.valueElement.DesiredSize.Height));
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                var headerWidth = Math.Min(finalSize.Width * HeaderRelativeSize, HeaderMaxWidth);
                var valueWidth = finalSize.Width - headerWidth;

                this.headerElement.Arrange(new Rect(0, 0, headerWidth, finalSize.Height));
                this.valueElement.Arrange(new Rect(headerWidth, 0, valueWidth, finalSize.Height));

                return finalSize;
            }
        }

        private static readonly Thickness Spacing = new Thickness(10, 5, 10, 5);
        private static readonly Thickness RowSpacing = new Thickness(0, 0, 0, 20);
        private const int ColumnWidth = 100;

        public event EventHandler? RenderClicked;

        public FrameworkElement Content { get; }

        private readonly Window window;
        private readonly IApplicationTheme theme;
        private readonly SaveFileDialog saveFileDialog;
        private readonly HashSet<string> supportedExtensions;
        private readonly NumberTextBox frameRateTextBox;
        private readonly NumberTextBox frameCountTextBox;
        private readonly NumberTextBox startFrameTextBox;
        private readonly NumberTextBox endFrameTextBox;
        private readonly NumberTextBox startSecondTextBox;
        private readonly NumberTextBox durationTextBox;
        private readonly NumberTextBox endSecondTextBox;
        private readonly NumberTextBox widthTextBox;
        private readonly NumberTextBox heightTextBox;
        private readonly StyledTextBox locationTextBox;
        private readonly StyledCheckBox overridePathCheckBox;
        private readonly StyledCheckBox relativeIndexCheckBox;
        private readonly StyledCheckBox prerenderCheckBox;
        private readonly StyledComboBox bufferComboBox;
        private readonly StyledRichTextBox pathsDocumentView;
        private readonly Run startPathRun;
        private readonly Run endPathRun;
        private readonly DispatcherTimer validatePathsTimer;
        private readonly TextBlock statusTextBlock;
        private readonly StyledButton renderButton;
        private readonly bool isInitialized;
        private string? projectPath;
        private bool isChanging;
        private bool pathsValidated;
        private bool isValid;
        private bool pathsExist;
        private int buffersCount;

        public RenderSequenceSettingsView(Window window, IApplicationTheme theme, Transform scrollBarTransform, IEnumerable<string> supportedExtensions, SaveFileDialog saveFileDialog)
        {
            this.window = window;
            this.theme = theme;
            this.saveFileDialog = saveFileDialog;
            this.supportedExtensions = new HashSet<string>(supportedExtensions);
            this.validatePathsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5), IsEnabled = false };
            this.validatePathsTimer.Tick += (sender, e) =>
            {
                ValidatePaths();
                ValidateValues();
            };

            this.frameRateTextBox = CreateNumberTextBox(25, 1, 1000, 1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                SetFramesValues();
                SetSecondsValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            this.frameCountTextBox = CreateNumberTextBox(100, 0, 1000, 1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                this.endFrameTextBox!.Value = this.startFrameTextBox!.Value + this.frameCountTextBox!.Value;
                this.endFrameTextBox!.MinValue = this.startFrameTextBox.Value;
                SetSecondsValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            this.startFrameTextBox = CreateNumberTextBox(0, 0, 1000, 1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                this.endFrameTextBox!.Value = this.startFrameTextBox!.Value + this.frameCountTextBox.Value;
                this.endFrameTextBox!.MinValue = this.startFrameTextBox.Value;
                SetSecondsValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            this.endFrameTextBox = CreateNumberTextBox(0, 0, 1000, 1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                this.frameCountTextBox.Value = this.endFrameTextBox!.Value - this.startFrameTextBox.Value;
                this.frameCountTextBox!.Value = this.frameCountTextBox.Value;
                SetSecondsValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            this.startSecondTextBox = CreateNumberTextBox(0, 0, 1000, 0.1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                this.endSecondTextBox!.Value = this.startSecondTextBox!.Value + this.durationTextBox!.Value;
                this.endSecondTextBox!.MinValue = this.startSecondTextBox!.Value;
                SetFramesValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            this.durationTextBox = CreateNumberTextBox(0, 0, 1000, 0.1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                this.endSecondTextBox!.Value = this.startSecondTextBox.Value + this.durationTextBox!.Value;
                this.endSecondTextBox.MinValue = this.startSecondTextBox!.Value;
                SetFramesValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            this.endSecondTextBox = CreateNumberTextBox(1, 0, 1000, 0.1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => TryChangeValues(() =>
            {
                var frameRate = Math.Max(this.frameRateTextBox!.Value, 1);
                this.durationTextBox.Value = this.endSecondTextBox!.Value - this.startSecondTextBox.Value;
                SetFramesValues();
                SetPathRangeRuns();
                ValidateValues();
            }));

            SetSecondsValues();
            SetFramesValues();
            SetPathRangeRuns();

            this.widthTextBox = CreateNumberTextBox(800, 1, 10000, 1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => ValidateValues());
            this.heightTextBox = CreateNumberTextBox(600, 1, 10000, 1).WithHandler(NumberTextBox.ValueChangedEvent, (sender, e) => ValidateValues()); ;

            this.locationTextBox = new StyledTextBox(theme)
            {
                Margin = new Thickness(Spacing.Left, Spacing.Top, Spacing.Top, Spacing.Bottom)
            }.
            WithHandler(TextBoxBase.TextChangedEvent, (sender, e) =>
            {
                SetPathRangeRuns();
                InvalidatePaths();
                ValidateValues();
            });

            this.overridePathCheckBox = new StyledCheckBox(theme)
            {
                Content = "Override Existing Files",
                Opacity = 0.5,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = Spacing,
                IsChecked = false,
            }.
            WithHandler(ButtonBase.ClickEvent, (sender, e) => ValidateValues());

            this.relativeIndexCheckBox = new StyledCheckBox(theme)
            {
                Content = "Relative Frame Index",
                Opacity = 0.5,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = Spacing,
                IsChecked = false,
            }.
            WithHandler(ButtonBase.ClickEvent, (sender, e) => SetPathRangeRuns());

            this.prerenderCheckBox = new StyledCheckBox(theme)
            {
                Content = "Pre-render From Frame 0",
                Margin = Spacing,
                Opacity = 0.5,
                VerticalContentAlignment = VerticalAlignment.Center,
                IsChecked = true
            }.
            WithHandler(ButtonBase.ClickEvent, (sender, e) => ValidateValues());

            this.bufferComboBox = new StyledComboBox(theme) { Margin = Spacing, SelectedIndex = 0 }.WithItems
            (
                new StyledComboBoxItem(theme) { Content = "Image" },
                new StyledComboBoxItem(theme) { Content = "Buffer4" },
                new StyledComboBoxItem(theme) { Content = "Buffer3" },
                new StyledComboBoxItem(theme) { Content = "Buffer2" },
                new StyledComboBoxItem(theme) { Content = "Buffer1" }
            );

            this.startPathRun = new Run();
            this.endPathRun = new Run();

            var pathsParagraph = new Paragraph();
            pathsParagraph.Inlines.Add(this.startPathRun);
            pathsParagraph.Inlines.Add(new LineBreak());
            pathsParagraph.Inlines.Add(this.endPathRun);

            this.pathsDocumentView = new StyledRichTextBox(theme)
            {
                Document = new FlowDocument(pathsParagraph),
                ScrollBarTransform = scrollBarTransform,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = Spacing
            }.
            WithReference(Control.ForegroundProperty, theme.WindowForeground).
            WithReference(Control.BackgroundProperty, theme.WindowBackground).
            WithReference(TextElement.FontSizeProperty, theme.CodeFontSize).
            WithReference(TextElement.FontFamilyProperty, theme.CodeFontFamily);

            var panel = new StackPanel { Margin = Spacing }.WithChildren
            (
                new StackPanel { Margin = RowSpacing }.WithChildren
                (
                    new DockPanel { LastChildFill = true }.WithChildren
                    (
                        new TextBlock { Text = "Location", Width = ColumnWidth, VerticalAlignment = VerticalAlignment.Center, Margin = Spacing }.WithDock(Dock.Left),
                        new StyledButton(theme) { Content = "...", Width = 30, Padding = new Thickness(8, 2, 8, 2), Margin = new Thickness(Spacing.Top, Spacing.Top, Spacing.Right, Spacing.Bottom), VerticalAlignment = VerticalAlignment.Center }.WithDock(Dock.Right).WithHandler(ButtonBase.ClickEvent, OnBrowseClicked),
                        this.locationTextBox
                    ),
                    new DockPanel { LastChildFill = true }.WithChildren(new FrameworkElement { Width = ColumnWidth, Margin = Spacing }.WithDock(Dock.Left), this.overridePathCheckBox),
                    new DockPanel { LastChildFill = true }.WithChildren(new FrameworkElement { Width = ColumnWidth, Margin = Spacing }.WithDock(Dock.Left), this.relativeIndexCheckBox)
                ),
                new DockPanel { LastChildFill = true, Margin = RowSpacing }.WithChildren
                (
                    new TextBlock { Text = "Buffer", Width = ColumnWidth, VerticalAlignment = VerticalAlignment.Center, Margin = Spacing }.WithDock(Dock.Left),
                    this.bufferComboBox
                ),
                new DockPanel { LastChildFill = true, Margin = RowSpacing }.WithChildren
                (
                    new TextBlock { Text = "Resolution", Width = ColumnWidth, Margin = Spacing }.WithDock(Dock.Left),
                    new ColumnPanel().WithChildren
                    (
                        new PropertyPanel(CreateHeader("Width"), this.widthTextBox),
                        new PropertyPanel(CreateHeader("Height"), this.heightTextBox),
                        new FrameworkElement()
                    )
                ),
                new DockPanel { LastChildFill = true }.WithChildren
                (
                    new TextBlock { Text = "Range", Width = ColumnWidth, Margin = Spacing }.WithDock(Dock.Left),
                    this.prerenderCheckBox.WithDock(Dock.Bottom),

                    new StackPanel().WithChildren
                    (
                        new ColumnPanel().WithChildren(new PropertyPanel(CreateHeader("FPS"), this.frameRateTextBox), new FrameworkElement(), new FrameworkElement()),
                        new ColumnPanel().WithChildren(new PropertyPanel(CreateHeader("Start Frame"), this.startFrameTextBox), new PropertyPanel(CreateHeader("Count"), this.frameCountTextBox), new PropertyPanel(CreateHeader("End Frame"), this.endFrameTextBox)),
                        new ColumnPanel().WithChildren(new PropertyPanel(CreateHeader("Start Second"), this.startSecondTextBox), new PropertyPanel(CreateHeader("Duration"), this.durationTextBox), new PropertyPanel(CreateHeader("End Second"), this.endSecondTextBox))
                    )
                ),
                new StackPanel().WithChildren
                (
                    new Border { Height = 1, Margin = new Thickness(-Spacing.Left, Spacing.Top, -Spacing.Right, Spacing.Bottom) }.WithReference(Border.BackgroundProperty, theme.Separator),
                    new TextBlock { Text = "Target Paths", Margin = Spacing },
                    this.pathsDocumentView
                )
            );

            this.statusTextBlock = new TextBlock
            {
                Margin = Spacing,
                VerticalAlignment = VerticalAlignment.Center
            }.
            WithReference(TextElement.ForegroundProperty, theme.WarningTextForeground);

            this.renderButton = new StyledButton(theme)
            {
                Content = "Render",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = Spacing
            }.
            WithHandler(ButtonBase.ClickEvent, OnRenderClicked);

            this.Content = new DockPanel { LastChildFill = true }.WithChildren
            (
                new Grid { Margin = Spacing }.WithDock(Dock.Bottom).WithChildren(this.statusTextBlock, this.renderButton),
                new StyledScrollViewer(theme.ScrollBar)
                {
                    ScrollBarTransform = scrollBarTransform,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = panel
                }
            );

            this.isInitialized = true;
        }

        public void SetProject(string? projectPath, IEnumerable<string> buffersDisplayNames, RenderSequenceSettings settings)
        {
            var projectChanged = this.projectPath == projectPath;

            this.projectPath = projectPath;

            this.locationTextBox.Text = settings.TargetPath ?? Path.Combine(Path.GetDirectoryName(projectPath)!, Path.GetFileNameWithoutExtension(projectPath) + "-0000.png");
            this.frameRateTextBox.Value = settings.FrameRate;
            this.frameCountTextBox.Value = settings.FrameCount;
            this.startFrameTextBox.Value = settings.StartFrame;
            this.widthTextBox.Value = settings.RenderSize.Width;
            this.heightTextBox.Value = settings.RenderSize.Height;
            this.prerenderCheckBox.IsChecked = settings.Prerender;
            this.overridePathCheckBox.IsChecked = this.overridePathCheckBox.IsChecked == true && !projectChanged;
            this.relativeIndexCheckBox.IsChecked = settings.RelativeIndex;

            this.buffersCount = buffersDisplayNames.Count();
            this.bufferComboBox.Items.Clear();

            foreach (var bufferName in buffersDisplayNames.Reverse())
            {
                this.bufferComboBox.Items.Add(new StyledComboBoxItem(this.theme) { Content = bufferName });
            }

            this.bufferComboBox.SelectedIndex = 0;

            ValidatePaths();
            this.window.Dispatcher.InvokeAsync(ValidateValues, DispatcherPriority.Render);
        }

        public RenderSequenceSettings GetSettings()
        {
            var targetPath = this.locationTextBox.Text;
            var overridePath = this.overridePathCheckBox.IsChecked == true;
            var bufferIndex = this.buffersCount - this.bufferComboBox.SelectedIndex - 1;
            var size = new RenderSize((int)this.widthTextBox.Value, (int)this.heightTextBox.Value);
            var frameRate = (int)this.frameRateTextBox.Value;
            var startFrame = (int)this.startFrameTextBox.Value;
            var frameCount = (int)this.frameCountTextBox.Value;
            var prerender = this.prerenderCheckBox.IsChecked == true;
            var relativeIndex = this.relativeIndexCheckBox.IsChecked == true;

            return new RenderSequenceSettings(targetPath, bufferIndex, frameRate, frameCount, startFrame, size, prerender, overridePath, relativeIndex);
        }

        private void OnRenderClicked(object sender, RoutedEventArgs e)
        {
            ValidatePaths();
            ValidateValues();

            if (this.isValid)
            {
                var bufferIndex = this.buffersCount - this.bufferComboBox.SelectedIndex - 1;

                RenderClicked?.Invoke(this, EventArgs.Empty);

                this.pathsValidated = false;
                this.overridePathCheckBox.IsChecked = false;
            }
        }

        private void OnBrowseClicked(object sender, RoutedEventArgs e)
        {
            var path = this.locationTextBox.Text;

            if (this.saveFileDialog.ShowDialog(this.window, path, false) == true)
            {
                this.locationTextBox.Text = this.saveFileDialog.FileName;
            }
        }

        private void TryChangeValues(Action action)
        {
            if (this.isChanging)
            {
                return;
            }

            this.isChanging = true;
            action();
            this.isChanging = false;

            ValidateValues();
        }

        private void SetSecondsValues()
        {
            var frameRate = Math.Max(this.frameRateTextBox!.Value, 1);
            var startSecond = this.startFrameTextBox.Value / frameRate;
            var duration = this.frameCountTextBox.Value / frameRate;

            this.startSecondTextBox.Value = startSecond;
            this.durationTextBox.Value = duration;
            this.endSecondTextBox.Value = startSecond + duration;
            this.endSecondTextBox.MinValue = startSecond;
        }

        private void SetFramesValues()
        {
            var frameRate = Math.Max(this.frameRateTextBox!.Value, 1);
            this.startFrameTextBox.Value = this.startSecondTextBox.Value * frameRate;
            this.frameCountTextBox.Value = this.durationTextBox.Value * frameRate;
            this.endFrameTextBox.Value = this.startFrameTextBox.Value + this.frameCountTextBox.Value;
            this.endFrameTextBox.MinValue = this.startFrameTextBox.Value;
        }

        private void SetPathRangeRuns()
        {
            if (!this.isInitialized || this.projectPath == null)
            {
                return;
            }

            var path = this.locationTextBox.Text;

            if (!Path.IsPathFullyQualified(path) || !PathExtensions.IsPathValid(path))
            {
                this.startPathRun.Text = null;
                this.endPathRun.Text = null;
                return;
            }

            var startIndex = (int)this.startFrameTextBox.Value;
            var endIndex = (int)this.endFrameTextBox.Value;

            if (this.relativeIndexCheckBox?.IsChecked == true)
            {
                endIndex -= startIndex;
                startIndex = 0;
            }

            var formattedPath = FormattedIndexedPath.Create(this.projectPath!, path, startIndex, Math.Max(1, endIndex - startIndex));

            var startPath = formattedPath.GetPath(startIndex);
            var endPath = formattedPath.GetPath(Math.Max(startIndex, endIndex - 1));

            var warning = this.pathsValidated && this.pathsExist ? " (range conflict)" : null;

            this.startPathRun.Text = startPath + warning;
            this.endPathRun.Text = endPath + warning;

            this.pathsDocumentView.WithReference(TextElement.ForegroundProperty, warning != null ? this.theme.WarningTextForeground : this.theme.WindowForeground);
        }

        private void InvalidatePaths()
        {
            this.pathsValidated = false;

            this.validatePathsTimer.Stop();
            this.validatePathsTimer.Start();
        }

        private void ValidatePaths()
        {
            this.validatePathsTimer.Stop();

            this.pathsValidated = true;
            this.pathsExist = false;

            var path = this.locationTextBox.Text;

            if (PathExtensions.IsPathValid(path) && Path.IsPathFullyQualified(path))
            {
                var startIndex = (int)this.startFrameTextBox.Value;
                var endIndex = (int)this.endFrameTextBox.Value;

                var formattedPath = FormattedIndexedPath.Create(this.projectPath!, path, startIndex, Math.Max(1, endIndex - startIndex));
                var directoryPath = Path.GetDirectoryName(formattedPath.SearchPattern);
                this.pathsExist = directoryPath != null && Path.Exists(directoryPath) && Directory.GetFiles(directoryPath, Path.GetFileName(formattedPath.SearchPattern)).Length > 0;
            }
        }

        private void ValidateValues()
        {
            if (!this.isInitialized || this.projectPath == null)
            {
                return;
            }

            this.isValid = false;
            this.renderButton.IsEnabled = false;
            this.statusTextBlock.Text = null;
            this.statusTextBlock.Visibility = Visibility.Visible;

            this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            this.overridePathCheckBox.SetReference(TextElement.ForegroundProperty, this.theme.WindowForeground);
            this.prerenderCheckBox.SetReference(TextElement.ForegroundProperty, this.theme.WindowForeground);
            this.frameCountTextBox.ClearValue(TextElement.ForegroundProperty);
            this.durationTextBox.ClearValue(TextElement.ForegroundProperty);
            this.widthTextBox.ClearValue(TextElement.ForegroundProperty);
            this.heightTextBox.ClearValue(TextElement.ForegroundProperty);
            this.frameRateTextBox.ClearValue(TextElement.ForegroundProperty);

            this.relativeIndexCheckBox.Opacity = this.startFrameTextBox.Value != 0 ? 1 : 0.5;
            this.prerenderCheckBox.Opacity = this.startFrameTextBox.Value > 0 ? 1 : 0.5;
            this.overridePathCheckBox.Opacity = this.pathsExist ? 1.0 : 0.5;

            var path = this.locationTextBox.Text;
            var extension = Path.GetExtension(path);

            SetPathRangeRuns();

            if (!PathExtensions.IsPathValid(path))
            {
                this.statusTextBlock.Text = "Target path contains invalid characters";
                this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (!Path.IsPathFullyQualified(path))
            {
                this.statusTextBlock.Text = "Target path is not fully qualified";
                this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (String.IsNullOrEmpty(extension))
            {
                this.statusTextBlock.Text = "Target path file extension is missing";
                this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (!this.supportedExtensions.Contains(extension))
            {
                this.statusTextBlock.Text = $"Target path extension \"{extension}\" is not supported (use one of {String.Join(", ", this.supportedExtensions)})";
                this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (this.frameCountTextBox.Value < 1.0)
            {
                this.statusTextBlock.Text = "Empty range";
                this.frameCountTextBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                this.durationTextBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (this.widthTextBox.Value < 1.0)
            {
                this.statusTextBlock.Text = "Invalid resolution";
                this.widthTextBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (this.heightTextBox.Value < 1.0)
            {
                this.statusTextBlock.Text = "Invalid resolution";
                this.heightTextBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (this.frameRateTextBox.Value < 1.0)
            {
                this.statusTextBlock.Text = "Invalid frame rate";
                this.frameRateTextBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (this.startFrameTextBox.Value < 0 && this.prerenderCheckBox.IsChecked == true)
            {
                this.statusTextBlock.Text = "Start frame is lower than 0";
                this.prerenderCheckBox.Opacity = 1.0;
                this.prerenderCheckBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (this.pathsValidated && this.pathsExist && this.overridePathCheckBox.IsChecked != true)
            {
                this.statusTextBlock.Text = "Target path range conflicts with existing files";
                this.overridePathCheckBox.SetReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (!this.pathsValidated)
            {
                return;
            }

            this.isValid = true;
            this.renderButton.IsEnabled = true;
            this.statusTextBlock.Visibility = Visibility.Collapsed;
        }

        public void SetDragSensitivity(double dragSensitivity)
        {
            this.frameRateTextBox.DragSensitivity = dragSensitivity;
            this.frameCountTextBox.DragSensitivity = dragSensitivity;
            this.startFrameTextBox.DragSensitivity = dragSensitivity;
            this.endFrameTextBox.DragSensitivity = dragSensitivity;
            this.startSecondTextBox.DragSensitivity = dragSensitivity;
            this.durationTextBox.DragSensitivity = dragSensitivity;
            this.endSecondTextBox.DragSensitivity = dragSensitivity;
            this.widthTextBox.DragSensitivity = dragSensitivity;
            this.heightTextBox.DragSensitivity = dragSensitivity;
        }

        private static TextBlock CreateHeader(string header)
        {
            return new TextBlock { Text = header, Margin = Spacing };
        }

        private NumberTextBox CreateNumberTextBox(double value, double minValue, double maxValue, double stepSize)
        {
            var textBox = new NumberTextBox
            {
                MinValue = minValue,
                MaxValue = maxValue,
                StepSize = stepSize,
                Value = value,
                Margin = Spacing
            };

            textBox.SetReference(NumberTextBox.ProgressTrackBrushProperty, this.theme.TextProgressTrack);
            textBox.SetReference(NumberTextBox.EditForegroundProperty, this.theme.TextEditForeground);
            textBox.SetReference(NumberTextBox.DragForegroundProperty, this.theme.TextDragForeground);
            textBox.SetReference(NumberTextBox.HoverBackgroundProperty, this.theme.ControlHoveredBackground);
            textBox.SetReference(NumberTextBox.PressedBackgroundProperty, this.theme.ControlPressedBackground);
            textBox.SetReference(NumberTextBox.SelectionBrushProperty, this.theme.TextSelectionBackground);
            textBox.SetReference(NumberTextBox.SelectionOpacityProperty, this.theme.TextSelectionOpacity);
            textBox.SetReference(TextElement.FontFamilyProperty, this.theme.CodeFontFamily);

            return textBox;
        }
    }
}
