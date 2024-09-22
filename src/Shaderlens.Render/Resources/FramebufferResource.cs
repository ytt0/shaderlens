namespace Shaderlens.Render.Resources
{
    using static OpenGL.Gl;

    public interface IFramebufferResource : IRenderResource
    {
        uint TextureId { get; }
        int Width { get; }
        int Height { get; }

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

        public uint TextureId { get { return this.texture.Id; } }
        public int Width { get { return this.texture.Width; } }
        public int Height { get { return this.texture.Height; } }

        private readonly IThreadAccess threadAccess;
        private readonly Queue<ITextureResource> states;
        private ITextureResource texture;
        private bool isDisposed;

        public FramebufferResource(IThreadAccess threadAccess)
        {
            this.threadAccess = threadAccess;
            this.threadAccess.Verify();

            this.states = new Queue<ITextureResource>();

            this.Id = glGenFramebuffer();
            this.texture = TextureResource.Default;
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            if (!this.isDisposed)
            {
                this.texture.Dispose();
                glDeleteFramebuffer(this.Id);
                this.isDisposed = true;
            }
        }

        public void SetSize(int width, int height)
        {
            this.threadAccess.Verify();

            if (this.texture.Width == width && this.texture.Height == height)
            {
                return;
            }

            this.texture.Dispose();
            this.texture = new FramebufferTexture(this.threadAccess, width, height);

            BindTexture();
        }

        public void PushState()
        {
            this.threadAccess.Verify();

            this.states.Enqueue(this.texture);
            this.texture = new FramebufferTexture(this.threadAccess, this.texture.Width, this.texture.Height);

            BindTexture();
        }

        public void PopState()
        {
            this.threadAccess.Verify();

            this.texture.Dispose();
            this.texture = this.states.Dequeue();

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
            glBindTexture(GL_TEXTURE_2D, this.texture.Id);
            glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, this.texture.Id, 0);
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
        public uint TextureId { get { return this.framebuffers[this.readIndex].TextureId; } }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly IThreadAccess threadAccess;
        private readonly IFramebufferResource[] framebuffers;
        private readonly int framebuffersCount;
        private int writeIndex;
        private int readIndex;

        public ReadWriteFramebufferResource(IThreadAccess threadAccess, int framebuffersCount)
        {
            if (framebuffersCount < 2)
            {
                throw new ArgumentException("Count should be at least 2", nameof(framebuffersCount));
            }

            this.threadAccess = threadAccess;
            this.framebuffers = Enumerable.Range(0, framebuffersCount).Select(index => new FramebufferResource(threadAccess)).ToArray();
            this.framebuffersCount = framebuffersCount;

            this.readIndex = 0;
            this.writeIndex = 1;
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < this.framebuffersCount; i++)
            {
                this.framebuffers[i].Dispose();
            }
        }

        public void SwapBuffers()
        {
            this.threadAccess.Verify();

            this.readIndex = this.writeIndex;
            this.writeIndex = (this.writeIndex + 1) % this.framebuffersCount;
        }

        public void SetSize(int width, int height)
        {
            this.threadAccess.Verify();

            this.Width = width;
            this.Height = height;

            for (var i = 0; i < this.framebuffersCount; i++)
            {
                this.framebuffers[i].SetSize(width, height);
            }
        }

        public void PushState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < this.framebuffersCount; i++)
            {
                this.framebuffers[i].PushState();
            }
        }

        public void PopState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < this.framebuffersCount; i++)
            {
                this.framebuffers[i].PopState();
            }
        }

        public void ClearState()
        {
            this.threadAccess.Verify();

            for (var i = 0; i < this.framebuffersCount; i++)
            {
                this.framebuffers[i].ClearState();
            }
        }
    }
}
