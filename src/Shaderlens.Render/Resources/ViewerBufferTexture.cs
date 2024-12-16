namespace Shaderlens.Render.Resources
{
    public interface IViewerBufferTexture : ITextureResource
    {
        void SetIndex(int bufferIndex, int bufferTextureIndex);
    }

    public class ViewerBufferTexture : IViewerBufferTexture
    {
        public uint Id { get { return this.framebuffers[this.bufferIndex].GetTextureId(this.bufferTextureIndex); } }
        public int Width { get { return this.framebuffers[this.bufferIndex].Width; } }
        public int Height { get { return this.framebuffers[this.bufferIndex].Height; } }

        private readonly IFramebufferResource[] framebuffers;
        private int bufferIndex;
        private int bufferTextureIndex;

        public ViewerBufferTexture(IFramebufferResource[] framebuffers)
        {
            this.framebuffers = framebuffers;
        }

        public void SetIndex(int bufferIndex, int bufferTextureIndex)
        {
            this.bufferIndex = bufferIndex;
            this.bufferTextureIndex = bufferTextureIndex;
        }

        public void Dispose()
        {
        }
    }
}
