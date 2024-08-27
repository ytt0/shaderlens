namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class VectorUniformElement : VisualChildContainer, IRowHeaderContainer
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(VectorUniformElement));
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
        private readonly NumberTextBox[] valuesTextBox;
        private readonly ISettingsValue<Vector<double>> settingsValue;
        private bool isValueChanging;

        public VectorUniformElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, double dragSensitivity, IApplicationTheme theme)
        {
            this.settingsValue = settingsValue;

            this.valuesTextBox = settingsValue.Value.Select((value, index) => new NumberTextBox(theme)
            {
                Value = value,
                MinValue = minValue[index],
                MaxValue = maxValue[index],
                StepSize = step[index],
                DragSensitivity = dragSensitivity,
                RequireScrollModifierKey = true,
                HorizontalAlignment = HorizontalAlignment.Stretch
            }.WithHandler(NumberTextBox.ValueChangedEvent, OnValueChanged)).ToArray();

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue
            };

            this.child.ResetValue += (sender, e) =>
            {
                this.isValueChanging = true;
                try
                {
                    settingsValue.ResetValue();

                    for (var i = 0; i < this.valuesTextBox.Length; i++)
                    {
                        this.valuesTextBox[i].Value = this.settingsValue.Value[i];
                    }

                    this.child.IsResetButtonVisible = false;
                    RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                }
                finally
                {
                    this.isValueChanging = false;
                }
            };
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.child.ValueContent = new ColumnPanel().WithChildren(this.valuesTextBox);
        }

        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (this.isValueChanging)
            {
                return;
            }

            this.isValueChanging = true;
            try
            {
                this.settingsValue.Value = Vector.Create(this.valuesTextBox.Select(textBox => textBox.Value).ToArray());
                this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue;
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
            finally
            {
                this.isValueChanging = false;
            }
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }
    }


}
