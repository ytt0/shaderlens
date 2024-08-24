namespace Shaderlens.Presentation.Elements
{
    public class MenuSeparatorStyle : IStyle<Separator>
    {
        private readonly IMenuTheme theme;

        public MenuSeparatorStyle(IMenuTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(Separator target)
        {
            target.Margin = new Thickness(-3, 3, -3, 3);

            var descendants = target.GetVisualDescendants(2).OfType<FrameworkElement>().ToArray();

            var border = descendants.OfType<Border>().FirstOrDefault();

            if (border != null)
            {
                border.Margin = new Thickness(4, 0, 4, 0);
                border.SetReference(Border.BackgroundProperty, this.theme.Separator);
            }
        }
    }

    public class StyledMenuSeparator : Separator
    {
        private readonly IStyle<Separator> style;

        public StyledMenuSeparator(IMenuTheme theme) :
            this(new MenuSeparatorStyle(theme))
        {
        }

        public StyledMenuSeparator(IStyle<Separator> style)
        {
            this.style = style;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);
        }
    }
}
