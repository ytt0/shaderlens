namespace Shaderlens.Presentation.Behaviors
{
    public class TextBoxSelectionBehavior : IDisposable
    {
        private readonly TextBox target;
        private bool restoreSelection;
        private int selectionStart;
        private int selectionLength;
        private bool skipMenuReopen;

        public TextBoxSelectionBehavior(TextBox target)
        {
            this.target = target;
            this.target.PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
            this.target.PreviewKeyUp += OnPreviewKeyUp;
            this.target.PreviewMouseDown += OnPreviewMouseDown;
            this.target.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
            this.target.ContextMenuOpening += OnContextMenuOpening;
            this.target.ContextMenuClosing += OnContextMenuClosing;
        }

        public void Dispose()
        {
            this.target.PreviewGotKeyboardFocus -= OnPreviewGotKeyboardFocus;
            this.target.PreviewKeyUp -= OnPreviewKeyUp;
            this.target.PreviewMouseDown -= OnPreviewMouseDown;
            this.target.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;
            this.target.ContextMenuOpening -= OnContextMenuOpening;
            this.target.ContextMenuClosing -= OnContextMenuClosing;
        }

        private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.skipMenuReopen = false;
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            this.restoreSelection = false;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.restoreSelection)
            {
                this.target.Select(this.selectionStart, this.selectionLength);
                this.restoreSelection = false;
                e.Handled = true;
            }

            if (e.ClickCount == 3)
            {
                this.target.SelectAll();
                e.Handled = true;
            }
        }

        private void OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.skipMenuReopen)
            {
                this.skipMenuReopen = false;
                e.Handled = true;
                return;
            }

            this.restoreSelection = true;
            this.selectionStart = this.target.SelectionStart;
            this.selectionLength = this.target.SelectionLength;
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.restoreSelection)
            {
                this.target.Select(this.selectionStart, this.selectionLength);
            }
        }

        private void OnContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            this.restoreSelection = false;

            if (Mouse.PrimaryDevice.RightButton == MouseButtonState.Pressed)
            {
                this.skipMenuReopen = true;
            }
        }

        public static IDisposable Register(TextBox target)
        {
            return new TextBoxSelectionBehavior(target);
        }
    }
}
