using Shaderlens.Presentation.Extensions;

namespace Shaderlens.Presentation.Input
{
    public interface IInputPositionBinding
    {
        IDisposable PushScope(Action<Point> positionChangedHandler);
    }

    public interface IInputPositionListener
    {
        void InputPositionChanged(Point position);
    }

    public class InputPositionBinding : IInputPositionBinding, IInputPositionListener
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

            public void InputPositionChanged(Point position)
            {
                this.handler(position);
            }
        }

        private readonly Stack<Scope?> scopesStack;
        private Scope? scope;
        private Point lastPosition;

        public InputPositionBinding()
        {
            this.scopesStack = new Stack<Scope?>();
        }

        public void InputPositionChanged(Point position)
        {
            if (this.lastPosition.DistanceApprox(position) >= 1.0)
            {
                this.lastPosition = position;
                this.scope?.InputPositionChanged(position);
            }
        }

        public IDisposable PushScope(Action<Point> positionChangedHandler)
        {
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
        }
    }
}
