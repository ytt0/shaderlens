namespace Shaderlens
{
    public readonly struct Vec4 : IVector<double>
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
                    case 3: return this.W;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public readonly int Length { get { return 4; } }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double W { get; }

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

        public readonly IEnumerator<double> GetEnumerator()
        {
            return new[] { this.X, this.Y, this.Z, this.W }.Cast<double>().GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
