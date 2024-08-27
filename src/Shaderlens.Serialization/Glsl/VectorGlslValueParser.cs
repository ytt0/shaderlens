namespace Shaderlens.Serialization.Glsl
{
    public partial class VectorGlslValueParser<T> : IGlslValueParser<Vector<T>>
    {
        public string ValueType { get; }

        private readonly IGlslValueParser<T> parser;
        private readonly int size;

        public VectorGlslValueParser(IGlslValueParser<T> parser, string valueType, int size)
        {
            this.parser = parser;
            this.ValueType = valueType;
            this.size = size;
        }

        public bool TryParse(string source, [MaybeNullWhen(false)] out Vector<T> result)
        {
            result = null;

            var match = VectorRegex().Match(source);
            if (match.Success)
            {
                var rawValues = match.Groups["values"].Value.Split(",");

                if (rawValues.Length != 1 && rawValues.Length != this.size || match.Groups["type"].Value != this.ValueType)
                {
                    return false;
                }

                var values = new List<T>();
                foreach (var rawValue in rawValues)
                {
                    if (!this.parser.TryParse(rawValue, out var value))
                    {
                        return false;
                    }

                    values.Add(value);
                }

                result = values.Count == 1 ? Vector.Create(this.size, values[0]) : Vector.Create(values);
                return true;
            }

            if (this.parser.TryParse(source, out var singleValue))
            {
                result = Vector.Create(this.size, singleValue);
                return true;
            }

            return false;
        }

        [GeneratedRegex(@"(?<type>(\w+))\s*\((?<values>[^)]+)\)")]
        private static partial Regex VectorRegex();
    }

    public static class VectorGlslValueParser
    {
        public static readonly VectorGlslValueParser<double> Vec2 = new VectorGlslValueParser<double>(GlslValueParser.Float, "vec2", 2);
        public static readonly VectorGlslValueParser<double> Vec3 = new VectorGlslValueParser<double>(GlslValueParser.Float, "vec3", 3);
        public static readonly VectorGlslValueParser<double> Vec4 = new VectorGlslValueParser<double>(GlslValueParser.Float, "vec4", 4);
        public static readonly VectorGlslValueParser<int> IVec2 = new VectorGlslValueParser<int>(GlslValueParser.Int, "ivec2", 2);
        public static readonly VectorGlslValueParser<int> IVec3 = new VectorGlslValueParser<int>(GlslValueParser.Int, "ivec3", 3);
        public static readonly VectorGlslValueParser<int> IVec4 = new VectorGlslValueParser<int>(GlslValueParser.Int, "ivec4", 4);
        public static readonly VectorGlslValueParser<uint> UVec2 = new VectorGlslValueParser<uint>(GlslValueParser.UInt, "uvec2", 2);
        public static readonly VectorGlslValueParser<uint> UVec3 = new VectorGlslValueParser<uint>(GlslValueParser.UInt, "uvec3", 3);
        public static readonly VectorGlslValueParser<uint> UVec4 = new VectorGlslValueParser<uint>(GlslValueParser.UInt, "uvec4", 4);
        public static readonly VectorGlslValueParser<bool> BVec2 = new VectorGlslValueParser<bool>(GlslValueParser.Bool, "bvec2", 2);
        public static readonly VectorGlslValueParser<bool> BVec3 = new VectorGlslValueParser<bool>(GlslValueParser.Bool, "bvec3", 3);
        public static readonly VectorGlslValueParser<bool> BVec4 = new VectorGlslValueParser<bool>(GlslValueParser.Bool, "bvec4", 4);
    }
}
