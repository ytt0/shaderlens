using Shaderlens.Presentation.Extensions;

namespace Shaderlens.Presentation.Elements
{
    public interface IGraphTheme : IThemeResources
    {
        IThemeResource<Brush> CursorFill { get; }
        IThemeResource<Brush> CursorStroke { get; }
        IThemeResource<Brush> SourceCursorFill { get; }
        IThemeResource<Brush> SourceCursorStroke { get; }
        IThemeResource<Brush> EdgeCursorFill { get; }
        IThemeResource<Brush> EdgeCursorStroke { get; }
        IThemeResource<Brush> SourceEdgeCursorFill { get; }
        IThemeResource<Brush> SourceEdgeCursorStroke { get; }
        IThemeResource<Brush> GridStroke { get; }
        IThemeResource<Brush> BoundsStroke { get; }
        IThemeResource<Brush> BoundsFill { get; }
    }

    public class PositionGraph : FrameworkElement
    {
        private const double CursorRadius = 10.0;
        private const double CursorThickness = 1.5;
        private const double CursorBorderThickness = 0.5;
        private const double CursorDragMargin = 15.0;
        private const double EdgeCursorMargin = 8.0;
        private const int GridBase = 10;
        private static readonly Geometry BoundsBrushGeometry = Geometry.Parse($"M0,0 L16,16 L15,16 L0,1Z M15,0 L16,0 L16,1Z").WithFreeze();

        private static readonly Geometry CursorGeometry = Geometry.Combine(new EllipseGeometry(new Point(), CursorRadius, CursorRadius), new EllipseGeometry(new Point(), CursorRadius - CursorThickness, CursorRadius - CursorThickness), GeometryCombineMode.Exclude, Transform.Identity);

        private static readonly Geometry TopLeftEdgeCursorGeometry = Geometry.Parse($"M-6,-6 L3,-6 L-6,3Z").WithFreeze();
        private static readonly Geometry TopRightEdgeCursorGeometry = Geometry.Parse($"M6,-6 L-3,-6 L6,3Z").WithFreeze();
        private static readonly Geometry TopEdgeCursorGeometry = Geometry.Parse($"M0,-6 L6,0 L-6,0Z").WithFreeze();

        private static readonly Geometry BottomLeftEdgeCursorGeometry = Geometry.Parse($"M-6,6 L3,6 L-6,-3Z").WithFreeze();
        private static readonly Geometry BottomRightEdgeCursorGeometry = Geometry.Parse($"M6,6 L-3,6 L6,-3Z").WithFreeze();
        private static readonly Geometry BottomEdgeCursorGeometry = Geometry.Parse($"M0,6 L6,0 L-6,0Z").WithFreeze();

        private static readonly Geometry LeftEdgeCursorGeometry = Geometry.Parse($"M-6,0 L0,-6 L0,6Z").WithFreeze();
        private static readonly Geometry RightEdgeCursorGeometry = Geometry.Parse($"M6,0 L0,-6 L0,6Z").WithFreeze();

        public event EventHandler? ToggleTargetValueRequested;
        public event EventHandler? ToggleSourceValueRequested;

        public static readonly DependencyProperty AxisXStrokeProperty = DependencyProperty.Register("AxisXStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush AxisXStroke
        {
            get { return (Brush)GetValue(AxisXStrokeProperty); }
            set { SetValue(AxisXStrokeProperty, value); }
        }

        public static readonly DependencyProperty AxisYStrokeProperty = DependencyProperty.Register("AxisYStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Green, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush AxisYStroke
        {
            get { return (Brush)GetValue(AxisYStrokeProperty); }
            set { SetValue(AxisYStrokeProperty, value); }
        }

        public static readonly DependencyProperty CursorFillProperty = DependencyProperty.Register("CursorFill", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush CursorFill
        {
            get { return (Brush)GetValue(CursorFillProperty); }
            set { SetValue(CursorFillProperty, value); }
        }

        public static readonly DependencyProperty CursorStrokeProperty = DependencyProperty.Register("CursorStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, (sender, e) => ((PositionGraph)sender).cursorPen.Brush = (Brush)e.NewValue));
        public Brush CursorStroke
        {
            get { return (Brush)GetValue(CursorStrokeProperty); }
            set { SetValue(CursorStrokeProperty, value); }
        }

        public static readonly DependencyProperty SourceCursorFillProperty = DependencyProperty.Register("SourceCursorFill", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush SourceCursorFill
        {
            get { return (Brush)GetValue(SourceCursorFillProperty); }
            set { SetValue(SourceCursorFillProperty, value); }
        }

        public static readonly DependencyProperty SourceCursorStrokeProperty = DependencyProperty.Register("SourceCursorStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender, (sender, e) => ((PositionGraph)sender).sourceCursorPen.Brush = (Brush)e.NewValue));
        public Brush SourceCursorStroke
        {
            get { return (Brush)GetValue(SourceCursorStrokeProperty); }
            set { SetValue(SourceCursorStrokeProperty, value); }
        }

        public static readonly DependencyProperty EdgeCursorFillProperty = DependencyProperty.Register("EdgeCursorFill", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush EdgeCursorFill
        {
            get { return (Brush)GetValue(EdgeCursorFillProperty); }
            set { SetValue(EdgeCursorFillProperty, value); }
        }

        public static readonly DependencyProperty EdgeCursorStrokeProperty = DependencyProperty.Register("EdgeCursorStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, (sender, e) => ((PositionGraph)sender).edgeCursorPen.Brush = (Brush)e.NewValue));
        public Brush EdgeCursorStroke
        {
            get { return (Brush)GetValue(EdgeCursorStrokeProperty); }
            set { SetValue(EdgeCursorStrokeProperty, value); }
        }

        public static readonly DependencyProperty SourceEdgeCursorFillProperty = DependencyProperty.Register("SourceEdgeCursorFill", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush SourceEdgeCursorFill
        {
            get { return (Brush)GetValue(SourceEdgeCursorFillProperty); }
            set { SetValue(SourceEdgeCursorFillProperty, value); }
        }

        public static readonly DependencyProperty SourceEdgeCursorStrokeProperty = DependencyProperty.Register("SourceEdgeCursorStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender, (sender, e) => ((PositionGraph)sender).sourceEdgeCursorPen.Brush = (Brush)e.NewValue));
        public Brush SourceEdgeCursorStroke
        {
            get { return (Brush)GetValue(SourceEdgeCursorStrokeProperty); }
            set { SetValue(SourceEdgeCursorStrokeProperty, value); }
        }

        public static readonly DependencyProperty GridStrokeProperty = DependencyProperty.Register("GridStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush GridStroke
        {
            get { return (Brush)GetValue(GridStrokeProperty); }
            set { SetValue(GridStrokeProperty, value); }
        }

        public static readonly DependencyProperty BoundsFillProperty = DependencyProperty.Register("BoundsFill", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, (sender, e) => ((PositionGraph)sender).boundsFillDrawing.Brush = (Brush)e.NewValue));
        public Brush BoundsFill
        {
            get { return (Brush)GetValue(BoundsFillProperty); }
            set { SetValue(BoundsFillProperty, value); }
        }

        public static readonly DependencyProperty BoundsStrokeProperty = DependencyProperty.Register("BoundsStroke", typeof(Brush), typeof(PositionGraph), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, (sender, e) => ((PositionGraph)sender).boundsPen.Brush = (Brush)e.NewValue));
        public Brush BoundsStroke
        {
            get { return (Brush)GetValue(BoundsStrokeProperty); }
            set { SetValue(BoundsStrokeProperty, value); }
        }

        public int ValueDisplayDecimals { get; private set; }

        public bool NormalizeValue { get; set; }
        public bool NormalBounds { get; set; }
        public Point MinValue { get; set; }
        public Point MaxValue { get; set; }

        public event EventHandler? ValueChanged;
        private Point value;
        public Point Value
        {
            get { return this.value; }
            set
            {
                var roundValue = RoundValue(value, this.RoundDecimals);

                if (this.value != roundValue)
                {
                    this.value = roundValue;
                    this.ValueChanged?.Invoke(this, EventArgs.Empty);
                    InvalidateVisual();
                }
            }
        }

        private Point sourceValue;
        public Point SourceValue
        {
            get { return this.sourceValue; }
            set
            {
                this.sourceValue = value;
                InvalidateVisual();
            }
        }

        public event EventHandler? OffsetChanged;
        private Point offset;
        public Point Offset
        {
            get { return this.offset; }
            set
            {
                this.offset = value;
                this.OffsetChanged?.Invoke(this, EventArgs.Empty);
                InvalidateVisual();
            }
        }

        public double MinScale { get; set; }
        public double MaxScale { get; set; }

        public event EventHandler? ScaleChanged;
        private double scale;
        private double scaleStep;
        public double Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = Math.Clamp(value, this.MinScale, this.MaxScale);
                this.scaleStep = Math.Pow(GridBase, Math.Max(-this.RoundDecimals, -Math.Floor(Math.Log(this.Scale) / Math.Log(GridBase)) - 3));

                this.ScaleChanged?.Invoke(this, EventArgs.Empty);
                InvalidateVisual();
            }
        }

        public double ScaleFactor { get; set; }
        public double ScaleDragFactor { get; set; }

        public int RoundDecimals { get; set; }
        public double SmallStepFactor { get; set; }
        public double MediumStepFactor { get; set; }
        public double LargeStepFactor { get; set; }
        public Point Step { get; set; }

        private readonly InputStateBindings inputStateBindings;
        private readonly InputStateSourceBehavior inputStateSource;
        private readonly InputPositionBindings inputPositionBindings;
        private readonly MousePositionSourceWrapBehavior mousePositionSource;
        private readonly Pen cursorPen;
        private readonly Pen sourceCursorPen;
        private readonly Pen edgeCursorPen;
        private readonly Pen sourceEdgeCursorPen;
        private readonly GeometryDrawing boundsFillDrawing;
        private readonly Pen boundsPen;
        private readonly DrawingBrush boundsBrush;
        private readonly RectangleGeometry boundsBaseGeometry;
        private readonly RectangleGeometry boundsRectangleGeometry;
        private readonly EllipseGeometry boundsEllipseGeometry;
        private readonly IPositionGraphInputs inputs;
        private double deviceScale;
        private readonly ScaleTransform deviceScaleTransform;
        private Size scaledRenderSize;
        private double drawingScale;
        private bool isDeviceRenderSizeValid;
        private bool isResetViewPending;

        public PositionGraph(IPositionGraphInputs inputs, IGraphTheme theme)
        {
            this.inputs = inputs;

            this.inputStateBindings = new InputStateBindings(GlobalInputBindings.Empty);
            this.inputStateSource = InputStateSourceBehavior.Register(this, this.inputStateBindings);

            this.inputPositionBindings = new InputPositionBindings();
            this.mousePositionSource = MousePositionSourceWrapBehavior.Register(this, this.inputPositionBindings);

            this.inputStateBindings.Add(this.inputs.Drag.CreateStartEvent(), DragStart);
            this.inputStateBindings.Add(this.inputs.Pan.CreateStartEvent(), PanStart);
            this.inputStateBindings.Add(this.inputs.Scale.CreateStartEvent(), ScaleStart);
            this.inputStateBindings.Add(this.inputs.ScaleUp, () => SetScale(true), null, true, true);
            this.inputStateBindings.Add(this.inputs.ScaleDown, () => SetScale(false), null, true, true);
            this.inputStateBindings.Add(this.inputs.ScaleReset, ResetScale);
            this.inputStateBindings.Add(this.inputs.FocusView, FocusView);
            this.inputStateBindings.Add(this.inputs.ResetView, ResetView);
            this.inputStateBindings.Add(this.inputs.ToggleTargetValue, ToggleTargetValue);
            this.inputStateBindings.Add(this.inputs.ToggleSourceValue, ToggleSourceValue);

            this.cursorPen = new Pen(this.CursorStroke, CursorBorderThickness) { LineJoin = PenLineJoin.Round };
            this.sourceCursorPen = new Pen(this.SourceCursorStroke, CursorBorderThickness) { LineJoin = PenLineJoin.Round };
            this.edgeCursorPen = new Pen(this.EdgeCursorStroke, CursorBorderThickness) { LineJoin = PenLineJoin.Round };
            this.sourceEdgeCursorPen = new Pen(this.SourceEdgeCursorStroke, CursorBorderThickness) { LineJoin = PenLineJoin.Round };

            this.boundsPen = new Pen(this.BoundsStroke, 1.0);
            this.boundsFillDrawing = new GeometryDrawing(this.BoundsFill, null, BoundsBrushGeometry);
            this.boundsBrush = new DrawingBrush
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 16, 16),
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = this.boundsFillDrawing
            };

            this.boundsBaseGeometry = new RectangleGeometry();
            this.boundsRectangleGeometry = new RectangleGeometry();
            this.boundsEllipseGeometry = new EllipseGeometry();

            this.deviceScaleTransform = new ScaleTransform(1.0, 1.0);

            this.Value = new Point();
            this.Offset = new Point(0.0, 0.0);

            this.MinValue = new Point(Double.NegativeInfinity, Double.NegativeInfinity);
            this.MaxValue = new Point(Double.PositiveInfinity, Double.PositiveInfinity);

            this.MinScale = 0.0000001;
            this.MaxScale = 1000000.0;
            this.ScaleFactor = 1.1;
            this.Scale = 1.0 / 1.1;
            this.ScaleDragFactor = 1.0;

            this.Step = new Point();
            this.RoundDecimals = 6;
            this.SmallStepFactor = 0.1;
            this.MediumStepFactor = 10.0;
            this.LargeStepFactor = 100.0;

            this.SnapsToDevicePixels = true;
            this.ClipToBounds = true;
            this.Focusable = true;
            this.FocusVisualStyle = null;

            theme.CursorFill.SetReference(this, CursorFillProperty);
            theme.CursorStroke.SetReference(this, CursorStrokeProperty);
            theme.SourceCursorFill.SetReference(this, SourceCursorFillProperty);
            theme.SourceCursorStroke.SetReference(this, SourceCursorStrokeProperty);

            theme.EdgeCursorFill.SetReference(this, EdgeCursorFillProperty);
            theme.SourceEdgeCursorFill.SetReference(this, SourceEdgeCursorFillProperty);
            theme.EdgeCursorStroke.SetReference(this, EdgeCursorStrokeProperty);
            theme.SourceEdgeCursorStroke.SetReference(this, SourceEdgeCursorStrokeProperty);

            theme.GridStroke.SetReference(this, GridStrokeProperty);
            theme.BoundsFill.SetReference(this, BoundsFillProperty);
            theme.BoundsStroke.SetReference(this, BoundsStrokeProperty);
        }

        public void FocusView()
        {
            this.Offset = this.Value;
        }

        public void ResetView()
        {
            if (this.RenderSize.Width == 0 || this.RenderSize.Height == 0)
            {
                this.isResetViewPending = true;
                return;
            }

            this.isResetViewPending = false;

            var scaleFactor = Math.Max(0.0, 1.0 - (2.0 * CursorRadius + CursorBorderThickness) / Math.Min(this.RenderSize.Width, this.RenderSize.Height));

            if (this.NormalBounds)
            {
                this.Offset = default;
                this.Scale = scaleFactor * Math.Min(this.RenderSize.Width, this.RenderSize.Height) / Math.Max(this.RenderSize.Width, this.RenderSize.Height);
            }
            else
            {
                var hasMinX = this.MinValue.X > Double.MinValue;
                var hasMaxX = this.MaxValue.X < Double.MaxValue;
                var hasMinY = this.MinValue.Y > Double.MinValue;
                var hasMaxY = this.MaxValue.Y < Double.MaxValue;
                var hasRangeX = hasMinX && hasMaxX;
                var hasRangeY = hasMinY && hasMaxY;
                var rangeX = hasRangeX ? this.MaxValue.X - this.MinValue.X : 0.0;
                var rangeY = hasRangeY ? this.MaxValue.Y - this.MinValue.Y : 0.0;

                this.Offset = new Point(
                    hasRangeX ? (this.MaxValue.X + this.MinValue.X) / 2.0 :
                    hasMinX ? this.MinValue.X + rangeY / 2.0 :
                    hasMaxX ? this.MaxValue.X - rangeY / 2.0 :
                    0.0,
                    hasRangeY ? (this.MaxValue.Y + this.MinValue.Y) / 2.0 :
                    hasMinY ? this.MinValue.Y + rangeX / 2.0 :
                    hasMaxY ? this.MaxValue.Y - rangeX / 2.0 :
                    0.0);

                var viewRange = new Point(
                    Math.Max(hasRangeX ? rangeX / 2 : 0.0, Math.Abs(this.Offset.X - this.Value.X)),
                    Math.Max(hasRangeY ? rangeY / 2 : 0.0, Math.Abs(this.Offset.Y - this.Value.Y)));

                if (viewRange.LengthApprox() > 0.0)
                {
                    var scaleX = viewRange.X * Math.Max(this.RenderSize.Width, this.RenderSize.Height) / this.RenderSize.Width;
                    var scaleY = viewRange.Y * Math.Max(this.RenderSize.Width, this.RenderSize.Height) / this.RenderSize.Height;

                    this.Scale = scaleFactor / Math.Max(scaleX, scaleY);
                }
                else
                {
                    this.Scale = scaleFactor * Math.Min(this.RenderSize.Width, this.RenderSize.Height) / Math.Max(this.RenderSize.Width, this.RenderSize.Height);
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.isDeviceRenderSizeValid = false;

            if (this.isResetViewPending)
            {
                ResetView();
            }

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!this.isDeviceRenderSizeValid)
            {
                var p1 = PointToScreen(new Point());
                var p2 = PointToScreen(new Point(1000, 0));
                this.deviceScale = (p2.X - p1.X) / 1000.0;
                this.deviceScaleTransform.ScaleX = 1.0 / this.deviceScale;
                this.deviceScaleTransform.ScaleY = 1.0 / this.deviceScale;
                this.scaledRenderSize = new Size(Math.Round(this.RenderSize.Width * this.deviceScale), Math.Round(this.RenderSize.Height * this.deviceScale));
                this.drawingScale = Math.Max(this.scaledRenderSize.Width, this.scaledRenderSize.Height) / 2.0;

                this.isDeviceRenderSizeValid = true;
            }

            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));

            var gridScale = Math.Log(this.Scale) / Math.Log(GridBase);
            var gridRatio = Fract(gridScale);
            var gridSize = Math.Pow(GridBase, gridRatio) * this.drawingScale;

            var opacity1 = Sqr(Math.Min(1.0, 0.5 + gridRatio));
            var opacity2 = Sqr(gridRatio * 0.5);

            using (drawingContext.PushTransformScope(this.deviceScaleTransform))
            {
                DrawBounds(drawingContext);

                DrawAxis(drawingContext);

                DrawGrid(drawingContext, gridSize, this.GridStroke, 0);

                using (drawingContext.PushOpacityScope(opacity1))
                {
                    DrawGrid(drawingContext, gridSize / GridBase, this.GridStroke, GridBase);
                }

                using (drawingContext.PushOpacityScope(opacity2))
                {
                    DrawGrid(drawingContext, gridSize / (GridBase * GridBase), this.GridStroke, GridBase * GridBase);
                }
            }


            DrawCursor(drawingContext, this.SourceValue, this.SourceCursorFill, this.sourceCursorPen, this.SourceEdgeCursorFill, this.sourceEdgeCursorPen);
            DrawCursor(drawingContext, this.Value, this.CursorFill, this.cursorPen, this.EdgeCursorFill, this.edgeCursorPen);
        }

        private void DrawCursor(DrawingContext drawingContext, Point position, Brush brush, Pen pen, Brush edgeBrush, Pen edgePen)
        {
            var center = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);

            var scaledPosition = new Point(
                center.X + (position.X - this.Offset.X) * this.Scale * this.drawingScale / this.deviceScale,
                center.Y - (position.Y - this.Offset.Y) * this.Scale * this.drawingScale / this.deviceScale);

            if (TryGetEdgeCursorGeometry(scaledPosition, this.RenderSize, out var geometry))
            {
                scaledPosition.X = Math.Clamp(scaledPosition.X, EdgeCursorMargin, Math.Max(EdgeCursorMargin, this.RenderSize.Width - EdgeCursorMargin));
                scaledPosition.Y = Math.Clamp(scaledPosition.Y, EdgeCursorMargin, Math.Max(EdgeCursorMargin, this.RenderSize.Height - EdgeCursorMargin));
                brush = edgeBrush;
                pen = edgePen;
            }
            else
            {
                geometry = CursorGeometry;
            }

            using (drawingContext.PushTransformScope(new TranslateTransform(scaledPosition.X, scaledPosition.Y)))
            {
                drawingContext.DrawGeometry(brush, pen, geometry);
            }
        }

        private void DrawBounds(DrawingContext drawingContext)
        {
            const double DrawMargin = 10;

            this.boundsBaseGeometry.Rect = new Rect(-DrawMargin, -DrawMargin, this.scaledRenderSize.Width + DrawMargin + DrawMargin, this.scaledRenderSize.Height + DrawMargin + DrawMargin);

            if (this.NormalBounds)
            {
                var r = this.Scale * this.drawingScale;
                var center = new Point(this.scaledRenderSize.Width / 2.0 - r * this.Offset.X, this.scaledRenderSize.Height / 2.0 + r * this.Offset.Y);

                this.boundsEllipseGeometry.Center = center;
                this.boundsEllipseGeometry.RadiusX = r;
                this.boundsEllipseGeometry.RadiusY = r;

                drawingContext.DrawGeometry(this.boundsBrush, this.boundsPen, Geometry.Combine(this.boundsBaseGeometry, this.boundsEllipseGeometry, GeometryCombineMode.Exclude, Transform.Identity));
            }
            else if (this.MinValue.X > Double.MinValue || this.MinValue.Y > Double.MinValue || this.MaxValue.X < Double.MaxValue || this.MaxValue.Y < Double.MaxValue)
            {
                var center = new Point(this.scaledRenderSize.Width / 2.0, this.scaledRenderSize.Height / 2.0);

                var x0 = Math.Clamp(center.X + (this.MinValue.X - this.Offset.X) * this.Scale * this.drawingScale, -DrawMargin, this.scaledRenderSize.Width + DrawMargin);
                var x1 = Math.Clamp(center.X + (this.MaxValue.X - this.Offset.X) * this.Scale * this.drawingScale, -DrawMargin, this.scaledRenderSize.Width + DrawMargin);
                var y0 = Math.Clamp(center.Y - (this.MaxValue.Y - this.Offset.Y) * this.Scale * this.drawingScale, -DrawMargin, this.scaledRenderSize.Height + DrawMargin);
                var y1 = Math.Clamp(center.Y - (this.MinValue.Y - this.Offset.Y) * this.Scale * this.drawingScale, -DrawMargin, this.scaledRenderSize.Height + DrawMargin);

                this.boundsRectangleGeometry.Rect = new Rect(x0, y0, x1 - x0, y1 - y0);

                drawingContext.DrawGeometry(this.boundsBrush, this.boundsPen, Geometry.Combine(this.boundsBaseGeometry, this.boundsRectangleGeometry, GeometryCombineMode.Exclude, Transform.Identity));
            }
        }

        private void DrawAxis(DrawingContext drawingContext)
        {
            var center = new Point(this.scaledRenderSize.Width / 2.0, this.scaledRenderSize.Height / 2.0);
            var originX = -this.Offset.X * this.Scale * this.drawingScale + center.X;
            var originY = this.Offset.Y * this.Scale * this.drawingScale + center.Y;

            originX = Math.Round(originX) + 0.5;
            originY = Math.Round(originY) + 0.5;

            DrawRectangle(drawingContext, this.AxisXStroke, 0, originY - 1.5, this.scaledRenderSize.Width, originY + 1.5);
            DrawRectangle(drawingContext, this.AxisYStroke, originX - 1.5, 0, originX + 1.5, this.scaledRenderSize.Height);
        }

        private void DrawGrid(DrawingContext drawingContext, double gridSize, Brush brush, int skipBase)
        {
            var center = new Point(this.scaledRenderSize.Width / 2.0, this.scaledRenderSize.Height / 2.0);
            var gridScale = this.Scale * this.drawingScale / gridSize;

            var columnCount = (int)Math.Ceiling(this.scaledRenderSize.Width / gridSize + 1);
            var columnsOffset = Fract(this.Offset.X * gridScale - center.X / gridSize);
            var startColumn = (long)Math.Floor(this.Offset.X * gridScale - center.X / gridSize);

            for (var i = 0; i < columnCount; i++)
            {
                if (i + startColumn == 0 || skipBase > 0 && (i + startColumn) % skipBase == 0)
                {
                    continue;
                }

                var x = (i - columnsOffset) * gridSize;
                DrawRectangle(drawingContext, brush, x - 0.5, 0, x + 0.5, this.scaledRenderSize.Height);
            }

            var rowCount = (int)Math.Ceiling(this.scaledRenderSize.Height / gridSize + 1);
            var rowsOffset = Fract(-this.Offset.Y * gridScale - center.Y / gridSize);
            var startRow = (long)Math.Floor(-this.Offset.Y * gridScale - center.Y / gridSize);

            for (var i = 0; i < rowCount; i++)
            {
                if (i + startRow == 0 || skipBase > 0 && (i + startRow) % skipBase == 0)
                {
                    continue;
                }

                var y = (i - rowsOffset) * gridSize;
                DrawRectangle(drawingContext, brush, 0, y - 0.5, this.scaledRenderSize.Width, y + 0.5);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
            {
                Focus();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
            {
                Focus();
            }
        }

        private void ToggleTargetValue()
        {
            ToggleTargetValueRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ToggleSourceValue()
        {
            ToggleSourceValueRequested?.Invoke(this, EventArgs.Empty);
        }

        private void PanStart()
        {
            var startDragPosition = this.mousePositionSource.GetPosition();

            var startOffset = this.Offset;

            this.inputStateSource.SkipLastKeyRepeat();
            var mouseCaptureScope = this.mousePositionSource.Capture(true);
            var inputStateScope = this.inputStateBindings.PushScope();
            var inputPositionScope = this.inputPositionBindings.PushScope(position =>
            {
                this.Offset = new Point(
                    startOffset.X - (position.X - startDragPosition.X) / (this.Scale * this.drawingScale),
                    startOffset.Y + (position.Y - startDragPosition.Y) / (this.Scale * this.drawingScale));
            });

            this.inputStateBindings.Add(this.inputs.Pan.CreateEndEvent(), () =>
            {
                inputPositionScope.Dispose();
                inputStateScope.Dispose();
                mouseCaptureScope.Dispose();
            }, null, false);
        }

        private void ScaleStart()
        {
            var startDragPosition = this.mousePositionSource.GetPosition();

            var startScale = this.Scale;
            var startOffset = this.Offset;

            this.inputStateSource.SkipLastKeyRepeat();
            var mouseCaptureScope = this.mousePositionSource.Capture(true);
            var inputStateScope = this.inputStateBindings.PushScope();
            var inputPositionScope = this.inputPositionBindings.PushScope(position =>
            {
                var scaleFactor = Math.Exp(this.ScaleDragFactor * 0.01 * (startDragPosition.Y - position.Y));
                var offsetFactor = 1.0 / scaleFactor;
                offsetFactor = Math.Min(1.0, offsetFactor * offsetFactor); // slide the offset toward the value

                this.Scale = startScale * scaleFactor;
                this.Offset = new Point(
                    offsetFactor * startOffset.X + (1.0 - offsetFactor) * this.Value.X,
                    offsetFactor * startOffset.Y + (1.0 - offsetFactor) * this.Value.Y);
            });

            this.inputStateBindings.Add(this.inputs.Scale.CreateEndEvent(), () =>
            {
                inputPositionScope.Dispose();
                inputStateScope.Dispose();
                mouseCaptureScope.Dispose();
            }, null, false);
        }

        private void DragStart()
        {
            var startValue = this.Value;

            var lastPosition = this.mousePositionSource.GetPosition();
            var positionOffset = DrawingToScreenPosition(this.Value) - lastPosition;

            var value = ScreenToDrawingPosition(lastPosition);

            if (value.Distance(this.Value) * this.Scale * this.drawingScale / this.deviceScale > CursorDragMargin)
            {
                positionOffset = default;

                if (this.NormalBounds)
                {
                    var length = Math.Max(0.000001, value.Length());

                    if (length > 1.0 || this.NormalizeValue)
                    {
                        value.X /= length;
                        value.Y /= length;
                    }
                }

                this.Value = value;
            }

            var dragWithinBounds = this.MinValue.X <= this.value.X && this.value.X <= this.MaxValue.X && this.MinValue.Y <= this.value.Y && this.value.Y <= this.MaxValue.Y;

            this.inputStateSource.SkipLastKeyRepeat();
            var mouseCaptureScope = this.mousePositionSource.Capture(false);
            var inputStateScope = this.inputStateBindings.PushScope();
            var inputPositionScope = this.inputPositionBindings.PushScope(position =>
            {
                if (lastPosition.DistanceApprox(position) > 1)
                {
                    lastPosition = position;

                    var stepFactor = GetStepFactor();
                    var step = new Point(
                            Math.Max(this.scaleStep, this.NormalBounds ? 0.0 : this.Step.X) * stepFactor,
                            Math.Max(this.scaleStep, this.NormalBounds ? 0.0 : this.Step.Y) * stepFactor);

                    var value = ScreenToDrawingPosition(position + positionOffset);

                    value.X = Math.Round(Math.Round(value.X / step.X) * step.X, this.RoundDecimals);
                    value.Y = Math.Round(Math.Round(value.Y / step.Y) * step.Y, this.RoundDecimals);

                    if (this.NormalBounds)
                    {
                        var length = value.Length();
                        if (length > 1.0 || this.NormalizeValue)
                        {
                            value.X = length > 0.0 ? value.X / length : 1.0;
                            value.Y = length > 0.0 ? value.Y / length : 0.0;
                        }
                    }
                    else if (dragWithinBounds)
                    {
                        value.X = Math.Clamp(value.X, this.MinValue.X, this.MaxValue.X);
                        value.Y = Math.Clamp(value.Y, this.MinValue.Y, this.MaxValue.Y);
                    }

                    SetValueDisplayDecimals(step);
                    this.Value = value;
                }
            });

            this.inputStateBindings.Add(this.inputs.Drag.CreateEndEvent(), () =>
            {
                inputPositionScope.Dispose();
                inputStateScope.Dispose();
                mouseCaptureScope.Dispose();
            }, null, false);

            this.inputStateBindings.Add(this.inputs.DragCancel, () =>
            {
                this.Value = startValue;
                inputPositionScope.Dispose();
                inputStateScope.Dispose();
                mouseCaptureScope.Dispose();
            }, null, false);

            this.inputStateBindings.Add(this.inputs.ScaleUp, () => SetScale(true), null, true, true);
            this.inputStateBindings.Add(this.inputs.ScaleDown, () => SetScale(false), null, true, true);
            this.inputStateBindings.Add(this.inputs.FocusView, FocusView);
            this.inputStateBindings.Add(this.inputs.ResetView, ResetView);
        }

        private void SetScale(bool scaleUp)
        {
            var scaleFactor = scaleUp ? this.ScaleFactor : 1.0 / this.ScaleFactor;
            var offsetFactor = 1.0 / scaleFactor;
            offsetFactor = Math.Min(1.0, offsetFactor * offsetFactor); // slide the offset toward the value

            this.Scale = this.Scale * scaleFactor;
            this.Offset = new Point(
                offsetFactor * this.Offset.X + (1.0 - offsetFactor) * this.Value.X,
                offsetFactor * this.Offset.Y + (1.0 - offsetFactor) * this.Value.Y);
        }

        private void ResetScale()
        {
            var scaleFactor = Math.Max(0.0, 1.0 - (2.0 * CursorRadius + CursorBorderThickness) / Math.Min(this.RenderSize.Width, this.RenderSize.Height));
            this.Scale = scaleFactor * Math.Min(this.RenderSize.Width, this.RenderSize.Height) / Math.Max(this.RenderSize.Width, this.RenderSize.Height);
            this.Offset = new Point();
        }

        private Point DrawingToScreenPosition(Point position)
        {
            var center = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
            var scale = this.Scale * this.drawingScale / this.deviceScale;

            position.X = (position.X - this.Offset.X) * scale + center.X;
            position.Y = -(position.Y - this.Offset.Y) * scale + center.Y;
            position = PointToScreen(position);

            return position;
        }

        private Point ScreenToDrawingPosition(Point position)
        {
            var center = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
            var scale = this.Scale * this.drawingScale / this.deviceScale;

            position = PointFromScreen(position);
            position.X = (position.X - center.X) / scale + this.Offset.X;
            position.Y = -(position.Y - center.Y) / scale + this.Offset.Y;

            return position;
        }

        private void SetValueDisplayDecimals(Point value)
        {
            this.ValueDisplayDecimals = Math.Max(GetDecimalSize(value.X, this.RoundDecimals), GetDecimalSize(value.Y, this.RoundDecimals));
        }

        private double GetStepFactor()
        {
            var smallMatch = this.inputs.SmallStepModifier.Match(PrimaryDeviceInputState.Instance);
            var mediumMatch = this.inputs.MediumStepModifier.Match(PrimaryDeviceInputState.Instance);
            var largeMatch = this.inputs.LargeStepModifier.Match(PrimaryDeviceInputState.Instance);

            return
                largeMatch > mediumMatch && largeMatch > smallMatch ? this.LargeStepFactor :
                mediumMatch > smallMatch ? this.MediumStepFactor :
                smallMatch > 0 ? this.SmallStepFactor : 1.0;
        }

        private static int GetDecimalSize(double value, int maxDecimalSize)
        {
            if (maxDecimalSize <= 0)
            {
                return 0;
            }

            return Math.Min(maxDecimalSize, Math.Max(0, -(int)Math.Floor(Math.Log10(value))));
        }

        private static void DrawRectangle(DrawingContext drawingContext, Brush brush, double x0, double y0, double x1, double y1)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(Math.Round(x0), Math.Round(y0), Math.Round(x1 - x0), Math.Round(y1 - y0)));
        }

        private static Point RoundValue(Point value, int roundDecimals)
        {
            value.X = Math.Round(value.X, roundDecimals);
            value.Y = Math.Round(value.Y, roundDecimals);

            if (Math.Abs(value.X) == 0)
            {
                value.X = 0;
            }

            if (Math.Abs(value.Y) == 0)
            {
                value.Y = 0;
            }

            return value;
        }

        private static double Fract(double value)
        {
            return value - Math.Floor(value);
        }

        private static double Sqr(double value)
        {
            return value * value;
        }

        private static bool TryGetEdgeCursorGeometry(Point position, Size size, [MaybeNullWhen(false)] out Geometry geometry)
        {
            var top = position.Y < -CursorRadius;
            var bottom = position.Y > size.Height + CursorRadius;
            var left = position.X < -CursorRadius;
            var right = position.X > size.Width + CursorRadius;

            geometry =
                top ? (left ? TopLeftEdgeCursorGeometry : (right ? TopRightEdgeCursorGeometry : TopEdgeCursorGeometry)) :
                bottom ? (left ? BottomLeftEdgeCursorGeometry : (right ? BottomRightEdgeCursorGeometry : BottomEdgeCursorGeometry)) :
                left ? LeftEdgeCursorGeometry :
                right ? RightEdgeCursorGeometry :
                null;

            return geometry != null;
        }
    }
}
