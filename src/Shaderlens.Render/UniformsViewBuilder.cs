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

            public double DefaultValue { get { return this.settingsValue.DefaultValue; } }
            public double BaseValue { get { return this.settingsValue.BaseValue; } }

            private readonly ISettingsValue<int> settingsValue;

            public IntAdapter(ISettingsValue<int> settingsValue)
            {
                this.settingsValue = settingsValue;
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

            public SrgbColor DefaultValue { get; }
            public SrgbColor BaseValue { get; }

            private readonly ISettingsValue<LinearRgbColor> settingsValue;

            public SrgbColorAdapter(ISettingsValue<LinearRgbColor> settingsValue)
            {
                this.settingsValue = settingsValue;
                this.DefaultValue = this.settingsValue.DefaultValue.ToSrgb();
                this.BaseValue = this.settingsValue.BaseValue.ToSrgb();
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
