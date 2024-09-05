namespace Shaderlens.Presentation.Behaviors
{
    public class InputStateSourceBehavior : IDisposable, IInputStateSource
    {
        private class Scope : IDisposable
        {
            private readonly InputStateSourceBehavior behavior;

            public Scope(InputStateSourceBehavior behavior)
            {
                this.behavior = behavior;
            }

            public void Dispose()
            {
                this.behavior.skipKeyRepeat = default;
            }
        }

        private Key skipKeyRepeat;
        private Key lastProcessedKeyDown;

        private readonly FrameworkElement source;
        private readonly InputStateSource inputStateSource;

        private InputStateSourceBehavior(FrameworkElement source, IInputStateListener listner)
        {
            this.source = source;

            this.inputStateSource = new InputStateSource(source, listner);

            this.source.MouseDown += OnMouseDown;
            this.source.MouseUp += OnMouseUp;
            this.source.MouseWheel += OnMouseWheel;
            this.source.KeyDown += OnKeyDown;
            this.source.KeyUp += OnKeyUp;
            this.source.GotKeyboardFocus += OnGotKeyboardFocus;
            this.source.LostKeyboardFocus += OnLostKeyboardFocus;
            this.source.MouseEnter += OnMouseEnter;
            this.source.MouseLeave += OnMouseLeave;
        }

        public void Dispose()
        {
            this.source.MouseDown -= OnMouseDown;
            this.source.MouseUp -= OnMouseUp;
            this.source.MouseWheel -= OnMouseWheel;
            this.source.KeyDown -= OnKeyDown;
            this.source.KeyUp -= OnKeyUp;
            this.source.GotKeyboardFocus -= OnGotKeyboardFocus;
            this.source.LostKeyboardFocus -= OnLostKeyboardFocus;
        }

        public int Match(IInputSpan inputSpan)
        {
            return this.inputStateSource.Match(inputSpan);
        }

        public void Refresh()
        {
            this.inputStateSource.Refresh();
        }

        public void SkipLastKeyRepeat()
        {
            this.skipKeyRepeat = this.lastProcessedKeyDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBoxBase || (Keyboard.FocusedElement as DependencyObject)?.GetAncestor<TextBoxBase>() != null)
            {
                return;
            }

            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (key == this.skipKeyRepeat)
            {
                if (e.IsRepeat)
                {
                    e.Handled = true;
                    return;
                }

                this.skipKeyRepeat = default;
            }

            this.lastProcessedKeyDown = key;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBoxBase || (Keyboard.FocusedElement as DependencyObject)?.GetAncestor<TextBoxBase>() != null)
            {
                return;
            }

            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (key == this.skipKeyRepeat)
            {
                this.skipKeyRepeat = default;
            }

            this.lastProcessedKeyDown = default;
            this.inputStateSource.ProcessInputEvent(e);
        }

        private void OnGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            this.inputStateSource.Refresh();
        }

        private void OnLostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            this.source.Dispatcher.InvokeAsync(() =>
            {
                if (this.source.IsKeyboardFocusWithin)
                {
                    this.inputStateSource.Refresh();
                }
                else
                {
                    this.inputStateSource.ClearKeysState();
                }
            });
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            this.inputStateSource.Refresh();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.inputStateSource.ClearMouseButtonsState();
        }

        public static InputStateSourceBehavior Register(FrameworkElement source, IInputStateListener listener)
        {
            return new InputStateSourceBehavior(source, listener);
        }
    }
}
