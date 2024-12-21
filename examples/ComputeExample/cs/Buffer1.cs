namespace ComputeExample;

class Buffer1 : Common
{
    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        vec2 uv = (fragCoord / iResolution.xy) * 2.0 - 1.0;
        uv *= iResolution.xy / min(iResolution.x, iResolution.y); // aspect ratio
        uv *= 4.0; // scale

        vec2 cell = vec2(uv.xy);
        cell += vec2(2.0 * sin(0.2 * iTime), 2.0 * sin(0.2 * iTime + 1.0));
        cell /= 0.5;

        float_ hue = (floor(cell.x) + floor(cell.y)) * 0.1;
        vec3 col = vec3(sin(hue * 6.28), sin((hue + 0.33) * 6.28), sin((hue + 0.66) * 6.28)) * 0.5 + 0.8;

        col *= step(max(abs(fract(cell.x) - 0.5), abs(fract(cell.y) - 0.5)), 0.45);

        col = pow3(col, 2.2); // convert to linear rgb

        fragColor = vec4(col, 1.0);
    }
}
