namespace Shaderlens.Presentation.Extensions
{
    public static class PointExtensions
    {
        public static Point Add(this Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }
    }
}
