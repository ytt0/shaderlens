namespace Shaderlens
{
    public class vec4
    {
        public Float this[int index] { get { return default; } set { } }
        public Float this[uint index] { get { return default; } set { } }

        public Float x, y, z, w;
        public vec2 xx, xy, xz, xw, yx, yy, yz, yw, zx, zy, zz, zw, wx, wy, wz, ww;
        public vec3 xxx, xxy, xxz, xxw, xyx, xyy, xyz, xyw, xzx, xzy, xzz, xzw, xwx, xwy, xwz, xww, yxx, yxy, yxz, yxw, yyx, yyy, yyz, yyw, yzx, yzy, yzz, yzw, ywx, ywy, ywz, yww, zxx, zxy, zxz, zxw, zyx, zyy, zyz, zyw, zzx, zzy, zzz, zzw, zwx, zwy, zwz, zww, wxx, wxy, wxz, wxw, wyx, wyy, wyz, wyw, wzx, wzy, wzz, wzw, wwx, wwy, wwz, www;
        public vec4 xxxx, xxxy, xxxz, xxxw, xxyx, xxyy, xxyz, xxyw, xxzx, xxzy, xxzz, xxzw, xxwx, xxwy, xxwz, xxww, xyxx, xyxy, xyxz, xyxw, xyyx, xyyy, xyyz, xyyw, xyzx, xyzy, xyzz, xyzw, xywx, xywy, xywz, xyww, xzxx, xzxy, xzxz, xzxw, xzyx, xzyy, xzyz, xzyw, xzzx, xzzy, xzzz, xzzw, xzwx, xzwy, xzwz, xzww, xwxx, xwxy, xwxz, xwxw, xwyx, xwyy, xwyz, xwyw, xwzx, xwzy, xwzz, xwzw, xwwx, xwwy, xwwz, xwww, yxxx, yxxy, yxxz, yxxw, yxyx, yxyy, yxyz, yxyw, yxzx, yxzy, yxzz, yxzw, yxwx, yxwy, yxwz, yxww, yyxx, yyxy, yyxz, yyxw, yyyx, yyyy, yyyz, yyyw, yyzx, yyzy, yyzz, yyzw, yywx, yywy, yywz, yyww, yzxx, yzxy, yzxz, yzxw, yzyx, yzyy, yzyz, yzyw, yzzx, yzzy, yzzz, yzzw, yzwx, yzwy, yzwz, yzww, ywxx, ywxy, ywxz, ywxw, ywyx, ywyy, ywyz, ywyw, ywzx, ywzy, ywzz, ywzw, ywwx, ywwy, ywwz, ywww, zxxx, zxxy, zxxz, zxxw, zxyx, zxyy, zxyz, zxyw, zxzx, zxzy, zxzz, zxzw, zxwx, zxwy, zxwz, zxww, zyxx, zyxy, zyxz, zyxw, zyyx, zyyy, zyyz, zyyw, zyzx, zyzy, zyzz, zyzw, zywx, zywy, zywz, zyww, zzxx, zzxy, zzxz, zzxw, zzyx, zzyy, zzyz, zzyw, zzzx, zzzy, zzzz, zzzw, zzwx, zzwy, zzwz, zzww, zwxx, zwxy, zwxz, zwxw, zwyx, zwyy, zwyz, zwyw, zwzx, zwzy, zwzz, zwzw, zwwx, zwwy, zwwz, zwww, wxxx, wxxy, wxxz, wxxw, wxyx, wxyy, wxyz, wxyw, wxzx, wxzy, wxzz, wxzw, wxwx, wxwy, wxwz, wxww, wyxx, wyxy, wyxz, wyxw, wyyx, wyyy, wyyz, wyyw, wyzx, wyzy, wyzz, wyzw, wywx, wywy, wywz, wyww, wzxx, wzxy, wzxz, wzxw, wzyx, wzyy, wzyz, wzyw, wzzx, wzzy, wzzz, wzzw, wzwx, wzwy, wzwz, wzww, wwxx, wwxy, wwxz, wwxw, wwyx, wwyy, wwyz, wwyw, wwzx, wwzy, wwzz, wwzw, wwwx, wwwy, wwwz, wwww;

        public Float r, g, b, a;
        public vec2 rr, rg, rb, ra, gr, gg, gb, ga, br, bg, bb, ba, ar, ag, ab, aa;
        public vec3 rrr, rrg, rrb, rra, rgr, rgg, rgb, rga, rbr, rbg, rbb, rba, rar, rag, rab, raa, grr, grg, grb, gra, ggr, ggg, ggb, gga, gbr, gbg, gbb, gba, gar, gag, gab, gaa, brr, brg, brb, bra, bgr, bgg, bgb, bga, bbr, bbg, bbb, bba, bar, bag, bab, baa, arr, arg, arb, ara, agr, agg, agb, aga, abr, abg, abb, aba, aar, aag, aab, aaa;
        public vec4 rrrr, rrrg, rrrb, rrra, rrgr, rrgg, rrgb, rrga, rrbr, rrbg, rrbb, rrba, rrar, rrag, rrab, rraa, rgrr, rgrg, rgrb, rgra, rggr, rggg, rggb, rgga, rgbr, rgbg, rgbb, rgba, rgar, rgag, rgab, rgaa, rbrr, rbrg, rbrb, rbra, rbgr, rbgg, rbgb, rbga, rbbr, rbbg, rbbb, rbba, rbar, rbag, rbab, rbaa, rarr, rarg, rarb, rara, ragr, ragg, ragb, raga, rabr, rabg, rabb, raba, raar, raag, raab, raaa, grrr, grrg, grrb, grra, grgr, grgg, grgb, grga, grbr, grbg, grbb, grba, grar, grag, grab, graa, ggrr, ggrg, ggrb, ggra, gggr, gggg, gggb, ggga, ggbr, ggbg, ggbb, ggba, ggar, ggag, ggab, ggaa, gbrr, gbrg, gbrb, gbra, gbgr, gbgg, gbgb, gbga, gbbr, gbbg, gbbb, gbba, gbar, gbag, gbab, gbaa, garr, garg, garb, gara, gagr, gagg, gagb, gaga, gabr, gabg, gabb, gaba, gaar, gaag, gaab, gaaa, brrr, brrg, brrb, brra, brgr, brgg, brgb, brga, brbr, brbg, brbb, brba, brar, brag, brab, braa, bgrr, bgrg, bgrb, bgra, bggr, bggg, bggb, bgga, bgbr, bgbg, bgbb, bgba, bgar, bgag, bgab, bgaa, bbrr, bbrg, bbrb, bbra, bbgr, bbgg, bbgb, bbga, bbbr, bbbg, bbbb, bbba, bbar, bbag, bbab, bbaa, barr, barg, barb, bara, bagr, bagg, bagb, baga, babr, babg, babb, baba, baar, baag, baab, baaa, arrr, arrg, arrb, arra, argr, argg, argb, arga, arbr, arbg, arbb, arba, arar, arag, arab, araa, agrr, agrg, agrb, agra, aggr, aggg, aggb, agga, agbr, agbg, agbb, agba, agar, agag, agab, agaa, abrr, abrg, abrb, abra, abgr, abgg, abgb, abga, abbr, abbg, abbb, abba, abar, abag, abab, abaa, aarr, aarg, aarb, aara, aagr, aagg, aagb, aaga, aabr, aabg, aabb, aaba, aaar, aaag, aaab, aaaa;

        public static vec4 operator +(vec4 v1, vec4 v2) { return default; }
        public static vec4 operator +(vec4 v1, Float v2) { return default; }
        public static vec4 operator +(Float v1, vec4 v2) { return default; }
        public static vec4 operator -(vec4 v) { return default; }
        public static vec4 operator -(vec4 v1, vec4 v2) { return default; }
        public static vec4 operator -(vec4 v1, Float v2) { return default; }
        public static vec4 operator -(Float v1, vec4 v2) { return default; }
        public static vec4 operator *(vec4 v1, vec4 v2) { return default; }
        public static vec4 operator *(Float v1, vec4 v2) { return default; }
        public static vec4 operator *(vec4 v1, Float v2) { return default; }
        public static vec4 operator /(vec4 v1, vec4 v2) { return default; }
        public static vec4 operator /(Float v1, vec4 v2) { return default; }
        public static vec4 operator /(vec4 v1, Float v2) { return default; }
    }

    public static partial class Environment
    {
        public static vec4 vec4(Float xyzw) { return default; }
        public static vec4 vec4(Float x, Float y, Float z, Float w) { return default; }
        public static vec4 vec4(Float x, Float y, vec2 zw) { return default; }
        public static vec4 vec4(Float x, vec2 yz, Float w) { return default; }
        public static vec4 vec4(Float x, vec3 yzw) { return default; }
        public static vec4 vec4(vec2 xy, Float z, Float w) { return default; }
        public static vec4 vec4(vec2 xy, vec2 zw) { return default; }
        public static vec4 vec4(vec3 xyz, Float w) { return default; }

        public static vec4 vec4(vec4 xyzw) { return default; }
        public static vec4 vec4(ivec4 xyzw) { return default; }
        public static vec4 vec4(uvec4 xyzw) { return default; }
        public static vec4 vec4(bvec4 xyzw) { return default; }
    }

    public class ivec4
    {
        public int this[int index] { get { return default; } set { } }
        public int this[uint index] { get { return default; } set { } }

        public int x, y, z, w;
        public ivec2 xx, xy, xz, xw, yx, yy, yz, yw, zx, zy, zz, zw, wx, wy, wz, ww;
        public ivec3 xxx, xxy, xxz, xxw, xyx, xyy, xyz, xyw, xzx, xzy, xzz, xzw, xwx, xwy, xwz, xww, yxx, yxy, yxz, yxw, yyx, yyy, yyz, yyw, yzx, yzy, yzz, yzw, ywx, ywy, ywz, yww, zxx, zxy, zxz, zxw, zyx, zyy, zyz, zyw, zzx, zzy, zzz, zzw, zwx, zwy, zwz, zww, wxx, wxy, wxz, wxw, wyx, wyy, wyz, wyw, wzx, wzy, wzz, wzw, wwx, wwy, wwz, www;
        public ivec4 xxxx, xxxy, xxxz, xxxw, xxyx, xxyy, xxyz, xxyw, xxzx, xxzy, xxzz, xxzw, xxwx, xxwy, xxwz, xxww, xyxx, xyxy, xyxz, xyxw, xyyx, xyyy, xyyz, xyyw, xyzx, xyzy, xyzz, xyzw, xywx, xywy, xywz, xyww, xzxx, xzxy, xzxz, xzxw, xzyx, xzyy, xzyz, xzyw, xzzx, xzzy, xzzz, xzzw, xzwx, xzwy, xzwz, xzww, xwxx, xwxy, xwxz, xwxw, xwyx, xwyy, xwyz, xwyw, xwzx, xwzy, xwzz, xwzw, xwwx, xwwy, xwwz, xwww, yxxx, yxxy, yxxz, yxxw, yxyx, yxyy, yxyz, yxyw, yxzx, yxzy, yxzz, yxzw, yxwx, yxwy, yxwz, yxww, yyxx, yyxy, yyxz, yyxw, yyyx, yyyy, yyyz, yyyw, yyzx, yyzy, yyzz, yyzw, yywx, yywy, yywz, yyww, yzxx, yzxy, yzxz, yzxw, yzyx, yzyy, yzyz, yzyw, yzzx, yzzy, yzzz, yzzw, yzwx, yzwy, yzwz, yzww, ywxx, ywxy, ywxz, ywxw, ywyx, ywyy, ywyz, ywyw, ywzx, ywzy, ywzz, ywzw, ywwx, ywwy, ywwz, ywww, zxxx, zxxy, zxxz, zxxw, zxyx, zxyy, zxyz, zxyw, zxzx, zxzy, zxzz, zxzw, zxwx, zxwy, zxwz, zxww, zyxx, zyxy, zyxz, zyxw, zyyx, zyyy, zyyz, zyyw, zyzx, zyzy, zyzz, zyzw, zywx, zywy, zywz, zyww, zzxx, zzxy, zzxz, zzxw, zzyx, zzyy, zzyz, zzyw, zzzx, zzzy, zzzz, zzzw, zzwx, zzwy, zzwz, zzww, zwxx, zwxy, zwxz, zwxw, zwyx, zwyy, zwyz, zwyw, zwzx, zwzy, zwzz, zwzw, zwwx, zwwy, zwwz, zwww, wxxx, wxxy, wxxz, wxxw, wxyx, wxyy, wxyz, wxyw, wxzx, wxzy, wxzz, wxzw, wxwx, wxwy, wxwz, wxww, wyxx, wyxy, wyxz, wyxw, wyyx, wyyy, wyyz, wyyw, wyzx, wyzy, wyzz, wyzw, wywx, wywy, wywz, wyww, wzxx, wzxy, wzxz, wzxw, wzyx, wzyy, wzyz, wzyw, wzzx, wzzy, wzzz, wzzw, wzwx, wzwy, wzwz, wzww, wwxx, wwxy, wwxz, wwxw, wwyx, wwyy, wwyz, wwyw, wwzx, wwzy, wwzz, wwzw, wwwx, wwwy, wwwz, wwww;

        public int r, g, b, a;
        public ivec2 rr, rg, rb, ra, gr, gg, gb, ga, br, bg, bb, ba, ar, ag, ab, aa;
        public ivec3 rrr, rrg, rrb, rra, rgr, rgg, rgb, rga, rbr, rbg, rbb, rba, rar, rag, rab, raa, grr, grg, grb, gra, ggr, ggg, ggb, gga, gbr, gbg, gbb, gba, gar, gag, gab, gaa, brr, brg, brb, bra, bgr, bgg, bgb, bga, bbr, bbg, bbb, bba, bar, bag, bab, baa, arr, arg, arb, ara, agr, agg, agb, aga, abr, abg, abb, aba, aar, aag, aab, aaa;
        public ivec4 rrrr, rrrg, rrrb, rrra, rrgr, rrgg, rrgb, rrga, rrbr, rrbg, rrbb, rrba, rrar, rrag, rrab, rraa, rgrr, rgrg, rgrb, rgra, rggr, rggg, rggb, rgga, rgbr, rgbg, rgbb, rgba, rgar, rgag, rgab, rgaa, rbrr, rbrg, rbrb, rbra, rbgr, rbgg, rbgb, rbga, rbbr, rbbg, rbbb, rbba, rbar, rbag, rbab, rbaa, rarr, rarg, rarb, rara, ragr, ragg, ragb, raga, rabr, rabg, rabb, raba, raar, raag, raab, raaa, grrr, grrg, grrb, grra, grgr, grgg, grgb, grga, grbr, grbg, grbb, grba, grar, grag, grab, graa, ggrr, ggrg, ggrb, ggra, gggr, gggg, gggb, ggga, ggbr, ggbg, ggbb, ggba, ggar, ggag, ggab, ggaa, gbrr, gbrg, gbrb, gbra, gbgr, gbgg, gbgb, gbga, gbbr, gbbg, gbbb, gbba, gbar, gbag, gbab, gbaa, garr, garg, garb, gara, gagr, gagg, gagb, gaga, gabr, gabg, gabb, gaba, gaar, gaag, gaab, gaaa, brrr, brrg, brrb, brra, brgr, brgg, brgb, brga, brbr, brbg, brbb, brba, brar, brag, brab, braa, bgrr, bgrg, bgrb, bgra, bggr, bggg, bggb, bgga, bgbr, bgbg, bgbb, bgba, bgar, bgag, bgab, bgaa, bbrr, bbrg, bbrb, bbra, bbgr, bbgg, bbgb, bbga, bbbr, bbbg, bbbb, bbba, bbar, bbag, bbab, bbaa, barr, barg, barb, bara, bagr, bagg, bagb, baga, babr, babg, babb, baba, baar, baag, baab, baaa, arrr, arrg, arrb, arra, argr, argg, argb, arga, arbr, arbg, arbb, arba, arar, arag, arab, araa, agrr, agrg, agrb, agra, aggr, aggg, aggb, agga, agbr, agbg, agbb, agba, agar, agag, agab, agaa, abrr, abrg, abrb, abra, abgr, abgg, abgb, abga, abbr, abbg, abbb, abba, abar, abag, abab, abaa, aarr, aarg, aarb, aara, aagr, aagg, aagb, aaga, aabr, aabg, aabb, aaba, aaar, aaag, aaab, aaaa;

        public static ivec4 operator +(ivec4 v1, ivec4 v2) { return default; }
        public static ivec4 operator +(ivec4 v1, int v2) { return default; }
        public static ivec4 operator +(int v1, ivec4 v2) { return default; }
        public static ivec4 operator -(ivec4 v) { return default; }
        public static ivec4 operator -(ivec4 v1, ivec4 v2) { return default; }
        public static ivec4 operator -(ivec4 v1, int v2) { return default; }
        public static ivec4 operator -(int v1, ivec4 v2) { return default; }
        public static ivec4 operator *(ivec4 v1, ivec4 v2) { return default; }
        public static ivec4 operator *(int v1, ivec4 v2) { return default; }
        public static ivec4 operator *(ivec4 v1, int v2) { return default; }
        public static ivec4 operator /(ivec4 v1, ivec4 v2) { return default; }
        public static ivec4 operator /(int v1, ivec4 v2) { return default; }
        public static ivec4 operator /(ivec4 v1, int v2) { return default; }
    }

    public static partial class Environment
    {
        public static ivec4 ivec4(int xyzw) { return default; }
        public static ivec4 ivec4(int x, int y, int z, int w) { return default; }
        public static ivec4 ivec4(int x, int y, ivec2 zw) { return default; }
        public static ivec4 ivec4(int x, ivec2 yz, int w) { return default; }
        public static ivec4 ivec4(int x, ivec3 yzw) { return default; }
        public static ivec4 ivec4(ivec2 xy, int z, int w) { return default; }
        public static ivec4 ivec4(ivec2 xy, ivec2 zw) { return default; }
        public static ivec4 ivec4(ivec3 xyz, int w) { return default; }

        public static ivec4 ivec4(vec4 xyzw) { return default; }
        public static ivec4 ivec4(ivec4 xyzw) { return default; }
        public static ivec4 ivec4(uvec4 xyzw) { return default; }
        public static ivec4 ivec4(bvec4 xyzw) { return default; }
    }

    public class uvec4
    {
        public uint this[int index] { get { return default; } set { } }
        public uint this[uint index] { get { return default; } set { } }

        public uint x, y, z, w;
        public uvec2 xx, xy, xz, xw, yx, yy, yz, yw, zx, zy, zz, zw, wx, wy, wz, ww;
        public uvec3 xxx, xxy, xxz, xxw, xyx, xyy, xyz, xyw, xzx, xzy, xzz, xzw, xwx, xwy, xwz, xww, yxx, yxy, yxz, yxw, yyx, yyy, yyz, yyw, yzx, yzy, yzz, yzw, ywx, ywy, ywz, yww, zxx, zxy, zxz, zxw, zyx, zyy, zyz, zyw, zzx, zzy, zzz, zzw, zwx, zwy, zwz, zww, wxx, wxy, wxz, wxw, wyx, wyy, wyz, wyw, wzx, wzy, wzz, wzw, wwx, wwy, wwz, www;
        public uvec4 xxxx, xxxy, xxxz, xxxw, xxyx, xxyy, xxyz, xxyw, xxzx, xxzy, xxzz, xxzw, xxwx, xxwy, xxwz, xxww, xyxx, xyxy, xyxz, xyxw, xyyx, xyyy, xyyz, xyyw, xyzx, xyzy, xyzz, xyzw, xywx, xywy, xywz, xyww, xzxx, xzxy, xzxz, xzxw, xzyx, xzyy, xzyz, xzyw, xzzx, xzzy, xzzz, xzzw, xzwx, xzwy, xzwz, xzww, xwxx, xwxy, xwxz, xwxw, xwyx, xwyy, xwyz, xwyw, xwzx, xwzy, xwzz, xwzw, xwwx, xwwy, xwwz, xwww, yxxx, yxxy, yxxz, yxxw, yxyx, yxyy, yxyz, yxyw, yxzx, yxzy, yxzz, yxzw, yxwx, yxwy, yxwz, yxww, yyxx, yyxy, yyxz, yyxw, yyyx, yyyy, yyyz, yyyw, yyzx, yyzy, yyzz, yyzw, yywx, yywy, yywz, yyww, yzxx, yzxy, yzxz, yzxw, yzyx, yzyy, yzyz, yzyw, yzzx, yzzy, yzzz, yzzw, yzwx, yzwy, yzwz, yzww, ywxx, ywxy, ywxz, ywxw, ywyx, ywyy, ywyz, ywyw, ywzx, ywzy, ywzz, ywzw, ywwx, ywwy, ywwz, ywww, zxxx, zxxy, zxxz, zxxw, zxyx, zxyy, zxyz, zxyw, zxzx, zxzy, zxzz, zxzw, zxwx, zxwy, zxwz, zxww, zyxx, zyxy, zyxz, zyxw, zyyx, zyyy, zyyz, zyyw, zyzx, zyzy, zyzz, zyzw, zywx, zywy, zywz, zyww, zzxx, zzxy, zzxz, zzxw, zzyx, zzyy, zzyz, zzyw, zzzx, zzzy, zzzz, zzzw, zzwx, zzwy, zzwz, zzww, zwxx, zwxy, zwxz, zwxw, zwyx, zwyy, zwyz, zwyw, zwzx, zwzy, zwzz, zwzw, zwwx, zwwy, zwwz, zwww, wxxx, wxxy, wxxz, wxxw, wxyx, wxyy, wxyz, wxyw, wxzx, wxzy, wxzz, wxzw, wxwx, wxwy, wxwz, wxww, wyxx, wyxy, wyxz, wyxw, wyyx, wyyy, wyyz, wyyw, wyzx, wyzy, wyzz, wyzw, wywx, wywy, wywz, wyww, wzxx, wzxy, wzxz, wzxw, wzyx, wzyy, wzyz, wzyw, wzzx, wzzy, wzzz, wzzw, wzwx, wzwy, wzwz, wzww, wwxx, wwxy, wwxz, wwxw, wwyx, wwyy, wwyz, wwyw, wwzx, wwzy, wwzz, wwzw, wwwx, wwwy, wwwz, wwww;

        public uint r, g, b, a;
        public uvec2 rr, rg, rb, ra, gr, gg, gb, ga, br, bg, bb, ba, ar, ag, ab, aa;
        public uvec3 rrr, rrg, rrb, rra, rgr, rgg, rgb, rga, rbr, rbg, rbb, rba, rar, rag, rab, raa, grr, grg, grb, gra, ggr, ggg, ggb, gga, gbr, gbg, gbb, gba, gar, gag, gab, gaa, brr, brg, brb, bra, bgr, bgg, bgb, bga, bbr, bbg, bbb, bba, bar, bag, bab, baa, arr, arg, arb, ara, agr, agg, agb, aga, abr, abg, abb, aba, aar, aag, aab, aaa;
        public uvec4 rrrr, rrrg, rrrb, rrra, rrgr, rrgg, rrgb, rrga, rrbr, rrbg, rrbb, rrba, rrar, rrag, rrab, rraa, rgrr, rgrg, rgrb, rgra, rggr, rggg, rggb, rgga, rgbr, rgbg, rgbb, rgba, rgar, rgag, rgab, rgaa, rbrr, rbrg, rbrb, rbra, rbgr, rbgg, rbgb, rbga, rbbr, rbbg, rbbb, rbba, rbar, rbag, rbab, rbaa, rarr, rarg, rarb, rara, ragr, ragg, ragb, raga, rabr, rabg, rabb, raba, raar, raag, raab, raaa, grrr, grrg, grrb, grra, grgr, grgg, grgb, grga, grbr, grbg, grbb, grba, grar, grag, grab, graa, ggrr, ggrg, ggrb, ggra, gggr, gggg, gggb, ggga, ggbr, ggbg, ggbb, ggba, ggar, ggag, ggab, ggaa, gbrr, gbrg, gbrb, gbra, gbgr, gbgg, gbgb, gbga, gbbr, gbbg, gbbb, gbba, gbar, gbag, gbab, gbaa, garr, garg, garb, gara, gagr, gagg, gagb, gaga, gabr, gabg, gabb, gaba, gaar, gaag, gaab, gaaa, brrr, brrg, brrb, brra, brgr, brgg, brgb, brga, brbr, brbg, brbb, brba, brar, brag, brab, braa, bgrr, bgrg, bgrb, bgra, bggr, bggg, bggb, bgga, bgbr, bgbg, bgbb, bgba, bgar, bgag, bgab, bgaa, bbrr, bbrg, bbrb, bbra, bbgr, bbgg, bbgb, bbga, bbbr, bbbg, bbbb, bbba, bbar, bbag, bbab, bbaa, barr, barg, barb, bara, bagr, bagg, bagb, baga, babr, babg, babb, baba, baar, baag, baab, baaa, arrr, arrg, arrb, arra, argr, argg, argb, arga, arbr, arbg, arbb, arba, arar, arag, arab, araa, agrr, agrg, agrb, agra, aggr, aggg, aggb, agga, agbr, agbg, agbb, agba, agar, agag, agab, agaa, abrr, abrg, abrb, abra, abgr, abgg, abgb, abga, abbr, abbg, abbb, abba, abar, abag, abab, abaa, aarr, aarg, aarb, aara, aagr, aagg, aagb, aaga, aabr, aabg, aabb, aaba, aaar, aaag, aaab, aaaa;

        public static uvec4 operator +(uvec4 v1, uvec4 v2) { return default; }
        public static uvec4 operator +(uvec4 v1, uint v2) { return default; }
        public static uvec4 operator +(uint v1, uvec4 v2) { return default; }
        public static uvec4 operator -(uvec4 v) { return default; }
        public static uvec4 operator -(uvec4 v1, uvec4 v2) { return default; }
        public static uvec4 operator -(uvec4 v1, uint v2) { return default; }
        public static uvec4 operator -(uint v1, uvec4 v2) { return default; }
        public static uvec4 operator *(uvec4 v1, uvec4 v2) { return default; }
        public static uvec4 operator *(uint v1, uvec4 v2) { return default; }
        public static uvec4 operator *(uvec4 v1, uint v2) { return default; }
        public static uvec4 operator /(uvec4 v1, uvec4 v2) { return default; }
        public static uvec4 operator /(uint v1, uvec4 v2) { return default; }
        public static uvec4 operator /(uvec4 v1, uint v2) { return default; }
    }

    public static partial class Environment
    {
        public static uvec4 uvec4(uint xyzw) { return default; }
        public static uvec4 uvec4(uint x, uint y, uint z, uint w) { return default; }
        public static uvec4 uvec4(uint x, uint y, uvec2 zw) { return default; }
        public static uvec4 uvec4(uint x, uvec2 yz, uint w) { return default; }
        public static uvec4 uvec4(uint x, uvec3 yzw) { return default; }
        public static uvec4 uvec4(uvec2 xy, uint z, uint w) { return default; }
        public static uvec4 uvec4(uvec2 xy, uvec2 zw) { return default; }
        public static uvec4 uvec4(uvec3 xyz, uint w) { return default; }

        public static uvec4 uvec4(vec4 xyzw) { return default; }
        public static uvec4 uvec4(ivec4 xyzw) { return default; }
        public static uvec4 uvec4(uvec4 xyzw) { return default; }
        public static uvec4 uvec4(bvec4 xyzw) { return default; }
    }

    public class bvec4
    {
        public bool this[int index] { get { return default; } set { } }
        public bool this[uint index] { get { return default; } set { } }

        public bool x, y, z, w;
        public bvec2 xx, xy, xz, xw, yx, yy, yz, yw, zx, zy, zz, zw, wx, wy, wz, ww;
        public bvec3 xxx, xxy, xxz, xxw, xyx, xyy, xyz, xyw, xzx, xzy, xzz, xzw, xwx, xwy, xwz, xww, yxx, yxy, yxz, yxw, yyx, yyy, yyz, yyw, yzx, yzy, yzz, yzw, ywx, ywy, ywz, yww, zxx, zxy, zxz, zxw, zyx, zyy, zyz, zyw, zzx, zzy, zzz, zzw, zwx, zwy, zwz, zww, wxx, wxy, wxz, wxw, wyx, wyy, wyz, wyw, wzx, wzy, wzz, wzw, wwx, wwy, wwz, www;
        public bvec4 xxxx, xxxy, xxxz, xxxw, xxyx, xxyy, xxyz, xxyw, xxzx, xxzy, xxzz, xxzw, xxwx, xxwy, xxwz, xxww, xyxx, xyxy, xyxz, xyxw, xyyx, xyyy, xyyz, xyyw, xyzx, xyzy, xyzz, xyzw, xywx, xywy, xywz, xyww, xzxx, xzxy, xzxz, xzxw, xzyx, xzyy, xzyz, xzyw, xzzx, xzzy, xzzz, xzzw, xzwx, xzwy, xzwz, xzww, xwxx, xwxy, xwxz, xwxw, xwyx, xwyy, xwyz, xwyw, xwzx, xwzy, xwzz, xwzw, xwwx, xwwy, xwwz, xwww, yxxx, yxxy, yxxz, yxxw, yxyx, yxyy, yxyz, yxyw, yxzx, yxzy, yxzz, yxzw, yxwx, yxwy, yxwz, yxww, yyxx, yyxy, yyxz, yyxw, yyyx, yyyy, yyyz, yyyw, yyzx, yyzy, yyzz, yyzw, yywx, yywy, yywz, yyww, yzxx, yzxy, yzxz, yzxw, yzyx, yzyy, yzyz, yzyw, yzzx, yzzy, yzzz, yzzw, yzwx, yzwy, yzwz, yzww, ywxx, ywxy, ywxz, ywxw, ywyx, ywyy, ywyz, ywyw, ywzx, ywzy, ywzz, ywzw, ywwx, ywwy, ywwz, ywww, zxxx, zxxy, zxxz, zxxw, zxyx, zxyy, zxyz, zxyw, zxzx, zxzy, zxzz, zxzw, zxwx, zxwy, zxwz, zxww, zyxx, zyxy, zyxz, zyxw, zyyx, zyyy, zyyz, zyyw, zyzx, zyzy, zyzz, zyzw, zywx, zywy, zywz, zyww, zzxx, zzxy, zzxz, zzxw, zzyx, zzyy, zzyz, zzyw, zzzx, zzzy, zzzz, zzzw, zzwx, zzwy, zzwz, zzww, zwxx, zwxy, zwxz, zwxw, zwyx, zwyy, zwyz, zwyw, zwzx, zwzy, zwzz, zwzw, zwwx, zwwy, zwwz, zwww, wxxx, wxxy, wxxz, wxxw, wxyx, wxyy, wxyz, wxyw, wxzx, wxzy, wxzz, wxzw, wxwx, wxwy, wxwz, wxww, wyxx, wyxy, wyxz, wyxw, wyyx, wyyy, wyyz, wyyw, wyzx, wyzy, wyzz, wyzw, wywx, wywy, wywz, wyww, wzxx, wzxy, wzxz, wzxw, wzyx, wzyy, wzyz, wzyw, wzzx, wzzy, wzzz, wzzw, wzwx, wzwy, wzwz, wzww, wwxx, wwxy, wwxz, wwxw, wwyx, wwyy, wwyz, wwyw, wwzx, wwzy, wwzz, wwzw, wwwx, wwwy, wwwz, wwww;

        public bool r, g, b, a;
        public bvec2 rr, rg, rb, ra, gr, gg, gb, ga, br, bg, bb, ba, ar, ag, ab, aa;
        public bvec3 rrr, rrg, rrb, rra, rgr, rgg, rgb, rga, rbr, rbg, rbb, rba, rar, rag, rab, raa, grr, grg, grb, gra, ggr, ggg, ggb, gga, gbr, gbg, gbb, gba, gar, gag, gab, gaa, brr, brg, brb, bra, bgr, bgg, bgb, bga, bbr, bbg, bbb, bba, bar, bag, bab, baa, arr, arg, arb, ara, agr, agg, agb, aga, abr, abg, abb, aba, aar, aag, aab, aaa;
        public bvec4 rrrr, rrrg, rrrb, rrra, rrgr, rrgg, rrgb, rrga, rrbr, rrbg, rrbb, rrba, rrar, rrag, rrab, rraa, rgrr, rgrg, rgrb, rgra, rggr, rggg, rggb, rgga, rgbr, rgbg, rgbb, rgba, rgar, rgag, rgab, rgaa, rbrr, rbrg, rbrb, rbra, rbgr, rbgg, rbgb, rbga, rbbr, rbbg, rbbb, rbba, rbar, rbag, rbab, rbaa, rarr, rarg, rarb, rara, ragr, ragg, ragb, raga, rabr, rabg, rabb, raba, raar, raag, raab, raaa, grrr, grrg, grrb, grra, grgr, grgg, grgb, grga, grbr, grbg, grbb, grba, grar, grag, grab, graa, ggrr, ggrg, ggrb, ggra, gggr, gggg, gggb, ggga, ggbr, ggbg, ggbb, ggba, ggar, ggag, ggab, ggaa, gbrr, gbrg, gbrb, gbra, gbgr, gbgg, gbgb, gbga, gbbr, gbbg, gbbb, gbba, gbar, gbag, gbab, gbaa, garr, garg, garb, gara, gagr, gagg, gagb, gaga, gabr, gabg, gabb, gaba, gaar, gaag, gaab, gaaa, brrr, brrg, brrb, brra, brgr, brgg, brgb, brga, brbr, brbg, brbb, brba, brar, brag, brab, braa, bgrr, bgrg, bgrb, bgra, bggr, bggg, bggb, bgga, bgbr, bgbg, bgbb, bgba, bgar, bgag, bgab, bgaa, bbrr, bbrg, bbrb, bbra, bbgr, bbgg, bbgb, bbga, bbbr, bbbg, bbbb, bbba, bbar, bbag, bbab, bbaa, barr, barg, barb, bara, bagr, bagg, bagb, baga, babr, babg, babb, baba, baar, baag, baab, baaa, arrr, arrg, arrb, arra, argr, argg, argb, arga, arbr, arbg, arbb, arba, arar, arag, arab, araa, agrr, agrg, agrb, agra, aggr, aggg, aggb, agga, agbr, agbg, agbb, agba, agar, agag, agab, agaa, abrr, abrg, abrb, abra, abgr, abgg, abgb, abga, abbr, abbg, abbb, abba, abar, abag, abab, abaa, aarr, aarg, aarb, aara, aagr, aagg, aagb, aaga, aabr, aabg, aabb, aaba, aaar, aaag, aaab, aaaa;
    }

    public static partial class Environment
    {
        public static bvec4 bvec4(bool xyzw) { return default; }
        public static bvec4 bvec4(bool x, bool y, bool z, bool w) { return default; }
        public static bvec4 bvec4(bool x, bool y, bvec2 zw) { return default; }
        public static bvec4 bvec4(bool x, bvec2 yz, bool w) { return default; }
        public static bvec4 bvec4(bool x, bvec3 yzw) { return default; }
        public static bvec4 bvec4(bvec2 xy, bool z, bool w) { return default; }
        public static bvec4 bvec4(bvec2 xy, bvec2 zw) { return default; }
        public static bvec4 bvec4(bvec3 xyz, bool w) { return default; }

        public static bvec4 bvec4(vec4 xyzw) { return default; }
        public static bvec4 bvec4(ivec4 xyzw) { return default; }
        public static bvec4 bvec4(uvec4 xyzw) { return default; }
        public static bvec4 bvec4(bvec4 xyzw) { return default; }
    }
}
