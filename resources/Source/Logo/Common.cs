namespace Logo;

class Common
{
    public static readonly float_ PI = 3.1415926535;
    public static readonly float_ TAU = 6.2831853071;

    //@define
    public vec3 pow3(vec3 a, float_ b) => vec3(pow((a).x, b), pow((a).y, b), pow((a).z, b));

    public vec4 premultiplyAlpha(vec4 color)
    {
        return vec4(color.rgb * color.a, color.a);
    }

    public vec4 blendSourceOver(vec4 target, vec4 source)
    {
        return source + (1.0 - source.a) * target;
    }
}
