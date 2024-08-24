namespace Shaderlens
{
    public struct OkhsvColor
    {
        private const double Epsilon = 0.0001;

        public class HueProperties
        {
            public double H { get; }
            public double A { get; }
            public double B { get; }
            public double SMax { get; }
            public double TMax { get; }

            public HueProperties(double h)
            {
                this.H = h;
                this.A = Math.Cos(MathExtensions.Tau * h);
                this.B = Math.Sin(MathExtensions.Tau * h);

                GetSTMax(this.A, this.B, out var s, out var t);
                this.SMax = s;
                this.TMax = t;
            }
        }

        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }

        public OkhsvColor(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        public readonly LinearRgbColor ToLinearRgb()
        {
            return ToLinearRgb(new HueProperties(this.H));
        }

        public readonly LinearRgbColor ToLinearRgb(HueProperties okhueProperties)
        {
            if (okhueProperties.H != this.H)
            {
                throw new ArgumentException($"Invalid hue properties ({okhueProperties.H} != {this.H})");
            }

            var a = okhueProperties.A;
            var b = okhueProperties.B;

            var S_0 = 0.5;
            var T = okhueProperties.TMax;
            var k = 1 - S_0 / okhueProperties.SMax;

            var L_v = 1 - this.S * S_0 / (S_0 + T - T * k * this.S);
            var c_v = this.S * T * S_0 / (S_0 + T - T * k * this.S);

            var L = this.V * L_v;
            var C = this.V * c_v;

            var L_vt = ToeInverse(L_v);
            var c_vt = c_v * L_vt / L_v;

            var L_new = ToeInverse(L); // * L_v/L_vt;
            C = C * L_new / Math.Max(Epsilon, L);
            L = L_new;

            var rgb_scale = new OklabColor(L_vt, a * c_vt, b * c_vt).ToLinearRgb();
            var scale_L = Cbrt(1 / Max(rgb_scale.R, rgb_scale.G, rgb_scale.B, 0));

            // remove to see effect without rescaling
            L = L * scale_L;
            C = C * scale_L;

            return new OklabColor(L, C * a, C * b).ToLinearRgb();
        }

        public readonly OkhsvColor Clamp()
        {
            return new OkhsvColor(MathExtensions.Clamp(this.H, 0.0, 1.0), MathExtensions.Clamp(this.S, 0.0, 1.0), MathExtensions.Clamp(this.V, 0.0, 1.0));
        }

        public static OkhsvColor FromLinearRgb(LinearRgbColor c)
        {
            return FromLab(OklabColor.FromLinearRgb(c));
        }

        public static OkhsvColor FromLab(OklabColor lab)
        {
            var C = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
            var a_ = C < Epsilon ? 0.0 : lab.A / C;
            var b_ = C < Epsilon ? 0.0 : lab.B / C;

            var L = lab.L;
            var h = C < Epsilon ? 0.0 : 0.5 + 0.5 * Math.Atan2(-lab.B, -lab.A) / Math.PI;

            GetSTMax(a_, b_, out var S_max, out var T_max);
            var S_0 = 0.5;
            var T = T_max;
            var k = 1 - S_0 / S_max;

            var t = C + L * T < Epsilon ? 0.0 : T / (C + L * T);
            var L_v = t * L;
            var c_v = t * C;

            var L_vt = ToeInverse(L_v);
            var c_vt = L_v < Epsilon ? 0.0 : c_v * L_vt / L_v;

            var rgb_scale = new OklabColor(L_vt, a_ * c_vt, b_ * c_vt).ToLinearRgb();
            var scale_L = Cbrt(1 / Max(rgb_scale.R, rgb_scale.G, rgb_scale.B, Epsilon));

            L = L / scale_L;
            //C = C / scale_L;

            //C = C * Toe(L) / L;
            L = Toe(L);

            var v = L_v < Epsilon ? 0.0 : L / L_v;
            var s = (S_0 + T) * c_v / Math.Max(Epsilon, T * S_0 + T * k * c_v);

            return new OkhsvColor(h, s, v);
        }

        private static double Toe(double x)
        {
            var k_1 = 0.206;
            var k_2 = 0.03;
            var k_3 = (1 + k_1) / (1 + k_2);

            return 0.5 * (k_3 * x - k_1 + Math.Sqrt((k_3 * x - k_1) * (k_3 * x - k_1) + 4 * k_2 * k_3 * x));
        }

        private static double ToeInverse(double x)
        {
            var k_1 = 0.206;
            var k_2 = 0.03;
            var k_3 = (1 + k_1) / (1 + k_2);
            return (x * x + k_1 * x) / (k_3 * (x + k_2));
        }

        // Finds the maximum saturation possible for a given hue that fits in sRGB
        // Saturation here is defined as S = C/L
        // a and b must be normalized so a^2 + b^2 == 1
        private static double ComputeMaxSaturation(double a, double b)
        {
            // Max saturation will be when one of r, g or b goes below zero.

            // Select different coefficients depending on which component goes below zero first
            double k0, k1, k2, k3, k4, wl, wm, ws;

            if (-1.88170328 * a - 0.80936493 * b > 1)
            {
                // Red component
                k0 = +1.19086277; k1 = +1.76576728; k2 = +0.59662641; k3 = +0.75515197; k4 = +0.56771245;
                wl = +4.0767416621; wm = -3.3077115913; ws = +0.2309699292;
            }
            else if (1.81444104 * a - 1.19445276 * b > 1)
            {
                // Green component
                k0 = +0.73956515; k1 = -0.45954404; k2 = +0.08285427; k3 = +0.12541070; k4 = +0.14503204;
                wl = -1.2684380046; wm = +2.6097574011; ws = -0.3413193965;
            }
            else
            {
                // Blue component
                k0 = +1.35733652; k1 = -0.00915799; k2 = -1.15130210; k3 = -0.50559606; k4 = +0.00692167;
                wl = -0.0041960863; wm = -0.7034186147; ws = +1.7076147010;
            }

            // Approximate max saturation using a polynomial:
            var S = k0 + k1 * a + k2 * b + k3 * a * a + k4 * a * b;

            // Do one step Halley's method to get closer
            // this gives an error less than 10e6, except for some blue hues where the dS/dh is close to infinite
            // this should be sufficient for most applications, otherwise do two/three steps

            var k_l = +0.3963377774 * a + 0.2158037573 * b;
            var k_m = -0.1055613458 * a - 0.0638541728 * b;
            var k_s = -0.0894841775 * a - 1.2914855480 * b;

            {
                var l_ = 1 + S * k_l;
                var m_ = 1 + S * k_m;
                var s_ = 1 + S * k_s;

                var l = l_ * l_ * l_;
                var m = m_ * m_ * m_;
                var s = s_ * s_ * s_;

                var l_dS = 3 * k_l * l_ * l_;
                var m_dS = 3 * k_m * m_ * m_;
                var s_dS = 3 * k_s * s_ * s_;

                var l_dS2 = 6 * k_l * k_l * l_;
                var m_dS2 = 6 * k_m * k_m * m_;
                var s_dS2 = 6 * k_s * k_s * s_;

                var f = wl * l + wm * m + ws * s;
                var f1 = wl * l_dS + wm * m_dS + ws * s_dS;
                var f2 = wl * l_dS2 + wm * m_dS2 + ws * s_dS2;

                S = S - f * f1 / (f1 * f1 - 0.5 * f * f2);
            }

            return S;
        }

        private static void FindCusp(double a, double b, out double l, out double c)
        {
            // First, find the maximum saturation (saturation S = C/L)
            var s = ComputeMaxSaturation(a, b);

            // Convert to linear sRGB to find the first point where at least one of r,g or b >= 1:
            var rgb_at_max = new OklabColor(1, s * a, s * b).ToLinearRgb();
            l = Cbrt(1 / Max(rgb_at_max.R, rgb_at_max.G, rgb_at_max.B));
            c = l * s;
        }

        private static void GetSTMax(double a_, double b_, out double s, out double t)
        {
            if (Math.Abs(a_) + Math.Abs(b_) < Epsilon)
            {
                s = 1.0;
                t = 1.0;
            }
            else
            {
                FindCusp(a_, b_, out var l, out var c);
                s = c / l;
                t = c / (1 - l);
            }
        }

        private static double Max(double a, double b, double c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        private static double Max(double a, double b, double c, double d)
        {
            return Math.Max(Math.Max(a, b), Math.Max(c, d));
        }

        private static double Cbrt(double v)
        {
            return Math.Pow(v, 1.0 / 3.0);
        }
    }
}
