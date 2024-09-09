namespace Shaderlens.Presentation.Behaviors
{
    public class MultiToggleButtonClickBehavior : IDisposable
    {
        public static readonly DependencyProperty GroupKeyProperty = MultiButtonClickBehavior.GroupKeyProperty.AddOwner(typeof(MultiToggleButtonClickBehavior));

        private readonly FrameworkElement container;
        private readonly RoutedEventHandler clickedHandler;
        private ToggleButton? source;
        private object? sourceKey;

        public MultiToggleButtonClickBehavior(FrameworkElement container)
        {
            this.container = container;

            this.clickedHandler = new RoutedEventHandler(OnClicked);

            this.container.PreviewMouseDown += OnPreviewMouseDown;
            this.container.MouseMove += OnMouseMove;
            this.container.AddHandler(ButtonBase.ClickEvent, this.clickedHandler);
        }

        public void Dispose()
        {
            this.container.PreviewMouseDown -= OnPreviewMouseDown;
            this.container.MouseMove -= OnMouseMove;
            this.container.RemoveHandler(ButtonBase.ClickEvent, this.clickedHandler);
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            if (this.source == null)
            {
                this.source = e.OriginalSource as ToggleButton;
                this.sourceKey = this.source?.GetValue(GroupKeyProperty);
                Mouse.PrimaryDevice.Capture(null);
            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.source = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.source == null || e.LeftButton == MouseButtonState.Released)
            {
                return;
            }

            var target = e.OriginalSource as ToggleButton ?? (e.OriginalSource as DependencyObject)?.GetAncestor<ToggleButton>();
            var targetKey = target?.GetValue(GroupKeyProperty);
            if (target != null && target != this.source && targetKey == this.sourceKey)
            {
                if (target.IsChecked != this.source.IsChecked)
                {
                    target.IsChecked = this.source.IsChecked;
                    target.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }

                target.Focus();
            }
        }

        public static IDisposable Register(FrameworkElement container)
        {
            return new MultiToggleButtonClickBehavior(container);
        }
    }
}
