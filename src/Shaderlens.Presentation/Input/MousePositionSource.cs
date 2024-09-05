namespace Shaderlens.Presentation.Input
{
    public static class MousePositionSourceExtensions
    {
        public static Point GetPosition(this IMousePositionSource source)
        {
            source.GetPosition(out var x, out var y);
            return new Point(x, y);
        }
    }

    public class TransformedMousePositionSource : IMousePositionSource
    {
        private readonly object locker;
        private readonly IMousePositionSource mousePositionSource;

        private double viewerScale;
        private double viewerOffsetX;
        private double viewerOffsetY;

        private Point viewportPosition;
        private int viewportHeight;

        public TransformedMousePositionSource(IMousePositionSource mousePositionSource)
        {
            this.locker = new object();

            this.viewerScale = 1;
            this.mousePositionSource = mousePositionSource;
        }

        public void GetPosition(out double x, out double y)
        {
            var position = this.mousePositionSource.GetPosition();

            var x1 = position.X;
            var y1 = position.Y;

            lock (this.locker)
            {
                x1 -= this.viewportPosition.X;
                y1 -= this.viewportPosition.Y;

                y1 = this.viewportHeight - y1; // horizontal flip

                x1 -= this.viewerOffsetX;
                y1 -= this.viewerOffsetY;

                var scale = this.viewerScale;
                x1 /= scale;
                y1 /= scale;
            }

            x = x1;
            y = y1;
        }

        public void SetViewerTransform(double scale, double offsetX, double offsetY)
        {
            lock (this.locker)
            {
                this.viewerScale = scale;
                this.viewerOffsetX = offsetX;
                this.viewerOffsetY = offsetY;
            }
        }

        public void SetViewportPosition(Point origin)
        {
            lock (this.locker)
            {
                this.viewportPosition = origin;
            }
        }

        public void SetViewportSize(int viewportHeight)
        {
            lock (this.locker)
            {
                this.viewportHeight = viewportHeight;
            }
        }
    }
}
