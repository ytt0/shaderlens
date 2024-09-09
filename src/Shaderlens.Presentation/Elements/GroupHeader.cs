
namespace Shaderlens.Presentation.Elements
{
    public class GroupHeader : ToggleButton
    {
        private static readonly Geometry ExpandGeometry = Geometry.Parse("M 3,2 7,6 11,2").WithFreeze();
        private static readonly Geometry CollapseGeometry = Geometry.Parse("M 3,6 7,2 11,6").WithFreeze();

        private static readonly object GroupKey = new object();

        public static readonly DependencyProperty IconForegroundProperty = Icon.ForegroundProperty.AddOwner(typeof(GroupHeader), new FrameworkPropertyMetadata((sender, e) => ((GroupHeader)sender).path.Stroke = (Brush)e.NewValue));
        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(GroupHeader), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(GroupHeader), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(GroupHeader));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        protected override int VisualChildrenCount { get { return 2; } }

        private readonly ContentPresenter contentPresenter;
        private readonly Path path;

        public GroupHeader(IApplicationTheme theme)
        {
            this.contentPresenter = new ContentPresenter();
            AddVisualChild(this.contentPresenter);

            this.path = new Path
            {
                Data = CollapseGeometry,
                Stroke = this.IconForeground,
                StrokeThickness = 1.0,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                Margin = new Thickness(4),
                Width = 14,
                Height = 8,
            };
            AddVisualChild(this.path);

            this.Template = null;
            this.Padding = new Thickness(4, 2, 4, 2);
            this.CornerRadius = new CornerRadius(4);
            this.ClickMode = ClickMode.Press;

            SetValue(MultiToggleButtonClickBehavior.GroupKeyProperty, GroupKey);

            theme.IconForeground.SetReference(this, Icon.ForegroundProperty);
            theme.ControlHoveredBackground.SetReference(this, HoverBackgroundProperty);
            theme.ControlPressedBackground.SetReference(this, PressedBackgroundProperty);
            theme.GroupBackground.SetReference(this, BackgroundProperty);
            theme.WindowForeground.SetReference(this, ForegroundProperty);
        }

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? this.contentPresenter :
                index == 1 ? this.path :
                throw new IndexOutOfRangeException();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PaddingProperty)
            {
                this.contentPresenter.Margin = (Thickness)e.NewValue;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            this.contentPresenter.Content = newContent;
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            this.path.Data = CollapseGeometry;
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            this.path.Data = ExpandGeometry;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.path.Measure(constraint);

            constraint.Width = Math.Max(0.0, constraint.Width - this.path.DesiredSize.Width);
            this.contentPresenter.Measure(constraint);

            return this.contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.path.Arrange(new Rect(arrangeBounds.Width - this.path.DesiredSize.Width, (arrangeBounds.Height - this.path.DesiredSize.Height) / 2.0, this.path.DesiredSize.Width, this.path.DesiredSize.Height));
            this.contentPresenter.Arrange(new Rect(0.0, 0.0, Math.Max(0.0, arrangeBounds.Width - this.path.DesiredSize.Width), arrangeBounds.Height));

            return arrangeBounds;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var capturedElement = Mouse.PrimaryDevice.Captured;
            var isMousePressed = Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed;

            var isCaptured = capturedElement == this;
            var isPressed = capturedElement == null && this.IsMouseOver && isMousePressed;
            var isHovered = capturedElement == null && this.IsMouseOver && !isMousePressed;

            var brush = isCaptured || isPressed ? this.PressedBackground : isHovered ? this.HoverBackground : this.Background;

            drawingContext.DrawRoundedRectangle(brush, null, new Rect(this.RenderSize), this.CornerRadius.TopLeft, this.CornerRadius.TopRight);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }
    }
}
