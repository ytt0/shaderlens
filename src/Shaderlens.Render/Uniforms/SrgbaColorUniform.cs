namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class SrgbaColorUniform : Uniform<Vector<double>>
    {
        private class Vec4Adapter : ISettingsValue<SrgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public SrgbColor Value
            {
                get { return new SrgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVector(true); }
            }

            public SrgbColor BaseValue { get; }
            public SrgbColor DefaultValue { get; }

            private readonly ISettingsValue<Vector<double>> settingsValue;

            public Vec4Adapter(ISettingsValue<Vector<double>> settingsValue)
            {
                this.settingsValue = settingsValue;

                this.BaseValue = new SrgbColor(this.settingsValue.BaseValue);
                this.DefaultValue = new SrgbColor(this.settingsValue.DefaultValue);
            }
        }

        private readonly string name;
        private readonly string displayName;

        public SrgbaColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vector<double>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.name = name;
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<double>> settingsValue)
        {
            uniformElementBuilder.AddColorElement(new Vec4Adapter(settingsValue), false, this.name, this.displayName);
        }

        protected override void SetUniformValue(int location, Vector<double> value)
        {
            glUniform4f(location, (float)value[0], (float)value[1], (float)value[2], (float)value[3]);
        }
    }
}
