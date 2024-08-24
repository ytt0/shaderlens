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
}
