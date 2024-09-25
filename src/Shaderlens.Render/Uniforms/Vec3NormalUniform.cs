﻿namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec3NormalUniform : Uniform<Vector<double>>
    {
        private readonly string displayName;

        public Vec3NormalUniform(IDispatcherThread renderThread, string displayName, ISettingsValue<Vector<double>> settingsValue) :
            base(renderThread, settingsValue)
        {
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<double>> settingsValue)
        {
            uniformElementBuilder.AddNormalElement(settingsValue, this.displayName);
        }

        protected override void SetUniformValue(int location, Vector<double> value)
        {
            glUniform3f(location, (float)value[0], (float)value[1], (float)value[2]);
        }
    }
}