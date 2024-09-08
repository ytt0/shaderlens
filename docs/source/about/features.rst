Features
========

Render Modes
------------
In addition to :ref:`viewer transformations<getting-started-usage>`, the following modes, can also help inspecting a running shader.

Buffers
    Show framebuffers of different passes (when defined). |br|
    Viewport menu: :menuselection:`Buffers --> ...`, or :kbd:`Ctrl` + :kbd:`B`. |br|
    Select previous/next buffer with :kbd:`Ctrl` + :kbd:`<` :kbd:`>`, select **Image** buffer with :kbd:`Ctrl` + :kbd:`/`.

Resolution
    Downscale the viewport resolution without reducing the viewport window size. |br|
    Viewport menu: :menuselection:`Resolution --> ...`. |br|
    Scale resolution down/up with :kbd:`Ctrl` + :kbd:`[` :kbd:`]`, reset resolution with :kbd:`Ctrl` + :kbd:`\\`.

Frame Rate
    Render at lower frame rate. |br|
    Viewport menu: :menuselection:`Frame Rate --> ...`.

Speed
    Set :glsl:`iTime` uniform value change speed. |br|
    Viewport menu: :menuselection:`Speed --> ...`. |br|
    Set lower/higher speed with :kbd:`Shift` + :kbd:`<` :kbd:`>`, reset speed with :kbd:`Shift` + :kbd:`/`.

Viewer
    Select a :doc:`/manual/viewer-pass`. |br|
    Viewport menu: :menuselection:`Viewer --> ...`. |br|
    The default "**Values Overlay**" viewer, adds components value information inside each pixel (visible when pixels are scaled above a certain level (:ref:`settings<settings-overlay>`)). |br|
    Custom :doc:`viewers</manual/viewer-pass>` can be :ref:`defined<definition-viewer-pass>` to display specific intermediate buffer information, such as normal direction that is encoded in one of the pixel components (`example <https://github.com/ytt0/shaderlens/tree/main/examples/ViewersExample/glsl/NormalsViewer.glsl>`_).

.. _features-new-project:

New Project
-----------
Provides a quick way to create a new project from a template. |br|
Viewport menu: :menuselection:`Export --> Render Sequence...`, or :kbd:`Ctrl` + :kbd:`N` |br|
Templates can be found under the application folder, at :file:`Resources\\Templates` (:ref:`settings<settings-project-templates-path>`).


.. _features-uniforms-view:

Uniforms View
-------------
The Uniforms view lists the current :doc:`declared uniforms</manual/uniforms>`, and is updated automatically when sources are reloaded.
Uniforms view properties and underlying type, can be customized using the :ref:`uniform line annotation<uniforms-properties>`.
Uniforms can also be arranged in groups, using the :ref:`uniform group line annotation<uniforms-group>`.

Color uniforms are edited using an `Okhsv <https://bottosson.github.io/posts/oklab/>`_ color space, to automatically adjust the perceived brightness for different hues.

Uniforms values text box can parse expressions with basic arithmetic operations and some GLSL functions (for example :glsl:`sin(pi/3)^2`).

Uniforms values can be copied and pasted with mouse hover while pressing :kbd:`Ctrl` + :kbd:`C` and |br| :kbd:`Ctrl` + :kbd:`V`, and reset to default value with :kbd:`Backspace`.

The mouse drag sensitivity while changing values can be adjusted in the :ref:`settings<settings-drag-sensitivity>`.


.. _features-copy-to-clipboard:

Copy to Clipboard
-----------------
Copy options, viewport menu: :menuselection:`Copy --> ...`, or :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`C`. |br|
Copy again with :kbd:`Ctrl` + :kbd:`C`.

The source value position, and the frame content, are captured when the copy submenu is first opened.

Copy value formats are defined in the :ref:`settings <settings-application-copy-formatters>`.

.. _features-render-sequence:

Render Sequence
---------------
Renders and stores a series of images, from a selected pass, with a specific resolution, and frame rate. |br|
Viewport menu: :menuselection:`Export --> Render Sequence...`

- The extension of the :guilabel:`Location` path defines the output type.
- The format and position of the frame index in the file name can be defined by adding a number to the location path (the last number is used), for example: :file:`Render\\Animation2-0001.png`.
- When :guilabel:`Start Frame` is larger than 0:

    - If :guilabel:`Relative Frame Index` is selected, the output would start at index 0, instead of the absolute frame index.
    - If :guilabel:`Pre-render From Frame 0` is selected, all the frames leading to the start frame would be rendered too, but not stored.
