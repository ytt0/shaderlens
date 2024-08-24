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

        public SrgbColor(double[] values) :
            this(values[0], values[1], values[2], values.Length == 3 ? 1.0 : values[3])
        {
        }

        public SrgbColor(double r, double g, double b) :
            this(r, g, b, 1.0)
        {
        }

        public SrgbColor(Vec3 value) :
            this(value.X, value.Y, value.Z)
        {
        }

        public SrgbColor(Vec4 value) :
            this(value.X, value.Y, value.Z, value.W)
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

        public readonly Vec3 ToVec3()
        {
            return new Vec3(this.R, this.G, this.B);
        }

        public readonly Vec4 ToVec4()
        {
            return new Vec4(this.R, this.G, this.B, this.A);
        }

        public readonly SrgbColor Clamp()
        {
            return new SrgbColor(MathExtensions.Clamp(this.R, 0.0, 1.0), MathExtensions.Clamp(this.G, 0.0, 1.0), MathExtensions.Clamp(this.B, 0.0, 1.0), MathExtensions.Clamp(this.A, 0.0, 1.0));
        }

        private static double SrgbTransferInverse(double x)
        {
            return 0.04045 < x ? Math.Pow((x + 0.055) / 1.055, 2.4) : x / 12.92;
        }
    }
}
