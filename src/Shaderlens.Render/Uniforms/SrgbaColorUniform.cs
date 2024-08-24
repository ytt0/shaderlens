namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class SrgbaColorUniform : Uniform<Vec4>
    {
        private class Vec4Adapter : ISettingsValue<SrgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public SrgbColor Value
            {
                get { return new SrgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVec4(); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<Vec4> settingsValue;

            public Vec4Adapter(ISettingsValue<Vec4> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public SrgbColor GetMergedValue(SrgbColor newSettingsValue, SrgbColor newDefaultValue)
            {
                return new SrgbColor(this.settingsValue.GetMergedValue(newSettingsValue.ToVec4(), newDefaultValue.ToVec4()));
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

        public SrgbaColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vec4> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.name = name;
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vec4> settingsValue)
        {
            uniformElementBuilder.AddColorElement(new Vec4Adapter(settingsValue), false, this.name, this.displayName);
        }

        protected override void SetUniformValue(int location, Vec4 value)
        {
            glUniform4f(location, (float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
        }
    }
}
