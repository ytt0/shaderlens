namespace Shaderlens
{
    public struct Vec3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vec3(double value) :
            this(value, value, value)
        {
        }

        public Vec3(double[] values) :
            this(values[0], values[1], values[2])
        {
        }

        public Vec3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
