namespace Shaderlens
{
    public class vec3
    {
        public Float this[int index] { get { return default; } set { } }
        public Float this[uint index] { get { return default; } set { } }

        public Float x, y, z;
        public vec2 xx, xy, xz, yx, yy, yz, zx, zy, zz;
        public vec3 xxx, xxy, xxz, xyx, xyy, xyz, xzx, xzy, xzz, yxx, yxy, yxz, yyx, yyy, yyz, yzx, yzy, yzz, zxx, zxy, zxz, zyx, zyy, zyz, zzx, zzy, zzz;
        public vec4 xxxx, xxxy, xxxz, xxyx, xxyy, xxyz, xxzx, xxzy, xxzz, xyxx, xyxy, xyxz, xyyx, xyyy, xyyz, xyzx, xyzy, xyzz, xzxx, xzxy, xzxz, xzyx, xzyy, xzyz, xzzx, xzzy, xzzz, yxxx, yxxy, yxxz, yxyx, yxyy, yxyz, yxzx, yxzy, yxzz, yyxx, yyxy, yyxz, yyyx, yyyy, yyyz, yyzx, yyzy, yyzz, yzxx, yzxy, yzxz, yzyx, yzyy, yzyz, yzzx, yzzy, yzzz, zxxx, zxxy, zxxz, zxyx, zxyy, zxyz, zxzx, zxzy, zxzz, zyxx, zyxy, zyxz, zyyx, zyyy, zyyz, zyzx, zyzy, zyzz, zzxx, zzxy, zzxz, zzyx, zzyy, zzyz, zzzx, zzzy, zzzz;

        public Float r, g, b;
        public vec2 rr, rg, rb, gr, gg, gb, br, bg, bb;
        public vec3 rrr, rrg, rrb, rgr, rgg, rgb, rbr, rbg, rbb, grr, grg, grb, ggr, ggg, ggb, gbr, gbg, gbb, brr, brg, brb, bgr, bgg, bgb, bbr, bbg, bbb;
        public vec4 rrrr, rrrg, rrrb, rrgr, rrgg, rrgb, rrbr, rrbg, rrbb, rgrr, rgrg, rgrb, rggr, rggg, rggb, rgbr, rgbg, rgbb, rbrr, rbrg, rbrb, rbgr, rbgg, rbgb, rbbr, rbbg, rbbb, grrr, grrg, grrb, grgr, grgg, grgb, grbr, grbg, grbb, ggrr, ggrg, ggrb, gggr, gggg, gggb, ggbr, ggbg, ggbb, gbrr, gbrg, gbrb, gbgr, gbgg, gbgb, gbbr, gbbg, gbbb, brrr, brrg, brrb, brgr, brgg, brgb, brbr, brbg, brbb, bgrr, bgrg, bgrb, bggr, bggg, bggb, bgbr, bgbg, bgbb, bbrr, bbrg, bbrb, bbgr, bbgg, bbgb, bbbr, bbbg, bbbb;

        public static vec3 operator +(vec3 v1, vec3 v2) { return default; }
        public static vec3 operator +(vec3 v1, Float v2) { return default; }
        public static vec3 operator +(Float v1, vec3 v2) { return default; }
        public static vec3 operator -(vec3 v) { return default; }
        public static vec3 operator -(vec3 v1, vec3 v2) { return default; }
        public static vec3 operator -(Float v1, vec3 v2) { return default; }
        public static vec3 operator -(vec3 v1, Float v2) { return default; }
        public static vec3 operator *(vec3 v1, vec3 v2) { return default; }
        public static vec3 operator *(Float v1, vec3 v2) { return default; }
        public static vec3 operator *(vec3 v1, Float v2) { return default; }
        public static vec3 operator /(vec3 v1, vec3 v2) { return default; }
        public static vec3 operator /(Float v1, vec3 v2) { return default; }
        public static vec3 operator /(vec3 v1, Float v2) { return default; }
    }

    public static partial class Environment
    {
        public static vec3 vec3(Float xyz) { return default; }
        public static vec3 vec3(Float x, Float y, Float z) { return default; }
        public static vec3 vec3(Float x, vec2 yz) { return default; }
        public static vec3 vec3(vec2 xy, Float z) { return default; }

        public static vec3 vec3(vec3 xyz) { return default; }
        public static vec3 vec3(ivec3 xyz) { return default; }
        public static vec3 vec3(uvec3 xyz) { return default; }
        public static vec3 vec3(bvec3 xyz) { return default; }
    }

    public class ivec3
    {
        public int this[int index] { get { return default; } set { } }
        public int this[uint index] { get { return default; } set { } }

        public int x, y, z;
        public ivec2 xx, xy, xz, yx, yy, yz, zx, zy, zz;
        public ivec3 xxx, xxy, xxz, xyx, xyy, xyz, xzx, xzy, xzz, yxx, yxy, yxz, yyx, yyy, yyz, yzx, yzy, yzz, zxx, zxy, zxz, zyx, zyy, zyz, zzx, zzy, zzz;
        public ivec4 xxxx, xxxy, xxxz, xxyx, xxyy, xxyz, xxzx, xxzy, xxzz, xyxx, xyxy, xyxz, xyyx, xyyy, xyyz, xyzx, xyzy, xyzz, xzxx, xzxy, xzxz, xzyx, xzyy, xzyz, xzzx, xzzy, xzzz, yxxx, yxxy, yxxz, yxyx, yxyy, yxyz, yxzx, yxzy, yxzz, yyxx, yyxy, yyxz, yyyx, yyyy, yyyz, yyzx, yyzy, yyzz, yzxx, yzxy, yzxz, yzyx, yzyy, yzyz, yzzx, yzzy, yzzz, zxxx, zxxy, zxxz, zxyx, zxyy, zxyz, zxzx, zxzy, zxzz, zyxx, zyxy, zyxz, zyyx, zyyy, zyyz, zyzx, zyzy, zyzz, zzxx, zzxy, zzxz, zzyx, zzyy, zzyz, zzzx, zzzy, zzzz;

        public int r, g, b;
        public ivec2 rr, rg, rb, gr, gg, gb, br, bg, bb;
        public ivec3 rrr, rrg, rrb, rgr, rgg, rgb, rbr, rbg, rbb, grr, grg, grb, ggr, ggg, ggb, gbr, gbg, gbb, brr, brg, brb, bgr, bgg, bgb, bbr, bbg, bbb;
        public ivec4 rrrr, rrrg, rrrb, rrgr, rrgg, rrgb, rrbr, rrbg, rrbb, rgrr, rgrg, rgrb, rggr, rggg, rggb, rgbr, rgbg, rgbb, rbrr, rbrg, rbrb, rbgr, rbgg, rbgb, rbbr, rbbg, rbbb, grrr, grrg, grrb, grgr, grgg, grgb, grbr, grbg, grbb, ggrr, ggrg, ggrb, gggr, gggg, gggb, ggbr, ggbg, ggbb, gbrr, gbrg, gbrb, gbgr, gbgg, gbgb, gbbr, gbbg, gbbb, brrr, brrg, brrb, brgr, brgg, brgb, brbr, brbg, brbb, bgrr, bgrg, bgrb, bggr, bggg, bggb, bgbr, bgbg, bgbb, bbrr, bbrg, bbrb, bbgr, bbgg, bbgb, bbbr, bbbg, bbbb;

        public static ivec3 operator +(ivec3 v1, ivec3 v2) { return default; }
        public static ivec3 operator +(ivec3 v1, int v2) { return default; }
        public static ivec3 operator +(int v1, ivec3 v2) { return default; }
        public static ivec3 operator -(ivec3 v) { return default; }
        public static ivec3 operator -(ivec3 v1, ivec3 v2) { return default; }
        public static ivec3 operator -(int v1, ivec3 v2) { return default; }
        public static ivec3 operator -(ivec3 v1, int v2) { return default; }
        public static ivec3 operator *(ivec3 v1, ivec3 v2) { return default; }
        public static ivec3 operator *(int v1, ivec3 v2) { return default; }
        public static ivec3 operator *(ivec3 v1, int v2) { return default; }
        public static ivec3 operator /(ivec3 v1, ivec3 v2) { return default; }
        public static ivec3 operator /(int v1, ivec3 v2) { return default; }
        public static ivec3 operator /(ivec3 v1, int v2) { return default; }
    }

    public static partial class Environment
    {
        public static ivec3 ivec3(int xyz) { return default; }
        public static ivec3 ivec3(int x, int y, int z) { return default; }
        public static ivec3 ivec3(int x, ivec2 yz) { return default; }
        public static ivec3 ivec3(ivec2 xy, int z) { return default; }

        public static ivec3 ivec3(vec3 xyz) { return default; }
        public static ivec3 ivec3(ivec3 xyz) { return default; }
        public static ivec3 ivec3(uvec3 xyz) { return default; }
        public static ivec3 ivec3(bvec3 xyz) { return default; }
    }

    public class uvec3
    {
        public uint this[int index] { get { return default; } set { } }
        public uint this[uint index] { get { return default; } set { } }

        public uint x, y, z;
        public uvec2 xx, xy, xz, yx, yy, yz, zx, zy, zz;
        public uvec3 xxx, xxy, xxz, xyx, xyy, xyz, xzx, xzy, xzz, yxx, yxy, yxz, yyx, yyy, yyz, yzx, yzy, yzz, zxx, zxy, zxz, zyx, zyy, zyz, zzx, zzy, zzz;
        public uvec4 xxxx, xxxy, xxxz, xxyx, xxyy, xxyz, xxzx, xxzy, xxzz, xyxx, xyxy, xyxz, xyyx, xyyy, xyyz, xyzx, xyzy, xyzz, xzxx, xzxy, xzxz, xzyx, xzyy, xzyz, xzzx, xzzy, xzzz, yxxx, yxxy, yxxz, yxyx, yxyy, yxyz, yxzx, yxzy, yxzz, yyxx, yyxy, yyxz, yyyx, yyyy, yyyz, yyzx, yyzy, yyzz, yzxx, yzxy, yzxz, yzyx, yzyy, yzyz, yzzx, yzzy, yzzz, zxxx, zxxy, zxxz, zxyx, zxyy, zxyz, zxzx, zxzy, zxzz, zyxx, zyxy, zyxz, zyyx, zyyy, zyyz, zyzx, zyzy, zyzz, zzxx, zzxy, zzxz, zzyx, zzyy, zzyz, zzzx, zzzy, zzzz;

        public uint r, g, b;
        public uvec2 rr, rg, rb, gr, gg, gb, br, bg, bb;
        public uvec3 rrr, rrg, rrb, rgr, rgg, rgb, rbr, rbg, rbb, grr, grg, grb, ggr, ggg, ggb, gbr, gbg, gbb, brr, brg, brb, bgr, bgg, bgb, bbr, bbg, bbb;
        public uvec4 rrrr, rrrg, rrrb, rrgr, rrgg, rrgb, rrbr, rrbg, rrbb, rgrr, rgrg, rgrb, rggr, rggg, rggb, rgbr, rgbg, rgbb, rbrr, rbrg, rbrb, rbgr, rbgg, rbgb, rbbr, rbbg, rbbb, grrr, grrg, grrb, grgr, grgg, grgb, grbr, grbg, grbb, ggrr, ggrg, ggrb, gggr, gggg, gggb, ggbr, ggbg, ggbb, gbrr, gbrg, gbrb, gbgr, gbgg, gbgb, gbbr, gbbg, gbbb, brrr, brrg, brrb, brgr, brgg, brgb, brbr, brbg, brbb, bgrr, bgrg, bgrb, bggr, bggg, bggb, bgbr, bgbg, bgbb, bbrr, bbrg, bbrb, bbgr, bbgg, bbgb, bbbr, bbbg, bbbb;

        public static uvec3 operator +(uvec3 v1, uvec3 v2) { return default; }
        public static uvec3 operator +(uvec3 v1, uint v2) { return default; }
        public static uvec3 operator +(uint v1, uvec3 v2) { return default; }
        public static uvec3 operator -(uvec3 v) { return default; }
        public static uvec3 operator -(uvec3 v1, uvec3 v2) { return default; }
        public static uvec3 operator -(uint v1, uvec3 v2) { return default; }
        public static uvec3 operator -(uvec3 v1, uint v2) { return default; }
        public static uvec3 operator *(uvec3 v1, uvec3 v2) { return default; }
        public static uvec3 operator *(uint v1, uvec3 v2) { return default; }
        public static uvec3 operator *(uvec3 v1, uint v2) { return default; }
        public static uvec3 operator /(uvec3 v1, uvec3 v2) { return default; }
        public static uvec3 operator /(uint v1, uvec3 v2) { return default; }
        public static uvec3 operator /(uvec3 v1, uint v2) { return default; }
    }

    public static partial class Environment
    {
        public static uvec3 uvec3(uint xyz) { return default; }
        public static uvec3 uvec3(uint x, uint y, uint z) { return default; }
        public static uvec3 uvec3(uint x, uvec2 yz) { return default; }
        public static uvec3 uvec3(uvec2 xy, uint z) { return default; }

        public static uvec3 uvec3(vec3 xyz) { return default; }
        public static uvec3 uvec3(ivec3 xyz) { return default; }
        public static uvec3 uvec3(uvec3 xyz) { return default; }
        public static uvec3 uvec3(bvec3 xyz) { return default; }
    }

    public class bvec3
    {
        public bool this[int index] { get { return default; } set { } }
        public bool this[uint index] { get { return default; } set { } }

        public bool x, y, z;
        public bvec2 xx, xy, xz, yx, yy, yz, zx, zy, zz;
        public bvec3 xxx, xxy, xxz, xyx, xyy, xyz, xzx, xzy, xzz, yxx, yxy, yxz, yyx, yyy, yyz, yzx, yzy, yzz, zxx, zxy, zxz, zyx, zyy, zyz, zzx, zzy, zzz;
        public bvec4 xxxx, xxxy, xxxz, xxyx, xxyy, xxyz, xxzx, xxzy, xxzz, xyxx, xyxy, xyxz, xyyx, xyyy, xyyz, xyzx, xyzy, xyzz, xzxx, xzxy, xzxz, xzyx, xzyy, xzyz, xzzx, xzzy, xzzz, yxxx, yxxy, yxxz, yxyx, yxyy, yxyz, yxzx, yxzy, yxzz, yyxx, yyxy, yyxz, yyyx, yyyy, yyyz, yyzx, yyzy, yyzz, yzxx, yzxy, yzxz, yzyx, yzyy, yzyz, yzzx, yzzy, yzzz, zxxx, zxxy, zxxz, zxyx, zxyy, zxyz, zxzx, zxzy, zxzz, zyxx, zyxy, zyxz, zyyx, zyyy, zyyz, zyzx, zyzy, zyzz, zzxx, zzxy, zzxz, zzyx, zzyy, zzyz, zzzx, zzzy, zzzz;

        public bool r, g, b;
        public bvec2 rr, rg, rb, gr, gg, gb, br, bg, bb;
        public bvec3 rrr, rrg, rrb, rgr, rgg, rgb, rbr, rbg, rbb, grr, grg, grb, ggr, ggg, ggb, gbr, gbg, gbb, brr, brg, brb, bgr, bgg, bgb, bbr, bbg, bbb;
        public bvec4 rrrr, rrrg, rrrb, rrgr, rrgg, rrgb, rrbr, rrbg, rrbb, rgrr, rgrg, rgrb, rggr, rggg, rggb, rgbr, rgbg, rgbb, rbrr, rbrg, rbrb, rbgr, rbgg, rbgb, rbbr, rbbg, rbbb, grrr, grrg, grrb, grgr, grgg, grgb, grbr, grbg, grbb, ggrr, ggrg, ggrb, gggr, gggg, gggb, ggbr, ggbg, ggbb, gbrr, gbrg, gbrb, gbgr, gbgg, gbgb, gbbr, gbbg, gbbb, brrr, brrg, brrb, brgr, brgg, brgb, brbr, brbg, brbb, bgrr, bgrg, bgrb, bggr, bggg, bggb, bgbr, bgbg, bgbb, bbrr, bbrg, bbrb, bbgr, bbgg, bbgb, bbbr, bbbg, bbbb;
    }

    public static partial class Environment
    {
        public static bvec3 bvec3(bool xyz) { return default; }
        public static bvec3 bvec3(bool x, bool y, bool z) { return default; }
        public static bvec3 bvec3(bool x, bvec2 yz) { return default; }
        public static bvec3 bvec3(bvec2 xy, bool z) { return default; }

        public static bvec3 bvec3(vec3 xyz) { return default; }
        public static bvec3 bvec3(ivec3 xyz) { return default; }
        public static bvec3 bvec3(uvec3 xyz) { return default; }
        public static bvec3 bvec3(bvec3 xyz) { return default; }
    }

    public class dvec3
    {
        public double this[int index] { get { return default; } set { } }
        public double this[uint index] { get { return default; } set { } }

        public double x, y, z;
        public vec2 xx, xy, xz, yx, yy, yz, zx, zy, zz;
        public dvec3 xxx, xxy, xxz, xyx, xyy, xyz, xzx, xzy, xzz, yxx, yxy, yxz, yyx, yyy, yyz, yzx, yzy, yzz, zxx, zxy, zxz, zyx, zyy, zyz, zzx, zzy, zzz;
        public vec4 xxxx, xxxy, xxxz, xxyx, xxyy, xxyz, xxzx, xxzy, xxzz, xyxx, xyxy, xyxz, xyyx, xyyy, xyyz, xyzx, xyzy, xyzz, xzxx, xzxy, xzxz, xzyx, xzyy, xzyz, xzzx, xzzy, xzzz, yxxx, yxxy, yxxz, yxyx, yxyy, yxyz, yxzx, yxzy, yxzz, yyxx, yyxy, yyxz, yyyx, yyyy, yyyz, yyzx, yyzy, yyzz, yzxx, yzxy, yzxz, yzyx, yzyy, yzyz, yzzx, yzzy, yzzz, zxxx, zxxy, zxxz, zxyx, zxyy, zxyz, zxzx, zxzy, zxzz, zyxx, zyxy, zyxz, zyyx, zyyy, zyyz, zyzx, zyzy, zyzz, zzxx, zzxy, zzxz, zzyx, zzyy, zzyz, zzzx, zzzy, zzzz;

        public double r, g, b;
        public vec2 rr, rg, rb, gr, gg, gb, br, bg, bb;
        public dvec3 rrr, rrg, rrb, rgr, rgg, rgb, rbr, rbg, rbb, grr, grg, grb, ggr, ggg, ggb, gbr, gbg, gbb, brr, brg, brb, bgr, bgg, bgb, bbr, bbg, bbb;
        public vec4 rrrr, rrrg, rrrb, rrgr, rrgg, rrgb, rrbr, rrbg, rrbb, rgrr, rgrg, rgrb, rggr, rggg, rggb, rgbr, rgbg, rgbb, rbrr, rbrg, rbrb, rbgr, rbgg, rbgb, rbbr, rbbg, rbbb, grrr, grrg, grrb, grgr, grgg, grgb, grbr, grbg, grbb, ggrr, ggrg, ggrb, gggr, gggg, gggb, ggbr, ggbg, ggbb, gbrr, gbrg, gbrb, gbgr, gbgg, gbgb, gbbr, gbbg, gbbb, brrr, brrg, brrb, brgr, brgg, brgb, brbr, brbg, brbb, bgrr, bgrg, bgrb, bggr, bggg, bggb, bgbr, bgbg, bgbb, bbrr, bbrg, bbrb, bbgr, bbgg, bbgb, bbbr, bbbg, bbbb;

        public static dvec3 operator +(dvec3 v1, dvec3 v2) { return default; }
        public static dvec3 operator +(dvec3 v1, double v2) { return default; }
        public static dvec3 operator +(double v1, dvec3 v2) { return default; }
        public static dvec3 operator -(dvec3 v) { return default; }
        public static dvec3 operator -(dvec3 v1, dvec3 v2) { return default; }
        public static dvec3 operator -(double v1, dvec3 v2) { return default; }
        public static dvec3 operator -(dvec3 v1, double v2) { return default; }
        public static dvec3 operator *(dvec3 v1, dvec3 v2) { return default; }
        public static dvec3 operator *(double v1, dvec3 v2) { return default; }
        public static dvec3 operator *(dvec3 v1, double v2) { return default; }
        public static dvec3 operator /(dvec3 v1, dvec3 v2) { return default; }
        public static dvec3 operator /(double v1, dvec3 v2) { return default; }
        public static dvec3 operator /(dvec3 v1, double v2) { return default; }
    }

    public static partial class Environment
    {
        public static dvec3 dvec3(double xyz) { return default; }
        public static dvec3 dvec3(double x, double y, double z) { return default; }
        public static dvec3 dvec3(double x, vec2 yz) { return default; }
        public static dvec3 dvec3(vec2 xy, double z) { return default; }

        public static dvec3 dvec3(vec3 xyz) { return default; }
        public static dvec3 dvec3(dvec3 xyz) { return default; }
        public static dvec3 dvec3(ivec3 xyz) { return default; }
        public static dvec3 dvec3(uvec3 xyz) { return default; }
        public static dvec3 dvec3(bvec3 xyz) { return default; }
    }
}
