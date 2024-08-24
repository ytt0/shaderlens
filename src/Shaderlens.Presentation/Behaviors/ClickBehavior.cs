namespace Shaderlens.Presentation.Behaviors
{
    public class ClickBehavior : IDisposable
    {
        private readonly FrameworkElement element;
        private readonly RoutedEventHandler handler;

        private ClickBehavior(FrameworkElement element, RoutedEventHandler handler)
        {
            this.element = element;
            this.handler = handler;
            this.element.MouseDown += OnMouseDown;
            this.element.MouseUp += OnMouseUp;
        }

        public void Dispose()
        {
            this.element.MouseDown -= OnMouseDown;
            this.element.MouseUp -= OnMouseUp;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.Captured == null)
            {
                e.MouseDevice.Capture(this.element);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.element.IsMouseCaptured)
            {
                e.MouseDevice.Capture(null);

                if (this.element.IsMouseOver)
                {
                    this.handler(this.element, e);
                }
            }
        }

        public static IDisposable Register(FrameworkElement element, RoutedEventHandler handler)
        {
            return new ClickBehavior(element, handler);
        }
    }
}
