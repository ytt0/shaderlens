namespace Shaderlens.Presentation.Elements
{
    public interface IColumnSplitSource
    {
        event EventHandler? RatioChanged;
        double Ratio { get; }
    }

    public class ColumnSplitContainer : Decorator, IColumnSplitSource
    {
        public event EventHandler? RatioChanged;
        public double Ratio
        {
            get { return this.splitterHandle.Ratio; }
            set { this.splitterHandle.Ratio = value; }
        }

        protected override int VisualChildrenCount { get { return (this.Child != null ? 1 : 0) + 1; } }

        public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.Register("HoverBrush", typeof(Brush), typeof(ColumnSplitContainer), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(15, 0, 0, 0)),
            (sender, e) => ((ColumnSplitContainer)sender).splitterHandle.HoverBrush = (Brush)e.NewValue));

        public Brush HoverBrush
        {
            get { return (Brush)GetValue(HoverBrushProperty); }
            set { SetValue(HoverBrushProperty, value); }
        }

        private readonly SplitterHandle splitterHandle;

        public ColumnSplitContainer()
        {
            this.splitterHandle = new SplitterHandle();
            this.splitterHandle.RatioChanged += (sender, e) => RatioChanged?.Invoke(this, e);

            AddVisualChild(this.splitterHandle);
        }

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? this.Child ?? this.splitterHandle :
                index == 1 && this.Child != null ? this.splitterHandle :
                throw new ArgumentOutOfRangeException(nameof(index));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.splitterHandle.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            this.splitterHandle.Arrange(new Rect(arrangeSize));
            return base.ArrangeOverride(arrangeSize);
        }
    }
}
