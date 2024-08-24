Project Templates
=================

Minimal Project
---------------

.. code-block:: json

    {
        "Image": {
            "Source": "Image.glsl",
            "Channel0": "Animation.gif"
        }
    }

.. note::

    If channel binding, or other :ref:`pass options<definition-pass>` are not needed, :file:`Image.glsl` can be loaded directly.

Two Pass Project
----------------

.. code-block:: json

    {
        "Common": "Common.glsl",

        "Buffer1": {
            "Source": "Buffer1.glsl",
            "Channel0": "Buffer1", // read the previous frame data
            "Channel1": "Keyboard"
        },

        "Image": {
            "Source": "Image.glsl",
            "Channel0": "Buffer1"
        }
    }


Multi Pass Project
------------------

.. code-block:: json

    {
        "Common": [
            "%SHADERS_COMMON%/Include/SDF.glsl",
            "%SHADERS_COMMON%/Include/BlendModes.glsl"
        ],

        "Buffer1": {
            "DisplayName": "Computer Model",
            "Source": "Model.glsl",
            "DefaultViewer": "NormalsOverlay",
            "Channel0": "Buffer1", // read camera state from last frame
            "Channel1": "Keyboard" // use keyboard to move the camera
        },

        "Buffer2": {
            "DisplayName": "Outlines",
            "Source": "Outlines.glsl",
            "Channel0": {
                "Type": "Framebuffer",
                "Source": "Buffer1",
                "MipmapFilter": "Linear" // create mipmap, for blur approximation
            }
        },

        "Buffer3": {
            "DisplayName": "Monitor Animation",
            "Source": "ProcessAnimation.glsl",
            "Channel0": {
                "Type": "TextureSequence",
                "Source": "Resources/MonitorAnimation/Frame*.png",
                "FrameRate": 10
            },
            "RenderSize": "400, 300" // fixed size
        },

        "Image": {
            "DisplayName": "Composition",
            "Channel0": "Buffer2", // model and outlines data
            "Channel1": "Buffer3", // processed animation
            "Source": "Image.glsl"
        },

        "Viewers": {
            "NormalsOverlay": {
                "DisplayName": "Normals Overlay",
                "Source": "%SHADERS_COMMON%/Viewers/NormalsOverlay.glsl",
                "Channel0": "Viewer" // selected buffer
            }
        }
    }
