namespace Shaderlens.Presentation.Behaviors
{
    public class MouseHoverKeyEventArgs : KeyEventArgs
    {
        public MouseHoverKeyEventArgs(KeyEventArgs e) :
            base(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key)
        {
            this.RoutedEvent = e.RoutedEvent;
            this.Source = e.Source;
        }
    }

    public class MouseHoverKeyEventBehavior : IDisposable
    {
        private readonly FrameworkElement target;

        private MouseHoverKeyEventBehavior(FrameworkElement target)
        {
            target.PreviewKeyDown += OnPreviewKeyDown;
            target.KeyDown += OnKeyDown;
            this.target = target;
        }

        public void Dispose()
        {
            this.target.PreviewKeyDown -= OnPreviewKeyDown;
            this.target.KeyDown -= OnKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e is not MouseHoverKeyEventArgs)
            {
                var e2 = new MouseHoverKeyEventArgs(e);

                var target = GetMouseTarget();
                if (target != null)
                {
                    target.RaiseEvent(e2);
                    e.Handled = e2.Handled;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e is not MouseHoverKeyEventArgs)
            {
                var e2 = new MouseHoverKeyEventArgs(e);

                var target = GetMouseTarget();
                if (target != null)
                {
                    target.RaiseEvent(e2);
                    e.Handled = e2.Handled;
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
