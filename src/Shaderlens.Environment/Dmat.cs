namespace Shaderlens
{
    // dmat<columns>x<rows>

    public class dmat { }

    // a double-precision floating-point matrix with 2 columns and 2 rows
    public class dmat2 : dmat
    {
        public dvec2 this[int index] { get { return default; } set { } }
        public dvec2 this[uint index] { get { return default; } set { } }

        public static dmat2 operator +(dmat2 v1, dmat2 v2) { return default; }
        public static dmat2 operator +(double v1, dmat2 v2) { return default; }
        public static dmat2 operator +(dmat2 v1, double v2) { return default; }
        public static dmat2 operator -(dmat2 v) { return default; }
        public static dmat2 operator -(dmat2 v1, dmat2 v2) { return default; }
        public static dmat2 operator -(dmat2 v1, double v2) { return default; }
        public static dmat2 operator *(double v1, dmat2 v2) { return default; }
        public static dmat2 operator *(dmat2 v1, double v2) { return default; }
        public static dmat2 operator /(dmat2 v1, double v2) { return default; }
        public static dmat2 operator /(dmat2 v1, dmat2 v2) { return default; }

        public static dvec2 operator *(dvec2 v1, dmat2 v2) { return default; }
        public static dvec2 operator *(dmat2 v1, dvec2 v2) { return default; }
        public static dmat2 operator *(dmat2 v1, dmat2 v2) { return default; }
        public static dmat3x2 operator *(dmat2 v1, dmat3x2 v2) { return default; }
        public static dmat4x2 operator *(dmat2 v1, dmat4x2 v2) { return default; }
    }

    // a double-precision floating-point matrix with 2 columns and 3 rows
    public class dmat2x3 : dmat
    {
        public dvec3 this[int index] { get { return default; } set { } }
        public dvec3 this[uint index] { get { return default; } set { } }

        public static dmat2x3 operator +(dmat2x3 v1, dmat2x3 v2) { return default; }
        public static dmat2x3 operator +(double v1, dmat2x3 v2) { return default; }
        public static dmat2x3 operator +(dmat2x3 v1, double v2) { return default; }
        public static dmat2x3 operator -(dmat2x3 v) { return default; }
        public static dmat2x3 operator -(dmat2x3 v1, dmat2x3 v2) { return default; }
        public static dmat2x3 operator -(dmat2x3 v1, double v2) { return default; }
        public static dmat2x3 operator *(double v1, dmat2x3 v2) { return default; }
        public static dmat2x3 operator *(dmat2x3 v1, double v2) { return default; }
        public static dmat2x3 operator /(dmat2x3 v1, double v2) { return default; }
        public static dmat2x3 operator /(dmat2x3 v1, dmat2x3 v2) { return default; }

        public static dvec2 operator *(dvec3 v1, dmat2x3 v2) { return default; }
        public static dvec3 operator *(dmat2x3 v1, dvec2 v2) { return default; }
        public static dmat2x3 operator *(dmat2x3 v1, dmat2 v2) { return default; }
        public static dmat3 operator *(dmat2x3 v1, dmat3x2 v2) { return default; }
        public static dmat4x3 operator *(dmat2x3 v1, dmat4x2 v2) { return default; }
    }

    // a double-precision floating-point matrix with 2 columns and 4 rows
    public class dmat2x4 : dmat
    {
        public dvec4 this[int index] { get { return default; } set { } }
        public dvec4 this[uint index] { get { return default; } set { } }

        public static dmat2x4 operator +(dmat2x4 v1, dmat2x4 v2) { return default; }
        public static dmat2x4 operator +(double v1, dmat2x4 v2) { return default; }
        public static dmat2x4 operator +(dmat2x4 v1, double v2) { return default; }
        public static dmat2x4 operator -(dmat2x4 v) { return default; }
        public static dmat2x4 operator -(dmat2x4 v1, dmat2x4 v2) { return default; }
        public static dmat2x4 operator -(dmat2x4 v1, double v2) { return default; }
        public static dmat2x4 operator *(double v1, dmat2x4 v2) { return default; }
        public static dmat2x4 operator *(dmat2x4 v1, double v2) { return default; }
        public static dmat2x4 operator /(dmat2x4 v1, double v2) { return default; }
        public static dmat2x4 operator /(dmat2x4 v1, dmat2x4 v2) { return default; }

        public static dvec2 operator *(dvec4 v1, dmat2x4 v2) { return default; }
        public static dvec4 operator *(dmat2x4 v1, dvec2 v2) { return default; }
        public static dmat2x4 operator *(dmat2x4 v1, dmat2 v2) { return default; }
        public static dmat3x4 operator *(dmat2x4 v1, dmat3x2 v2) { return default; }
        public static dmat4 operator *(dmat2x4 v1, dmat4x2 v2) { return default; }
    }

    // a double-precision floating-point matrix with 3 columns and 2 rows
    public class dmat3x2 : dmat
    {
        public dvec2 this[int index] { get { return default; } set { } }
        public dvec2 this[uint index] { get { return default; } set { } }

        public static dmat3x2 operator +(dmat3x2 v1, dmat3x2 v2) { return default; }
        public static dmat3x2 operator +(double v1, dmat3x2 v2) { return default; }
        public static dmat3x2 operator +(dmat3x2 v1, double v2) { return default; }
        public static dmat3x2 operator -(dmat3x2 v) { return default; }
        public static dmat3x2 operator -(dmat3x2 v1, dmat3x2 v2) { return default; }
        public static dmat3x2 operator -(dmat3x2 v1, double v2) { return default; }
        public static dmat3x2 operator *(double v1, dmat3x2 v2) { return default; }
        public static dmat3x2 operator *(dmat3x2 v1, double v2) { return default; }
        public static dmat3x2 operator /(dmat3x2 v1, double v2) { return default; }
        public static dmat3x2 operator /(dmat3x2 v1, dmat3x2 v2) { return default; }

        public static dvec3 operator *(dvec2 v1, dmat3x2 v2) { return default; }
        public static dvec2 operator *(dmat3x2 v1, dvec3 v2) { return default; }
        public static dmat2 operator *(dmat3x2 v1, dmat2x3 v2) { return default; }
        public static dmat3x2 operator *(dmat3x2 v1, dmat3 v2) { return default; }
        public static dmat4x2 operator *(dmat3x2 v1, dmat4x3 v2) { return default; }
    }

    // a double-precision floating-point matrix with 3 columns and 3 rows
    public class dmat3 : dmat
    {
        public dvec3 this[int index] { get { return default; } set { } }
        public dvec3 this[uint index] { get { return default; } set { } }

        public static dmat3 operator +(dmat3 v1, dmat3 v2) { return default; }
        public static dmat3 operator +(double v1, dmat3 v2) { return default; }
        public static dmat3 operator +(dmat3 v1, double v2) { return default; }
        public static dmat3 operator -(dmat3 v) { return default; }
        public static dmat3 operator -(dmat3 v1, dmat3 v2) { return default; }
        public static dmat3 operator -(dmat3 v1, double v2) { return default; }
        public static dmat3 operator *(double v1, dmat3 v2) { return default; }
        public static dmat3 operator *(dmat3 v1, double v2) { return default; }
        public static dmat3 operator /(dmat3 v1, double v2) { return default; }
        public static dmat3 operator /(dmat3 v1, dmat3 v2) { return default; }

        public static dvec3 operator *(dvec3 v1, dmat3 v2) { return default; }
        public static dvec3 operator *(dmat3 v1, dvec3 v2) { return default; }
        public static dmat2x3 operator *(dmat3 v1, dmat2x3 v2) { return default; }
        public static dmat3 operator *(dmat3 v1, dmat3 v2) { return default; }
        public static dmat4x3 operator *(dmat3 v1, dmat4x3 v2) { return default; }
    }

    // a double-precision floating-point matrix with 3 columns and 4 rows
    public class dmat3x4 : dmat
    {
        public dvec4 this[int index] { get { return default; } set { } }
        public dvec4 this[uint index] { get { return default; } set { } }

        public static dmat3x4 operator +(dmat3x4 v1, dmat3x4 v2) { return default; }
        public static dmat3x4 operator +(double v1, dmat3x4 v2) { return default; }
        public static dmat3x4 operator +(dmat3x4 v1, double v2) { return default; }
        public static dmat3x4 operator -(dmat3x4 v) { return default; }
        public static dmat3x4 operator -(dmat3x4 v1, dmat3x4 v2) { return default; }
        public static dmat3x4 operator -(dmat3x4 v1, double v2) { return default; }
        public static dmat3x4 operator *(double v1, dmat3x4 v2) { return default; }
        public static dmat3x4 operator *(dmat3x4 v1, double v2) { return default; }
        public static dmat3x4 operator /(dmat3x4 v1, double v2) { return default; }
        public static dmat3x4 operator /(dmat3x4 v1, dmat3x4 v2) { return default; }

        public static dvec3 operator *(dvec4 v1, dmat3x4 v2) { return default; }
        public static dvec4 operator *(dmat3x4 v1, dvec3 v2) { return default; }
        public static dmat2x4 operator *(dmat3x4 v1, dmat2x3 v2) { return default; }
        public static dmat3x4 operator *(dmat3x4 v1, dmat3 v2) { return default; }
        public static dmat4 operator *(dmat3x4 v1, dmat4x3 v2) { return default; }
    }

    // a double-precision floating-point matrix with 4 columns and 2 rows
    public class dmat4x2 : dmat
    {
        public dvec2 this[int index] { get { return default; } set { } }
        public dvec2 this[uint index] { get { return default; } set { } }

        public static dmat4x2 operator +(dmat4x2 v1, dmat4x2 v2) { return default; }
        public static dmat4x2 operator +(double v1, dmat4x2 v2) { return default; }
        public static dmat4x2 operator +(dmat4x2 v1, double v2) { return default; }
        public static dmat4x2 operator -(dmat4x2 v) { return default; }
        public static dmat4x2 operator -(dmat4x2 v1, dmat4x2 v2) { return default; }
        public static dmat4x2 operator -(dmat4x2 v1, double v2) { return default; }
        public static dmat4x2 operator *(double v1, dmat4x2 v2) { return default; }
        public static dmat4x2 operator *(dmat4x2 v1, double v2) { return default; }
        public static dmat4x2 operator /(dmat4x2 v1, double v2) { return default; }
        public static dmat4x2 operator /(dmat4x2 v1, dmat4x2 v2) { return default; }

        public static dvec4 operator *(dvec2 v1, dmat4x2 v2) { return default; }
        public static dvec2 operator *(dmat4x2 v1, dvec4 v2) { return default; }
        public static dmat2 operator *(dmat4x2 v1, dmat2x4 v2) { return default; }
        public static dmat3x2 operator *(dmat4x2 v1, dmat3x4 v2) { return default; }
        public static dmat4x2 operator *(dmat4x2 v1, dmat4 v2) { return default; }
    }

    // a double-precision floating-point matrix with 4 columns and 3 rows
    public class dmat4x3 : dmat
    {
        public dvec3 this[int index] { get { return default; } set { } }
        public dvec3 this[uint index] { get { return default; } set { } }

        public static dmat4x3 operator +(dmat4x3 v1, dmat4x3 v2) { return default; }
        public static dmat4x3 operator +(double v1, dmat4x3 v2) { return default; }
        public static dmat4x3 operator +(dmat4x3 v1, double v2) { return default; }
        public static dmat4x3 operator -(dmat4x3 v) { return default; }
        public static dmat4x3 operator -(dmat4x3 v1, dmat4x3 v2) { return default; }
        public static dmat4x3 operator -(dmat4x3 v1, double v2) { return default; }
        public static dmat4x3 operator *(double v1, dmat4x3 v2) { return default; }
        public static dmat4x3 operator *(dmat4x3 v1, double v2) { return default; }
        public static dmat4x3 operator /(dmat4x3 v1, double v2) { return default; }
        public static dmat4x3 operator /(dmat4x3 v1, dmat4x3 v2) { return default; }

        public static dvec4 operator *(dvec3 v1, dmat4x3 v2) { return default; }
        public static dvec3 operator *(dmat4x3 v1, dvec4 v2) { return default; }
        public static dmat2x3 operator *(dmat4x3 v1, dmat2x4 v2) { return default; }
        public static dmat3 operator *(dmat4x3 v1, dmat3x4 v2) { return default; }
        public static dmat4x3 operator *(dmat4x3 v1, dmat4 v2) { return default; }
    }

    // a double-precision floating-point matrix with 4 columns and 4 rows
    public class dmat4 : dmat
    {
        public dvec4 this[int index] { get { return default; } set { } }
        public dvec4 this[uint index] { get { return default; } set { } }

        public static dmat4 operator +(dmat4 v1, dmat4 v2) { return default; }
        public static dmat4 operator +(double v1, dmat4 v2) { return default; }
        public static dmat4 operator +(dmat4 v1, double v2) { return default; }
        public static dmat4 operator -(dmat4 v) { return default; }
        public static dmat4 operator -(dmat4 v1, dmat4 v2) { return default; }
        public static dmat4 operator -(dmat4 v1, double v2) { return default; }
        public static dmat4 operator *(double v1, dmat4 v2) { return default; }
        public static dmat4 operator *(dmat4 v1, double v2) { return default; }
        public static dmat4 operator /(dmat4 v1, double v2) { return default; }
        public static dmat4 operator /(dmat4 v1, dmat4 v2) { return default; }

        public static dvec4 operator *(dvec4 v1, dmat4 v2) { return default; }
        public static dvec4 operator *(dmat4 v1, dvec4 v2) { return default; }
        public static dmat2x4 operator *(dmat4 v1, dmat2x4 v2) { return default; }
        public static dmat3x4 operator *(dmat4 v1, dmat3x4 v2) { return default; }
        public static dmat4 operator *(dmat4 v1, dmat4 v2) { return default; }
    }

    public static partial class Environment
    {
        public static dmat2 dmat2(dvec4 m) { return default; }
        public static dmat2 dmat2(mat m) { return default; }
        public static dmat2 dmat2(double v) { return default; }
        public static dmat2 dmat2(dvec2 c1, dvec2 c2) { return default; }
        public static dmat2 dmat2(
            double c1r1, double c1r2,
            double c2r1, double c2r2) { return default; }

        public static dmat3 dmat3(mat m) { return default; }
        public static dmat3 dmat3(double v) { return default; }
        public static dmat3 dmat3(dvec3 c1, dvec3 c2, dvec3 c3) { return default; }
        public static dmat3 dmat3(
            double c1r1, double c1r2, double c1r3,
            double c2r1, double c2r2, double c2r3,
            double c3r1, double c3r2, double c3r3) { return default; }

        public static dmat4 dmat4(mat m) { return default; }
        public static dmat4 dmat4(double v) { return default; }
        public static dmat4 dmat4(dvec4 c1, dvec4 c2, dvec4 c3, dvec4 c4) { return default; }
        public static dmat4 dmat4(
            double c1r1, double c1r2, double c1r3, double c1r4,
            double c2r1, double c2r2, double c2r3, double c2r4,
            double c3r1, double c3r2, double c3r3, double c3r4,
            double c4r1, double c4r2, double c4r3, double c4r4) { return default; }

        public static dmat2 dmat2x2(dvec4 v) { return default; }
        public static dmat2 dmat2x2(mat m) { return default; }
        public static dmat2 dmat2x2(double v) { return default; }
        public static dmat2 dmat2x2(dvec2 c1, dvec2 c2) { return default; }
        public static dmat2 dmat2x2(
            double c1r1, double c1r2,
            double c2r1, double c2r2) { return default; }

        public static dmat2x3 dmat2x3(mat m) { return default; }
        public static dmat2x3 dmat2x3(double v) { return default; }
        public static dmat2x3 dmat2x3(dvec3 c1, dvec3 c2) { return default; }
        public static dmat2x3 dmat2x3(
            double c1r1, double c1r2, double c1r3,
            double c2r1, double c2r2, double c2r3) { return default; }

        public static dmat2x4 dmat2x4(mat m) { return default; }
        public static dmat2x4 dmat2x4(double v) { return default; }
        public static dmat2x4 dmat2x4(dvec4 c1, dvec4 c2) { return default; }
        public static dmat2x4 dmat2x4(
            double c1r1, double c1r2, double c1r3, double c1r4,
            double c2r1, double c2r2, double c2r3, double c2r4) { return default; }

        public static dmat3x2 dmat3x2(mat m) { return default; }
        public static dmat3x2 dmat3x2(double v) { return default; }
        public static dmat3x2 dmat3x2(dvec2 c1, dvec2 c2, dvec2 c3) { return default; }
        public static dmat3x2 dmat3x2(
            double c1r1, double c1r2,
            double c2r1, double c2r2,
            double c3r1, double c3r2) { return default; }

        public static dmat3 dmat3x3(mat m) { return default; }
        public static dmat3 dmat3x3(double v) { return default; }
        public static dmat3 dmat3x3(dvec3 c1, dvec3 c2, dvec3 c3) { return default; }
        public static dmat3 dmat3x3(
            double c1r1, double c1r2, double c1r3,
            double c2r1, double c2r2, double c2r3,
            double c3r1, double c3r2, double c3r3) { return default; }

        public static dmat3x4 dmat3x4(mat m) { return default; }
        public static dmat3x4 dmat3x4(double v) { return default; }
        public static dmat3x4 dmat3x4(dvec4 c1, dvec4 c2, dvec4 c3) { return default; }
        public static dmat3x4 dmat3x4(
            double c1r1, double c1r2, double c1r3, double c1r4,
            double c2r1, double c2r2, double c2r3, double c2r4,
            double c3r1, double c3r2, double c3r3, double c3r4) { return default; }

        public static dmat4x2 dmat4x2(mat m) { return default; }
        public static dmat4x2 dmat4x2(double v) { return default; }
        public static dmat4x2 dmat4x2(dvec2 c1, dvec2 c2, dvec2 c3, dvec2 c4) { return default; }
        public static dmat4x2 dmat4x2(
            double c1r1, double c1r2,
            double c2r1, double c2r2,
            double c3r1, double c3r2,
            double c4r1, double c4r2) { return default; }

        public static dmat4x3 dmat4x3(mat m) { return default; }
        public static dmat4x3 dmat4x3(double v) { return default; }
        public static dmat4x3 dmat4x3(dvec3 c1, dvec3 c2, dvec3 c3, dvec3 c4) { return default; }
        public static dmat4x3 dmat4x3(
            double c1r1, double c1r2, double c1r3,
            double c2r1, double c2r2, double c2r3,
            double c3r1, double c3r2, double c3r3,
            double c4r1, double c4r2, double c4r3) { return default; }

        public static dmat4 dmat4x4(mat m) { return default; }
        public static dmat4 dmat4x4(double v) { return default; }
        public static dmat4 dmat4x4(dvec4 c1, dvec4 c2, dvec4 c3, dvec4 c4) { return default; }
        public static dmat4 dmat4x4(
            double c1r1, double c1r2, double c1r3, double c1r4,
            double c2r1, double c2r2, double c2r3, double c2r4,
            double c3r1, double c3r2, double c3r3, double c3r4,
            double c4r1, double c4r2, double c4r3, double c4r4) { return default; }
    }
}
