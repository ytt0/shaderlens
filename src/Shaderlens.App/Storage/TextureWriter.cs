namespace Shaderlens.Storage
{

    public class TextureWriterFactory : ITextureWriterFactory
    {
        public IEnumerable<string> SupportedExtensions { get; } = new[] { ".png", ".jpg", ".bmp", ".tiff" };

        public ITextureWriter Create(string extension)
        {
            switch (extension)
            {
                case ".png": return new TextureWriter(new PngBitmapEncoder());
                case ".jpg": return new TextureWriter(new JpegBitmapEncoder());
                case ".bmp": return new TextureWriter(new BmpBitmapEncoder());
                case ".tiff": return new TextureWriter(new TiffBitmapEncoder());
            }

            throw new NotSupportedException($"File extension {extension} is not supported");
        }
    }

    public class TextureWriter : ITextureWriter
    {
        private readonly BitmapEncoder bitmapEncoder;

        public TextureWriter(BitmapEncoder bitmapEncoder)
        {
            this.bitmapEncoder = bitmapEncoder;
        }

        public void Write(ITextureBuffer source, Stream target)
        {
            var bufferArray = source.Buffer.ToArray().Select(value => (byte)Math.Clamp((int)Math.Round(255.0 * value), 0, 255)).ToArray();
            bufferArray.FlipBufferRows(source.Height);
            bufferArray.FlipBgraRgba();

            var bitmap = BitmapSource.Create(source.Width, source.Height, 96.0, 96.0, PixelFormats.Bgra32, null, bufferArray, source.Width * 4);

            this.bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
            this.bitmapEncoder.Save(target);
        }
    }

    public class TiffTextureWriter : ITextureWriter
    {
        public void Write(ITextureBuffer source, Stream target)
        {
            var bufferArray = source.Buffer.ToArray();
            var bitmap = BitmapSource.Create(source.Width, source.Height, 96.0, 96.0, PixelFormats.Rgba128Float, null, bufferArray, source.Width * 4 * sizeof(float));

            var bitmapEncoder = new TiffBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
            bitmapEncoder.Compression = TiffCompressOption.Zip;
            bitmapEncoder.Save(target);
        }
    }
}
