namespace Shaderlens
{
    public class mat4
    {
        public Float this[int index] { get { return default; } set { } }
        public Float this[uint index] { get { return default; } set { } }

        public static mat4 operator +(mat4 v1, mat4 v2) { return default; }
        public static mat4 operator +(Float v1, mat4 v2) { return default; }
        public static mat4 operator +(mat4 v1, Float v2) { return default; }
        public static mat4 operator -(mat4 v) { return default; }
        public static mat4 operator -(mat4 v1, mat4 v2) { return default; }
        public static mat4 operator -(mat4 v1, Float v2) { return default; }
        public static mat4 operator *(Float v1, mat4 v2) { return default; }
        public static mat4 operator *(mat4 v1, Float v2) { return default; }
        public static mat4 operator *(mat4 v1, mat4 v2) { return default; }
        public static mat4 operator /(mat4 v1, Float v2) { return default; }
        public static mat4 operator /(mat4 v1, mat4 v2) { return default; }
        public static vec4 operator *(mat4 v1, vec4 v2) { return default; }
        public static vec4 operator *(vec4 v1, mat4 v2) { return default; }
    }

    public static partial class Environment
    {
        public static mat4 mat4(Float v) { return default; }
        public static mat4 mat4(vec4 c1, vec4 c2, vec4 c3, vec4 c4) { return default; }
    }
}
