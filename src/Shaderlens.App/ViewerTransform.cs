namespace Shaderlens
{
    public interface IViewerTransform
    {
        double Scale { get; }
        Point Offset { get; }

        void SetSize(int bufferWidth, int bufferHeight, int viewportWidth, int viewportHeight);
        void SetScale(double scale);
        void SetOffset(Point offset);
    }

    public class ViewerTransform : IViewerTransform
    {
        public double Scale { get; private set; }
        public Point Offset { get; private set; }

        private double viewerScale;
        private Point viewerOffset;
        private int bufferWidth;
        private int bufferHeight;
        private int viewportWidth;
        private int viewportHeight;

        public ViewerTransform()
        {
            this.viewerScale = 1;
            this.Scale = this.viewerScale;
            this.Offset = this.viewerOffset;
        }

        public void SetSize(int bufferWidth, int bufferHeight, int viewportWidth, int viewportHeight)
        {
            var scale = GetRelativeScale();

            this.bufferWidth = bufferWidth;
            this.bufferHeight = bufferHeight;
            this.viewportWidth = viewportWidth;
            this.viewportHeight = viewportHeight;

            SetRelativeScale(scale);

            SetTransform();
        }

        public void SetScale(double scale)
        {
            this.viewerScale = scale;
            SetTransform();
        }

        public void SetOffset(Point offset)
        {
            this.viewerOffset = offset;
            SetTransform();
        }

        private double GetRelativeScale()
        {
            return this.viewportWidth != 0 && this.viewportHeight != 0 && this.bufferWidth != 0 && this.bufferHeight != 0 ?
                this.viewerScale * Math.Max((double)this.bufferWidth / this.viewportWidth, (double)this.bufferHeight / this.viewportHeight) : 0;
        }

        private void SetRelativeScale(double relativeScale)
        {
            if (relativeScale != 0)
            {
                this.viewerScale = relativeScale * Math.Min((double)this.viewportWidth / this.bufferWidth, (double)this.viewportHeight / this.bufferHeight);
            }
        }

        private void SetTransform()
        {
            if (this.bufferWidth == 0 || this.bufferHeight == 0)
            {
                return;
            }

            var minScale = Math.Max(1.0, Math.Min((double)this.viewportWidth / this.bufferWidth, (double)this.viewportHeight / this.bufferHeight));

            this.viewerScale = Math.Clamp(this.viewerScale, minScale, 10000.0);

            var extentWidth = this.viewerScale * this.bufferWidth - this.viewportWidth;
            var extentHeight = this.viewerScale * this.bufferHeight - this.viewportHeight;

            this.viewerOffset.X = extentWidth < 0 ? -extentWidth / 2.0 : Math.Clamp(this.viewerOffset.X, Math.Min(0.0, -extentWidth), 0.0);
            this.viewerOffset.Y = extentHeight < 0 ? -extentHeight / 2.0 : Math.Clamp(this.viewerOffset.Y, Math.Min(0.0, -extentHeight), 0.0);

            this.Scale = this.viewerScale;
            this.Offset = this.viewerOffset;
        }
    }
}
