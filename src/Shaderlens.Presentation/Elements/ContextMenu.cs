namespace Shaderlens.Presentation.Elements
{
    public class ContextMenuStyle : IStyle<ContextMenu>
    {
        private readonly IMenuTheme theme;
        private readonly MenuContentStyle contentStyle;

        public ContextMenuStyle(IMenuTheme theme)
        {
            this.theme = theme;
            this.contentStyle = new MenuContentStyle(theme);
        }

        public void Apply(ContextMenu target)
        {
            target.FontSize = 16;
            target.UseLayoutRounding = true;
            target.HasDropShadow = false;
            target.SetReference(Control.BackgroundProperty, this.theme.Background);
            target.SetReference(Control.ForegroundProperty, this.theme.Foreground);

            this.contentStyle.Apply(target);
        }
    }

    public class StyledContextMenu : ContextMenu
    {
        private readonly IStyle<ContextMenu> style;

        public StyledContextMenu(IMenuTheme theme) :
            this(new ContextMenuStyle(theme))
        {
        }

        public StyledContextMenu(IStyle<ContextMenu> style)
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
