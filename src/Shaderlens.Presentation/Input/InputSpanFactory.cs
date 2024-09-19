namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanFactory
    {
        IInputSpan Create(object? value);
    }

    public static class InputSpanFactoryExtensions
    {
        public static IInputSpan All(this IInputSpanFactory factory, params object?[] values)
        {
            return new AllInputSpans(values.Select(factory.Create).ToArray());
        }
    }

    public class InputSpanFactory : IInputSpanFactory
    {
        public static readonly IInputSpanFactory Instance = new InputSpanFactory();

        private InputSpanFactory()
        {
        }

        public IInputSpan Create(object? value)
        {
            return value is null ? InputSpan.None :
                value is object[] inputs ? new AllInputSpans(inputs.Select(Create).ToArray()) :
                value is IInputSpan inputSpan ? inputSpan :
                value is Key key ? new KeyInputSpan(key) :
                value is ModifierKey modifierKey ? new ModifierKeyInputSpan(modifierKey) :
                value is MouseButton mouseButton ? new MouseButtonInputSpan(mouseButton) :
                value is MouseScroll mouseScroll ? new MouseScrollInputSpan(mouseScroll) :
                value is ModifierKeys ?
                    throw new NotSupportedException($"Unexpected modifier key type {typeof(ModifierKeys).FullName}, modifier key type {typeof(ModifierKey).FullName} should be used instead") :
                    throw new NotSupportedException($"Unexpected input type {value?.GetType().Name}");
        }
    }
}
