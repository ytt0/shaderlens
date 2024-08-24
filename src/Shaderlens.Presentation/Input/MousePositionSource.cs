namespace Shaderlens.Presentation.Input
{
    using static WinApi;

    public static class MousePositionSourceExtensions
    {
        public static Point GetPosition(this IMousePositionSource source)
        {
            source.GetPosition(out var x, out var y);
            return new Point(x, y);
        }
    }

    public class AbsoluteMousePositionSource : IMousePositionSource
    {
        private const int InitialBoundsMargin = -1; // wrap the cursor when it moves outside the element bounds + this margin
        private const int DragBoundsMargin = -20; // wrap the cursor when it moves outside the element bounds + this margin
        private const int SetDragBoundsThreshold = 100; // move from InitialBoundsMargin to DragBoundsMargin when the cursor has moved over this threadhold
        private const int JumpMargin = 20; // jump from one side of the bounds to the other + this margin (for example, from left to right - 20, or from right to left + 20), this is in addtion to the bounds margin
        private readonly object locker;

        private bool enableCursorWrap;
        private FrameworkElement boundsElement;
        private Rect elementBounds;
        private POINT wrapOffset;
        private Rect wrapBounds;
        private POINT initialCursorPosition;
        private bool isInitialBounds;
        private bool isCaptured;
        private POINT lastCursorPosition;

        public AbsoluteMousePositionSource(FrameworkElement boundsElement)
        {
            this.locker = new object();
            this.boundsElement = boundsElement;
        }

        public void StartCapture(bool enableCursorWrap)
        {
            lock (this.locker)
            {
                if (this.isCaptured)
                {
                    throw new Exception("Mouse capture has already started");
                }

                this.enableCursorWrap = enableCursorWrap;

                Mouse.PrimaryDevice.Capture(this.boundsElement);

                if (this.boundsElement is Window)
                {
                    this.boundsElement = this.boundsElement.GetVisualDescendants(2).OfType<FrameworkElement>().Skip(1).FirstOrDefault() ?? throw new Exception("Failed to get wrap bounds element");
                }

                this.elementBounds = new Rect(this.boundsElement.PointToScreen(new Point()), this.boundsElement.PointToScreen(new Point(this.boundsElement.RenderSize.Width, this.boundsElement.RenderSize.Height)));

                this.wrapOffset = default;
                this.wrapBounds = this.elementBounds.AddMargin(InitialBoundsMargin);
                this.isInitialBounds = true;

                GetCursorPos(out var cursorPosition);
                this.initialCursorPosition = cursorPosition;
                this.lastCursorPosition = cursorPosition;

                this.isCaptured = true;
            }
        }

        public void EndCapture()
        {
            lock (this.locker)
            {
                if (!this.isCaptured)
                {
                    throw new Exception("Mouse capture has not been started");
                }

                Mouse.PrimaryDevice.Capture(null);
                this.isCaptured = false;
            }
        }

        public void GetPosition(out double x, out double y)
        {
            lock (this.locker)
            {
                GetCursorPos(out var cursorPosition);

                x = cursorPosition.X;
                y = cursorPosition.Y;

                if (!this.isCaptured)
                {
                    return;
                }

                if (this.enableCursorWrap && (this.lastCursorPosition.X != cursorPosition.X || this.lastCursorPosition.Y != cursorPosition.Y))
                {
                    this.lastCursorPosition = cursorPosition;

                    var offsetLeft = Math.Max(0, this.wrapBounds.Left - x);
                    var offsetTop = Math.Max(0, this.wrapBounds.Top - y);
                    var offsetRight = Math.Min(0, this.wrapBounds.Right - 1 - x);
                    var offsetBottom = Math.Min(0, this.wrapBounds.Bottom - 1 - y);

                    var isOutsideBounds = offsetLeft > 0 || offsetTop > 0 || offsetRight < 0 || offsetBottom < 0;

                    var wrapX = (int)this.wrapBounds.Width - JumpMargin;
                    var wrapY = (int)this.wrapBounds.Height - JumpMargin;

                    if (this.isInitialBounds &&
                        (isOutsideBounds ||
                        // outside initial cursor area
                        (Math.Abs(this.initialCursorPosition.X - cursorPosition.X) > SetDragBoundsThreshold ||
                        Math.Abs(this.initialCursorPosition.Y - cursorPosition.Y) > SetDragBoundsThreshold) &&
                        // not over element's margin
                        this.elementBounds.AddMargin(-SetDragBoundsThreshold).Contains(cursorPosition.X, cursorPosition.Y)))
                    {
                        this.isInitialBounds = false;
                        this.wrapBounds = this.elementBounds.AddMargin(DragBoundsMargin);
                        wrapX -= InitialBoundsMargin - DragBoundsMargin;
                        wrapY -= InitialBoundsMargin - DragBoundsMargin;
                    }

                    if (isOutsideBounds && wrapX > 0 && wrapY > 0)
                    {
                        var offsetX = offsetLeft > -offsetRight ? offsetLeft : offsetRight;
                        var offsetY = offsetTop > -offsetBottom ? offsetTop : offsetBottom;

                        var jumpX = wrapX * Math.Sign(offsetX) * Math.Ceiling(Math.Abs(offsetX / wrapX));
                        var jumpY = wrapY * Math.Sign(offsetY) * Math.Ceiling(Math.Abs(offsetY / wrapY));

                        x += jumpX;
                        y += jumpY;
                        this.wrapOffset.X += (int)jumpX;
                        this.wrapOffset.Y += (int)jumpY;

                        SetCursorPos((int)x, (int)y);
                    }
                }

                x -= this.wrapOffset.X;
                y -= this.wrapOffset.Y;
            }
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

        public void StartCapture(bool enableCursorWrap)
        {
            this.mousePositionSource.StartCapture(enableCursorWrap);
        }

        public void EndCapture()
        {
            this.mousePositionSource.EndCapture();
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
