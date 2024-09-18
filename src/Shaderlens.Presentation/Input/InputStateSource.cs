namespace Shaderlens.Presentation.Input
{
    public interface IInputStateSource
    {
        int Match(IInputSpan inputSpan);
    }

    public static class InputSourceExtensions
    {
        public static bool IsMatch(this IInputStateSource inputSource, IInputSpan inputSpan)
        {
            return inputSource.Match(inputSpan) > 0;
        }
    }

    public class InputStateSource : IInputStateSource
    {
        private const int KeysCount = 173;
        private const int MouseButtonsCount = 5;

        private readonly FrameworkElement source;
        private readonly IInputStateListener listener;

        private readonly bool[] keysDown;
        private readonly bool[] previousKeysDown;
        private readonly bool[] mouseButtonsDown;
        private readonly bool[] previousMouseButtonsDown;

        public InputStateSource(FrameworkElement source, IInputStateListener listener)
        {
            this.source = source;
            this.listener = listener;

            this.keysDown = new bool[KeysCount];
            this.previousKeysDown = new bool[KeysCount];

            this.mouseButtonsDown = new bool[MouseButtonsCount];
            this.previousMouseButtonsDown = new bool[MouseButtonsCount];
        }

        public int Match(IInputSpan inputSpan)
        {
            var state = new InputState(this.keysDown, this.mouseButtonsDown, null);
            return inputSpan?.Match(state) ?? 0;
        }

        public void Refresh()
        {
            var isChanged = false;

            SetNextState();

            for (var i = 1; i < KeysCount; i++)
            {
                var isDown = Keyboard.PrimaryDevice.IsKeyDown((Key)i);
                this.keysDown[i] = isDown;

                isChanged |= this.previousKeysDown[i] != isDown;
            }

            var isMouseOver = this.source.IsMouseOver;
            this.mouseButtonsDown[0] = (this.mouseButtonsDown[0] || isMouseOver) && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed;
            this.mouseButtonsDown[1] = (this.mouseButtonsDown[1] || isMouseOver) && Mouse.PrimaryDevice.MiddleButton == MouseButtonState.Pressed;
            this.mouseButtonsDown[2] = (this.mouseButtonsDown[2] || isMouseOver) && Mouse.PrimaryDevice.RightButton == MouseButtonState.Pressed;
            this.mouseButtonsDown[3] = (this.mouseButtonsDown[3] || isMouseOver) && Mouse.PrimaryDevice.XButton1 == MouseButtonState.Pressed;
            this.mouseButtonsDown[4] = (this.mouseButtonsDown[4] || isMouseOver) && Mouse.PrimaryDevice.XButton2 == MouseButtonState.Pressed;

            for (var i = 0; i < MouseButtonsCount; i++)
            {
                isChanged |= this.previousMouseButtonsDown[i] != this.mouseButtonsDown[i];
            }

            if (isChanged)
            {
                TryHandleInputStateChange(false);
            }
        }

        public void ClearKeysState()
        {
            var isChanged = false;

            SetNextState();

            for (var i = 0; i < KeysCount; i++)
            {
                if (this.keysDown[i])
                {
                    this.keysDown[i] = false;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                TryHandleInputStateChange(false);
            }
        }

        public void ClearMouseButtonsState()
        {
            var isChanged = false;

            SetNextState();

            for (var i = 0; i < MouseButtonsCount; i++)
            {
                if (this.mouseButtonsDown[i])
                {
                    this.mouseButtonsDown[i] = false;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                TryHandleInputStateChange(false);
            }
        }

        public void ProcessInputEvent(KeyEventArgs e)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            var repeatKeyUpHandled = false;

            if (e.IsDown && e.IsRepeat)
            {
                SetNextState();
                this.keysDown[(int)key] = false;
                if (!e.Handled)
                {
                    repeatKeyUpHandled = TryHandleInputStateChange(true);
                }
            }

            SetNextState();
            this.keysDown[(int)key] = e.IsDown;
            if (!e.Handled)
            {
                e.Handled = TryHandleInputStateChange(e.IsRepeat) || repeatKeyUpHandled;
            }
        }

        public void ProcessInputEvent(MouseButtonEventArgs e)
        {
            SetNextState();
            this.mouseButtonsDown[(int)e.ChangedButton] = e.ButtonState == MouseButtonState.Pressed;
            if (!e.Handled)
            {
                e.Handled = TryHandleInputStateChange(false);
            }
        }

        public void ProcessInputEvent(MouseWheelEventArgs e)
        {
            var mouseScroll = e.Delta > 0 ? MouseScroll.ScrollUp : MouseScroll.ScrollDown;

            var noScrollState = new InputState(this.keysDown, this.mouseButtonsDown, null);
            var scrollState = new InputState(this.keysDown, this.mouseButtonsDown, mouseScroll);

            var scrollStartEvent = new InputSpanEventArgs(noScrollState, scrollState, false);
            var scrollEndEvent = new InputSpanEventArgs(scrollState, noScrollState, false);

            this.listener.InputStateChanged(scrollStartEvent);
            this.listener.InputStateChanged(scrollEndEvent);

            e.Handled = scrollStartEvent.Handled || scrollEndEvent.Handled;
        }

        private bool TryHandleInputStateChange(bool isRepeat)
        {
            var previousState = new InputState(this.previousKeysDown, this.previousMouseButtonsDown, null);
            var state = new InputState(this.keysDown, this.mouseButtonsDown, null);

            var stateChangedEvent = new InputSpanEventArgs(previousState, state, isRepeat);
            this.listener.InputStateChanged(stateChangedEvent);
            return stateChangedEvent.Handled;
        }

        private void SetNextState()
        {
            Array.Copy(this.keysDown, this.previousKeysDown, KeysCount);
            Array.Copy(this.mouseButtonsDown, this.previousMouseButtonsDown, MouseButtonsCount);
        }
    }
}
