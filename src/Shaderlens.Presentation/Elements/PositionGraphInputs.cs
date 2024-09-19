namespace Shaderlens.Presentation.Elements
{
    public interface IPositionGraphInputs
    {
        IInputSpan Drag { get; }
        IInputSpan DragCancel { get; }
        IInputSpan SmallStepModifier { get; }
        IInputSpan MediumStepModifier { get; }
        IInputSpan LargeStepModifier { get; }

        IInputSpan Pan { get; }

        IInputSpan Scale { get; }
        IInputSpan ScaleUp { get; }
        IInputSpan ScaleDown { get; }
        IInputSpan ScaleReset { get; }

        IInputSpan FocusView { get; }
        IInputSpan ResetView { get; }
        IInputSpan ToggleTargetValue { get; }
        IInputSpan ToggleSourceValue { get; }
    }

    public class PositionGraphInputs : IPositionGraphInputs
    {
        public IInputSpan Drag { get; }
        public IInputSpan DragCancel { get; }
        public IInputSpan SmallStepModifier { get; }
        public IInputSpan MediumStepModifier { get; }
        public IInputSpan LargeStepModifier { get; }

        public IInputSpan Pan { get; }

        public IInputSpan Scale { get; }
        public IInputSpan ScaleUp { get; }
        public IInputSpan ScaleDown { get; }
        public IInputSpan ScaleReset { get; }

        public IInputSpan FocusView { get; }
        public IInputSpan ResetView { get; }
        public IInputSpan ToggleTargetValue { get; }
        public IInputSpan ToggleSourceValue { get; }

        public PositionGraphInputs(IInputSettings settings, string scope)
        {
            var factory = InputSpanFactory.Instance;

            this.Drag = settings.GetOrSetDefault($"{scope}.Drag", factory.Create(MouseButton.Left));
            this.DragCancel = settings.GetOrSetDefault($"{scope}.DragCancel", factory.Create(MouseButton.Right), factory.Create(Key.Escape));
            this.SmallStepModifier = settings.GetOrSetDefault($"{scope}.SmallStepModifier", factory.Create(ModifierKey.Shift));
            this.MediumStepModifier = settings.GetOrSetDefault($"{scope}.MediumStepModifier", factory.Create(ModifierKey.Ctrl));
            this.LargeStepModifier = settings.GetOrSetDefault($"{scope}.LargeStepModifier", factory.All(ModifierKey.Ctrl, ModifierKey.Shift));
            this.Pan = settings.GetOrSetDefault($"{scope}.Pan", factory.Create(MouseButton.Middle));
            this.Scale = settings.GetOrSetDefault($"{scope}.Scale", factory.All(ModifierKey.Ctrl, MouseButton.Middle), factory.All(MouseButton.Right, MouseButton.Middle));
            this.ScaleUp = settings.GetOrSetDefault($"{scope}.ScaleUp", factory.Create(MouseScroll.ScrollUp), factory.All(MouseButton.Right, MouseScroll.ScrollUp), factory.All(ModifierKey.Ctrl, Key.OemPlus), factory.All(ModifierKey.Ctrl, Key.Add));
            this.ScaleDown = settings.GetOrSetDefault($"{scope}.ScaleDown", factory.Create(MouseScroll.ScrollDown), factory.All(MouseButton.Right, MouseScroll.ScrollDown), factory.All(ModifierKey.Ctrl, Key.OemMinus), factory.All(ModifierKey.Ctrl, Key.Subtract));
            this.ScaleReset = settings.GetOrSetDefault($"{scope}.ScaleReset", factory.All(ModifierKey.Ctrl, Key.D0), factory.All(ModifierKey.Ctrl, Key.NumPad0));
            this.ResetView = settings.GetOrSetDefault($"{scope}.ResetView", factory.Create(Key.R));
            this.FocusView = settings.GetOrSetDefault($"{scope}.FocusView", factory.Create(Key.F));
            this.ToggleTargetValue = settings.GetOrSetDefault($"{scope}.ToggleTargetValue", factory.Create(MouseButton.Right));
            this.ToggleSourceValue = settings.GetOrSetDefault($"{scope}.ToggleSourceValue", factory.All(ModifierKey.Ctrl, MouseButton.Right));
        }
    }
}
