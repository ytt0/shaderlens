namespace Shaderlens.Presentation.Elements
{
    public class RowsPanel : Panel
    {
        private const double Padding = 4;

        protected override Size MeasureOverride(Size availableSize)
        {
            var height = 0.0;
            var maxWidth = 0.0;

            foreach (var child in this.Children.Cast<UIElement>())
            {
                child.Measure(new Size(availableSize.Width, Double.PositiveInfinity));
                maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
                height += child.DesiredSize.Height + Padding;
            }

            height -= Padding;
            height = Math.Max(height, 0.0);

            return new Size(maxWidth, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var top = 0.0;

            foreach (var child in this.Children.Cast<UIElement>())
            {
                var rect = new Rect(0, top, finalSize.Width, child.DesiredSize.Height);
                child.Arrange(rect);
                top += rect.Height + Padding;
            }

            return finalSize;
        }
    }
}
