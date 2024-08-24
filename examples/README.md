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

To see a selected viewer effect, scale in or reduce the resolution, and change the viewer uniforms values.

## Mipmap Example

Using `TextureFilter`, and `MipmapFilter`, to sample the previous buffer with `textureLod` at different levels.

# Download

A zip file with the content of this folder can be downloaded from any [release](https://github.com/ytt0/shaderlens/releases/latest).
