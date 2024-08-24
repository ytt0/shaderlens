namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class FloatUniform : Uniform<double>
    {
        private readonly string displayName;
        private readonly double minValue;
        private readonly double maxValue;
        private readonly double step;

        public FloatUniform(IDispatcherThread renderThread, string name, string displayName, double minValue, double maxValue, double step, ISettingsValue<double> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<double> settingsValue)
        {
            uniformElementBuilder.AddFloatElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, double value)
        {
            glUniform1f(location, (float)value);
        }
    }
}
