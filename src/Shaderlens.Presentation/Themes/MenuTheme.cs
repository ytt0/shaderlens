namespace Shaderlens.Presentation.Themes
{
    public interface IMenuTheme : IThemeResources
    {
        IThemeResource<Brush> Background { get; }
        IThemeResource<Brush> Foreground { get; }
        IThemeResource<Brush> Border { get; }
        IThemeResource<Brush> HighlightBackground { get; }
        IThemeResource<Brush> HeaderBackground { get; }
        IThemeResource<Brush> HeaderForeground { get; }
        IThemeResource<Brush> SelectionBackground { get; }
        IThemeResource<Brush> IconForeground { get; }
        IThemeResource<Brush> GestureForeground { get; }
        IThemeResource<Brush> Separator { get; }
        IThemeResource<Brush> Arrow { get; }
        IThemeResource<Brush> Checkmark { get; }
        IThemeResource<Brush> ValueBorder { get; }
        IThemeResource<double> Opacity { get; }
        IThemeResource<double> FontSize { get; }
        IThemeResource<FontFamily> FontFamily { get; }
        IThemeResource<double> CodeFontSize { get; }
        IThemeResource<FontFamily> CodeFontFamily { get; }
    }

    public class LightMenuTheme : IMenuTheme
    {
        public IThemeResource<Brush> Background { get; }
        public IThemeResource<Brush> Foreground { get; }
        public IThemeResource<Brush> Border { get; }
        public IThemeResource<Brush> HighlightBackground { get; }
        public IThemeResource<Brush> HeaderBackground { get; }
        public IThemeResource<Brush> HeaderForeground { get; }
        public IThemeResource<Brush> SelectionBackground { get; }
        public IThemeResource<Brush> IconForeground { get; }
        public IThemeResource<Brush> GestureForeground { get; }
        public IThemeResource<Brush> Separator { get; }
        public IThemeResource<Brush> Arrow { get; }
        public IThemeResource<Brush> Checkmark { get; }
        public IThemeResource<Brush> ValueBorder { get; }
        public IThemeResource<double> Opacity { get; }
        public IThemeResource<double> FontSize { get; }
        public IThemeResource<FontFamily> FontFamily { get; }
        public IThemeResource<double> CodeFontSize { get; }
        public IThemeResource<FontFamily> CodeFontFamily { get; }

        private readonly ThemeResources resources;

        public LightMenuTheme(IApplicationTheme applicationTheme, IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "Menu");

            var brushSerializer = new BrushJsonSerializer();
            var doubleSerializer = new DoubleJsonSerializer(2);

            this.Background = this.resources.AddResource(nameof(this.Background), brushSerializer, Brushes.White);
            this.Foreground = this.resources.AddResource(nameof(this.Foreground), brushSerializer, Brushes.Black);
            this.Border = this.resources.AddResource(nameof(this.Border), brushSerializer, Brushes.DarkGray);
            this.HighlightBackground = this.resources.AddResource(nameof(this.HighlightBackground), brushSerializer, new SolidColorBrush(Color.FromArgb(10, 0, 0, 0)));
            this.HeaderBackground = this.resources.AddResource(nameof(this.HeaderBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(250, 250, 250)));
            this.HeaderForeground = this.resources.AddResource(nameof(this.HeaderForeground), brushSerializer, Brushes.Black);
            this.SelectionBackground = this.resources.AddResource(nameof(this.SelectionBackground), brushSerializer, Brushes.Gray);
            this.IconForeground = this.resources.AddResource(nameof(this.IconForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.GestureForeground = this.resources.AddResource(nameof(this.GestureForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
            this.Separator = this.resources.AddResource(nameof(this.Separator), brushSerializer, new SolidColorBrush(Color.FromRgb(200, 200, 200)));
            this.Arrow = this.resources.AddResource(nameof(this.Arrow), brushSerializer, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
            this.Checkmark = this.resources.AddResource(nameof(this.Checkmark), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 0, 0)));
            this.ValueBorder = this.resources.AddResource(nameof(this.ValueBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.Opacity = this.resources.AddResource(nameof(this.Opacity), doubleSerializer, 0.95);
            this.FontSize = this.resources.AddResource(applicationTheme.WindowFontSize);
            this.FontFamily = this.resources.AddResource(applicationTheme.WindowFontFamily);
            this.CodeFontSize = this.resources.AddResource(applicationTheme.CodeFontSize);
            this.CodeFontFamily = this.resources.AddResource(applicationTheme.CodeFontFamily);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }

    public class DarkMenuTheme : IMenuTheme
    {
        public IThemeResource<Brush> Background { get; }
        public IThemeResource<Brush> Foreground { get; }
        public IThemeResource<Brush> Border { get; }
        public IThemeResource<Brush> HighlightBackground { get; }
        public IThemeResource<Brush> HeaderBackground { get; }
        public IThemeResource<Brush> HeaderForeground { get; }
        public IThemeResource<Brush> SelectionBackground { get; }
        public IThemeResource<Brush> IconForeground { get; }
        public IThemeResource<Brush> GestureForeground { get; }
        public IThemeResource<Brush> Separator { get; }
        public IThemeResource<Brush> Arrow { get; }
        public IThemeResource<Brush> Checkmark { get; }
        public IThemeResource<Brush> ValueBorder { get; }
        public IThemeResource<double> Opacity { get; }
        public IThemeResource<double> FontSize { get; }
        public IThemeResource<FontFamily> FontFamily { get; }
        public IThemeResource<double> CodeFontSize { get; }
        public IThemeResource<FontFamily> CodeFontFamily { get; }

        private readonly ThemeResources resources;

        public DarkMenuTheme(IApplicationTheme applicationTheme, IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "Menu");

            var brushSerializer = new BrushJsonSerializer();
            var doubleSerializer = new DoubleJsonSerializer(2);

            this.Background = this.resources.AddResource(nameof(this.Background), brushSerializer, new SolidColorBrush(Color.FromRgb(32, 32, 32)));
            this.Foreground = this.resources.AddResource(nameof(this.Foreground), brushSerializer, Brushes.White);
            this.Border = this.resources.AddResource(nameof(this.Border), brushSerializer, new SolidColorBrush(Color.FromRgb(57, 57, 57)));
            this.HighlightBackground = this.resources.AddResource(nameof(this.HighlightBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(57, 57, 57)));
            this.HeaderBackground = this.resources.AddResource(nameof(this.HeaderBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(20, 20, 20)));
            this.HeaderForeground = this.resources.AddResource(nameof(this.HeaderForeground), brushSerializer, Brushes.White);
            this.SelectionBackground = this.resources.AddResource(nameof(this.SelectionBackground), brushSerializer, Brushes.Gray);
            this.IconForeground = this.resources.AddResource(nameof(this.IconForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 180, 180)));
            this.GestureForeground = this.resources.AddResource(nameof(this.GestureForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.Separator = this.resources.AddResource(nameof(this.Separator), brushSerializer, new SolidColorBrush(Color.FromRgb(57, 57, 57)));
            this.Arrow = this.resources.AddResource(nameof(this.Arrow), brushSerializer, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
            this.Checkmark = this.resources.AddResource(nameof(this.Checkmark), brushSerializer, Brushes.White);
            this.ValueBorder = this.resources.AddResource(nameof(this.ValueBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
            this.Opacity = this.resources.AddResource(nameof(this.Opacity), doubleSerializer, 0.95);
            this.FontSize = this.resources.AddResource(applicationTheme.WindowFontSize);
            this.FontFamily = this.resources.AddResource(applicationTheme.WindowFontFamily);
            this.CodeFontSize = this.resources.AddResource(applicationTheme.CodeFontSize);
            this.CodeFontFamily = this.resources.AddResource(applicationTheme.CodeFontFamily);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }
}
