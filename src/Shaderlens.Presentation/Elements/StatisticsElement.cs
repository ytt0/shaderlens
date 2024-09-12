namespace Shaderlens.Presentation.Elements
{
    public class StatisticsElement : FrameworkElement
    {
        private const double Spacing = 10.0;
        private const double LineHeight = 1.6;
        private const double GraphVericalMargin = 0.1;
        private const double GraphWidth = 120.0;
        private const int GraphValues = 60;

        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(StatisticsElement));
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(StatisticsElement));
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        private readonly StatisticsFormatter formatter;
        private readonly TextBlock textBlock;
        private readonly StatisticsGraphElement frameRateGraph;
        private readonly StatisticsGraphElement frameTimeGraph;
        private readonly StatisticsGraphElement utilizationGraph;
        private readonly StatisticsGraphElement maxRateGraph;
        private double graphHeight;
        private double maxTextBoxWidth;

        protected override int VisualChildrenCount { get { return 5; } }

        public StatisticsElement(IApplicationTheme theme)
        {
            this.formatter = new StatisticsFormatter(Environment.NewLine);
            this.textBlock = new TextBlock();

            this.frameRateGraph = CreateGraphElement(theme);
            this.frameTimeGraph = CreateGraphElement(theme);
            this.utilizationGraph = CreateGraphElement(theme);
            this.maxRateGraph = CreateGraphElement(theme);

            this.utilizationGraph.MaxValue = 1.0;

            AddVisualChild(this.textBlock);
            AddVisualChild(this.frameRateGraph);
            AddVisualChild(this.frameTimeGraph);
            AddVisualChild(this.utilizationGraph);
            AddVisualChild(this.maxRateGraph);
        }

        public void SetStatisticsValues(FrameIndex frameIndex, double rate, double average, int bufferWidth, int bufferHeight, bool includeMilliseconds)
        {
            if (this.IsVisible)
            {
                var formattedStatistics = this.formatter.GetStatistics(frameIndex, rate, average, bufferWidth, bufferHeight, includeMilliseconds);
                this.textBlock.Text = formattedStatistics;

                var graphVisibility = average > 0 ? Visibility.Visible : Visibility.Collapsed;
                this.frameRateGraph.Visibility = graphVisibility;
                this.frameTimeGraph.Visibility = graphVisibility;
                this.utilizationGraph.Visibility = graphVisibility;
                this.maxRateGraph.Visibility = graphVisibility;
            }

            if (average > 0)
            {
                this.frameRateGraph.SetNextValue(rate);
                this.frameTimeGraph.SetNextValue(average);
                this.utilizationGraph.SetNextValue(Math.Min(1.0, rate * average / TimeSpan.TicksPerSecond));
                this.maxRateGraph.SetNextValue(TimeSpan.TicksPerSecond / average);
            }
        }

        public void ClearStatisticsValues()
        {
            this.frameRateGraph.ClearValues();
            this.frameTimeGraph.ClearValues();
            this.utilizationGraph.ClearValues();
            this.maxRateGraph.ClearValues();
            this.maxTextBoxWidth = 0.0;
        }

        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0: return this.textBlock;
                case 1: return this.frameRateGraph;
                case 2: return this.frameTimeGraph;
                case 3: return this.utilizationGraph;
                case 4: return this.maxRateGraph;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var lineHeight = this.FontSize * LineHeight;

            var margin = GraphVericalMargin * this.textBlock.FontSize;
            this.textBlock.Measure(new Size(Double.MaxValue, Double.MaxValue));
            this.graphHeight = lineHeight - margin - margin;
            this.maxTextBoxWidth = Math.Max(this.maxTextBoxWidth, this.textBlock.DesiredSize.Width);

            var graphSize = new Size(GraphWidth, lineHeight);
            this.frameRateGraph.Measure(graphSize);
            this.frameTimeGraph.Measure(graphSize);
            this.utilizationGraph.Measure(graphSize);
            this.maxRateGraph.Measure(graphSize);

            return new Size(this.maxTextBoxWidth + Spacing + GraphWidth, this.textBlock.DesiredSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var lineHeight = this.FontSize * LineHeight;

            var textBlockOffset = 0.5 * (LineHeight - 1.0);
            textBlockOffset += this.FontFamily.Baseline - this.FontFamily.LineSpacing;

            var margin = GraphVericalMargin * this.textBlock.FontSize;
            this.textBlock.Arrange(new Rect(new Point(0, this.FontSize * textBlockOffset), this.textBlock.DesiredSize));
            this.frameRateGraph.Arrange(new Rect(finalSize.Width - GraphWidth, 2 * lineHeight + margin, GraphWidth, this.graphHeight));
            this.frameTimeGraph.Arrange(new Rect(finalSize.Width - GraphWidth, 3 * lineHeight + margin, GraphWidth, this.graphHeight));
            this.utilizationGraph.Arrange(new Rect(finalSize.Width - GraphWidth, 4 * lineHeight + margin, GraphWidth, this.graphHeight));
            this.maxRateGraph.Arrange(new Rect(finalSize.Width - GraphWidth, 5 * lineHeight + margin, GraphWidth, this.graphHeight));

            return finalSize;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FontSizeProperty)
            {
                this.textBlock.LineHeight = (double)e.NewValue * LineHeight;
            }
        }

        private static StatisticsGraphElement CreateGraphElement(IApplicationTheme theme)
        {
            return new StatisticsGraphElement(GraphValues) { Visibility = Visibility.Collapsed }.
                WithReference(StatisticsGraphElement.StrokeProperty, theme.StatisticsGraphStroke).
                WithReference(StatisticsGraphElement.FillProperty, theme.StatisticsGraphFill);
        }
    }
}
