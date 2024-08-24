namespace Shaderlens.Presentation.Behaviors
{
    public class MenuVerticalCenterBehavior : IDisposable
    {
        private Popup? popup;
        private readonly MenuItem menuItem;
        private readonly double offset;

        private MenuVerticalCenterBehavior(MenuItem menuItem, double offset)
        {
            this.menuItem = menuItem;
            this.offset = offset;

            this.menuItem.Loaded += OnLoaded;
        }

        public void Dispose()
        {
            this.menuItem.Loaded -= OnLoaded;

            if (this.popup != null)
            {
                this.popup.CustomPopupPlacementCallback += GetCustomPopupPlacement;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.popup = this.menuItem.GetVisualDescendants(4).OfType<Popup>().FirstOrDefault();

            if (this.popup != null)
            {
                this.popup.Placement = PlacementMode.Custom;
                this.popup.CustomPopupPlacementCallback += GetCustomPopupPlacement;
            }
        }

        private CustomPopupPlacement[] GetCustomPopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            return new[]
            {
                new CustomPopupPlacement(new Point(targetSize.Width, (targetSize.Height - popupSize.Height) / 2 + this.offset), PopupPrimaryAxis.Vertical),
                new CustomPopupPlacement(new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) / 2 + this.offset), PopupPrimaryAxis.Vertical),
            };
        }

        public static IDisposable Register(MenuItem menuItem, double offset)
        {
            return new MenuVerticalCenterBehavior(menuItem, offset);
        }
    }
}
