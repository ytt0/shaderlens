namespace Shaderlens.Render
{
    using static OpenGL.Gl;

    public interface IRenderProgram
    {
        void Render(IRenderContext context, IFramebufferResource framebuffer);
    }

    public class RenderProgram : IRenderProgram
    {
        private readonly IThreadAccess threadAccess;
        private readonly IRenderResource program;
        private readonly IUniformBinding uniformBinding;
        private readonly IVertexArrayResource vertexArray;
        private readonly string displayName;

        public RenderProgram(IThreadAccess threadAccess, IRenderResource program, IUniformBinding uniformBinding, IVertexArrayResource vertexArray, string displayName)
        {
            threadAccess.Verify();

            this.threadAccess = threadAccess;
            this.program = program;
            this.uniformBinding = uniformBinding;
            this.vertexArray = vertexArray;
            this.displayName = displayName;
        }

        public void Render(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.threadAccess.Verify();

            glPushDebugGroup(GL_DEBUG_SOURCE_APPLICATION, 0, this.displayName);
            glUseProgram(this.program.Id);
            glBindFramebuffer(GL_FRAMEBUFFER, framebuffer.Id);

            this.uniformBinding.BindValue(context, framebuffer);
            this.vertexArray.Draw();

            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            glUseProgram(0);
            glPopDebugGroup();
        }
    }
}
