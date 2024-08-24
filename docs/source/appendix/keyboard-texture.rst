Keyboard Texture
================

The keyboard texture reflects the current state of the keyboard, and could be accessed by the shader.
The texture contains 3 rows of 256 pixels, where the **red** channel is either ``0.0`` or ``1.0``.

Usage Example
-------------

The texture can be used by setting a pass **Channel#** :ref:`binding<definition-binding>` to ``Keyboard``, for example:

.. code-block:: json
    :emphasize-lines: 4,4

    {
        "Image": {
            "Source": "Image.glsl",
            "Channel0": "Keyboard"
        }
    }

And reading it in the following way:

.. code-block:: glsl

    // is key currently down (true for multiple frames)
    bool isKeyDown = texelFetch(iChannel0, ivec2(keyCode, 0), 0).r > 0.0;

    // is key down started at the current frame ("pressed" event, true for a single frame)
    bool isKeyPressed = texelFetch(iChannel0, ivec2(keyCode, 1), 0).r > 0.0;

    // is key toggled (had an odd number of pressed events)
    bool isKeyToggled = texelFetch(iChannel0, ivec2(keyCode, 2), 0).r > 0.0;


KeyCodes
----------

Available keys are mapped as follows:

.. code-block:: glsl

    #define KeyBackspace 8
    #define KeyTab 9
    #define KeyEnter 13
    #define KeyShift 16
    #define KeyCtrl 17
    #define KeyAlt 18
    #define KeyPause 19
    #define KeyCapsLock 20
    #define KeyEscape 27
    #define KeySpace 32
    #define KeyPageUp 33
    #define KeyPageDown 34
    #define KeyEnd 35
    #define KeyHome 36
    #define KeyLeft 37
    #define KeyUp 38
    #define KeyRight 39
    #define KeyDown 40
    #define KeyPrintScreen 44
    #define KeyInsert 45
    #define KeyDelete 46
    #define KeyNum0 48
    #define KeyNum1 49
    #define KeyNum2 50
    #define KeyNum3 51
    #define KeyNum4 52
    #define KeyNum5 53
    #define KeyNum6 54
    #define KeyNum7 55
    #define KeyNum8 56
    #define KeyNum9 57
    #define KeySemicolon 59
    #define KeyPlus 61
    #define KeyA 65
    #define KeyB 66
    #define KeyC 67
    #define KeyD 68
    #define KeyE 69
    #define KeyF 70
    #define KeyG 71
    #define KeyH 72
    #define KeyI 73
    #define KeyJ 74
    #define KeyK 75
    #define KeyL 76
    #define KeyM 77
    #define KeyN 78
    #define KeyO 79
    #define KeyP 80
    #define KeyQ 81
    #define KeyR 82
    #define KeyS 83
    #define KeyT 84
    #define KeyU 85
    #define KeyV 86
    #define KeyW 87
    #define KeyX 88
    #define KeyY 89
    #define KeyZ 90
    #define KeyMenu 93
    #define Key0 96
    #define Key1 97
    #define Key2 98
    #define Key3 99
    #define Key4 100
    #define Key5 101
    #define Key6 102
    #define Key7 103
    #define Key8 104
    #define Key9 105
    #define KeyMultiply 106
    #define KeyAdd 107
    #define KeySubtract 109
    #define KeyNumDecimal 110
    #define KeyNumDivide 111
    #define KeyF1 112
    #define KeyF2 113
    #define KeyF3 114
    #define KeyF4 115
    #define KeyF5 116
    #define KeyF6 117
    #define KeyF7 118
    #define KeyF8 119
    #define KeyF9 120
    #define KeyF10 121
    #define KeyF11 122
    #define KeyF12 123
    #define KeyNumLock 144
    #define KeyScrollLock 145
    #define KeyMinus 173
    #define KeyComma 188
    #define KeyPeriod 190
    #define KeyQuestion 191
    #define KeyTilde 192
    #define KeyOpenBrackets 219
    #define KeyPipe 220
    #define KeyCloseBrackets 221
    #define KeyQuotes 222
