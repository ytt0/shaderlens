namespace Shaderlens.Presentation.Extensions
{
    public static class RectExtensions
    {
        public static Rect AddMargin(this Rect rect, double margin)
        {
            return new Rect(rect.Left - margin, rect.Top - margin, Math.Max(0.0, rect.Width + margin + margin), Math.Max(0.0, rect.Height + margin + margin));
        }
    }
}
