namespace Shaderlens.Render.Uniforms
{
    public class UniformsGroup : IUniform
    {
        private readonly string displayName;
        private readonly ISettingsValue<bool> expandedSettingsValue;
        private readonly IEnumerable<IUniform> uniforms;

        public UniformsGroup(string displayName, ISettingsValue<bool> expandedSettingsValue, IEnumerable<IUniform> uniforms)
        {
            this.displayName = displayName;
            this.expandedSettingsValue = expandedSettingsValue;
            this.uniforms = uniforms;
        }

        public IUniformBinding CreateBinding(uint programId)
        {
            return new UniformBindingCollection(this.uniforms.Select(uniform => uniform.CreateBinding(programId)).ToArray());
        }

        public void AddViewElement(IUniformsViewBuilder uniformElementBuilder)
        {
            using (uniformElementBuilder.AddGroup(this.expandedSettingsValue, this.displayName))
            {
                foreach (var uniform in this.uniforms)
                {
                    uniform.AddViewElement(uniformElementBuilder);
                }
            }
        }
    }

    public class RootUniformsGroup : IUniform
    {
        private readonly IEnumerable<IUniform> uniforms;

        public RootUniformsGroup(IEnumerable<IUniform> uniforms)
        {
            this.uniforms = uniforms;
        }

        public IUniformBinding CreateBinding(uint programId)
        {
            return new UniformBindingCollection(this.uniforms.Select(uniform => uniform.CreateBinding(programId)).ToArray());
        }

        public void AddViewElement(IUniformsViewBuilder uniformElementBuilder)
        {
            foreach (var uniform in this.uniforms)
            {
                uniform.AddViewElement(uniformElementBuilder);
            }
        }
    }
}
