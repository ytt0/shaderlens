namespace Shaderlens.Render
{
    using static OpenGL.Gl;

    public interface IRenderProgram
    {
        void Render(IRenderContext context, IFramebufferResource framebuffer);
    }

    public class FragmentRenderProgram : IRenderProgram
    {
        private readonly IThreadAccess threadAccess;
        private readonly IRenderResource program;
        private readonly IUniformBinding uniformBinding;
        private readonly IVertexArrayResource vertexArray;
        private readonly string displayName;

        public FragmentRenderProgram(IThreadAccess threadAccess, IRenderResource program, IUniformBinding uniformBinding, IVertexArrayResource vertexArray, string displayName)
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

    public class ComputeRenderProgram : IRenderProgram
    {
        private readonly IThreadAccess threadAccess;
        private readonly IRenderResource program;
        private readonly IUniformBinding uniformBinding;
        private readonly Vector<int> workGroupSize;
        private readonly bool isRelativeWorkGroups;
        private readonly string displayName;

        private int workGroupsWidth;
        private int workGroupsHeight;
        private uint workGroupsX;
        private uint workGroupsY;
        private uint workGroupsZ;

        public ComputeRenderProgram(IThreadAccess threadAccess, IRenderResource program, IUniformBinding uniformBinding, Vector<int>? workGroupSize, Vector<int>? workGroups, string displayName)
        {
            threadAccess.Verify();

            this.threadAccess = threadAccess;
            this.program = program;
            this.uniformBinding = uniformBinding;
            this.workGroupSize = workGroupSize ?? Vector.Create(3, 1);
            this.displayName = displayName;

            if (workGroups != null)
            {
                this.workGroupsX = (uint)workGroups[0];
                this.workGroupsY = (uint)workGroups[1];
                this.workGroupsZ = (uint)workGroups[2];
            }
            else
            {
                this.isRelativeWorkGroups = true;
            }
        }

        public void Render(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.threadAccess.Verify();

            TrySetRelativeWorkGroups(framebuffer.Width, framebuffer.Height);

            glPushDebugGroup(GL_DEBUG_SOURCE_APPLICATION, 0, this.displayName);

            glUseProgram(this.program.Id);

            this.uniformBinding.BindValue(context, framebuffer);

            for (var i = 0; i < framebuffer.TexturesCount; i++)
            {
                glBindImageTexture((uint)i, framebuffer.GetTextureId(i), 0, false, 0, GL_WRITE_ONLY, GL_RGBA32F);
            }

            glDispatchCompute(this.workGroupsX, this.workGroupsY, this.workGroupsZ);
            glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);

            glUseProgram(0);

            glPopDebugGroup();
        }

        private void TrySetRelativeWorkGroups(int width, int height)
        {
            if (this.isRelativeWorkGroups && (this.workGroupsWidth != width || this.workGroupsHeight != height))
            {
                this.workGroupsWidth = width;
                this.workGroupsHeight = height;
                this.workGroupsX = (uint)((width + this.workGroupSize[0] - 1) / this.workGroupSize[0]);
                this.workGroupsY = (uint)((height + this.workGroupSize[1] - 1) / this.workGroupSize[1]);
                this.workGroupsZ = 1u;
            }
        }
    }
}
