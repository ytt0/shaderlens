namespace Shaderlens.Presentation.Elements
{
    public class AlphaBackgroundView : FrameworkElement
    {
        private const byte AlphaBackgroundValue1 = 94;
        private const byte AlphaBackgroundValue2 = 144;

        private static readonly Brush AlphaBackgroundBrush1 = new SolidColorBrush(Color.FromArgb(255, AlphaBackgroundValue1, AlphaBackgroundValue1, AlphaBackgroundValue1)).WithFreeze();
        private static readonly Brush AlphaBackgroundBrush2 = new DrawingBrush
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, 16, 16),
            ViewportUnits = BrushMappingMode.Absolute,
            Drawing = new GeometryDrawing(new SolidColorBrush(Color.FromArgb(255, AlphaBackgroundValue2, AlphaBackgroundValue2, AlphaBackgroundValue2)), null, Geometry.Parse("M0,0 H1 V1 H2 V2 H1 V1 H0Z"))
        }.WithFreeze();

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var rect = new Rect(this.RenderSize);
            drawingContext.DrawRectangle(AlphaBackgroundBrush1, null, rect);
            drawingContext.DrawRectangle(AlphaBackgroundBrush2, null, rect);
        }
    }
}
