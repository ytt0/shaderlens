namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class LinearRgbaColorUniform : Uniform<Vector<double>>
    {
        private class VectorAdapter : ISettingsValue<LinearRgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public LinearRgbColor Value
            {
                get { return new LinearRgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVector(true); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<Vector<double>> settingsValue;

            public VectorAdapter(ISettingsValue<Vector<double>> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public LinearRgbColor GetMergedValue(LinearRgbColor newSettingsValue, LinearRgbColor newDefaultValue)
            {
                return new LinearRgbColor(this.settingsValue.GetMergedValue(newSettingsValue.ToVector(true), newDefaultValue.ToVector(true)));
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

        private readonly string name;
        private readonly string displayName;

        public LinearRgbaColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vector<double>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.name = name;
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<double>> settingsValue)
        {
            uniformElementBuilder.AddColorElement(new VectorAdapter(settingsValue), true, this.name, this.displayName);
        }

        protected override void SetUniformValue(int location, Vector<double> value)
        {
            glUniform4f(location, (float)value[0], (float)value[1], (float)value[2], (float)value[3]);
        }
    }
}
