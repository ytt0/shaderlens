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
        private readonly ISettingsValue<bool> settingsValue;
        private readonly StyledCheckBox valueCheckBox;

        public BoolUniformElement(ISettingsValue<bool> settingsValue, string displayName, IClipboard clipboard, IApplicationTheme theme)
        {
            this.settingsValue = settingsValue;

            this.valueCheckBox = new StyledCheckBox(theme)
            {
                IsChecked = settingsValue.Value,
                ClickMode = ClickMode.Press,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            this.valueCheckBox.Click += (sender, e) =>
            {
                this.settingsValue.Value = this.valueCheckBox.IsChecked == true;
                this.child!.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                e.Handled = true;
            };

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = this.valueCheckBox
            };

            UniformElementResetValueBehavior.Register(this.child, settingsValue, InvalidateValue);
            UniformElementClipboardBehavior.Register(this.child, settingsValue, clipboard, ValueTextSerializer.Bool, InvalidateValue);
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        private void InvalidateValue()
        {
            this.valueCheckBox.IsChecked = this.settingsValue.Value;
            this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }
    }
}
