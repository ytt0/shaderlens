namespace Shaderlens.Presentation.Elements
{
    public class ColorPicker : FrameworkElement
    {
        private static readonly TimeSpan SnapReleaseTime = TimeSpan.FromSeconds(0.2);

        private const double HueThickness = 0.15;
        private const double PivotPosition = 0.68;

        private const double SVArea = 1.0 - 2.0 * HueThickness;
        private const double PivotRadius = 1.0 - HueThickness - PivotPosition;
        private const double EdgeLength = PivotPosition * 2.0;
        private const double CornerLength = PivotRadius * Math.PI / 2.0;
        private const double SegmentLength = EdgeLength + CornerLength;

        private const int HCanvasSize = 101;
        private const int SVCanvasSize = 10;

        private const double HueOffset = 0.125;
        private const double HueDirection = 1.0;

        private const double CursorRadius = 0.05;
        private const double CursorDragMargin = 0.02;

        public EventHandler? ColorChanged;
        private OkhsvColor color;
        public OkhsvColor Color
        {
            get { return this.color; }
            set
            {
                if (this.color.H != value.H)
                {
                    this.svBitmap = null;
                }

                ClearCursorsPosition();

                this.color = value;
                this.ColorChanged?.Invoke(this, EventArgs.Empty);

                InvalidateVisual();
            }
        }

        public bool AutoAdjustSV { get; set; }

        private readonly BitmapSource hBitmap;

        private BitmapSource? svBitmap;
        private RectangleGeometry? svBitmapClip;
        private Pen? cursorPen1;
        private Pen? cursorPen2;

        private Point? hCursorPosition;
        private Point? svCursorPosition;

        private bool hCursorCaptured;
        private bool svCursorCaptured;
        private Point captureOffset;
        private OkhsvColor captureStartColor;
        private DateTime snapSStartTime;
        private DateTime snapVStartTime;
        private bool snapS;
        private bool snapV;

        public ColorPicker()
        {
            this.hBitmap = CreateHBitmap(HCanvasSize);

            this.Color = new OkhsvColor(0.76, 0.0, 1.0);
            this.AutoAdjustSV = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.svBitmap = null;
            this.hCursorPosition = null;
            this.svCursorPosition = null;
            this.cursorPen1 = null;
            this.cursorPen2 = null;
            this.svBitmapClip = null;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var drawingSize = GetDrawingSize();
            var drawingOffset = GetDrawingOffset(drawingSize);

            var hSize = drawingSize;
            var svSize = drawingSize * SVArea;
            var svPixelSize = svSize / (SVCanvasSize - 1.0);
            var svRect = new Rect(-svPixelSize / 2.0, -svPixelSize / 2.0, svSize + svPixelSize, svSize + svPixelSize);

            if (this.svBitmap is null)
            {
                this.svBitmap = CreateSVBitmap(this.Color.H, SVCanvasSize);
            }

            if (this.svBitmapClip is null)
            {
                this.svBitmapClip = new RectangleGeometry(new Rect(0, 0, svSize, svSize));
            }

            if (this.cursorPen1 is null)
            {
                this.cursorPen1 = new Pen(Brushes.Black, 0.008 * drawingSize);
            }

            if (this.cursorPen2 is null)
            {
                this.cursorPen2 = new Pen(Brushes.White, 0.004 * drawingSize);
            }

            drawingContext.PushTransform(new TranslateTransform(drawingOffset.X, drawingOffset.Y));
            {
                drawingContext.DrawImage(this.hBitmap, new Rect(0.0, 0.0, hSize, hSize));

                drawingContext.PushTransform(new TranslateTransform((drawingSize - svSize) / 2.0, (drawingSize - svSize) / 2.0));
                {
                    drawingContext.PushClip(this.svBitmapClip);
                    drawingContext.DrawImage(this.svBitmap, svRect);
                    drawingContext.Pop();
                }
                drawingContext.Pop();
            }
            drawingContext.Pop();

            SetCursorsPosition(drawingSize, drawingOffset);

            drawingContext.DrawEllipse(null, this.cursorPen1, this.hCursorPosition!.Value, CursorRadius * drawingSize, CursorRadius * drawingSize);
            drawingContext.DrawEllipse(null, this.cursorPen2, this.hCursorPosition!.Value, CursorRadius * drawingSize, CursorRadius * drawingSize);

            drawingContext.DrawEllipse(null, this.cursorPen1, this.svCursorPosition!.Value, CursorRadius * drawingSize, CursorRadius * drawingSize);
            drawingContext.DrawEllipse(null, this.cursorPen2, this.svCursorPosition!.Value, CursorRadius * drawingSize, CursorRadius * drawingSize);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                e.MouseDevice.Capture(this);

                var drawingSize = GetDrawingSize();
                var drawingOffset = GetDrawingOffset(drawingSize);

                var mousePosition = e.GetPosition(this);

                SetCursorsPosition(drawingSize, drawingOffset);

                this.hCursorCaptured = false;
                this.svCursorCaptured = false;

                var dragRadius = (CursorRadius + CursorDragMargin) * drawingSize;

                var x = (mousePosition.X - drawingOffset.X) / drawingSize;
                var y = (mousePosition.Y - drawingOffset.Y) / drawingSize;
                var overSVArea = Math.Abs(x * 2.0 - 1.0) < SVArea && Math.Abs(y * 2.0 - 1.0) < SVArea;

                this.captureStartColor = this.Color;

                if (Distance(mousePosition, this.hCursorPosition!.Value) < dragRadius)
                {
                    this.hCursorCaptured = true;
                    this.captureOffset = new Point(mousePosition.X - this.hCursorPosition!.Value.X, mousePosition.Y - this.hCursorPosition!.Value.Y);
                }
                else if (Distance(mousePosition, this.svCursorPosition!.Value) < dragRadius)
                {
                    this.svCursorCaptured = true;
                    this.captureOffset = new Point(mousePosition.X - this.svCursorPosition!.Value.X, mousePosition.Y - this.svCursorPosition!.Value.Y);
                }
                else
                {
                    this.hCursorCaptured = !overSVArea;
                    this.svCursorCaptured = overSVArea;
                    this.captureOffset = new Point();

                    SetSnap();

                    SetCapturedCursors(x, y, this.snapS, this.snapV);
                }

                e.Handled = true;
            }

            if (e.ChangedButton == MouseButton.Right && IsDragging())
            {
                DragCancel();
                DragEnd();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.MouseDevice.Captured == this)
            {
                var drawingSize = GetDrawingSize();
                var drawingOffset = GetDrawingOffset(drawingSize);

                var mousePosition = e.GetPosition(this);

                var x = (mousePosition.X - drawingOffset.X - this.captureOffset.X) / drawingSize;
                var y = (mousePosition.Y - drawingOffset.Y - this.captureOffset.Y) / drawingSize;

                SetSnap();

                SetCapturedCursors(x, y, this.snapS, this.snapV);

                e.Handled = true;
            }
        }

        private void SetSnap()
        {
            var now = DateTime.Now;

            if (Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                this.snapSStartTime = now;
            }

            if (Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Shift)
            {
                this.snapVStartTime = now;
            }

            this.snapS = now - this.snapSStartTime < SnapReleaseTime;
            this.snapV = now - this.snapVStartTime < SnapReleaseTime;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsDragging())
            {
                DragEnd();

                e.Handled = true;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsDragging())
            {
                DragCancel();
                DragEnd();

                e.Handled = true;
            }
        }

        private bool IsDragging()
        {
            return Mouse.PrimaryDevice.Captured == this;
        }

        private void DragCancel()
        {
            this.Color = this.captureStartColor;
        }

        private void DragEnd()
        {
            this.hCursorCaptured = false;
            this.svCursorCaptured = false;

            Mouse.PrimaryDevice.Capture(null);
        }

        private void SetCapturedCursors(double x, double y, bool snapS, bool snapV)
        {
            if (this.hCursorCaptured)
            {
                var h = GetPickerHueAt(2.0 * x - 1.0, 2.0 * y - 1.0, true);
                var sourceColor = this.Color;

                if (this.AutoAdjustSV)
                {
                    var lab = OklabColor.FromLinearRgb(this.captureStartColor.ToLinearRgb());

                    var r = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);

                    lab.A = r * Math.Cos(h * Math.Tau);
                    lab.B = r * Math.Sin(h * Math.Tau);

                    this.Color = OkhsvColor.FromLab(lab);
                }
                else
                {
                    var s = this.Color.S;
                    var v = this.Color.V;

                    this.Color = new OkhsvColor(h, s, v);
                }

                if (sourceColor.S < 0.0001 || sourceColor.V < 0.0001)
                {
                    this.Color = new OkhsvColor(h, sourceColor.S, this.Color.V);
                }
            }

            if (this.svCursorCaptured)
            {
                var s = snapS ? this.captureStartColor.S : Math.Clamp((x - HueThickness) / SVArea, 0.0, 1.0);
                var v = snapV ? this.captureStartColor.V : Math.Clamp(1.0 - (y - HueThickness) / SVArea, 0.0, 1.0);

                this.Color = new OkhsvColor(this.Color.H, s, v);
            }
        }

        private void ClearCursorsPosition()
        {
            this.hCursorPosition = null;
            this.svCursorPosition = null;
        }

        private void SetCursorsPosition(double drawingSize, Point drawingOffset)
        {
            if (this.hCursorPosition is null)
            {
                this.hCursorPosition = GetHPosition(this.Color.H);
                this.hCursorPosition = new Point(this.hCursorPosition.Value.X * drawingSize + drawingOffset.X, this.hCursorPosition.Value.Y * drawingSize + drawingOffset.Y);
            }

            if (this.svCursorPosition is null)
            {
                this.svCursorPosition = GetSVPosition(this.Color.S, this.Color.V);
                this.svCursorPosition = new Point(this.svCursorPosition.Value.X * drawingSize + drawingOffset.X, this.svCursorPosition.Value.Y * drawingSize + drawingOffset.Y);
            }
        }

        private double GetDrawingSize()
        {
            return Math.Min(this.RenderSize.Width, this.RenderSize.Height);
        }

        private Point GetDrawingOffset(double drawingSize)
        {
            return new Point((this.RenderSize.Width - drawingSize) / 2.0, (this.RenderSize.Height - drawingSize) / 2.0);
        }

        private static BitmapSource CreateHBitmap(int size)
        {
            var buffer = new byte[size * size * 3];
            var r = (double)(size / 2);

            var i = 0;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var h = GetPickerHueAt((x - r) / r, (y - r) / r, false);

                    var l = 0.8;
                    var a = 0.4 * Math.Cos(h * Math.Tau);
                    var b = 0.4 * Math.Sin(h * Math.Tau);
                    var rgb = new OklabColor(l, a, b).ToLinearRgb().ToSrgb().Clamp();

                    buffer[i++] = (byte)(255 * rgb.R);
                    buffer[i++] = (byte)(255 * rgb.G);
                    buffer[i++] = (byte)(255 * rgb.B);
                }
            }

            return BitmapSource.Create(size, size, 96.0, 96.0, PixelFormats.Rgb24, null, buffer, 3 * size);
        }

        private static BitmapSource CreateSVBitmap(double h, int size)
        {
            var buffer = new byte[size * size * 3];

            var hueProperties = new OkhsvColor.HueProperties(h);
            var step = 1.0 / (size - 1.0);

            var i = 0;
            for (var y = 0; y < size; y++)
            {
                var v = 1.0 - y * step;

                for (var x = 0; x < size; x++)
                {
                    var s = x * step;
                    var rgb = new OkhsvColor(h, s, v).ToLinearRgb(hueProperties).ToSrgb();

                    buffer[i++] = (byte)(255 * rgb.R);
                    buffer[i++] = (byte)(255 * rgb.G);
                    buffer[i++] = (byte)(255 * rgb.B);
                }
            }

            return BitmapSource.Create(size, size, 96.0, 96.0, PixelFormats.Rgb24, null, buffer, 3 * size);
        }

        private static double GetPickerHueAt(double x, double y, bool smooth)
        {
            var q = x > 0.0 ? y > 0.0 ? 0 : 3 : y > 0.0 ? 1 : 2;

            var x0 = x;
            var y0 = y;

            x = Math.Abs(x);
            y = Math.Abs(y);
            if (q == 1 || q == 3)
            {
                (y, x) = (x, y);
            }

            var f = x >= PivotPosition && y >= PivotPosition ? 0.5 * EdgeLength :
                x < y ? EdgeLength * (1.0 - 0.5 * x / PivotPosition) : 0.5 * EdgeLength * y / PivotPosition;

            f = EdgeLength * q + f;

            var an = x <= PivotPosition && y <= PivotPosition ? x < y ? 1.0 : 0.0 :
                2.0 * Math.Atan2(Math.Max(0.0, y - PivotPosition), Math.Max(0.0, x - PivotPosition)) / Math.PI;

            an = CornerLength * (q + an);

            var h = (an + f) / (4.0 * SegmentLength);

            if (smooth)
            {
                var edge = 1.0 - Math.Max(x, y);
                var corner = 1.0 - Math.Abs(x - y);
                var t = Math.Clamp((edge - (2.0 - corner * corner) * HueThickness) / HueThickness, 0.0, 1.0);
                var h2 = (Math.Atan2(y0, x0) + Math.Tau) / Math.Tau;
                h2 -= Math.Floor(h2 - 0.0001);
                h = (1.0 - t) * h + t * h2;
            }

            h = (h + HueOffset + 0.5) * HueDirection;
            h = (h + 10.0) % 1.0;

            return h;
        }

        private static Point GetHPosition(double h)
        {
            h = h * HueDirection - HueOffset - 0.5;
            h = (h + 10.0) % 1.0;

            h += EdgeLength / (8.0 * SegmentLength);

            var index = (int)Math.Floor(h * 4.0) % 4;

            h = h * 4.0 % 1.0;
            h *= SegmentLength;

            var an = Math.PI / 2.0 * (h - EdgeLength) / CornerLength;
            var f = h / EdgeLength;

            double cx, cy;

            if (f < 1.0)
            {
                cx = 1.0 - HueThickness;
                cy = f * EdgeLength - 0.5 * EdgeLength;
            }
            else
            {
                cx = PivotPosition + PivotRadius * Math.Cos(an);
                cy = PivotPosition + PivotRadius * Math.Sin(an);
            }

            if (index == 1)
            {
                var w = cx;
                cx = -cy;
                cy = w;
            }
            if (index == 2)
            {
                cx = -cx;
                cy = -cy;
            }
            if (index == 3)
            {
                var w = cx;
                cx = cy;
                cy = -w;
            }

            cx = (cx + 1.0) / 2.0;
            cy = (cy + 1.0) / 2.0;

            return new Point(cx, cy);
        }

        private static Point GetSVPosition(double s, double v)
        {
            return new Point(HueThickness + Math.Clamp(s, 0.0, 1.0) * SVArea, HueThickness + Math.Clamp(1.0 - v, 0.0, 1.0) * SVArea);
        }

        private static double Distance(Point p1, Point p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
