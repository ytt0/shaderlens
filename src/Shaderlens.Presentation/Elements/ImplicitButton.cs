namespace Shaderlens.Presentation.Elements
{
    public class ImplicitButton : Border
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Direct, typeof(MouseButtonEventHandler), typeof(ImplicitButton));
        public event MouseButtonEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            InvalidateVisual();

            if (e.MouseDevice.Captured == null)
            {
                e.MouseDevice.Capture(this);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            InvalidateVisual();

            if (this.IsMouseCaptured)
            {
                e.MouseDevice.Capture(null);

                if (this.IsMouseOver)
                {
                    RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton) { RoutedEvent = ClickEvent });
                }
            }
        }
    }
}
