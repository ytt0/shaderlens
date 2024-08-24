namespace Shaderlens.Render
{
    public interface IMouseStateSource
    {
        int MouseX { get; }
        int MouseY { get; }
        int MousePressedX { get; }
        int MousePressedY { get; }
        bool IsMouseDown { get; }
        bool IsMousePressed { get; }

        void TranslatePosition(int viewportWidth, int viewportHeight, int renderDownscale);

        void SetNextState(bool isMouseDown);
        void PushState();
        void PopState();
        void ClearState();
    }

    public class MouseStateSource : IMouseStateSource
    {
        private struct State
        {
            public int MouseX;
            public int MouseY;
            public int MousePressedX;
            public int MousePressedY;
            public bool IsMouseDown;
            public bool IsMousePressed;
            public int ViewportWidth;
            public int ViewportHeight;
            public int RenderDownscale;
        }

        public int MouseX { get; private set; }
        public int MouseY { get; private set; }
        public int MousePressedX { get; private set; }
        public int MousePressedY { get; private set; }
        public bool IsMouseDown { get; private set; }
        public bool IsMousePressed { get; private set; }

        private readonly IThreadAccess threadAccess;
        private readonly IMousePositionSource mousePositionSource;
        private readonly Queue<State> states;

        private int viewportWidth;
        private int viewportHeight;
        private int renderDownscale;

        public MouseStateSource(IThreadAccess threadAccess, IMousePositionSource mousePositionSource)
        {
            this.threadAccess = threadAccess;
            this.mousePositionSource = mousePositionSource;
            this.states = new Queue<State>();
        }

        public void SetNextState(bool isMouseDown)
        {
            this.threadAccess.Verify();

            this.IsMousePressed = isMouseDown && !this.IsMouseDown;
            this.IsMouseDown = isMouseDown;

            if (this.IsMouseDown)
            {
                this.mousePositionSource.GetPosition(out var mouseX, out var mouseY);

                this.MouseX = (int)mouseX;
                this.MouseY = (int)mouseY;

                if (this.IsMousePressed)
                {
                    this.MousePressedX = (int)mouseX;
                    this.MousePressedY = (int)mouseY;
                }
            }
        }

        public void PushState()
        {
            this.threadAccess.Verify();

            this.states.Enqueue(new State
            {
                MouseX = this.MouseX,
                MouseY = this.MouseY,
                MousePressedX = this.MousePressedX,
                MousePressedY = this.MousePressedY,
                IsMouseDown = this.IsMouseDown,
                IsMousePressed = this.IsMousePressed,
                ViewportWidth = this.viewportWidth,
                ViewportHeight = this.viewportHeight,
                RenderDownscale = this.renderDownscale
            });
        }

        public void PopState()
        {
            this.threadAccess.Verify();

            var state = this.states.Dequeue();

            this.MouseX = state.MouseX;
            this.MouseY = state.MouseY;
            this.MousePressedX = state.MousePressedX;
            this.MousePressedY = state.MousePressedY;
            this.IsMouseDown = state.IsMouseDown;
            this.IsMousePressed = state.IsMousePressed;
            this.viewportWidth = state.ViewportWidth;
            this.viewportHeight = state.ViewportHeight;
            this.renderDownscale = state.RenderDownscale;
        }

        public void ClearState()
        {
            this.threadAccess.Verify();

            this.MouseX = default;
            this.MouseY = default;
            this.MousePressedX = default;
            this.MousePressedY = default;
            this.IsMouseDown = default;
            this.IsMousePressed = default;
            this.viewportWidth = default;
            this.viewportHeight = default;
            this.renderDownscale = default;
        }

        public void TranslatePosition(int viewportWidth, int viewportHeight, int renderDownscale)
        {
            this.threadAccess.Verify();

            if (this.viewportWidth != 0 && this.viewportHeight != 0 && renderDownscale != 0)
            {
                var renderScale = (double)this.renderDownscale / renderDownscale;
                var viewportScaleX = renderScale * viewportWidth / this.viewportWidth;
                var viewportScaleY = renderScale * viewportHeight / this.viewportHeight;

                this.MouseX = (int)(this.MouseX * viewportScaleX);
                this.MouseY = (int)(this.MouseY * viewportScaleY);
                this.MousePressedX = (int)(this.MousePressedX * viewportScaleX);
                this.MousePressedY = (int)(this.MousePressedY * viewportScaleY);
            }

            this.viewportWidth = viewportWidth;
            this.viewportHeight = viewportHeight;
            this.renderDownscale = renderDownscale;
        }
    }
}
