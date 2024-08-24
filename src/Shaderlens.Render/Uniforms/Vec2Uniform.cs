namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec2Uniform : Uniform<Vec2>
    {
        private readonly string displayName;
        private readonly Vec2 minValue;
        private readonly Vec2 maxValue;
        private readonly Vec2 step;

        public Vec2Uniform(IDispatcherThread renderThread, string name, string displayName, Vec2 minValue, Vec2 maxValue, Vec2 step, ISettingsValue<Vec2> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vec2> settingsValue)
        {
            uniformElementBuilder.AddVec2Element(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vec2 value)
        {
            glUniform2f(location, (float)value.X, (float)value.Y);
        }
    }
}
