void main()
{
    ivec2 texelCoord = ivec2(gl_GlobalInvocationID.xy);

    vec2 localUv = vec2(gl_LocalInvocationID.xy) / vec2(gl_WorkGroupSize.xy);
    vec2 globalUv = vec2(gl_GlobalInvocationID.xy) / vec2(gl_NumWorkGroups.xy * gl_WorkGroupSize.xy);

    vec2 uv = localUv;

    // Time varying pixel color
    float hue = 0.5 * sin(0.2 * (iTime + uv.x)) + 0.5 * sin(0.2 * (iTime + uv.y) + 1.0);
    vec3 col = vec3(sin(hue * 6.28), sin((hue + 0.33) * 6.28), sin((hue + 0.66) * 6.28)) * 0.3 + 0.7;

    // Output to screen
    imageStore(OutputTexture0, texelCoord, vec4(col, 1.0));
}
