namespace Shaderlens.Render.Resources
{
    public interface IViewerBufferTexture : ITextureResource
    {
        void SetViewerBufferIndex(int index);
    }

    public class ViewerBufferTexture : IViewerBufferTexture
    {
        public uint Id { get { return this.framebuffers[this.index].TextureId; } }
        public int Width { get { return this.framebuffers[this.index].Width; } }
        public int Height { get { return this.framebuffers[this.index].Height; } }

        private readonly IFramebufferResource[] framebuffers;
        private int index;

        public ViewerBufferTexture(IFramebufferResource[] framebuffers)
        {
            this.framebuffers = framebuffers;
        }

        public void SetViewerBufferIndex(int index)
        {
            this.index = index;
        }

        public void Dispose()
        {
        }
    }
}
