void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    vec2 position = (fragCoord.xy - iViewerOffset) / iViewerScale;

    if (position.x < 0 || position.x > iChannelResolution[0].x || position.y < 0 || position.y > iChannelResolution[0].y)
    {
        fragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    fragColor = texelFetch(iChannel0, ivec2(floor(position)), 0);
    fragColor.a = 1.0;
}
