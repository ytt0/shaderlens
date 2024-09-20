namespace Shaderlens.Presentation.Behaviors
{
    public class MenuOpenInputReleaseBehavior : IDisposable
    {
        private const double MoveThreshold = 10.0;

        private readonly ContextMenu menu;
        private Point openPosition;
        private bool skipInput;

        private MenuOpenInputReleaseBehavior(ContextMenu menu)
        {
            this.menu = menu;
            this.menu.Opened += OnOpened;
            this.menu.PreviewMouseDown += OnPreviewMouseDown;
            this.menu.PreviewMouseMove += OnPreviewMouseMove;
            this.menu.PreviewMouseUp += OnPreviewMouseUp;
            this.menu.PreviewKeyDown += OnPreviewKeyDown;
            this.menu.PreviewKeyUp += OnPreviewKeyUp;
        }

        public void Dispose()
        {
            this.menu.Opened -= OnOpened;
            this.menu.PreviewMouseDown -= OnPreviewMouseDown;
            this.menu.PreviewMouseMove -= OnPreviewMouseMove;
            this.menu.PreviewMouseUp -= OnPreviewMouseUp;
            this.menu.PreviewKeyDown -= OnPreviewKeyDown;
            this.menu.PreviewKeyUp -= OnPreviewKeyUp;
        }

        private void OnOpened(object sender, RoutedEventArgs e)
        {
            this.openPosition = this.menu.PointToScreen(Mouse.GetPosition(this.menu));
            this.menu.Dispatcher.InvokeAsync(() => this.skipInput = true);
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = this.skipInput;
            this.skipInput = false;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = this.skipInput;
            this.skipInput = false;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.skipInput && this.menu.PointFromScreen(this.openPosition).DistanceApprox(Mouse.GetPosition(this.menu)) > MoveThreshold)
            {
                this.skipInput = false;
            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.skipInput = false;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.skipInput = false;
        }

        public static IDisposable Register(ContextMenu menu)
        {
            return new MenuOpenInputReleaseBehavior(menu);
        }
    }
}
