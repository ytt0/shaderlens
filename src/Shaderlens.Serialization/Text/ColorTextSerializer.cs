namespace Shaderlens.Serialization.Text
{
    public partial class ColorTextSerializer : ITextSerializer<SrgbColor>
    {
        public string Serialize(SrgbColor value)
        {
            return $"[{value.R:f3}, {value.G:f3}, {value.B:f3}, {value.A:f3}]";
        }

        public bool TryDeserialize(string value, [MaybeNullWhen(false)] out SrgbColor result)
        {
            var match = ColorFloatArrayFormatRegex().Match(value);
            if (match.Success)
            {
                var a = 1.0;
                if (Double.TryParse(match.Groups["r"].Value, out var r) &&
                    Double.TryParse(match.Groups["g"].Value, out var g) &&
                    Double.TryParse(match.Groups["b"].Value, out var b) &&
                    (match.Groups["a"].Length == 0 || Double.TryParse(match.Groups["a"].Value, out a)))
                {
                    result = new SrgbColor(Round(r), Round(g), Round(b), Round(a));
                    return true;
                }
            }

            match = ColorHexFormatRegex().Match(value);
            if (match.Success)
            {
                var hexValue = match.Groups["value"].Value;

                if (hexValue.Length == 3) // rgb
                {
                    result = new SrgbColor(
                        Round(Convert.ToByte($"{hexValue[0]}{hexValue[0]}", 16) / 255.0),
                        Round(Convert.ToByte($"{hexValue[1]}{hexValue[1]}", 16) / 255.0),
                        Round(Convert.ToByte($"{hexValue[2]}{hexValue[2]}", 16) / 255.0));
                    return true;
                }

                if (hexValue.Length == 4) // rgba
                {
                    result = new SrgbColor(
                        Round(Convert.ToByte($"{hexValue[0]}{hexValue[0]}", 16) / 255.0),
                        Round(Convert.ToByte($"{hexValue[1]}{hexValue[1]}", 16) / 255.0),
                        Round(Convert.ToByte($"{hexValue[2]}{hexValue[2]}", 16) / 255.0),
                        Round(Convert.ToByte($"{hexValue[3]}{hexValue[3]}", 16) / 255.0));
                    return true;
                }

                if (hexValue.Length == 6) // rrggbb
                {
                    result = new SrgbColor(
                        Round(Convert.ToByte(hexValue[0..2], 16) / 255.0),
                        Round(Convert.ToByte(hexValue[2..4], 16) / 255.0),
                        Round(Convert.ToByte(hexValue[4..6], 16) / 255.0));
                    return true;
                }

                if (hexValue.Length == 8) // rrggbbaa
                {
                    result = new SrgbColor(
                        Round(Convert.ToByte(hexValue[0..2], 16) / 255.0),
                        Round(Convert.ToByte(hexValue[2..4], 16) / 255.0),
                        Round(Convert.ToByte(hexValue[4..6], 16) / 255.0),
                        Round(Convert.ToByte(hexValue[6..8], 16) / 255.0));
                    return true;
                }
            }

            match = ColorCommaSeparatedByteFormatRegex().Match(value);
            if (match.Success)
            {
                var a = (byte)255;
                if (Byte.TryParse(match.Groups["r"].Value, out var r) &&
                    Byte.TryParse(match.Groups["g"].Value, out var g) &&
                    Byte.TryParse(match.Groups["b"].Value, out var b) &&
                    (String.IsNullOrEmpty(match.Groups["a"].Value) || Byte.TryParse(match.Groups["a"].Value, out a)))
                {
                    result = new SrgbColor(Round(r / 255.0), Round(g / 255.0), Round(b / 255.0), Round(a / 255.0));
                    return true;
                }
            }

            match = ColorCommaSeparatedFloatFormatRegex().Match(value);
            if (match.Success)
            {
                var a = 1.0;
                if (Double.TryParse(match.Groups["r"].Value, out var r) &&
                    Double.TryParse(match.Groups["g"].Value, out var g) &&
                    Double.TryParse(match.Groups["b"].Value, out var b) &&
                    (String.IsNullOrEmpty(match.Groups["a"].Value) || Double.TryParse(match.Groups["a"].Value, out a)))
                {
                    result = new SrgbColor(Round(r), Round(g), Round(b), Round(a));
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static double Round(double value)
        {
            return Math.Round(value, 3);
        }

        [GeneratedRegex("^\\s*\\[\\s*(?<r>[0-9.]+)\\s*,\\s*(?<g>[0-9.]+)\\s*,\\s*(?<b>[0-9.]+)\\s*(,\\s*(?<a>[0-9.]+)\\s*)?\\]\\s*$")]
        private static partial Regex ColorFloatArrayFormatRegex();

        [GeneratedRegex("^\\s*#?(?<value>[0-9A-Fa-f]{3,8})\\s*$")]
        private static partial Regex ColorHexFormatRegex();

        [GeneratedRegex("^\\s*(?<r>[0-9]+)\\s*,\\s*(?<g>[0-9]+)\\s*,\\s*(?<b>[0-9]+)(\\s*,\\s*(?<a>[0-9]+))?\\s*$")]
        private static partial Regex ColorCommaSeparatedByteFormatRegex();

        [GeneratedRegex("^\\s*(?<r>[0-9.]+)\\s*,\\s*(?<g>[0-9.]+)\\s*,\\s*(?<b>[0-9.]+)(\\s*,\\s*(?<a>[0-9.]+))?\\s*$")]
        private static partial Regex ColorCommaSeparatedFloatFormatRegex();
    }
}
