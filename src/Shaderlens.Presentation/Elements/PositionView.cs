namespace Shaderlens.Presentation.Elements
{
    public class PositionView : ImplicitButton
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Vector<double>), typeof(PositionView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Vector<double> Value
        {
            get { return (Vector<double>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty NormalBoundsProperty = DependencyProperty.Register("NormalBounds", typeof(bool), typeof(PositionView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public bool NormalBounds
        {
            get { return (bool)GetValue(NormalBoundsProperty); }
            set { SetValue(NormalBoundsProperty, value); }
        }

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(Vector<double>), typeof(PositionView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Vector<double> Step
        {
            get { return (Vector<double>)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        private readonly Pen arrowPen;
        private readonly Pen borderPen;

        public PositionView(IApplicationTheme theme) :
            base(theme)
        {
            this.arrowPen = new Pen(null, 1.0) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            this.borderPen = new Pen(null, 1.0) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };

            this.SnapsToDevicePixels = false;

            theme.CodeFontFamily.SetReference(this, TextElement.FontFamilyProperty);
            theme.CodeFontSize.SetReference(this, TextElement.FontSizeProperty);
            theme.ControlBorder.SetReference(this, BorderBrushProperty);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(finalSize.Height, finalSize.Height);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var size = this.RenderSize.Height;
            var center = new Point(size / 2, size / 2);

            this.arrowPen.Brush = (Brush)GetValue(Icon.ForegroundProperty);
            this.borderPen.Brush = (Brush)GetValue(BorderBrushProperty);

            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));

            var x = 0.0;
            var y = 0.0;

            if (this.Value?.Count >= 2)
            {
                x = this.Value[0];
                y = -this.Value[1];
            }

            if (this.NormalBounds)
            {
                drawingContext.DrawEllipse(null, this.borderPen, center, size / 2, size / 2);

                var length = Math.Sqrt(x * x + y * y);

                if (length < 0.0001)
                {
                    drawingContext.DrawEllipse(null, this.arrowPen, new Point(center.X, center.Y), 2, 2);
                }
                else
                {
                    x /= length;
                    y /= length;

                    x = x * size * 0.3;
                    y = y * size * 0.3;

                    drawingContext.DrawLine(this.arrowPen, new Point(-x + center.X, -y + center.Y), new Point(x + center.X, y + center.Y));
                    drawingContext.DrawLine(this.arrowPen, new Point(0.3 * x - 0.4 * y + center.X, 0.3 * y + 0.4 * x + center.Y), new Point(x + center.X, y + center.Y));
                    drawingContext.DrawLine(this.arrowPen, new Point(0.3 * x + 0.4 * y + center.X, 0.3 * y - 0.4 * x + center.Y), new Point(x + center.X, y + center.Y));
                }
            }
            else
            {
                drawingContext.DrawRectangle(null, this.borderPen, new Rect(0, 0, size, size));

                drawingContext.DrawLine(this.borderPen, new Point(size * 0.35, 2), new Point(size * 0.35, size - 2));
                drawingContext.DrawLine(this.borderPen, new Point(size * 0.65, 2), new Point(size * 0.65, size - 2));
                drawingContext.DrawLine(this.borderPen, new Point(2, size * 0.35), new Point(size - 2, size * 0.35));
                drawingContext.DrawLine(this.borderPen, new Point(2, size * 0.65), new Point(size - 2, size * 0.65));

                var length = Math.Sqrt(x * x + y * y);

                if (length < 0.0001)
                {
                    x = 0;
                    y = 0;
                }
                else
                {
                    x /= length;
                    y /= length;
                }

                x = x * size * 0.3;
                y = y * size * 0.3;

                drawingContext.DrawEllipse(null, this.arrowPen, new Point(x + center.X, y + center.Y), 2, 2);
            }
        }
    }
}
