namespace Shaderlens.Presentation.Elements
{
    public class MenuContentStyle : IStyle<FrameworkElement>
    {
        private readonly IMenuTheme theme;

        public MenuContentStyle(IMenuTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(FrameworkElement target)
        {
            var isPopup = target is Popup;
            if (isPopup)
            {
                target = (FrameworkElement)((Popup)target).Child;
            }

            var descendants = target.GetVisualDescendants(10).OfType<FrameworkElement>().ToArray();

            var border = descendants.OfType<Border>().FirstOrDefault();
            var scrollViewer = descendants.OfType<ScrollViewer>().FirstOrDefault();
            var canvas = descendants.OfType<Canvas>().FirstOrDefault();
            var path = descendants.OfType<Path>().FirstOrDefault();
            var grid = descendants.OfType<Grid>().FirstOrDefault(grid => grid.Children.OfType<ItemsPresenter>().Any());

            if (border != null)
            {
                border.Padding = new Thickness(isPopup ? 2 : 0);
                border.CornerRadius = new CornerRadius(4);
                border.SetReference(UIElement.OpacityProperty, this.theme.Opacity);
                border.SetReference(Border.BackgroundProperty, this.theme.Background);
                border.SetReference(Border.BorderBrushProperty, this.theme.Border);
            }

            if (scrollViewer != null)
            {
                scrollViewer.Margin = new Thickness(0);
            }

            canvas?.Children.Clear();

            path?.SetReference(Shape.FillProperty, this.theme.Arrow);

            if (grid != null)
            {
                var itemsPresenter = grid.Children.OfType<ItemsPresenter>().First();
                grid.Children.Clear();
                grid.Children.Add(itemsPresenter);
            }
        }
    }
}
