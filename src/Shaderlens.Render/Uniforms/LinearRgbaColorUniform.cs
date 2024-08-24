namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class LinearRgbaColorUniform : Uniform<Vec4>
    {
        private class Vec4Adapter : ISettingsValue<LinearRgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public LinearRgbColor Value
            {
                get { return new LinearRgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVec4(); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<Vec4> settingsValue;

            public Vec4Adapter(ISettingsValue<Vec4> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public LinearRgbColor GetMergedValue(LinearRgbColor newSettingsValue, LinearRgbColor newDefaultValue)
            {
                return new LinearRgbColor(this.settingsValue.GetMergedValue(newSettingsValue.ToVec4(), newDefaultValue.ToVec4()));
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

        public LinearRgbaColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vec4> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.name = name;
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vec4> settingsValue)
        {
            uniformElementBuilder.AddColorElement(new Vec4Adapter(settingsValue), true, this.name, this.displayName);
        }

        protected override void SetUniformValue(int location, Vec4 value)
        {
            glUniform4f(location, (float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
        }
    }
}
