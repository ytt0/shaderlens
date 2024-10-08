﻿namespace Shaderlens
{
    public interface ICopySource
    {
        ITextureBuffer? Texture { get; }
        Vector<double> Value { get; }
        int X { get; }
        int Y { get; }
    }

    public class CopySource : ICopySource
    {
        public ITextureBuffer? Texture { get; }
        public Vector<double> Value { get; }
        public int X { get; }
        public int Y { get; }

        public CopySource(ITextureBuffer? texture, int x, int y)
        {
            this.Texture = texture;
            this.Value = texture?.GetValue(x, y) ?? Vector.Create(4, 0.0);
            this.X = x;
            this.Y = y;
        }
    }
}