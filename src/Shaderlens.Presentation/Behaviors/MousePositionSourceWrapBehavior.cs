namespace Shaderlens.Presentation.Behaviors
{
    using static WinApi;

    public class MousePositionSourceWrapBehavior : IMousePositionSource, IDisposable
    {
        private class Token : IDisposable
        {
            private readonly MousePositionSourceWrapBehavior behavior;

            public Token(MousePositionSourceWrapBehavior behavior)
            {
                this.behavior = behavior;
            }

            public void Dispose()
            {
                this.behavior.ReleaseCapture();
            }
        }

        private const int InitialBoundsMargin = -1; // wrap the cursor when it moves outside the element bounds + this margin
        private const int DragBoundsMargin = -20; // wrap the cursor when it moves outside the element bounds + this margin
        private const int SetDragBoundsThreshold = 100; // move from InitialBoundsMargin to DragBoundsMargin when the cursor has moved over this threadhold
        private const int JumpMargin = 20; // jump from one side of the bounds to the other + this margin (for example, from left to right - 20, or from right to left + 20), this is in addtion to the bounds margin
        private readonly object locker;
        private readonly IInputPositionListener listener;
        private readonly DispatcherTimer captureTimer;

        private bool enableCursorWrap;
        private FrameworkElement boundsElement;
        private Rect elementBounds;
        private POINT wrapOffset;
        private Rect wrapBounds;
        private POINT initialCursorPosition;
        private bool isInitialBounds;
        private POINT lastCursorPosition;

        private MousePositionSourceWrapBehavior(FrameworkElement boundsElement, IInputPositionListener listener)
        {
            this.locker = new object();
            this.boundsElement = boundsElement;
            this.boundsElement.PreviewMouseMove += OnPreviewMouseMove;
            this.listener = listener;
            this.captureTimer = new DispatcherTimer(DispatcherPriority.Input, boundsElement.Dispatcher) { Interval = TimeSpan.FromSeconds(1.0 / 60.0), IsEnabled = false };
            this.captureTimer.Tick += (sender, e) => ProcessMouseMove();
        }

        public void Dispose()
        {
            this.boundsElement.PreviewMouseMove -= OnPreviewMouseMove;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ProcessMouseMove();
        }

        public void GetPosition(out double x, out double y)
        {
            GetCursorPos(out var cursorPosition);

            x = cursorPosition.X;
            y = cursorPosition.Y;

            lock (this.locker)
            {
                if (this.enableCursorWrap)
                {
                    x -= this.wrapOffset.X;
                    y -= this.wrapOffset.Y;
                }
            }
        }

        public IDisposable Capture(bool enableCursorWrap)
        {
            lock (this.locker)
            {
                if (Mouse.PrimaryDevice.Captured != null)
                {
                    throw new Exception("Mouse is already captured");
                }

                this.enableCursorWrap = enableCursorWrap;

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

                Mouse.PrimaryDevice.Capture(this.boundsElement);
                this.captureTimer.IsEnabled = !AnyMouseButtonPressed();

                return new Token(this);
            }
        }

        private void ReleaseCapture()
        {
            lock (this.locker)
            {
                if (Mouse.PrimaryDevice.Captured != null && Mouse.PrimaryDevice.Captured != this.boundsElement)
                {
                    throw new Exception("Mouse has been captured by a different element");
                }

                Mouse.PrimaryDevice.Capture(null);
                this.captureTimer.IsEnabled = false;
                this.enableCursorWrap = false;
            }
        }

        private void ProcessMouseMove()
        {
            GetCursorPos(out var cursorPosition);

            var x = (double)cursorPosition.X;
            var y = (double)cursorPosition.Y;

            lock (this.locker)
            {
                if (this.lastCursorPosition.X == cursorPosition.X && this.lastCursorPosition.Y == cursorPosition.Y)
                {
                    return;
                }

                if (this.enableCursorWrap)
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

            this.listener.InputPositionChanged(new Point(x, y));
        }

        private static bool AnyMouseButtonPressed()
        {
            return Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.MiddleButton == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.RightButton == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.XButton1 == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.XButton2 == MouseButtonState.Pressed;
        }

        public static MousePositionSourceWrapBehavior Register(FrameworkElement boundsElement, IInputPositionListener listener)
        {
            return new MousePositionSourceWrapBehavior(boundsElement, listener);
        }
    }
}
