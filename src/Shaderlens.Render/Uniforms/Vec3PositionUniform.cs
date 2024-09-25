﻿namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec3PositionUniform : Uniform<Vector<double>>
    {
        private readonly string displayName;
        private readonly Vector<double> minValue;
        private readonly Vector<double> maxValue;
        private readonly Vector<double> step;

        public Vec3PositionUniform(IDispatcherThread renderThread, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, ISettingsValue<Vector<double>> settingsValue) :
            base(renderThread, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<double>> settingsValue)
        {
            uniformElementBuilder.AddPositionElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vector<double> value)
        {
            glUniform3f(location, (float)value[0], (float)value[1], (float)value[2]);
        }
    }

    public class IVec3PositionUniform : Uniform<Vector<int>>
    {
        private readonly string displayName;
        private readonly Vector<int> minValue;
        private readonly Vector<int> maxValue;
        private readonly Vector<int> step;

        public IVec3PositionUniform(IDispatcherThread renderThread, string displayName, Vector<int> minValue, Vector<int> maxValue, Vector<int> step, ISettingsValue<Vector<int>> settingsValue) :
            base(renderThread, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<int>> settingsValue)
        {
            uniformElementBuilder.AddPositionElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vector<int> value)
        {
            glUniform3i(location, value[0], value[1], value[2]);
        }
    }

    public class UVec3PositionUniform : Uniform<Vector<uint>>
    {
        private readonly string displayName;
        private readonly Vector<uint> minValue;
        private readonly Vector<uint> maxValue;
        private readonly Vector<uint> step;

        public UVec3PositionUniform(IDispatcherThread renderThread, string displayName, Vector<uint> minValue, Vector<uint> maxValue, Vector<uint> step, ISettingsValue<Vector<uint>> settingsValue) :
            base(renderThread, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<uint>> settingsValue)
        {
            uniformElementBuilder.AddPositionElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vector<uint> value)
        {
            glUniform3ui(location, value[0], value[1], value[2]);
        }
    }
}
