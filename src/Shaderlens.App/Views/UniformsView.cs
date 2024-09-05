namespace Shaderlens.Views
{
    public interface IUniformsView
    {
        bool IsVisible { get; }

        void Show();
        void Hide();
        void SetProjectName(string name);
        void SetProjectChangeState(bool isChanged);
        void SetTheme(IApplicationTheme theme);
        void SetContent(IUniform projectUniforms, IProjectSettings projectSettings);
        void ClearContent();
    }

    public class UniformsView : IUniformsView
    {
        public bool IsVisible { get { return this.window.IsVisible; } }

        private readonly Window window;
        private readonly IApplication application;
        private readonly IApplicationSettings settings;
        private readonly IApplicationCommands commands;
        private readonly IApplicationTheme theme;
        private readonly WindowContainer windowContainer;
        private readonly InputStateBindings inputStateBindings;
        private readonly InputStateSourceBehavior inputStateSource;
        private readonly Decorator windowContent;
        private string? projectName;
        private bool isProjectChanged;

        public UniformsView(Window window, IApplication application, IApplicationSettings settings, IApplicationCommands commands, IApplicationTheme theme)
        {
            this.window = window;
            this.application = application;
            this.settings = settings;
            this.commands = commands;
            this.theme = theme;
            this.windowContainer = new WindowContainer(window, theme, settings.UniformsWindowState, "Uniforms");

            this.window.Closed += OnClosed;
            this.window.Activated += OnActivated;

            this.windowContent = new Border();
            this.windowContent.SetReference(TextElement.ForegroundProperty, theme.WindowForeground);
            this.windowContent.SetReference(Control.BackgroundProperty, theme.WindowBackground);

            this.windowContainer.SetContent(this.windowContent);

            this.inputStateBindings = new InputStateBindings();
            this.inputStateSource = InputStateSourceBehavior.Register(this.window, this.inputStateBindings);
            this.commands.AddBindings(this.inputStateBindings);
        }

        public void SetTheme(IApplicationTheme theme)
        {
            this.windowContainer.SetTheme(theme);
        }

        public void SetProjectName(string name)
        {
            this.window.Dispatcher.InvokeAsync(() =>
            {
                this.projectName = name;
                this.isProjectChanged = false;
                SetTitle();
            });
        }

        public void SetProjectChangeState(bool isChanged)
        {
            this.window.Dispatcher.InvokeAsync(() =>
            {
                if (this.isProjectChanged != isChanged)
                {
                    this.isProjectChanged = isChanged;
                    SetTitle();
                }
            });
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

        public void Hide()
        {
            this.window.Hide();
        }

        public void SetContent(IUniform projectUniforms, IProjectSettings projectSettings)
        {
            var dragSensitivity = this.application.GetDragSensitivity(this.window);
            var builder = new UniformsViewBuilder(this.application, projectSettings, this.theme, this.windowContainer.InverseTransform, this.application.Clipboard, dragSensitivity);
            projectUniforms.AddViewElement(builder);
            builder.SetSettingsState();

            this.windowContent.Child = builder.ViewElement;
        }

        public void ClearContent()
        {
            this.windowContent.Child = null;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            this.settings.UniformsOpened = this.window.IsVisible;
        }

        private void OnActivated(object? sender, EventArgs e)
        {
            this.inputStateSource.Refresh();
        }

        private void SetTitle()
        {
            this.window.Title = this.projectName == null ? "Uniforms" : $"{this.projectName}{(this.isProjectChanged ? "*" : "")} - Uniforms";
        }
    }
}