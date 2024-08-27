namespace Shaderlens.Serialization.Glsl
{
    public interface IGlslValueParser<T>
    {
        string ValueType { get; }
        bool TryParse(string source, [MaybeNullWhen(false)] out T result);
    }

    public static class GlslValueParser
    {
        public static readonly IGlslValueParser<bool> Bool = new BoolGlslValueParser();
        public static readonly IGlslValueParser<int> Int = new IntGlslValueParser();
        public static readonly IGlslValueParser<uint> UInt = new UIntGlslValueParser();
        public static readonly IGlslValueParser<double> Float = new FloatGlslValueParser();

        private class BoolGlslValueParser : IGlslValueParser<bool>
        {
            public string ValueType { get { return "bool"; } }

            public bool TryParse(string source, [MaybeNullWhen(false)] out bool result)
            {
                source = source.Trim();
                result = source == "true";
                return source == "true" || source == "false";
            }
        }

        private class IntGlslValueParser : IGlslValueParser<int>
        {
            public string ValueType { get { return "int"; } }

            public bool TryParse(string source, [MaybeNullWhen(false)] out int result)
            {
                return Int32.TryParse(source.Trim(), out result);
            }
        }

        private class UIntGlslValueParser : IGlslValueParser<uint>
        {
            public string ValueType { get { return "uint"; } }

            public bool TryParse(string source, [MaybeNullWhen(false)] out uint result)
            {
                return UInt32.TryParse(source.Trim(), out result);
            }
        }

        private class FloatGlslValueParser : IGlslValueParser<double>
        {
            public string ValueType { get { return "float"; } }

            public bool TryParse(string source, [MaybeNullWhen(false)] out double result)
            {
                return System.Double.TryParse(source.Trim(), out result);
            }
        }
    }
}
