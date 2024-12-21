# Examples

## Inputs Example

Displaying mouse and keyboard input states:

- Using the `iMouse` uniform to get mouse down / pressed states, and current / pressed positions.
- Using the `Keyboard` texture to get key down / pressed / toggled states.

To see the pressed events, pause or set a lower frame rate.

## Uniforms Example

Defining different types of uniforms, and uniforms groups, that can be changed from the **Uniforms** view.

## Viewers Example

Adding custom viewers to the `Viewers > ...` submenu.

- Normals Overlay - Displays a normal direction inside each pixel.  
- Opacity Background - Overlays the selected buffer using its alpha channel, over a checkerboard background, and adds a background brightness uniform.
- Color Adjustment - Displays the selected buffer with color adjustment, controlled by uniforms.
- Values Range - Displays the selected buffer interpolated linearly between min and max values, controlled by uniforms.

To see a selected viewer effect, scale in or reduce the resolution, and change the viewer uniforms values.

## Mipmap Example

Using `TextureFilter`, and `MipmapFilter`, to sample the previous buffer with `textureLod` at different levels.

## Compute Example

A single pass Gaussian blur with a radius of 4, using Compute shader.

- Shared arrays are used for minimizing image access calls.
- A memory barrier is used for synchronizing the horizontal and vertical blur steps.
- A uniform is used for scaling the blur radius.

# Download

A zip file with the content of this folder can be downloaded from any [release](https://github.com/ytt0/shaderlens/releases/latest).
