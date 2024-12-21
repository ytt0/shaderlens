Application Settings
====================

Settings are defined under the application folder, at :file:`Shaderlens.settings.json`, the following settings can only be edited from the settings file:

VertexHeader (*string*)
    Vertex shader code header, defines the shading language version.

    :Default: ``"#version 460"``.

FragmentHeader (*string*)
    Fragment shader code header, defines the shading language version.

    :Default: ``"#version 460"``.

ComputeHeader (*string*)
    Compute shader code header, defines the shading language version.

    :Default: ``"#version 460"``.

InactivityPauseSeconds (*integer*)
    Number of second used by the viewport menu :menuselection:`Options --> Pause On Inactivity` option.

    :Default: ``60``.

.. _settings-application-copy-formatters:

CopyFormatters (*array*)
    Copy menu available formats.

    Format parameters:
        | ``x`` ``y`` ``z`` ``w`` - Original value.
        | ``r`` ``g`` ``b`` ``a`` - Clamped value (between 0.0 and 1.0).
        | ``R`` ``G`` ``B`` ``A`` - Byte value (between 0 and 255).
        | ``lr`` ``lg`` ``lb`` ``la`` - Linear color value (between 0.0 and 1.0).
        | ``px`` ``py`` - Relative position value (between 0.0 and 1.0).
        | ``pX`` ``pY`` - Absolute position value (between 0 and texture width or height).

    :Default:

        .. code-block:: json

            [
                { "Name": "Value", "Format": "{{x}:0.0##}, {{y}:0.0##}, {{z}:0.0##}, {{w}:0.0##}" },
                { "Name": "sRGBA", "Format": "{{r}:0.0#}, {{g}:0.0#}, {{b}:0.0#}, {{a}:0.0#}" },
                { "Name": "sRGBA", "Format": "{{R}}, {{G}}, {{B}}, {{A}}" },
                { "Name": "sRGBA", "Format": "{{R}:x2}{{G}:x2}{{B}:x2}{{A}:x2}" },
                { "Name": "Linear RGBA", "Format": "{{lr}:0.0#}, {{lg}:0.0#}, {{lb}:0.0#}, {{la}:0.0#}" },
                { "Name": "Position", "Format": "{{px}:0.0##}, {{py}:0.0##}" },
                { "Name": "Position", "Format": "{{pX}}, {{pY}}" }
            ]

.. _settings-scale-factor:

ScaleFactor (*float*)
    Scale factor, when scaling with the mouse or the keyboard (``Viewer.Scale*`` :ref:`input<input-viewer-scale>`).

    :Default: ``1.1``.

.. _settings-scale-drag-factor:

ScaleDragFactor (*float*)
    Scale speed without a modifier, use < 1.0 for slower scale, and > 1.0 for faster scale.

    :Default: ``1.0``.

.. _settings-scale-speed-factor:

ScaleSpeedFactor (*float*)
    Scale speed with a modifier (``Viewer.ScaleSpeed`` :ref:`input<input-viewer-scale-speed>`).

    :Default: ``2.0``.

PanSpeedFactor (*float*)
    Pan speed with a modifier (``Viewer.PanSpeed`` :ref:`input<input-viewer-pan-speed>`).

    :Default: ``20.0``.

.. _settings-overlay:

OverlayGridVisibleScale (*float*)
    Lowest scale where the viewer grid overlay start being visible.

    :Default: ``6.0``.

OverlayValueVisibleScale (*float*)
    Lowest scale where the viewer values overlay start being visible.

    :Default: ``8.0``.

OverlayFontScale (*float*)
    Lowest font scale when values start being visible.

    :Default: ``2.0``.

LogErrorContextLines (*integer*)
    Number of lines to include when showing a source error location in the log.

    :Default: ``5``.

LogVisibilityDelaySeconds (*float*)
    Number of seconds to wait after compilation has started, before showing the compilation state (reduces flickering for short compilation times).

    :Default: ``0.5``.

MemoryCachedResources (*integer*)
    Number of resources (such as compiled shaders) that should be kept in memory during reload (prevents recompilation when a code changes are toggled).

    :Default: ``100``.

.. _settings-project-templates-path:

ProjectTemplatesPath (*string*)
    :ref:`New Project<features-new-project>` templates path.

    :Default: ``Resources\\Templates``.

.. _settings-drag-sensitivity:

TextBoxDragSensitivity (*float*)
    Mouse drag sensitivity for adjusting uniforms and other values.

    :Default: ``1.0``.

CursorVisibilityTimeoutSeconds (*float*)
    Number of seconds to wait before hiding the cursor when it's over the viewport.

    :Default: ``2.0``.

ConfirmSaveOnClose (*boolean*)
    Show a dialog before closing a project, if there are any unsaved changes.

    :Default: ``true``.

ShowStartPage (*boolean*)
    Show the **Start Page** view on application start.

    :Default: ``true``.

.. _settings-default-viewer-pass:

DefaultViewerPass (string)
    Default viewer pass key, should be one of ``"@None"``, ``"@ValuesOverlay"``, or a custom viewer key (has to be available in the project).

    :Default: ``"@ValuesOverlay"`` (built-in "Values Overlay" viewer).
