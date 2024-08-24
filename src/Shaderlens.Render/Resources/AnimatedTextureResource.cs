namespace Shaderlens.Render.Resources
{
    public interface IAnimatedTextureResource : ITextureResource
    {
        long Time { get; }
        long Duration { get; }

        void SetFrameContent(FrameIndex frameIndex);
    }

    public interface IAnimationFrameTextureResource : IDisposable
    {
        ITextureResource TextureResource { get; }
        long EndTime { get; }
    }

    public class AnimatedTextureResource : IAnimatedTextureResource
    {
        public uint Id { get { return this.frames[this.frameIndex].TextureResource.Id; } }

        public int Width { get; }
        public int Height { get; }
        public long Time { get; private set; }
        public long Duration { get; }

        private readonly IAnimationFrameTextureResource[] frames;

        private int frameIndex;
        private long lastFrameIndexTime;

        public AnimatedTextureResource(IEnumerable<IAnimationFrameTextureResource> frames)
        {
            this.frames = frames.ToArray();
            this.Duration = frames.Last().EndTime;
            this.Width = frames.First().TextureResource.Width;
            this.Height = frames.First().TextureResource.Height;
        }

        public void Dispose()
        {
            foreach (var frame in this.frames)
            {
                frame.Dispose();
            }
        }

        public void SetFrameContent(FrameIndex frameIndex)
        {
            if (this.lastFrameIndexTime == frameIndex.Time)
            {
                return;
            }

            this.lastFrameIndexTime = frameIndex.Time;

            this.Time = frameIndex.Time % this.Duration;

            for (var i = 0; i < this.frames.Length; i++)
            {
                var frameStartTime = this.frameIndex == 0 ? 0 : this.frames[this.frameIndex - 1].EndTime;
                var frameEndTime = this.frames[this.frameIndex].EndTime;

                if (frameStartTime <= this.Time && this.Time < frameEndTime)
                {
                    return;
                }

                this.frameIndex = (this.frameIndex + 1) % this.frames.Length;
            }
        }
    }

    public class AnimationFrameTextureResource : IAnimationFrameTextureResource
    {
        public ITextureResource TextureResource { get; }
        public long EndTime { get; }

        public AnimationFrameTextureResource(ITextureResource textureResource, long endTime)
        {
            this.TextureResource = textureResource;
            this.EndTime = endTime;
        }

        public void Dispose()
        {
            this.TextureResource.Dispose();
        }
    }
}
