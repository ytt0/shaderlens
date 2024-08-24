namespace Shaderlens.Presentation.Themes
{
    public interface ILogTheme : IThemeResources
    {
        IThemeResource<Brush> ProgressTrack { get; }
        IThemeResource<Brush> HeaderForeground { get; }
        IThemeResource<Brush> HeaderBackground { get; }
        IThemeResource<Brush> SuccessProgressTrack { get; }
        IThemeResource<Brush> SuccessHeaderForeground { get; }
        IThemeResource<Brush> SuccessHeaderBackground { get; }
        IThemeResource<Brush> FailureProgressTrack { get; }
        IThemeResource<Brush> FailureHeaderForeground { get; }
        IThemeResource<Brush> FailureHeaderBackground { get; }
        IThemeResource<Brush> FailureDetailsForeground { get; }
        IThemeResource<Brush> FailureDetailsBackground { get; }
        IThemeResource<Brush> LinkForeground { get; }
        IThemeResource<Brush> ContextForeground { get; }
        IThemeResource<Brush> ContextBackground { get; }
        IThemeResource<Brush> ContextHighlightForeground { get; }
        IThemeResource<Brush> ContextHighlightBackground { get; }
        IThemeResource<double> FailureViewportOpacity { get; }
        IThemeResource<double> CodeFontSize { get; }
        IThemeResource<FontFamily> CodeFontFamily { get; }
        IThemeResource<double> TextSelectionOpacity { get; }
        IThemeResource<Brush> TextSelectionBackground { get; }
    }

    public class LightLogTheme : ILogTheme
    {
        public IThemeResource<Brush> ProgressTrack { get; }
        public IThemeResource<Brush> HeaderForeground { get; }
        public IThemeResource<Brush> HeaderBackground { get; }
        public IThemeResource<Brush> SuccessProgressTrack { get; }
        public IThemeResource<Brush> SuccessHeaderForeground { get; }
        public IThemeResource<Brush> SuccessHeaderBackground { get; }
        public IThemeResource<Brush> FailureProgressTrack { get; }
        public IThemeResource<Brush> FailureHeaderForeground { get; }
        public IThemeResource<Brush> FailureHeaderBackground { get; }
        public IThemeResource<Brush> FailureDetailsForeground { get; }
        public IThemeResource<Brush> FailureDetailsBackground { get; }
        public IThemeResource<Brush> LinkForeground { get; }
        public IThemeResource<Brush> ContextForeground { get; }
        public IThemeResource<Brush> ContextBackground { get; }
        public IThemeResource<Brush> ContextHighlightForeground { get; }
        public IThemeResource<Brush> ContextHighlightBackground { get; }
        public IThemeResource<double> FailureViewportOpacity { get; }
        public IThemeResource<double> CodeFontSize { get; }
        public IThemeResource<FontFamily> CodeFontFamily { get; }
        public IThemeResource<double> TextSelectionOpacity { get; }
        public IThemeResource<Brush> TextSelectionBackground { get; }

        private readonly ThemeResources resources;

        public LightLogTheme(IApplicationTheme applicationTheme, IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "Log");

            var brushSerializer = new BrushJsonSerializer();
            var doubleSerializer = new DoubleJsonSerializer(2);

            this.ProgressTrack = this.resources.AddResource(nameof(this.ProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(60, 60, 60)));
            this.HeaderForeground = this.resources.AddResource(nameof(this.HeaderForeground), brushSerializer, Brushes.Black);
            this.HeaderBackground = this.resources.AddResource(nameof(this.HeaderBackground), brushSerializer, new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)));
            this.SuccessProgressTrack = this.resources.AddResource(nameof(this.SuccessProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(85, 255, 0)));
            this.SuccessHeaderForeground = this.resources.AddResource(nameof(this.SuccessHeaderForeground), brushSerializer, Brushes.Black);
            this.SuccessHeaderBackground = this.resources.AddResource(nameof(this.SuccessHeaderBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(190, 255, 150)));
            this.FailureProgressTrack = this.resources.AddResource(nameof(this.FailureProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 0, 0)));
            this.FailureHeaderForeground = this.resources.AddResource(nameof(this.FailureHeaderForeground), brushSerializer, Brushes.Black);
            this.FailureHeaderBackground = this.resources.AddResource(nameof(this.FailureHeaderBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 180, 180)));
            this.FailureDetailsForeground = this.resources.AddResource(nameof(this.FailureDetailsForeground), brushSerializer, Brushes.Black);
            this.FailureDetailsBackground = this.resources.AddResource(nameof(this.FailureDetailsBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 210, 210)));
            this.LinkForeground = this.resources.AddResource(nameof(this.LinkForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(140, 0, 0)));
            this.ContextForeground = this.resources.AddResource(nameof(this.ContextForeground), brushSerializer, Brushes.Black);
            this.ContextBackground = this.resources.AddResource(nameof(this.ContextBackground), brushSerializer, Brushes.Transparent);
            this.ContextHighlightForeground = this.resources.AddResource(nameof(this.ContextHighlightForeground), brushSerializer, Brushes.Black);
            this.ContextHighlightBackground = this.resources.AddResource(nameof(this.ContextHighlightBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 255, 0)));
            this.FailureViewportOpacity = this.resources.AddResource(nameof(this.FailureViewportOpacity), doubleSerializer, 0.1);
            this.CodeFontSize = this.resources.AddResource(applicationTheme.CodeFontSize);
            this.CodeFontFamily = this.resources.AddResource(applicationTheme.CodeFontFamily);
            this.TextSelectionOpacity = this.resources.AddResource(applicationTheme.TextSelectionOpacity);
            this.TextSelectionBackground = this.resources.AddResource(applicationTheme.TextSelectionBackground);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }

    public class DarkLogTheme : ILogTheme
    {
        public IThemeResource<Brush> ProgressTrack { get; }
        public IThemeResource<Brush> HeaderForeground { get; }
        public IThemeResource<Brush> HeaderBackground { get; }
        public IThemeResource<Brush> SuccessProgressTrack { get; }
        public IThemeResource<Brush> SuccessHeaderForeground { get; }
        public IThemeResource<Brush> SuccessHeaderBackground { get; }
        public IThemeResource<Brush> FailureProgressTrack { get; }
        public IThemeResource<Brush> FailureHeaderForeground { get; }
        public IThemeResource<Brush> FailureHeaderBackground { get; }
        public IThemeResource<Brush> FailureDetailsForeground { get; }
        public IThemeResource<Brush> FailureDetailsBackground { get; }
        public IThemeResource<Brush> LinkForeground { get; }
        public IThemeResource<Brush> ContextForeground { get; }
        public IThemeResource<Brush> ContextBackground { get; }
        public IThemeResource<Brush> ContextHighlightForeground { get; }
        public IThemeResource<Brush> ContextHighlightBackground { get; }
        public IThemeResource<double> FailureViewportOpacity { get; }
        public IThemeResource<double> CodeFontSize { get; }
        public IThemeResource<FontFamily> CodeFontFamily { get; }
        public IThemeResource<double> TextSelectionOpacity { get; }
        public IThemeResource<Brush> TextSelectionBackground { get; }

        private readonly ThemeResources resources;

        public DarkLogTheme(IApplicationTheme applicationTheme, IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "Log");

            var brushSerializer = new BrushJsonSerializer();
            var doubleSerializer = new DoubleJsonSerializer(2);

            this.ProgressTrack = this.resources.AddResource(nameof(this.ProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.HeaderForeground = this.resources.AddResource(nameof(this.HeaderForeground), brushSerializer, Brushes.White);
            this.HeaderBackground = this.resources.AddResource(nameof(this.HeaderBackground), brushSerializer, new SolidColorBrush(Color.FromArgb(220, 0, 0, 0)));
            this.SuccessProgressTrack = this.resources.AddResource(nameof(this.SuccessProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(85, 255, 0)));
            this.SuccessHeaderForeground = this.resources.AddResource(nameof(this.SuccessHeaderForeground), brushSerializer, Brushes.White);
            this.SuccessHeaderBackground = this.resources.AddResource(nameof(this.SuccessHeaderBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(51, 150, 0)));
            this.FailureProgressTrack = this.resources.AddResource(nameof(this.FailureProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 0, 0)));
            this.FailureHeaderForeground = this.resources.AddResource(nameof(this.FailureHeaderForeground), brushSerializer, Brushes.White);
            this.FailureHeaderBackground = this.resources.AddResource(nameof(this.FailureHeaderBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(110, 0, 0)));
            this.FailureDetailsForeground = this.resources.AddResource(nameof(this.FailureDetailsForeground), brushSerializer, Brushes.White);
            this.FailureDetailsBackground = this.resources.AddResource(nameof(this.FailureDetailsBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(80, 0, 0)));
            this.LinkForeground = this.resources.AddResource(nameof(this.LinkForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 180, 180)));
            this.ContextForeground = this.resources.AddResource(nameof(this.ContextForeground), brushSerializer, Brushes.White);
            this.ContextBackground = this.resources.AddResource(nameof(this.ContextBackground), brushSerializer, Brushes.Transparent);
            this.ContextHighlightForeground = this.resources.AddResource(nameof(this.ContextHighlightForeground), brushSerializer, Brushes.Black);
            this.ContextHighlightBackground = this.resources.AddResource(nameof(this.ContextHighlightBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 255, 0)));
            this.FailureViewportOpacity = this.resources.AddResource(nameof(this.FailureViewportOpacity), doubleSerializer, 0.2);
            this.CodeFontSize = this.resources.AddResource(applicationTheme.CodeFontSize);
            this.CodeFontFamily = this.resources.AddResource(applicationTheme.CodeFontFamily);
            this.TextSelectionOpacity = this.resources.AddResource(applicationTheme.TextSelectionOpacity);
            this.TextSelectionBackground = this.resources.AddResource(applicationTheme.TextSelectionBackground);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }
}
