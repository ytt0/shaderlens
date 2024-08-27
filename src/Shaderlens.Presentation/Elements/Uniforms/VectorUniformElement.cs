﻿namespace Shaderlens.Presentation.Elements.Uniforms
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

        public VectorUniformElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, double dragSensitivity, IClipboard clipboard, IApplicationTheme theme)
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
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = new ColumnPanel().WithChildren(this.valuesTextBox)
            };

            UniformElementResetValueBehavior.Register(this.child, settingsValue, InvalidateValue);
            UniformElementClipboardBehavior.Register(this.child, settingsValue, clipboard, FixedSizeVectorTextSerializer.Create(VectorTextSerializer.Create(ValueTextSerializer.Double), this.settingsValue.Value.Count), InvalidateValue);
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (!this.isValueChanging)
            {
                this.settingsValue.Value = Vector.Create(this.valuesTextBox.Select(textBox => textBox.Value).ToArray());
                this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        private void InvalidateValue()
        {
            this.isValueChanging = true;
            try
            {
                for (var i = 0; i < this.valuesTextBox.Length; i++)
                {
                    this.valuesTextBox[i].Value = this.settingsValue.Value[i];
                }

                this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
            finally
            {
                this.isValueChanging = false;
            }
        }
    }
}