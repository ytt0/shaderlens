namespace Shaderlens.Render.Resources
{
    using static OpenGL.Gl;

    public class ProgramResource : IRenderResource
    {
        private readonly IThreadAccess threadAccess;

        public uint Id { get; }

        public ProgramResource(IThreadAccess threadAccess, uint programId)
        {
            this.threadAccess = threadAccess;
            this.Id = programId;
        }

        public void Dispose()
        {
            this.threadAccess.Verify();
            glDeleteProgram(this.Id);
        }
    }
}
