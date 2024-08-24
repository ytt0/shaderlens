namespace Shaderlens.Render.Resources
{
    using static OpenGL.Gl;

    public class ShaderResource : IRenderResource
    {
        private readonly IThreadAccess threadAccess;

        public uint Id { get; }

        public ShaderResource(IThreadAccess threadAccess, uint shaderId)
        {
            this.threadAccess = threadAccess;
            this.Id = shaderId;
        }

        public void Dispose()
        {
            this.threadAccess.Verify();
            glDeleteShader(this.Id);
        }
    }
}
