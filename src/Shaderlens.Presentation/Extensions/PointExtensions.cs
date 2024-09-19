namespace Shaderlens.Presentation.Extensions
{
    public static class PointExtensions
    {
        public static Point Add(this Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static double Length(this Point point)
        {
            return Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }

        public static double LengthApprox(this Point point)
        {
            return Math.Max(Math.Abs(point.X), Math.Abs(point.Y));
        }

        public static double Distance(this Point point1, Point point2)
        {
            var x = point1.X - point2.X;
            var y = point1.Y - point2.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public static double DistanceApprox(this Point point1, Point point2)
        {
            return Math.Max(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
        }
    }
}
