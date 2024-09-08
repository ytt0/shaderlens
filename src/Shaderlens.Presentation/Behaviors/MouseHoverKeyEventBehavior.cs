namespace Shaderlens.Presentation.Behaviors
{
    public class MouseHoverKeyEventBehavior : IDisposable
    {
        private static readonly Lazy<MethodInfo?> InvokeHandlersMethodInfo = new Lazy<MethodInfo?>(() => typeof(EventRoute).GetMethod("InvokeHandlers", BindingFlags.NonPublic | BindingFlags.Instance));

        private readonly FrameworkElement source;

        private MouseHoverKeyEventBehavior(FrameworkElement source)
        {
            this.source = source;

            this.source.PreviewKeyDown += OnKeyEvent;
            this.source.PreviewKeyUp += OnKeyEvent;
            this.source.KeyDown += OnKeyEvent;
            this.source.KeyUp += OnKeyEvent;
        }

        public void Dispose()
        {
            this.source.PreviewKeyDown -= OnKeyEvent;
            this.source.PreviewKeyUp -= OnKeyEvent;
            this.source.KeyDown -= OnKeyEvent;
            this.source.KeyUp -= OnKeyEvent;
        }

        private void OnKeyEvent(object sender, KeyEventArgs e)
        {
            var target = GetMouseTarget();
            if (target == null)
            {
                return;
            }

            var mouseTargetPath = GetPathFromRoot(target);
            var originalSourcePath = GetPathFromRoot(e.OriginalSource as FrameworkElement);
            var divergencePath = mouseTargetPath.Skip(GetDivergenceIndex(mouseTargetPath, originalSourcePath)).Reverse().ToArray();

            var eventRoute = new EventRoute(e.RoutedEvent);
            foreach (var element in divergencePath)
            {
                element.AddToEventRoute(eventRoute, e);
            }

            var targetArgs = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key) { RoutedEvent = e.RoutedEvent, Source = target };
            InvokeHandlers(eventRoute, target, targetArgs);

            e.Handled = targetArgs.Handled;
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

        private static List<FrameworkElement> GetPathFromRoot(FrameworkElement? target)
        {
            var path = new List<FrameworkElement>();

            while (target != null && path.Count < 1000)
            {
                path.Add(target);
                target = target.Parent as FrameworkElement;
            }

            path.Reverse();

            return path;
        }

        private static int GetDivergenceIndex(IReadOnlyList<FrameworkElement> pathFromRoot1, IReadOnlyList<FrameworkElement> pathFromRoot2)
        {
            var i = 0;
            while (i < pathFromRoot1.Count && i < pathFromRoot2.Count)
            {
                if (pathFromRoot1[i] != pathFromRoot2[i])
                {
                    return i;
                }

                i++;
            }

            return i;
        }

        private static void InvokeHandlers(EventRoute eventRoute, object source, RoutedEventArgs args)
        {
            InvokeHandlersMethodInfo.Value?.Invoke(eventRoute, new[] { source, args });
        }

        public static IDisposable Register(FrameworkElement target)
        {
            return new MouseHoverKeyEventBehavior(target);
        }
    }
}
