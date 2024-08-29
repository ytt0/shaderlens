namespace Shaderlens.Presentation.Behaviors
{
    public class MouseHoverKeyEventBehavior : IDisposable
    {
        private readonly FrameworkElement source;
        private bool isHandled;

        private MouseHoverKeyEventBehavior(FrameworkElement source)
        {
            this.source = source;

            this.source.PreviewKeyDown += OnPreviewKeyDown;
            this.source.PreviewKeyUp += OnPreviewKeyUp;
        }

        public void Dispose()
        {
            this.source.PreviewKeyDown -= OnPreviewKeyDown;
            this.source.PreviewKeyUp -= OnPreviewKeyUp;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnPreviewKeyEvent(e, UIElement.KeyDownEvent);
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            OnPreviewKeyEvent(e, UIElement.KeyUpEvent);
        }

        private void OnPreviewKeyEvent(KeyEventArgs e, RoutedEvent routedEvent)
        {
            if (!this.isHandled)
            {
                this.isHandled = true;

                try
                {
                    var target = GetMouseTarget();
                    if (target != null)
                    {
                        var previewRoutedEvent = e.RoutedEvent;
                        target.RaiseEvent(e);

                        e.RoutedEvent = routedEvent;
                        target.RaiseEvent(e);

                        e.RoutedEvent = previewRoutedEvent;
                        e.Handled |= target.GetAncestor<Window>() == e.InputSource.RootVisual;
                    }
                }
                finally
                {
                    this.isHandled = false;
                }
            }
        }

        private static FrameworkElement? GetMouseTarget()
        {
            var keyboardElement = Keyboard.FocusedElement as FrameworkElement;
            if (keyboardElement is TextBoxBase || keyboardElement?.GetAncestor<TextBoxBase>() != null)
            {
                return null;
            }

            var target = Mouse.DirectlyOver as FrameworkElement;

            var textBoxAncestor = target?.GetAncestor<TextBoxBase>();
            if (textBoxAncestor != null)
            {
                return textBoxAncestor.GetAncestor<FrameworkElement>();
            }

            return target;
        }

        public static IDisposable Register(FrameworkElement target)
        {
            return new MouseHoverKeyEventBehavior(target);
        }
    }
}
