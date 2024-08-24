namespace Shaderlens.Render
{
    public interface IMousePositionSource
    {
        void GetPosition(out double x, out double y);
        void StartCapture(bool enableCursorWrap);
        void EndCapture();
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

            public void StartCapture(bool enableCursorWrap)
            {
            }

            public void EndCapture()
            {
            }
        }

        public static readonly IMousePositionSource Empty = new EmptyMousePositionSource();
    }
}
