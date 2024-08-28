using System.IO;

namespace Shaderlens.Presentation
{
    public interface IClipboard
    {
        void SetImage(ReadOnlyMemory<float> buffer, int width, int height, bool includeAlphaChannel);
        void SetText(string text);
        bool TryGetText(out string text);
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

        public bool TryGetText(out string text)
        {
            text = System.Windows.Clipboard.GetText();
            return !String.IsNullOrEmpty(text);
        }
    }
}