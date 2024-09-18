namespace Shaderlens.Presentation.Elements
{
    public class SplitterHandle : FrameworkElement
    {
        public event EventHandler? RatioChanged;
        private double ratio;
        public double Ratio
        {
            get { return this.ratio; }
            set
            {
                if (this.ratio != value)
                {
                    this.ratio = value;
                    RatioChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private const double CaptureThickness = 4;
        private const double MouseOverThickness = 1;

        public Brush HoverBrush { get; set; }

        private Point moveStartPosition;
        private double moveStartRatio;

        public SplitterHandle()
        {
            this.HoverBrush = new SolidColorBrush(Color.FromArgb(15, 0, 0, 0));
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            e.Cursor = Cursors.SizeWE;
            e.Handled = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var ratio = this.Ratio * this.RenderSize.Width;

            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(ratio - CaptureThickness / 2, 0, CaptureThickness, this.RenderSize.Height));

            if (this.IsMouseOver)
            {
                drawingContext.DrawRectangle(this.HoverBrush, null, new Rect(ratio - MouseOverThickness / 2, 0, MouseOverThickness, this.RenderSize.Height));
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsResizing())
            {
                CancelResize();
                e.Handled = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                StartResize();
                e.Handled = true;
            }
            else if (IsResizing())
            {
                CancelResize();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsResizing())
            {
                var offset = PointFromScreen(this.moveStartPosition).X - Mouse.PrimaryDevice.GetPosition(this).X;

                this.Ratio = Math.Clamp(this.moveStartRatio - offset / this.RenderSize.Width, 0.0, 1.0);
                InvalidateVisual();
                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsResizing())
            {
                EndResize();
                e.Handled = true;
            }
        }

        private bool IsResizing()
        {
            return Mouse.PrimaryDevice.Captured == this;
        }

        private void StartResize()
        {
            Mouse.PrimaryDevice.Capture(this);
            this.moveStartPosition = PointToScreen(Mouse.PrimaryDevice.GetPosition(this));
            this.moveStartRatio = this.Ratio;
        }

        private void CancelResize()
        {
            this.Ratio = this.moveStartRatio;
            InvalidateVisual();
            Mouse.PrimaryDevice.Capture(null);
        }

        private static void EndResize()
        {
            Mouse.PrimaryDevice.Capture(null);
        }
    }
}
