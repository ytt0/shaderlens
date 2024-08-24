namespace Shaderlens
{
    public struct Vec2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vec2(double value) :
            this(value, value)
        {
        }

        public Vec2(double[] values) :
            this(values[0], values[1])
        {
        }

        public Vec2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
