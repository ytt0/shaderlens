namespace Shaderlens
{
    public struct RenderSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public RenderSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
