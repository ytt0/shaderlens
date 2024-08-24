namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class SrgbColorUniform : Uniform<Vec3>
    {
        private class Vec3Adapter : ISettingsValue<SrgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public SrgbColor Value
            {
                get { return new SrgbColor(this.settingsValue.Value); }
                set { this.settingsValue.Value = value.ToVec3(); }
            }

            public bool IsDefaultValue { get { return this.settingsValue.IsDefaultValue; } }

            private readonly ISettingsValue<Vec3> settingsValue;

            public Vec3Adapter(ISettingsValue<Vec3> settingsValue)
            {
                this.settingsValue = settingsValue;
            }

            public SrgbColor GetMergedValue(SrgbColor newSettingsValue, SrgbColor newDefaultValue)
            {
                return new SrgbColor(this.settingsValue.GetMergedValue(newSettingsValue.ToVec3(), newDefaultValue.ToVec3()));
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

        public SrgbColorUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vec3> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.name = name;
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vec3> settingsValue)
        {
            uniformElementBuilder.AddColorElement(new Vec3Adapter(settingsValue), false, this.name, this.displayName);
        }

        protected override void SetUniformValue(int location, Vec3 value)
        {
            glUniform3f(location, (float)value.X, (float)value.Y, (float)value.Z);
        }
    }
}
