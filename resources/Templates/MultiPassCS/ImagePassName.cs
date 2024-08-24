namespace ProjectName;

class ImagePassName : Common
{
    void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        // Read the previous buffer
        fragColor = texelFetch(iChannel0, ivec2(fragCoord), 0);
    }
}
