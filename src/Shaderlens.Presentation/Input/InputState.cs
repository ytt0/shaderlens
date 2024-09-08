namespace Shaderlens.Presentation.Input
{
    public interface IInputState
    {
        bool IsKeyDown(Key key);
        bool IsMouseButtonDown(MouseButton button);
        bool IsMouseScroll(MouseScroll direction);
    }

    public static class InputStateExtensions
    {
        public static bool IsMatch(this IInputState state, IInputSpan inputSpan)
        {
            return inputSpan.Match(state) > 0;
        }

        public static bool IsKeyDown(this IInputState state, ModifierKey modifierKey)
        {
            switch (modifierKey)
            {
                case ModifierKey.Alt: return state.IsKeyDown(Key.LeftAlt) || state.IsKeyDown(Key.RightAlt);
                case ModifierKey.Ctrl: return state.IsKeyDown(Key.LeftCtrl) || state.IsKeyDown(Key.RightCtrl);
                case ModifierKey.Shift: return state.IsKeyDown(Key.LeftShift) || state.IsKeyDown(Key.RightShift);
            }

            throw new NotSupportedException($"Unexpected {nameof(ModifierKey)} {modifierKey}");
        }
    }

    public class InputState : IInputState
    {
        private readonly bool[] keysDown;
        private readonly bool[] mouseButtonsDown;
        private readonly MouseScroll? mouseScroll;

        public InputState(bool[] keysDown, bool[] mouseButtonsDown, MouseScroll? mouseScroll)
        {
            this.keysDown = keysDown;
            this.mouseButtonsDown = mouseButtonsDown;
            this.mouseScroll = mouseScroll;
        }

        public bool IsKeyDown(Key key)
        {
            return this.keysDown[(int)key];
        }

        public bool IsMouseButtonDown(MouseButton button)
        {
            return this.mouseButtonsDown[(int)button];
        }

        public bool IsMouseScroll(MouseScroll direction)
        {
            return this.mouseScroll == direction;
        }
    }

    public class PrimaryDeviceInputState : IInputState
    {
        public static readonly IInputState Instance = new PrimaryDeviceInputState();

        private PrimaryDeviceInputState()
        {
        }

        public bool IsKeyDown(Key key)
        {
            return Keyboard.PrimaryDevice.IsKeyDown(key);
        }

        public bool IsMouseButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left: return Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed;
                case MouseButton.Middle: return Mouse.PrimaryDevice.MiddleButton == MouseButtonState.Pressed;
                case MouseButton.Right: return Mouse.PrimaryDevice.RightButton == MouseButtonState.Pressed;
                case MouseButton.XButton1: return Mouse.PrimaryDevice.XButton1 == MouseButtonState.Pressed;
                case MouseButton.XButton2: return Mouse.PrimaryDevice.XButton2 == MouseButtonState.Pressed;
            }

            return false;
        }

        public bool IsMouseScroll(MouseScroll direction)
        {
            return false;
        }
    }
}
