namespace Shaderlens
{
    public interface IApplicationSettings
    {
        string Path { get; }

        string VertexHeader { get; }
        string FragmentHeader { get; }
        string ComputeHeader { get; }

        WindowContainerState StartPageWindowState { get; }
        WindowContainerState ViewportWindowState { get; set; }
        WindowContainerState UniformsWindowState { get; set; }
        WindowContainerState CreateProjectWindowState { get; set; }
        WindowContainerState RenderSequenceWindowState { get; set; }

        bool UniformsOpened { get; set; }
        string OpenProjectPath { get; set; }
        string CreateProjectPath { get; set; }
        int CreateProjectTemplateIndex { get; set; }
        bool CreateProjectOpenFolder { get; set; }

        bool AlwaysOnTop { get; set; }
        bool AutoReload { get; set; }
        bool RestartOnAutoReload { get; set; }
        bool ClearStateOnRestart { get; set; }
        bool PauseOnInactivity { get; set; }
        bool RenderInputEventsWhenPaused { get; set; }
        bool WrapShaderInputCursor { get; set; }
        bool EnableCache { get; set; }
        bool DarkTheme { get; set; }
        int InactivityPauseSeconds { get; }
        double StepIncrementSeconds { get; }

        IEnumerable<string> PinnedProjects { get; set; }
        IEnumerable<string> RecentProjects { get; set; }

        int ProjectOpenFileDialogFilterIndex { get; set; }
        int FrameSaveFileDialogFilterIndex { get; set; }

        IEnumerable<ICopyFormatter> CopyFormatters { get; }
        ViewerPassSelection DefaultViewerPass { get; }

        double ScaleFactor { get; }
        double ScaleDragFactor { get; }
        double ScaleSpeedFactor { get; }
        double PanSpeedFactor { get; }

        double OverlayGridVisibleScale { get; }
        double OverlayValueVisibleScale { get; }
        double OverlayFontScale { get; }

        int LogErrorContextLines { get; }
        double LogVisibilityDelaySeconds { get; }
        int MemoryCachedResources { get; }

        string ProjectTemplatesPath { get; }
        double TextBoxDragSensitivity { get; }
        double CursorVisibilityTimeoutSeconds { get; }

        bool ConfirmSaveOnClose { get; }
        bool ShowStartPage { get; }

        void Save();
    }

    public class ApplicationSettings : IApplicationSettings
    {
        private static readonly IEnumerable<ICopyFormatter> DefaultCopyFormatters = new[]
        {
            new CopyFormatter("Value", "{{x}:0.0##}, {{y}:0.0##}, {{z}:0.0##}, {{w}:0.0##}"),
            new CopyFormatter("sRGBA", "{{r}:0.0#}, {{g}:0.0#}, {{b}:0.0#}, {{a}:0.0#}"),
            new CopyFormatter("sRGBA", "{{R}}, {{G}}, {{B}}, {{A}}"),
            new CopyFormatter("sRGBA", "{{R}:x2}{{G}:x2}{{B}:x2}{{A}:x2}"),
            new CopyFormatter("Linear RGBA", "{{lr}:0.0#}, {{lg}:0.0#}, {{lb}:0.0#}, {{la}:0.0#}"),
            new CopyFormatter("Position", "{{px}:0.0##}, {{py}:0.0##}"),
            new CopyFormatter("Position", "{{pX}}, {{pY}}"),
        };

        public string Path { get { return this.settingsFile.Path; } }

        public string VertexHeader { get; set; } = "#version 460";
        public string FragmentHeader { get; set; } = "#version 460";
        public string ComputeHeader { get; set; } = "#version 460";

        public WindowContainerState StartPageWindowState { get; set; } = GetCenteredWindowContainerState(new Size(800, 500));
        public WindowContainerState ViewportWindowState { get; set; } = GetAbsoluteState(GetCenteredWindowContainerState(new Size(800, 500)));
        public WindowContainerState UniformsWindowState { get; set; } = GetCenteredWindowContainerState(new Size(400, 550), new Point(300, 0));
        public WindowContainerState CreateProjectWindowState { get; set; } = GetCenteredWindowContainerState(new Size(800, 500), new Point(40, 40));
        public WindowContainerState RenderSequenceWindowState { get; set; } = GetCenteredWindowContainerState(new Size(800, 500), new Point(40, 40));

        public bool UniformsOpened { get; set; } = false;
        public string OpenProjectPath { get; set; } = String.Empty;
        public string SaveFramePath { get; set; } = String.Empty;
        public string CreateProjectPath { get; set; } = String.Empty;
        public int CreateProjectTemplateIndex { get; set; } = 0;
        public bool CreateProjectOpenFolder { get; set; } = true;

        public bool AlwaysOnTop { get; set; } = false;
        public bool AutoReload { get; set; } = true;
        public bool RestartOnAutoReload { get; set; } = false;
        public bool ClearStateOnRestart { get; set; } = true;
        public bool PauseOnInactivity { get; set; } = true;
        public bool RenderInputEventsWhenPaused { get; set; } = true;
        public bool WrapShaderInputCursor { get; set; } = true;
        public bool EnableCache { get; set; } = false;
        public bool DarkTheme { get; set; } = true;
        public int InactivityPauseSeconds { get; } = 60;
        public double StepIncrementSeconds { get; } = 0.016;

        public IEnumerable<string> PinnedProjects { get; set; } = Array.Empty<string>();
        public IEnumerable<string> RecentProjects { get; set; } = Array.Empty<string>();

        public int ProjectOpenFileDialogFilterIndex { get; set; } = 2;
        public int FrameSaveFileDialogFilterIndex { get; set; } = 2;

        public IEnumerable<ICopyFormatter> CopyFormatters { get; } = DefaultCopyFormatters;
        public ViewerPassSelection DefaultViewerPass { get; } = ViewerPassSelection.ValuesOverlay;

        public double ScaleFactor { get; } = 1.1;
        public double ScaleDragFactor { get; } = 1.0;
        public double ScaleSpeedFactor { get; } = 2.0;
        public double PanSpeedFactor { get; } = 20.0;

        public double OverlayGridVisibleScale { get; } = 6.0;
        public double OverlayValueVisibleScale { get; } = 8.0;
        public double OverlayFontScale { get; } = 2.0;

        public int LogErrorContextLines { get; } = 5;
        public double LogVisibilityDelaySeconds { get; } = 0.5;
        public int MemoryCachedResources { get; } = 100;

        public string ProjectTemplatesPath { get; } = "Resources\\Templates";
        public double TextBoxDragSensitivity { get; } = 1.0;
        public double CursorVisibilityTimeoutSeconds { get; } = 2.0;

        public bool ConfirmSaveOnClose { get; } = true;
        public bool ShowStartPage { get; } = true;

        private readonly IJsonSettingsFile settingsFile;
        private readonly PointJsonSerializer pointSerializer;
        private readonly Presentation.Serialization.SizeJsonSerializer sizeSerializer;
        private readonly ValueJsonSerializer<bool> boolSerializer;
        private readonly ValueJsonSerializer<int> intSerializer;
        private readonly DoubleJsonSerializer doubleSerializer;
        private readonly ValueJsonSerializer<string> stringSerializer;
        private readonly CopyFormatterSerializer formattersSerializer;
        private readonly ViewerPassSelectionSerializer viewerPassSelectionSerializer;

        public ApplicationSettings(IJsonSettingsFile settingsFile)
        {
            this.settingsFile = settingsFile;

            this.pointSerializer = new PointJsonSerializer();
            this.sizeSerializer = new Presentation.Serialization.SizeJsonSerializer();
            this.boolSerializer = new ValueJsonSerializer<bool>();
            this.intSerializer = new ValueJsonSerializer<int>();
            this.doubleSerializer = new DoubleJsonSerializer(2);
            this.stringSerializer = new ValueJsonSerializer<string>();
            this.formattersSerializer = new CopyFormatterSerializer();
            this.viewerPassSelectionSerializer = new ViewerPassSelectionSerializer();

            var settings = settingsFile.Content;

            this.VertexHeader = settings.GetOrSetDefault(this.stringSerializer, nameof(this.VertexHeader), this.VertexHeader);
            this.FragmentHeader = settings.GetOrSetDefault(this.stringSerializer, nameof(this.FragmentHeader), this.FragmentHeader);
            this.ComputeHeader = settings.GetOrSetDefault(this.stringSerializer, nameof(this.ComputeHeader), this.ComputeHeader);

            this.StartPageWindowState = GetOrSetDefaultWindowState(settings, this.StartPageWindowState, "StartPage");
            this.ViewportWindowState = GetOrSetDefaultWindowState(settings, this.ViewportWindowState, "Viewport");
            this.UniformsWindowState = GetOrSetDefaultWindowState(settings, this.UniformsWindowState, "Uniforms");
            this.CreateProjectWindowState = GetOrSetDefaultWindowState(settings, this.CreateProjectWindowState, "CreateProject");
            this.RenderSequenceWindowState = GetOrSetDefaultWindowState(settings, this.RenderSequenceWindowState, "RenderSequence");

            this.UniformsOpened = settings.GetOrSetDefault(this.boolSerializer, nameof(this.UniformsOpened), this.UniformsOpened);
            this.OpenProjectPath = settings.GetOrSetDefault(this.stringSerializer, nameof(this.OpenProjectPath), this.OpenProjectPath);
            this.CreateProjectPath = settings.GetOrSetDefault(this.stringSerializer, nameof(this.CreateProjectPath), this.CreateProjectPath);
            this.CreateProjectTemplateIndex = settings.GetOrSetDefault(this.intSerializer, nameof(this.CreateProjectTemplateIndex), this.CreateProjectTemplateIndex);
            this.CreateProjectOpenFolder = settings.GetOrSetDefault(this.boolSerializer, nameof(this.CreateProjectOpenFolder), this.CreateProjectOpenFolder);

            this.AlwaysOnTop = settings.GetOrSetDefault(this.boolSerializer, nameof(this.AlwaysOnTop), this.AlwaysOnTop);
            this.AutoReload = settings.GetOrSetDefault(this.boolSerializer, nameof(this.AutoReload), this.AutoReload);
            this.RestartOnAutoReload = settings.GetOrSetDefault(this.boolSerializer, nameof(this.RestartOnAutoReload), this.RestartOnAutoReload);
            this.ClearStateOnRestart = settings.GetOrSetDefault(this.boolSerializer, nameof(this.ClearStateOnRestart), this.ClearStateOnRestart);
            this.PauseOnInactivity = settings.GetOrSetDefault(this.boolSerializer, nameof(this.PauseOnInactivity), this.PauseOnInactivity);
            this.RenderInputEventsWhenPaused = settings.GetOrSetDefault(this.boolSerializer, nameof(this.RenderInputEventsWhenPaused), this.RenderInputEventsWhenPaused);
            this.WrapShaderInputCursor = settings.GetOrSetDefault(this.boolSerializer, nameof(this.WrapShaderInputCursor), this.WrapShaderInputCursor);
            this.EnableCache = settings.GetOrSetDefault(this.boolSerializer, nameof(this.EnableCache), this.EnableCache);
            this.DarkTheme = settings.GetOrSetDefault(this.boolSerializer, nameof(this.DarkTheme), this.DarkTheme);
            this.InactivityPauseSeconds = settings.GetOrSetDefault(this.intSerializer, nameof(this.InactivityPauseSeconds), this.InactivityPauseSeconds);
            this.StepIncrementSeconds = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.StepIncrementSeconds), this.StepIncrementSeconds);

            this.PinnedProjects = settings.GetOrSetDefault(this.stringSerializer, nameof(this.PinnedProjects), this.PinnedProjects);
            this.RecentProjects = settings.GetOrSetDefault(this.stringSerializer, nameof(this.RecentProjects), this.RecentProjects);

            this.ProjectOpenFileDialogFilterIndex = settings.GetOrSetDefault(this.intSerializer, nameof(this.ProjectOpenFileDialogFilterIndex), this.ProjectOpenFileDialogFilterIndex);
            this.FrameSaveFileDialogFilterIndex = settings.GetOrSetDefault(this.intSerializer, nameof(this.FrameSaveFileDialogFilterIndex), this.FrameSaveFileDialogFilterIndex);

            this.CopyFormatters = settings.GetOrSetDefault(this.formattersSerializer, nameof(this.CopyFormatters), DefaultCopyFormatters.ToArray());
            this.DefaultViewerPass = settings.GetOrSetDefault(this.viewerPassSelectionSerializer, nameof(this.DefaultViewerPass), this.DefaultViewerPass) ?? ViewerPassSelection.ValuesOverlay;

            this.ScaleFactor = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.ScaleFactor), this.ScaleFactor);
            this.ScaleDragFactor = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.ScaleDragFactor), this.ScaleDragFactor);
            this.ScaleSpeedFactor = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.ScaleSpeedFactor), this.ScaleSpeedFactor);
            this.PanSpeedFactor = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.PanSpeedFactor), this.PanSpeedFactor);

            this.OverlayGridVisibleScale = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.OverlayGridVisibleScale), this.OverlayGridVisibleScale);
            this.OverlayValueVisibleScale = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.OverlayValueVisibleScale), this.OverlayValueVisibleScale);
            this.OverlayFontScale = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.OverlayFontScale), this.OverlayFontScale);

            this.LogErrorContextLines = settings.GetOrSetDefault(this.intSerializer, nameof(this.LogErrorContextLines), this.LogErrorContextLines);
            this.LogVisibilityDelaySeconds = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.LogVisibilityDelaySeconds), this.LogVisibilityDelaySeconds);
            this.MemoryCachedResources = settings.GetOrSetDefault(this.intSerializer, nameof(this.MemoryCachedResources), this.MemoryCachedResources);

            this.ProjectTemplatesPath = settings.GetOrSetDefault(this.stringSerializer, nameof(this.ProjectTemplatesPath), this.ProjectTemplatesPath);
            this.TextBoxDragSensitivity = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.TextBoxDragSensitivity), this.TextBoxDragSensitivity);
            this.CursorVisibilityTimeoutSeconds = settings.GetOrSetDefault(this.doubleSerializer, nameof(this.CursorVisibilityTimeoutSeconds), this.CursorVisibilityTimeoutSeconds);

            this.ConfirmSaveOnClose = settings.GetOrSetDefault(this.boolSerializer, nameof(this.ConfirmSaveOnClose), this.ConfirmSaveOnClose);
            this.ShowStartPage = settings.GetOrSetDefault(this.boolSerializer, nameof(this.ShowStartPage), this.ShowStartPage);
        }

        public void Save()
        {
            var settings = this.settingsFile.Content;

            SetWindowState(settings, this.StartPageWindowState, "StartPage");
            SetWindowState(settings, this.ViewportWindowState, "Viewport");
            SetWindowState(settings, this.UniformsWindowState, "Uniforms");
            SetWindowState(settings, this.CreateProjectWindowState, "CreateProject");
            SetWindowState(settings, this.RenderSequenceWindowState, "RenderSequence");

            settings.Set(this.stringSerializer, nameof(this.OpenProjectPath), this.OpenProjectPath);
            settings.Set(this.stringSerializer, nameof(this.SaveFramePath), this.SaveFramePath);
            settings.Set(this.boolSerializer, nameof(this.UniformsOpened), this.UniformsOpened);
            settings.Set(this.stringSerializer, nameof(this.CreateProjectPath), this.CreateProjectPath);
            settings.Set(this.intSerializer, nameof(this.CreateProjectTemplateIndex), this.CreateProjectTemplateIndex);
            settings.Set(this.boolSerializer, nameof(this.CreateProjectOpenFolder), this.CreateProjectOpenFolder);

            settings.Set(this.boolSerializer, nameof(this.AlwaysOnTop), this.AlwaysOnTop);
            settings.Set(this.boolSerializer, nameof(this.AutoReload), this.AutoReload);
            settings.Set(this.boolSerializer, nameof(this.RestartOnAutoReload), this.RestartOnAutoReload);
            settings.Set(this.boolSerializer, nameof(this.ClearStateOnRestart), this.ClearStateOnRestart);
            settings.Set(this.boolSerializer, nameof(this.PauseOnInactivity), this.PauseOnInactivity);
            settings.Set(this.boolSerializer, nameof(this.RenderInputEventsWhenPaused), this.RenderInputEventsWhenPaused);
            settings.Set(this.boolSerializer, nameof(this.WrapShaderInputCursor), this.WrapShaderInputCursor);
            settings.Set(this.boolSerializer, nameof(this.DarkTheme), this.DarkTheme);
            settings.Set(this.boolSerializer, nameof(this.EnableCache), this.EnableCache);

            settings.Set(this.stringSerializer, nameof(this.PinnedProjects), this.PinnedProjects);
            settings.Set(this.stringSerializer, nameof(this.RecentProjects), this.RecentProjects);

            settings.Set(this.intSerializer, nameof(this.ProjectOpenFileDialogFilterIndex), this.ProjectOpenFileDialogFilterIndex);
            settings.Set(this.intSerializer, nameof(this.FrameSaveFileDialogFilterIndex), this.FrameSaveFileDialogFilterIndex);

            this.settingsFile.Content.ClearUnusedValues();
            this.settingsFile.Save();
        }

        private WindowContainerState GetOrSetDefaultWindowState(IJsonSettings settings, WindowContainerState defaultState, string prefix)
        {
            return new WindowContainerState
            {
                Position = settings.GetOrSetDefault(this.pointSerializer, prefix + nameof(defaultState.Position), defaultState.Position),
                Size = settings.GetOrSetDefault(this.sizeSerializer, prefix + nameof(defaultState.Size), defaultState.Size),
                Scale = settings.GetOrSetDefault(this.doubleSerializer, prefix + nameof(defaultState.Scale), defaultState.Scale),
                Maximized = settings.GetOrSetDefault(this.boolSerializer, prefix + nameof(defaultState.Maximized), defaultState.Maximized)
            };
        }

        private void SetWindowState(IJsonSettings settings, WindowContainerState state, string prefix)
        {
            settings.Set(this.pointSerializer, prefix + nameof(state.Position), state.Position);
            settings.Set(this.sizeSerializer, prefix + nameof(state.Size), state.Size);
            settings.Set(this.doubleSerializer, prefix + nameof(state.Scale), state.Scale);
            settings.Set(this.boolSerializer, prefix + nameof(state.Maximized), state.Maximized);
        }

        private static WindowContainerState GetCenteredWindowContainerState(Size size, Point offset = default)
        {
            return new WindowContainerState
            {
                Position = new Point((SystemParameters.PrimaryScreenWidth - size.Width) / 2 + offset.X, (SystemParameters.PrimaryScreenHeight - size.Height) / 2 + offset.Y),
                Size = size,
                Scale = 1.0
            };
        }

        private static WindowContainerState GetAbsoluteState(WindowContainerState state)
        {
            var dpiProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            var dpi = dpiProperty?.GetValue(null) is int value ? value / 96.0 : 0.0;

            if (dpi > 0.1)
            {
                state.Position = new Point(state.Position.X * dpi, state.Position.Y * dpi);
                state.Size = new Size(state.Size.Width * dpi, state.Size.Height * dpi);
            }

            return state;
        }
    }
}
