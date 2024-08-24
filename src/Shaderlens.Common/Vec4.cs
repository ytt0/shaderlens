namespace Shaderlens
{
    public struct Vec4
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public Vec4(double value) :
            this(value, value, value, value)
        {
        }

        public Vec4(double[] values) :
            this(values[0], values[1], values[2], values[3])
        {
        }

        public Vec4(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
    }
}
