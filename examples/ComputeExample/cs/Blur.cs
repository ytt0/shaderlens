// Adapted from: https://github.com/godotengine/godot/blob/master/servers/rendering/renderer_rd/shaders/effects/copy.glsl

namespace ComputeExample;

class Blur : Common
{
    //@uniform, min: 0.0, max: 4.0
    float_ blurSize = 4.0;

    //@replace-line: shared vec4 local_cache[256];
    vec4[] local_cache = new vec4[256];

    //@replace-line: shared vec4 temp_cache[128];
    vec4[] temp_cache = new vec4[128];

    vec4 sample_local_cache(float_ offset)
    {
        int index = int_(floor(offset));
        index = clamp(index, 0, 254);
        return mix(local_cache[index], local_cache[index + 1], fract(offset));
    }

    vec4 sample_temp_cache(float_ offset)
    {
        int index = int_(floor(offset));
        index = clamp(index, 0, 126);
        return mix(temp_cache[index], temp_cache[index + 1], fract(offset));
    }

    void main()
    {
        // Pixel being shaded
        ivec2 pos = ivec2(gl_GlobalInvocationID.xy);

        ivec2 size = ivec2(iResolution.xy);

        if (any(greaterThanEqual(pos, size + 4)))
        { //too large, do nothing
            return;
        }

        // First pass copy texture into 16x16 local memory for every 8x8 thread block
        ivec2 quad_center = ivec2(gl_GlobalInvocationID.xy + gl_LocalInvocationID.xy) - ivec2(4, 4);
        quad_center = clamp(quad_center, ivec2(0), size - 2);

        uint dest_index = gl_LocalInvocationID.x * 2 + gl_LocalInvocationID.y * 2 * 16;

        local_cache[dest_index] = texelFetch(iChannel0, quad_center, 0);
        local_cache[dest_index + 1] = texelFetch(iChannel0, quad_center + ivec2(1, 0), 0);
        local_cache[dest_index + 16] = texelFetch(iChannel0, quad_center + ivec2(0, 1), 0);
        local_cache[dest_index + 16 + 1] = texelFetch(iChannel0, quad_center + ivec2(1, 1), 0);

        // Simpler blur uses SIGMA2 for the gaussian kernel for a stronger effect.
        //@const
        float_[] kernel = new float_[] { 0.204164, 0.180174, 0.123832, 0.066282, 0.027631 };

        float_[] read_offset = new float_[] { 0.0, 0.25 * blurSize, 0.5 * blurSize, 0.75 * blurSize, blurSize };

        memoryBarrierShared();
        barrier();

        // Horizontal pass. Needs to copy into 8x16 chunk of local memory so vertical pass has full resolution
        uint read_index = gl_LocalInvocationID.x + gl_LocalInvocationID.y * 32 + 4;
        vec4 color_top = vec4(0.0);
        color_top += sample_local_cache(read_index - read_offset[4]) * kernel[4];
        color_top += sample_local_cache(read_index - read_offset[3]) * kernel[3];
        color_top += sample_local_cache(read_index - read_offset[2]) * kernel[2];
        color_top += sample_local_cache(read_index - read_offset[1]) * kernel[1];
        color_top += sample_local_cache(read_index) * kernel[0];
        color_top += sample_local_cache(read_index + read_offset[1]) * kernel[1];
        color_top += sample_local_cache(read_index + read_offset[2]) * kernel[2];
        color_top += sample_local_cache(read_index + read_offset[3]) * kernel[3];
        color_top += sample_local_cache(read_index + read_offset[4]) * kernel[4];

        // Next row
        read_index += 16;

        vec4 color_bottom = vec4(0.0);
        color_bottom += sample_local_cache(read_index - read_offset[4]) * kernel[4];
        color_bottom += sample_local_cache(read_index - read_offset[3]) * kernel[3];
        color_bottom += sample_local_cache(read_index - read_offset[2]) * kernel[2];
        color_bottom += sample_local_cache(read_index - read_offset[1]) * kernel[1];
        color_bottom += sample_local_cache(read_index) * kernel[0];
        color_bottom += sample_local_cache(read_index + read_offset[1]) * kernel[1];
        color_bottom += sample_local_cache(read_index + read_offset[2]) * kernel[2];
        color_bottom += sample_local_cache(read_index + read_offset[3]) * kernel[3];
        color_bottom += sample_local_cache(read_index + read_offset[4]) * kernel[4];

        // rotate samples to take advantage of cache coherency
        uint write_index = gl_LocalInvocationID.y * 2 + gl_LocalInvocationID.x * 16;

        temp_cache[write_index] = color_top;
        temp_cache[write_index + 1] = color_bottom;

        memoryBarrierShared();
        barrier();

        // If destination outside of texture, can stop doing work now
        if (any(greaterThanEqual(pos, size)))
        {
            return;
        }

        // Vertical pass
        uint index = gl_LocalInvocationID.y + gl_LocalInvocationID.x * 16 + 4;
        vec4 color = vec4(0.0);

        color += sample_temp_cache(index - read_offset[4]) * kernel[4];
        color += sample_temp_cache(index - read_offset[3]) * kernel[3];
        color += sample_temp_cache(index - read_offset[2]) * kernel[2];
        color += sample_temp_cache(index - read_offset[1]) * kernel[1];
        color += sample_temp_cache(index) * kernel[0];
        color += sample_temp_cache(index + read_offset[1]) * kernel[1];
        color += sample_temp_cache(index + read_offset[2]) * kernel[2];
        color += sample_temp_cache(index + read_offset[3]) * kernel[3];
        color += sample_temp_cache(index + read_offset[4]) * kernel[4];

        imageStore(OutputTexture0, pos, color);
    }
}
