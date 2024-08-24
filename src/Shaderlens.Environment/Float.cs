namespace Shaderlens
{
    public struct Float
    {
        public static implicit operator Float(float value) { return default; }
        public static implicit operator float(Float value) { return default; }
        public static implicit operator Float(double value) { return default; }
        public static implicit operator double(Float value) { return default; }

        public static Float operator +(Float value1, Float value2) { return default; }
        public static Float operator -(Float value1, Float value2) { return default; }
        public static Float operator *(Float value1, Float value2) { return default; }
        public static Float operator /(Float value1, Float value2) { return default; }
    }
}
