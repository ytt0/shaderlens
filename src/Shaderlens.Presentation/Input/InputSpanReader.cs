namespace Shaderlens.Presentation.Input
{
    public interface IInputSpanReader
    {
        bool TryRead(string value, [MaybeNullWhen(false)] out IInputSpan inputSpan);
    }

    public class InputSpanReader : IInputSpanReader
    {
        private readonly IInputValueSerializer serializer;
        private readonly IInputSpanFactory factory;

        public InputSpanReader(IInputValueSerializer serializer, IInputSpanFactory factory)
        {
            this.serializer = serializer;
            this.factory = factory;
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

            inputSpan = factory.All(spans);
            return true;
        }

        private bool TryReadValue(string value, [MaybeNullWhen(false)] out IInputSpan inputSpan)
        {
            if (this.serializer.TryDeserialize(value, out Key key))
            {
                inputSpan = factory.Create(key);
                return true;
            }

            if (this.serializer.TryDeserialize(value, out ModifierKey modifierKey))
            {
                inputSpan = factory.Create(modifierKey);
                return true;
            }

            if (this.serializer.TryDeserialize(value, out MouseButton button))
            {
                inputSpan = factory.Create(button);
                return true;
            }

            if (this.serializer.TryDeserialize(value, out MouseScroll direction))
            {
                inputSpan = factory.Create(direction);
                return true;
            }

            inputSpan = null;
            return false;
        }
    }
}
