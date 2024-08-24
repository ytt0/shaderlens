namespace Shaderlens.Views.Menus
{
    public class OptionsMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationCommands commands;
        private readonly IMenuResourcesFactory resources;

        public OptionsMenuSource(IApplication application, IApplicationCommands commands, IMenuResourcesFactory resources)
        {
            this.application = application;
            this.commands = commands;
            this.resources = resources;
        }

        public void AddTo(IMenuBuilder builder)
        {
            this.commands.AlwaysOnTop.AddMenuItem(builder, "Always On Top");
            this.commands.AutoReload.AddMenuItem(builder, "Auto Reload Project Files");
            this.commands.RestartOnAutoReload.AddMenuItem(builder, "Restart On Auto Reload");
            this.commands.ClearStateOnRestart.AddMenuItem(builder, "Clear State On Restart");
            this.commands.PauseOnInactivity.AddMenuItem(builder, "Pause On Inactivity");
            this.commands.RenderInputEventsWhenPaused.AddMenuItem(builder, "Render Input Events When Paused");
            this.commands.WrapShaderInputCursor.AddMenuItem(builder, "Wrap Shader Input Cursor");
            this.commands.EnableShaderCache.AddMenuItem(builder, "Enable Shader Cache");
            builder.AddSeparator();
            this.commands.DarkTheme.AddMenuItem(builder, "Dark Theme");
            builder.AddSeparator();
            this.commands.OpenSettingsFile.AddMenuItem(builder, "Open Settings File", this.resources.CreateFileSettingsIcon(), this.application.SettingsPath);
            this.commands.OpenInputsFile.AddMenuItem(builder, "Open Inputs File", this.resources.CreateFileSettingsIcon(), this.application.InputsPath);
            this.commands.OpenThemeFile.AddMenuItem(builder, "Open Theme File", this.resources.CreateFileSettingsIcon(), null, state => state.SetTooltip(this.application.ThemePath));
        }
    }
}