namespace Shaderlens.Render.Uniforms
{
    using Shaderlens.Serialization.Project;
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

            public LinearRgbColor BaseValue { get; }
            public LinearRgbColor DefaultValue { get; }

            private readonly ISettingsValue<Vector<double>> settingsValue;

            public VectorAdapter(ISettingsValue<Vector<double>> settingsValue)
            {
                this.settingsValue = settingsValue;

                this.BaseValue = new LinearRgbColor(this.settingsValue.BaseValue);
                this.DefaultValue = new LinearRgbColor(this.settingsValue.DefaultValue);
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
