namespace Shaderlens.Presentation.Input
{
    public interface IInputPositionBinding
    {
        IDisposable PushScope(bool enableCursorWrap, Action<Point> positionChangedHandler);
    }

    public class InputPositionBinding : IInputPositionBinding
    {
        private class Scope : IDisposable
        {
            private readonly InputPositionBinding binding;
            private readonly Action<Point> handler;

            public Scope(InputPositionBinding binding, Action<Point> handler)
            {
                this.binding = binding;
                this.handler = handler;
            }

            public void Dispose()
            {
                this.binding.PopScope(this);
            }

            public void ProcessPosition()
            {
                this.handler(this.binding.mousePositionSource.GetPosition());
            }
        }

        private readonly IMousePositionSource mousePositionSource;
        private readonly Stack<Scope?> scopesStack;
        private readonly DispatcherTimer timer;
        private Scope? scope;

        public InputPositionBinding(Dispatcher dispatcher, IMousePositionSource mousePositionSource)
        {
            this.mousePositionSource = mousePositionSource;
            this.scopesStack = new Stack<Scope?>();
            this.timer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher) { Interval = TimeSpan.FromSeconds(1.0 / 60.0) };
            this.timer.Tick += (sender, e) => ProcessPosition();
        }

        public void ProcessPosition()
        {
            this.scope?.ProcessPosition();
        }

        public IDisposable PushScope(bool enableCursorWrap, Action<Point> positionChangedHandler)
        {
            if (this.scope == null)
            {
                CaptureMouse(enableCursorWrap);
            }

            this.scopesStack.Push(this.scope);
            this.scope = new Scope(this, positionChangedHandler);

            return this.scope;
        }

        private void PopScope(Scope scope)
        {
            if (this.scope != scope)
            {
                throw new Exception("Cannot restore input position scope, scopes were disposed out of order");
            }

            this.scope = this.scopesStack.Pop();

            if (this.scope == null)
            {
                ReleaseMouseCapture();
            }
        }

        private void CaptureMouse(bool enableCursorWrap)
        {
            if (Mouse.PrimaryDevice.Captured != null)
            {
                throw new Exception("Failed to acquire mouse capture");
            }

            this.mousePositionSource.StartCapture(enableCursorWrap);

            this.timer.IsEnabled = !AnyMouseButtonPressed();
        }

        private void ReleaseMouseCapture()
        {
            this.mousePositionSource.EndCapture();

            this.timer.IsEnabled = false;
        }

        private static bool AnyMouseButtonPressed()
        {
            return Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.MiddleButton == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.RightButton == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.XButton1 == MouseButtonState.Pressed ||
                Mouse.PrimaryDevice.XButton2 == MouseButtonState.Pressed;
        }
    }
}
