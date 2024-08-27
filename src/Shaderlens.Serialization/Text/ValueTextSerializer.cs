
namespace Shaderlens.Serialization.Text
{
    public class ValueTextSerializer
    {
        public static readonly ITextSerializer<bool> Bool = new BoolTextSerializer();
        public static readonly ITextSerializer<int> Int = new IntTextSerializer();
        public static readonly ITextSerializer<double> Double = new DoubleTextSerializer();

        private abstract class TextSerializer<T> : ITextSerializer<T>
        {
            public string Serialize(T value)
            {
                return value!.ToString()!.ToLowerInvariant();
            }

            public abstract bool TryDeserialize(string value, [MaybeNullWhen(false)] out T result);
        }

        private class BoolTextSerializer : TextSerializer<bool>
        {
            public override bool TryDeserialize(string value, [MaybeNullWhen(false)] out bool result)
            {
                return Boolean.TryParse(value.Trim().ToLowerInvariant(), out result);
            }
        }

        private class IntTextSerializer : TextSerializer<int>
        {
            public override bool TryDeserialize(string value, [MaybeNullWhen(false)] out int result)
            {
                return Int32.TryParse(value.Trim(), out result);
            }
        }

        private class DoubleTextSerializer : TextSerializer<double>
        {
            public override bool TryDeserialize(string value, [MaybeNullWhen(false)] out double result)
            {
                return System.Double.TryParse(value.Trim(), out result);
            }
        }
    }
}
