namespace Shaderlens.Presentation.Elements
{
    public class ColumnPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return new Size();
            }

            var columnSize = new Size(availableSize.Width / this.Children.Count, availableSize.Height);

            var maxWidth = 0.0;
            var maxHeight = 0.0;

            foreach (var child in this.Children.Cast<FrameworkElement>())
            {
                child.Measure(columnSize);
                maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }

            return new Size(maxWidth * this.Children.Count, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count == 0)
            {
                return new Size();
            }

            var index = 0;
            var width = finalSize.Width / this.Children.Count;

            foreach (var child in this.Children.Cast<FrameworkElement>())
            {
                child.Arrange(new Rect(index * width, 0, width, finalSize.Height));
                index++;
            }

            return finalSize;
        }
    }
}
