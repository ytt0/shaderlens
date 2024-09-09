namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class UniformGroupElement : FrameworkElement
    {
        public Panel Content { get; }

        protected override int VisualChildrenCount { get { return 2; } }

        private readonly GroupHeader header;

        public UniformGroupElement(ISettingsValue<bool> expandedSettingsValue, string displayName, IApplicationTheme theme)
        {
            this.header = new GroupHeader(theme)
            {
                IsChecked = expandedSettingsValue.Value,
                Content = new TextBlock { Text = displayName, FontWeight = FontWeights.Bold },
            };

            this.header.Click += (sender, e) =>
            {
                expandedSettingsValue.Value = !expandedSettingsValue.Value;

                this.header.IsChecked = expandedSettingsValue.Value;
                this.Content!.Visibility = expandedSettingsValue.Value ? Visibility.Visible : Visibility.Collapsed;
            };

            this.Content = new RowsPanel
            {
                Margin = new Thickness(0, 4, 0, 0),
                Visibility = expandedSettingsValue.Value ? Visibility.Visible : Visibility.Collapsed
            };

            AddVisualChild(this.header);
            AddVisualChild(this.Content);
        }

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? this.header : index == 1 ? this.Content : throw new IndexOutOfRangeException();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.header.Measure(availableSize);
            this.Content.Measure(availableSize);

            return new Size(
                Math.Max(this.header.DesiredSize.Width, this.Content.DesiredSize.Width),
                this.header.DesiredSize.Height + this.Content.DesiredSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.header.Arrange(new Rect(0, 0, finalSize.Width, this.header.DesiredSize.Height));
            this.Content.Arrange(new Rect(0, this.header.DesiredSize.Height, finalSize.Width, this.Content.DesiredSize.Height));
            return new Size(finalSize.Width, this.header.DesiredSize.Height + this.Content.DesiredSize.Height);
        }
    }
}
