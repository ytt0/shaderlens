namespace Shaderlens
{
    public interface ICommand
    {
        void AddBindings(IInputStateBindings bindings);
        void AddMenuItem(IMenuBuilder builder, object header, object? icon = null, object? tooltip = null, Action<IMenuItemState>? setState = null);
    }

    public interface IApplicationCommands
    {
        ICommand Play { get; }
        ICommand Pause { get; }
        ICommand Step { get; }
        ICommand Restart { get; }
        ICommand Uniforms { get; }
        ICommand ProjectNew { get; }
        ICommand ProjectOpen { get; }
        ICommand ProjectReload { get; }
        ICommand ProjectSave { get; }
        ICommand Help { get; }

        ICommand ProjectOpenFolder { get; }

        ICommand FrameRateFull { get; }
        ICommand FrameRate2 { get; }
        ICommand FrameRate4 { get; }
        ICommand FrameRate8 { get; }
        ICommand FrameRate16 { get; }
        ICommand FrameRateIncrease { get; }
        ICommand FrameRateDecrease { get; }

        ICommand ResolutionFull { get; }
        ICommand Resolution2 { get; }
        ICommand Resolution4 { get; }
        ICommand Resolution8 { get; }
        ICommand Resolution16 { get; }
        ICommand Resolution32 { get; }
        ICommand Resolution64 { get; }
        ICommand ResolutionIncrease { get; }
        ICommand ResolutionDecrease { get; }

        ICommand Speed1_16 { get; }
        ICommand Speed1_8 { get; }
        ICommand Speed1_4 { get; }
        ICommand Speed1_2 { get; }
        ICommand SpeedNormal { get; }
        ICommand Speed2 { get; }
        ICommand Speed4 { get; }
        ICommand Speed8 { get; }
        ICommand Speed16 { get; }
        ICommand SpeedIncrease { get; }
        ICommand SpeedDecrease { get; }

        IEnumerable<ICommand> Buffer { get; }
        ICommand BufferImage { get; }
        ICommand BufferNext { get; }
        ICommand BufferPrevious { get; }

        ICommand ExportFrame { get; }
        ICommand ExportFrameRepeat { get; }
        ICommand ExportRenderSequence { get; }

        IEnumerable<ICommand> PinnedProject { get; }
        IEnumerable<ICommand> RecentProject { get; }

        ICommand ViewerNone { get; }
        ICommand ViewerValuesOverlay { get; }

        ICommand AlwaysOnTop { get; }
        ICommand AutoReload { get; }
        ICommand RestartOnAutoReload { get; }
        ICommand ClearStateOnRestart { get; }
        ICommand PauseOnInactivity { get; }
        ICommand RenderInputEventsWhenPaused { get; }
        ICommand WrapShaderInputCursor { get; }
        ICommand EnableShaderCache { get; }
        ICommand DarkTheme { get; }
        ICommand OpenSettingsFile { get; }
        ICommand OpenInputsFile { get; }
        ICommand OpenThemeFile { get; }

        void AddBindings(IInputStateBindings bindings);
    }

    public class ApplicationCommands : IApplicationCommands
    {
        private class Command : ICommand
        {
            private readonly Action action;
            private readonly Func<bool>? isVisible;
            private readonly Func<bool>? isChecked;
            private readonly IInputSpan? inputSpan;
            private readonly bool isStartAction;

            public Command(IInputSpan? inputSpan, Action action, Func<bool>? isChecked, Func<bool>? isVisible, bool isStartAction = true)
            {
                this.action = action;
                this.isVisible = isVisible;
                this.isChecked = isChecked;
                this.inputSpan = inputSpan;
                this.isStartAction = isStartAction;
            }

            public void Invoke()
            {
                if (this.isVisible?.Invoke() ?? true)
                {
                    this.action();
                }
            }

            public void AddBindings(IInputStateBindings bindings)
            {
                bindings.AddSpan(this.inputSpan, StartAction, EndAction);
            }

            public void AddMenuItem(IMenuBuilder builder, object header, object? icon = null, object? tooltip = null, Action<IMenuItemState>? setState = null)
            {
                builder.AddItem(header, this.inputSpan, icon, tooltip, this.action, state =>
                {
                    state.IsVisible = this.isVisible?.Invoke() ?? true;
                    state.IsChecked = this.isChecked?.Invoke() ?? false;
                    setState?.Invoke(state);
                });
            }

            private void StartAction(InputSpanEventArgs e)
            {
                if (this.isVisible?.Invoke() ?? true)
                {
                    if (this.isStartAction)
                    {
                        this.action();
                    }

                    e.Handled = true;
                }
            }

            private void EndAction(InputSpanEventArgs e)
            {
                if (this.isVisible?.Invoke() ?? true)
                {
                    if (!this.isStartAction)
                    {
                        this.action();
                    }

                    e.Handled = true;
                }
            }
        }

        public ICommand Play { get; }
        public ICommand Pause { get; }
        public ICommand Step { get; }
        public ICommand Restart { get; }
        public ICommand Uniforms { get; }
        public ICommand StartPage { get; }
        public ICommand ProjectNew { get; }
        public ICommand ProjectOpen { get; }
        public ICommand ProjectReload { get; }
        public ICommand ProjectSave { get; }
        public ICommand Help { get; }

        public ICommand ProjectOpenFolder { get; }

        public ICommand FrameRateFull { get; }
        public ICommand FrameRate2 { get; }
        public ICommand FrameRate4 { get; }
        public ICommand FrameRate8 { get; }
        public ICommand FrameRate16 { get; }
        public ICommand FrameRateIncrease { get; }
        public ICommand FrameRateDecrease { get; }

        public ICommand ResolutionFull { get; }
        public ICommand Resolution2 { get; }
        public ICommand Resolution4 { get; }
        public ICommand Resolution8 { get; }
        public ICommand Resolution16 { get; }
        public ICommand Resolution32 { get; }
        public ICommand Resolution64 { get; }
        public ICommand ResolutionIncrease { get; }
        public ICommand ResolutionDecrease { get; }

        public ICommand Speed1_16 { get; }
        public ICommand Speed1_8 { get; }
        public ICommand Speed1_4 { get; }
        public ICommand Speed1_2 { get; }
        public ICommand SpeedNormal { get; }
        public ICommand Speed2 { get; }
        public ICommand Speed4 { get; }
        public ICommand Speed8 { get; }
        public ICommand Speed16 { get; }
        public ICommand SpeedIncrease { get; }
        public ICommand SpeedDecrease { get; }

        public IEnumerable<ICommand> Buffer { get; }
        public ICommand BufferImage { get; }
        public ICommand BufferNext { get; }
        public ICommand BufferPrevious { get; }

        public ICommand ExportFrame { get; }
        public ICommand ExportFrameRepeat { get; }
        public ICommand ExportRenderSequence { get; }

        public IEnumerable<ICommand> PinnedProject { get; }
        public IEnumerable<ICommand> RecentProject { get; }

        public ICommand ViewerNone { get; }
        public ICommand ViewerValuesOverlay { get; }

        public ICommand AlwaysOnTop { get; }
        public ICommand AutoReload { get; }
        public ICommand RestartOnAutoReload { get; }
        public ICommand ClearStateOnRestart { get; }
        public ICommand PauseOnInactivity { get; }
        public ICommand RenderInputEventsWhenPaused { get; }
        public ICommand WrapShaderInputCursor { get; }
        public ICommand EnableShaderCache { get; }
        public ICommand DarkTheme { get; }
        public ICommand OpenSettingsFile { get; }
        public ICommand OpenInputsFile { get; }
        public ICommand OpenThemeFile { get; }

        private readonly List<ICommand> commands;
        private readonly IApplication application;

        public ApplicationCommands(IApplication application, IApplicationInputs inputs)
        {
            this.application = application;
            var commands = new List<ICommand>();
            this.commands = commands;

            this.Play = Add(commands, new Command(inputs.Play, application.ResumePipeline, null, () => application.IsFullyLoaded && application.IsPaused));
            this.Pause = Add(commands, new Command(inputs.Pause, application.PausePipeline, null, () => application.IsFullyLoaded && !application.IsPaused));
            this.Step = Add(commands, new Command(inputs.Step, application.StepPipeline, null, () => application.IsFullyLoaded && application.IsPaused));
            this.Restart = Add(commands, new Command(inputs.Restart, application.RestartPipeline, null, IsFullyLoaded));
            this.Uniforms = Add(commands, new Command(inputs.Uniforms, application.ToggleUniformsView, null, IsPartiallyLoaded, false));
            this.StartPage = Add(commands, new Command(inputs.StartPage, application.ShowStartPage, null, null));
            this.ProjectNew = Add(commands, new Command(inputs.ProjectNew, application.CreateProject, null, null));
            this.ProjectOpen = Add(commands, new Command(inputs.ProjectOpen, application.OpenProject, null, null));
            this.ProjectReload = Add(commands, new Command(inputs.ProjectReload, application.ReloadProject, null, IsPartiallyLoaded));
            this.ProjectSave = Add(commands, new Command(inputs.ProjectSave, application.SaveProject, null, IsFullyLoaded));
            this.Help = Add(commands, new Command(inputs.Help, application.Help, null, null));

            this.ProjectOpenFolder = Add(commands, new Command(inputs.ProjectOpenFolder, () => application.OpenExternalPath(Path.GetDirectoryName(application.ProjectPath)!), null, IsPartiallyLoaded));

            this.FrameRateFull = Add(commands, new Command(inputs.FrameRateFull, () => application.FrameRate = 1, () => application.FrameRate == 1, null));
            this.FrameRate2 = Add(commands, new Command(inputs.FrameRate2, () => application.FrameRate = 2, () => application.FrameRate == 2, null));
            this.FrameRate4 = Add(commands, new Command(inputs.FrameRate4, () => application.FrameRate = 4, () => application.FrameRate == 4, null));
            this.FrameRate8 = Add(commands, new Command(inputs.FrameRate8, () => application.FrameRate = 8, () => application.FrameRate == 8, null));
            this.FrameRate16 = Add(commands, new Command(inputs.FrameRate16, () => application.FrameRate = 16, () => application.FrameRate == 16, null));
            this.FrameRateIncrease = Add(commands, new Command(inputs.FrameRateIncrease, () => application.FrameRate = Math.Max(1, application.FrameRate / 2), null, null));
            this.FrameRateDecrease = Add(commands, new Command(inputs.FrameRateDecrease, () => application.FrameRate = Math.Min(128, application.FrameRate * 2), null, null));

            this.ResolutionFull = Add(commands, new Command(inputs.ResolutionFull, () => application.RenderDownscale = 1, () => application.RenderDownscale == 1, IsFullyLoaded));
            this.Resolution2 = Add(commands, new Command(inputs.Resolution2, () => application.RenderDownscale = 2, () => application.RenderDownscale == 2, IsFullyLoaded));
            this.Resolution4 = Add(commands, new Command(inputs.Resolution4, () => application.RenderDownscale = 4, () => application.RenderDownscale == 4, IsFullyLoaded));
            this.Resolution8 = Add(commands, new Command(inputs.Resolution8, () => application.RenderDownscale = 8, () => application.RenderDownscale == 8, IsFullyLoaded));
            this.Resolution16 = Add(commands, new Command(inputs.Resolution16, () => application.RenderDownscale = 16, () => application.RenderDownscale == 16, IsFullyLoaded));
            this.Resolution32 = Add(commands, new Command(inputs.Resolution32, () => application.RenderDownscale = 32, () => application.RenderDownscale == 32, IsFullyLoaded));
            this.Resolution64 = Add(commands, new Command(inputs.Resolution64, () => application.RenderDownscale = 64, () => application.RenderDownscale == 64, IsFullyLoaded));
            this.ResolutionIncrease = Add(commands, new Command(inputs.ResolutionIncrease, () => application.RenderDownscale = Math.Max(1, application.RenderDownscale / 2), null, IsFullyLoaded));
            this.ResolutionDecrease = Add(commands, new Command(inputs.ResolutionDecrease, () => application.RenderDownscale = Math.Min(1024, application.RenderDownscale * 2), null, IsFullyLoaded));

            this.Speed1_16 = Add(commands, new Command(inputs.Speed1_16, () => application.Speed = 0.0625, () => Math.Abs(application.Speed - 0.0625) < 0.001, IsFullyLoaded));
            this.Speed1_8 = Add(commands, new Command(inputs.Speed1_8, () => application.Speed = 0.125, () => Math.Abs(application.Speed - 0.125) < 0.001, IsFullyLoaded));
            this.Speed1_4 = Add(commands, new Command(inputs.Speed1_4, () => application.Speed = 0.25, () => Math.Abs(application.Speed - 0.25) < 0.001, IsFullyLoaded));
            this.Speed1_2 = Add(commands, new Command(inputs.Speed1_2, () => application.Speed = 0.5, () => Math.Abs(application.Speed - 0.5) < 0.001, IsFullyLoaded));
            this.SpeedNormal = Add(commands, new Command(inputs.SpeedNormal, () => application.Speed = 1, () => Math.Abs(application.Speed - 1) < 0.001, IsFullyLoaded));
            this.Speed2 = Add(commands, new Command(inputs.Speed2, () => application.Speed = 2, () => Math.Abs(application.Speed - 2) < 0.001, IsFullyLoaded));
            this.Speed4 = Add(commands, new Command(inputs.Speed4, () => application.Speed = 4, () => Math.Abs(application.Speed - 4) < 0.001, IsFullyLoaded));
            this.Speed8 = Add(commands, new Command(inputs.Speed8, () => application.Speed = 8, () => Math.Abs(application.Speed - 8) < 0.001, IsFullyLoaded));
            this.Speed16 = Add(commands, new Command(inputs.Speed16, () => application.Speed = 16, () => Math.Abs(application.Speed - 16) < 0.001, IsFullyLoaded));
            this.SpeedIncrease = Add(commands, new Command(inputs.SpeedIncrease, () => application.Speed = Math.Min(1024, application.Speed * 2), null, IsFullyLoaded));
            this.SpeedDecrease = Add(commands, new Command(inputs.SpeedDecrease, () => application.Speed = Math.Max(1.0 / 1024, application.Speed / 2), null, IsFullyLoaded));

            this.Buffer = Add(commands, inputs.Buffer.Select((inputSpan, index) => new Command(inputSpan, () => application.ViewerBufferIndex = index, () => application.ViewerBufferIndex == index, IsFullyLoaded)).ToArray());
            this.BufferImage = Add(commands, new Command(inputs.BufferImage, () => application.ViewerBufferIndex = application.ViewerBuffersCount - 1, null, null));
            this.BufferNext = Add(commands, new Command(inputs.BufferNext, () => application.ViewerBufferIndex++, null, IsFullyLoaded));
            this.BufferPrevious = Add(commands, new Command(inputs.BufferPrevious, () => application.ViewerBufferIndex--, null, IsFullyLoaded));

            this.ExportFrame = Add(commands, new Command(inputs.ExportFrame, application.ExportFrame, null, IsFullyLoaded));
            this.ExportFrameRepeat = Add(commands, new Command(inputs.ExportFrameRepeat, application.ExportFrameRepeat, null, () => application.IsFullyLoaded && application.SaveFramePath != null));
            this.ExportRenderSequence = Add(commands, new Command(inputs.ExportRenderSequence, application.ExportSequence, null, IsFullyLoaded));

            this.PinnedProject = Add(commands, inputs.PinnedProject.Select((inputSpan, index) => new Command(inputSpan, () => TryOpenProject(application.PinnedProjects.ElementAtOrDefault(index)), null, null)).ToArray());
            this.RecentProject = Add(commands, inputs.RecentProject.Select((inputSpan, index) => new Command(inputSpan, () => TryOpenProject(application.RecentProjects.ElementAtOrDefault(index)), null, null)).ToArray());

            this.ViewerNone = Add(commands, new Command(inputs.ViewerNone, () => application.ViewerPass = ViewerPassSelection.None, () => Equals(application.ViewerPass, ViewerPassSelection.None), null));
            this.ViewerValuesOverlay = Add(commands, new Command(inputs.ViewerValuesOverlay, () => application.ViewerPass = ViewerPassSelection.ValuesOverlay, () => Equals(application.ViewerPass, ViewerPassSelection.ValuesOverlay), null));

            this.AlwaysOnTop = Add(commands, new Command(inputs.AlwaysOnTop, () => application.AlwaysOnTop = !application.AlwaysOnTop, () => application.AlwaysOnTop, null));
            this.AutoReload = Add(commands, new Command(inputs.AutoReload, () => application.AutoReload = !application.AutoReload, () => application.AutoReload, null));
            this.RestartOnAutoReload = Add(commands, new Command(inputs.RestartOnAutoReload, () => application.RestartOnAutoReload = !application.RestartOnAutoReload, () => application.RestartOnAutoReload, null));
            this.ClearStateOnRestart = Add(commands, new Command(inputs.ClearStateOnRestart, () => application.ClearStateOnRestart = !application.ClearStateOnRestart, () => application.ClearStateOnRestart, null));
            this.PauseOnInactivity = Add(commands, new Command(inputs.PauseOnInactivity, () => application.PauseOnInactivity = !application.PauseOnInactivity, () => application.PauseOnInactivity, null));
            this.RenderInputEventsWhenPaused = Add(commands, new Command(inputs.RenderInputEventsWhenPaused, () => application.RenderInputEventsWhenPaused = !application.RenderInputEventsWhenPaused, () => application.RenderInputEventsWhenPaused, null));
            this.WrapShaderInputCursor = Add(commands, new Command(inputs.WrapShaderInputCursor, () => application.WrapShaderInputCursor = !application.WrapShaderInputCursor, () => application.WrapShaderInputCursor, null));
            this.EnableShaderCache = Add(commands, new Command(inputs.EnableShaderCache, () => application.EnableShaderCache = !application.EnableShaderCache, () => application.EnableShaderCache, null));
            this.DarkTheme = Add(commands, new Command(inputs.DarkTheme, () => application.DarkTheme = !application.DarkTheme, () => application.DarkTheme, null));
            this.OpenSettingsFile = Add(commands, new Command(inputs.OpenSettingsFile, () => application.OpenExternalPath(application.SettingsPath), null, null));
            this.OpenInputsFile = Add(commands, new Command(inputs.OpenInputsFile, () => application.OpenExternalPath(application.InputsPath), null, null));
            this.OpenThemeFile = Add(commands, new Command(inputs.OpenThemeFile, () => application.OpenExternalPath(application.ThemePath!), null, () => application.ThemePath != null));
        }

        public void AddBindings(IInputStateBindings bindings)
        {
            foreach (var command in this.commands)
            {
                command.AddBindings(bindings);
            }
        }

        private bool IsPartiallyLoaded()
        {
            return this.application.IsPartiallyLoaded;
        }

        private bool IsFullyLoaded()
        {
            return this.application.IsFullyLoaded;
        }

        private void TryOpenProject(string? path)
        {
            if (path != null)
            {
                this.application.OpenProject(path);
            }
        }

        private static Command Add(List<ICommand> target, Command command)
        {
            target.Add(command);
            return command;
        }

        private static IEnumerable<Command> Add(List<ICommand> target, IEnumerable<Command> commands)
        {
            target.AddRange(commands);
            return commands;
        }
    }
}
