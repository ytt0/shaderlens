namespace Shaderlens
{
    public class mat2
    {
        public Float this[int index] { get { return default; } set { } }
        public Float this[uint index] { get { return default; } set { } }

        public static mat2 operator +(mat2 v1, mat2 v2) { return default; }
        public static mat2 operator +(Float v1, mat2 v2) { return default; }
        public static mat2 operator +(mat2 v1, Float v2) { return default; }
        public static mat2 operator -(mat2 v) { return default; }
        public static mat2 operator -(mat2 v1, mat2 v2) { return default; }
        public static mat2 operator -(mat2 v1, Float v2) { return default; }
        public static mat2 operator *(Float v1, mat2 v2) { return default; }
        public static mat2 operator *(mat2 v1, Float v2) { return default; }
        public static mat2 operator *(mat2 v1, mat2 v2) { return default; }
        public static mat2 operator /(mat2 v1, Float v2) { return default; }
        public static mat2 operator /(mat2 v1, mat2 v2) { return default; }
        public static vec2 operator *(mat2 v1, vec2 v2) { return default; }
        public static vec2 operator *(vec2 v1, mat2 v2) { return default; }
    }

    public static partial class Environment
    {
        public static mat2 mat2(Float xz) { return default; }

        public static mat2 mat2(Float x, Float y, Float z, Float w) { return default; }
        public static mat2 mat2(vec2 xy, vec2 zw) { return default; }
        public static mat2 mat2(vec4 xyzw) { return default; }
    }
}
