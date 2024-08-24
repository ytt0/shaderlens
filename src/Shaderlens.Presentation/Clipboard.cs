using System.IO;

namespace Shaderlens.Presentation
{
    public interface IClipboard
    {
        bool TryGetColor(string value, out SrgbColor color);
        void SetImage(ReadOnlyMemory<float> buffer, int width, int height, bool includeAlphaChannel);
        void SetText(string text);
        void SetColor(SrgbColor srgbColor);
    }

    public partial class Clipboard : IClipboard
    {
        public void SetImage(ReadOnlyMemory<float> buffer, int width, int height, bool includeAlphaChannel)
        {
            var bufferArray = buffer.ToArray().Select(value => (byte)Math.Clamp((int)Math.Round(255.0 * value), 0, 255)).ToArray();
            bufferArray.FlipBufferRows(height);
            bufferArray.FlipBgraRgba();

            if (!includeAlphaChannel)
            {
                bufferArray.RemoveAlphaChannel();
            }

            var bitmap = BitmapSource.Create(width, height, 96.0, 96.0, PixelFormats.Bgra32, null, bufferArray, width * 4);
            var encoder = new PngBitmapEncoder();
            var stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);

            var data = new DataObject();
            data.SetImage(bitmap);
            data.SetData("PNG", stream);

            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(data);
            System.Windows.Clipboard.Flush();
        }

        public void SetText(string text)
        {
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetText(text);
            System.Windows.Clipboard.Flush();
        }

        public void SetColor(SrgbColor srgbColor)
        {
            SetText($"[{srgbColor.R:f3}, {srgbColor.G:f3}, {srgbColor.B:f3}, {srgbColor.A:f3}]");
        }

        public bool TryGetColor(string value, out SrgbColor color)
        {
            var match = ColorFloatArrayFormatRegex().Match(value);
            if (match.Success)
            {
                if (Double.TryParse(match.Groups["r"].Value, out var r) &&
                    Double.TryParse(match.Groups["g"].Value, out var g) &&
                    Double.TryParse(match.Groups["b"].Value, out var b) &&
                    Double.TryParse(match.Groups["a"].Value, out var a))
                {
                    color = new SrgbColor(r, g, b, a);
                    return true;
                }
            }

            match = ColorHexFormatRegex().Match(value);
            if (match.Success)
            {
                var hexValue = match.Value;

                if (hexValue.Length == 3) // rgb
                {
                    color = new SrgbColor(
                        Convert.ToByte($"{hexValue[0]}{hexValue[0]}", 16) / 255.0,
                        Convert.ToByte($"{hexValue[1]}{hexValue[1]}", 16) / 255.0,
                        Convert.ToByte($"{hexValue[2]}{hexValue[2]}", 16) / 255.0);
                    return true;
                }

                if (hexValue.Length == 4) // rgba
                {
                    color = new SrgbColor(
                        Convert.ToByte($"{hexValue[0]}{hexValue[0]}", 16) / 255.0,
                        Convert.ToByte($"{hexValue[1]}{hexValue[1]}", 16) / 255.0,
                        Convert.ToByte($"{hexValue[2]}{hexValue[2]}", 16) / 255.0,
                        Convert.ToByte($"{hexValue[3]}{hexValue[3]}", 16) / 255.0);
                    return true;
                }

                if (hexValue.Length == 6) // rrggbb
                {
                    color = new SrgbColor(
                        Convert.ToByte(hexValue[0..2], 16) / 255.0,
                        Convert.ToByte(hexValue[2..4], 16) / 255.0,
                        Convert.ToByte(hexValue[4..6], 16) / 255.0);
                    return true;
                }

                if (hexValue.Length == 8) // rrggbbaa
                {
                    color = new SrgbColor(
                        Convert.ToByte(hexValue[0..2], 16) / 255.0,
                        Convert.ToByte(hexValue[2..4], 16) / 255.0,
                        Convert.ToByte(hexValue[4..6], 16) / 255.0,
                        Convert.ToByte(hexValue[6..8], 16) / 255.0);
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
                    color = new SrgbColor(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
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
                    color = new SrgbColor(r, g, b, a);
                    return true;
                }
            }

            color = default;
            return false;
        }

        [GeneratedRegex("^\\s*\\[\\s*(?<r>[0-9.]+)\\s*,\\s*(?<g>[0-9.]+)\\s*,\\s*(?<b>[0-9.]+)\\s*,\\s*(?<a>[0-9.]+)\\s*\\]\\s*$")]
        private static partial Regex ColorFloatArrayFormatRegex();

        [GeneratedRegex("^\\s*#?[0-9A-Fa-f]{3,8}\\s*$")]
        private static partial Regex ColorHexFormatRegex();

        [GeneratedRegex("^\\s*(?<r>[0-9]+)\\s*,\\s*(?<g>[0-9]+)\\s*,\\s*(?<b>[0-9]+)(\\s*,\\s*(?<a>[0-9]+))?\\s*$")]
        private static partial Regex ColorCommaSeparatedByteFormatRegex();

        [GeneratedRegex("^\\s*(?<r>[0-9.]+)\\s*,\\s*(?<g>[0-9.]+)\\s*,\\s*(?<b>[0-9.]+)(\\s*,\\s*(?<a>[0-9.]+))?\\s*$")]
        private static partial Regex ColorCommaSeparatedFloatFormatRegex();
    }
}