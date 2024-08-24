namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec3Uniform : Uniform<Vec3>
    {
        private readonly string displayName;
        private readonly Vec3 minValue;
        private readonly Vec3 maxValue;
        private readonly Vec3 step;

        public Vec3Uniform(IDispatcherThread renderThread, string name, string displayName, Vec3 minValue, Vec3 maxValue, Vec3 step, ISettingsValue<Vec3> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vec3> settingsValue)
        {
            uniformElementBuilder.AddVec3Element(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vec3 value)
        {
            glUniform3f(location, (float)value.X, (float)value.Y, (float)value.Z);
        }
    }
}
