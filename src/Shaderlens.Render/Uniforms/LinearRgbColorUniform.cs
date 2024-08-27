namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class LinearRgbColorUniform : Uniform<Vector<double>>
    {
        private class Vec3Adapter : ISettingsValue<LinearRgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public LinearRgbColor Value
            {
                get { return new LinearRgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVector(false); }
            }

            public LinearRgbColor BaseValue { get; }
            public LinearRgbColor DefaultValue { get; }

            private readonly ISettingsValue<Vector<double>> settingsValue;

            public Vec3Adapter(ISettingsValue<Vector<double>> settingsValue)
            {
                this.settingsValue = settingsValue;

                this.BaseValue = new LinearRgbColor(this.settingsValue.BaseValue);
                this.DefaultValue = new LinearRgbColor(this.settingsValue.DefaultValue);
            }
        }

        private readonly string name;
        private readonly string displayName;

        public LinearRgbColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vector<double>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.name = name;
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<double>> settingsValue)
        {
            uniformElementBuilder.AddColorElement(new Vec3Adapter(settingsValue), false, this.name, this.displayName);
        }

        protected override void SetUniformValue(int location, Vector<double> value)
        {
            glUniform3f(location, (float)value[0], (float)value[1], (float)value[2]);
        }
    }
}
