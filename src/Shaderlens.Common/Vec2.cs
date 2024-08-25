namespace Shaderlens
{
    public readonly struct Vec2 : IVector<double>
    {
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.X;
                    case 1: return this.Y;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public readonly int Length { get { return 2; } }

        public double X { get; }
        public double Y { get; }

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

        public readonly IEnumerator<double> GetEnumerator()
        {
            return new[] { this.X, this.Y }.Cast<double>().GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
