namespace Shaderlens
{
    public interface IApplicationInputs
    {
        IPositionGraphInputs PositionGraph { get; }

        string Path { get; }

        IInputSpan Play { get; }
        IInputSpan Pause { get; }
        IInputSpan Step { get; }
        IInputSpan Restart { get; }
        IInputSpan Uniforms { get; }
        IInputSpan StartPage { get; }
        IInputSpan ProjectNew { get; }
        IInputSpan ProjectOpen { get; }
        IInputSpan ProjectReload { get; }
        IInputSpan ProjectSave { get; }
        IInputSpan Help { get; }

        IInputSpan FullScreenToggle { get; }
        IInputSpan FullScreenLeave { get; }

        IInputSpan ShaderMouseState { get; }

        IInputSpan MenuMain { get; }
        IInputSpan MenuRecentProjects { get; }
        IInputSpan MenuProjectFiles { get; }
        IInputSpan MenuBuffers { get; }
        IInputSpan MenuExport { get; }
        IInputSpan MenuCopy { get; }
        IInputSpan MenuResolution { get; }
        IInputSpan MenuFrameRate { get; }
        IInputSpan MenuSpeed { get; }
        IInputSpan MenuViewer { get; }
        IInputSpan MenuOptions { get; }

        IInputSpan ResizeSnapSmall { get; }
        IInputSpan ResizeSnapMedium { get; }
        IInputSpan ResizeKeepRatio { get; }

        IInputSpan ViewerPan { get; }
        IInputSpan ViewerPanSpeed { get; }
        IInputSpan ViewerPanSnap { get; }
        IInputSpan ViewerScale { get; }
        IInputSpan ViewerScaleUp { get; }
        IInputSpan ViewerScaleDown { get; }
        IInputSpan ViewerScaleReset { get; }
        IInputSpan ViewerScaleSpeed { get; }

        IInputSpan CopyRepeat { get; }
        IInputSpan CopyFrame { get; }
        IInputSpan CopyFrameWithAlpha { get; }

        IInputSpan ProjectOpenFolder { get; }

        IInputSpan FrameRateFull { get; }
        IInputSpan FrameRate2 { get; }
        IInputSpan FrameRate4 { get; }
        IInputSpan FrameRate8 { get; }
        IInputSpan FrameRate16 { get; }
        IInputSpan FrameRateIncrease { get; }
        IInputSpan FrameRateDecrease { get; }

        IInputSpan ResolutionFull { get; }
        IInputSpan Resolution2 { get; }
        IInputSpan Resolution4 { get; }
        IInputSpan Resolution8 { get; }
        IInputSpan Resolution16 { get; }
        IInputSpan Resolution32 { get; }
        IInputSpan Resolution64 { get; }
        IInputSpan ResolutionIncrease { get; }
        IInputSpan ResolutionDecrease { get; }

        IInputSpan Speed1_16 { get; }
        IInputSpan Speed1_8 { get; }
        IInputSpan Speed1_4 { get; }
        IInputSpan Speed1_2 { get; }
        IInputSpan SpeedNormal { get; }
        IInputSpan Speed2 { get; }
        IInputSpan Speed4 { get; }
        IInputSpan Speed8 { get; }
        IInputSpan Speed16 { get; }
        IInputSpan SpeedIncrease { get; }
        IInputSpan SpeedDecrease { get; }

        IEnumerable<IInputSpan?> Buffer { get; }
        IInputSpan BufferImage { get; }
        IInputSpan BufferNext { get; }
        IInputSpan BufferPrevious { get; }

        IInputSpan ExportFrame { get; }
        IInputSpan ExportFrameRepeat { get; }
        IInputSpan ExportRenderSequence { get; }

        IEnumerable<IInputSpan?> PinnedProject { get; }
        IEnumerable<IInputSpan?> RecentProject { get; }

        IInputSpan ViewerNone { get; }
        IInputSpan ViewerValuesOverlay { get; }

        IInputSpan AlwaysOnTop { get; }
        IInputSpan AutoReload { get; }
        IInputSpan RestartOnAutoReload { get; }
        IInputSpan ClearStateOnRestart { get; }
        IInputSpan PauseOnInactivity { get; }
        IInputSpan RenderInputEventsWhenPaused { get; }
        IInputSpan WrapShaderInputCursor { get; }
        IInputSpan EnableShaderCache { get; }
        IInputSpan DarkTheme { get; }
        IInputSpan OpenSettingsFile { get; }
        IInputSpan OpenInputsFile { get; }
        IInputSpan OpenThemeFile { get; }
    }

    public class ApplicationInputs : IApplicationInputs
    {
        public IPositionGraphInputs PositionGraph { get; }

        public string Path { get; }

        public IInputSpan Play { get; }
        public IInputSpan Pause { get; }
        public IInputSpan Step { get; }
        public IInputSpan Restart { get; }
        public IInputSpan Uniforms { get; }
        public IInputSpan StartPage { get; }
        public IInputSpan ProjectNew { get; }
        public IInputSpan ProjectOpen { get; }
        public IInputSpan ProjectReload { get; }
        public IInputSpan ProjectSave { get; }
        public IInputSpan Help { get; }

        public IInputSpan FullScreenToggle { get; }
        public IInputSpan FullScreenLeave { get; }

        public IInputSpan ShaderMouseState { get; }

        public IInputSpan MenuMain { get; }
        public IInputSpan MenuRecentProjects { get; }
        public IInputSpan MenuProjectFiles { get; }
        public IInputSpan MenuBuffers { get; }
        public IInputSpan MenuExport { get; }
        public IInputSpan MenuCopy { get; }
        public IInputSpan MenuResolution { get; }
        public IInputSpan MenuFrameRate { get; }
        public IInputSpan MenuSpeed { get; }
        public IInputSpan MenuViewer { get; }
        public IInputSpan MenuOptions { get; }

        public IInputSpan ResizeSnapSmall { get; }
        public IInputSpan ResizeSnapMedium { get; }
        public IInputSpan ResizeKeepRatio { get; }

        public IInputSpan ViewerPan { get; }
        public IInputSpan ViewerPanSpeed { get; }
        public IInputSpan ViewerPanSnap { get; }
        public IInputSpan ViewerScale { get; }
        public IInputSpan ViewerScaleUp { get; }
        public IInputSpan ViewerScaleDown { get; }
        public IInputSpan ViewerScaleReset { get; }
        public IInputSpan ViewerScaleSpeed { get; }

        public IInputSpan CopyRepeat { get; }
        public IInputSpan CopyFrame { get; }
        public IInputSpan CopyFrameWithAlpha { get; }

        public IInputSpan ProjectOpenFolder { get; }

        public IInputSpan FrameRateFull { get; }
        public IInputSpan FrameRate2 { get; }
        public IInputSpan FrameRate4 { get; }
        public IInputSpan FrameRate8 { get; }
        public IInputSpan FrameRate16 { get; }
        public IInputSpan FrameRateIncrease { get; }
        public IInputSpan FrameRateDecrease { get; }

        public IInputSpan ResolutionFull { get; }
        public IInputSpan Resolution2 { get; }
        public IInputSpan Resolution4 { get; }
        public IInputSpan Resolution8 { get; }
        public IInputSpan Resolution16 { get; }
        public IInputSpan Resolution32 { get; }
        public IInputSpan Resolution64 { get; }
        public IInputSpan ResolutionIncrease { get; }
        public IInputSpan ResolutionDecrease { get; }

        public IInputSpan Speed1_16 { get; }
        public IInputSpan Speed1_8 { get; }
        public IInputSpan Speed1_4 { get; }
        public IInputSpan Speed1_2 { get; }
        public IInputSpan SpeedNormal { get; }
        public IInputSpan Speed2 { get; }
        public IInputSpan Speed4 { get; }
        public IInputSpan Speed8 { get; }
        public IInputSpan Speed16 { get; }
        public IInputSpan SpeedIncrease { get; }
        public IInputSpan SpeedDecrease { get; }

        public IEnumerable<IInputSpan?> Buffer { get; }
        public IInputSpan BufferImage { get; }
        public IInputSpan BufferNext { get; }
        public IInputSpan BufferPrevious { get; }

        public IInputSpan ExportFrame { get; }
        public IInputSpan ExportFrameRepeat { get; }
        public IInputSpan ExportRenderSequence { get; }

        public IEnumerable<IInputSpan?> PinnedProject { get; }
        public IEnumerable<IInputSpan?> RecentProject { get; }

        public IInputSpan ViewerNone { get; }
        public IInputSpan ViewerValuesOverlay { get; }

        public IInputSpan AlwaysOnTop { get; }
        public IInputSpan AutoReload { get; }
        public IInputSpan RestartOnAutoReload { get; }
        public IInputSpan ClearStateOnRestart { get; }
        public IInputSpan PauseOnInactivity { get; }
        public IInputSpan RenderInputEventsWhenPaused { get; }
        public IInputSpan WrapShaderInputCursor { get; }
        public IInputSpan EnableShaderCache { get; }
        public IInputSpan DarkTheme { get; }
        public IInputSpan OpenSettingsFile { get; }
        public IInputSpan OpenInputsFile { get; }
        public IInputSpan OpenThemeFile { get; }

        public ApplicationInputs(IInputSettings settings, string settingsPath)
        {
            this.Path = settingsPath;

            var factory = InputSpanFactory.Instance;

            this.Play = settings.GetOrSetDefault("Shader.Play", factory.All(ModifierKey.Alt, Key.Up));
            this.Pause = settings.GetOrSetDefault("Shader.Pause", factory.All(ModifierKey.Alt, Key.Up));
            this.Step = settings.GetOrSetDefault("Shader.Step", factory.All(ModifierKey.Alt, Key.Right), factory.Create(Key.OemPlus));
            this.Restart = settings.GetOrSetDefault("Shader.Restart", factory.All(ModifierKey.Alt, Key.Left), factory.All(ModifierKey.Alt, Key.Down));
            this.Uniforms = settings.GetOrSetDefault("Uniforms", factory.All(ModifierKey.Ctrl, Key.U));
            this.StartPage = settings.GetOrSetDefault("StartPage");
            this.ProjectNew = settings.GetOrSetDefault("Project.New", factory.All(ModifierKey.Ctrl, Key.N));
            this.ProjectOpen = settings.GetOrSetDefault("Project.Open", factory.All(ModifierKey.Ctrl, Key.O));
            this.ProjectReload = settings.GetOrSetDefault("Project.Reload", factory.All(ModifierKey.Ctrl, ModifierKey.Shift, Key.R));
            this.ProjectSave = settings.GetOrSetDefault("Project.Save", factory.All(ModifierKey.Ctrl, Key.S));
            this.Help = settings.GetOrSetDefault("Help");

            this.ShaderMouseState = settings.GetOrSetDefault("Shader.Mouse", factory.Create(MouseButton.Left));

            this.MenuMain = settings.GetOrSetDefault("Menu.Main", factory.Create(MouseButton.Right), factory.Create(Key.Apps));
            this.MenuRecentProjects = settings.GetOrSetDefault("Menu.RecentProjects", factory.All(ModifierKey.Ctrl, ModifierKey.Shift, Key.O));
            this.MenuProjectFiles = settings.GetOrSetDefault("Menu.ProjectFiles", factory.All(ModifierKey.Ctrl, ModifierKey.Shift, Key.F));
            this.MenuBuffers = settings.GetOrSetDefault("Menu.Buffers", factory.All(ModifierKey.Ctrl, Key.B));
            this.MenuExport = settings.GetOrSetDefault("Menu.Export", factory.All(ModifierKey.Ctrl, ModifierKey.Shift, Key.E));
            this.MenuCopy = settings.GetOrSetDefault("Menu.Copy", factory.All(ModifierKey.Ctrl, ModifierKey.Shift, Key.C));
            this.MenuResolution = settings.GetOrSetDefault("Menu.Resolution");
            this.MenuFrameRate = settings.GetOrSetDefault("Menu.FrameRate");
            this.MenuSpeed = settings.GetOrSetDefault("Menu.Speed");
            this.MenuViewer = settings.GetOrSetDefault("Menu.Viewer");
            this.MenuOptions = settings.GetOrSetDefault("Menu.Options");

            this.ResizeSnapSmall = settings.GetOrSetDefault("Resize.SnapSmall", factory.Create(ModifierKey.Shift));
            this.ResizeSnapMedium = settings.GetOrSetDefault("Resize.SnapMedium", factory.Create(ModifierKey.Ctrl));
            this.ResizeKeepRatio = settings.GetOrSetDefault("Resize.KeepRatio", factory.Create(ModifierKey.Alt));

            this.ViewerPan = settings.GetOrSetDefault("Viewer.Pan", factory.Create(MouseButton.Middle));
            this.ViewerPanSpeed = settings.GetOrSetDefault("Viewer.PanSpeed", factory.Create(ModifierKey.Shift));
            this.ViewerPanSnap = settings.GetOrSetDefault("Viewer.PanSnap", factory.Create(ModifierKey.Alt));
            this.ViewerScale = settings.GetOrSetDefault("Viewer.Scale", factory.All(ModifierKey.Ctrl, MouseButton.Middle), factory.All(MouseButton.Right, MouseButton.Middle));
            this.ViewerScaleUp = settings.GetOrSetDefault("Viewer.ScaleUp", factory.All(ModifierKey.Ctrl, MouseScroll.ScrollUp), factory.All(MouseButton.Right, MouseScroll.ScrollUp), factory.All(ModifierKey.Ctrl, Key.OemPlus));
            this.ViewerScaleDown = settings.GetOrSetDefault("Viewer.ScaleDown", factory.All(ModifierKey.Ctrl, MouseScroll.ScrollDown), factory.All(MouseButton.Right, MouseScroll.ScrollDown), factory.All(ModifierKey.Ctrl, Key.OemMinus));
            this.ViewerScaleReset = settings.GetOrSetDefault("Viewer.ScaleReset", factory.All(ModifierKey.Ctrl, Key.D0));
            this.ViewerScaleSpeed = settings.GetOrSetDefault("Viewer.ScaleSpeed", factory.Create(ModifierKey.Shift));

            this.CopyRepeat = settings.GetOrSetDefault("Copy.Repeat", factory.All(ModifierKey.Ctrl, Key.C));
            this.CopyFrame = settings.GetOrSetDefault("Copy.Frame");
            this.CopyFrameWithAlpha = settings.GetOrSetDefault("Copy.FrameWithAlpha");

            this.FullScreenToggle = settings.GetOrSetDefault("FullScreen.Toggle", factory.Create(Key.F11));
            this.FullScreenLeave = settings.GetOrSetDefault("FullScreen.Leave", factory.Create(Key.Escape));
            this.ProjectOpenFolder = settings.GetOrSetDefault("Project.OpenFolder");

            this.FrameRateFull = settings.GetOrSetDefault("FrameRate.Full");
            this.FrameRate2 = settings.GetOrSetDefault("FrameRate.2");
            this.FrameRate4 = settings.GetOrSetDefault("FrameRate.4");
            this.FrameRate8 = settings.GetOrSetDefault("FrameRate.8");
            this.FrameRate16 = settings.GetOrSetDefault("FrameRate.16");
            this.FrameRateDecrease = settings.GetOrSetDefault("FrameRate.Decrease");
            this.FrameRateIncrease = settings.GetOrSetDefault("FrameRate.Increase");

            this.ResolutionFull = settings.GetOrSetDefault("Resolution.Full", factory.All(ModifierKey.Ctrl, Key.OemPipe));
            this.Resolution2 = settings.GetOrSetDefault("Resolution.2");
            this.Resolution4 = settings.GetOrSetDefault("Resolution.4");
            this.Resolution8 = settings.GetOrSetDefault("Resolution.8");
            this.Resolution16 = settings.GetOrSetDefault("Resolution.16");
            this.Resolution32 = settings.GetOrSetDefault("Resolution.32");
            this.Resolution64 = settings.GetOrSetDefault("Resolution.64");
            this.ResolutionDecrease = settings.GetOrSetDefault("Resolution.Decrease", factory.All(ModifierKey.Ctrl, Key.OemOpenBrackets));
            this.ResolutionIncrease = settings.GetOrSetDefault("Resolution.Increase", factory.All(ModifierKey.Ctrl, Key.OemCloseBrackets));

            this.Speed1_16 = settings.GetOrSetDefault("Speed.1_16");
            this.Speed1_8 = settings.GetOrSetDefault("Speed.1_8");
            this.Speed1_4 = settings.GetOrSetDefault("Speed.1_4");
            this.Speed1_2 = settings.GetOrSetDefault("Speed.1_2");
            this.SpeedNormal = settings.GetOrSetDefault("Speed.Normal", factory.All(ModifierKey.Shift, Key.OemQuestion));
            this.Speed2 = settings.GetOrSetDefault("Speed.2");
            this.Speed4 = settings.GetOrSetDefault("Speed.4");
            this.Speed8 = settings.GetOrSetDefault("Speed.8");
            this.Speed16 = settings.GetOrSetDefault("Speed.16");
            this.SpeedIncrease = settings.GetOrSetDefault("Speed.Increase", factory.All(ModifierKey.Shift, Key.OemPeriod));
            this.SpeedDecrease = settings.GetOrSetDefault("Speed.Decrease", factory.All(ModifierKey.Shift, Key.OemComma));

            this.Buffer = Enumerable.Range(0, 8).Select(index => settings.GetOrSetDefault($"Buffer.{index + 1}")).ToArray();
            this.BufferImage = settings.GetOrSetDefault("Buffer.Image", factory.All(ModifierKey.Ctrl, Key.OemQuestion));
            this.BufferNext = settings.GetOrSetDefault("Buffer.Next", factory.All(ModifierKey.Ctrl, Key.OemPeriod));
            this.BufferPrevious = settings.GetOrSetDefault("Buffer.Previous", factory.All(ModifierKey.Ctrl, Key.OemComma));

            this.ExportFrame = settings.GetOrSetDefault("Export.Frame");
            this.ExportFrameRepeat = settings.GetOrSetDefault("Export.FrameRepeat");
            this.ExportRenderSequence = settings.GetOrSetDefault("Export.RenderSequence");

            this.PinnedProject = Enumerable.Range(0, 5).Select(index => settings.GetOrSetDefault($"PinnedProject.{index + 1}")).ToArray();
            this.RecentProject = Enumerable.Range(0, 5).Select(index => settings.GetOrSetDefault($"RecentProject.{index + 1}", index == 0 ? factory.All(ModifierKey.Ctrl, ModifierKey.Shift, ModifierKey.Alt, Key.O) : InputSpan.None)).ToArray();

            this.ViewerNone = settings.GetOrSetDefault("Viewer.None");
            this.ViewerValuesOverlay = settings.GetOrSetDefault("Viewer.ValuesOverlay");

            this.AlwaysOnTop = settings.GetOrSetDefault("Options.AlwaysOnTop", factory.All(ModifierKey.Ctrl, ModifierKey.Shift, Key.A));
            this.AutoReload = settings.GetOrSetDefault("Options.AutoReload");
            this.RestartOnAutoReload = settings.GetOrSetDefault("Options.RestartOnAutoReload");
            this.ClearStateOnRestart = settings.GetOrSetDefault("Options.ClearStateOnRestart");
            this.PauseOnInactivity = settings.GetOrSetDefault("Options.PauseOnInactivity");
            this.RenderInputEventsWhenPaused = settings.GetOrSetDefault("Options.RenderInputEventsWhenPaused");
            this.WrapShaderInputCursor = settings.GetOrSetDefault("Options.WrapShaderInputCursor");
            this.EnableShaderCache = settings.GetOrSetDefault("Options.EnableShaderCache");
            this.DarkTheme = settings.GetOrSetDefault("Options.DarkTheme");
            this.OpenSettingsFile = settings.GetOrSetDefault("Options.OpenSettingsFile");
            this.OpenInputsFile = settings.GetOrSetDefault("Options.OpenInputsFile");
            this.OpenThemeFile = settings.GetOrSetDefault("Options.OpenThemeFile");

            this.PositionGraph = new PositionGraphInputs(settings, "Graph");
        }
    }
}
