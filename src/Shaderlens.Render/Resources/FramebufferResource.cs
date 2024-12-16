namespace Shaderlens.Render.Resources
{
    using static OpenGL.Gl;

    public interface IFramebufferResource : IRenderResource
    {
        int TexturesCount { get; }

        int Width { get; }
        int Height { get; }

        uint GetTextureId(int index);
        void SetSize(int width, int height);
        void PushState();
        void PopState();
        void ClearState();
    }

    public interface IReadWriteFramebufferResource : IFramebufferResource
    {
        void SwapBuffers();
    }

    public class FramebufferResource : IFramebufferResource
    {
        private class FramebufferTexture : ITextureResource
        {
            public uint Id { get; }
            public int Width { get; }
            public int Height { get; }

            private readonly IThreadAccess threadAccess;
            private bool isDisposed;

            public FramebufferTexture(IThreadAccess threadAccess, int width, int height)
            {
                this.threadAccess = threadAccess;
                this.threadAccess.Verify();

                this.Width = width;
                this.Height = height;
                this.Id = glGenTexture();

                glBindTexture(GL_TEXTURE_2D, this.Id);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA32F, width, height, 0, GL_RGBA, GL_FLOAT, 0);
                glGenerateMipmap(GL_TEXTURE_2D);
                glBindTexture(GL_TEXTURE_2D, 0);
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

        public uint Id { get; }

        public int TexturesCount { get; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly IThreadAccess threadAccess;
        private readonly Queue<ITextureResource[]> states;
        private ITextureResource[] textures;
        private bool isDisposed;

        public FramebufferResource(IThreadAccess threadAccess, int texturesCount)
        {
            this.threadAccess = threadAccess;
            this.threadAccess.Verify();

            this.states = new Queue<ITextureResource[]>();

            this.Id = glGenFramebuffer();
            this.TexturesCount = texturesCount;

            this.textures = new ITextureResource[this.TexturesCount];
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            if (!this.isDisposed)
            {
                foreach (var texture in this.textures)
                {
                    texture?.Dispose();
                }

                glDeleteFramebuffer(this.Id);
                this.isDisposed = true;
            }
        }

        public uint GetTextureId(int index)
        {
            return this.textures[index].Id;
        }

        public void SetSize(int width, int height)
        {
            this.threadAccess.Verify();

            if (this.Width == width && this.Height == height)
            {
                return;
            }

            this.Width = width;
            this.Height = height;

            for (var i = 0; i < this.textures.Length; i++)
            {
                this.textures[i]?.Dispose();
                this.textures[i] = new FramebufferTexture(this.threadAccess, width, height);
            }

            BindTexture();
        }

        public void PushState()
        {
            this.threadAccess.Verify();

            this.states.Enqueue(this.textures);

            this.textures = new ITextureResource[this.TexturesCount];

            for (var i = 0; i < this.TexturesCount; i++)
            {
                this.textures[i] = new FramebufferTexture(this.threadAccess, this.Width, this.Height);
            }

            BindTexture();
        }

        public void PopState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < this.TexturesCount; i++)
            {
                this.textures[i].Dispose();
            }

            this.textures = this.states.Dequeue();

            BindTexture();
        }

        public void ClearState()
        {
            this.threadAccess.Verify();

            glBindFramebuffer(GL_FRAMEBUFFER, this.Id);
            glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
        }

        private void BindTexture()
        {
            glBindFramebuffer(GL_FRAMEBUFFER, this.Id);

            for (var i = 0; i < this.TexturesCount; i++)
            {
                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0 + i, GL_TEXTURE_2D, this.textures[i].Id, 0);
            }

            glDrawBuffers(Enumerable.Range(0, this.TexturesCount).Select(i => GL_COLOR_ATTACHMENT0 + i).ToArray());

            var status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
            glBindFramebuffer(GL_FRAMEBUFFER, 0);

            if (status != GL_FRAMEBUFFER_COMPLETE)
            {
                throw new Exception("Framebuffer is not complete");
            }
        }
    }

    public class ReadWriteFramebufferResource : IReadWriteFramebufferResource
    {
        public uint Id { get { return this.framebuffers[this.writeIndex].Id; } }

        public int TexturesCount { get; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly IThreadAccess threadAccess;
        private readonly IFramebufferResource[] framebuffers;
        private int writeIndex;
        private int readIndex;

        public ReadWriteFramebufferResource(IThreadAccess threadAccess, int texturesCount)
        {
            this.threadAccess = threadAccess;
            this.TexturesCount = texturesCount;
            this.framebuffers = new[] { new FramebufferResource(threadAccess, texturesCount), new FramebufferResource(threadAccess, texturesCount) };

            this.readIndex = 0;
            this.writeIndex = 1;
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < 2; i++)
            {
                this.framebuffers[i].Dispose();
            }
        }

        public void SwapBuffers()
        {
            this.threadAccess.Verify();

            this.readIndex = this.writeIndex;
            this.writeIndex = (this.writeIndex + 1) % 2;
        }

        public uint GetTextureId(int index)
        {
            return this.framebuffers[this.readIndex].GetTextureId(index);
        }

        public void SetSize(int width, int height)
        {
            this.threadAccess.Verify();

            this.Width = width;
            this.Height = height;

            for (var i = 0; i < 2; i++)
            {
                this.framebuffers[i].SetSize(width, height);
            }
        }

        public void PushState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < 2; i++)
            {
                this.framebuffers[i].PushState();
            }
        }

        public void PopState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < 2; i++)
            {
                this.framebuffers[i].PopState();
            }
        }

        public void ClearState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < 2; i++)
            {
                this.framebuffers[i].ClearState();
            }
        }
    }
}
