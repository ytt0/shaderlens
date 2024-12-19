Compute Shader Template
=======================

Compute shaders keep the default :glsl:`main()` entry point method, and write output data using the :glsl:`OutputTexture#` uniforms.

In addition to the :doc:`built-in<built-in-uniforms>` and :doc:`custom</manual/uniforms>` uniforms, OpenGL `Compute shader variables <https://www.khronos.org/opengl/wiki/Compute_Shader#Inputs>`_ are also available, containing the invocation context.

The work group size declaration (:glsl:`layout(local_size_x = X, local_size_y = Y, local_size_z = Z) in`) is automatically added based on the :ref:`Pass WorkGroupSize property<definition-pass>` value.

A compute shader pass can be defined as follows:

Shader:

.. code-block:: glsl

    void main()
    {
        ivec2 texelCoord = ivec2(gl_GlobalInvocationID.xy);

        vec2 localUv = vec2(gl_LocalInvocationID.xy) / vec2(gl_WorkGroupSize.xy);
        vec2 globalUv = vec2(gl_GlobalInvocationID.xy) / vec2(gl_NumWorkGroups.xy * gl_WorkGroupSize.xy);

        vec2 uv = localUv;

        // Time varying pixel color
        float hue = 0.5 * sin(0.2 * (iTime + uv.x)) + 0.5 * sin(0.2 * (iTime + uv.y) + 1.0);
        vec3 col = vec3(sin(hue * 6.28), sin((hue + 0.33) * 6.28), sin((hue + 0.66) * 6.28)) * 0.3 + 0.7;

        // Output to screen
        imageStore(OutputTexture0, texelCoord, vec4(col, 1.0));
    }

Pass definition:

.. code-block:: json

    {
        "Image": {
            "Type": "Compute",
            "Source": "Image.glsl",
            "WorkGroupSize": "8, 8", // group size, default value is (1, 1, 1)
            //"WorkGroups": "20, 10", // groups count, default value is (ceil(RenderSize / WorkGroupSize.xy), 1)
            //"RenderSize": "160, 80", // fixed buffer size
        }
    }
