namespace Shaderlens.Presentation.Elements
{
    public class GroupHeader : ImplicitButton
    {
        private const double DrawingSize = 6;

        protected override int VisualChildrenCount { get { return base.VisualChildrenCount + 1; } }

        public static readonly DependencyProperty IconForegroundProperty = Icon.ForegroundProperty.AddOwner(typeof(GroupHeader), new FrameworkPropertyMetadata((sender, e) => ((GroupHeader)sender).path.Stroke = (Brush)e.NewValue));
        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                this.isExpanded = value;
                this.path.Data = this.isExpanded ? CollapseGeometry : ExpandGeometry;
            }
        }

        private static readonly Geometry ExpandGeometry = Geometry.Parse("M 3,2 7,6 11,2").WithFreeze();
        private static readonly Geometry CollapseGeometry = Geometry.Parse("M 3,6 7,2 11,6").WithFreeze();
        private readonly Path path;

        public GroupHeader(IApplicationTheme theme) :
            base(theme)
        {
            this.isExpanded = true;
            this.Padding = new Thickness(4, 2, 4, 2);
            this.CornerRadius = new CornerRadius(4);

            this.path = new Path
            {
                Data = CollapseGeometry,
                Stroke = this.IconForeground,
                StrokeThickness = 1.0,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                Width = 14,
                Height = 8,
            };

            AddVisualChild(this.path);

            theme.GroupBackground.SetReference(this, BackgroundProperty);
        }

        protected override Visual GetVisualChild(int index)
        {
            return index == this.VisualChildrenCount - 1 ? this.path : base.GetVisualChild(index);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var horizontalPadding = this.Padding.Left + this.Padding.Right;
            var verticalPadding = this.Padding.Top + this.Padding.Bottom;
            this.path.Measure(constraint);
            var size = base.MeasureOverride(new Size(Math.Max(0, constraint.Width - DrawingSize - horizontalPadding), Math.Max(0, constraint.Height - verticalPadding)));
            return new Size(size.Width + DrawingSize + horizontalPadding, size.Height + verticalPadding);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var horizontalPadding = this.Padding.Left + this.Padding.Right;
            var verticalPadding = this.Padding.Top + this.Padding.Bottom;
            this.path.Arrange(new Rect(arrangeSize.Width - this.path.DesiredSize.Width - this.Padding.Right, (arrangeSize.Height - this.path.DesiredSize.Height) / 2, this.path.DesiredSize.Width, this.path.DesiredSize.Height));
            this.VisualOffset = new System.Windows.Vector(this.Padding.Left, this.Padding.Top);
            base.ArrangeOverride(new Size(Math.Max(0, arrangeSize.Width - arrangeSize.Height - horizontalPadding), Math.Max(0, arrangeSize.Height - verticalPadding)));
            return arrangeSize;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            InvalidateVisual();
        }
    }
}
