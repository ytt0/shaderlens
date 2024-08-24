const float GridVisibleScale = 6.0;
const float NormalsVisibleScale = 3.0;

const float MaskContrastThreshold = 0.5;

const float ArrowThickness = 1.0;
const float ArrowSize = 0.9;

float grayscale(vec3 color)
{
    return dot(color, vec3(0.299, 0.587, 0.114));
}

vec3 mixColorMask(vec3 color, float mask)
{
    return mask > 0.01 ? mix(color, vec3(step(grayscale(color), MaskContrastThreshold)), min(1.0, mask)) : color;
}

float sdLine(vec2 pos, vec2 end)
{
    float h = clamp(dot(pos, end) / dot(end, end), 0.0, 1.0);
    return length(pos - end * h);
}

float sdArrow(vec2 pos, vec2 start, vec2 end)
{
    vec2 axis = end - start;

    vec2 head1 = 0.2 * vec2(-axis.y, axis.x) - 0.3 * axis;
    vec2 head2 = 0.2 * vec2(axis.y, -axis.x) - 0.3 * axis;

    float d = sdLine(pos - start, end - start);
    d = min(d, sdLine(pos - end, head1));
    d = min(d, sdLine(pos - end, head2));

    return d;
}

float sdNormalArrow(vec2 pos, float angle)
{
    vec2 end = vec2(cos(angle), sin(angle)) * ArrowSize;
    vec2 start = -end;

    return sdArrow(pos, start, end);
}

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    vec2 position = (fragCoord.xy - iViewerOffset) / iViewerScale;

    if (position.x < 0 || position.x > iChannelResolution[0].x || position.y < 0 || position.y > iChannelResolution[0].y)
    {
        fragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    vec4 value = texelFetch(iChannel0, ivec2(floor(position)), 0);

    float gridMask = 0.0;
    float normalMask = 0.0;

    if (iViewerScale > GridVisibleScale)
    {
        vec2 diff = floor((fragCoord.xy - iViewerOffset + vec2(1.0)) / iViewerScale) - floor(position);

        gridMask = min(1.0, diff.x + diff.y);
        gridMask *= smoothstep(GridVisibleScale, 10.0 * GridVisibleScale, iViewerScale) * 0.5;
    }

    if (iViewerScale > NormalsVisibleScale)
    {
        vec2 cellPosition = (fract(position) - 0.5) * 2.0;

        // unpack a custom angle from the buffer data
        float angle = atan((value.x - 0.7) / 0.3, (value.y - 0.7) / 0.3);

        float d = sdNormalArrow(cellPosition, angle) * iViewerScale / ArrowThickness;

        normalMask = smoothstep(2.0, 0.0, d);
        normalMask *= smoothstep(NormalsVisibleScale, 10.0 * NormalsVisibleScale, iViewerScale);
    }

    fragColor = vec4(mixColorMask(clamp(value.rgb, 0.0, 1.0), gridMask + normalMask), 1.0);
}
