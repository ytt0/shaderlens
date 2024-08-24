namespace Shaderlens.Storage
{
    using static OpenGL.Gl;

    public class TextureReaderFactory : ITextureReaderFactory
    {
        private readonly IThreadAccess threadAccess;

        public TextureReaderFactory(IThreadAccess threadAccess)
        {
            this.threadAccess = threadAccess;
        }

        public ITextureReader Create(string extension)
        {
            switch (extension)
            {
                case ".png": return new PngTextureReader(this.threadAccess);
                case ".jpg": return new JpegTextureReader(this.threadAccess);
                case ".bmp": return new BmpTextureReader(this.threadAccess);
                case ".tiff": return new TiffTextureReader(this.threadAccess);
                case ".gif": return new GifTextureReader(this.threadAccess);
            }

            throw new NotSupportedException($"File extension {extension} is not supported");
        }
    }

    public abstract class TextureReader : ITextureReader
    {
        private readonly IThreadAccess threadAccess;

        public TextureReader(IThreadAccess threadAccess)
        {
            this.threadAccess = threadAccess;
        }

        public ITextureResource Read(Stream source)
        {
            this.threadAccess.Verify();

            var decoder = CreateBitmapDecoder(source);

            var frame = decoder.Frames[0];

            var width = frame.PixelWidth;
            var height = frame.PixelHeight;

            var isFloatBuffer = frame.Format.BitsPerPixel > 32;

            var converterdFrame = new FormatConvertedBitmap(frame, isFloatBuffer ? PixelFormats.Rgba128Float : PixelFormats.Bgra32, null, 0.0);
            var type = isFloatBuffer ? GL_FLOAT : GL_UNSIGNED_BYTE;

            var bpp = converterdFrame.Format.BitsPerPixel;
            var buffer = new byte[width * height * bpp / 8];
            converterdFrame.CopyPixels(buffer, width * bpp / 8, 0);

            if (!isFloatBuffer)
            {
                buffer.FlipBufferRows(height);
                buffer.FlipBgraRgba();
            }

            return new BufferTextureResource(this.threadAccess, width, height, buffer, type);
        }

        protected abstract BitmapDecoder CreateBitmapDecoder(Stream source);
    }

    public class PngTextureReader : TextureReader
    {
        public PngTextureReader(IThreadAccess threadAccess) :
            base(threadAccess)
        {
        }

        protected override BitmapDecoder CreateBitmapDecoder(Stream source)
        {
            return new PngBitmapDecoder(source, BitmapCreateOptions.None, BitmapCacheOption.None);
        }
    }

    public class JpegTextureReader : TextureReader
    {
        public JpegTextureReader(IThreadAccess threadAccess) :
            base(threadAccess)
        {
        }

        protected override BitmapDecoder CreateBitmapDecoder(Stream source)
        {
            return new JpegBitmapDecoder(source, BitmapCreateOptions.None, BitmapCacheOption.None);
        }
    }

    public class BmpTextureReader : TextureReader
    {
        public BmpTextureReader(IThreadAccess threadAccess) :
            base(threadAccess)
        {
        }

        protected override BitmapDecoder CreateBitmapDecoder(Stream source)
        {
            return new BmpBitmapDecoder(source, BitmapCreateOptions.None, BitmapCacheOption.None);
        }
    }

    public class TiffTextureReader : TextureReader
    {
        public TiffTextureReader(IThreadAccess threadAccess) :
            base(threadAccess)
        {
        }

        protected override BitmapDecoder CreateBitmapDecoder(Stream source)
        {
            return new TiffBitmapDecoder(source, BitmapCreateOptions.None, BitmapCacheOption.None);
        }
    }

    public class GifTextureReader : ITextureReader
    {
        //https://www.w3.org/Graphics/GIF/spec-gif89a.txt

        private enum DisposalMethod : byte
        {
            NoDisposal = 0,
            DoNotDispose = 1,
            RestoreToBackgroundColor = 2,
            RestoreToPrevious = 3
        }

        private readonly IThreadAccess threadAccess;

        public GifTextureReader(IThreadAccess threadAccess)
        {
            this.threadAccess = threadAccess;
        }

        public ITextureResource Read(Stream source)
        {
            this.threadAccess.Verify();

            var decoder = new GifBitmapDecoder(source, BitmapCreateOptions.None, BitmapCacheOption.None);
            var frames = decoder.Frames;

            if (frames.Count == 0)
            {
                throw new TextureReaderException("Failed to read gif frames");
            }

            if (frames.Count == 1)
            {
                return CreateTextureResource(this.threadAccess, frames[0]);
            }

            var displayWidth = frames[0].PixelWidth;
            var displayHeight = frames[0].PixelHeight;

            var displayBuffer = new byte[displayWidth * displayHeight * 4];

            var textureResources = new List<IAnimationFrameTextureResource>();

            var startTime = 0L;
            foreach (var frame in frames)
            {
                frame.Freeze();
                var frameTextureResource = CreateFrameTextureResource(this.threadAccess, frame, displayBuffer, displayWidth, displayHeight, startTime);
                textureResources.Add(frameTextureResource);
                startTime = frameTextureResource.EndTime;
            }

            return new AnimatedTextureResource(textureResources);
        }

        private static AnimationFrameTextureResource CreateFrameTextureResource(IThreadAccess threadAccess, BitmapFrame frame, byte[] displayBuffer, int displayWidth, int displayHeight, long startTime)
        {
            var metadata = (BitmapMetadata)frame.Metadata;

            var delay = QueryMetadata<ushort>(metadata, "/grctlext/Delay", 10) * 10 * TimeSpan.TicksPerMillisecond;
            var disposal = QueryMetadata<DisposalMethod>(metadata, "/grctlext/Disposal");
            var transparencyFlag = QueryMetadata<bool>(metadata, "/grctlext/TransparencyFlag");
            var transparentColorIndex = QueryMetadata<byte>(metadata, "/grctlext/TransparentColorIndex");

            var frameLeft = QueryMetadata<ushort>(metadata, "/imgdesc/Left");
            var frameTop = QueryMetadata<ushort>(metadata, "/imgdesc/Top");
            var frameWidth = QueryMetadata<ushort>(metadata, "/imgdesc/Width");
            var frameHeight = QueryMetadata<ushort>(metadata, "/imgdesc/Height");

            if (frame.PixelWidth != frameWidth || frame.PixelHeight != frameHeight)
            {
                throw new TextureReaderException("Failed to read frame dimensions");
            }

            var frameSourceBuffer = new byte[frameWidth * frameHeight];
            frame.CopyPixels(frameSourceBuffer, frameWidth, 0);

            var frameTargetBuffer = new byte[displayBuffer.Length];
            Array.Copy(displayBuffer, frameTargetBuffer, displayBuffer.Length);

            var palette = frame.Palette.Colors.ToArray();

            var sourceIndex = 0;
            for (var y = 0; y < frameHeight; y++)
            {
                var targetIndex = ((y + frameTop) * displayWidth + frameLeft) * 4;

                for (var x = 0; x < frameWidth; x++)
                {
                    var colorIndex = frameSourceBuffer[sourceIndex];
                    if (!transparencyFlag || colorIndex != transparentColorIndex)
                    {
                        var color = colorIndex < palette.Length ? palette[colorIndex] : Color.FromArgb(0, 0, 0, 0);
                        frameTargetBuffer[targetIndex] = Mix(frameTargetBuffer[targetIndex], color.R, color.A);
                        frameTargetBuffer[targetIndex + 1] = Mix(frameTargetBuffer[targetIndex + 1], color.G, color.A);
                        frameTargetBuffer[targetIndex + 2] = Mix(frameTargetBuffer[targetIndex + 2], color.B, color.A);
                        frameTargetBuffer[targetIndex + 3] = Mix(frameTargetBuffer[targetIndex + 3], color.A, color.A);
                    }

                    sourceIndex++;
                    targetIndex += 4;
                }
            }

            if (disposal == DisposalMethod.NoDisposal || disposal == DisposalMethod.DoNotDispose)
            {
                Array.Copy(frameTargetBuffer, displayBuffer, frameTargetBuffer.Length);
            }
            else if (disposal == DisposalMethod.RestoreToBackgroundColor)
            {
                if (frameLeft == 0 && frameTop == 0 && frameWidth == displayWidth && frameHeight == displayHeight)
                {
                    Array.Clear(displayBuffer);
                }
                else
                {
                    for (var y = 0; y < frameHeight; y++)
                    {
                        var targetIndex = ((y + frameTop) * displayWidth + frameLeft) * 4;

                        for (var x = 0; x < frameWidth; x++)
                        {
                            displayBuffer[targetIndex++] = 0;
                            displayBuffer[targetIndex++] = 0;
                            displayBuffer[targetIndex++] = 0;
                            displayBuffer[targetIndex++] = 0;
                        }
                    }
                }
            }

            frameTargetBuffer.FlipBufferRows(displayHeight);
            return new AnimationFrameTextureResource(new BufferTextureResource(threadAccess, displayWidth, displayHeight, frameTargetBuffer, GL_UNSIGNED_BYTE), startTime + delay);
        }

        private static BufferTextureResource CreateTextureResource(IThreadAccess threadAccess, BitmapFrame frame)
        {
            var source = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, null, 0.0);

            var width = source.PixelWidth;
            var height = source.PixelHeight;
            var bpp = source.Format.BitsPerPixel;
            var buffer = new byte[width * height * bpp / 8];
            source.CopyPixels(buffer, width * bpp / 8, 0);

            buffer.FlipBufferRows(height);
            buffer.FlipBgraRgba();

            return new BufferTextureResource(threadAccess, width, height, buffer, GL_UNSIGNED_BYTE);
        }

        private static T QueryMetadata<T>(BitmapMetadata metadata, string query, T defaultValue = default!)
        {
            try
            {
                return (T)metadata.GetQuery(query);
            }
            catch
            {
            }

            return defaultValue;
        }

        private static byte Mix(byte c1, byte c2, byte alpha)
        {
            var f = alpha / 255.0;
            return alpha == 0 ? c1 : alpha == 255 ? c2 :
                (byte)Math.Clamp((1.0 - f) * c1 + f * c2, 0, 255);
        }
    }

    public class TextureReaderException : Exception
    {
        public TextureReaderException(string? message, Exception? innerException = null) :
            base(message, innerException)
        {
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
}
