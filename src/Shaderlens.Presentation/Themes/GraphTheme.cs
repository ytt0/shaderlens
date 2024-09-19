namespace Shaderlens.Presentation.Themes
{
    public class LightGraphTheme : IGraphTheme
    {
        public IThemeResource<Brush> CursorFill { get; }
        public IThemeResource<Brush> CursorStroke { get; }
        public IThemeResource<Brush> SourceCursorFill { get; }
        public IThemeResource<Brush> SourceCursorStroke { get; }
        public IThemeResource<Brush> EdgeCursorFill { get; }
        public IThemeResource<Brush> EdgeCursorStroke { get; }
        public IThemeResource<Brush> SourceEdgeCursorFill { get; }
        public IThemeResource<Brush> SourceEdgeCursorStroke { get; }
        public IThemeResource<Brush> GridStroke { get; }
        public IThemeResource<Brush> BoundsFill { get; }
        public IThemeResource<Brush> BoundsStroke { get; }

        private readonly ThemeResources resources;

        public LightGraphTheme(IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "Graph");

            var brushSerializer = new BrushJsonSerializer();

            this.CursorFill = this.resources.AddResource(nameof(this.CursorFill), brushSerializer, Brushes.White);
            this.CursorStroke = this.resources.AddResource(nameof(this.CursorStroke), brushSerializer, Brushes.Black);
            this.SourceCursorFill = this.resources.AddResource(nameof(this.SourceCursorFill), brushSerializer, Brushes.White);
            this.SourceCursorStroke = this.resources.AddResource(nameof(this.SourceCursorStroke), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 180, 180)));

            this.EdgeCursorFill = this.resources.AddResource(nameof(this.EdgeCursorFill), brushSerializer, Brushes.Yellow);
            this.EdgeCursorStroke = this.resources.AddResource(nameof(this.EdgeCursorStroke), brushSerializer, Brushes.Black);
            this.SourceEdgeCursorFill = this.resources.AddResource(nameof(this.SourceEdgeCursorFill), brushSerializer, Brushes.Yellow);
            this.SourceEdgeCursorStroke = this.resources.AddResource(nameof(this.SourceEdgeCursorStroke), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 180, 180)));

            this.GridStroke = this.resources.AddResource(nameof(this.GridStroke), brushSerializer, Brushes.Black);
            this.BoundsFill = this.resources.AddResource(nameof(this.BoundsFill), brushSerializer, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
            this.BoundsStroke = this.resources.AddResource(nameof(this.BoundsStroke), brushSerializer, Brushes.Black);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }

    public class DarkGraphTheme : IGraphTheme
    {
        public IThemeResource<Brush> CursorFill { get; }
        public IThemeResource<Brush> CursorStroke { get; }
        public IThemeResource<Brush> SourceCursorFill { get; }
        public IThemeResource<Brush> SourceCursorStroke { get; }

        public IThemeResource<Brush> EdgeCursorFill { get; }
        public IThemeResource<Brush> EdgeCursorStroke { get; }
        public IThemeResource<Brush> SourceEdgeCursorFill { get; }
        public IThemeResource<Brush> SourceEdgeCursorStroke { get; }

        public IThemeResource<Brush> GridStroke { get; }
        public IThemeResource<Brush> BoundsFill { get; }
        public IThemeResource<Brush> BoundsStroke { get; }

        private readonly ThemeResources resources;

        public DarkGraphTheme(IJsonSettings settings)
        {
            this.resources = new ThemeResources(settings, "Graph");

            var brushSerializer = new BrushJsonSerializer();

            this.CursorFill = this.resources.AddResource(nameof(this.CursorFill), brushSerializer, Brushes.White);
            this.CursorStroke = this.resources.AddResource(nameof(this.CursorStroke), brushSerializer, Brushes.Black);
            this.SourceCursorFill = this.resources.AddResource(nameof(this.SourceCursorFill), brushSerializer, new SolidColorBrush(Color.FromRgb(120, 120, 120)));
            this.SourceCursorStroke = this.resources.AddResource(nameof(this.SourceCursorStroke), brushSerializer, Brushes.Black);

            this.EdgeCursorFill = this.resources.AddResource(nameof(this.EdgeCursorFill), brushSerializer, Brushes.Yellow);
            this.EdgeCursorStroke = this.resources.AddResource(nameof(this.EdgeCursorStroke), brushSerializer, Brushes.Black);
            this.SourceEdgeCursorFill = this.resources.AddResource(nameof(this.SourceEdgeCursorFill), brushSerializer, new SolidColorBrush(Color.FromRgb(160, 160, 0)));
            this.SourceEdgeCursorStroke = this.resources.AddResource(nameof(this.SourceEdgeCursorStroke), brushSerializer, Brushes.Black);

            this.GridStroke = this.resources.AddResource(nameof(this.GridStroke), brushSerializer, new SolidColorBrush(Color.FromRgb(180, 180, 180)));
            this.BoundsFill = this.resources.AddResource(nameof(this.BoundsFill), brushSerializer, new SolidColorBrush(Color.FromRgb(160, 160, 160)));
            this.BoundsStroke = this.resources.AddResource(nameof(this.BoundsStroke), brushSerializer, Brushes.White);
        }

        public void SetResources(ResourceDictionary target)
        {
            this.resources.SetResources(target);
        }
    }
}
