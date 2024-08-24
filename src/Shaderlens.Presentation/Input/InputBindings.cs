namespace Shaderlens.Presentation.Input
{
    public delegate void InputSpanEventHandler(InputSpanEventArgs e);

    public interface IInputBindings
    {
        IDisposable PushScope();
        void Add(IInputSpan inputSpan, InputSpanEventHandler startHandler, InputSpanEventHandler endHandler, bool continuous);
    }

    public interface IInputListener
    {
        void InputStateChanged(InputSpanEventArgs e);
    }

    public class InputSpanEventArgs : EventArgs
    {
        public IInputState PreviousState { get; }
        public IInputState State { get; }
        public bool Handled { get; set; }

        public InputSpanEventArgs(IInputState previousState, IInputState state)
        {
            this.PreviousState = previousState;
            this.State = state;
        }
    }

    public static class InputBindingsExtensions
    {
        private static readonly InputSpanEventHandler EmptyAction = e => e.Handled = true;

        public static void AddSpanStart(this IInputBindings inputBindings, IInputSpan? inputSpan, InputSpanEventHandler startAction)
        {
            inputBindings.AddSpan(inputSpan, startAction, EmptyAction, true);
        }

        public static void AddSpanStart(this IInputBindings inputBindings, IInputSpan? inputSpan, Action startAction)
        {
            inputBindings.AddSpan(inputSpan, e => { startAction(); e.Handled = true; }, EmptyAction, true);
        }

        public static void AddSpanEnd(this IInputBindings inputBindings, IInputSpan? inputSpan, InputSpanEventHandler endAction, bool continuous = false)
        {
            inputBindings.AddSpan(inputSpan, EmptyAction, endAction, continuous);
        }

        public static void AddSpanEnd(this IInputBindings inputBindings, IInputSpan? inputSpan, Action endAction, bool continuous = false)
        {
            inputBindings.AddSpan(inputSpan, EmptyAction, e => { endAction(); e.Handled = true; }, continuous);
        }

        public static void AddSpan(this IInputBindings inputBindings, IInputSpan? inputSpan, Action startAction, Action endAction, bool continuous = false)
        {
            inputBindings.AddSpan(inputSpan, e => { startAction(); e.Handled = true; }, e => { endAction(); e.Handled = true; }, continuous);
        }

        public static void AddSpan(this IInputBindings inputBindings, IInputSpan? inputSpan, InputSpanEventHandler startAction, InputSpanEventHandler endAction, bool continuous = false)
        {
            if (inputSpan != null)
            {
                inputBindings.Add(inputSpan, startAction, endAction, continuous);
            }
        }
    }

    public class InputBindings : IInputBindings, IInputListener
    {
        private record Binding(IInputSpan InputSpan, InputSpanEventHandler StartHandler, InputSpanEventHandler EndHandler, bool Continuous);
        private record BindingMatch(IInputSpan InputSpan, int Length, InputSpanEventHandler Handler);

        private class Scope : IDisposable
        {
            private readonly InputBindings inputBindings;
            private readonly List<Binding> bindings;

            private IInputSpan? startInputSpan;
            private bool isDisposed;

            public Scope(InputBindings inputBindings)
            {
                this.inputBindings = inputBindings;
                this.bindings = new List<Binding>();
            }

            public void Dispose()
            {
                if (this.isDisposed)
                {
                    throw new Exception("Scope has already been disposed");
                }

                this.isDisposed = true;
                this.inputBindings.PopScope(this);
            }

            public void AddBinding(Binding binding)
            {
                this.bindings.Add(binding);
            }

            public void TryInvoke(InputSpanEventArgs e)
            {
                var startMatches = new List<BindingMatch>();
                var endMatches = new List<BindingMatch>();

                if (e.Handled)
                {
                    return;
                }

                foreach (var binding in this.bindings)
                {
                    var startLength = MatchStart(binding.InputSpan, e.PreviousState, e.State);
                    if (startLength > 0)
                    {
                        startMatches.Add(new BindingMatch(binding.InputSpan, startLength, binding.StartHandler));
                    }

                    if (!binding.Continuous || binding.InputSpan == this.startInputSpan)
                    {
                        var endLength = MatchEnd(binding.InputSpan, e.PreviousState, e.State);
                        if (endLength > 0)
                        {
                            endMatches.Add(new BindingMatch(binding.InputSpan, endLength, binding.EndHandler));
                        }
                    }
                }

                if (startMatches.Count == 0 && endMatches.Count == 0)
                {
                    return;
                }

                foreach (var bindingMatch in endMatches.OrderByDescending(match => match.InputSpan == this.startInputSpan ? 1 : 0).OrderByDescending(bindingMatch => bindingMatch.Length))
                {
                    bindingMatch.Handler(e);

                    if (e.Handled)
                    {
                        this.startInputSpan = null;
                        return;
                    }
                }

                foreach (var bindingMatch in startMatches.OrderByDescending(bindingMatch => bindingMatch.Length))
                {
                    bindingMatch.Handler(e);

                    if (e.Handled)
                    {
                        this.startInputSpan = bindingMatch.InputSpan;
                        return;
                    }
                }
            }

            private static int MatchStart(IInputSpan inputSpan, IInputState previousState, IInputState state)
            {
                return inputSpan.Match(previousState) == 0 ? inputSpan.Match(state) : 0;
            }

            public static int MatchEnd(IInputSpan inputSpan, IInputState previousState, IInputState state)
            {
                return inputSpan.Match(state) == 0 ? inputSpan.Match(previousState) : 0;
            }
        }

        private readonly Stack<Scope> scopesStack;
        private Scope scope;

        public InputBindings()
        {
            this.scopesStack = new Stack<Scope>();
            this.scope = new Scope(this);
        }

        public void InputStateChanged(InputSpanEventArgs e)
        {
            this.scope.TryInvoke(e);
        }

        public void Add(IInputSpan inputSpan, InputSpanEventHandler startHandler, InputSpanEventHandler endHandler, bool continuous)
        {
            this.scope.AddBinding(new Binding(inputSpan, startHandler, endHandler, continuous));
        }

        public IDisposable PushScope()
        {
            this.scopesStack.Push(this.scope);
            this.scope = new Scope(this);
            return this.scope;
        }

        private void PopScope(Scope scope)
        {
            if (this.scope != scope)
            {
                throw new Exception("Cannot restore input scope, scopes were disposed out of order");
            }

            this.scope = this.scopesStack.Pop();
        }
    }
}
