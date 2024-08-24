//@uniform-group: image
//@uniform, min: 0.0, max: 7.0
uniform float textureScale = 0.0;

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    // RG components are used as a normal direction, and visible when scaled in, with the "Normals Overlay" viewer
    // Alpha component is visible with the "Opacity Background" viewer

    vec2 uv = fragCoord / iResolution.xy;

    vec2 p = uv * 2.0 - 1.0;
    p *= iResolution.xy / min(iResolution.x, iResolution.y);

    float alpha = smoothstep(0.85, 0.8, length(p));

    p *= exp(textureScale);

    float hue = 0.5 * sin(0.2 * (iTime + p.x)) + 0.5 * sin(0.2 * (iTime + p.y) + 1.0);
    vec3 col = vec3(sin(hue * 6.28), sin((hue + 0.33) * 6.28), sin((hue + 0.66) * 6.28)) * 0.3 + 0.7;

    fragColor = vec4(col, alpha);
}
