// see key codes at https://ytt0.github.io/shaderlens/appendix/keyboard-texture.html
#define KeyA 65

// is key currently down (true for multiple frames)
#define IsKeyDown(keyCode) (texelFetch(iChannel0, ivec2((keyCode), 0), 0).r > 0.0)

// is key down started at the current frame ("pressed" event, true for a single frame)
#define IsKeyPressed(keyCode) (texelFetch(iChannel0, ivec2((keyCode), 1), 0).r > 0.0)

// is key toggled (had an odd number of pressed events)
#define IsKeyToggled(keyCode) (texelFetch(iChannel0, ivec2((keyCode), 2), 0).r > 0.0)

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    float radius = 0.5 * iResolution.x / 26.0;

    float mask = 0.0;

    // keyboard input (A-Z)

    // row 1: is key down
    // row 2: is key pressed
    // row 3: is key toggled

    for (int i = 0; i < 26; i++)
    {
        int keyCode = i + KeyA;

        bool isKeyDown = IsKeyDown(keyCode);
        bool isKeyPressed = IsKeyPressed(keyCode);
        bool isKeyToggled = IsKeyToggled(keyCode);

        vec2 p = vec2((float(i) + 0.5) * radius * 2, radius);
        mask += step(length(fragCoord - p), radius) * (float(isKeyToggled) + 1.0);

        p += vec2(0.0, 2.0 * radius);
        mask += step(length(fragCoord - p), radius) * (float(isKeyPressed) + 1.0);

        p += vec2(0.0, 2.0 * radius);
        mask += step(length(fragCoord - p), radius) * (float(isKeyDown) + 1.0);
    }

    // mouse input

    vec2 mousePosition = iMouse.xy;
    vec2 pressedMousePosition = abs(iMouse.zw);

    // is mouse currently down (true for multiple frames)
    bool isMouseDown = iMouse.z > 0.0;

    // is mouse down started at the current frame ("pressed" event, true for a single frame)
    bool isMousePressed = iMouse.w > 0.0;

    mask += step(length(fragCoord - mousePosition), radius) * (float(isMouseDown) + 1.0);
    mask += step(abs(length(fragCoord - pressedMousePosition) - radius * 1.5), radius * 0.25) * (float(isMousePressed) + 1.0);

    float hue = fragCoord.x / iResolution.x;
    vec3 col = vec3(sin(hue * 6.28), sin((hue + 0.33) * 6.28), sin((hue + 0.66) * 6.28)) * 0.3 + 0.7;

    col *= 0.5 * mask;
    fragColor = vec4(col, 1.0);
}
