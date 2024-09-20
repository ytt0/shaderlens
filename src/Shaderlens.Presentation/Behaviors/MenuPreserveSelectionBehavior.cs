namespace Shaderlens.Presentation.Behaviors
{
    public class MenuPreserveSelectionBehavior : IDisposable
    {
        private readonly ContextMenu menu;
        private Point? targetPosition;

        private MenuPreserveSelectionBehavior(ContextMenu menu)
        {
            this.menu = menu;
            this.menu.PreviewMouseDown += OnPreviewMouseDown;
            this.menu.PreviewKeyDown += OnPreviewKeyDown;
            this.menu.Opened += OnOpened;
            this.menu.Closed += OnClosed;
        }

        public void Dispose()
        {
            this.menu.PreviewMouseDown -= OnPreviewMouseDown;
            this.menu.PreviewKeyDown -= OnPreviewKeyDown;
            this.menu.Opened -= OnOpened;
            this.menu.Closed -= OnClosed;
        }

        private void OnClosed(object sender, RoutedEventArgs e)
        {
            this.menu.HorizontalOffset = 0;
            this.menu.VerticalOffset = 0;
        }

        private void OnOpened(object sender, RoutedEventArgs e)
        {
            this.menu.Visibility = Visibility.Hidden;

            this.menu.Dispatcher.InvokeAsync(() =>
            {
                if (this.targetPosition == null && this.menu.Items.OfType<MenuItem>().Any())
                {
                    var selectedItem = this.menu.Items.OfType<MenuItem>().FirstOrDefault(menuItem => menuItem.IsEnabled && menuItem.Visibility == Visibility.Visible) ?? this.menu.Items.OfType<MenuItem>().First();
                    var selectedItemPoint = new Point(selectedItem.ActualWidth / 3, selectedItem.ActualHeight / 2);
                    this.targetPosition = this.menu.LayoutTransform.Transform(selectedItem.TranslatePoint(selectedItemPoint, this.menu));
                }

                if (this.targetPosition != null && WinApi.GetCursorPos(out var position))
                {
                    var mousePosition = this.menu.LayoutTransform.Transform(this.menu.PointFromScreen(new Point(position.X, position.Y)));

                    this.menu.HorizontalOffset = mousePosition.X - this.targetPosition.Value.X;
                    this.menu.VerticalOffset = mousePosition.Y - this.targetPosition.Value.Y;
                }

                this.menu.Visibility = Visibility.Visible;

                this.menu.Dispatcher.InvokeAsync(() =>
                {
                    this.menu.HorizontalOffset -= 1.0;
                    this.menu.HorizontalOffset += 1.0;
                }, DispatcherPriority.Render);
            }, DispatcherPriority.Render);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTargetPosition();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetTargetPosition();
            }
        }

        private void SetTargetPosition()
        {
            this.targetPosition = null;

            foreach (var item in this.menu.Items.OfType<MenuItem>())
            {
                if (item.IsHighlighted)
                {
                    var selectedItem = item;
                    var selectedItemPoint = new Point(item.IsSubmenuOpen ? item.ActualWidth - 20 : Math.Clamp(Mouse.GetPosition(item).X, 10, item.ActualWidth - 10), item.ActualHeight / 2);
                    this.targetPosition = this.menu.LayoutTransform.Transform(selectedItem.TranslatePoint(selectedItemPoint, this.menu));
                    return;
                }
            }
        }

        public static IDisposable Register(ContextMenu menu)
        {
            return new MenuPreserveSelectionBehavior(menu);
        }
    }
}
