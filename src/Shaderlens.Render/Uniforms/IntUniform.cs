namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class IntUniform : Uniform<int>
    {
        private readonly string displayName;
        private readonly int minValue;
        private readonly int maxValue;
        private readonly int step;

        public IntUniform(IDispatcherThread renderThread, string name, string displayName, int minValue, int maxValue, int step, ISettingsValue<int> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<int> settingsValue)
        {
            uniformElementBuilder.AddIntElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, int value)
        {
            glUniform1i(location, value);
        }
    }
}
