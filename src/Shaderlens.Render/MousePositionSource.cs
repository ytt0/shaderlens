namespace Shaderlens.Render
{
    public interface IMousePositionSource
    {
        void GetPosition(out double x, out double y);
    }

    public static class MousePositionSource
    {
        private class EmptyMousePositionSource : IMousePositionSource
        {
            public void GetPosition(out double x, out double y)
            {
                x = 0;
                y = 0;
            }
        }

        public static readonly IMousePositionSource Empty = new EmptyMousePositionSource();
    }
}
