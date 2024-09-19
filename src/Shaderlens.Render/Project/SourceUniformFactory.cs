
using Shaderlens.Serialization.Glsl;

namespace Shaderlens.Render.Project
{
    public interface ISourceUniformFactory
    {
        IUniform CreateRootUniformGroup(IEnumerable<IUniform> uniforms);
        IUniform CreateUniformGroup(string name, string? displayName, IEnumerable<IUniform> uniforms, SourceLine sourceLine, IProjectSettings settings);
        IUniform CreateUniform(string name, string type, string value, ISourceLineAnnotationReader annotationReader, SourceLine sourceLine, IProjectSettings settings);
    }

    public partial class SourceUniformFactory : ISourceUniformFactory
    {
        private readonly DisplayNameFormatter displayNameFormatter;
        private readonly IDispatcherThread renderThread;

        public SourceUniformFactory(IDispatcherThread renderThread)
        {
            this.displayNameFormatter = new DisplayNameFormatter();

            this.renderThread = renderThread;
        }

        public IUniform CreateRootUniformGroup(IEnumerable<IUniform> uniforms)
        {
            return new RootUniformsGroup(uniforms);
        }

        public IUniform CreateUniformGroup(string name, string? displayName, IEnumerable<IUniform> uniforms, SourceLine sourceLine, IProjectSettings settings)
        {
            return new UniformsGroup(displayName ?? this.displayNameFormatter.GetDisplayName(name), settings.GetGroupExpandedValue(name, true), uniforms);
        }

        public IUniform CreateUniform(string name, string type, string value, ISourceLineAnnotationReader annotationReader, SourceLine sourceLine, IProjectSettings settings)
        {
            var displayName = GetDisplayName(name, annotationReader);

            if (annotationReader.TryGetValue("type", out var conversionType))
            {
                if (type == "vec3")
                {
                    if (conversionType.ToString() == "srgb")
                    {
                        var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(3, 0.0) : ParseValue(VectorGlslValueParser.Vec3, sourceLine, name, "default", value);
                        var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                        annotationReader.ValidateEmpty();
                        return new SrgbColorUniform(this.renderThread, displayName, settingsValue);
                    }

                    if (conversionType.ToString() == "linear-rgb")
                    {
                        var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(3, 0.0) : ParseValue(VectorGlslValueParser.Vec3, sourceLine, name, "default", value);
                        var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                        annotationReader.ValidateEmpty();
                        return new LinearRgbColorUniform(this.renderThread, displayName, settingsValue);
                    }

                    throw new SourceLineException($"Unexpected vec3 uniform conversion type \"{conversionType}\", supported types are \"srgb\", \"linear-rgb\"", annotationReader.SourceLine);
                }

                if (type == "vec4")
                {
                    if (conversionType.ToString() == "srgb")
                    {
                        var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(4, 0.0) : ParseValue(VectorGlslValueParser.Vec4, sourceLine, name, "default", value);
                        var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                        annotationReader.ValidateEmpty();
                        return new SrgbaColorUniform(this.renderThread, displayName, settingsValue);
                    }

                    if (conversionType.ToString() == "linear-rgb")
                    {
                        var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(4, 0.0) : ParseValue(VectorGlslValueParser.Vec4, sourceLine, name, "default", value);
                        var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                        annotationReader.ValidateEmpty();
                        return new LinearRgbaColorUniform(this.renderThread, displayName, settingsValue);
                    }

                    throw new SourceLineException($"Unexpected vec4 uniform conversion type \"{conversionType}\", supported types are \"srgb\", \"linear-rgb\"", annotationReader.SourceLine);
                }
            }

            if (type == "bool")
            {
                var parser = GlslValueParser.Bool;
                var defaultValue = !String.IsNullOrEmpty(value) && ParseValue(parser, sourceLine, name, "default", value);
                var settingsValue = settings.GetUniformValue(ValueJsonSerializer.Bool, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new BoolUniform(this.renderThread, displayName, settingsValue);
            }

            if (type == "int")
            {
                GetRangePropertiesValues(name, value, annotationReader, sourceLine, GlslValueParser.Int, 0, Int32.MinValue, Int32.MaxValue, 1, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = settings.GetUniformValue(ValueJsonSerializer.Int, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new IntUniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "float")
            {
                GetRangePropertiesValues(name, value, annotationReader, sourceLine, GlslValueParser.Float, 0.0, Double.MinValue, Double.MaxValue, 0.01, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = settings.GetUniformValue(ValueJsonSerializer.Double, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new FloatUniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "vec2")
            {
                GetVectorRangePropertiesValues("vec2", 2, name, value, annotationReader, sourceLine, GlslValueParser.Float, 0.0, Double.MinValue, Double.MaxValue, 0.01, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new Vec2Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "ivec2")
            {
                GetVectorRangePropertiesValues("ivec2", 2, name, value, annotationReader, sourceLine, GlslValueParser.Int, 0, Int32.MinValue, Int32.MaxValue, 1, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Int, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new IVec2Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "uvec2")
            {
                GetVectorRangePropertiesValues("uvec2", 2, name, value, annotationReader, sourceLine, GlslValueParser.UInt, 0u, UInt32.MinValue, UInt32.MaxValue, 1u, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.UInt, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new UVec2Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "bvec2")
            {
                var defaultValue = !String.IsNullOrEmpty(value) ? ParseValue(VectorGlslValueParser.BVec2, sourceLine, name, "default", value) : Vector.Create(2, false);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Bool, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new BVec2Uniform(this.renderThread, displayName, settingsValue);
            }

            if (type == "vec3")
            {
                GetVectorRangePropertiesValues("vec3", 3, name, value, annotationReader, sourceLine, GlslValueParser.Float, 0.0, Double.MinValue, Double.MaxValue, 0.01, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new Vec3Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "ivec3")
            {
                GetVectorRangePropertiesValues("ivec3", 3, name, value, annotationReader, sourceLine, GlslValueParser.Int, 0, Int32.MinValue, Int32.MaxValue, 1, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Int, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new IVec3Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "uvec3")
            {
                GetVectorRangePropertiesValues("uvec3", 3, name, value, annotationReader, sourceLine, GlslValueParser.UInt, 0u, UInt32.MinValue, UInt32.MaxValue, 1u, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.UInt, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new UVec3Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "bvec3")
            {
                var defaultValue = !String.IsNullOrEmpty(value) ? ParseValue(VectorGlslValueParser.BVec3, sourceLine, name, "default", value) : Vector.Create(3, false);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Bool, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new BVec3Uniform(this.renderThread, displayName, settingsValue);
            }

            if (type == "vec4")
            {
                GetVectorRangePropertiesValues("vec4", 4, name, value, annotationReader, sourceLine, GlslValueParser.Float, 0.0, Double.MinValue, Double.MaxValue, 0.01, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Double, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new Vec4Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "ivec4")
            {
                GetVectorRangePropertiesValues("ivec4", 4, name, value, annotationReader, sourceLine, GlslValueParser.Int, 0, Int32.MinValue, Int32.MaxValue, 1, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Int, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new IVec4Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "uvec4")
            {
                GetVectorRangePropertiesValues("uvec4", 4, name, value, annotationReader, sourceLine, GlslValueParser.UInt, 0u, UInt32.MinValue, UInt32.MaxValue, 1u, out var defaultValue, out var minValue, out var maxValue, out var stepValue);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.UInt, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new UVec4Uniform(this.renderThread, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "bvec4")
            {
                var defaultValue = !String.IsNullOrEmpty(value) ? ParseValue(VectorGlslValueParser.BVec4, sourceLine, name, "default", value) : Vector.Create(4, false);
                var settingsValue = GetVectorSettingsValue(settings, VectorJsonSerializer.Bool, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new BVec4Uniform(this.renderThread, displayName, settingsValue);
            }

            throw new SourceLineException($"Unexpected uniform type \"{type}\"", annotationReader.SourceLine);
        }

        private string GetDisplayName(string name, ISourceLineAnnotationReader annotationReader)
        {
            return annotationReader.TryGetValue("display-name", out var displayNameValue) ? displayNameValue!.ToString()! : this.displayNameFormatter.GetDisplayName(name);
        }

        private static ISettingsValue<Vector<T>> GetVectorSettingsValue<T>(IProjectSettings settings, IJsonSerializer<Vector<T>> vectorSerializer, string name, Vector<T> defaultValue)
        {
            var settingsValue = settings.GetUniformValue(new FixedSizeVectorJsonSerializer<T>(vectorSerializer, defaultValue), name, defaultValue);

            if (settingsValue.Value.Count != defaultValue.Count)
            {
                settingsValue.Value = new Vector<T>(settingsValue.Value.Take(defaultValue.Count).Concat(defaultValue.Skip(settingsValue.Value.Count)).ToArray());
            }

            return settingsValue;
        }

        private static void GetVectorRangePropertiesValues<T>(string valueType, int size, string name, string value, ISourceLineAnnotationReader annotationReader, SourceLine sourceLine, IGlslValueParser<T> parser,
            T defaultDefaultValue, T defaultMinValue, T defaultMaxValue, T defaultStepValue,
            out Vector<T> defaultValue, out Vector<T> minValue, out Vector<T> maxValue, out Vector<T> stepValue)
        {
            GetRangePropertiesValues(name, value, annotationReader, sourceLine, new VectorGlslValueParser<T>(parser, valueType, size),
                Vector.Create(size, defaultDefaultValue), Vector.Create(size, defaultMinValue), Vector.Create(size, defaultMaxValue), Vector.Create(size, defaultStepValue),
                out defaultValue, out minValue, out maxValue, out stepValue);
        }

        private static void GetRangePropertiesValues<T>(string name, string value, ISourceLineAnnotationReader annotationReader, SourceLine sourceLine, IGlslValueParser<T> parser,
            T defaultDefaultValue, T defaultMinValue, T defaultMaxValue, T defaultStepValue,
            out T defaultValue, out T minValue, out T maxValue, out T stepValue)
        {
            defaultValue = String.IsNullOrEmpty(value) ? defaultDefaultValue : ParseValue(parser, sourceLine, name, "default", value);
            minValue = GetPropertyValue(parser, annotationReader, name, "min", defaultMinValue);
            maxValue = GetPropertyValue(parser, annotationReader, name, "max", defaultMaxValue);
            stepValue = GetPropertyValue(parser, annotationReader, name, "step", defaultStepValue);
        }

        private static T GetPropertyValue<T>(IGlslValueParser<T> parser, ISourceLineAnnotationReader annotationReader, string uniformName, string propertyName, T defaultValue)
        {
            if (!annotationReader.TryGetValue(propertyName, out var rawValue))
            {
                return defaultValue;
            }

            var sourceLine = annotationReader.SourceLine;

            return ParseValue(parser, sourceLine, uniformName, propertyName, rawValue);
        }

        private static T ParseValue<T>(IGlslValueParser<T> parser, SourceLine sourceLine, string uniformName, string propertyName, string rawValue)
        {
            if (!parser.TryParse(rawValue, out var value))
            {
                throw new SourceLineException($"Uniform {uniformName} {propertyName} value \"{rawValue}\" cannot be parsed as {parser.ValueType}", sourceLine);
            }

            return value;
        }
    }
}
