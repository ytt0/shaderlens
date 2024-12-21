namespace Shaderlens
{
    // mat<columns>x<rows>

    public class mat { }

    // a single-precision floating-point matrix with 2 columns and 2 rows
    public class mat2 : mat
    {
        public vec2 this[int index] { get { return default; } set { } }
        public vec2 this[uint index] { get { return default; } set { } }

        public static mat2 operator +(mat2 v1, mat2 v2) { return default; }
        public static mat2 operator +(Float v1, mat2 v2) { return default; }
        public static mat2 operator +(mat2 v1, Float v2) { return default; }
        public static mat2 operator -(mat2 v) { return default; }
        public static mat2 operator -(mat2 v1, mat2 v2) { return default; }
        public static mat2 operator -(mat2 v1, Float v2) { return default; }
        public static mat2 operator *(Float v1, mat2 v2) { return default; }
        public static mat2 operator *(mat2 v1, Float v2) { return default; }
        public static mat2 operator /(mat2 v1, Float v2) { return default; }
        public static mat2 operator /(mat2 v1, mat2 v2) { return default; }

        public static vec2 operator *(vec2 v1, mat2 v2) { return default; }
        public static vec2 operator *(mat2 v1, vec2 v2) { return default; }
        public static mat2 operator *(mat2 v1, mat2 v2) { return default; }
        public static mat3x2 operator *(mat2 v1, mat3x2 v2) { return default; }
        public static mat4x2 operator *(mat2 v1, mat4x2 v2) { return default; }
    }

    // a single-precision floating-point matrix with 2 columns and 3 rows
    public class mat2x3 : mat
    {
        public vec3 this[int index] { get { return default; } set { } }
        public vec3 this[uint index] { get { return default; } set { } }

        public static mat2x3 operator +(mat2x3 v1, mat2x3 v2) { return default; }
        public static mat2x3 operator +(Float v1, mat2x3 v2) { return default; }
        public static mat2x3 operator +(mat2x3 v1, Float v2) { return default; }
        public static mat2x3 operator -(mat2x3 v) { return default; }
        public static mat2x3 operator -(mat2x3 v1, mat2x3 v2) { return default; }
        public static mat2x3 operator -(mat2x3 v1, Float v2) { return default; }
        public static mat2x3 operator *(Float v1, mat2x3 v2) { return default; }
        public static mat2x3 operator *(mat2x3 v1, Float v2) { return default; }
        public static mat2x3 operator /(mat2x3 v1, Float v2) { return default; }
        public static mat2x3 operator /(mat2x3 v1, mat2x3 v2) { return default; }

        public static vec2 operator *(vec3 v1, mat2x3 v2) { return default; }
        public static vec3 operator *(mat2x3 v1, vec2 v2) { return default; }
        public static mat2x3 operator *(mat2x3 v1, mat2 v2) { return default; }
        public static mat3 operator *(mat2x3 v1, mat3x2 v2) { return default; }
        public static mat4x3 operator *(mat2x3 v1, mat4x2 v2) { return default; }
    }

    // a single-precision floating-point matrix with 2 columns and 4 rows
    public class mat2x4 : mat
    {
        public vec4 this[int index] { get { return default; } set { } }
        public vec4 this[uint index] { get { return default; } set { } }

        public static mat2x4 operator +(mat2x4 v1, mat2x4 v2) { return default; }
        public static mat2x4 operator +(Float v1, mat2x4 v2) { return default; }
        public static mat2x4 operator +(mat2x4 v1, Float v2) { return default; }
        public static mat2x4 operator -(mat2x4 v) { return default; }
        public static mat2x4 operator -(mat2x4 v1, mat2x4 v2) { return default; }
        public static mat2x4 operator -(mat2x4 v1, Float v2) { return default; }
        public static mat2x4 operator *(Float v1, mat2x4 v2) { return default; }
        public static mat2x4 operator *(mat2x4 v1, Float v2) { return default; }
        public static mat2x4 operator /(mat2x4 v1, Float v2) { return default; }
        public static mat2x4 operator /(mat2x4 v1, mat2x4 v2) { return default; }

        public static vec2 operator *(vec4 v1, mat2x4 v2) { return default; }
        public static vec4 operator *(mat2x4 v1, vec2 v2) { return default; }
        public static mat2x4 operator *(mat2x4 v1, mat2 v2) { return default; }
        public static mat3x4 operator *(mat2x4 v1, mat3x2 v2) { return default; }
        public static mat4 operator *(mat2x4 v1, mat4x2 v2) { return default; }
    }

    // a single-precision floating-point matrix with 3 columns and 2 rows
    public class mat3x2 : mat
    {
        public vec2 this[int index] { get { return default; } set { } }
        public vec2 this[uint index] { get { return default; } set { } }

        public static mat3x2 operator +(mat3x2 v1, mat3x2 v2) { return default; }
        public static mat3x2 operator +(Float v1, mat3x2 v2) { return default; }
        public static mat3x2 operator +(mat3x2 v1, Float v2) { return default; }
        public static mat3x2 operator -(mat3x2 v) { return default; }
        public static mat3x2 operator -(mat3x2 v1, mat3x2 v2) { return default; }
        public static mat3x2 operator -(mat3x2 v1, Float v2) { return default; }
        public static mat3x2 operator *(Float v1, mat3x2 v2) { return default; }
        public static mat3x2 operator *(mat3x2 v1, Float v2) { return default; }
        public static mat3x2 operator /(mat3x2 v1, Float v2) { return default; }
        public static mat3x2 operator /(mat3x2 v1, mat3x2 v2) { return default; }

        public static vec3 operator *(vec2 v1, mat3x2 v2) { return default; }
        public static vec2 operator *(mat3x2 v1, vec3 v2) { return default; }
        public static mat2 operator *(mat3x2 v1, mat2x3 v2) { return default; }
        public static mat3x2 operator *(mat3x2 v1, mat3 v2) { return default; }
        public static mat4x2 operator *(mat3x2 v1, mat4x3 v2) { return default; }
    }

    // a single-precision floating-point matrix with 3 columns and 3 rows
    public class mat3 : mat
    {
        public vec3 this[int index] { get { return default; } set { } }
        public vec3 this[uint index] { get { return default; } set { } }

        public static mat3 operator +(mat3 v1, mat3 v2) { return default; }
        public static mat3 operator +(Float v1, mat3 v2) { return default; }
        public static mat3 operator +(mat3 v1, Float v2) { return default; }
        public static mat3 operator -(mat3 v) { return default; }
        public static mat3 operator -(mat3 v1, mat3 v2) { return default; }
        public static mat3 operator -(mat3 v1, Float v2) { return default; }
        public static mat3 operator *(Float v1, mat3 v2) { return default; }
        public static mat3 operator *(mat3 v1, Float v2) { return default; }
        public static mat3 operator /(mat3 v1, Float v2) { return default; }
        public static mat3 operator /(mat3 v1, mat3 v2) { return default; }

        public static vec3 operator *(vec3 v1, mat3 v2) { return default; }
        public static vec3 operator *(mat3 v1, vec3 v2) { return default; }
        public static mat2x3 operator *(mat3 v1, mat2x3 v2) { return default; }
        public static mat3 operator *(mat3 v1, mat3 v2) { return default; }
        public static mat4x3 operator *(mat3 v1, mat4x3 v2) { return default; }
    }

    // a single-precision floating-point matrix with 3 columns and 4 rows
    public class mat3x4 : mat
    {
        public vec4 this[int index] { get { return default; } set { } }
        public vec4 this[uint index] { get { return default; } set { } }

        public static mat3x4 operator +(mat3x4 v1, mat3x4 v2) { return default; }
        public static mat3x4 operator +(Float v1, mat3x4 v2) { return default; }
        public static mat3x4 operator +(mat3x4 v1, Float v2) { return default; }
        public static mat3x4 operator -(mat3x4 v) { return default; }
        public static mat3x4 operator -(mat3x4 v1, mat3x4 v2) { return default; }
        public static mat3x4 operator -(mat3x4 v1, Float v2) { return default; }
        public static mat3x4 operator *(Float v1, mat3x4 v2) { return default; }
        public static mat3x4 operator *(mat3x4 v1, Float v2) { return default; }
        public static mat3x4 operator /(mat3x4 v1, Float v2) { return default; }
        public static mat3x4 operator /(mat3x4 v1, mat3x4 v2) { return default; }

        public static vec3 operator *(vec4 v1, mat3x4 v2) { return default; }
        public static vec4 operator *(mat3x4 v1, vec3 v2) { return default; }
        public static mat2x4 operator *(mat3x4 v1, mat2x3 v2) { return default; }
        public static mat3x4 operator *(mat3x4 v1, mat3 v2) { return default; }
        public static mat4 operator *(mat3x4 v1, mat4x3 v2) { return default; }
    }

    // a single-precision floating-point matrix with 4 columns and 2 rows
    public class mat4x2 : mat
    {
        public vec2 this[int index] { get { return default; } set { } }
        public vec2 this[uint index] { get { return default; } set { } }

        public static mat4x2 operator +(mat4x2 v1, mat4x2 v2) { return default; }
        public static mat4x2 operator +(Float v1, mat4x2 v2) { return default; }
        public static mat4x2 operator +(mat4x2 v1, Float v2) { return default; }
        public static mat4x2 operator -(mat4x2 v) { return default; }
        public static mat4x2 operator -(mat4x2 v1, mat4x2 v2) { return default; }
        public static mat4x2 operator -(mat4x2 v1, Float v2) { return default; }
        public static mat4x2 operator *(Float v1, mat4x2 v2) { return default; }
        public static mat4x2 operator *(mat4x2 v1, Float v2) { return default; }
        public static mat4x2 operator /(mat4x2 v1, Float v2) { return default; }
        public static mat4x2 operator /(mat4x2 v1, mat4x2 v2) { return default; }

        public static vec4 operator *(vec2 v1, mat4x2 v2) { return default; }
        public static vec2 operator *(mat4x2 v1, vec4 v2) { return default; }
        public static mat2 operator *(mat4x2 v1, mat2x4 v2) { return default; }
        public static mat3x2 operator *(mat4x2 v1, mat3x4 v2) { return default; }
        public static mat4x2 operator *(mat4x2 v1, mat4 v2) { return default; }
    }

    // a single-precision floating-point matrix with 4 columns and 3 rows
    public class mat4x3 : mat
    {
        public vec3 this[int index] { get { return default; } set { } }
        public vec3 this[uint index] { get { return default; } set { } }

        public static mat4x3 operator +(mat4x3 v1, mat4x3 v2) { return default; }
        public static mat4x3 operator +(Float v1, mat4x3 v2) { return default; }
        public static mat4x3 operator +(mat4x3 v1, Float v2) { return default; }
        public static mat4x3 operator -(mat4x3 v) { return default; }
        public static mat4x3 operator -(mat4x3 v1, mat4x3 v2) { return default; }
        public static mat4x3 operator -(mat4x3 v1, Float v2) { return default; }
        public static mat4x3 operator *(Float v1, mat4x3 v2) { return default; }
        public static mat4x3 operator *(mat4x3 v1, Float v2) { return default; }
        public static mat4x3 operator /(mat4x3 v1, Float v2) { return default; }
        public static mat4x3 operator /(mat4x3 v1, mat4x3 v2) { return default; }

        public static vec4 operator *(vec3 v1, mat4x3 v2) { return default; }
        public static vec3 operator *(mat4x3 v1, vec4 v2) { return default; }
        public static mat2x3 operator *(mat4x3 v1, mat2x4 v2) { return default; }
        public static mat3 operator *(mat4x3 v1, mat3x4 v2) { return default; }
        public static mat4x3 operator *(mat4x3 v1, mat4 v2) { return default; }
    }

    // a single-precision floating-point matrix with 4 columns and 4 rows
    public class mat4 : mat
    {
        public vec4 this[int index] { get { return default; } set { } }
        public vec4 this[uint index] { get { return default; } set { } }

        public static mat4 operator +(mat4 v1, mat4 v2) { return default; }
        public static mat4 operator +(Float v1, mat4 v2) { return default; }
        public static mat4 operator +(mat4 v1, Float v2) { return default; }
        public static mat4 operator -(mat4 v) { return default; }
        public static mat4 operator -(mat4 v1, mat4 v2) { return default; }
        public static mat4 operator -(mat4 v1, Float v2) { return default; }
        public static mat4 operator *(Float v1, mat4 v2) { return default; }
        public static mat4 operator *(mat4 v1, Float v2) { return default; }
        public static mat4 operator /(mat4 v1, Float v2) { return default; }
        public static mat4 operator /(mat4 v1, mat4 v2) { return default; }

        public static vec4 operator *(vec4 v1, mat4 v2) { return default; }
        public static vec4 operator *(mat4 v1, vec4 v2) { return default; }
        public static mat2x4 operator *(mat4 v1, mat2x4 v2) { return default; }
        public static mat3x4 operator *(mat4 v1, mat3x4 v2) { return default; }
        public static mat4 operator *(mat4 v1, mat4 v2) { return default; }
    }

    public static partial class Environment
    {
        public static mat2 mat2(vec4 v) { return default; }
        public static mat2 mat2(mat m) { return default; }
        public static mat2 mat2(Float v) { return default; }
        public static mat2 mat2(vec2 c1, vec2 c2) { return default; }
        public static mat2 mat2(
            Float c1r1, Float c1r2,
            Float c2r1, Float c2r2) { return default; }

        public static mat3 mat3(mat m) { return default; }
        public static mat3 mat3(Float v) { return default; }
        public static mat3 mat3(vec3 c1, vec3 c2, vec3 c3) { return default; }
        public static mat3 mat3(
            Float c1r1, Float c1r2, Float c1r3,
            Float c2r1, Float c2r2, Float c2r3,
            Float c3r1, Float c3r2, Float c3r3) { return default; }

        public static mat4 mat4(mat m) { return default; }
        public static mat4 mat4(Float v) { return default; }
        public static mat4 mat4(vec4 c1, vec4 c2, vec4 c3, vec4 c4) { return default; }
        public static mat4 mat4(
            Float c1r1, Float c1r2, Float c1r3, Float c1r4,
            Float c2r1, Float c2r2, Float c2r3, Float c2r4,
            Float c3r1, Float c3r2, Float c3r3, Float c3r4,
            Float c4r1, Float c4r2, Float c4r3, Float c4r4) { return default; }

        public static mat2 mat2x2(vec4 v) { return default; }
        public static mat2 mat2x2(mat m) { return default; }
        public static mat2 mat2x2(Float v) { return default; }
        public static mat2 mat2x2(vec2 c1, vec2 c2) { return default; }
        public static mat2 mat2x2(
            Float c1r1, Float c1r2,
            Float c2r1, Float c2r2) { return default; }

        public static mat2x3 mat2x3(mat m) { return default; }
        public static mat2x3 mat2x3(Float v) { return default; }
        public static mat2x3 mat2x3(vec3 c1, vec3 c2) { return default; }
        public static mat2x3 mat2x3(
            Float c1r1, Float c1r2, Float c1r3,
            Float c2r1, Float c2r2, Float c2r3) { return default; }

        public static mat2x3 mat2x4(mat m) { return default; }
        public static mat2x4 mat2x4(Float v) { return default; }
        public static mat2x4 mat2x4(vec4 c1, vec4 c2) { return default; }
        public static mat2x4 mat2x4(
            Float c1r1, Float c1r2, Float c1r3, Float c1r4,
            Float c2r1, Float c2r2, Float c2r3, Float c2r4) { return default; }

        public static mat3x2 mat3x2(mat m) { return default; }
        public static mat3x2 mat3x2(Float v) { return default; }
        public static mat3x2 mat3x2(vec2 c1, vec2 c2, vec2 c3) { return default; }
        public static mat3x2 mat3x2(
            Float c1r1, Float c1r2,
            Float c2r1, Float c2r2,
            Float c3r1, Float c3r2) { return default; }

        public static mat3 mat3x3(mat m) { return default; }
        public static mat3 mat3x3(Float v) { return default; }
        public static mat3 mat3x3(vec3 c1, vec3 c2, vec3 c3) { return default; }
        public static mat3 mat3x3(
            Float c1r1, Float c1r2, Float c1r3,
            Float c2r1, Float c2r2, Float c2r3,
            Float c3r1, Float c3r2, Float c3r3) { return default; }

        public static mat3x4 mat3x4(mat m) { return default; }
        public static mat3x4 mat3x4(Float v) { return default; }
        public static mat3x4 mat3x4(vec4 c1, vec4 c2, vec4 c3) { return default; }
        public static mat3x4 mat3x4(
            Float c1r1, Float c1r2, Float c1r3, Float c1r4,
            Float c2r1, Float c2r2, Float c2r3, Float c2r4,
            Float c3r1, Float c3r2, Float c3r3, Float c3r4) { return default; }

        public static mat4x2 mat4x2(mat m) { return default; }
        public static mat4x2 mat4x2(Float v) { return default; }
        public static mat4x2 mat4x2(vec2 c1, vec2 c2, vec2 c3, vec2 c4) { return default; }
        public static mat4x2 mat4x2(
            Float c1r1, Float c1r2,
            Float c2r1, Float c2r2,
            Float c3r1, Float c3r2,
            Float c4r1, Float c4r2) { return default; }

        public static mat4x3 mat4x3(mat m) { return default; }
        public static mat4x3 mat4x3(Float v) { return default; }
        public static mat4x3 mat4x3(vec3 c1, vec3 c2, vec3 c3, vec3 c4) { return default; }
        public static mat4x3 mat4x3(
            Float c1r1, Float c1r2, Float c1r3,
            Float c2r1, Float c2r2, Float c2r3,
            Float c3r1, Float c3r2, Float c3r3,
            Float c4r1, Float c4r2, Float c4r3) { return default; }

        public static mat4 mat4x4(mat m) { return default; }
        public static mat4 mat4x4(Float v) { return default; }
        public static mat4 mat4x4(vec4 c1, vec4 c2, vec4 c3, vec4 c4) { return default; }
        public static mat4 mat4x4(
            Float c1r1, Float c1r2, Float c1r3, Float c1r4,
            Float c2r1, Float c2r2, Float c2r3, Float c2r4,
            Float c3r1, Float c3r2, Float c3r3, Float c3r4,
            Float c4r1, Float c4r2, Float c4r3, Float c4r4) { return default; }
    }
}
