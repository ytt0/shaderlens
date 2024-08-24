namespace Shaderlens.Presentation.Extensions
{
    public static class UIElementExtensions
    {
        public static T WithHandler<T>(this T target, RoutedEvent rountedEvent, RoutedEventHandler handler) where T : IInputElement
        {
            target.AddHandler(rountedEvent, handler);
            return target;
        }

        public static T WithValue<T>(this T target, DependencyProperty dependencyProperty, object value) where T : DependencyObject
        {
            target.SetValue(dependencyProperty, value);
            return target;
        }

        public static T WithDock<T>(this T target, Dock value) where T : DependencyObject
        {
            target.SetValue(DockPanel.DockProperty, value);
            return target;
        }

        public static T WithChildren<T>(this T target, params UIElement?[] children) where T : Panel
        {
            foreach (var child in children)
            {
                if (child != null)
                {
                    target.Children.Add(child);
                }
            }

            return target;
        }

        public static T WithItems<T>(this T target, params object[] items) where T : ItemsControl
        {
            foreach (var item in items)
            {
                target.Items.Add(item);
            }

            return target;
        }

        public static T WithReference<T>(this T target, DependencyProperty dependencyProperty, IThemeResource themeResource) where T : FrameworkElement
        {
            themeResource.SetReference(target, dependencyProperty);
            return target;
        }

        public static T WithContentReference<T>(this T target, DependencyProperty dependencyProperty, IThemeResource themeResource) where T : FrameworkContentElement
        {
            themeResource.SetReference(target, dependencyProperty);
            return target;
        }

        public static T? GetAncestor<T>(this DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element);

                if (element is T target)
                {
                    return target;
                }
            }

            return default;
        }
    }
}
