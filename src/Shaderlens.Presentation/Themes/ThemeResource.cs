namespace Shaderlens.Presentation.Themes
{
    public interface IThemeResource
    {
        void SetResource(ResourceDictionary target);
        void SetReference(FrameworkElement target, DependencyProperty property);
        void SetReference(FrameworkContentElement target, DependencyProperty property);
    }

    public interface IThemeResource<T> : IThemeResource
    {
        T Value { get; }
    }

    public class ThemeResource<T> : IThemeResource<T>
    {
        public T Value { get; }

        private readonly string key;

        public ThemeResource(string key, T value)
        {
            this.key = key;
            this.Value = value;
        }

        public void SetResource(ResourceDictionary target)
        {
            target[this.key] = this.Value;
        }

        public void SetReference(FrameworkElement target, DependencyProperty property)
        {
            target.SetResourceReference(property, this.key);
        }

        public void SetReference(FrameworkContentElement target, DependencyProperty property)
        {
            target.SetResourceReference(property, this.key);
        }
    }
}
