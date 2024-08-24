namespace Logo;

class OpacityViewer : Common
{
    //@uniform, min: 0.0, max: 1.0, step: 0.01
    readonly float_ opacityBackground = 0.2;

    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        vec2 position = (fragCoord.xy - iViewerOffset) / iViewerScale;

        if (position.x < 0 || position.x > iChannelResolution[0].x || position.y < 0 || position.y > iChannelResolution[0].y)
        {
            fragColor = vec4(0.0, 0.0, 0.0, 1.0);
            return;
        }

        fragColor = texelFetch(iChannel0, ivec2(floor(position)), 0);

        float grid = mod(floor(fragCoord.x / 15.0) + floor(fragCoord.y / 15.0), 2.0);
        vec4 gridColor = vec4(vec3(clamp(opacityBackground * (1.0 + grid * 0.1), 0.0, 1.0)), 1.0);

        fragColor = blendSourceOver(gridColor, premultiplyAlpha(fragColor));
    }
}
