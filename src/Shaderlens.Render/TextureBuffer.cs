namespace Shaderlens.Render
{
    public interface ITextureBuffer
    {
        float[] Buffer { get; }
        int Width { get; }
        int Height { get; }
    }

    public static class TextureBufferExtensions
    {
        public static Vector<double> GetValue(this ITextureBuffer texture, int x, int y)
        {
            var offset = (y * texture.Width + x) * 4;
            if (x >= 0 && y >= 0 && x < texture.Width && y < texture.Height && offset < texture.Buffer.Length - 3)
            {
                return Vector.Create(texture.Buffer.Skip(offset).Take(4).ToArray().Select(value => (double)value).ToArray());
            }

            return Vector.Create(4, 0.0);
        }
    }

    public class TextureBuffer : ITextureBuffer
    {
        public float[] Buffer { get; }
        public int Width { get; }
        public int Height { get; }

        public TextureBuffer(float[] buffer, int width, int height)
        {
            this.Buffer = buffer;
            this.Width = width;
            this.Height = height;
        }
    }
}
