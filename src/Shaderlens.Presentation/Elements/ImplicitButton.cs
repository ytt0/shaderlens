namespace Shaderlens.Presentation.Elements
{
    public class ImplicitButton : ButtonBase
    {
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(ImplicitButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(ImplicitButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(ImplicitButton));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        protected override int VisualChildrenCount { get { return 1; } }

        private readonly ContentPresenter contentPresenter;

        public ImplicitButton(IApplicationTheme theme)
        {
            this.contentPresenter = new ContentPresenter();
            AddVisualChild(this.contentPresenter);

            this.Background = Brushes.Transparent;
            this.Template = null;
            this.FocusVisualStyle = null;
            this.Focusable = false;

            theme.IconForeground.SetReference(this, Icon.ForegroundProperty);
            theme.ControlHoveredBackground.SetReference(this, HoverBackgroundProperty);
            theme.ControlPressedBackground.SetReference(this, PressedBackgroundProperty);
        }

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? this.contentPresenter : throw new IndexOutOfRangeException();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PaddingProperty)
            {
                this.contentPresenter.Margin = (Thickness)e.NewValue;
            }

            if (e.Property == BackgroundProperty)
            {
                InvalidateVisual();
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            this.contentPresenter.Content = newContent;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.contentPresenter.Measure(constraint);
            return this.contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.contentPresenter.Arrange(new Rect(arrangeBounds));
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
