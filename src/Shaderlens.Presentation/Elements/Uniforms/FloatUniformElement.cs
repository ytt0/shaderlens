﻿namespace Shaderlens.Presentation.Elements.Uniforms
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

        private readonly NumberTextBox valueTextBox;
        private readonly UniformElement child;
        private readonly ISettingsValue<double> settingsValue;
        private bool skipChangeEvent;

        public FloatUniformElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step, int roundDecimals, double dragSensitivity, IClipboard clipboard, IApplicationTheme theme)
        {
            this.settingsValue = settingsValue;

            this.valueTextBox = new NumberTextBox(theme)
            {
                RawValue = settingsValue.Value,
                MinValue = minValue,
                MaxValue = maxValue,
                Step = step,
                RoundDecimals = roundDecimals,
                DragSensitivity = dragSensitivity,
                ScrollModifier = new ModifierKeyInputSpan(ModifierKey.Alt),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            this.valueTextBox.ValueChanged += (sender, e) =>
            {
                if (!this.skipChangeEvent)
                {
                    this.settingsValue.Value = this.valueTextBox.Value;
                    this.child!.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
                    RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                }
            };

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = this.valueTextBox
            };

            UniformElementResetValueBehavior.Register(this.child, settingsValue, InvalidateValue);
            UniformElementClipboardBehavior.Register(this.child, settingsValue, clipboard, ValueTextSerializer.Double, InvalidateValue);
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        private void InvalidateValue()
        {
            this.skipChangeEvent = true;
            try
            {
                this.valueTextBox.RawValue = this.settingsValue.Value;
                this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }
    }
}
