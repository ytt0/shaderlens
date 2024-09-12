namespace Shaderlens.Presentation.Elements
{
    public class StatisticsGraphElement : FrameworkElement
    {
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Color), typeof(StatisticsGraphElement), new PropertyMetadata(Colors.Red, (sender, e) => ((StatisticsGraphElement)sender).OnStrokeChanged(e)));
        public Color Stroke
        {
            get { return (Color)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Color), typeof(StatisticsGraphElement), new PropertyMetadata(Colors.Red, (sender, e) => ((StatisticsGraphElement)sender).OnFillChanged(e)));
        public Color Fill
        {
            get { return (Color)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(StatisticsGraphElement), new PropertyMetadata(0.75, (sender, e) => ((StatisticsGraphElement)sender).OnThicknessChanged(e)));
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public double MaxValue { get; set; }

        private readonly PolyLineSegment topSegment;
        private readonly PolyLineSegment bottomSegment;
        private readonly PathFigure figure;
        private readonly PathGeometry geometry;
        private readonly double[] values;
        private int nextIndex;
        private Brush brush;
        private readonly Pen pen;

        public StatisticsGraphElement(int valuesCount)
        {
            this.values = new double[valuesCount];
            this.topSegment = new PolyLineSegment(Enumerable.Range(0, this.values.Length).Select(i => new Point()).ToArray(), true) { IsSmoothJoin = true };
            this.bottomSegment = new PolyLineSegment(new[] { new Point(), new Point() }, false);
            this.figure = new PathFigure(new Point(), new PathSegment[] { this.topSegment, this.bottomSegment }, false);
            this.geometry = new PathGeometry(new[] { this.figure });
            this.MaxValue = Double.NaN;

            this.brush = Brushes.Red;
            this.pen = new Pen();
            this.Thickness = 0.75;
        }

        public void SetNextValue(double value)
        {
            this.values[this.nextIndex] = value;
            this.nextIndex = (this.nextIndex + 1) % this.values.Length;

            if (this.IsVisible)
            {
                InvalidateVisual();
            }
        }

        public void ClearValues()
        {
            Array.Clear(this.values);
            this.nextIndex = 0;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var spacing = this.RenderSize.Width / (this.values.Length - 1);
            var maxValue = Math.Max(1.0, Double.IsNaN(this.MaxValue) ? this.values.Max() : this.MaxValue);

            for (var i = 0; i < this.values.Length; i++)
            {
                var value = this.values[(this.nextIndex + i) % this.values.Length];
                this.topSegment.Points[i] = new Point(i * spacing, this.RenderSize.Height * (1.0 - value / maxValue));
            }

            this.figure.StartPoint = this.topSegment.Points[0];

            this.bottomSegment.Points[0] = new Point(this.RenderSize.Width, this.RenderSize.Height);
            this.bottomSegment.Points[1] = new Point(0, this.RenderSize.Height);

            drawingContext.DrawGeometry(this.brush, this.pen, this.geometry);
        }

        private void OnStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.pen.Brush = new SolidColorBrush((Color)e.NewValue);
        }

        private void OnFillChanged(DependencyPropertyChangedEventArgs e)
        {
            var startColor = (Color)e.NewValue;
            var endColor = startColor;
            endColor.A = (byte)(endColor.A * 0.5);

            this.brush = new LinearGradientBrush(startColor, endColor, 90);
        }

        private void OnThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            this.pen.Thickness = (double)e.NewValue;
        }
    }
}
