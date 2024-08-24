namespace Shaderlens
{
    public interface IApplicationInputs
    {
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

        private readonly IJsonSettings settings;
        private readonly IJsonSerializer<IInputSpan> serializer;

        public ApplicationInputs(IJsonSerializer<IInputSpan> serializer, IJsonSettings settings, string settingsPath)
        {
            this.settings = settings;
            this.serializer = serializer;

            this.Path = settingsPath;

            this.Play = GetInputSpan("Shader.Play", All(ModifierKey.Alt, Key.Up));
            this.Pause = GetInputSpan("Shader.Pause", All(ModifierKey.Alt, Key.Up));
            this.Step = GetInputSpan("Shader.Step", Any(All(ModifierKey.Alt, Key.Right), Key.OemPlus));
            this.Restart = GetInputSpan("Shader.Restart", Any(All(ModifierKey.Alt, Key.Left), All(ModifierKey.Alt, Key.Down)));
            this.Uniforms = GetInputSpan("Uniforms", All(ModifierKey.Ctrl, Key.U));
            this.StartPage = GetInputSpan("StartPage");
            this.ProjectNew = GetInputSpan("Project.New", All(ModifierKey.Ctrl, Key.N));
            this.ProjectOpen = GetInputSpan("Project.Open", All(ModifierKey.Ctrl, Key.O));
            this.ProjectReload = GetInputSpan("Project.Reload", All(ModifierKey.Ctrl, ModifierKey.Shift, Key.R));
            this.ProjectSave = GetInputSpan("Project.Save", All(ModifierKey.Ctrl, Key.S));
            this.Help = GetInputSpan("Help");

            this.ShaderMouseState = GetInputSpan("Shader.Mouse", MouseButton.Left);

            this.MenuMain = GetInputSpan("Menu.Main", Any(MouseButton.Right, Key.Apps));
            this.MenuRecentProjects = GetInputSpan("Menu.RecentProjects", All(ModifierKey.Ctrl, ModifierKey.Shift, Key.O));
            this.MenuProjectFiles = GetInputSpan("Menu.ProjectFiles", All(ModifierKey.Ctrl, ModifierKey.Shift, Key.F));
            this.MenuBuffers = GetInputSpan("Menu.Buffers", All(ModifierKey.Ctrl, Key.B));
            this.MenuExport = GetInputSpan("Menu.Export", All(ModifierKey.Ctrl, ModifierKey.Shift, Key.E));
            this.MenuCopy = GetInputSpan("Menu.Copy", All(ModifierKey.Ctrl, ModifierKey.Shift, Key.C));
            this.MenuResolution = GetInputSpan("Menu.Resolution");
            this.MenuFrameRate = GetInputSpan("Menu.FrameRate");
            this.MenuSpeed = GetInputSpan("Menu.Speed");
            this.MenuViewer = GetInputSpan("Menu.Viewer");
            this.MenuOptions = GetInputSpan("Menu.Options");

            this.ResizeSnapSmall = GetInputSpan("Resize.SnapSmall", ModifierKey.Shift);
            this.ResizeSnapMedium = GetInputSpan("Resize.SnapMedium", ModifierKey.Ctrl);
            this.ResizeKeepRatio = GetInputSpan("Resize.KeepRatio", ModifierKey.Alt);

            this.ViewerPan = GetInputSpan("Viewer.Pan", Any(MouseButton.Middle));
            this.ViewerPanSpeed = GetInputSpan("Viewer.PanSpeed", Any(ModifierKey.Shift));
            this.ViewerPanSnap = GetInputSpan("Viewer.PanSnap", Any(ModifierKey.Alt));
            this.ViewerScale = GetInputSpan("Viewer.Scale", Any(All(ModifierKey.Ctrl, MouseButton.Middle), All(MouseButton.Right, MouseButton.Middle)));
            this.ViewerScaleUp = GetInputSpan("Viewer.ScaleUp", Any(All(ModifierKey.Ctrl, MouseScroll.ScrollUp), All(MouseButton.Right, MouseScroll.ScrollUp), All(ModifierKey.Ctrl, Key.OemPlus)));
            this.ViewerScaleDown = GetInputSpan("Viewer.ScaleDown", Any(All(ModifierKey.Ctrl, MouseScroll.ScrollDown), All(MouseButton.Right, MouseScroll.ScrollDown), All(ModifierKey.Ctrl, Key.OemMinus)));
            this.ViewerScaleReset = GetInputSpan("Viewer.ScaleReset", All(ModifierKey.Ctrl, Key.D0));
            this.ViewerScaleSpeed = GetInputSpan("Viewer.ScaleSpeed", ModifierKey.Shift);

            this.CopyRepeat = GetInputSpan("Copy.Repeat", All(ModifierKey.Ctrl, Key.C));
            this.CopyFrame = GetInputSpan("Copy.Frame");
            this.CopyFrameWithAlpha = GetInputSpan("Copy.FrameWithAlpha");

            this.FullScreenToggle = GetInputSpan("FullScreen.Toggle", Key.F11);
            this.FullScreenLeave = GetInputSpan("FullScreen.Leave", Key.Escape);
            this.ProjectOpenFolder = GetInputSpan("Project.OpenFolder");

            this.FrameRateFull = GetInputSpan("FrameRate.Full");
            this.FrameRate2 = GetInputSpan("FrameRate.2");
            this.FrameRate4 = GetInputSpan("FrameRate.4");
            this.FrameRate8 = GetInputSpan("FrameRate.8");
            this.FrameRate16 = GetInputSpan("FrameRate.16");
            this.FrameRateDecrease = GetInputSpan("FrameRate.Decrease");
            this.FrameRateIncrease = GetInputSpan("FrameRate.Increase");

            this.ResolutionFull = GetInputSpan("Resolution.Full", All(ModifierKey.Ctrl, Key.OemPipe));
            this.Resolution2 = GetInputSpan("Resolution.2");
            this.Resolution4 = GetInputSpan("Resolution.4");
            this.Resolution8 = GetInputSpan("Resolution.8");
            this.Resolution16 = GetInputSpan("Resolution.16");
            this.Resolution32 = GetInputSpan("Resolution.32");
            this.Resolution64 = GetInputSpan("Resolution.64");
            this.ResolutionDecrease = GetInputSpan("Resolution.Decrease", All(ModifierKey.Ctrl, Key.OemOpenBrackets));
            this.ResolutionIncrease = GetInputSpan("Resolution.Increase", All(ModifierKey.Ctrl, Key.OemCloseBrackets));

            this.Speed1_16 = GetInputSpan("Speed.1_16");
            this.Speed1_8 = GetInputSpan("Speed.1_8");
            this.Speed1_4 = GetInputSpan("Speed.1_4");
            this.Speed1_2 = GetInputSpan("Speed.1_2");
            this.SpeedNormal = GetInputSpan("Speed.Normal", All(ModifierKey.Shift, Key.OemQuestion));
            this.Speed2 = GetInputSpan("Speed.2");
            this.Speed4 = GetInputSpan("Speed.4");
            this.Speed8 = GetInputSpan("Speed.8");
            this.Speed16 = GetInputSpan("Speed.16");
            this.SpeedIncrease = GetInputSpan("Speed.Increase", All(ModifierKey.Shift, Key.OemPeriod));
            this.SpeedDecrease = GetInputSpan("Speed.Decrease", All(ModifierKey.Shift, Key.OemComma));

            this.Buffer = Enumerable.Range(0, 8).Select(index => GetInputSpan($"Buffer.{index + 1}")).ToArray();
            this.BufferImage = GetInputSpan("Buffer.Image", All(ModifierKey.Ctrl, Key.OemQuestion));
            this.BufferNext = GetInputSpan("Buffer.Next", All(ModifierKey.Ctrl, Key.OemPeriod));
            this.BufferPrevious = GetInputSpan("Buffer.Previous", All(ModifierKey.Ctrl, Key.OemComma));

            this.ExportFrame = GetInputSpan("Export.Frame");
            this.ExportFrameRepeat = GetInputSpan("Export.FrameRepeat");
            this.ExportRenderSequence = GetInputSpan("Export.RenderSequence");

            this.PinnedProject = Enumerable.Range(0, 5).Select(index => GetInputSpan($"PinnedProject.{index + 1}")).ToArray();
            this.RecentProject = Enumerable.Range(0, 5).Select(index => GetInputSpan($"RecentProject.{index + 1}", index == 0 ? All(ModifierKey.Ctrl, ModifierKey.Shift, ModifierKey.Alt, Key.O) : null)).ToArray();

            this.ViewerNone = GetInputSpan("Viewer.None");
            this.ViewerValuesOverlay = GetInputSpan("Viewer.ValuesOverlay");

            this.AlwaysOnTop = GetInputSpan("Options.AlwaysOnTop", All(ModifierKey.Ctrl, ModifierKey.Shift, Key.A));
            this.AutoReload = GetInputSpan("Options.AutoReload");
            this.RestartOnAutoReload = GetInputSpan("Options.RestartOnAutoReload");
            this.ClearStateOnRestart = GetInputSpan("Options.ClearStateOnRestart");
            this.PauseOnInactivity = GetInputSpan("Options.PauseOnInactivity");
            this.RenderInputEventsWhenPaused = GetInputSpan("Options.RenderInputEventsWhenPaused");
            this.WrapShaderInputCursor = GetInputSpan("Options.WrapShaderInputCursor");
            this.EnableShaderCache = GetInputSpan("Options.EnableShaderCache");
            this.DarkTheme = GetInputSpan("Options.DarkTheme");
            this.OpenSettingsFile = GetInputSpan("Options.OpenSettingsFile");
            this.OpenInputsFile = GetInputSpan("Options.OpenInputsFile");
            this.OpenThemeFile = GetInputSpan("Options.OpenThemeFile");
        }

        private IInputSpan GetInputSpan(string name, object defaultInput)
        {
            return GetInputSpan(name, Create(defaultInput));
        }

        private IInputSpan GetInputSpan(string name, IInputSpan? defaultInput = null)
        {
            if (this.settings.TryGet(this.serializer, name, out var inputSpan))
            {
                return inputSpan;
            }

            this.settings.Set(this.serializer, name, defaultInput ?? InputSpan.None);
            return defaultInput ?? InputSpan.None;
        }

        private static AnyInputSpan Any(params object[] values)
        {
            return new AnyInputSpan(values.Select(Create).ToArray());
        }

        private static AllInputSpans All(params object[] values)
        {
            return new AllInputSpans(values.Select(Create).ToArray());
        }

        private static IInputSpan Create(object value)
        {
            return value is IInputSpan inputSpan ? inputSpan :
                value is Key key ? new KeyInputSpan(key) :
                value is ModifierKey modifierKey ? new ModifierKeyInputSpan(modifierKey) :
                value is MouseButton mouseButton ? new MouseButtonInputSpan(mouseButton) :
                value is MouseScroll mouseScroll ? new MouseScrollInputSpan(mouseScroll) :
                value is ModifierKeys ?
                    throw new NotSupportedException($"Unexpected modifier key type {typeof(ModifierKeys).FullName}, modifier key type {typeof(ModifierKey).FullName} should be used instead") :
                    throw new NotSupportedException($"Unexpected input type {value?.GetType().Name}");
        }
    }
}
