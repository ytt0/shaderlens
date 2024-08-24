namespace Shaderlens
{
    public class mat3
    {
        public Float this[int index] { get { return default; } set { } }
        public Float this[uint index] { get { return default; } set { } }

        public static mat3 operator +(mat3 v1, mat3 v2) { return default; }
        public static mat3 operator +(Float v1, mat3 v2) { return default; }
        public static mat3 operator +(mat3 v1, Float v2) { return default; }
        public static mat3 operator -(mat3 v) { return default; }
        public static mat3 operator -(mat3 v1, mat3 v2) { return default; }
        public static mat3 operator -(mat3 v1, Float v2) { return default; }
        public static mat3 operator *(Float v1, mat3 v2) { return default; }
        public static mat3 operator *(mat3 v1, Float v2) { return default; }
        public static mat3 operator *(mat3 v1, mat3 v2) { return default; }
        public static mat3 operator /(mat3 v1, Float v2) { return default; }
        public static mat3 operator /(mat3 v1, mat3 v2) { return default; }
        public static vec3 operator *(mat3 v1, vec3 v2) { return default; }
        public static vec3 operator *(vec3 v1, mat3 v2) { return default; }
    }

    public static partial class Environment
    {
        public static mat3 mat3(Float v) { return default; }
        public static mat3 mat3(vec3 c1, vec3 c2, vec3 c3) { return default; }
    }
}
