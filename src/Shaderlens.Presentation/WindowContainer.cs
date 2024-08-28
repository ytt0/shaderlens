namespace Shaderlens.Presentation
{
    public interface IWindowContainer
    {
        Transform Transform { get; }
        Transform InverseTransform { get; }

        void Show();
        void ShowDialog();
        void Close();
        void SetContent(FrameworkElement content);
        void SetTheme(IApplicationTheme theme);
        void SetTitle(string title);
    }

    public class WindowContainerState
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public double Scale { get; set; }
        public bool Maximized { get; set; }
    }

    public class WindowContainer : IWindowContainer
    {
        public Transform Transform { get { return this.scaleBehavior.Transform; } }
        public Transform InverseTransform { get { return this.scaleBehavior.InverseTransform; } }

        private readonly Window window;
        private readonly WindowContainerState state;
        private IApplicationTheme theme;
        private IntPtr handle;
        private Color windowTitleBackground;
        private Size normalWindowSize;
        private Point normalWindowPosition;
        private readonly IScaleBehavior scaleBehavior;
        private readonly Decorator windowContent;

        public WindowContainer(Window window, IApplicationTheme theme, WindowContainerState state, string title)
        {
            this.window = window;
            this.theme = theme;
            this.state = state;
            this.theme.SetResources(this.window.Resources);
            this.window.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            this.window.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            this.window.SetReference(TextElement.FontSizeProperty, this.theme.WindowFontSize);
            this.window.SetReference(TextElement.FontFamilyProperty, this.theme.WindowFontFamily);

            this.window.Title = title;
            this.window.SnapsToDevicePixels = true;
            this.window.Left = state.Position.X;
            this.window.Top = state.Position.Y;
            this.window.Width = state.Size.Width;
            this.window.Height = state.Size.Height;
            this.window.WindowState = state.Maximized ? WindowState.Maximized : WindowState.Normal;
            this.windowTitleBackground = theme.WindowTitleBackground.Value;

            this.window.SourceInitialized += OnSourceInitialized;
            this.window.Closed += OnClosed;
            this.window.StateChanged += OnStateChanged;

            this.scaleBehavior = ScaleBehavior.Register(this.window);
            this.scaleBehavior.Scale = state.Scale;

            this.windowContent = new Decorator { LayoutTransform = this.scaleBehavior.Transform };
            this.window.Content = this.windowContent;

            MouseHoverKeyEventBehavior.Register(this.window);
        }

        public void SetContent(FrameworkElement content)
        {
            this.windowContent.Child = content;
        }

        public void SetTheme(IApplicationTheme theme)
        {
            this.theme = theme;
            this.theme.SetResources(this.window.Resources);
            this.windowTitleBackground = theme.WindowTitleBackground.Value;

            WinApi.SetWindowCaptionColor(this.handle, GetColorValue(this.windowTitleBackground));
        }

        public void SetTitle(string title)
        {
            this.window.Title = title;
        }

        public void Show()
        {
            if (this.window.WindowState == WindowState.Minimized)
            {
                this.window.WindowState = WindowState.Normal;
            }

            this.window.Show();
            this.window.Activate();
        }

        public void ShowDialog()
        {
            if (this.window.WindowState == WindowState.Minimized)
            {
                this.window.WindowState = WindowState.Normal;
            }

            this.window.ShowDialog();
        }

        public void Close()
        {
            this.window.Close();
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            this.handle = new WindowInteropHelper(this.window).Handle;
            WinApi.SetWindowCaptionColor(this.handle, GetColorValue(this.windowTitleBackground));
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            this.state.Position = this.window.WindowState == WindowState.Maximized ? this.normalWindowPosition : new Point(this.window.Left, this.window.Top);
            this.state.Size = this.window.WindowState == WindowState.Maximized ? this.normalWindowSize : new Size(this.window.Width, this.window.Height);
            this.state.Scale = this.scaleBehavior.Scale;
            this.state.Maximized = this.window.WindowState == WindowState.Maximized;
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            if (this.window.WindowState == WindowState.Maximized)
            {
                this.normalWindowPosition = new Point(this.window.Left, this.window.Top);
                this.normalWindowSize = new Size(this.window.Width, this.window.Height);
            }
        }

        private static uint GetColorValue(Color color)
        {
            return color == Colors.Transparent ? WinApi.DWMWA_COLOR_DEFAULT : (uint)(color.R | color.G << 8 | color.B << 16);
        }
    }
}
