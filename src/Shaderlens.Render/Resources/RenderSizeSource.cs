namespace Shaderlens.Render.Resources
{
    public interface IRenderSizeSource
    {
        void GetSize(int viewportWidth, int viewportHeight, int renderDownscale, out int width, out int height);
    }

    public class FixedRenderSizeSource : IRenderSizeSource
    {
        private readonly int width;
        private readonly int height;

        public FixedRenderSizeSource(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void GetSize(int viewportWidth, int viewportHeight, int renderDownscale, out int width, out int height)
        {
            width = this.width;
            height = this.height;
        }
    }

    public class ScalableRenderSizeSource : IRenderSizeSource
    {
        private readonly int width;
        private readonly int height;

        public ScalableRenderSizeSource(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void GetSize(int viewportWidth, int viewportHeight, int renderDownscale, out int width, out int height)
        {
            width = Math.Max(1, this.width / renderDownscale);
            height = Math.Max(1, this.height / renderDownscale);
        }
    }

    public class ViewportRenderSizeSource : IRenderSizeSource
    {
        public static readonly IRenderSizeSource Instance = new ViewportRenderSizeSource();

        private ViewportRenderSizeSource()
        {
        }

        public void GetSize(int viewportWidth, int viewportHeight, int renderDownscale, out int width, out int height)
        {
            width = Math.Max(1, viewportWidth / renderDownscale);
            height = Math.Max(1, viewportHeight / renderDownscale);
        }
    }
}
