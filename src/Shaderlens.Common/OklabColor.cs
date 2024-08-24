namespace Shaderlens
{
    public struct OklabColor
    {
        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public OklabColor(double l, double a, double b)
        {
            this.L = l;
            this.A = a;
            this.B = b;
        }

        public readonly LinearRgbColor ToLinearRgb()
        {
            var l_ = this.L + 0.3963377774 * this.A + 0.2158037573 * this.B;
            var m_ = this.L - 0.1055613458 * this.A - 0.0638541728 * this.B;
            var s_ = this.L - 0.0894841775 * this.A - 1.2914855480 * this.B;

            var l = l_ * l_ * l_;
            var m = m_ * m_ * m_;
            var s = s_ * s_ * s_;

            return new LinearRgbColor(
                +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s,
                -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s,
                -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s);
        }

        public static OklabColor FromLinearRgb(LinearRgbColor c)
        {
            var l = 0.4122214708 * c.R + 0.5363325363 * c.G + 0.0514459929 * c.B;
            var m = 0.2119034982 * c.R + 0.6806995451 * c.G + 0.1073969566 * c.B;
            var s = 0.0883024619 * c.R + 0.2817188376 * c.G + 0.6299787005 * c.B;

            var l_ = Cbrt(l);
            var m_ = Cbrt(m);
            var s_ = Cbrt(s);

            return new OklabColor(
                0.2104542553 * l_ + 0.7936177850 * m_ - 0.0040720468 * s_,
                1.9779984951 * l_ - 2.4285922050 * m_ + 0.4505937099 * s_,
                0.0259040371 * l_ + 0.7827717662 * m_ - 0.8086757660 * s_);
        }

        private static double Cbrt(double v)
        {
            return Math.Pow(Math.Max(0.0, v), 1.0 / 3.0);
        }
    }
}
