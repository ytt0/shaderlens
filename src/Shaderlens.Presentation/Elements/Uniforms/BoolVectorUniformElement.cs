namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class BoolVectorUniformElement : VisualChildContainer, IRowHeaderContainer
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(BoolVectorUniformElement));
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
        private readonly StyledCheckBox[] valuesCheckBox;
        private readonly ISettingsValue<Vector<bool>> settingsValue;

        public BoolVectorUniformElement(ISettingsValue<Vector<bool>> settingsValue, string displayName, IClipboard clipboard, IApplicationTheme theme)
        {
            this.settingsValue = settingsValue;

            this.valuesCheckBox = settingsValue.Value.Select((value, index) => new StyledCheckBox(theme)
            {
                IsChecked = value,
                HorizontalAlignment = HorizontalAlignment.Stretch
            }.WithHandler(ButtonBase.ClickEvent, OnClick)).ToArray();

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = new ColumnPanel().WithChildren(this.valuesCheckBox)
            };

            UniformElementResetValueBehavior.Register(this.child, settingsValue, InvalidateValue);
            UniformElementClipboardBehavior.Register(this.child, settingsValue, clipboard, FixedSizeVectorTextSerializer.Create(VectorTextSerializer.Create(ValueTextSerializer.Bool), this.settingsValue.Value.Count), InvalidateValue);
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            this.settingsValue.Value = Vector.Create(this.valuesCheckBox.Select(checkBox => checkBox.IsChecked == true).ToArray());
            this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        private void InvalidateValue()
        {
            for (var i = 0; i < this.valuesCheckBox.Length; i++)
            {
                this.valuesCheckBox[i].IsChecked = this.settingsValue.Value[i];
            }

            this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }
    }
}
