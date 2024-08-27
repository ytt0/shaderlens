namespace Shaderlens.Render
{
    public interface IUniformsViewBuilder
    {
        IDisposable AddGroup(ISettingsValue<bool> expandedSettingsValue, string displayName);
        void AddBoolElement(ISettingsValue<bool> settingsValue, string displayName);
        void AddIntElement(ISettingsValue<int> settingsValue, string displayName, int minValue, int maxValue, int step);
        void AddFloatElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step);
        void AddVectorElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step);
        void AddColorElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, string name, string displayName);
        void SetSettingsState();
    }

    public static class UniformsViewBuilderExtensions
    {
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

        public static void AddColorElement(this IUniformsViewBuilder builder, ISettingsValue<LinearRgbColor> settingsValue, bool editAlpha, string name, string displayName)
        {
            builder.AddColorElement(new SrgbColorAdapter(settingsValue), editAlpha, name, displayName);
        }
    }
}
