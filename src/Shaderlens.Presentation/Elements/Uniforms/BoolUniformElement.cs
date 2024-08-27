namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class BoolUniformElement : VisualChildContainer, IRowHeaderContainer
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(BoolUniformElement));
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

        public BoolUniformElement(ISettingsValue<bool> settingsValue, string displayName, IApplicationTheme theme)
        {
            var valueCheckBox = new StyledCheckBox(theme) { IsChecked = settingsValue.Value, HorizontalAlignment = HorizontalAlignment.Stretch };

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = valueCheckBox
            };

            this.child.ResetValue += (sender, e) =>
            {
                settingsValue.ResetValue();
                valueCheckBox.IsChecked = settingsValue.Value;
                this.child.IsResetButtonVisible = false;
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            };

            valueCheckBox.Click += (sender, e) =>
            {
                settingsValue.Value = valueCheckBox.IsChecked == true;
                this.child.IsResetButtonVisible = !settingsValue.DefaultValue;
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            };
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }
    }
}
