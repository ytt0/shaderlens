namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanFactory
    {
        IInputSpan Create(Key key);
        IInputSpan Create(ModifierKey modifierKey);
        IInputSpan Create(MouseButton mouseButton);
        IInputSpan Create(MouseScroll mouseScroll);
        IInputSpan All(IEnumerable<IInputSpan> inputSpans);
        IInputSpan Any(IEnumerable<IInputSpan> inputSpans);
    }

    public static class InputSpanFactoryExtensions
    {
        public static IInputSpan All(this IInputSpanFactory factory, params object?[] values)
        {
            return factory.All(values.Select(factory.CreateFromObject).ToArray());
        }

        public static IInputSpanEvent CreateStart(this IInputSpanFactory factory, object? value)
        {
            return factory.CreateFromObject(value).CreateStartEvent();
        }

        public static IInputSpanEvent CreateEnd(this IInputSpanFactory factory, object? value)
        {
            return factory.CreateFromObject(value).CreateEndEvent();
        }

        public static IInputSpanEvent AllStart(this IInputSpanFactory factory, params object?[] values)
        {
            return factory.All(values.Select(factory.CreateFromObject).ToArray()).CreateStartEvent();
        }

        public static IInputSpanEvent AllEnd(this IInputSpanFactory factory, params object?[] values)
        {
            return factory.All(values.Select(factory.CreateFromObject).ToArray()).CreateEndEvent();
        }

        public static IInputSpanEvent AllGlobal(this IInputSpanFactory factory, params object?[] values)
        {
            return factory.All(values.Select(factory.CreateFromObject).ToArray()).CreateGlobalEvent();
        }

        private static IInputSpan CreateFromObject(this IInputSpanFactory factory, object? value)
        {
            return value is null ? InputSpan.None :
                value is object?[] inputs ? factory.All(inputs.Select(factory.CreateFromObject).ToArray()) :
                value is IInputSpan inputSpan ? inputSpan :
                value is Key key ? factory.Create(key) :
                value is ModifierKey modifierKey ? factory.Create(modifierKey) :
                value is MouseButton mouseButton ? factory.Create(mouseButton) :
                value is MouseScroll mouseScroll ? factory.Create(mouseScroll) :
                value is ModifierKeys ?
                    throw new NotSupportedException($"Unexpected modifier key type {typeof(ModifierKeys).FullName}, modifier key type {typeof(ModifierKey).FullName} should be used instead") :
                    throw new NotSupportedException($"Unexpected input type {value?.GetType().Name}");
        }
    }

    public class InputSpanFactory : IInputSpanFactory
    {
        public static readonly IInputSpanFactory Instance = new InputSpanFactory();

        private InputSpanFactory()
        {
        }

        public IInputSpan Create(Key key)
        {
            return new KeyInputSpan(key);
        }

        public IInputSpan Create(ModifierKey modifierKey)
        {
            return new ModifierKeyInputSpan(modifierKey);
        }

        public IInputSpan Create(MouseButton mouseButton)
        {
            return new MouseButtonInputSpan(mouseButton);
        }

        public IInputSpan Create(MouseScroll mouseScroll)
        {
            return new MouseScrollInputSpan(mouseScroll);
        }

        public IInputSpan All(IEnumerable<IInputSpan> inputSpans)
        {
            return new AllInputSpans(inputSpans);
        }

        public IInputSpan Any(IEnumerable<IInputSpan> inputSpans)
        {
            return new AnyInputSpan(inputSpans);
        }
    }
}
