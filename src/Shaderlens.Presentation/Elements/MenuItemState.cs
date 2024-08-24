namespace Shaderlens.Presentation.Elements
{
    public interface IMenuItemState
    {
        bool IsEnabled { get; set; }
        bool IsChecked { get; set; }
        bool IsChanged { get; set; }
        bool IsVisible { get; set; }
        object? Tooltip { get; set; }
    }

    public static class MenuItemStateExtensions
    {
        public static IMenuItemState SetIsEnabled(this IMenuItemState state, bool isEnabled)
        {
            state.IsEnabled = isEnabled;
            return state;
        }

        public static IMenuItemState SetIsChecked(this IMenuItemState state, bool isChecked)
        {
            state.IsChecked = isChecked;
            return state;
        }

        public static IMenuItemState SetIsChanged(this IMenuItemState state, bool isChanged)
        {
            state.IsChanged = isChanged;
            return state;
        }

        public static IMenuItemState SetIsVisible(this IMenuItemState state, bool isVisible)
        {
            state.IsVisible = isVisible;
            return state;
        }

        public static IMenuItemState SetTooltip(this IMenuItemState state, object? tooltip)
        {
            state.Tooltip = tooltip;
            return state;
        }
    }

}
