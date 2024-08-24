namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec4Uniform : Uniform<Vec4>
    {
        private readonly string displayName;
        private readonly Vec4 minValue;
        private readonly Vec4 maxValue;
        private readonly Vec4 step;

        public Vec4Uniform(IDispatcherThread renderThread, string name, string displayName, Vec4 minValue, Vec4 maxValue, Vec4 step, ISettingsValue<Vec4> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vec4> settingsValue)
        {
            uniformElementBuilder.AddVec4Element(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vec4 value)
        {
            glUniform4f(location, (float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
        }
    }
}
