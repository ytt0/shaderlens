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

        public LinearRgbColor(double[] values) :
            this(values[0], values[1], values[2], values.Length == 3 ? 1.0 : values[3])
        {
        }

        public LinearRgbColor(double r, double g, double b) :
            this(r, g, b, 1.0)
        {
        }

        public LinearRgbColor(Vec3 value) :
            this(value.X, value.Y, value.Z)
        {
        }

        public LinearRgbColor(Vec4 value) :
            this(value.X, value.Y, value.Z, value.W)
        {
        }

        public LinearRgbColor(double r, double g, double b, double a)
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

        public readonly Vec3 ToVec3()
        {
            return new Vec3(this.R, this.G, this.B);
        }

        public readonly Vec4 ToVec4()
        {
            return new Vec4(this.R, this.G, this.B, this.A);
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
