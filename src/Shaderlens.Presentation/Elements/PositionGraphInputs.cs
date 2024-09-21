namespace Shaderlens.Presentation.Elements
{
    public interface IPositionGraphInputs
    {
        IInputSpan Drag { get; }
        IInputSpanEvent DragCancel { get; }
        IInputSpan SmallStepModifier { get; }
        IInputSpan MediumStepModifier { get; }
        IInputSpan LargeStepModifier { get; }

        IInputSpan Pan { get; }

        IInputSpan Scale { get; }
        IInputSpanEvent ScaleUp { get; }
        IInputSpanEvent ScaleDown { get; }
        IInputSpanEvent ScaleReset { get; }

        IInputSpanEvent FocusView { get; }
        IInputSpanEvent ResetView { get; }
        IInputSpanEvent ToggleTargetValue { get; }
        IInputSpanEvent ToggleSourceValue { get; }
    }

    public class PositionGraphInputs : IPositionGraphInputs
    {
        public IInputSpan Drag { get; }
        public IInputSpanEvent DragCancel { get; }
        public IInputSpan SmallStepModifier { get; }
        public IInputSpan MediumStepModifier { get; }
        public IInputSpan LargeStepModifier { get; }

        public IInputSpan Pan { get; }

        public IInputSpan Scale { get; }
        public IInputSpanEvent ScaleUp { get; }
        public IInputSpanEvent ScaleDown { get; }
        public IInputSpanEvent ScaleReset { get; }

        public IInputSpanEvent FocusView { get; }
        public IInputSpanEvent ResetView { get; }
        public IInputSpanEvent ToggleTargetValue { get; }
        public IInputSpanEvent ToggleSourceValue { get; }

        public PositionGraphInputs(IInputSettings settings, string scope)
        {
            var factory = InputSpanFactory.Instance;

            this.Drag = settings.GetOrSetDefault($"{scope}.Drag", factory.Create(MouseButton.Left));
            this.DragCancel = settings.GetOrSetDefault($"{scope}.DragCancel", factory.CreateStart(MouseButton.Right), factory.CreateStart(Key.Escape));
            this.SmallStepModifier = settings.GetOrSetDefault($"{scope}.SmallStepModifier", factory.Create(ModifierKey.Shift));
            this.MediumStepModifier = settings.GetOrSetDefault($"{scope}.MediumStepModifier", factory.Create(ModifierKey.Ctrl));
            this.LargeStepModifier = settings.GetOrSetDefault($"{scope}.LargeStepModifier", factory.All(ModifierKey.Ctrl, ModifierKey.Shift));
            this.Pan = settings.GetOrSetDefault($"{scope}.Pan", factory.Create(MouseButton.Middle));
            this.Scale = settings.GetOrSetDefault($"{scope}.Scale", factory.All(ModifierKey.Ctrl, MouseButton.Middle), factory.All(MouseButton.Right, MouseButton.Middle));
            this.ScaleUp = settings.GetOrSetDefault($"{scope}.ScaleUp", factory.CreateStart(MouseScroll.ScrollUp), factory.AllStart(MouseButton.Right, MouseScroll.ScrollUp), factory.AllStart(ModifierKey.Ctrl, Key.OemPlus), factory.AllStart(ModifierKey.Ctrl, Key.Add));
            this.ScaleDown = settings.GetOrSetDefault($"{scope}.ScaleDown", factory.CreateStart(MouseScroll.ScrollDown), factory.AllStart(MouseButton.Right, MouseScroll.ScrollDown), factory.AllStart(ModifierKey.Ctrl, Key.OemMinus), factory.AllStart(ModifierKey.Ctrl, Key.Subtract));
            this.ScaleReset = settings.GetOrSetDefault($"{scope}.ScaleReset", factory.AllStart(ModifierKey.Ctrl, Key.D0), factory.AllStart(ModifierKey.Ctrl, Key.NumPad0));
            this.ResetView = settings.GetOrSetDefault($"{scope}.ResetView", factory.CreateStart(Key.R));
            this.FocusView = settings.GetOrSetDefault($"{scope}.FocusView", factory.CreateStart(Key.F));
            this.ToggleTargetValue = settings.GetOrSetDefault($"{scope}.ToggleTargetValue", factory.CreateEnd(MouseButton.Right));
            this.ToggleSourceValue = settings.GetOrSetDefault($"{scope}.ToggleSourceValue", factory.AllEnd(ModifierKey.Ctrl, MouseButton.Right));
        }
    }
}
