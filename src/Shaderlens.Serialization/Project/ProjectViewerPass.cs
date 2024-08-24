namespace Shaderlens.Serialization.Project
{
    public interface IProjectViewerPass
    {
        string Key { get; }
        IProjectProgram Program { get; }
    }

    public class ProjectViewerPass : IProjectViewerPass
    {
        public string Key { get; }
        public IProjectProgram Program { get; }

        public ProjectViewerPass(string key, IProjectProgram program)
        {
            this.Key = key;
            this.Program = program;
        }
    }
}
