namespace Shaderlens
{
    public struct SrgbColor
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double A { get; set; }

        public SrgbColor(double grayscale) :
            this(grayscale, grayscale, grayscale)
        {
        }

        public SrgbColor(byte[] values) :
            this(values[0] / 255.0, values[1] / 255.0, values[2] / 255.0, values.Length == 3 ? 1.0 : values[3] / 255.0)
        {
        }

        public SrgbColor(IEnumerable<double> values) :
            this(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2), values.Count() < 4 ? 1.0 : values.ElementAt(3))
        {
        }

        public SrgbColor(double r, double g, double b) :
            this(r, g, b, 1.0)
        {
        }

        public SrgbColor(double r, double g, double b, double a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public readonly LinearRgbColor ToLinearRgb()
        {
            return new LinearRgbColor(SrgbTransferInverse(this.R), SrgbTransferInverse(this.G), SrgbTransferInverse(this.B), this.A);
        }

        public readonly Vector<double> ToVector(bool includeAlpha)
        {
            return Vector.Create(includeAlpha ? new[] { this.R, this.G, this.B, this.A } : new[] { this.R, this.G, this.B });
        }

        public readonly SrgbColor Clamp()
        {
            return new SrgbColor(MathExtensions.Clamp(this.R, 0.0, 1.0), MathExtensions.Clamp(this.G, 0.0, 1.0), MathExtensions.Clamp(this.B, 0.0, 1.0), MathExtensions.Clamp(this.A, 0.0, 1.0));
        }

        public readonly SrgbColor Round(double scale)
        {
            return new SrgbColor(Math.Round(this.R / scale) * scale, Math.Round(this.G / scale) * scale, Math.Round(this.B / scale) * scale, Math.Round(this.A / scale) * scale);
        }

        private static double SrgbTransferInverse(double x)
        {
            return 0.04045 < x ? Math.Pow((x + 0.055) / 1.055, 2.4) : x / 12.92;
        }
    }
}
