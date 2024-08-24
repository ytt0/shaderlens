namespace Shaderlens.Render.Resources
{
    using static MarshalExtension;
    using static OpenGL.Gl;

    public interface IKeyboardTextureResource : IAnimatedTextureResource
    {
        void PushState();
        void PopState();
        void ClearState();
        void ClearKeysDownState();
        void SetKeyState(int index, bool isDown);
    }

    public class KeyboardTextureResource : IKeyboardTextureResource
    {
        private class EmptyKeyboardTextureResource : IKeyboardTextureResource
        {
            public long Time { get; }
            public long Duration { get; }
            public int Width { get; }
            public int Height { get; }
            public uint Id { get; }

            public void Dispose()
            {
            }

            public void SetFrameContent(FrameIndex frameIndex)
            {
            }

            public void PushState()
            {
            }

            public void PopState()
            {
            }

            public void ClearState()
            {
            }

            public void ClearKeysDownState()
            {
            }

            public void SetKeyState(int index, bool isDown)
            {
            }
        }

        public static readonly IKeyboardTextureResource Empty = new EmptyKeyboardTextureResource();

        public uint Id { get; }
        public int Width { get; }
        public int Height { get; }
        public long Time { get; private set; }
        public long Duration { get { return 0; } }

        private readonly IThreadAccess threadAccess;
        private readonly object locker;
        private readonly byte[] buffer;
        private readonly Queue<byte[]> states;
        private bool isChanged;
        private bool isDisposed;

        public KeyboardTextureResource(IThreadAccess threadAccess)
        {
            this.threadAccess = threadAccess;
            this.threadAccess.Verify();

            this.Id = glGenTexture();
            this.Width = 256;
            this.Height = 3;

            this.locker = new object();
            this.buffer = new byte[256 * 3];
            this.states = new Queue<byte[]>();

            SetTextureBuffer(this.Id, this.buffer);
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

        public void PushState()
        {
            lock (this.locker)
            {
                var state = new byte[this.buffer.Length];
                Array.Copy(this.buffer, state, this.buffer.Length);
                this.states.Enqueue(state);
            }
        }

        public void PopState()
        {
            lock (this.locker)
            {
                var state = this.states.Dequeue();
                Array.Copy(state, this.buffer, this.buffer.Length);
                this.isChanged = true;
            }
        }

        public void ClearState()
        {
            lock (this.locker)
            {
                Array.Clear(this.buffer);
                this.isChanged = true;
            }
        }

        public void ClearKeysDownState()
        {
            lock (this.locker)
            {
                Array.Clear(this.buffer, 0, 512);
                this.isChanged = true;
            }
        }

        public void SetKeyState(int index, bool isDown)
        {
            lock (this.locker)
            {
                var bufferIsDown = this.buffer[index] == 255;
                if (bufferIsDown == isDown)
                {
                    return;
                }

                if (isDown)
                {
                    this.buffer[index] = 255; // is down
                    this.buffer[index + 256] = 255; // is pressed (cleared after one frame)
                    this.buffer[index + 512] = (byte)(255 - this.buffer[index + 512]); // toggled
                }
                else
                {
                    this.buffer[index] = 0; // is down
                }

                this.isChanged = true;
            }
        }

        public void SetFrameContent(FrameIndex frameIndex)
        {
            this.threadAccess.Verify();

            lock (this.locker)
            {
                if (this.isChanged)
                {
                    SetTextureBuffer(this.Id, this.buffer);
                    this.Time = frameIndex.Time;
                    this.isChanged = false;

                    // clear is down
                    for (var i = 0; i < 256; i++)
                    {
                        this.isChanged |= this.buffer[i + 256] != 0;
                        this.buffer[i + 256] = 0;
                    }
                }
            }
        }

        private static void SetTextureBuffer(uint id, byte[] buffer)
        {
            var bufferPtr = AllocHGlobal(buffer);
            glBindTexture(GL_TEXTURE_2D, id);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RED, 256, 3, 0, GL_RED, GL_UNSIGNED_BYTE, bufferPtr);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            glBindTexture(GL_TEXTURE_2D, 0);
            Marshal.FreeHGlobal(bufferPtr);
        }
    }
}
