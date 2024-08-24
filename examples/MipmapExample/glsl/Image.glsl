#define pow3(a, b) vec3(pow((a).x, b), pow((a).y, b), pow((a).z, b))

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    // top: discrete mipmap levels
    // bottom: continuous mipmap level (when MipmapFilter is set to Linear)

    const float segments = 8.0;
    const float borderThickness = 2.0;

    vec2 uv = fragCoord / iResolution.xy;

    float level = uv.x * segments;

    if (uv.y > 0.5)
    {
        level = floor(level); // discrete levels
    }

    // sample mipmap
    vec3 col = textureLod(iChannel0, uv, level).rgb;

    // add borders
    col = 1.0 - (1.0 - col) * step(floor(2.0 * (fragCoord.y + borderThickness) / iResolution.y), floor(2.0 * fragCoord.y / iResolution.y));
    col = 1.0 - (1.0 - col) * step(step(0.5, uv.y) * floor(segments * (fragCoord.x + borderThickness) / iResolution.x), floor(segments * fragCoord.x / iResolution.x));

    col = pow3(col, 1.0 / 2.2); // convert to srgb

    fragColor = vec4(col, 1.0);
}
