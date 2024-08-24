namespace Logo;

class Image : Common
{
    //@uniform
    readonly float_ saturationStart = 0.1;
    //@uniform
    readonly float_ saturationIncrease = 0.5;

    //@uniform
    readonly float_ marginPosition = 0.8;

    //@uniform
    readonly bool scalePreviewEnabled = false;

    //@uniform-group: margin

    //@uniform
    readonly bool marginHighlightEnabled = true;

    //@uniform
    readonly float_ marginHighlightOffset = 0.16;
    //@uniform
    readonly float_ marginHighlightThickness = 0.3;
    //@uniform
    readonly float_ marginHighlightValue = 0.0;
    //@uniform
    readonly float_ marginHighlightSaturation = 0.5;

    //@uniform-group: notch

    //@uniform
    readonly bool notchEnabled = true;

    //@uniform, step: 0.001
    readonly float_ notchSkew = 0.0;

    //@uniform-group: notch-thickness, parent: notch

    //@uniform
    readonly float_ notchHighlightThickness = 0.24;
    //@uniform
    readonly float_ notchHighlightOffset = -0.06;
    //@uniform
    readonly float_ notchHighlightValue = 0.07;
    //@uniform
    readonly float_ notchHighlightSaturation = -0.03;

    //@uniform-group: notch-shadow, parent: notch

    //@uniform
    readonly float_ notchShadowThickness = 0.05;
    //@uniform
    readonly float_ notchShadowOffset = 0.0;
    //@uniform
    readonly float_ notchShadowValue = -0.15;
    //@uniform
    readonly float_ notchShadowSaturation = 0.2;

    //@uniform-group: highlight

    //@uniform
    readonly bool borderHighlightEnabled = true;

    //@uniform
    readonly float_ borderHighlightThickness = 0.07;

    //@uniform
    readonly float_ borderHighlightOffset = 0.0;

    //@uniform
    readonly float_ borderHighlightValue = 0.7;
    //@uniform
    readonly float_ borderHighlightSaturation = -0.15;

    private vec4 lensImage(vec2 fragCoord, vec2 resolution)
    {
        vec2 uv = fragCoord / resolution;

        vec2 p = 2.0 * uv - 1.0;

        float resolutionScale = min(resolution.x, resolution.y);

        p *= resolution / resolutionScale;
        p *= 1.0 + (2.0 / resolutionScale); // add margin
        p *= 1.02; // scale

        float_ r = length(p);
        float_ a = atan(p.y, p.x);

        float_ h = (a / PI + 1.0) / 2.0;
        h += 0.45;

        vec3 col = vec3(sin(h * TAU), sin((h + 0.33) * TAU), sin((h + 0.66) * TAU)) * 0.5 + 0.7;

        float_ s = saturationIncrease * r + saturationStart;
        float_ v = 1.0;

        if (marginHighlightEnabled)
        {
            v += marginHighlightValue * smoothstep(0.0, marginHighlightThickness, r - marginPosition + marginHighlightOffset);
            s += marginHighlightSaturation * smoothstep(0.0, marginHighlightThickness, r - marginPosition + marginHighlightOffset);
        }

        if (notchEnabled)
        {
            // highlight
            float_ r1 = length(p + vec2(0.0, notchSkew));
            float_ r2 = length(p - vec2(0.0, notchSkew));

            v += notchHighlightValue * smoothstep(notchHighlightThickness, 0.0, abs(r1 - marginPosition - notchHighlightOffset));
            s += notchHighlightSaturation * smoothstep(notchHighlightThickness, 0.0, abs(r1 - marginPosition - notchHighlightOffset));

            // shadow
            v += notchShadowValue * smoothstep(notchShadowThickness, 0.0, abs(r2 - marginPosition - notchShadowOffset));
            s += notchShadowSaturation * smoothstep(notchShadowThickness, 0.0, abs(r2 - marginPosition - notchShadowOffset));
        }

        if (borderHighlightEnabled)
        {
            v += borderHighlightValue * smoothstep(borderHighlightThickness, 0.0, 1.0 - r - borderHighlightOffset);
            s += borderHighlightSaturation * smoothstep(borderHighlightThickness, 0.0, 1.0 - r - borderHighlightOffset);
        }

        col = pow3(col, pow(max(0.01, s), 2.0));
        col *= v;

        float_ edge = 3.0 / resolutionScale;
        float_ alpha = smoothstep(1.0 + edge, 1.0 - edge, r);

        return clamp(vec4(col, alpha), 0.0, 1.0);
    }

    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        if (scalePreviewEnabled)
        {
            float_ resolutionScale = min(iResolution.x, iResolution.y);

            fragColor = premultiplyAlpha(lensImage(fragCoord + 0.5 * iResolution.xy - 0.5 * vec2(resolutionScale), iResolution.xy));

            vec2 pos = vec2(resolutionScale, iResolution.y - 32.0);
            fragColor += premultiplyAlpha(lensImage(fragCoord - pos, vec2(32.0))); pos -= vec2(-4.0, 24.0);
            fragColor += premultiplyAlpha(lensImage(fragCoord - pos, vec2(24.0))); pos -= vec2(-4.0, 16.0);
            fragColor += premultiplyAlpha(lensImage(fragCoord - pos, vec2(16.0)));

            if (fragColor.a > 0.0)
            {
                fragColor.rgb /= fragColor.a;
            }
        }
        else
        {
            fragColor = lensImage(fragCoord, iResolution.xy);
        }
    }
}
