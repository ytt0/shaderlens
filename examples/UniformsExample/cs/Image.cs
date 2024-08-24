namespace UniformsExample;

class Image
{
    //@define
    readonly float_ PI = 3.1415926535;
    //@define
    readonly float_ TAU = 6.2831853071;

    //@uniform
    readonly float_ scale = 0.0;

    //@uniform, type: linear-rgb
    readonly vec4 backgroundColor = vec4(1.0);

    //@uniform, min: 0, max: 1000
    readonly int count = 100;

    //@uniform-group: color

    //@uniform
    readonly float_ hueFactor = 0.5;

    //@uniform
    readonly float_ hueBase = 0.7;

    //@uniform
    readonly float_ hueShift = 0.0;

    //@uniform-group: scatter

    //@uniform, min: 0.0
    readonly float_ distanceFactor = 1.0;

    //@uniform, min: -10.0, max: 10.0
    readonly float_ distanceGain = 0.5;

    //@uniform
    readonly float_ angleFactor = 2.4;

    //@uniform-group: shape

    //@uniform, min: 0.0, max: 1.0
    readonly float_ radius = 0.05;

    //@uniform, min: -10.0, max: 10.0
    readonly float_ edge = 2.0;

    //@define
    vec3 pow3(vec3 a, float_ b) => vec3(pow((a).x, b), pow((a).y, b), pow((a).z, b));

    vec3 getColor(float_ hue)
    {
        return clamp(vec3(sin(hue * TAU), sin((hue + 0.33) * TAU), sin((hue + 0.66) * TAU)) * hueFactor + hueBase, 0.0, 1.0);
    }

    vec3 toLinearRgb(vec3 c)
    {
        return pow3(c, 2.2);
    }

    vec3 toSrgb(vec3 c)
    {
        return pow3(c, 1.0 / 2.2);
    }

    vec4 blendSourceOver(vec4 target, vec4 source) { return source + (1.0 - source.a) * target; }

    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        vec2 pos = (fragCoord / iResolution.xy) * 2.0 - 1.0;
        pos *= iResolution.xy / min(iResolution.x, iResolution.y);

        pos *= 1.1 + radius;
        pos /= exp(scale);

        vec4 col = vec4(0.0);

        for (float_ i = 1.0; i <= count; i += 1.0)
        {
            float_ an = i * angleFactor;

            vec2 pos1 = distanceFactor * pow(i / float_(count), distanceGain) * vec2(cos(an), sin(an));
            float_ dist = max(0.0, 1.0 - pow(length(pos - pos1) / radius, exp(edge)));

            vec4 col1 = vec4(toLinearRgb(getColor(fract(an / TAU + i * hueShift / 100.0))), 1.0) * dist;

            col = blendSourceOver(col, col1);
        }

        col = blendSourceOver(backgroundColor, col);

        fragColor = vec4(toSrgb(col.rgb), col.a);
    }
}
