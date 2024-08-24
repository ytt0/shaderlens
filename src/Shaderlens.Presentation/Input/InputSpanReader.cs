namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanReader
    {
        bool TryRead(string value, [MaybeNullWhen(false)] out IInputSpan inputSpan);
    }

    public class InputSpanReader : IInputSpanReader
    {
        private readonly IInputValueSerializer serializer;

        public InputSpanReader(IInputValueSerializer serializer)
        {
            this.serializer = serializer;
        }

        public bool TryRead(string value, [MaybeNullWhen(false)] out IInputSpan inputSpan)
        {
            var items = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0)
            {
                inputSpan = InputSpan.None;
                return true;
            }

            if (items.Length == 1)
            {
                return TryReadValue(items[0], out inputSpan);
            }

            var spans = new List<IInputSpan>();

            foreach (var item in items)
            {
                if (!TryReadValue(item, out var span))
                {
                    inputSpan = null;
                    return false;
                }

                spans.Add(span);
            }

            inputSpan = new AllInputSpans(spans);
            return true;
        }

        private bool TryReadValue(string value, [MaybeNullWhen(false)] out IInputSpan inputSpan)
        {
            if (this.serializer.TryDeserialize(value, out Key key))
            {
                inputSpan = new KeyInputSpan(key);
                return true;
            }

            if (this.serializer.TryDeserialize(value, out ModifierKey modifierKey))
            {
                inputSpan = new ModifierKeyInputSpan(modifierKey);
                return true;
            }

            if (this.serializer.TryDeserialize(value, out MouseButton button))
            {
                inputSpan = new MouseButtonInputSpan(button);
                return true;
            }

            if (this.serializer.TryDeserialize(value, out MouseScroll direction))
            {
                inputSpan = new MouseScrollInputSpan(direction);
                return true;
            }

            inputSpan = null;
            return false;
        }
    }
}
