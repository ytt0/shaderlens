namespace Shaderlens
{
    public struct LinearRgbColor
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double A { get; set; }

        public LinearRgbColor(double grayscale) :
            this(grayscale, grayscale, grayscale)
        {
        }

        public LinearRgbColor(IEnumerable<double> values) :
            this(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2), values.Count() == 4 ? 1.0 : values.ElementAt(3))
        {
        }

        public LinearRgbColor(double r, double g, double b, double a = 1.0)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public readonly SrgbColor ToSrgb()
        {
            return new SrgbColor(SrgbTransfer(this.R), SrgbTransfer(this.G), SrgbTransfer(this.B), this.A);
        }

        public readonly Vector<double> ToVector(bool includeAlpha)
        {
            return Vector.Create(includeAlpha ? new[] { this.R, this.G, this.B, this.A } : new[] { this.R, this.G, this.B });
        }

        public readonly LinearRgbColor Clamp()
        {
            return new LinearRgbColor(MathExtensions.Clamp(this.R, 0.0, 1.0), MathExtensions.Clamp(this.G, 0.0, 1.0), MathExtensions.Clamp(this.B, 0.0, 1.0), MathExtensions.Clamp(this.A, 0.0, 1.0));
        }

        public readonly LinearRgbColor Round(double scale)
        {
            return new LinearRgbColor(Math.Round(this.R / scale) * scale, Math.Round(this.G / scale) * scale, Math.Round(this.B / scale) * scale, Math.Round(this.A / scale) * scale);
        }

        private static double SrgbTransfer(double x)
        {
            return 0.0031308 >= x ? 12.92 * x : 1.055 * Math.Pow(x, 0.4166666666666667) - 0.055;
        }
    }
}
