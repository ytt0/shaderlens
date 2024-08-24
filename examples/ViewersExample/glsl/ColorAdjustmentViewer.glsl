//@uniform-group: color-adjustment-viewer
//@uniform, min: -1.0, max: 1.0, step: 0.01
uniform float imageBrightness = 0.0;
//@uniform, min: -1.0, max: 1.0, step: 0.01
uniform float imageContrast = 0.0;
//@uniform, min: -1.0, max: 1.0, step: 0.01
uniform float imageGamma = 0.0;

//@uniform-group: color-adjustment-channels, parent: color-adjustment-viewer
//@uniform, min: -1.0, max: 1.0, step: 0.01
uniform vec3 channelBrightness = vec3(0.0);
//@uniform, min: -1.0, max: 1.0, step: 0.01
uniform vec3 channelContrast = vec3(0.0);
//@uniform, min: -1.0, max: 1.0, step: 0.01
uniform vec3 channelGamma = vec3(0.0);

float adjustChannel(float channel, float brightness, float contrast, float gamma)
{
    return pow(exp(4.0 * contrast) * (channel - 0.5) + 0.5, exp(4.0 * gamma)) + brightness;
}

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    vec2 position = (fragCoord.xy - iViewerOffset) / iViewerScale;

    if (position.x < 0 || position.x > iChannelResolution[0].x || position.y < 0 || position.y > iChannelResolution[0].y)
    {
        fragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    fragColor = texelFetch(iChannel0, ivec2(floor(position)), 0);

    vec3 brightness = imageBrightness + channelBrightness;
    vec3 contrast = imageContrast + channelContrast;
    vec3 gamma = imageGamma + channelGamma;

    fragColor.rgb = vec3(
        adjustChannel(fragColor.r, brightness.r, contrast.r, gamma.r),
        adjustChannel(fragColor.g, brightness.g, contrast.g, gamma.g),
        adjustChannel(fragColor.b, brightness.b, contrast.b, gamma.b));
}
