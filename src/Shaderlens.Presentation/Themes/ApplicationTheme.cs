namespace Shaderlens.Presentation.Themes
{
    public interface IApplicationTheme : IThemeResources
    {
        string? Path { get; }

        IScrollBarTheme ScrollBar { get; }
        IMenuTheme Menu { get; }
        ILogTheme Log { get; }

        IThemeResource<Brush> WindowBackground { get; }
        IThemeResource<Brush> WindowForeground { get; }
        IThemeResource<Color> WindowTitleBackground { get; }
        IThemeResource<Brush> WindowMessageBackground { get; }
        IThemeResource<Brush> TextEditForeground { get; }
        IThemeResource<Brush> TextInvalidForeground { get; }
        IThemeResource<Brush> TextDragForeground { get; }
        IThemeResource<Brush> TextProgress { get; }
        IThemeResource<Brush> TextProgressTrack { get; }
        IThemeResource<Brush> ControlHoveredBackground { get; }
        IThemeResource<Brush> ControlPressedBackground { get; }
        IThemeResource<Brush> ControlSelectionBackground { get; }
        IThemeResource<Brush> ControlBorder { get; }
        IThemeResource<Brush> ControlHoveredBorder { get; }
        IThemeResource<Brush> ControlFocusedBorder { get; }
        IThemeResource<Brush> GroupBackground { get; }
        IThemeResource<Brush> IconForeground { get; }
        IThemeResource<Brush> Separator { get; }
        IThemeResource<Brush> WarningTextForeground { get; }
        IThemeResource<Brush> CommentTextForeground { get; }
        IThemeResource<Brush> TextHighlightForeground { get; }
        IThemeResource<Brush> TextHighlightBackground { get; }
        IThemeResource<double> WindowFontSize { get; }
        IThemeResource<FontFamily> WindowFontFamily { get; }
        IThemeResource<double> CodeFontSize { get; }
        IThemeResource<FontFamily> CodeFontFamily { get; }
        IThemeResource<double> TextSelectionOpacity { get; }
        IThemeResource<Brush> TextSelectionBackground { get; }
    }

    public class LightApplicationTheme : IApplicationTheme
    {
        public string? Path { get; }

        public IScrollBarTheme ScrollBar { get; }
        public IMenuTheme Menu { get; }
        public ILogTheme Log { get; }

        public IThemeResource<Brush> WindowBackground { get; }
        public IThemeResource<Brush> WindowForeground { get; }
        public IThemeResource<Color> WindowTitleBackground { get; }
        public IThemeResource<Brush> WindowMessageBackground { get; }
        public IThemeResource<Brush> TextEditForeground { get; }
        public IThemeResource<Brush> TextDragForeground { get; }
        public IThemeResource<Brush> TextInvalidForeground { get; }
        public IThemeResource<Brush> TextProgress { get; }
        public IThemeResource<Brush> TextProgressTrack { get; }
        public IThemeResource<Brush> ControlHoveredBackground { get; }
        public IThemeResource<Brush> ControlPressedBackground { get; }
        public IThemeResource<Brush> ControlSelectionBackground { get; }
        public IThemeResource<Brush> ControlBorder { get; }
        public IThemeResource<Brush> ControlHoveredBorder { get; }
        public IThemeResource<Brush> ControlFocusedBorder { get; }
        public IThemeResource<Brush> GroupBackground { get; }
        public IThemeResource<Brush> IconForeground { get; }
        public IThemeResource<Brush> Separator { get; }
        public IThemeResource<Brush> WarningTextForeground { get; }
        public IThemeResource<Brush> CommentTextForeground { get; }
        public IThemeResource<Brush> TextHighlightForeground { get; }
        public IThemeResource<Brush> TextHighlightBackground { get; }
        public IThemeResource<double> WindowFontSize { get; }
        public IThemeResource<FontFamily> WindowFontFamily { get; }
        public IThemeResource<double> CodeFontSize { get; }
        public IThemeResource<FontFamily> CodeFontFamily { get; }
        public IThemeResource<Brush> TextSelectionBackground { get; }
        public IThemeResource<double> TextSelectionOpacity { get; }

        private readonly ThemeResources resources;

        public LightApplicationTheme() :
            this(new JsonSettings(new JsonObject()), null)
        {
        }

        public LightApplicationTheme(IJsonSettings settings, string? path)
        {
            this.resources = new ThemeResources(settings);
            this.Path = path;

            var colorSerializer = new ColorJsonSerializer();
            var brushSerializer = new BrushJsonSerializer();
            var doubleSerializer = new DoubleJsonSerializer(2);
            var fontSizeSerializer = new DoubleJsonSerializer(1);
            var fontFamilySerializer = new FontFamilyJsonSerializer();

            this.WindowBackground = this.resources.AddResource(nameof(this.WindowBackground), brushSerializer, Brushes.White);
            this.WindowForeground = this.resources.AddResource(nameof(this.WindowForeground), brushSerializer, Brushes.Black);
            this.WindowTitleBackground = this.resources.AddResource(nameof(this.WindowTitleBackground), colorSerializer, Colors.Transparent);
            this.WindowMessageBackground = this.resources.AddResource(nameof(this.WindowMessageBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(240, 240, 240)));
            this.TextEditForeground = this.resources.AddResource(nameof(this.TextEditForeground), brushSerializer, Brushes.Black);
            this.TextDragForeground = this.resources.AddResource(nameof(this.TextDragForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 200, 255)));
            this.TextInvalidForeground = this.resources.AddResource(nameof(this.TextInvalidForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 0, 0)));
            this.TextProgress = this.resources.AddResource(nameof(this.TextProgress), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.TextProgressTrack = this.resources.AddResource(nameof(this.TextProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(235, 235, 235)));
            this.ControlHoveredBackground = this.resources.AddResource(nameof(this.ControlHoveredBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(240, 240, 240)));
            this.ControlPressedBackground = this.resources.AddResource(nameof(this.ControlPressedBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(235, 235, 235)));
            this.ControlSelectionBackground = this.resources.AddResource(nameof(this.ControlSelectionBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(235, 235, 235)));
            this.ControlBorder = this.resources.AddResource(nameof(this.ControlBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(170, 170, 170)));
            this.ControlFocusedBorder = this.resources.AddResource(nameof(this.ControlFocusedBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 0, 0)));
            this.ControlHoveredBorder = this.resources.AddResource(nameof(this.ControlHoveredBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(140, 140, 140)));
            this.GroupBackground = this.resources.AddResource(nameof(this.GroupBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(245, 245, 245)));
            this.IconForeground = this.resources.AddResource(nameof(this.IconForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 180, 180)));
            this.Separator = this.resources.AddResource(nameof(this.Separator), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.WarningTextForeground = this.resources.AddResource(nameof(this.WarningTextForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 0, 0)));
            this.CommentTextForeground = this.resources.AddResource(nameof(this.CommentTextForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.TextHighlightForeground = this.resources.AddResource(nameof(this.TextHighlightForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 60, 76)));
            this.TextHighlightBackground = this.resources.AddResource(nameof(this.TextHighlightBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(179, 239, 255)));
            this.WindowFontSize = this.resources.AddResource(nameof(this.WindowFontSize), fontSizeSerializer, 16.0);
            this.WindowFontFamily = this.resources.AddResource(nameof(this.WindowFontFamily), fontFamilySerializer, new FontFamily());
            this.CodeFontSize = this.resources.AddResource(nameof(this.CodeFontSize), fontSizeSerializer, 16.0);
            this.CodeFontFamily = this.resources.AddResource(nameof(this.CodeFontFamily), fontFamilySerializer, new FontFamily("Consolas"));
            this.TextSelectionOpacity = this.resources.AddResource(nameof(this.TextSelectionOpacity), doubleSerializer, 0.3);
            this.TextSelectionBackground = this.resources.AddResource(nameof(this.TextSelectionBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 200, 255)));

            this.ScrollBar = new LightScrollBarTheme(settings);
            this.Menu = new LightMenuTheme(this, settings);
            this.Log = new LightLogTheme(this, settings);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);

            this.ScrollBar.SetResources(target);
            this.Menu.SetResources(target);
            this.Log.SetResources(target);
        }
    }

    public class DarkApplicationTheme : IApplicationTheme
    {
        public string? Path { get; }

        public IScrollBarTheme ScrollBar { get; }
        public IMenuTheme Menu { get; }
        public ILogTheme Log { get; }

        public IThemeResource<Brush> WindowBackground { get; }
        public IThemeResource<Brush> WindowForeground { get; }
        public IThemeResource<Color> WindowTitleBackground { get; }
        public IThemeResource<Brush> WindowMessageBackground { get; }
        public IThemeResource<Brush> TextEditForeground { get; }
        public IThemeResource<Brush> TextDragForeground { get; }
        public IThemeResource<Brush> TextInvalidForeground { get; }
        public IThemeResource<Brush> TextProgress { get; }
        public IThemeResource<Brush> TextProgressTrack { get; }
        public IThemeResource<Brush> ControlHoveredBackground { get; }
        public IThemeResource<Brush> ControlPressedBackground { get; }
        public IThemeResource<Brush> ControlSelectionBackground { get; }
        public IThemeResource<Brush> ControlBorder { get; }
        public IThemeResource<Brush> ControlHoveredBorder { get; }
        public IThemeResource<Brush> ControlFocusedBorder { get; }
        public IThemeResource<Brush> GroupBackground { get; }
        public IThemeResource<Brush> IconForeground { get; }
        public IThemeResource<Brush> Separator { get; }
        public IThemeResource<Brush> WarningTextForeground { get; }
        public IThemeResource<Brush> CommentTextForeground { get; }
        public IThemeResource<Brush> TextHighlightForeground { get; }
        public IThemeResource<Brush> TextHighlightBackground { get; }
        public IThemeResource<double> WindowFontSize { get; }
        public IThemeResource<FontFamily> WindowFontFamily { get; }
        public IThemeResource<double> CodeFontSize { get; }
        public IThemeResource<FontFamily> CodeFontFamily { get; }
        public IThemeResource<double> TextSelectionOpacity { get; }
        public IThemeResource<Brush> TextSelectionBackground { get; }

        private readonly ThemeResources resources;

        public DarkApplicationTheme() :
            this(new JsonSettings(new JsonObject()), null)
        {
        }

        public DarkApplicationTheme(IJsonSettings settings, string? path)
        {
            this.resources = new ThemeResources(settings);
            this.Path = path;

            var colorSerializer = new ColorJsonSerializer();
            var brushSerializer = new BrushJsonSerializer();
            var doubleSerializer = new DoubleJsonSerializer(2);
            var fontSizeSerializer = new DoubleJsonSerializer(1);
            var fontFamilySerializer = new FontFamilyJsonSerializer();

            this.WindowBackground = this.resources.AddResource(nameof(this.WindowBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(12, 12, 12)));
            this.WindowForeground = this.resources.AddResource(nameof(this.WindowForeground), brushSerializer, Brushes.White);
            this.WindowTitleBackground = this.resources.AddResource(nameof(this.WindowTitleBackground), colorSerializer, Color.FromRgb(32, 32, 32));
            this.WindowMessageBackground = this.resources.AddResource(nameof(this.WindowMessageBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(32, 32, 32)));
            this.TextEditForeground = this.resources.AddResource(nameof(this.TextEditForeground), brushSerializer, Brushes.White);
            this.TextDragForeground = this.resources.AddResource(nameof(this.TextDragForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 200, 255)));
            this.TextInvalidForeground = this.resources.AddResource(nameof(this.TextInvalidForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 150, 150)));
            this.TextProgress = this.resources.AddResource(nameof(this.TextProgress), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 180, 180)));
            this.TextProgressTrack = this.resources.AddResource(nameof(this.TextProgressTrack), brushSerializer, new SolidColorBrush(Color.FromRgb(80, 80, 80)));
            this.ControlHoveredBackground = this.resources.AddResource(nameof(this.ControlHoveredBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(45, 45, 45)));
            this.ControlPressedBackground = this.resources.AddResource(nameof(this.ControlPressedBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(55, 55, 55)));
            this.ControlSelectionBackground = this.resources.AddResource(nameof(this.ControlSelectionBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(55, 55, 55)));
            this.ControlBorder = this.resources.AddResource(nameof(this.ControlBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
            this.ControlFocusedBorder = this.resources.AddResource(nameof(this.ControlFocusedBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 255, 255)));
            this.ControlHoveredBorder = this.resources.AddResource(nameof(this.ControlHoveredBorder), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.GroupBackground = this.resources.AddResource(nameof(this.GroupBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(32, 32, 32)));
            this.IconForeground = this.resources.AddResource(nameof(this.IconForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(200, 200, 200)));
            this.Separator = this.resources.AddResource(nameof(this.Separator), brushSerializer, new SolidColorBrush(Color.FromRgb(57, 57, 57)));
            this.WarningTextForeground = this.resources.AddResource(nameof(this.WarningTextForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(255, 150, 150)));
            this.CommentTextForeground = this.resources.AddResource(nameof(this.CommentTextForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            this.TextHighlightForeground = this.resources.AddResource(nameof(this.TextHighlightForeground), brushSerializer, new SolidColorBrush(Color.FromRgb(179, 239, 255)));
            this.TextHighlightBackground = this.resources.AddResource(nameof(this.TextHighlightBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(8, 68, 84)));
            this.WindowFontSize = this.resources.AddResource(nameof(this.WindowFontSize), fontSizeSerializer, 16.0);
            this.WindowFontFamily = this.resources.AddResource(nameof(this.WindowFontFamily), fontFamilySerializer, new FontFamily());
            this.CodeFontSize = this.resources.AddResource(nameof(this.CodeFontSize), fontSizeSerializer, 16.0);
            this.CodeFontFamily = this.resources.AddResource(nameof(this.CodeFontFamily), fontFamilySerializer, new FontFamily("Consolas"));
            this.TextSelectionOpacity = this.resources.AddResource(nameof(this.TextSelectionOpacity), doubleSerializer, 0.3);
            this.TextSelectionBackground = this.resources.AddResource(nameof(this.TextSelectionBackground), brushSerializer, new SolidColorBrush(Color.FromRgb(0, 200, 255)));

            this.ScrollBar = new DarkScrollBarTheme(settings);
            this.Menu = new DarkMenuTheme(this, settings);
            this.Log = new DarkLogTheme(this, settings);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);

            this.ScrollBar.SetResources(target);
            this.Menu.SetResources(target);
            this.Log.SetResources(target);
        }
    }
}
