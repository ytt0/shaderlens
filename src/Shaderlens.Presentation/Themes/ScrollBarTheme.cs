namespace Shaderlens.Presentation.Themes
{
    public interface IScrollBarTheme : IThemeResources
    {
        public IThemeResource<Brush> Track { get; }
        public IThemeResource<Brush> Thumb { get; }
        public IThemeResource<Brush> Arrow { get; }
    }

    public class LightScrollBarTheme : IScrollBarTheme
    {
        public IThemeResource<Brush> Track { get; }
        public IThemeResource<Brush> Thumb { get; }
        public IThemeResource<Brush> Arrow { get; }

        private readonly ThemeResources resources;

        public LightScrollBarTheme(IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "ScrollBar");

            var brushSerializer = new BrushJsonSerializer();

            this.Track = this.resources.AddResource(nameof(this.Track), brushSerializer, new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)));
            this.Thumb = this.resources.AddResource(nameof(this.Thumb), brushSerializer, new SolidColorBrush(Color.FromRgb(225, 225, 225)));
            this.Arrow = this.resources.AddResource(nameof(this.Arrow), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 0, 0)));
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }

    public class DarkScrollBarTheme : IScrollBarTheme
    {
        public IThemeResource<Brush> Track { get; }
        public IThemeResource<Brush> Thumb { get; }
        public IThemeResource<Brush> Arrow { get; }

        private readonly ThemeResources resources;

        public DarkScrollBarTheme(IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "ScrollBar");

            var brushSerializer = new BrushJsonSerializer();

            this.Track = this.resources.AddResource(nameof(this.Track), brushSerializer, new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)));
            this.Thumb = this.resources.AddResource(nameof(this.Thumb), brushSerializer, new SolidColorBrush(Color.FromRgb(80, 80, 80)));
            this.Arrow = this.resources.AddResource(nameof(this.Arrow), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 255, 255)));
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }
}
