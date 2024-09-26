namespace Shaderlens
{
    public interface IApplicationInputs
    {
        IPositionGraphInputs PositionGraph { get; }

        string Path { get; }

        IInputSpanEvent Play { get; }
        IInputSpanEvent Pause { get; }
        IInputSpanEvent Step { get; }
        IInputSpanEvent Restart { get; }
        IInputSpanEvent Uniforms { get; }
        IInputSpanEvent StartPage { get; }
        IInputSpanEvent ProjectNew { get; }
        IInputSpanEvent ProjectOpen { get; }
        IInputSpanEvent ProjectReload { get; }
        IInputSpanEvent ProjectSave { get; }
        IInputSpanEvent Help { get; }

        IInputSpanEvent FullScreenToggle { get; }
        IInputSpanEvent FullScreenLeave { get; }

        IInputSpan ShaderMouseState { get; }

        IInputSpanEvent MenuMain { get; }
        IInputSpanEvent MenuRecentProjects { get; }
        IInputSpanEvent MenuProjectFiles { get; }
        IInputSpanEvent MenuBuffers { get; }
        IInputSpanEvent MenuExport { get; }
        IInputSpanEvent MenuCopy { get; }
        IInputSpanEvent MenuResolution { get; }
        IInputSpanEvent MenuFrameRate { get; }
        IInputSpanEvent MenuSpeed { get; }
        IInputSpanEvent MenuViewer { get; }
        IInputSpanEvent MenuOptions { get; }

        IInputSpan ResizeSnapSmall { get; }
        IInputSpan ResizeSnapMedium { get; }
        IInputSpan ResizeKeepRatio { get; }

        IInputSpan ViewerPan { get; }
        IInputSpan ViewerPanSpeed { get; }
        IInputSpan ViewerPanSnap { get; }
        IInputSpan ViewerScale { get; }
        IInputSpanEvent ViewerScaleUp { get; }
        IInputSpanEvent ViewerScaleDown { get; }
        IInputSpanEvent ViewerScaleReset { get; }
        IInputSpan ViewerScaleSpeed { get; }

        IInputSpanEvent CopyRepeat { get; }
        IInputSpanEvent CopyFrame { get; }
        IInputSpanEvent CopyFrameWithAlpha { get; }

        IInputSpanEvent ProjectOpenFolder { get; }

        IInputSpanEvent FrameRateFull { get; }
        IInputSpanEvent FrameRate2 { get; }
        IInputSpanEvent FrameRate4 { get; }
        IInputSpanEvent FrameRate8 { get; }
        IInputSpanEvent FrameRate16 { get; }
        IInputSpanEvent FrameRateIncrease { get; }
        IInputSpanEvent FrameRateDecrease { get; }

        IInputSpanEvent ResolutionFull { get; }
        IInputSpanEvent Resolution2 { get; }
        IInputSpanEvent Resolution4 { get; }
        IInputSpanEvent Resolution8 { get; }
        IInputSpanEvent Resolution16 { get; }
        IInputSpanEvent Resolution32 { get; }
        IInputSpanEvent Resolution64 { get; }
        IInputSpanEvent ResolutionIncrease { get; }
        IInputSpanEvent ResolutionDecrease { get; }

        IInputSpanEvent Speed1_16 { get; }
        IInputSpanEvent Speed1_8 { get; }
        IInputSpanEvent Speed1_4 { get; }
        IInputSpanEvent Speed1_2 { get; }
        IInputSpanEvent SpeedNormal { get; }
        IInputSpanEvent Speed2 { get; }
        IInputSpanEvent Speed4 { get; }
        IInputSpanEvent Speed8 { get; }
        IInputSpanEvent Speed16 { get; }
        IInputSpanEvent SpeedIncrease { get; }
        IInputSpanEvent SpeedDecrease { get; }

        IEnumerable<IInputSpanEvent?> Buffer { get; }
        IInputSpanEvent BufferImage { get; }
        IInputSpanEvent BufferNext { get; }
        IInputSpanEvent BufferPrevious { get; }

        IInputSpanEvent ExportFrame { get; }
        IInputSpanEvent ExportFrameRepeat { get; }
        IInputSpanEvent ExportRenderSequence { get; }

        IEnumerable<IInputSpanEvent?> PinnedProject { get; }
        IEnumerable<IInputSpanEvent?> RecentProject { get; }

        IInputSpanEvent ViewerNone { get; }
        IInputSpanEvent ViewerValuesOverlay { get; }

        IInputSpanEvent AlwaysOnTop { get; }
        IInputSpanEvent AutoReload { get; }
        IInputSpanEvent RestartOnAutoReload { get; }
        IInputSpanEvent ClearStateOnRestart { get; }
        IInputSpanEvent PauseOnInactivity { get; }
        IInputSpanEvent RenderInputEventsWhenPaused { get; }
        IInputSpanEvent WrapShaderInputCursor { get; }
        IInputSpanEvent EnableShaderCache { get; }
        IInputSpanEvent DarkTheme { get; }
        IInputSpanEvent OpenSettingsFile { get; }
        IInputSpanEvent OpenInputsFile { get; }
        IInputSpanEvent OpenThemeFile { get; }
    }

    public class ApplicationInputs : IApplicationInputs
    {
        public IPositionGraphInputs PositionGraph { get; }

        public string Path { get; }

        public IInputSpanEvent Play { get; }
        public IInputSpanEvent Pause { get; }
        public IInputSpanEvent Step { get; }
        public IInputSpanEvent Restart { get; }
        public IInputSpanEvent Uniforms { get; }
        public IInputSpanEvent StartPage { get; }
        public IInputSpanEvent ProjectNew { get; }
        public IInputSpanEvent ProjectOpen { get; }
        public IInputSpanEvent ProjectReload { get; }
        public IInputSpanEvent ProjectSave { get; }
        public IInputSpanEvent Help { get; }

        public IInputSpanEvent FullScreenToggle { get; }
        public IInputSpanEvent FullScreenLeave { get; }

        public IInputSpan ShaderMouseState { get; }

        public IInputSpanEvent MenuMain { get; }
        public IInputSpanEvent MenuRecentProjects { get; }
        public IInputSpanEvent MenuProjectFiles { get; }
        public IInputSpanEvent MenuBuffers { get; }
        public IInputSpanEvent MenuExport { get; }
        public IInputSpanEvent MenuCopy { get; }
        public IInputSpanEvent MenuResolution { get; }
        public IInputSpanEvent MenuFrameRate { get; }
        public IInputSpanEvent MenuSpeed { get; }
        public IInputSpanEvent MenuViewer { get; }
        public IInputSpanEvent MenuOptions { get; }

        public IInputSpan ResizeSnapSmall { get; }
        public IInputSpan ResizeSnapMedium { get; }
        public IInputSpan ResizeKeepRatio { get; }

        public IInputSpan ViewerPan { get; }
        public IInputSpan ViewerPanSpeed { get; }
        public IInputSpan ViewerPanSnap { get; }
        public IInputSpan ViewerScale { get; }
        public IInputSpanEvent ViewerScaleUp { get; }
        public IInputSpanEvent ViewerScaleDown { get; }
        public IInputSpanEvent ViewerScaleReset { get; }
        public IInputSpan ViewerScaleSpeed { get; }

        public IInputSpanEvent CopyRepeat { get; }
        public IInputSpanEvent CopyFrame { get; }
        public IInputSpanEvent CopyFrameWithAlpha { get; }

        public IInputSpanEvent ProjectOpenFolder { get; }

        public IInputSpanEvent FrameRateFull { get; }
        public IInputSpanEvent FrameRate2 { get; }
        public IInputSpanEvent FrameRate4 { get; }
        public IInputSpanEvent FrameRate8 { get; }
        public IInputSpanEvent FrameRate16 { get; }
        public IInputSpanEvent FrameRateIncrease { get; }
        public IInputSpanEvent FrameRateDecrease { get; }

        public IInputSpanEvent ResolutionFull { get; }
        public IInputSpanEvent Resolution2 { get; }
        public IInputSpanEvent Resolution4 { get; }
        public IInputSpanEvent Resolution8 { get; }
        public IInputSpanEvent Resolution16 { get; }
        public IInputSpanEvent Resolution32 { get; }
        public IInputSpanEvent Resolution64 { get; }
        public IInputSpanEvent ResolutionIncrease { get; }
        public IInputSpanEvent ResolutionDecrease { get; }

        public IInputSpanEvent Speed1_16 { get; }
        public IInputSpanEvent Speed1_8 { get; }
        public IInputSpanEvent Speed1_4 { get; }
        public IInputSpanEvent Speed1_2 { get; }
        public IInputSpanEvent SpeedNormal { get; }
        public IInputSpanEvent Speed2 { get; }
        public IInputSpanEvent Speed4 { get; }
        public IInputSpanEvent Speed8 { get; }
        public IInputSpanEvent Speed16 { get; }
        public IInputSpanEvent SpeedIncrease { get; }
        public IInputSpanEvent SpeedDecrease { get; }

        public IEnumerable<IInputSpanEvent?> Buffer { get; }
        public IInputSpanEvent BufferImage { get; }
        public IInputSpanEvent BufferNext { get; }
        public IInputSpanEvent BufferPrevious { get; }

        public IInputSpanEvent ExportFrame { get; }
        public IInputSpanEvent ExportFrameRepeat { get; }
        public IInputSpanEvent ExportRenderSequence { get; }

        public IEnumerable<IInputSpanEvent?> PinnedProject { get; }
        public IEnumerable<IInputSpanEvent?> RecentProject { get; }

        public IInputSpanEvent ViewerNone { get; }
        public IInputSpanEvent ViewerValuesOverlay { get; }

        public IInputSpanEvent AlwaysOnTop { get; }
        public IInputSpanEvent AutoReload { get; }
        public IInputSpanEvent RestartOnAutoReload { get; }
        public IInputSpanEvent ClearStateOnRestart { get; }
        public IInputSpanEvent PauseOnInactivity { get; }
        public IInputSpanEvent RenderInputEventsWhenPaused { get; }
        public IInputSpanEvent WrapShaderInputCursor { get; }
        public IInputSpanEvent EnableShaderCache { get; }
        public IInputSpanEvent DarkTheme { get; }
        public IInputSpanEvent OpenSettingsFile { get; }
        public IInputSpanEvent OpenInputsFile { get; }
        public IInputSpanEvent OpenThemeFile { get; }

        public ApplicationInputs(IInputSettings settings, string settingsPath)
        {
            this.Path = settingsPath;

            var factory = InputSpanFactory.Instance;

            this.Play = settings.GetOrSetDefault("Shader.Play", factory.AllStart(ModifierKey.Alt, Key.Up), factory.AllGlobal(ModifierKey.Ctrl, ModifierKey.Shift, ModifierKey.Win, Key.P));
            this.Pause = settings.GetOrSetDefault("Shader.Pause", factory.AllStart(ModifierKey.Alt, Key.Up), factory.AllGlobal(ModifierKey.Ctrl, ModifierKey.Shift, ModifierKey.Win, Key.P));
            this.Step = settings.GetOrSetDefault("Shader.Step", factory.AllStart(ModifierKey.Alt, Key.Right), factory.CreateStart(Key.OemTilde));
            this.Restart = settings.GetOrSetDefault("Shader.Restart", factory.AllStart(ModifierKey.Alt, Key.Left), factory.AllStart(ModifierKey.Alt, Key.Down), factory.AllGlobal(ModifierKey.Ctrl, ModifierKey.Shift, ModifierKey.Win, Key.O));
            this.Uniforms = settings.GetOrSetDefault("Uniforms", factory.AllEnd(ModifierKey.Ctrl, Key.U));
            this.StartPage = settings.GetOrSetDefault("StartPage", InputSpanEvent.None);
            this.ProjectNew = settings.GetOrSetDefault("Project.New", factory.AllStart(ModifierKey.Ctrl, Key.N));
            this.ProjectOpen = settings.GetOrSetDefault("Project.Open", factory.AllStart(ModifierKey.Ctrl, Key.O));
            this.ProjectReload = settings.GetOrSetDefault("Project.Reload", factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, Key.R));
            this.ProjectSave = settings.GetOrSetDefault("Project.Save", factory.AllStart(ModifierKey.Ctrl, Key.S));
            this.Help = settings.GetOrSetDefault("Help", InputSpanEvent.None);

            this.ShaderMouseState = settings.GetOrSetDefault("Shader.Mouse", factory.Create(MouseButton.Left));

            this.MenuMain = settings.GetOrSetDefault("Menu.Main", factory.CreateEnd(MouseButton.Right), factory.CreateEnd(Key.Apps));
            this.MenuRecentProjects = settings.GetOrSetDefault("Menu.RecentProjects", factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, Key.O));
            this.MenuProjectFiles = settings.GetOrSetDefault("Menu.ProjectFiles", factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, Key.F));
            this.MenuBuffers = settings.GetOrSetDefault("Menu.Buffers", factory.AllStart(ModifierKey.Ctrl, Key.B));
            this.MenuExport = settings.GetOrSetDefault("Menu.Export", factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, Key.E));
            this.MenuCopy = settings.GetOrSetDefault("Menu.Copy", factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, Key.C));
            this.MenuResolution = settings.GetOrSetDefault("Menu.Resolution", InputSpanEvent.None);
            this.MenuFrameRate = settings.GetOrSetDefault("Menu.FrameRate", InputSpanEvent.None);
            this.MenuSpeed = settings.GetOrSetDefault("Menu.Speed", InputSpanEvent.None);
            this.MenuViewer = settings.GetOrSetDefault("Menu.Viewer", InputSpanEvent.None);
            this.MenuOptions = settings.GetOrSetDefault("Menu.Options", InputSpanEvent.None);

            this.ResizeSnapSmall = settings.GetOrSetDefault("Resize.SnapSmall", factory.Create(ModifierKey.Shift));
            this.ResizeSnapMedium = settings.GetOrSetDefault("Resize.SnapMedium", factory.Create(ModifierKey.Ctrl));
            this.ResizeKeepRatio = settings.GetOrSetDefault("Resize.KeepRatio", factory.Create(ModifierKey.Alt));

            this.ViewerPan = settings.GetOrSetDefault("Viewer.Pan", factory.Create(MouseButton.Middle));
            this.ViewerPanSpeed = settings.GetOrSetDefault("Viewer.PanSpeed", factory.Create(ModifierKey.Shift));
            this.ViewerPanSnap = settings.GetOrSetDefault("Viewer.PanSnap", factory.Create(ModifierKey.Alt));
            this.ViewerScale = settings.GetOrSetDefault("Viewer.Scale", factory.All(ModifierKey.Ctrl, MouseButton.Middle), factory.All(MouseButton.Right, MouseButton.Middle));
            this.ViewerScaleUp = settings.GetOrSetDefault("Viewer.ScaleUp", factory.AllStart(ModifierKey.Ctrl, MouseScroll.ScrollUp), factory.AllStart(MouseButton.Right, MouseScroll.ScrollUp), factory.AllStart(ModifierKey.Ctrl, Key.OemPlus));
            this.ViewerScaleDown = settings.GetOrSetDefault("Viewer.ScaleDown", factory.AllStart(ModifierKey.Ctrl, MouseScroll.ScrollDown), factory.AllStart(MouseButton.Right, MouseScroll.ScrollDown), factory.AllStart(ModifierKey.Ctrl, Key.OemMinus));
            this.ViewerScaleReset = settings.GetOrSetDefault("Viewer.ScaleReset", factory.AllStart(ModifierKey.Ctrl, Key.D0));
            this.ViewerScaleSpeed = settings.GetOrSetDefault("Viewer.ScaleSpeed", factory.Create(ModifierKey.Shift));

            this.CopyRepeat = settings.GetOrSetDefault("Copy.Repeat", factory.AllStart(ModifierKey.Ctrl, Key.C));
            this.CopyFrame = settings.GetOrSetDefault("Copy.Frame", InputSpanEvent.None);
            this.CopyFrameWithAlpha = settings.GetOrSetDefault("Copy.FrameWithAlpha", InputSpanEvent.None);

            this.FullScreenToggle = settings.GetOrSetDefault("FullScreen.Toggle", factory.CreateStart(Key.F11));
            this.FullScreenLeave = settings.GetOrSetDefault("FullScreen.Leave", factory.CreateStart(Key.Escape));
            this.ProjectOpenFolder = settings.GetOrSetDefault("Project.OpenFolder", InputSpanEvent.None);

            this.FrameRateFull = settings.GetOrSetDefault("FrameRate.Full", InputSpanEvent.None);
            this.FrameRate2 = settings.GetOrSetDefault("FrameRate.2", InputSpanEvent.None);
            this.FrameRate4 = settings.GetOrSetDefault("FrameRate.4", InputSpanEvent.None);
            this.FrameRate8 = settings.GetOrSetDefault("FrameRate.8", InputSpanEvent.None);
            this.FrameRate16 = settings.GetOrSetDefault("FrameRate.16", InputSpanEvent.None);
            this.FrameRateDecrease = settings.GetOrSetDefault("FrameRate.Decrease", InputSpanEvent.None);
            this.FrameRateIncrease = settings.GetOrSetDefault("FrameRate.Increase", InputSpanEvent.None);

            this.ResolutionFull = settings.GetOrSetDefault("Resolution.Full", factory.AllStart(ModifierKey.Ctrl, Key.OemPipe));
            this.Resolution2 = settings.GetOrSetDefault("Resolution.2", InputSpanEvent.None);
            this.Resolution4 = settings.GetOrSetDefault("Resolution.4", InputSpanEvent.None);
            this.Resolution8 = settings.GetOrSetDefault("Resolution.8", InputSpanEvent.None);
            this.Resolution16 = settings.GetOrSetDefault("Resolution.16", InputSpanEvent.None);
            this.Resolution32 = settings.GetOrSetDefault("Resolution.32", InputSpanEvent.None);
            this.Resolution64 = settings.GetOrSetDefault("Resolution.64", InputSpanEvent.None);
            this.ResolutionDecrease = settings.GetOrSetDefault("Resolution.Decrease", factory.AllStart(ModifierKey.Ctrl, Key.OemOpenBrackets));
            this.ResolutionIncrease = settings.GetOrSetDefault("Resolution.Increase", factory.AllStart(ModifierKey.Ctrl, Key.OemCloseBrackets));

            this.Speed1_16 = settings.GetOrSetDefault("Speed.1_16", InputSpanEvent.None);
            this.Speed1_8 = settings.GetOrSetDefault("Speed.1_8", InputSpanEvent.None);
            this.Speed1_4 = settings.GetOrSetDefault("Speed.1_4", InputSpanEvent.None);
            this.Speed1_2 = settings.GetOrSetDefault("Speed.1_2", InputSpanEvent.None);
            this.SpeedNormal = settings.GetOrSetDefault("Speed.Normal", factory.AllStart(ModifierKey.Shift, Key.OemQuestion));
            this.Speed2 = settings.GetOrSetDefault("Speed.2", InputSpanEvent.None);
            this.Speed4 = settings.GetOrSetDefault("Speed.4", InputSpanEvent.None);
            this.Speed8 = settings.GetOrSetDefault("Speed.8", InputSpanEvent.None);
            this.Speed16 = settings.GetOrSetDefault("Speed.16", InputSpanEvent.None);
            this.SpeedIncrease = settings.GetOrSetDefault("Speed.Increase", factory.AllStart(ModifierKey.Shift, Key.OemPeriod));
            this.SpeedDecrease = settings.GetOrSetDefault("Speed.Decrease", factory.AllStart(ModifierKey.Shift, Key.OemComma));

            this.Buffer = Enumerable.Range(0, 8).Select(index => settings.GetOrSetDefault($"Buffer.{index + 1}", InputSpanEvent.None)).ToArray();
            this.BufferImage = settings.GetOrSetDefault("Buffer.Image", factory.AllStart(ModifierKey.Ctrl, Key.OemQuestion));
            this.BufferNext = settings.GetOrSetDefault("Buffer.Next", factory.AllStart(ModifierKey.Ctrl, Key.OemPeriod));
            this.BufferPrevious = settings.GetOrSetDefault("Buffer.Previous", factory.AllStart(ModifierKey.Ctrl, Key.OemComma));

            this.ExportFrame = settings.GetOrSetDefault("Export.Frame", InputSpanEvent.None);
            this.ExportFrameRepeat = settings.GetOrSetDefault("Export.FrameRepeat", InputSpanEvent.None);
            this.ExportRenderSequence = settings.GetOrSetDefault("Export.RenderSequence", InputSpanEvent.None);

            this.PinnedProject = Enumerable.Range(0, 5).Select(index => settings.GetOrSetDefault($"PinnedProject.{index + 1}", InputSpanEvent.None)).ToArray();
            this.RecentProject = Enumerable.Range(0, 5).Select(index => settings.GetOrSetDefault($"RecentProject.{index + 1}", index == 0 ? factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, ModifierKey.Alt, Key.O) : InputSpanEvent.None)).ToArray();

            this.ViewerNone = settings.GetOrSetDefault("Viewer.None", InputSpanEvent.None);
            this.ViewerValuesOverlay = settings.GetOrSetDefault("Viewer.ValuesOverlay", InputSpanEvent.None);

            this.AlwaysOnTop = settings.GetOrSetDefault("Options.AlwaysOnTop", factory.AllStart(ModifierKey.Ctrl, ModifierKey.Shift, Key.A));
            this.AutoReload = settings.GetOrSetDefault("Options.AutoReload", InputSpanEvent.None);
            this.RestartOnAutoReload = settings.GetOrSetDefault("Options.RestartOnAutoReload", InputSpanEvent.None);
            this.ClearStateOnRestart = settings.GetOrSetDefault("Options.ClearStateOnRestart", InputSpanEvent.None);
            this.PauseOnInactivity = settings.GetOrSetDefault("Options.PauseOnInactivity", InputSpanEvent.None);
            this.RenderInputEventsWhenPaused = settings.GetOrSetDefault("Options.RenderInputEventsWhenPaused", InputSpanEvent.None);
            this.WrapShaderInputCursor = settings.GetOrSetDefault("Options.WrapShaderInputCursor", InputSpanEvent.None);
            this.EnableShaderCache = settings.GetOrSetDefault("Options.EnableShaderCache", InputSpanEvent.None);
            this.DarkTheme = settings.GetOrSetDefault("Options.DarkTheme", InputSpanEvent.None);
            this.OpenSettingsFile = settings.GetOrSetDefault("Options.OpenSettingsFile", InputSpanEvent.None);
            this.OpenInputsFile = settings.GetOrSetDefault("Options.OpenInputsFile", InputSpanEvent.None);
            this.OpenThemeFile = settings.GetOrSetDefault("Options.OpenThemeFile", InputSpanEvent.None);

            this.PositionGraph = new PositionGraphInputs(settings, "Graph");
        }
    }
}
