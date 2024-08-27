namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class FloatUniformElement : VisualChildContainer, IRowHeaderContainer
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FloatUniformElement));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public double HeaderWidth
        {
            get { return this.child.HeaderWidth; }
            set { this.child.HeaderWidth = value; }
        }

        private readonly UniformElement child;

        public FloatUniformElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step, double dragSensitivity, IApplicationTheme theme)
        {
            var valueTextBox = new NumberTextBox(theme)
            {
                Value = settingsValue.Value,
                MinValue = minValue,
                MaxValue = maxValue,
                StepSize = step,
                DragSensitivity = dragSensitivity,
                RequireScrollModifierKey = true,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = valueTextBox
            };

            this.child.ResetValue += (sender, e) =>
            {
                settingsValue.ResetValue();
                valueTextBox.Value = settingsValue.Value;
                this.child.IsResetButtonVisible = false;
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            };

            valueTextBox.ValueChanged += (sender, e) =>
            {
                settingsValue.Value = valueTextBox.Value;
                this.child.IsResetButtonVisible = !settingsValue.IsDefaultValue();
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            };
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }
    }
}
