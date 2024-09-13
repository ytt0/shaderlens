namespace Shaderlens.Presentation.Elements
{
    public class MenuHeaderStyle : IStyle<MenuItem>
    {
        private readonly IMenuTheme theme;

        public MenuHeaderStyle(IMenuTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(MenuItem target)
        {
            target.IsEnabled = false;
            target.Margin = new Thickness(0, 0, 0, 2);

            ContextMenuDragBehavior.Register(target);

            var descendants = target.GetVisualDescendants(10).OfType<FrameworkElement>().ToArray();

            var border = descendants.OfType<Border>().FirstOrDefault();
            var menuHeaderContainer = descendants.OfType<ContentPresenter>().FirstOrDefault(element => element.Name == "menuHeaderContainer");
            var grid = descendants.OfType<Grid>().FirstOrDefault(grid => grid.ColumnDefinitions.Count > 2);

            target.SetReference(Control.BackgroundProperty, this.theme.HeaderBackground);

            if (border != null)
            {
                border.BorderThickness = new Thickness(0, 0, 0, 1);
                border.CornerRadius = new CornerRadius(2, 2, 0, 0);
                border.Padding = new Thickness(0, 4, 0, 6);
                border.SetReference(TextElement.ForegroundProperty, this.theme.HeaderForeground);
            }

            if (menuHeaderContainer != null)
            {
                menuHeaderContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
            }

            if (grid != null)
            {
                grid.ColumnDefinitions.Clear();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.Margin = new Thickness(-4);
                grid.Opacity = 0.6;
            }
        }
    }
}
