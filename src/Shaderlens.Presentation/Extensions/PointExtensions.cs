namespace Shaderlens.Presentation.Extensions
{
    public static class PointExtensions
    {
        public static Point Add(this Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static double DistanceApprox(this Point point1, Point point2)
        {
            return Math.Max(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
        }
    }
}
