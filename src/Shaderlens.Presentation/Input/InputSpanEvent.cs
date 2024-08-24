namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanEvent
    {
        int Match(IInputState previousState, IInputState state);
    }

    public static class InputSpanEventExtensions
    {
        public static bool IsMatch(this IInputSpanEvent inputSpanEvent, IInputState previousState, IInputState state)
        {
            return inputSpanEvent.Match(previousState, state) > 0;
        }
    }

    public class StartInputSpanEvent : IInputSpanEvent
    {
        private readonly IInputSpan inputSpan;

        public StartInputSpanEvent(IInputSpan inputSpan)
        {
            this.inputSpan = inputSpan;
        }

        public int Match(IInputState previousState, IInputState state)
        {
            return this.inputSpan.Match(previousState) == 0 ? this.inputSpan.Match(state) : 0;
        }
    }

    public class EndInputSpanEvent : IInputSpanEvent
    {
        private readonly IInputSpan inputSpan;

        public EndInputSpanEvent(IInputSpan inputSpan)
        {
            this.inputSpan = inputSpan;
        }

        public int Match(IInputState previousState, IInputState state)
        {
            return this.inputSpan.Match(state) == 0 ? this.inputSpan.Match(previousState) : 0;
        }
    }
}
