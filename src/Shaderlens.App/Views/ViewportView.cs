namespace Shaderlens.Views
{
    using System.Windows;
    using static WinApi;

    public interface IViewportView
    {
        IFrameRateTarget FrameRateTarget { get; }
        IMousePositionSource MousePositionSource { get; }

        void Show();
        void SetProjectName(string name);
        void SetProject(IProject project);
        void SetProjectChangeState(bool isChanged);
        void SetViewerBufferSize(int bufferWidth, int bufferHeight);
        void SetTheme(IApplicationTheme theme);
        void ResetViewerTransform();
        bool IsVisible();
        IProjectLoadLogger CreateLogger(string projectPath);
    }

    public class ViewportView : IViewportView
    {
        private class ViewportFrameRateTarget : IFrameRateTarget
        {
            public FrameIndex FrameIndex { get; private set; }
            public double Rate { get; private set; }
            public double Average { get; private set; }

            private readonly ViewportView viewportView;
            private readonly IDispatcherThread viewThread;

            public ViewportFrameRateTarget(ViewportView viewportView, IDispatcherThread viewThread)
            {
                this.viewportView = viewportView;
                this.viewThread = viewThread;
            }

            public void SetFrameRate(FrameIndex frameIndex, double rate, double average)
            {
                this.viewThread.DispatchAsync(() =>
                {
                    this.FrameIndex = frameIndex;
                    this.Rate = rate;
                    this.Average = average;
                    this.viewportView.SetStatistics();
                });
            }
        }

        private class MenuContainer
        {
            private readonly ViewportView viewportView;
            private readonly IMenuSource menuSource;
            private readonly IMenuTheme theme;

            private StyledContextMenu? menu;
            private IDisposable? inputScope;

            public MenuContainer(ViewportView viewportView, IMenuSource menuSource, IMenuTheme theme)
            {
                this.viewportView = viewportView;
                this.menuSource = menuSource;
                this.theme = theme;
            }

            public void Open()
            {
                if (this.menu == null)
                {
                    this.menu = new StyledContextMenu(this.theme)
                    {
                        Resources = this.viewportView.window.Resources,
                        LayoutTransform = this.viewportView.scaleBehavior.Transform,
                        Visibility = Visibility.Hidden
                    };

                    this.menu.Opened += OnOpened;
                    this.menu.Closed += OnClosed;

                    MenuPreserveSelectionBehavior.Register(this.menu);

                    var builder = new MenuBuilder(this.menu, this.theme);
                    this.menuSource.AddTo(builder);
                }

                if (!this.menu.IsOpen)
                {
                    this.menu.IsOpen = true;
                }
            }

            private void OnOpened(object sender, RoutedEventArgs e)
            {
                this.viewportView.application.ClearKeysDownState();
                this.inputScope = this.viewportView.inputBindings.PushScope();
                this.menu!.Dispatcher.InvokeAsync(() => { this.menu.Visibility = Visibility.Visible; this.menu.Focus(); }, DispatcherPriority.Background);
            }

            private void OnClosed(object sender, RoutedEventArgs e)
            {
                this.inputScope?.Dispose();
                this.inputScope = null;
                this.viewportView.inputStateSource.Refresh();
            }
        }

        private const string ApplicationName = "Shaderlens";

        public IFrameRateTarget FrameRateTarget { get { return this.frameRateTarget; } }
        public IMousePositionSource MousePositionSource { get { return this.viewerMousePositionSource; } }

        private readonly Window window;
        private readonly IDispatcherThread viewThread;
        private readonly IApplication application;
        private readonly IApplicationSettings settings;
        private readonly IApplicationInputs inputs;
        private readonly IApplicationCommands commands;
        private readonly InputBindings inputBindings;
        private readonly InputStateSource inputStateSource;
        private readonly InputPositionBinding inputPositionBinding;
        private readonly StatisticsFormatter tooltipFormatter;
        private readonly StatisticsFormatter titleFormatter;
        private readonly char[] titleBuffer;
        private readonly ViewportFrameRateTarget frameRateTarget;
        private readonly InputDisplayNameFormatter inputDisplayNameFormatter;
        private readonly ViewerTransform viewerTransform;
        private readonly Decorator windowContent;
        private readonly DispatcherTimer viewValidationTimer;
        private readonly DispatcherTimer cursorVisibilityTimer;
        private readonly DispatcherTimer statisticsTooltipTimer;
        private readonly AbsoluteMousePositionSource mousePositionSource;
        private readonly TransformedMousePositionSource viewerMousePositionSource;
        private readonly Grid loggerPanel;
        private readonly Panel windowContentPanel;
        private readonly Border windowContentBackground;
        private readonly Panel resizePanel;
        private readonly TextBlock downscaleTextBlock;
        private readonly Border resizeMessageBorder;
        private readonly TextBlock frameRateTextBlock;
        private readonly TextBlock pixelCountTextBlock;
        private readonly IScaleBehavior scaleBehavior;
        private readonly CopyMenuState menuCopyState;
        private readonly ToolTip statisticsTooltip;
        private readonly TextBlock statisticsTooltipContent;

        private readonly MenuContainer exportMenu;
        private readonly MenuContainer resolutionMenu;
        private readonly MenuContainer frameRateMenu;
        private readonly MenuContainer speedMenu;
        private readonly MenuContainer optionsMenu;

        private MenuContainer contextMenu;
        private MenuContainer recentProjectsMenu;
        private MenuContainer projectFilesMenu;
        private MenuContainer buffersMenu;
        private MenuContainer viewerMenu;
        private MenuContainer copyMenu;
        private IProject? project;

        private IApplicationTheme theme;
        private Color windowTitleBackground;
        private Point initialSize;
        private bool isResizing;
        private bool isFullScreen;
        private WindowState lastFullScreenWindowState;
        private IDisposable? resizeInputScope;
        private bool resumePipelineOnResizeEnd;
        private int viewportWidth;
        private int viewportHeight;
        private IntPtr handle;
        private PresentationSource? presentationSource;
        private Key skipKeyDownRepeat;
        private int viewValidationCount;
        private DateTime cursorVisibilityTime;
        private string? projectName;
        private bool isProjectChanged;
        private int bufferWidth;
        private int bufferHeight;
        private Point normalWindowPosition;
        private Size normalWindowSize;
        private Key lastProcessedKeyDown;

        public ViewportView(Window window, IApplication application, IApplicationSettings settings, IApplicationInputs inputs, IApplicationCommands commands, IApplicationTheme theme, IDispatcherThread viewThread)
        {
            this.window = window;
            this.viewThread = viewThread;
            this.application = application;
            this.settings = settings;
            this.inputs = inputs;
            this.commands = commands;

            this.theme = theme;
            this.theme.SetResources(this.window.Resources);
            this.window.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            this.window.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            this.window.SetReference(TextElement.FontSizeProperty, this.theme.WindowFontSize);
            this.window.SetReference(TextElement.FontFamilyProperty, this.theme.WindowFontFamily);

            this.window.Title = ApplicationName;
            this.window.SnapsToDevicePixels = true;
            this.window.WindowState = settings.ViewportWindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
            this.window.AllowDrop = true;

            MouseHoverKeyEventBehavior.Register(this.window);

            this.window.SourceInitialized += OnSourceInitialized;
            this.window.Activated += OnActivated;
            this.window.Closing += OnClosing;
            this.window.PreviewDragEnter += (sender, e) => e.Handled = true;
            this.window.PreviewDragOver += (sender, e) => e.Handled = true;
            this.window.PreviewDrop += OnPreviewDrop;
            this.window.IsVisibleChanged += OnIsVisibleChanged;
            this.window.LocationChanged += OnLocationChanged;
            this.window.SizeChanged += OnSizeChanged;
            this.window.StateChanged += OnStateChanged;
            this.window.PreviewMouseDown += OnPreviewMouseDown;
            this.window.PreviewMouseUp += OnPreviewMouseUp;
            this.window.PreviewMouseMove += OnPreviewMouseMove;
            this.window.PreviewMouseWheel += OnPreviewMouseWheel;
            this.window.PreviewKeyDown += OnPreviewKeyDown;
            this.window.PreviewKeyUp += OnPreviewKeyUp;

            this.windowTitleBackground = theme.WindowTitleBackground.Value;

            this.scaleBehavior = ScaleBehavior.Register(this.window);
            this.scaleBehavior.Scale = settings.ViewportWindowState.Scale;

            this.mousePositionSource = new AbsoluteMousePositionSource(this.window);
            this.viewerMousePositionSource = new TransformedMousePositionSource(this.mousePositionSource);
            this.inputPositionBinding = new InputPositionBinding(this.window.Dispatcher, this.mousePositionSource);
            this.inputBindings = new InputBindings();
            this.inputStateSource = new InputStateSource(this.window, this.inputBindings);

            this.frameRateTarget = new ViewportFrameRateTarget(this, this.viewThread);

            this.tooltipFormatter = new StatisticsFormatter(Environment.NewLine);
            this.titleFormatter = new StatisticsFormatter(" | ");
            this.titleBuffer = new char[2000];

            this.statisticsTooltipContent = new TextBlock();
            this.statisticsTooltip = new ToolTip { Content = this.statisticsTooltipContent };
            this.inputDisplayNameFormatter = new InputDisplayNameFormatter(new InputValueSerializer());

            this.frameRateTextBlock = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center };
            this.pixelCountTextBlock = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, };
            this.downscaleTextBlock = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, Visibility = Visibility.Collapsed, Opacity = 0.5 };

            this.resizeMessageBorder = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 4, 8, 4),
                Child = new StackPanel().WithChildren(this.downscaleTextBlock, this.frameRateTextBlock, this.pixelCountTextBlock),
                Opacity = 0.8,
            }.
            WithReference(Border.BackgroundProperty, theme.WindowMessageBackground);

            this.resizePanel = new Grid { Visibility = Visibility.Collapsed };
            this.resizePanel.Children.Add(this.resizeMessageBorder);
            this.resizePanel.Children.Add(new TextBlock
            {
                Text = FormatResizeMessage(this.inputDisplayNameFormatter, this.inputs),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8),
                Opacity = 0.4,
            });

            this.viewValidationTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.05), DispatcherPriority.Normal, OnViewValidationTick, this.window.Dispatcher) { IsEnabled = false };

            this.cursorVisibilityTimer = new DispatcherTimer { IsEnabled = false, Interval = TimeSpan.FromSeconds(0.2) };
            this.cursorVisibilityTimer.Tick += OnCursorVisibilityTick;

            this.statisticsTooltipTimer = new DispatcherTimer { IsEnabled = false, Interval = TimeSpan.FromSeconds(0.5) };
            this.statisticsTooltipTimer.Tick += OnStatisticsTooltipTick;

            var backgroundImageContainer = new Decorator();
            this.loggerPanel = new Grid();

            this.windowContentBackground = new Border().WithReference(Border.BackgroundProperty, theme.WindowBackground);
            this.windowContentPanel = new Grid { LayoutTransform = this.scaleBehavior.Transform }.WithChildren
            (
                this.windowContentBackground,
                this.loggerPanel,
                this.resizePanel
            );

            this.windowContent = new Decorator { Child = this.windowContentPanel };
            this.window.Content = this.windowContent;

            this.menuCopyState = new CopyMenuState(this.application);
            this.viewerTransform = new ViewerTransform();

            this.commands.AddInputBindings(this.inputBindings);

            var menuResources = new MenuResourcesFactory(theme.Menu);
            this.contextMenu = new MenuContainer(this, new ViewportMenuSource(this.application, this.inputs, this.commands, menuResources, this.menuCopyState, null, this.theme.Menu), this.theme.Menu);
            this.recentProjectsMenu = new MenuContainer(this, new HeaderedMenuSource(new RecentProjectsMenuSource(this.application, this.inputs, menuResources), "Open Recent", menuResources.CreateRecentIcon()), this.theme.Menu);
            this.projectFilesMenu = new MenuContainer(this, new HeaderedMenuSource(new ProjectFilesMenuSource(this.application, this.inputs, menuResources, null), "Project Files", menuResources.CreateFilesIcon()), this.theme.Menu);
            this.buffersMenu = new MenuContainer(this, new HeaderedMenuSource(new BuffersMenuSource(this.application, inputs, null), "Buffers", menuResources.CreateBuffersIcon()), this.theme.Menu);
            this.exportMenu = new MenuContainer(this, new HeaderedMenuSource(new ExportMenuSource(this.application, this.commands, this.theme.Menu), "Export", menuResources.CreateExportIcon()), this.theme.Menu);
            this.copyMenu = new MenuContainer(this, new HeaderedMenuSource(new CopyMenuSource(this.application, this.inputs, this.menuCopyState, this.theme.Menu), "Copy", menuResources.CreateCopyIcon()), this.theme.Menu);
            this.resolutionMenu = new MenuContainer(this, new HeaderedMenuSource(new ResolutionMenuSource(this.application, this.commands), "Resolution", menuResources.CreateResolutionIcon()), this.theme.Menu);
            this.frameRateMenu = new MenuContainer(this, new HeaderedMenuSource(new FrameRateMenuSource(this.application, this.commands), "Frame Rate", menuResources.CreateFrameRateIcon()), this.theme.Menu);
            this.speedMenu = new MenuContainer(this, new HeaderedMenuSource(new SpeedMenuSource(this.application, this.commands), "Speed", menuResources.CreateSpeedIcon()), this.theme.Menu);
            this.viewerMenu = new MenuContainer(this, new HeaderedMenuSource(new ViewerMenuSource(this.application, this.commands, null), "Viewer", menuResources.CreateViewerIcon()), this.theme.Menu);
            this.optionsMenu = new MenuContainer(this, new HeaderedMenuSource(new OptionsMenuSource(this.application, this.commands, menuResources), "Options", menuResources.CreateOptionsIcon()), this.theme.Menu);

            this.inputBindings.AddSpanEnd(this.inputs.MenuMain, () => { ResetCopySource(); this.contextMenu.Open(); }, true);
            this.inputBindings.AddSpanEnd(this.inputs.MenuRecentProjects, () => this.recentProjectsMenu.Open());
            this.inputBindings.AddSpanEnd(this.inputs.MenuProjectFiles, () => { if (application.IsPartiallyLoaded) { this.projectFilesMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuBuffers, () => { if (application.IsFullyLoaded) { this.buffersMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuExport, () => { if (application.IsFullyLoaded) { this.exportMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuCopy, CopySelect);
            this.inputBindings.AddSpanEnd(this.inputs.MenuResolution, () => { if (application.IsFullyLoaded) { this.resolutionMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuFrameRate, () => { if (application.IsFullyLoaded) { this.frameRateMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuSpeed, () => { if (application.IsFullyLoaded) { this.speedMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuViewer, () => { if (application.IsFullyLoaded) { this.viewerMenu.Open(); } });
            this.inputBindings.AddSpanEnd(this.inputs.MenuOptions, () => this.optionsMenu.Open());

            this.inputBindings.AddRenderSpanStart(this.inputs.ShaderMouseState, this.application, ShaderMouseStateStart);
            this.inputBindings.AddRenderSpanStart(this.inputs.ViewerPan, this.application, ViewerPanStart);
            this.inputBindings.AddRenderSpanStart(this.inputs.ViewerScale, this.application, ViewerScaleStart);
            this.inputBindings.AddRenderSpanStart(this.inputs.ViewerScaleUp, this.application, e => SetViewerScale(e, true));
            this.inputBindings.AddRenderSpanStart(this.inputs.ViewerScaleDown, this.application, e => SetViewerScale(e, false));
            this.inputBindings.AddRenderSpanStart(this.inputs.ViewerScaleReset, this.application, ResetViewerScale);

            this.inputBindings.AddRenderSpanStart(this.inputs.CopyFrame, this.application, CopyFrame);
            this.inputBindings.AddRenderSpanStart(this.inputs.CopyFrameWithAlpha, this.application, CopyFrameWithAlpha);
            this.inputBindings.AddRenderSpanStart(this.inputs.CopyRepeat, this.application, CopyRepeat);

            this.inputBindings.AddRenderSpanStart(this.inputs.ExportFrame, this.application, this.application.ExportFrame);
            this.inputBindings.AddRenderSpanStart(this.inputs.ExportFrameRepeat, this.application, this.application.ExportFrameRepeat);

            this.inputBindings.AddSpanStart(this.inputs.FullScreenToggle, ToggleFullScreen);
            this.inputBindings.AddSpanStart(this.inputs.FullScreenLeave, ExitFullScreen);

            this.window.Focus();
        }

        public void Show()
        {
            this.window.Show();

            if (this.window.WindowState == WindowState.Minimized)
            {
                this.window.WindowState = WindowState.Normal;
            }

            this.window.Activate();
        }

        public void SetProjectName(string name)
        {
            this.projectName = name;
            SetStatistics();
        }

        public void SetProjectChangeState(bool isChanged)
        {
            this.isProjectChanged = isChanged;
            SetStatistics();
        }

        public void SetProject(IProject project)
        {
            this.project = project;

            var menuResources = new MenuResourcesFactory(this.theme.Menu);
            this.contextMenu = new MenuContainer(this, new ViewportMenuSource(this.application, this.inputs, this.commands, menuResources, this.menuCopyState, this.project.Source, this.theme.Menu), this.theme.Menu);
            this.recentProjectsMenu = new MenuContainer(this, new HeaderedMenuSource(new RecentProjectsMenuSource(this.application, this.inputs, menuResources), "Open Recent", menuResources.CreateRecentIcon()), this.theme.Menu);
            this.projectFilesMenu = new MenuContainer(this, new HeaderedMenuSource(new ProjectFilesMenuSource(this.application, this.inputs, menuResources, this.project.Source), "Project Files", menuResources.CreateFilesIcon()), this.theme.Menu);
            this.buffersMenu = new MenuContainer(this, new HeaderedMenuSource(new BuffersMenuSource(this.application, this.inputs, this.project.Source), "Buffers", menuResources.CreateBuffersIcon()), this.theme.Menu);
            this.viewerMenu = new MenuContainer(this, new HeaderedMenuSource(new ViewerMenuSource(this.application, this.commands, this.project.Source), "Viewer", menuResources.CreateBuffersIcon()), this.theme.Menu);
            this.copyMenu = new MenuContainer(this, new HeaderedMenuSource(new CopyMenuSource(this.application, this.inputs, this.menuCopyState, this.theme.Menu), "Copy", menuResources.CreateCopyIcon()), this.theme.Menu);

            this.scaleBehavior.IsEnabled = !this.project.IsFullyLoaded;
            this.window.Topmost = this.application.AlwaysOnTop;

            InvalidateView();
        }

        public void SetViewerBufferSize(int viewerBufferWidth, int viewerBufferHeight)
        {
            this.bufferWidth = viewerBufferWidth;
            this.bufferHeight = viewerBufferHeight;
            this.viewerTransform.SetSize(viewerBufferWidth, viewerBufferHeight, this.viewportWidth, this.viewportHeight);
            SetViewerTransform();
        }

        public void SetTheme(IApplicationTheme theme)
        {
            this.theme = theme;
            this.theme.SetResources(this.window.Resources);
            this.windowTitleBackground = theme.WindowTitleBackground.Value;

            SetWindowCaptionColor(this.handle, GetColorValue(this.windowTitleBackground));

            InvalidateView();
        }

        public IProjectLoadLogger CreateLogger(string projectPath)
        {
            var loggerContainer = new Decorator();
            var imageContainer = new Decorator { LayoutTransform = this.scaleBehavior.InverseTransform };

            this.loggerPanel.Children.Clear();
            this.loggerPanel.Children.Add(imageContainer);
            this.loggerPanel.Children.Add(loggerContainer);

            var loggerResourcesFactory = new LoggerResourcesFactory(this.theme.Log);

            IProjectLoadLogger logger = new ProjectLoadLogger(loggerContainer, this.application, this.scaleBehavior.InverseTransform, StopwatchTimeSource.Instance, loggerResourcesFactory, this.theme, projectPath, this.settings.LogErrorContextLines, TimeSpan.FromSeconds(this.settings.LogVisibilityDelaySeconds), true);
            logger = new BackgroundImageLogger(imageContainer, this.application, logger, this.theme.Log);
            logger = new DispatcherLogger(logger, this.viewThread);

            InvalidateView();

            return logger;
        }

        public void ResetViewerTransform()
        {
            this.viewerTransform.SetScale(1.0);
            this.viewerTransform.SetOffset(default);

            SetViewerTransform();
        }

        public bool IsVisible()
        {
            if (this.window.WindowState == WindowState.Minimized)
            {
                return false;
            }

            GetWindowRect(this.handle, out var rect);

            return WindowFromPoint(new POINT { X = (rect.Left + rect.Right) / 2, Y = (rect.Top + rect.Bottom) / 2 }) == this.handle ||
                   WindowFromPoint(new POINT { X = rect.Left + 10, Y = rect.Top + 10 }) == this.handle ||
                   WindowFromPoint(new POINT { X = rect.Right - 10, Y = rect.Top + 10 }) == this.handle ||
                   WindowFromPoint(new POINT { X = rect.Left + 10, Y = rect.Bottom - 10 }) == this.handle ||
                   WindowFromPoint(new POINT { X = rect.Right - 10, Y = rect.Bottom - 10 }) == this.handle;
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            this.handle = new WindowInteropHelper(this.window).Handle;
            this.presentationSource = PresentationSource.FromVisual(this.window);

            var hwndSource = HwndSource.FromHwnd(this.handle);
            hwndSource.AddHook(new HwndSourceHook(WndProc));

            SetWindowCaptionColor(this.handle, GetColorValue(this.windowTitleBackground));

            var locaiton = this.presentationSource.CompositionTarget.TransformFromDevice.Transform(this.settings.ViewportWindowState.Position);
            var size = this.presentationSource.CompositionTarget.TransformFromDevice.Transform(new Point(this.settings.ViewportWindowState.Size.Width, this.settings.ViewportWindowState.Size.Height));

            this.window.Left = locaiton.X;
            this.window.Top = locaiton.Y;
            this.window.Width = size.X;
            this.window.Height = size.Y;

            this.application.Start(this.handle);

            SetViewportSize();
        }

        private void OnActivated(object? sender, EventArgs e)
        {
            this.inputStateSource.Refresh();

            this.statisticsTooltipTimer.Stop();
            this.statisticsTooltipTimer.Start();
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            if (!this.application.ShowProjectCloseDialog())
            {
                e.Cancel = true;
                return;
            }

            if (this.window.WindowState == WindowState.Maximized)
            {
                var size = GetDevicePosition(new Point(this.normalWindowSize.Width, this.normalWindowSize.Height));

                this.settings.ViewportWindowState.Position = GetDevicePosition(this.normalWindowPosition);
                this.settings.ViewportWindowState.Size = new Size(Math.Round(size.X), Math.Round(size.Y));
            }
            else if (GetWindowRect(this.handle, out var rect))
            {
                this.settings.ViewportWindowState.Position = new Point(rect.Left, rect.Top);
                this.settings.ViewportWindowState.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            this.settings.ViewportWindowState.Scale = this.scaleBehavior.Scale;
            this.settings.ViewportWindowState.Maximized = this.window.WindowState == WindowState.Maximized;
        }

        private void OnPreviewDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files?.Length == 1)
            {
                this.application.OpenProject(files[0]);
                e.Handled = true;
            }
        }

        private void OnEnterSizeMove()
        {
            this.isResizing = true;
            this.resizeInputScope = this.inputBindings.PushScope();

            this.resumePipelineOnResizeEnd = !this.application.IsPaused;

            this.resizePanel.Visibility = Visibility.Visible;
            this.loggerPanel.Visibility = Visibility.Collapsed;

            var size = GetDevicePosition(new Point(this.windowContent.ActualWidth, this.windowContent.ActualHeight));
            SetResolutionTextBlock(size);

            this.initialSize = size;

            this.application.PausePipeline();

            InvalidateView();
        }

        private void OnLocationChanged(object? sender, EventArgs e)
        {
            if (this.window.IsVisible)
            {
                this.viewerMousePositionSource.SetViewportPosition(this.window.PointToScreen(new Point()));
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.window.IsVisible)
            {
                this.viewerMousePositionSource.SetViewportPosition(this.window.PointToScreen(new Point()));
            }
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            if (this.window.WindowState != WindowState.Maximized && this.window.WindowStyle == WindowStyle.None)
            {
                this.window.ResizeMode = ResizeMode.CanResize;
                this.window.WindowStyle = WindowStyle.SingleBorderWindow;
                this.window.Topmost = this.application.AlwaysOnTop && this.project != null;
                this.isFullScreen = false;
            }

            if (this.window.WindowState != WindowState.Minimized)
            {
                this.viewerMousePositionSource.SetViewportPosition(this.window.PointToScreen(new Point()));
                InvalidateView();
            }

            if (this.window.WindowState == WindowState.Maximized)
            {
                this.normalWindowPosition = new Point(this.window.Left, this.window.Top);
                this.normalWindowSize = new Size(this.window.Width, this.window.Height);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.isResizing)
            {
                var size = GetDevicePosition(new Point(this.windowContent.ActualWidth, this.windowContent.ActualHeight));
                SetResolutionTextBlock(size);
            }
            else
            {
                SetViewportSize();
            }
        }

        private void OnSizing(ref Size clientSize, bool horizonalChange, bool verticalChange, ref bool handled)
        {
            this.inputStateSource.Refresh();

            var snapSmall = this.inputStateSource.IsMatch(this.inputs.ResizeSnapSmall);
            var snapMedium = this.inputStateSource.IsMatch(this.inputs.ResizeSnapMedium);
            var keepRatio = this.inputStateSource.IsMatch(this.inputs.ResizeKeepRatio);

            if (!snapSmall && !snapMedium && !keepRatio)
            {
                return;
            }

            var snapSize =
                snapMedium && snapSmall ? 100 :
                snapMedium ? 10 :
                snapSmall ? 1 :
                0;

            clientSize.Width /= this.application.RenderDownscale;
            clientSize.Height /= this.application.RenderDownscale;

            if (verticalChange)
            {
                if (snapSize > 0)
                {
                    clientSize.Height = Math.Round(clientSize.Height / snapSize) * snapSize;
                }

                if (keepRatio && this.initialSize.Y > 0)
                {
                    clientSize.Width = clientSize.Height * this.initialSize.X / this.initialSize.Y;

                    if (snapSize > 0)
                    {
                        clientSize.Width = Math.Round(clientSize.Width / snapSize) * snapSize;
                    }
                }
            }

            if (horizonalChange)
            {
                if (snapSize > 0)
                {
                    clientSize.Width = Math.Round(clientSize.Width / snapSize) * snapSize;
                }

                if (keepRatio && this.initialSize.X > 0)
                {
                    clientSize.Height = clientSize.Width * this.initialSize.Y / this.initialSize.X;

                    if (snapSize > 0)
                    {
                        clientSize.Height = Math.Round(clientSize.Height / snapSize) * snapSize;
                    }
                }
            }

            clientSize.Width = Math.Max(snapSize, clientSize.Width);
            clientSize.Height = Math.Max(snapSize, clientSize.Height);

            clientSize.Width *= this.application.RenderDownscale;
            clientSize.Height *= this.application.RenderDownscale;

            handled = true;
        }

        private void OnExitSizeMove()
        {
            this.isResizing = false;
            this.resizeInputScope?.Dispose();
            this.resizeInputScope = null;

            this.resizePanel.Visibility = Visibility.Collapsed;
            this.loggerPanel.Visibility = Visibility.Visible;

            SetViewportSize();

            if (this.resumePipelineOnResizeEnd)
            {
                this.application.ResumePipeline();
            }
            else
            {
                InvalidateView();
            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ShowCursor();
            this.inputPositionBinding.ProcessPosition();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            this.lastProcessedKeyDown = key;
            this.inputStateSource.ProcessInputEvent(e);

            if (e.IsRepeat && this.skipKeyDownRepeat == key)
            {
                e.Handled = true;
                return;
            }

            if (!e.Handled && KeyMap.TryGetIndex(key, out var index))
            {
                this.application.SetKeyState(index, true);
            }

            e.Handled |= e.Key == Key.Tab || e.Key == Key.System && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt);
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);

            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (KeyMap.TryGetIndex(key, out var index))
            {
                this.application.SetKeyState(index, false);
            }
        }

        private void OnViewValidationTick(object? sender, EventArgs e)
        {
            this.viewValidationCount--;

            ValidateView();

            if (this.viewValidationCount <= 0)
            {
                this.viewValidationTimer.Stop();
            }
        }

        private void SetViewportSize()
        {
            if (this.presentationSource != null)
            {
                var size = GetDevicePosition(new Point(this.windowContent.ActualWidth, this.windowContent.ActualHeight));

                this.viewportWidth = (int)Math.Round(size.X);
                this.viewportHeight = (int)Math.Round(size.Y);

                this.application.SetViewportSize(this.viewportWidth, this.viewportHeight);
                this.viewerMousePositionSource.SetViewportSize(this.viewportHeight);

                SetViewerTransform();

                this.application.RenderFrame();
            }
        }

        private void SetResolutionTextBlock(Point size)
        {
            var x = (int)Math.Floor(Math.Round(size.X) / this.application.RenderDownscale);
            var y = (int)Math.Floor(Math.Round(size.Y) / this.application.RenderDownscale);

            this.downscaleTextBlock.Text = $"1/{this.application.RenderDownscale} resolution";
            this.downscaleTextBlock.Visibility = this.application.RenderDownscale == 1 ? Visibility.Collapsed : Visibility.Visible;
            this.frameRateTextBlock.Text = $"{x}\u00d7{y}";
            this.pixelCountTextBlock.Text = $"{x * y:n0} pixels";
        }

        private void InvalidateView()
        {
            this.viewValidationCount = 10;
            this.viewValidationTimer.Start();
            ValidateView();
        }

        private void ValidateView()
        {
            if (this.application.IsFullyLoaded && !this.isResizing)
            {
                this.application.RenderViewerFrame();
                this.windowContentBackground.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.windowContentBackground.Visibility = this.viewValidationCount > 0 && this.windowContentBackground.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ShowCursor()
        {
            this.window.Cursor = null;
            this.cursorVisibilityTime = DateTime.Now;
            this.cursorVisibilityTimer.IsEnabled = this.settings.CursorVisibilityTimeoutSeconds > 0.0;
        }

        private void OnCursorVisibilityTick(object? sender, EventArgs e)
        {
            if (!this.application.IsFullyLoaded)
            {
                this.cursorVisibilityTimer.IsEnabled = false;
                return;
            }

            if ((DateTime.Now - this.cursorVisibilityTime).TotalSeconds > this.settings.CursorVisibilityTimeoutSeconds &&
                !this.inputStateSource.IsMatch(this.inputs.ShaderMouseState))
            {
                this.window.Cursor = Cursors.None;
                this.cursorVisibilityTimer.IsEnabled = false;
            }
        }

        private void OnStatisticsTooltipTick(object? sender, EventArgs e)
        {
            this.statisticsTooltipTimer.Stop();

            if (this.application.IsFullyLoaded && IsCursorOverTitleBar(this.handle))
            {
                this.statisticsTooltip.IsOpen = true;
                SetStatistics();
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_ENTERSIZEMOVE)
            {
                var titleBarInfo = new TITLEBARINFO { cbSize = Marshal.SizeOf<TITLEBARINFO>() };

                if (!this.isResizing && GetCursorPos(out var position) && GetTitleBarInfo(hWnd, ref titleBarInfo) &&
                    !titleBarInfo.rcTitleBar.AddMargin(0, 2, 0, 4).Contains(position))
                {
                    OnEnterSizeMove();
                }
            }

            if (msg == WM_SIZING)
            {
                if (!this.isResizing)
                {
                    OnEnterSizeMove();
                }

                GetWindowRect(hWnd, out var windowRect);
                GetClientRect(hWnd, out var clientRect);

                var horizonalChange = wParam != WMSZ_TOP && wParam != WMSZ_BOTTOM;
                var verticalChange = wParam != WMSZ_LEFT && wParam != WMSZ_RIGHT;

                var borderSize = new Size(
                    Math.Max(0.0, windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left)),
                    Math.Max(0.0, windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top)));

                var rect = Marshal.PtrToStructure<RECT>(lParam);

                if (wParam >= WMSZ_LEFT && wParam <= WMSZ_BOTTOMRIGHT &&
                    rect.Right - rect.Left > borderSize.Width &&
                    rect.Bottom - rect.Top > borderSize.Height)
                {
                    var clientSize = new Size(rect.Right - rect.Left - borderSize.Width, rect.Bottom - rect.Top - borderSize.Height);

                    OnSizing(ref clientSize, horizonalChange, verticalChange, ref handled);

                    if (handled)
                    {
                        var windowSize = new Size(clientSize.Width + borderSize.Width, clientSize.Height + borderSize.Height);

                        if (wParam == WMSZ_LEFT || wParam == WMSZ_TOPLEFT || wParam == WMSZ_BOTTOMLEFT)
                        {
                            rect.Left = rect.Right - (int)windowSize.Width;
                        }
                        else
                        {
                            rect.Right = rect.Left + (int)windowSize.Width;
                        }

                        if (wParam == WMSZ_TOP || wParam == WMSZ_TOPLEFT || wParam == WMSZ_TOPRIGHT)
                        {
                            rect.Top = rect.Bottom - (int)windowSize.Height;
                        }
                        else
                        {
                            rect.Bottom = rect.Top + (int)windowSize.Height;
                        }

                        Marshal.StructureToPtr(rect, lParam, true);
                    }
                }
            }

            if (msg == WM_EXITSIZEMOVE)
            {
                OnExitSizeMove();
            }

            if (msg == WM_NCMOUSEMOVE)
            {
                this.statisticsTooltipTimer.Stop();
                this.statisticsTooltipTimer.Start();
            }

            if (msg == WM_NCMOUSELEAVE)
            {
                this.statisticsTooltipTimer.Stop();
                this.statisticsTooltip.IsOpen = false;
            }

            return IntPtr.Zero;
        }

        private void ViewerPanStart(InputSpanEventArgs e)
        {
            if (!this.application.IsFullyLoaded)
            {
                return;
            }

            var startDragPosition = this.mousePositionSource.GetPosition();

            var startViewerOffset = this.viewerTransform.Offset;
            var startPanSpeed = this.inputStateSource.IsMatch(this.inputs.ViewerPanSpeed);
            var startPanSnap = startPanSpeed || this.inputStateSource.IsMatch(this.inputs.ViewerPanSnap);

            var mouseScope = this.inputPositionBinding.PushScope(true, position =>
            {
                var snapFactor = startPanSnap ? this.viewerTransform.Scale : 1.0;
                var speedFactor = startPanSpeed ? this.settings.PanSpeedFactor : 1.0;

                this.viewerTransform.SetOffset(new Point(
                    startViewerOffset.X + Math.Round(speedFactor * (position.X - startDragPosition.X) / snapFactor) * snapFactor,
                    startViewerOffset.Y - Math.Round(speedFactor * (position.Y - startDragPosition.Y) / snapFactor) * snapFactor));

                SetViewerTransform();

                var panSpeed = this.inputStateSource.IsMatch(this.inputs.ViewerPanSpeed);
                var panSnap = panSpeed || this.inputStateSource.IsMatch(this.inputs.ViewerPanSnap);
                if (startPanSpeed != panSpeed || startPanSnap != panSnap)
                {
                    startViewerOffset = this.viewerTransform.Offset;
                    startPanSpeed = panSpeed;
                    startPanSnap = panSnap;
                    startDragPosition = position;
                }
            });

            var inputScope = this.inputBindings.PushScope();

            this.skipKeyDownRepeat = this.lastProcessedKeyDown;
            this.inputBindings.AddSpanEnd(this.inputs.ViewerPan, () =>
            {
                mouseScope.Dispose();
                inputScope.Dispose();
                this.skipKeyDownRepeat = default;
            });

            e.Handled = true;
        }

        private void ViewerScaleStart(InputSpanEventArgs e)
        {
            if (!this.application.IsFullyLoaded)
            {
                return;
            }

            var startDragPosition = this.mousePositionSource.GetPosition();

            var startViewerScale = this.viewerTransform.Scale;
            var startScaleSpeed = this.inputStateSource.IsMatch(this.inputs.ViewerScaleSpeed);

            var relativePosition1 = GetDevicePosition(Mouse.PrimaryDevice.GetPosition(this.window));
            var viewerPosition = TransformViewerPosition(relativePosition1);

            var mouseScope = this.inputPositionBinding.PushScope(true, position =>
            {
                var modifierFactor = startScaleSpeed ? this.settings.ScaleSpeedFactor : 1.0;

                this.viewerTransform.SetScale(startViewerScale * Math.Exp(this.settings.ScaleDragFactor * modifierFactor * 0.01 * (startDragPosition.Y - position.Y)));

                var relativePosition2 = TransformBackViewerPosition(viewerPosition);

                this.viewerTransform.SetOffset(new Point(
                    this.viewerTransform.Offset.X + relativePosition1.X - relativePosition2.X,
                    this.viewerTransform.Offset.Y - relativePosition1.Y + relativePosition2.Y));

                SetViewerTransform();

                var scaleSpeed = this.inputStateSource.IsMatch(this.inputs.ViewerScaleSpeed);
                if (startScaleSpeed != scaleSpeed)
                {
                    startScaleSpeed = scaleSpeed;
                    startViewerScale = this.viewerTransform.Scale;
                    startDragPosition = position;
                }
            });

            var inputScope = this.inputBindings.PushScope();

            this.skipKeyDownRepeat = this.lastProcessedKeyDown;
            this.inputBindings.AddSpanEnd(this.inputs.ViewerScale, () =>
            {
                mouseScope.Dispose();
                inputScope.Dispose();
                this.skipKeyDownRepeat = default;
            });

            e.Handled = true;
        }

        private void ShaderMouseStateStart(InputSpanEventArgs e)
        {
            if (!this.application.IsFullyLoaded)
            {
                return;
            }

            this.viewerMousePositionSource.GetPosition(out var lastPositionX, out var lastPositionY);
            this.application.SetMouseState(true);

            var inputScope = this.inputBindings.PushScope();
            var mouseScope = this.inputPositionBinding.PushScope(this.application.WrapShaderInputCursor, position =>
            {
                this.viewerMousePositionSource.GetPosition(out var positionX, out var positionY);
                if (lastPositionX != positionX || lastPositionY != positionY)
                {
                    lastPositionX = positionX;
                    lastPositionY = positionY;
                    this.application.SetMouseState(true);
                }
            });

            this.skipKeyDownRepeat = this.lastProcessedKeyDown;
            this.inputBindings.AddSpanEnd(this.inputs.ShaderMouseState, () =>
            {
                this.application.SetMouseState(false);
                inputScope.Dispose();
                mouseScope.Dispose();
                this.skipKeyDownRepeat = default;
            });

            e.Handled = true;
        }

        private void SetViewerScale(InputSpanEventArgs e, bool scaleUp)
        {
            if (!this.application.IsFullyLoaded)
            {
                return;
            }

            var position1 = GetDevicePosition(Mouse.PrimaryDevice.GetPosition(this.window));
            var viewerPosition = TransformViewerPosition(position1);
            var modifierFactor = this.inputStateSource.IsMatch(this.inputs.ViewerScaleSpeed) ? this.settings.ScaleSpeedFactor : 1.0;

            this.viewerTransform.SetScale(this.viewerTransform.Scale * (scaleUp ? this.settings.ScaleFactor * modifierFactor : 1.0 / (this.settings.ScaleFactor * modifierFactor)));

            var position2 = TransformBackViewerPosition(viewerPosition);

            this.viewerTransform.SetOffset(new Point(
                this.viewerTransform.Offset.X + position1.X - position2.X,
                this.viewerTransform.Offset.Y - position1.Y + position2.Y));

            SetViewerTransform();

            e.Handled = true;
        }

        private void ResetViewerScale(InputSpanEventArgs e)
        {
            if (!this.application.IsFullyLoaded)
            {
                return;
            }

            ResetViewerTransform();

            e.Handled = true;
        }

        private void ToggleFullScreen(InputSpanEventArgs e)
        {
            if (!this.isFullScreen && !this.application.IsFullyLoaded)
            {
                return;
            }

            this.isFullScreen = !this.isFullScreen;

            if (this.isFullScreen)
            {
                this.lastFullScreenWindowState = this.window.WindowState;
                this.window.Topmost = false;
                this.window.ResizeMode = ResizeMode.NoResize;
                this.window.WindowState = WindowState.Minimized;
                this.window.WindowStyle = WindowStyle.None;
                this.window.WindowState = WindowState.Maximized;
            }
            else
            {
                this.window.ResizeMode = ResizeMode.CanResize;
                this.window.WindowStyle = WindowStyle.SingleBorderWindow;
                this.window.WindowState = this.lastFullScreenWindowState;
                this.window.Topmost = this.application.AlwaysOnTop && this.project != null;
            }

            e.Handled = true;
        }

        private void ExitFullScreen(InputSpanEventArgs e)
        {
            if (this.isFullScreen)
            {
                this.isFullScreen = false;
                this.window.ResizeMode = ResizeMode.CanResize;
                this.window.WindowStyle = WindowStyle.SingleBorderWindow;
                this.window.WindowState = this.lastFullScreenWindowState;
                e.Handled = true;
            }
        }
        private void CopyFrame()
        {
            if (this.window.IsMouseOver)
            {
                this.application.CopyFrame(this.application.GetCopySource(), false);
            }
        }

        private void CopyFrameWithAlpha()
        {
            if (this.window.IsMouseOver)
            {
                this.application.CopyFrame(this.application.GetCopySource(), true);
            }
        }

        private void CopyRepeat()
        {
            if (this.window.IsMouseOver)
            {
                if (this.application.CopySelection != null)
                {
                    this.application.CopyRepeat(this.application.GetCopySource());
                }
                else
                {
                    CopySelect();
                }
            }
        }

        private void CopySelect()
        {
            if (this.window.IsMouseOver && this.application.IsFullyLoaded)
            {
                ResetCopySource();
                this.copyMenu.Open();
            }
        }

        private void ResetCopySource()
        {
            this.viewerMousePositionSource.GetPosition(out var x, out var y);
            this.menuCopyState.ResetCopySource((int)x, (int)y);
        }

        private void SetViewerTransform()
        {
            this.application.SetViewerTransform(this.viewerTransform.Scale, this.viewerTransform.Offset.X, this.viewerTransform.Offset.Y);
            this.viewerMousePositionSource.SetViewerTransform(this.viewerTransform.Scale, this.viewerTransform.Offset.X, this.viewerTransform.Offset.Y);
        }

        private Point TransformViewerPosition(Point position)
        {
            var x = position.X;
            var y = position.Y;

            y = this.viewportHeight - y; // horizontal flip

            x -= this.viewerTransform.Offset.X;
            y -= this.viewerTransform.Offset.Y;

            var scale = this.viewerTransform.Scale;
            x /= scale;
            y /= scale;

            return new Point(x, y);
        }

        private Point TransformBackViewerPosition(Point position)
        {
            var x = position.X;
            var y = position.Y;

            var scale = this.viewerTransform.Scale;
            x *= scale;
            y *= scale;

            x += this.viewerTransform.Offset.X;
            y += this.viewerTransform.Offset.Y;

            y = this.viewportHeight - y; // horizontal flip

            return new Point(x, y);
        }

        private void SetStatistics()
        {
            if (this.statisticsTooltip.IsOpen)
            {
                this.statisticsTooltipContent.Text = !this.application.IsFullyLoaded ? ApplicationName :
                    this.tooltipFormatter.GetStatistics(this.frameRateTarget.FrameIndex, this.frameRateTarget.Rate, this.frameRateTarget.Average, this.bufferWidth, this.bufferHeight, this.application.Speed < 1.0);
            }

            this.window.Title = FormatWindowTitle();
        }

        private string FormatWindowTitle()
        {
            if (this.projectName == null)
            {
                return ApplicationName;
            }

            var handler = new DefaultInterpolatedStringHandler(this.titleBuffer.Length, 10, null, this.titleBuffer.AsSpan());

            handler.AppendLiteral(this.projectName);
            handler.AppendLiteral(this.isProjectChanged ? "*" : String.Empty);

            if (this.application.IsFullyLoaded)
            {
                handler.AppendLiteral(" - ");
                handler.AppendLiteral(this.titleFormatter.GetStatistics(this.frameRateTarget.FrameIndex, this.frameRateTarget.Rate, this.frameRateTarget.Average, this.bufferWidth, this.bufferHeight, this.application.Speed < 1.0));
            }

            handler.AppendLiteral(" - ");
            handler.AppendLiteral(ApplicationName);

            return handler.ToString();
        }

        private static bool IsCursorOverTitleBar(IntPtr handle)
        {
            var titleBarInfo = new TITLEBARINFO { cbSize = Marshal.SizeOf<TITLEBARINFO>() };
            return GetCursorPos(out var position) && GetTitleBarInfo(handle, ref titleBarInfo) && titleBarInfo.rcTitleBar.Contains(position);
        }

        private Point GetDevicePosition(Point relativePosition)
        {
            return this.presentationSource!.CompositionTarget.TransformToDevice.Transform(relativePosition);
        }

        private static string? FormatResizeMessage(InputDisplayNameFormatter formatter, IApplicationInputs inputs)
        {
            var names = new[]
            {
                formatter.GetDisplayName(inputs.ResizeSnapSmall),
                formatter.GetDisplayName(inputs.ResizeSnapMedium),
                formatter.GetDisplayName(inputs.ResizeKeepRatio),
            }.OfType<string>().ToArray();

            return names.Length > 0 ? $"Hold {String.Join(" / ", names)} to limit the window size" : null;
        }

        private static uint GetColorValue(Color color)
        {
            return color == Colors.Transparent ? DWMWA_COLOR_DEFAULT : (uint)(color.R | color.G << 8 | color.B << 16);
        }
    }
}
