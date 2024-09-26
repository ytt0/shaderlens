Input Settings
==============

Inputs are defined under the application folder, at :file:`Shaderlens.inputs.json` (can be opened from the viewport menu: :menuselection:`Options --> Open Input File`). |br|
Each setting can be a string, or an array of strings for multiple options.

Each input string is a combination of one or more of the following values:

Mouse
    | ``MouseLeft`` ``MouseMiddle`` ``MouseRight`` ``MouseX1`` ``MouseX2``
    | ``ScrollUp`` ``ScrollDown``

Keyboard
    | ``Alt`` ``Ctrl`` ``Shift`` ``Win``
    | ``A``-``Z`` ``0``-``9`` ``Num0``-``Num9`` ``F1``-``F12``
    | ``+`` ``-`` ``*`` ``/`` ``.`` ``<`` ``>`` ``[`` ``]`` ``/`` ``\`` ``;`` ``'`` ``~``
    | ``Enter`` ``Space`` ``Esc`` ``Tab`` ``Menu`` ``Pause`` ``Backspace``
    | ``Left`` ``Right`` ``Up`` ``Down``
    | ``Insert`` ``Delete`` ``Home`` ``End`` ``PageUp`` ``PageDown``
    | ``LAlt`` ``RAlt`` ``LCtrl`` ``RCtrl`` ``LShift`` ``RShift``
    | (additional `Key Enum <https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.key>`_ values could also be used).

Each input settings can have one of the following event specifiers:

    - ``Press(<input>)`` - The associated action would run on key or mouse button press event (this is the default behavior).
    - ``Release(<input>)`` - The associated action would run on key or mouse button release event.
    - ``Global(<input>)`` - The input would be registered as a system-wide hotkey, and the associated action would run on key press event. The expected input is a single key and an optional combination of modifier keys (``Alt`` ``Ctrl`` ``Shift`` ``Win``).

Default Inputs
--------------

- ``Shader.Play`` - Start / Resume rendering.

    - :kbd:`Alt` + :kbd:`Up`
    - Global: :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`Win` + :kbd:`P`.

- ``Shader.Pause`` - Pause rendering.

    - :kbd:`Alt` + :kbd:`Up`
    - Global: :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`Win` + :kbd:`P`.

- ``Shader.Step`` - Render a single frame.

    - :kbd:`Alt` + :kbd:`Right`
    - :kbd:`~`

- ``Shader.Restart`` - Restart rendering

    - :kbd:`Alt` + :kbd:`Left`
    - :kbd:`Alt` + :kbd:`Down`
    - Global: :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`Win` + :kbd:`O`.

- ``Uniforms`` - Open the :ref:`Uniforms view<features-uniforms-view>`.

    - :kbd:`Ctrl` + :kbd:`U`

- ``StartPage`` - Open Start Page view.
- ``Project.New`` - Open the :ref:`New Project view<features-new-project>`.

    - :kbd:`Ctrl` + :kbd:`N`

- ``Project.Open`` - Open project.

    - :kbd:`Ctrl` + :kbd:`O`

- ``Project.Reload`` - Reload project.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`R`

- ``Project.Save`` - Save project changes.

    - :kbd:`Ctrl` + :kbd:`S`

- ``Help`` - Open help.

.. _input-shader-mouse:

- ``Shader.Mouse`` - Set shader mouse down state.

    - :kbd:`Mouse Left Button`

- ``Menu.Main`` - Open viewport menu.

    - :kbd:`Mouse Right Button`
    - :kbd:`Menu`

- ``Menu.RecentProjects`` - Open Recent Projects submenu.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`O`

- ``Menu.ProjectFiles`` - Open Project Files submenu.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`F`

- ``Menu.Buffers`` - Open Buffers submenu.

    - :kbd:`Ctrl` + :kbd:`B`

- ``Menu.Export`` - Open Export submenu.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`E`

- ``Menu.Copy`` - Open Copy submenu.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`C`

- ``Menu.Resolution`` - Open Resolution submenu.
- ``Menu.FrameRate`` - Open Frame Rate submenu.
- ``Menu.Speed`` - Open Speed submenu.
- ``Menu.Viewer`` - Open Viewer submenu.
- ``Menu.Options`` - Open Options submenu.
- ``Resize.SnapSmall`` - Viewport resize snap modifier (1px).

    - :kbd:`Shift`

- ``Resize.SnapMedium`` - Viewport resize snap modifier (10px).

    - :kbd:`Ctrl`

- ``Resize.KeepRatio`` - Viewport resize keep ratio modifier.

    - :kbd:`Alt`

- ``Viewer.Pan`` - Pan viewer.

    - :kbd:`Mouse Middle Button`

.. _input-viewer-pan-speed:

- ``Viewer.PanSpeed`` - Pan speed modifier.

    - :kbd:`Shift`

- ``Viewer.PanSnap`` - Pan snap to pixel size modifier.

    - :kbd:`Alt`

.. _input-viewer-scale:

- ``Viewer.Scale`` - Scale viewer.

    - :kbd:`Ctrl` + :kbd:`Mouse Middle Button`
    - :kbd:`Mouse Right Button` + :kbd:`Mouse Middle Button`

- ``Viewer.ScaleUp`` - Scale up.

    - :kbd:`Ctrl` + :kbd:`Mouse Scroll Up`
    - :kbd:`Mouse Right Button` + :kbd:`Mouse Scroll Up`
    - :kbd:`Ctrl` + :kbd:`+`

- ``Viewer.ScaleDown`` - Scale down.

    - :kbd:`Ctrl` + :kbd:`Mouse Scroll Down`
    - :kbd:`Mouse Right Button` + :kbd:`Mouse Scroll Down`
    - :kbd:`Ctrl` + :kbd:`-`

- ``Viewer.ScaleReset`` - Reset viewer scale.

    - :kbd:`Ctrl` + :kbd:`0`

.. _input-viewer-scale-speed:

- ``Viewer.ScaleSpeed`` - Scale speed modifier.

    - :kbd:`Shift`

- ``Copy.Frame`` - Copy frame to clipboard.
- ``Copy.FrameWithAlpha`` - Copy frame with alpha channel to clipboard.
- ``Copy.Repeat`` - Repeat last copy action.

    - :kbd:`Ctrl` + :kbd:`C`

- ``FullScreen.Toggle`` - Toggle full screen view.

    - :kbd:`F11`

- ``FullScreen.Leave`` - Leave full screen view.

    - :kbd:`Escape`

- ``Project.OpenFolder`` - Open project folder.
- ``FrameRate.Full`` - Set full frame rate.
- ``FrameRate.2`` - Set 1/2 frame rate.
- ``FrameRate.4`` - Set 1/4 frame rate.
- ``FrameRate.8`` - Set 1/8 frame rate.
- ``FrameRate.16`` - Set 1/16 frame rate.
- ``FrameRate.Decrease`` - Decrease the frame rate by a factor of 2.
- ``FrameRate.Increase`` - Increase the frame rate by a factor of 2.
- ``Resolution.Full`` - Set full resolution.

    - :kbd:`Ctrl` + :kbd:`\\`

- ``Resolution.2`` - Set 1/2 resolution.
- ``Resolution.4`` - Set 1/4 resolution.
- ``Resolution.8`` - Set 1/8 resolution.
- ``Resolution.16`` - Set 1/16 resolution.
- ``Resolution.32`` - Set 1/32 resolution.
- ``Resolution.64`` - Set 1/64 resolution.
- ``Resolution.Decrease`` - Downscale the resolution by a factor of 2.

    - :kbd:`Ctrl` + :kbd:`[`

- ``Resolution.Increase`` Upscale the resolution by a factor of 2.

    - :kbd:`Ctrl` + :kbd:`]`

- ``Speed.1_16`` - Set x1/16 speed.
- ``Speed.1_8`` - Set x1/8 speed.
- ``Speed.1_4`` - Set x1/4 speed.
- ``Speed.1_2`` - Set x1/2 speed.
- ``Speed.Normal`` - Set full speed.

    - :kbd:`Shift` + :kbd:`/`

- ``Speed.2`` Set x2 speed.
- ``Speed.4`` Set x4 speed.
- ``Speed.8`` Set x8 speed.
- ``Speed.16`` Set x16 speed.
- ``Speed.Decrease`` - Decrease the speed by a factor of 2.

    - :kbd:`Shift` + :kbd:`<`

- ``Speed.Increase`` - Increase the speed by a factor of 2.

    - :kbd:`Shift` + :kbd:`>`

- ``Buffer.1`` - Select Buffer 1.
- ``Buffer.2`` - Select Buffer 2.
- ``Buffer.3`` - Select Buffer 3.
- ``Buffer.4`` - Select Buffer 4.
- ``Buffer.5`` - Select Buffer 5.
- ``Buffer.6`` - Select Buffer 6.
- ``Buffer.7`` - Select Buffer 7.
- ``Buffer.8`` - Select Buffer 8.
- ``Buffer.Image`` - Select Image buffer.

    - :kbd:`Ctrl` + :kbd:`/`

- ``Buffer.Next`` - Select next buffer.

    - :kbd:`Ctrl` + :kbd:`>`

- ``Buffer.Previous`` - Select previous buffer.

    - :kbd:`Ctrl` + :kbd:`<`

- ``Export.Frame`` - Export current frame.
- ``Export.FrameRepeat`` - Export current frame to the next path.
- ``Export.RenderSequence`` - Open :ref:`Render Sequence view<features-render-sequence>`.
- ``PinnedProject.1`` - Open pinned project 1.
- ``PinnedProject.2`` - Open pinned project 2.
- ``PinnedProject.3`` - Open pinned project 3.
- ``PinnedProject.4`` - Open pinned project 4.
- ``PinnedProject.5`` - Open pinned project 5.
- ``RecentProject.1`` - Open most recent project.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`Alt` + :kbd:`O`

- ``RecentProject.2`` - Open recent project 2.
- ``RecentProject.3`` - Open recent project 3.
- ``RecentProject.4`` - Open recent project 4.
- ``RecentProject.5`` - Open recent project 5.
- ``Viewer.None`` - Disable viewer.
- ``Viewer.ValuesOverlay`` - Set "Values Overlay" viewer.
- ``Options.AlwaysOnTop`` - Toggle "Always On Top" option.

    - :kbd:`Ctrl` + :kbd:`Shift` + :kbd:`A`

- ``Options.AutoReload`` - Toggle "Auto Reload Project Files" option.
- ``Options.RestartOnAutoReload`` - Toggle "Restart On Auto Reload" option.
- ``Options.ClearStateOnRestart`` - Toggle "Clear State On Restart" option.
- ``Options.PauseOnInactivity`` - Toggle "Pause On Inactivity" option.
- ``Options.RenderInputEventsWhenPaused`` - Toggle "Render Input Events When Paused" option.
- ``Options.WrapShaderInputCursor`` - Toggle "Wrap Shader Input Cursor".
- ``Options.EnableShaderCache`` - Toggle "Enable Shader Cache".
- ``Options.DarkTheme`` - Toggle "Dark Theme" option.
- ``Options.OpenSettingsFile`` - Open application settings file.
- ``Options.OpenInputsFile`` - Open application inputs file.
- ``Options.OpenThemeFile`` - Open selected theme file.

Uniform Graph Editor
    - ``Graph.Drag`` -  Drag value cursor.

        - :kbd:`MouseLeft`,

    - ``Graph.DragCancel`` - Cancel value change.

        - :kbd:`MouseRight`
        - :kbd:`Esc`

    - ``Graph.SmallStepModifier`` - Snap to small increments.

        - :kbd:`Shift`

    - ``Graph.MediumStepModifier`` - Snap to medium increments.

        - :kbd:`Ctrl`
    - ``Graph.LargeStepModifier`` - Snap to large increments.

        - :kbd:`Ctrl` + :kbd:`Shift`,

    - ``Graph.Pan`` - Pan view.

        - :kbd:`Mouse Middle Button`

    - ``Graph.Scale`` - Scale view.

        - :kbd:`Ctrl` + :kbd:`Mouse Middle Button`
        - :kbd:`Mouse Middle Button` + :kbd:`Mouse Right Button`

    - ``Graph.ScaleUp`` - Scale up.

        - :kbd:`Mouse Scroll Up`
        - :kbd:`Mouse Right Button` + :kbd:`Mouse Scroll Up`
        - :kbd:`Ctrl` + :kbd:`+`
        - :kbd:`Ctrl` + :kbd:`Num+`

    - ``Graph.ScaleDown`` - Scale down.

        - :kbd:`Mouse Scroll Down`
        - :kbd:`Mouse Right Button` + :kbd:`Mouse Scroll Down`
        - :kbd:`Ctrl` + :kbd:`-`
        - :kbd:`Ctrl` + :kbd:`Num-`

    - ``Graph.ScaleReset`` - Reset scale.

        - :kbd:`Ctrl` + :kbd:`0`
        - :kbd:`Ctrl` + :kbd:`Num0`

    - ``Graph.ResetView`` - Reset pan and scale.

        - :kbd:`R`

    - ``Graph.FocusView`` - Focus on cursor position.

        - :kbd:`F`

    - ``Graph.ToggleTargetValue`` - Revert current value back to the initial value.

        - :kbd:`Mouse Right Button`

    - ``Graph.ToggleSourceValue`` - Set current value as an initial value.

        - :kbd:`Ctrl` + :kbd:`Mouse Right Button`
