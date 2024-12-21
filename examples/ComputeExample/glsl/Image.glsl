void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    vec3 color = texelFetch(iChannel0, ivec2(fragCoord), 0).rgb;
    color = pow3(color, 0.45); // convert to srgb
    fragColor = vec4(color, 1.0);
}
