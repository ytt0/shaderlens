namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public class Vec3Uniform : Uniform<Vector<double>>
    {
        private readonly string displayName;
        private readonly Vector<double> minValue;
        private readonly Vector<double> maxValue;
        private readonly Vector<double> step;

        public Vec3Uniform(IDispatcherThread renderThread, string name, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, ISettingsValue<Vector<double>> settingsValue) :
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
            glUniform3f(location, (float)value[0], (float)value[1], (float)value[2]);
        }
    }

    public class IVec3Uniform : Uniform<Vector<int>>
    {
        private readonly string displayName;
        private readonly Vector<int> minValue;
        private readonly Vector<int> maxValue;
        private readonly Vector<int> step;

        public IVec3Uniform(IDispatcherThread renderThread, string name, string displayName, Vector<int> minValue, Vector<int> maxValue, Vector<int> step, ISettingsValue<Vector<int>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<int>> settingsValue)
        {
            uniformElementBuilder.AddVectorElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vector<int> value)
        {
            glUniform3i(location, value[0], value[1], value[2]);
        }
    }

    public class UVec3Uniform : Uniform<Vector<uint>>
    {
        private readonly string displayName;
        private readonly Vector<uint> minValue;
        private readonly Vector<uint> maxValue;
        private readonly Vector<uint> step;

        public UVec3Uniform(IDispatcherThread renderThread, string name, string displayName, Vector<uint> minValue, Vector<uint> maxValue, Vector<uint> step, ISettingsValue<Vector<uint>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<uint>> settingsValue)
        {
            uniformElementBuilder.AddVectorElement(settingsValue, this.displayName, this.minValue, this.maxValue, this.step);
        }

        protected override void SetUniformValue(int location, Vector<uint> value)
        {
            glUniform3ui(location, value[0], value[1], value[2]);
        }
    }

    public class BVec3Uniform : Uniform<Vector<bool>>
    {
        private readonly string displayName;

        public BVec3Uniform(IDispatcherThread renderThread, string name, string displayName, ISettingsValue<Vector<bool>> settingsValue) :
            base(renderThread, name, settingsValue)
        {
            this.displayName = displayName;
        }

        protected override void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<Vector<bool>> settingsValue)
        {
            uniformElementBuilder.AddVectorElement(settingsValue, this.displayName);
        }

        protected override void SetUniformValue(int location, Vector<bool> value)
        {
            glUniform3i(location, value[0] ? 1 : 0, value[1] ? 1 : 0, value[2] ? 1 : 0);
        }
    }
}
