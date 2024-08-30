namespace ViewersExample;

class ValuesRangeViewer
{
    //@uniform-group: values-range-viewer
    //@uniform
    readonly bvec4 channelIncluded = bvec4(true);
    //@uniform
    readonly vec4 channelMin = vec4(0.0);
    //@uniform
    readonly vec4 channelMax = vec4(1.0);

    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        vec2 position = (fragCoord.xy - iViewerOffset) / iViewerScale;

        if (position.x < 0 || position.x > iChannelResolution[0].x || position.y < 0 || position.y > iChannelResolution[0].y)
        {
            fragColor = vec4(0.0, 0.0, 0.0, 1.0);
            return;
        }

        vec4 source = texelFetch(iChannel0, ivec2(floor(position)), 0);
        vec3 target = vec3(0.0);

        int includedCount = 0;
        int targetChannel = 0;

        for (int i = 0; i < 4 && targetChannel < 3; i++)
        {
            float_ a = channelMin[i];
            float_ b = channelMax[i];

            if (channelIncluded[i])
            {
                if (abs(a - b) > 0.0001)
                {
                    target[targetChannel] = clamp((source[i] - a) / (b - a), 0.0, 1.0);
                }

                targetChannel++;
                includedCount++;
            }
            else if (!channelIncluded.a)
            {
                targetChannel++;
            }
        }

        fragColor = vec4(includedCount == 1 ? vec3(target.r + target.g + target.b) : target, 1.0);
    }
}
