Project
=======

A project defines a series of one or more passes (an ``Image`` pass, and optional ``Buffer#`` passes), each with source files references, and ``Channel#`` binding definition. A project can also define common source files that should be included with all the passes.

A project file should contain a single json *object*, with a :ref:`Project definition<definition-project>`, for example:

.. code-block:: json

    {
        "Common": "Common.glsl",
        "Buffer1": "Buffer1.glsl",
        "Image": {
            "Source": "Image.glsl",
            "Channel0": "Buffer1"
        }
    }

See :doc:`/appendix/project-templates` for more project definition examples.

.. note::

    All of the properties described below are optional, except the ones marked as **(required)**.

.. _definition-project:

Project Definition
------------------
An *object* with the following properties:

:Image: Image :ref:`Pass definition<definition-pass>` **(required)**.
:Buffer#: Buffers :ref:`Pass definition<definition-pass>` (where **#** is a number, or an uppercase letter (A-Z)).
:Common: A common :ref:`Source definition<definition-source>`, that should be included with other passes sources (not included by default with **Viewer** passes).
:Viewer: A single :ref:`Viewer Pass definition<definition-viewer-pass>`.
:Viewers: One of:

    - An *array* of :ref:`Viewer Passes definition<definition-viewer-pass>`.
    - An *object* (dictionary) with :ref:`Viewer Passes definitions<definition-viewer-pass>` (where the property name is used as the viewer key).
:Paused: Start paused (*boolean*), could be used when the shader has the same output on every frame, the default value is :json:`false`.
:RenderSize: A global :ref:`RenderSize definition<definition-render-size>`, that is applied to all the passes (can be overridden individually), when not specified, the viewport size is used.
:Scalable: A global :ref:`Scalable definition<definition-scalable>` (*boolean*), that is applied to all the passes (can be overridden individually).

.. _definition-project-uniforms:

:Uniforms: Project uniforms values path, the default value is alongside the project file, with a :file:`.uniforms.json` suffix.

.. _definition-pass:

Pass Definition
---------------
- Can be a :ref:`Source definition<definition-source>` (such as a single source file path or an array of paths).

- Can be an *object* with the following properties:

    :Source: :ref:`Source definition<definition-source>` **(required)**.
    :Channel#: :ref:`Binding definition<definition-binding>` (where **#** is a number between 0 and 7).
    :Outputs: Number of output textures (*integer*), default value is 1. |br|
        Output textures can be accessed using :ref:`output variables<built-in-uniforms-outputs>`.
    :DisplayName: Pass display name (*string*).
    :DefaultViewer: A key (*string*) of a :ref:`Viewer Pass<definition-viewer-pass>` that should be selected by default. Viewers keys should match the :ref:`Project<definition-project>`'s **Viewers** object properties name.
    :IncludeCommon: Include common sources (*boolean*), the default value is :json:`true`.
    :RenderSize: :ref:`RenderSize definition<definition-render-size>`.
    :Scalable: :ref:`Scalable definition<definition-scalable>`.

.. _definition-viewer-pass:

Viewer Pass Definition
----------------------
- Can be a :ref:`Source definition<definition-source>` (such as a single source file path or an array of paths).

- Can be an *object* with the following properties:

    :Source: :ref:`Source definition<definition-source>` **(required)**.
    :Channel#: :ref:`Binding definition<definition-binding>` (where # is a number between 0 and 7).
    :DisplayName: Pass display name (*string*).
    :IncludeCommon: Include common sources (*boolean*), the default value is :json:`false`.

.. _definition-source:

Source Definition
-----------------
- Can be a single source file path (*string*), for example: :json:`"Source": "Image.glsl"`.
- Can be an *array* of paths, for example:|br|
  :json:`"Source": ["Image-include.glsl", "Image-main.glsl"]`.

    - The source files are concatenated at the same order as they appear on the array, a correct functions declaration and implementation order is assumed.

Notes
    Paths are relative to the project file, can contain forward ``/`` or backward slashes ``\\`` (escaped), and can contain environment variables in the form of ``%VAR_NAME%``, for example: |br| :json:`"Source": "%SHADERS_COMMON%/Include/BlendModes.glsl"`.

    Source files with a :file:`.cs` extension, are converted to GLSL using :doc:`/manual/csharp/conversion-rules`. |br|

.. _definition-binding:

Binding Definition
------------------
- Can be a *string value*, one of:

    - ``Image``, ``Buffer#`` - A defined pass default output texture.

        - For passes with multiple **Outputs**, a texture index can be added with square brackets (``Image[0-7]``, ``Buffer#[0-7]``).

    - ``Viewer`` - Current selected pass texture (from the viewport menu: :menuselection:`Buffers --> ...`).
    - ``Keyboard`` - :doc:`A Keyboard Texture</appendix/keyboard-texture>`.
    - A path to an image file (with a supported extension).

- Can be an *object* with the following properties:

    :Type: A *string value*, one of ``Framebuffer`` (default), ``Texture`` or ``TextureSequence``.
    :Source: A *string value* **(required)**, depends on **Type**:

        - For ``Framebuffer``, the value should be a pass name (``Image``, ``Buffer#``, or ``Viewer``).
        - For ``Texture``, the value should be a path to an image file (with a supported extension), or ``Keyboard``.
        - For ``TextureSequence``, the value should be a search pattern for image files (for example: ``"Animation/Frame*.png"``). A **FrameRate** property is also required.

    :FrameRate: Frames per second (*integer*), used with ``TextureSequence`` type.
    :TextureFilter: Texture sampling mode (*string*), one of ``Nearest`` (default), or ``Linear``.
    :MipmapFilter: Mipmap sampling mode (*string*), one of ``None`` (default), ``Nearest``, ``Linear`` (*string*). If ``None`` is selected, a mipmap would not be generated.
    :Wrap: Texture wrapping mode (*string*), one of ``ClampToEdge`` (default), ``Repeat``, ``MirroredRepeat`` (*string*).

        **WrapS** / **WrapT** can be used for different X / Y wrapping modes.

.. _definition-render-size:

RenderSize Definition
---------------------
A *string value* of the format ``"<width>, <height>"`` (for example ``"800, 600"``), defines a buffer size that should be used instead of the viewport size. |br|
When specified at project level, defines a default size for all the passes.

To allow downscaling, the :ref:`Scalable<definition-scalable>` property should be set to :json:`true`.

.. _definition-scalable:

Scalable Definition
-------------------
A *boolean value*, allows :ref:`RenderSize<definition-render-size>`, if specified, to be downscaled when selecting a lower viewer resolution (from the viewport menu: :menuselection:`Resolution --> ...`), the default value is :json:`false`. |br|
When specified at project level, defines a default behavior for all the passes.
