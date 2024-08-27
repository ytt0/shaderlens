namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public interface IUniform
    {
        void AddViewElement(IUniformsViewBuilder uniformElementBuilder);
        IUniformBinding CreateBinding(uint programId);
    }

    public abstract class Uniform<T> : IUniform
    {
        private class UniformBinding : IUniformBinding
        {
            private readonly Uniform<T> uniform;
            private readonly int location;

            public UniformBinding(Uniform<T> uniform, int location)
            {
                this.uniform = uniform;
                this.location = location;
            }

            public void BindValue(IRenderContext renderContext, IFramebufferResource framebuffer)
            {
                this.uniform.renderThread.VerifyAccess();
                this.uniform.SetUniformValue(this.location, this.uniform.value);
            }
        }

        private class UniformSettingsValue : ISettingsValue<T>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public T Value
            {
                get { return this.settingsValue.Value; }
                set
                {
                    this.settingsValue.Value = value;
                    this.uniform.renderThread.DispatchAsync(() => this.uniform.value = value);
                }
            }

            public T DefaultValue { get { return this.settingsValue.DefaultValue; } }
            public T BaseValue { get { return this.settingsValue.BaseValue; } }

            private readonly Uniform<T> uniform;
            private readonly ISettingsValue<T> settingsValue;

            public UniformSettingsValue(Uniform<T> uniform, ISettingsValue<T> settingsValue)
            {
                this.uniform = uniform;
                this.settingsValue = settingsValue;
            }
        }

        private readonly IDispatcherThread renderThread;
        private readonly string name;
        private readonly ISettingsValue<T> settingsValue;
        private T value;

        public Uniform(IDispatcherThread renderThread, string name, ISettingsValue<T> settingsValue)
        {
            this.renderThread = renderThread;
            this.name = name;
            this.settingsValue = settingsValue;
            this.value = settingsValue.Value;
        }

        public void AddViewElement(IUniformsViewBuilder uniformElementBuilder)
        {
            AddViewElement(uniformElementBuilder, new UniformSettingsValue(this, this.settingsValue));
        }

        public IUniformBinding CreateBinding(uint programId)
        {
            var location = glGetUniformLocation(programId, this.name);
            return location != -1 ? new UniformBinding(this, location) : Uniforms.UniformBinding.Empty;
        }

        protected abstract void AddViewElement(IUniformsViewBuilder uniformElementBuilder, ISettingsValue<T> settingsValue);

        protected abstract void SetUniformValue(int location, T value);
    }

    public static class Uniform
    {
        private class EmptyUniform : IUniform
        {
            public void AddViewElement(IUniformsViewBuilder uniformElementBuilder)
            {
            }

            public IUniformBinding CreateBinding(uint programId)
            {
                return UniformBinding.Empty;
            }
        }

        public static readonly IUniform Empty = new EmptyUniform();
    }
}
