namespace Shaderlens
{
    public class vec2
    {
        public Float this[int index] { get { return default; } set { } }
        public Float this[uint index] { get { return default; } set { } }

        public Float x, y;
        public vec2 xy, yx;
        public vec3 xxx, xxy, xyx, xyy, yxx, yxy, yyx, yyy;
        public vec4 xxxx, xxxy, xxyx, xxyy, xyxx, xyxy, xyyx, xyyy, yxxx, yxxy, yxyx, yxyy, yyxx, yyxy, yyyx, yyyy;

        public Float r, g;
        public vec2 rg, gr;
        public vec3 rrr, rrg, rgr, rgg, grr, grg, ggr, ggg;
        public vec4 rrrr, rrrg, rrgr, rrgg, rgrr, rgrg, rggr, rggg, grrr, grrg, grgr, grgg, ggrr, ggrg, gggr, gggg;

        public static vec2 operator +(vec2 v1, vec2 v2) { return default; }
        public static vec2 operator +(Float v1, vec2 v2) { return default; }
        public static vec2 operator +(vec2 v1, Float v2) { return default; }
        public static vec2 operator -(vec2 v) { return default; }
        public static vec2 operator -(vec2 v1, vec2 v2) { return default; }
        public static vec2 operator -(vec2 v1, Float v2) { return default; }
        public static vec2 operator -(Float v1, vec2 v2) { return default; }
        public static vec2 operator *(Float v1, vec2 v2) { return default; }
        public static vec2 operator *(vec2 v1, Float v2) { return default; }
        public static vec2 operator *(vec2 v1, vec2 v2) { return default; }
        public static vec2 operator /(Float v1, vec2 v2) { return default; }
        public static vec2 operator /(vec2 v1, Float v2) { return default; }
        public static vec2 operator /(vec2 v1, vec2 v2) { return default; }
    }

    public static partial class Environment
    {
        public static vec2 vec2(Float xy) { return default; }
        public static vec2 vec2(Float x, Float y) { return default; }

        public static vec2 vec2(vec2 xy) { return default; }
        public static vec2 vec2(ivec2 xy) { return default; }
        public static vec2 vec2(uvec2 xy) { return default; }
        public static vec2 vec2(bvec2 xy) { return default; }
    }

    public class ivec2
    {
        public int this[int index] { get { return default; } set { } }
        public int this[uint index] { get { return default; } set { } }

        public int x, y;
        public ivec2 xy, yx;
        public ivec3 xxx, xxy, xyx, xyy, yxx, yxy, yyx, yyy;
        public ivec4 xxxx, xxxy, xxyx, xxyy, xyxx, xyxy, xyyx, xyyy, yxxx, yxxy, yxyx, yxyy, yyxx, yyxy, yyyx, yyyy;

        public int r, g;
        public ivec2 rg, gr;
        public ivec3 rrr, rrg, rgr, rgg, grr, grg, ggr, ggg;
        public ivec4 rrrr, rrrg, rrgr, rrgg, rgrr, rgrg, rggr, rggg, grrr, grrg, grgr, grgg, ggrr, ggrg, gggr, gggg;

        public static ivec2 operator +(ivec2 v1, ivec2 v2) { return default; }
        public static ivec2 operator +(int v1, ivec2 v2) { return default; }
        public static ivec2 operator +(ivec2 v1, int v2) { return default; }
        public static ivec2 operator -(ivec2 v) { return default; }
        public static ivec2 operator -(ivec2 v1, ivec2 v2) { return default; }
        public static ivec2 operator -(ivec2 v1, int v2) { return default; }
        public static ivec2 operator -(int v1, ivec2 v2) { return default; }
        public static ivec2 operator *(int v1, ivec2 v2) { return default; }
        public static ivec2 operator *(ivec2 v1, int v2) { return default; }
        public static ivec2 operator *(ivec2 v1, ivec2 v2) { return default; }
        public static ivec2 operator /(int v1, ivec2 v2) { return default; }
        public static ivec2 operator /(ivec2 v1, int v2) { return default; }
        public static ivec2 operator /(ivec2 v1, ivec2 v2) { return default; }
    }

    public static partial class Environment
    {
        public static ivec2 ivec2(int xy) { return default; }
        public static ivec2 ivec2(int x, int y) { return default; }

        public static ivec2 ivec2(vec2 xy) { return default; }
        public static ivec2 ivec2(ivec2 xy) { return default; }
        public static ivec2 ivec2(uvec2 xy) { return default; }
        public static ivec2 ivec2(bvec2 xy) { return default; }
    }

    public class uvec2
    {
        public uint this[int index] { get { return default; } set { } }
        public uint this[uint index] { get { return default; } set { } }

        public uint x, y;
        public uvec2 xy, yx;
        public uvec3 xxx, xxy, xyx, xyy, yxx, yxy, yyx, yyy;
        public uvec4 xxxx, xxxy, xxyx, xxyy, xyxx, xyxy, xyyx, xyyy, yxxx, yxxy, yxyx, yxyy, yyxx, yyxy, yyyx, yyyy;

        public uint r, g;
        public uvec2 rg, gr;
        public uvec3 rrr, rrg, rgr, rgg, grr, grg, ggr, ggg;
        public uvec4 rrrr, rrrg, rrgr, rrgg, rgrr, rgrg, rggr, rggg, grrr, grrg, grgr, grgg, ggrr, ggrg, gggr, gggg;

        public static uvec2 operator +(uvec2 v1, uvec2 v2) { return default; }
        public static uvec2 operator +(uint v1, uvec2 v2) { return default; }
        public static uvec2 operator +(uvec2 v1, uint v2) { return default; }
        public static uvec2 operator -(uvec2 v) { return default; }
        public static uvec2 operator -(uvec2 v1, uvec2 v2) { return default; }
        public static uvec2 operator -(uvec2 v1, uint v2) { return default; }
        public static uvec2 operator -(uint v1, uvec2 v2) { return default; }
        public static uvec2 operator *(uint v1, uvec2 v2) { return default; }
        public static uvec2 operator *(uvec2 v1, uint v2) { return default; }
        public static uvec2 operator *(uvec2 v1, uvec2 v2) { return default; }
        public static uvec2 operator /(uint v1, uvec2 v2) { return default; }
        public static uvec2 operator /(uvec2 v1, uint v2) { return default; }
        public static uvec2 operator /(uvec2 v1, uvec2 v2) { return default; }
    }

    public static partial class Environment
    {
        public static uvec2 uvec2(uint xy) { return default; }
        public static uvec2 uvec2(uint x, uint y) { return default; }

        public static uvec2 uvec2(vec2 xy) { return default; }
        public static uvec2 uvec2(ivec2 xy) { return default; }
        public static uvec2 uvec2(uvec2 xy) { return default; }
        public static uvec2 uvec2(bvec2 xy) { return default; }
    }

    public class bvec2
    {
        public bool this[int index] { get { return default; } set { } }
        public bool this[uint index] { get { return default; } set { } }

        public bool x, y;
        public bvec2 xy, yx;
        public bvec3 xxx, xxy, xyx, xyy, yxx, yxy, yyx, yyy;
        public bvec4 xxxx, xxxy, xxyx, xxyy, xyxx, xyxy, xyyx, xyyy, yxxx, yxxy, yxyx, yxyy, yyxx, yyxy, yyyx, yyyy;

        public bool r, g;
        public bvec2 rg, gr;
        public bvec3 rrr, rrg, rgr, rgg, grr, grg, ggr, ggg;
        public bvec4 rrrr, rrrg, rrgr, rrgg, rgrr, rgrg, rggr, rggg, grrr, grrg, grgr, grgg, ggrr, ggrg, gggr, gggg;
    }

    public static partial class Environment
    {
        public static bvec2 bvec2(bool xy) { return default; }
        public static bvec2 bvec2(bool x, bool y) { return default; }

        public static bvec2 bvec2(vec2 xy) { return default; }
        public static bvec2 bvec2(ivec2 xy) { return default; }
        public static bvec2 bvec2(uvec2 xy) { return default; }
        public static bvec2 bvec2(bvec2 xy) { return default; }
    }

    public class dvec2
    {
        public double this[int index] { get { return default; } set { } }
        public double this[uint index] { get { return default; } set { } }

        public double x, y;
        public dvec2 xy, yx;
        public vec3 xxx, xxy, xyx, xyy, yxx, yxy, yyx, yyy;
        public vec4 xxxx, xxxy, xxyx, xxyy, xyxx, xyxy, xyyx, xyyy, yxxx, yxxy, yxyx, yxyy, yyxx, yyxy, yyyx, yyyy;

        public double r, g;
        public dvec2 rg, gr;
        public vec3 rrr, rrg, rgr, rgg, grr, grg, ggr, ggg;
        public vec4 rrrr, rrrg, rrgr, rrgg, rgrr, rgrg, rggr, rggg, grrr, grrg, grgr, grgg, ggrr, ggrg, gggr, gggg;

        public static dvec2 operator +(dvec2 v1, dvec2 v2) { return default; }
        public static dvec2 operator +(double v1, dvec2 v2) { return default; }
        public static dvec2 operator +(dvec2 v1, double v2) { return default; }
        public static dvec2 operator -(dvec2 v) { return default; }
        public static dvec2 operator -(dvec2 v1, dvec2 v2) { return default; }
        public static dvec2 operator -(dvec2 v1, double v2) { return default; }
        public static dvec2 operator -(double v1, dvec2 v2) { return default; }
        public static dvec2 operator *(double v1, dvec2 v2) { return default; }
        public static dvec2 operator *(dvec2 v1, double v2) { return default; }
        public static dvec2 operator *(dvec2 v1, dvec2 v2) { return default; }
        public static dvec2 operator /(double v1, dvec2 v2) { return default; }
        public static dvec2 operator /(dvec2 v1, double v2) { return default; }
        public static dvec2 operator /(dvec2 v1, dvec2 v2) { return default; }
    }

    public static partial class Environment
    {
        public static dvec2 dvec2(double xy) { return default; }
        public static dvec2 dvec2(double x, double y) { return default; }

        public static dvec2 dvec2(vec2 xy) { return default; }
        public static dvec2 dvec2(dvec2 xy) { return default; }
        public static dvec2 dvec2(ivec2 xy) { return default; }
        public static dvec2 dvec2(uvec2 xy) { return default; }
        public static dvec2 dvec2(bvec2 xy) { return default; }
    }
}
