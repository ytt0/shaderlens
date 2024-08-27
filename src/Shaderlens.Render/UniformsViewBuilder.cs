namespace Shaderlens.Render
{
    public interface IUniformsViewBuilder
    {
        IDisposable AddGroup(ISettingsValue<bool> expandedSettingsValue, string displayName);
        void AddBoolElement(ISettingsValue<bool> settingsValue, string displayName);
        void AddFloatElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step);
        void AddVectorElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step);
        void AddColorElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, string name, string displayName);
        void SetSettingsState();
    }

    public static class UniformsViewBuilderExtensions
    {
        private class IntAdapter : ISettingsValue<double>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public double Value
            {
                get { return this.settingsValue.Value; }
                set { this.settingsValue.Value = (int)Math.Round(value); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<int> settingsValue;

            public IntAdapter(ISettingsValue<int> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public double GetMergedValue(double newSettingsValue, double newDefaultValue)
            {
                return this.settingsValue.GetMergedValue((int)Math.Round(newSettingsValue), (int)Math.Round(newDefaultValue));
            }

            public void ResetValue()
            {
                this.settingsValue.ResetValue();
            }

            public void SaveValue()
            {
                this.settingsValue.SaveValue();
            }
        }

        private class SrgbColorAdapter : ISettingsValue<SrgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public SrgbColor Value
            {
                get { return this.settingsValue.Value.ToSrgb(); }
                set { this.settingsValue.Value = value.ToLinearRgb(); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<LinearRgbColor> settingsValue;

            public SrgbColorAdapter(ISettingsValue<LinearRgbColor> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public SrgbColor GetMergedValue(SrgbColor newSettingsValue, SrgbColor newDefaultValue)
            {
                return this.settingsValue.GetMergedValue(newSettingsValue.ToLinearRgb(), newDefaultValue.ToLinearRgb()).ToSrgb();
            }

            public void ResetValue()
            {
                this.settingsValue.ResetValue();
            }

            public void SaveValue()
            {
                this.settingsValue.SaveValue();
            }
        }

        public static void AddIntElement(this IUniformsViewBuilder builder, ISettingsValue<int> settingsValue, string displayName, int minValue, int maxValue, int step)
        {
            builder.AddFloatElement(new IntAdapter(settingsValue), displayName, minValue, maxValue, step);
        }

        public static void AddColorElement(this IUniformsViewBuilder builder, ISettingsValue<LinearRgbColor> settingsValue, bool editAlpha, string name, string displayName)
        {
            builder.AddColorElement(new SrgbColorAdapter(settingsValue), editAlpha, name, displayName);
        }
    }
}
