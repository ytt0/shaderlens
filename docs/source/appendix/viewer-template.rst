Viewer Template
===============

The following code could be used as a template for custom viewers:

.. code-block:: glsl

    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        // apply scale and pan
        vec2 position = (fragCoord.xy - iViewerOffset) / iViewerScale;

        // check for image boundaries
        if (position.x < 0 || position.x > iViewerChannelResolution.x || position.y < 0 || position.y > iViewerChannelResolution.y)
        {
            fragColor = vec4(0.0, 0.0, 0.0, 1.0);
            return;
        }

        // fetch the selected buffer pixel value
        fragColor = texelFetch(iChannelViewer, ivec2(floor(position)), 0);
        fragColor.a = 1.0;

        // modify fragColor value here, based on iViewerScale and iViewerOffset or position

        // Example: draw a circle inside each pixel, when scaled above 10.0
        if (iViewerScale > 10.0)
        {
            // scaled pixel size is at least 10x10 viewer pixels
            vec2 uv = fract(position); // position inside the pixel

            float circle = abs(length(uv * 2.0 - 1.0) - 0.8); // circle sdf, with a relative size of 0.8
            float mask = smoothstep(2.0 / iViewerScale, 0.0, circle); // circle line thickness of 2 viewer pixels
            float opacity = smoothstep(10.0, 20.0, iViewerScale); // fade-in from scale 10.0 to 20.0

            fragColor.rgb = clamp(fragColor.rgb, 0.0, 1.0); // clamp to visible value
            vec3 maskColor = vec3(step(fragColor.r + fragColor.g + fragColor.b, 1.5)); // opposite brightness
            fragColor.rgb = mix(fragColor.rgb, maskColor, mask * opacity);
        }
    }