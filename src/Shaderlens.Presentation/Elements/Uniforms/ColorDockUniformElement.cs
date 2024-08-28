namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class ColorDockUniformElement : VisualChildContainer
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ColorDockUniformElement));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private readonly ColorEditor colorEditor;
        private bool skipChangeEvent;
        private ISettingsValue<SrgbColor>? settingsValue;

        public ColorDockUniformElement(IApplicationTheme theme, double dragSensitivity)
        {
            this.colorEditor = new ColorEditor(theme) { DragSensitivity = dragSensitivity };

            this.colorEditor.ColorChanged += (sender, e) => OnValueChanged();
            this.colorEditor.AlphaChanged += (sender, e) => OnValueChanged();
        }

        protected override FrameworkElement GetChild()
        {
            return this.colorEditor;
        }

        public void SetValue(ISettingsValue<SrgbColor> settingsValue, bool editAlpha)
        {
            this.skipChangeEvent = true;
            try
            {
                this.settingsValue = settingsValue;

                var hsv = OkhsvColor.FromLinearRgb(settingsValue.Value.ToLinearRgb());
                var alpha = settingsValue.Value.A;

                this.colorEditor.Color = hsv;
                this.colorEditor.SourceColor = hsv;
                this.colorEditor.Alpha = alpha;
                this.colorEditor.SourceAlpha = alpha;
                this.colorEditor.IsAlphaVisible = editAlpha;
                this.colorEditor.ResetLastColors();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        public void InvalidateValue()
        {
            if (this.settingsValue != null)
            {
                this.skipChangeEvent = true;
                try
                {
                    this.colorEditor.Color = OkhsvColor.FromLinearRgb(this.settingsValue.Value.ToLinearRgb());
                }
                finally
                {
                    this.skipChangeEvent = false;
                }
            }
        }

        private void OnValueChanged()
        {
            if (!this.skipChangeEvent && this.settingsValue != null)
            {
                var color = this.colorEditor.Color.ToLinearRgb().Round(0.001).ToSrgb();
                color.A = this.colorEditor.Alpha;

                this.settingsValue.Value = color;
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }
    }
}
