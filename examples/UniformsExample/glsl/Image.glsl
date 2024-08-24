#define PI 3.1415926535
#define TAU 6.2831853071

uniform float scale = 0.0;

//@uniform, type: linear-rgb
uniform vec4 backgroundColor = vec4(1.0);

//@uniform, min: 0, max: 1000
uniform int count = 100;

//@uniform-group: color

uniform float hueFactor = 0.5;

uniform float hueBase = 0.7;

uniform float hueShift = 0.0;

//@uniform-group: scatter

//@uniform, min: 0.0
uniform float distanceFactor = 1.0;

//@uniform, min: -10.0, max: 10.0
uniform float distanceGain = 0.5;

uniform float angleFactor = 2.4;

//@uniform-group: shape

//@uniform, min: 0.0, max: 1.0
uniform float radius = 0.05;

//@uniform, min: -10.0, max: 10.0
uniform float edge = 2.0;


#define pow3(a, b) vec3(pow((a).x, b), pow((a).y, b), pow((a).z, b));

vec3 getColor(float hue)
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

    for (float i = 1.0; i <= count; i += 1.0)
    {
        float an = i * angleFactor;

        vec2 pos1 = distanceFactor * pow(i / float(count), distanceGain) * vec2(cos(an), sin(an));
        float dist = max(0.0, 1.0 - pow(length(pos - pos1) / radius, exp(edge)));

        vec4 col1 = vec4(toLinearRgb(getColor(fract(an / TAU + i * hueShift / 100.0))), 1.0) * dist;

        col = blendSourceOver(col, col1);
    }

    col = blendSourceOver(backgroundColor, col);

    fragColor = vec4(toSrgb(col.rgb), col.a);
}
