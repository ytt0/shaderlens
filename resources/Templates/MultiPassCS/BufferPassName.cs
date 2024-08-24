namespace ProjectName;

class BufferPassName : Common
{
    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        // Normalized pixel coordinates (from 0 to 1)
        vec2 uv = fragCoord / iResolution.xy;

        // Time varying pixel color
        float hue = 0.5 * sin(0.2 * (iTime + uv.x)) + 0.5 * sin(0.2 * (iTime + uv.y) + 1.0);
        vec3 col = getColor(hue);

        // Output to buffer
        fragColor = vec4(col, 1.0);
    }
}
