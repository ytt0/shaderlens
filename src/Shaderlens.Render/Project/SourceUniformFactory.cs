
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
        private readonly ValueJsonSerializer<bool> boolSerializer;
        private readonly ValueJsonSerializer<int> intSerializer;
        private readonly ValueJsonSerializer<double> doubleSerializer;
        private readonly VectorJsonSerializer vectorSerializer;
        private readonly DisplayNameFormatter displayNameFormatter;
        private readonly IDispatcherThread renderThread;

        public SourceUniformFactory(IDispatcherThread renderThread)
        {
            this.boolSerializer = new ValueJsonSerializer<bool>();
            this.intSerializer = new ValueJsonSerializer<int>();
            this.doubleSerializer = new ValueJsonSerializer<double>();
            this.vectorSerializer = new VectorJsonSerializer();
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

            if (type == "bool")
            {
                var defaultValue = !String.IsNullOrEmpty(value) && ParseBool(sourceLine, name, "default", value);
                var settingsValue = settings.GetUniformValue(this.boolSerializer, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new BoolUniform(this.renderThread, name, displayName, settingsValue);
            }

            if (type == "int")
            {
                var defaultValue = String.IsNullOrEmpty(value) ? 0 : ParseInt(sourceLine, name, "default", value);
                var minValue = GetPropertyValue(annotationReader, name, "min", Int32.MinValue);
                var maxValue = GetPropertyValue(annotationReader, name, "max", Int32.MaxValue);
                var stepValue = GetPropertyValue(annotationReader, name, "step", 1);
                var settingsValue = settings.GetUniformValue(this.intSerializer, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new IntUniform(this.renderThread, name, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "float")
            {
                var defaultValue = String.IsNullOrEmpty(value) ? 0.0 : ParseFloat(sourceLine, name, "default", value);
                var minValue = GetPropertyValue(annotationReader, name, "min", Double.MinValue);
                var maxValue = GetPropertyValue(annotationReader, name, "max", Double.MaxValue);
                var stepValue = GetPropertyValue(annotationReader, name, "step", 0.01);
                var settingsValue = settings.GetUniformValue(this.doubleSerializer, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new FloatUniform(this.renderThread, name, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "vec2")
            {
                var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(2, 0.0) : ParseVector(sourceLine, name, "default", value, 2);
                var minValue = GetPropertyValue(annotationReader, name, "min", Vector.Create(2, Double.MinValue));
                var maxValue = GetPropertyValue(annotationReader, name, "max", Vector.Create(2, Double.MaxValue));
                var stepValue = GetPropertyValue(annotationReader, name, "step", Vector.Create(2, 0.01));
                var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                annotationReader.ValidateEmpty();
                return new Vec2Uniform(this.renderThread, name, displayName, minValue, maxValue, stepValue, settingsValue);
            }

            if (type == "vec3")
            {
                if (!annotationReader.TryGetValue("type", out var conversionType))
                {
                    var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(3, 0.0) : ParseVector(sourceLine, name, "default", value, 3);
                    var minValue = GetPropertyValue(annotationReader, name, "min", Vector.Create(3, Double.MinValue));
                    var maxValue = GetPropertyValue(annotationReader, name, "max", Vector.Create(3, Double.MaxValue));
                    var stepValue = GetPropertyValue(annotationReader, name, "step", Vector.Create(3, 0.01));
                    var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                    annotationReader.ValidateEmpty();
                    return new Vec3Uniform(this.renderThread, name, displayName, minValue, maxValue, stepValue, settingsValue);
                }

                if (conversionType.ToString() == "srgb")
                {
                    var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(3, 0.0) : ParseVector(sourceLine, name, "default", value, 3);
                    var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                    annotationReader.ValidateEmpty();
                    return new SrgbColorUniform(this.renderThread, name, displayName, settingsValue);
                }

                if (conversionType.ToString() == "linear-rgb")
                {
                    var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(3, 0.0) : ParseVector(sourceLine, name, "default", value, 3);
                    var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                    annotationReader.ValidateEmpty();
                    return new LinearRgbColorUniform(this.renderThread, name, displayName, settingsValue);
                }

                throw new SourceLineException($"Unexpected vec3 uniform conversion type \"{conversionType}\", supported types are \"srgb\", \"linear-rgb\"", annotationReader.SourceLine);
            }

            if (type == "vec4")
            {
                if (!annotationReader.TryGetValue("type", out var conversionType))
                {
                    var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(4, 0.0) : ParseVector(sourceLine, name, "default", value, 4);
                    var minValue = GetPropertyValue(annotationReader, name, "min", Vector.Create(4, Double.MinValue));
                    var maxValue = GetPropertyValue(annotationReader, name, "max", Vector.Create(4, Double.MaxValue));
                    var stepValue = GetPropertyValue(annotationReader, name, "step", Vector.Create(4, 0.01));
                    var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                    annotationReader.ValidateEmpty();
                    return new Vec4Uniform(this.renderThread, name, displayName, minValue, maxValue, stepValue, settingsValue);
                }

                if (conversionType.ToString() == "srgb")
                {
                    var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(4, 0.0) : ParseVector(sourceLine, name, "default", value, 4);
                    var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                    annotationReader.ValidateEmpty();
                    return new SrgbaColorUniform(this.renderThread, name, displayName, settingsValue);
                }

                if (conversionType.ToString() == "linear-rgb")
                {
                    var defaultValue = String.IsNullOrEmpty(value) ? Vector.Create(4, 0.0) : ParseVector(sourceLine, name, "default", value, 4);
                    var settingsValue = GetVectorSettingsValue(settings, name, defaultValue);

                    annotationReader.ValidateEmpty();
                    return new LinearRgbaColorUniform(this.renderThread, name, displayName, settingsValue);
                }

                throw new SourceLineException($"Unexpected vec4 uniform conversion type \"{conversionType}\", supported types are \"srgb\", \"linear-rgb\"", annotationReader.SourceLine);
            }

            throw new SourceLineException($"Unexpected uniform type \"{type}\"", annotationReader.SourceLine);
        }

        private string GetDisplayName(string name, ISourceLineAnnotationReader annotationReader)
        {
            return annotationReader.TryGetValue("display-name", out var displayNameValue) ? displayNameValue!.ToString()! : this.displayNameFormatter.GetDisplayName(name);
        }

        private static int GetPropertyValue(ISourceLineAnnotationReader annotationReader, string uniformName, string propertyName, int defaultValue)
        {
            return !annotationReader.TryGetValue(propertyName, out var rawValue) ? defaultValue : ParseInt(annotationReader.SourceLine, uniformName, propertyName, rawValue);
        }

        private static double GetPropertyValue(ISourceLineAnnotationReader annotationReader, string uniformName, string propertyName, double defaultValue)
        {
            return !annotationReader.TryGetValue(propertyName, out var rawValue) ? defaultValue : ParseFloat(annotationReader.SourceLine, uniformName, propertyName, rawValue);
        }

        private ISettingsValue<Vector<double>> GetVectorSettingsValue(IProjectSettings settings, string name, Vector<double> defaultValue)
        {
            return settings.GetUniformValue(new FixedSizeVectorJsonSerializer<double>(this.vectorSerializer, defaultValue), name, defaultValue);
        }

        private static Vector<double> GetPropertyValue(ISourceLineAnnotationReader annotationReader, string uniformName, string propertyName, Vector<double> defaultValue)
        {
            return !annotationReader.TryGetValue(propertyName, out var rawValue) ? defaultValue : ParseVector(annotationReader.SourceLine, uniformName, propertyName, rawValue, defaultValue.Count);
        }

        private static bool ParseBool(SourceLine sourceLine, string uniformName, string propertyName, string rawValue)
        {
            if (Boolean.TryParse(rawValue, out var value))
            {
                return value;
            }

            throw new SourceLineException($"Uniform {uniformName} {propertyName} value \"{rawValue}\" cannot be parsed as bool", sourceLine);
        }

        private static int ParseInt(SourceLine sourceLine, string uniformName, string propertyName, string rawValue)
        {
            if (Int32.TryParse(rawValue, out var value))
            {
                return value;
            }

            throw new SourceLineException($"Uniform {uniformName} {propertyName} value \"{rawValue}\" cannot be parsed as int", sourceLine);
        }

        private static double ParseFloat(SourceLine sourceLine, string uniformName, string propertyName, string rawValue)
        {
            if (Double.TryParse(rawValue, out var value))
            {
                return value;
            }

            throw new SourceLineException($"Uniform {uniformName} {propertyName} value \"{rawValue}\" cannot be parsed as float", sourceLine);
        }

        private static Vector<double> ParseVector(SourceLine sourceLine, string uniformName, string propertyName, string rawValue, int count)
        {
            if (Double.TryParse(rawValue, out var value))
            {
                return Vector.Create(count, value);
            }

            if (rawValue is string stringValue && TryParseVectorValues(count, stringValue, out var values))
            {
                return values.Length == 1 ? Vector.Create(count, values[0]) : Vector.Create(values);
            }

            throw new SourceLineException($"Uniform {uniformName} {propertyName} value \"{rawValue}\" cannot be parsed as vec{count}", sourceLine);
        }

        private static bool TryParseVectorValues(int size, string rawValue, [MaybeNullWhen(false)] out double[] values)
        {
            var match = VecRegex().Match(rawValue);
            if (match.Success)
            {
                var rawValues = match.Groups["values"].Value.Split(",");

                if (rawValues.Length != 1 && rawValues.Length != size || match.Groups["type"].Value != $"vec{size}")
                {
                    values = null;
                    return false;
                }

                values = new double[rawValues.Length];
                for (var i = 0; i < rawValues.Length; i++)
                {
                    if (!Double.TryParse(rawValues[i].Trim(), out var value))
                    {
                        values = null;
                        return false;
                    }

                    values[i] = value;
                }

                return true;
            }

            values = null;
            return false;
        }

        [GeneratedRegex(@"(?<type>(vec2|vec3|vec4))\s*\((?<values>[-+0-9.,\s]+)\)")]
        private static partial Regex VecRegex();
    }
}
