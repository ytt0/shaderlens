namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class SrgbColorUniform : Uniform<Vector<double>>
    {
        private class Vec3Adapter : ISettingsValue<SrgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public SrgbColor Value
            {
                get { return new SrgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVector(false); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<Vector<double>> settingsValue;

            public Vec3Adapter(ISettingsValue<Vector<double>> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public SrgbColor GetMergedValue(SrgbColor newSettingsValue, SrgbColor newDefaultValue)
            {
                return new SrgbColor(this.settingsValue.GetMergedValue(newSettingsValue.ToVector(false), newDefaultValue.ToVector(false)));
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

        public SrgbColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vector<double>> settingsValue) :
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
