namespace Shaderlens.Render.Resources
{
    using static MarshalExtension;
    using static OpenGL.Gl;

    public interface ITextureResource : IRenderResource
    {
        int Width { get; }
        int Height { get; }
    }

    public static class TextureResource
    {
        private class DefaultTextureResource : ITextureResource
        {
            public uint Id { get; }
            public int Width { get; }
            public int Height { get; }

            public void Dispose()
            {
            }
        }

        public static readonly ITextureResource Default = new DefaultTextureResource();
    }

    public class BufferTextureResource : ITextureResource
    {
        public uint Id { get; }
        public int Width { get; }
        public int Height { get; }

        private readonly IThreadAccess threadAccess;
        private bool isDisposed;

        public BufferTextureResource(IThreadAccess threadAccess, int width, int height, byte[] buffer, int type)
        {
            this.threadAccess = threadAccess;
            this.threadAccess.Verify();

            var bufferPtr = AllocHGlobal(buffer);

            this.Width = width;
            this.Height = height;

            this.Id = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, this.Id);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);

            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, type, bufferPtr);
            glGenerateMipmap(GL_TEXTURE_2D);
            glBindTexture(GL_TEXTURE_2D, 0);
            Marshal.FreeHGlobal(bufferPtr);
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            if (!this.isDisposed)
            {
                glDeleteTexture(this.Id);
                this.isDisposed = true;
            }
        }
    }
}
