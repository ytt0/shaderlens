namespace Shaderlens.Render.Project
{
    public interface IProject
    {
        bool IsFullyLoaded { get; }

        IProjectSource Source { get; }

        IProjectSettings? Settings { get; }
        IRenderPipeline? Pipeline { get; }
        IProjectUniforms? Uniforms { get; }
    }

    public class Project : IProject
    {
        public bool IsFullyLoaded { get { return this.Pipeline != null; } }

        public IProjectSource Source { get; }
        public IProjectSettings? Settings { get; }
        public IRenderPipeline? Pipeline { get; }
        public IProjectUniforms? Uniforms { get; }

        public Project(IProjectSource source, IProjectSettings? settings, IRenderPipeline? pipeline, IProjectUniforms? uniforms)
        {
            this.Source = source;
            this.Settings = settings;
            this.Pipeline = pipeline;
            this.Uniforms = uniforms;
        }
    }
}
