namespace Shaderlens.Presentation.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> GetVisualDescendants(this FrameworkElement root, int maxDepth)
        {
            root.ApplyTemplate();

            var queue = new Queue<(int Depth, DependencyObject Element)>();
            queue.Enqueue((0, root));

            while (queue.TryDequeue(out var entry) && entry.Depth < maxDepth)
            {
                yield return entry.Element;

                if (entry.Element is FrameworkElement frameworkElement)
                {
                    frameworkElement.ApplyTemplate();
                }

                var count = VisualTreeHelper.GetChildrenCount(entry.Element);
                for (var i = 0; i < count; i++)
                {
                    queue.Enqueue((entry.Depth + 1, VisualTreeHelper.GetChild(entry.Element, i)));
                }
            }
        }

        public static IEnumerable<DependencyObject> GetLogicalDescendants(this FrameworkElement root, int maxDepth)
        {
            root.ApplyTemplate();

            var queue = new Queue<(int Depth, DependencyObject Element)>();
            queue.Enqueue((0, root));

            while (queue.TryDequeue(out var entry) && entry.Depth < maxDepth)
            {
                yield return entry.Element;

                if (entry.Element is FrameworkElement frameworkElement)
                {
                    frameworkElement.ApplyTemplate();
                }

                foreach (var child in LogicalTreeHelper.GetChildren(entry.Element).OfType<DependencyObject>())
                {
                    queue.Enqueue((entry.Depth + 1, child));
                }
            }
        }
    }
}
