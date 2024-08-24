namespace Shaderlens
{
    public interface ICopySource
    {
        ITextureBuffer? Texture { get; }
        Vec4 Value { get; }
        int X { get; }
        int Y { get; }
    }

    public class CopySource : ICopySource
    {
        public ITextureBuffer? Texture { get; }
        public Vec4 Value { get; }
        public int X { get; }
        public int Y { get; }

        public CopySource(ITextureBuffer? texture, int x, int y)
        {
            this.Texture = texture;
            this.Value = texture?.GetValue(x, y) ?? default;
            this.X = x;
            this.Y = y;
        }
    }
}