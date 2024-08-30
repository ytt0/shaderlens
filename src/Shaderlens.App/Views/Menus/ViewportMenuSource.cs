namespace Shaderlens.Views.Menus
{
    public class ViewportMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationInputs inputs;
        private readonly IMenuResourcesFactory resources;

        private readonly RecentProjectsMenuSource recentProjectsMenuSource;
        private readonly ProjectFilesMenuSource projectFilesMenuSource;
        private readonly ExportMenuSource exportMenuSource;
        private readonly CopyMenuSource copyMenuSource;
        private readonly BuffersMenuSource buffersMenuSource;
        private readonly ResolutionMenuSource resolutionMenuSource;
        private readonly FrameRateMenuSource frameRateMenuSource;
        private readonly SpeedMenuSource speedMenuSource;
        private readonly ViewerMenuSource viewerMenuSource;
        private readonly OptionsMenuSource optionsMenuSource;

        public ViewportMenuSource(IApplication application, IApplicationInputs inputs, IApplicationCommands commands, IMenuResourcesFactory resources, ICopyMenuState selectionSource, IProjectSource? project, IMenuTheme theme)
        {
            this.application = application;
            this.inputs = inputs;
            this.resources = resources;

            this.recentProjectsMenuSource = new RecentProjectsMenuSource(application, inputs, resources);
            this.projectFilesMenuSource = new ProjectFilesMenuSource(application, inputs, resources, project);
            this.exportMenuSource = new ExportMenuSource(application, commands, theme);
            this.copyMenuSource = new CopyMenuSource(application, inputs, selectionSource, theme);
            this.buffersMenuSource = new BuffersMenuSource(application, inputs, project);
            this.resolutionMenuSource = new ResolutionMenuSource(application, commands);
            this.frameRateMenuSource = new FrameRateMenuSource(application, commands);
            this.speedMenuSource = new SpeedMenuSource(application, commands);
            this.viewerMenuSource = new ViewerMenuSource(application, commands, project);
            this.optionsMenuSource = new OptionsMenuSource(application, commands, resources);
        }

        public void AddTo(IMenuBuilder builder)
        {
            builder.AddItem("Play", this.inputs.Play, this.resources.CreatePlayIcon(), null, this.application.ResumePipeline, state => state.IsVisible = this.application.IsFullyLoaded && this.application.IsPaused);
            builder.AddItem("Pause", this.inputs.Pause, this.resources.CreatePauseIcon(), null, this.application.PausePipeline, state => state.IsVisible = this.application.IsFullyLoaded && !this.application.IsPaused);
            builder.AddItem("Step", this.inputs.Step, this.resources.CreateStepIcon(), null, this.application.StepPipeline, state => state.IsVisible = this.application.IsPaused);
            builder.AddItem("Restart", this.inputs.Restart, this.resources.CreateRestartIcon(), null, this.application.RestartPipeline, state => state.IsVisible = this.application.IsFullyLoaded);
            builder.AddSeparator();
            builder.AddItem("Uniforms", this.inputs.Uniforms, this.resources.CreateUniformsIcon(), null, this.application.ToggleUniformsView, state => state.IsVisible = this.application.IsFullyLoaded);
            builder.AddSeparator();
            builder.AddSubmenu(this.buffersMenuSource, "Buffer", this.inputs.MenuBuffers, this.resources.CreateBuffersIcon(), null, state => state.SetIsChanged(this.application.ViewerBuffersCount > 0 && this.application.ViewerBufferIndex != this.application.ViewerBuffersCount - 1).SetIsVisible(this.application.IsFullyLoaded), () => this.application.ViewerBufferIndex = this.application.ViewerBuffersCount - 1);
            builder.AddSubmenu(this.resolutionMenuSource, "Resolution", this.inputs.MenuResolution, this.resources.CreateResolutionIcon(), null, state => state.SetIsChanged(this.application.RenderDownscale != 1).SetIsVisible(this.application.IsFullyLoaded), () => this.application.RenderDownscale = 1);
            builder.AddSubmenu(this.frameRateMenuSource, "Frame Rate", this.inputs.MenuFrameRate, this.resources.CreateFrameRateIcon(), null, state => state.SetIsChanged(state.IsChanged = this.application.FrameRate != 1).SetIsVisible(this.application.IsFullyLoaded), () => this.application.FrameRate = 1);
            builder.AddSubmenu(this.speedMenuSource, "Speed", this.inputs.MenuSpeed, this.resources.CreateSpeedIcon(), null, state => state.SetIsChanged(this.application.Speed != 1).SetIsVisible(this.application.IsFullyLoaded), () => this.application.Speed = 1, true);
            builder.AddSubmenu(this.viewerMenuSource, "Viewer", this.inputs.MenuViewer, this.resources.CreateViewerIcon(), null, state => state.IsVisible = this.application.IsFullyLoaded);
            builder.AddSeparator();
            builder.AddSubmenu(this.copyMenuSource, "Copy", this.inputs.MenuCopy, this.resources.CreateCopyIcon(), null, state => state.IsVisible = this.application.IsFullyLoaded);
            builder.AddSeparator();
            builder.AddSubmenu(this.projectFilesMenuSource, "Project Files", this.inputs.MenuProjectFiles, this.resources.CreateFilesIcon(), null, state => state.IsVisible = this.application.IsPartiallyLoaded);
            builder.AddSeparator();
            builder.AddItem("Start Page", this.inputs.StartPage, this.resources.CreateStartPageIcon(), null, this.application.ShowStartPage);
            builder.AddSeparator();
            builder.AddItem("New...", this.inputs.ProjectNew, this.resources.CreateNewIcon(), null, this.application.CreateProject);
            builder.AddItem("Open...", this.inputs.ProjectOpen, this.resources.CreateOpenIcon(), null, this.application.OpenProject);
            builder.AddSubmenu(this.recentProjectsMenuSource, "Open Recent", this.inputs.MenuRecentProjects, this.resources.CreateRecentIcon(), null, null);
            builder.AddItem("Reload", this.inputs.ProjectReload, this.resources.CreateReloadIcon(), null, this.application.ReloadProject, state => state.IsVisible = this.application.IsPartiallyLoaded);
            builder.AddItem("Save", this.inputs.ProjectSave, this.resources.CreateSaveIcon(), null, this.application.SaveProject, state => state.IsVisible = this.application.IsFullyLoaded);
            builder.AddSeparator();
            builder.AddSubmenu(this.exportMenuSource, "Export", this.inputs.MenuExport, this.resources.CreateExportIcon(), null, state => state.IsVisible = this.application.IsFullyLoaded);
            builder.AddSeparator();
            builder.AddSubmenu(this.optionsMenuSource, "Options", this.inputs.MenuOptions, this.resources.CreateOptionsIcon(), null, null);
            builder.AddSeparator();
            builder.AddItem("Help", this.inputs.Help, this.resources.CreateHelpIcon(), null, this.application.Help);
        }
    }
}
