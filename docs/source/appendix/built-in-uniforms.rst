Built-in Uniforms
=================

The following uniforms are available for all the passes, and are compatible with **Shadertoy**'s conventions:

- :glsl:`vec3 iResolution`
    Viewport resolution (``z`` is pixel aspect ratio, currently always set to ``1.0``).

- :glsl:`int iFrame`
    Current frame index.

- :glsl:`float iTime`
    Current time, in seconds.

- :glsl:`float iTimeDelta`
    Time passed between the render start of the previous, and the current frame, in seconds.

- :glsl:`float iFrameRate`
    Number of frames rendered per second.

- :glsl:`vec4 iDate`
    | ``x`` - Year
    | ``y`` - Month
    | ``z`` - Day
    | ``w`` - Time of day, in seconds.

- :glsl:`vec4 iMouse`
    | ``xy`` - Current mouse position (when ``Shader.Mouse`` :ref:`input<input-shader-mouse>` matches).
    | ``abs(zw)`` - Pressed mouse position.
    | ``z > 0.0`` - Is mouse currently down (true for multiple frames).
    | ``w > 0.0`` - Is mouse down started at the current frame ("pressed" event, true for a single frame).
    | Note that when mouse is down / pressed at ``x = 0`` or ``y = 0``, a small delta (``0.0001``) is added to ``z`` or ``w``, in order to keep the checks above correct.

- :glsl:`sampler2D iChannel#`
    **Channel#** sampler.

- :glsl:`float[] iChannelTime`
    Channel time, in seconds (for ``.gif``, or ``TextureSequence`` bindings).

- :glsl:`float[] iChannelDuration`
    Channel duration, in seconds (for ``.gif``, or ``TextureSequence`` bindings).

- :glsl:`vec3[] iChannelResolution`
    Channel resolution.

Viewer Uniforms
    The following uniforms are mostly needed for the :doc:`/manual/viewer-pass`, but are also available for all the passes.

.. _built-in-uniforms-viewer-channel:

- :glsl:`sampler2D iViewerChannel`
    Selected pass framebuffer sampler.

- :glsl:`vec3 iViewerChannelResolution`
    Selected pass framebuffer resolution.

.. _built-in-uniforms-viewer-scale:

- :glsl:`float iViewerScale`
    Viewer transformation scale.

.. _built-in-uniforms-viewer-offset:

- :glsl:`vec2 iViewerOffset`
    Viewer transformation offset.

.. _built-in-uniforms-outputs:

Output Uniforms / Variables
---------------------------

Fragment Shader
    :glsl:`out vec4 FragColor#` (where **#** is a number between 0 and 7)
        :glsl:`FragColor0` is set automatically to the :glsl:`mainImage` output variable :glsl:`fragColor`.

Compute Shader
    :glsl:`writeonly image2D OutputTexture#` (where **#** is a number between 0 and 7)
        Compute shader outputs can be set using :glsl:`imageStore` (for example:|br|
        :glsl:`imageStore(OutputTexture0, texelCoord, vec4(1.0))`).

By default only one texture is assigned, for additional textures the :ref:`Pass Outputs<definition-pass>` property can be set.
