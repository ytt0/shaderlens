namespace Shaderlens.Presentation.Behaviors
{
    public class ContextMenuDragBehavior : IDisposable
    {
        private readonly FrameworkElement source;
        private readonly ContextMenu? menu;
        private Point startOffset;
        private Point startPosition;
        private IInputElement? startCaptured;
        private bool isDragging;

        public ContextMenuDragBehavior(FrameworkElement source)
        {
            this.menu = source.GetAncestor<ContextMenu>();
            if (this.menu != null)
            {
                this.menu.PreviewMouseDown += OnMouseDown;
                this.menu.PreviewMouseMove += OnMouseMove;
                this.menu.PreviewMouseUp += OnMouseUp;
            }

            this.source = source;
        }

        public void Dispose()
        {
            if (this.menu != null)
            {
                this.menu.PreviewMouseDown -= OnMouseDown;
                this.menu.PreviewMouseMove -= OnMouseMove;
                this.menu.PreviewMouseUp -= OnMouseUp;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var position = e.GetPosition(this.source);

                if (position.X > 0 && position.Y > 0 && position.X < this.source.ActualWidth && position.Y < this.source.ActualHeight)
                {
                    this.startOffset = new Point(this.menu!.HorizontalOffset, this.menu.VerticalOffset);
                    this.startPosition = Mouse.PrimaryDevice.GetPosition(null);
                    this.startCaptured = e.MouseDevice.Captured;
                    this.isDragging = true;
                    e.MouseDevice.Capture(this.menu);
                    e.Handled = true;
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                var position = Mouse.PrimaryDevice.GetPosition(null);

                this.startOffset = new Point(this.menu!.HorizontalOffset, this.menu.VerticalOffset);

                var startScreenPosition = this.menu.PointToScreen(new Point());

                this.menu.HorizontalOffset = this.startOffset.X + position.X - this.startPosition.X;
                this.menu.VerticalOffset = this.startOffset.Y + position.Y - this.startPosition.Y;

                // revert offset change on placement target bounds
                var screenPosition = this.menu.PointToScreen(new Point());

                if (screenPosition.X == startScreenPosition.X)
                {
                    this.menu.HorizontalOffset = this.startOffset.X;
                }

                if (screenPosition.Y == startScreenPosition.Y)
                {
                    this.menu.VerticalOffset = this.startOffset.Y;
                }

                e.Handled = true;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.isDragging)
            {
                this.isDragging = false;
                e.MouseDevice.Capture(this.startCaptured);
                e.Handled = true;
            }
        }

        public static IDisposable Register(FrameworkElement source)
        {
            return new ContextMenuDragBehavior(source);
        }
    }
}
