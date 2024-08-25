namespace Shaderlens
{
    public readonly struct Vec3 : IVector<double>
    {
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.X;
                    case 1: return this.Y;
                    case 2: return this.Z;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public readonly int Length { get { return 3; } }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

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

        public readonly IEnumerator<double> GetEnumerator()
        {
            return new[] { this.X, this.Y, this.Z }.Cast<double>().GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
