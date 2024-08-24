namespace Shaderlens.Presentation.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static void SetReference(this FrameworkElement element, DependencyProperty dependencyProperty, IThemeResource themeResource)
        {
            themeResource.SetReference(element, dependencyProperty);
        }

        public static void SetReference(this FrameworkContentElement element, DependencyProperty dependencyProperty, IThemeResource themeResource)
        {
            themeResource.SetReference(element, dependencyProperty);
        }
    }
}
