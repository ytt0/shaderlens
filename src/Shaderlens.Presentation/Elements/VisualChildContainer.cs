namespace Shaderlens.Presentation.Elements
{
    public abstract class VisualChildContainer : FrameworkElement
    {
        protected override int VisualChildrenCount { get { return 1; } }

        [AllowNull]
        private FrameworkElement child;

        protected override void OnInitialized(EventArgs e)
        {
            this.child = GetChild();
            AddVisualChild(this.child);

            base.OnInitialized(e);
        }

        protected abstract FrameworkElement GetChild();

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? this.child : throw new IndexOutOfRangeException();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.child.Measure(availableSize);
            return this.child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
