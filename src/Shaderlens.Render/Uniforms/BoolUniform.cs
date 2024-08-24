namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class BoolUniform : Uniform<bool>
    {
        private readonly string displayName;

        public BoolUniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<bool> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<bool> settingsValue)
        {
            uniformElementBuilder.AddBoolElement(settingsValue, this.displayName);
        }

        protected override void SetUniformValue(int location, bool value)
        {
            glUniform1i(location, value ? 1 : 0);
        }
    }
}
