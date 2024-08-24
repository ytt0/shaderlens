namespace Shaderlens.Presentation.Elements
{
    public class ColorView : FrameworkElement
    {
        private Color color;
        public Color Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.colorBrush = new SolidColorBrush(Color.FromRgb(this.color.R, this.Color.G, this.Color.B));
                    this.alphaBrush = new SolidColorBrush(this.color);
                    InvalidateVisual();
                }
            }
        }

        private Brush colorBrush;
        private Brush alphaBrush;

        public ColorView()
        {
            this.colorBrush = Brushes.Transparent;
            this.alphaBrush = Brushes.Transparent;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.color.A == 255)
            {
                drawingContext.DrawRectangle(this.colorBrush, null, new Rect(this.RenderSize));
            }
            else
            {
                drawingContext.DrawRectangle(this.alphaBrush, null, new Rect(this.RenderSize));
                drawingContext.DrawRectangle(this.colorBrush, null, new Rect(0, 0, this.RenderSize.Width / 2.0, this.RenderSize.Height));
            }
        }
    }
}
