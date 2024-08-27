namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec2Uniform : Uniform<Vector<double>>
    {
        private readonly string displayName;
        private readonly Vector<double> minValue;
        private readonly Vector<double> maxValue;
        private readonly Vector<double> step;

        public Vec2Uniform(IDispatcherThread renderThread, string name, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, ISettingsValue<Vector<double>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<double>> settingsValue)
        {
            uniformElementBuilder.AddVectorElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vector<double> value)
        {
            glUniform2f(location, (float)value[0], (float)value[1]);
        }
    }
}
