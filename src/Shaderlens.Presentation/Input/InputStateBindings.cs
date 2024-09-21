namespace Shaderlens.Presentation.Input
{
    public interface IInputStateBindings
    {
        IDisposable PushScope();
        void Add(IInputSpan inputSpan, Action? startHandler, Action? endHandler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat);
        void AddGlobal(IInputSpan inputSpan, Action handler, Func<bool>? isEnabled, bool allowRepeat);
    }

    public interface IInputStateListener
    {
        void InputStateChanged(InputSpanEventArgs e);
    }

    public static class InputStateBindingsExtensions
    {
        public static void Add(this IInputStateBindings bindings, IInputSpanEvent inputSpanEvent, Action handler, Func<bool>? isEnabled = null, bool requireSpanStart = true, bool allowRepeat = false)
        {
            inputSpanEvent.AddTo(bindings, handler, isEnabled, requireSpanStart, allowRepeat);
        }
    }

    public class InputSpanEventArgs : EventArgs
    {
        public IInputState PreviousState { get; }
        public IInputState State { get; }
        public bool IsRepeat { get; }
        public bool Handled { get; set; }

        public InputSpanEventArgs(IInputState previousState, IInputState state, bool isRepeat)
        {
            this.PreviousState = previousState;
            this.State = state;
            this.IsRepeat = isRepeat;
        }
    }

    public class InputStateBindings : IInputStateBindings, IInputStateListener
    {
        private record Binding(IInputSpan InputSpan, Action? StartHandler, Action? EndHandler, Func<bool>? IsEnabled, bool RequireSpanStart, bool AllowRepeat);
        private record BindingMatch(IInputSpan InputSpan, int Length, Action? Handler);

        private class Scope : IDisposable
        {
            private readonly InputStateBindings inputStateBindings;
            private readonly List<Binding> bindings;

            private IInputSpan? startInputSpan;
            private bool isDisposed;

            public Scope(InputStateBindings inputStateBindings)
            {
                this.inputStateBindings = inputStateBindings;
                this.bindings = new List<Binding>();
            }

            public void Dispose()
            {
                if (this.isDisposed)
                {
                    throw new Exception("Scope has already been disposed");
                }

                this.isDisposed = true;
                this.inputStateBindings.PopScope(this);
            }

            public void AddBinding(Binding binding)
            {
                this.bindings.Add(binding);
            }

            public void TryInvoke(InputSpanEventArgs e)
            {
                if (e.Handled)
                {
                    return;
                }

                var startMatches = new List<BindingMatch>();
                var endMatches = new List<BindingMatch>();

                foreach (var binding in this.bindings)
                {
                    if (e.IsRepeat && !binding.AllowRepeat || binding.IsEnabled != null && !binding.IsEnabled())
                    {
                        continue;
                    }

                    var startLength = MatchStart(binding.InputSpan, e.PreviousState, e.State);
                    if (startLength > 0)
                    {
                        startMatches.Add(new BindingMatch(binding.InputSpan, startLength, binding.StartHandler));
                    }

                    if (!binding.RequireSpanStart || binding.InputSpan == this.startInputSpan)
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
                    e.Handled = e.IsRepeat && this.startInputSpan?.Match(e.State) > 0;
                    return;
                }

                var bindingMatch = endMatches.OrderByDescending(match => match.InputSpan == this.startInputSpan ? 1 : 0).OrderByDescending(bindingMatch => bindingMatch.Length).FirstOrDefault();
                if (bindingMatch != null)
                {
                    bindingMatch.Handler?.Invoke();
                    this.startInputSpan = null;
                    e.Handled = true;
                    return;
                }

                bindingMatch = startMatches.MaxBy(bindingMatch => bindingMatch.Length);
                if (bindingMatch != null)
                {
                    bindingMatch.Handler?.Invoke();
                    this.startInputSpan = bindingMatch.InputSpan;
                    e.Handled = true;
                    return;
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
        private readonly IGlobalInputBindings globalInputBindings;
        private Scope scope;

        public InputStateBindings(IGlobalInputBindings globalInputBindings)
        {
            this.scopesStack = new Stack<Scope>();
            this.scope = new Scope(this);
            this.globalInputBindings = globalInputBindings;
        }

        public void InputStateChanged(InputSpanEventArgs e)
        {
            this.scope.TryInvoke(e);
        }

        public void Add(IInputSpan inputSpan, Action? startHandler, Action? endHandler, Func<bool>? isEnabled, bool requireSpanStart, bool allowRepeat)
        {
            this.scope.AddBinding(new Binding(inputSpan, startHandler, endHandler, isEnabled, requireSpanStart, allowRepeat));
        }

        public void AddGlobal(IInputSpan inputSpan, Action handler, Func<bool>? isEnabled, bool allowRepeat)
        {
            this.globalInputBindings.Add(inputSpan, handler, isEnabled, allowRepeat);
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
