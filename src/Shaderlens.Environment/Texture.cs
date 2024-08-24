namespace Shaderlens
{
    public class sampler1D { }
    public class sampler1DArray { }
    public class sampler2D : sampler1D { }
    public class sampler2DArray { }
    public class sampler2DMS { }
    public class sampler2DMSArray { }
    public class sampler2DRect { }
    public class sampler3D : sampler2D { }
    public class samplerBuffer { }
    public class samplerCube { }
    public class samplerCubeArray { }
    public class samplerCubeArrayShadow { }
    public class sampler1DArrayShadow { }
    public class sampler1DShadow { }
    public class sampler2DArrayShadow { }
    public class sampler2DRectShadow { }
    public class sampler2DShadow { }
    public class samplerCubeShadow { }

    public static partial class Environment
    {
        #region 8.9.1 Texture Query Functions
        public static int textureSize(sampler1D sampler, int lod) { return default; }
        public static ivec2 textureSize(sampler2D sampler, int lod) { return default; }
        public static ivec3 textureSize(sampler3D sampler, int lod) { return default; }
        public static ivec2 textureSize(samplerCube sampler, int lod) { return default; }
        public static int textureSize(sampler1DShadow sampler, int lod) { return default; }
        public static ivec2 textureSize(sampler2DShadow sampler, int lod) { return default; }
        public static ivec2 textureSize(samplerCubeShadow sampler, int lod) { return default; }
        public static ivec3 textureSize(samplerCubeArray sampler, int lod) { return default; }
        public static ivec3 textureSize(samplerCubeArrayShadow sampler, int lod) { return default; }
        public static ivec2 textureSize(sampler2DRect sampler) { return default; }
        public static ivec2 textureSize(sampler2DRectShadow sampler) { return default; }
        public static ivec2 textureSize(sampler1DArray sampler, int lod) { return default; }
        public static ivec3 textureSize(sampler2DArray sampler, int lod) { return default; }
        public static ivec2 textureSize(sampler1DArrayShadow sampler, int lod) { return default; }
        public static ivec3 textureSize(sampler2DArrayShadow sampler, int lod) { return default; }
        public static int textureSize(samplerBuffer sampler) { return default; }
        public static ivec2 textureSize(sampler2DMS sampler) { return default; }
        public static ivec3 textureSize(sampler2DMSArray sampler) { return default; }
        public static vec2 textureQueryLod(sampler1D sampler, Float P) { return default; }
        public static vec2 textureQueryLod(sampler2D sampler, vec2 P) { return default; }
        public static vec2 textureQueryLod(sampler3D sampler, vec3 P) { return default; }
        public static vec2 textureQueryLod(samplerCube sampler, vec3 P) { return default; }
        public static vec2 textureQueryLod(sampler1DArray sampler, Float P) { return default; }
        public static vec2 textureQueryLod(sampler2DArray sampler, vec2 P) { return default; }
        public static vec2 textureQueryLod(samplerCubeArray sampler, vec3 P) { return default; }
        public static vec2 textureQueryLod(sampler1DShadow sampler, Float P) { return default; }
        public static vec2 textureQueryLod(sampler2DShadow sampler, vec2 P) { return default; }
        public static vec2 textureQueryLod(samplerCubeShadow sampler, vec3 P) { return default; }
        public static vec2 textureQueryLod(sampler1DArrayShadow sampler, Float P) { return default; }
        public static vec2 textureQueryLod(sampler2DArrayShadow sampler, vec2 P) { return default; }
        public static vec2 textureQueryLod(samplerCubeArrayShadow sampler, vec3 P) { return default; }
        public static int textureQueryLevels(sampler1D sampler) { return default; }
        public static int textureQueryLevels(sampler2D sampler) { return default; }
        public static int textureQueryLevels(sampler3D sampler) { return default; }
        public static int textureQueryLevels(samplerCube sampler) { return default; }
        public static int textureQueryLevels(sampler1DArray sampler) { return default; }
        public static int textureQueryLevels(sampler2DArray sampler) { return default; }
        public static int textureQueryLevels(samplerCubeArray sampler) { return default; }
        public static int textureQueryLevels(sampler1DShadow sampler) { return default; }
        public static int textureQueryLevels(sampler2DShadow sampler) { return default; }
        public static int textureQueryLevels(samplerCubeShadow sampler) { return default; }
        public static int textureQueryLevels(sampler1DArrayShadow sampler) { return default; }
        public static int textureQueryLevels(sampler2DArrayShadow sampler) { return default; }
        public static int textureQueryLevels(samplerCubeArrayShadow sampler) { return default; }
        #endregion

        #region 8.9.2 Texel Lookup Functions
        public static vec4 texture(sampler1D sampler, Float P, Float bias = default) { return default; }
        public static vec4 texture(sampler2D sampler, vec2 P, Float bias = default) { return default; }
        public static vec4 texture(sampler3D sampler, vec3 P, Float bias = default) { return default; }
        public static vec4 texture(samplerCube sampler, vec3 P, Float bias = default) { return default; }
        public static Float texture(sampler1DShadow sampler, vec3 P, Float bias = default) { return default; }
        public static Float texture(sampler2DShadow sampler, vec3 P, Float bias = default) { return default; }
        public static Float texture(samplerCubeShadow sampler, vec4 P, Float bias = default) { return default; }
        public static vec4 texture(sampler1DArray sampler, vec2 P, Float bias = default) { return default; }
        public static vec4 texture(sampler2DArray sampler, vec3 P, Float bias = default) { return default; }
        public static vec4 texture(samplerCubeArray sampler, vec4 P, Float bias = default) { return default; }
        public static Float texture(sampler1DArrayShadow sampler, vec3 P, Float bias = default) { return default; }
        public static Float texture(sampler2DArrayShadow sampler, vec4 P) { return default; }
        public static vec4 texture(sampler2DRect sampler, vec2 P) { return default; }
        public static Float texture(sampler2DRectShadow sampler, vec3 P) { return default; }
        public static Float texture(samplerCubeArrayShadow sampler, vec4 P, Float compare) { return default; }
        public static vec4 textureProj(sampler1D sampler, vec2 P, Float bias = default) { return default; }
        public static vec4 textureProj(sampler1D sampler, vec4 P, Float bias = default) { return default; }
        public static vec4 textureProj(sampler2D sampler, vec3 P, Float bias = default) { return default; }
        public static vec4 textureProj(sampler2D sampler, vec4 P, Float bias = default) { return default; }
        public static vec4 textureProj(sampler3D sampler, vec4 P, Float bias = default) { return default; }
        public static Float textureProj(sampler1DShadow sampler, vec4 P, Float lod) { return default; }
        public static vec4 textureLod(sampler1D sampler, Float P, Float lod) { return default; }
        public static vec4 textureLod(sampler2D sampler, vec2 P, Float lod) { return default; }
        public static vec4 textureLod(sampler3D sampler, vec3 P, Float lod) { return default; }
        public static vec4 textureLod(samplerCube sampler, vec3 P, Float lod) { return default; }
        public static Float textureLod(sampler1DShadow sampler, vec3 P, Float lod) { return default; }
        public static Float textureLod(sampler2DShadow sampler, vec3 P, Float lod) { return default; }
        public static vec4 textureLod(sampler1DArray sampler, vec2 P, Float lod) { return default; }
        public static vec4 textureLod(sampler2DArray sampler, vec3 P, Float lod) { return default; }
        public static Float textureLod(sampler1DArrayShadow sampler, vec3 P, Float lod) { return default; }
        public static vec4 textureLod(samplerCubeArray sampler, vec4 P, Float lod) { return default; }
        public static vec4 textureOffset(sampler1D sampler, Float P, int offset, Float bias = default) { return default; }
        public static vec4 textureOffset(sampler2D sampler, vec2 P, ivec2 offset, Float bias = default) { return default; }
        public static vec4 textureOffset(sampler3D sampler, vec3 P, ivec3 offset, Float bias = default) { return default; }
        public static vec4 textureOffset(sampler2DRect sampler, vec2 P, ivec2 offset) { return default; }
        public static Float textureOffset(sampler2DRectShadow sampler, vec3 P, ivec2 offset) { return default; }
        public static Float textureOffset(sampler1DShadow sampler, vec3 P, int offset, Float bias = default) { return default; }
        public static Float textureOffset(sampler2DShadow sampler, vec3 P, ivec2 offset, Float bias = default) { return default; }
        public static vec4 textureOffset(sampler1DArray sampler, vec2 P, int offset, Float bias = default) { return default; }
        public static vec4 textureOffset(sampler2DArray sampler, vec3 P, ivec2 offset, Float bias = default) { return default; }
        public static Float textureOffset(sampler1DArrayShadow sampler, vec3 P, int offset, Float bias = default) { return default; }
        public static Float textureOffset(sampler2DArrayShadow sampler, vec4 P, ivec2 offset) { return default; }
        public static vec4 texelFetch(sampler1D sampler, int P, int lod) { return default; }
        public static vec4 texelFetch(sampler2D sampler, ivec2 P, int lod) { return default; }
        public static vec4 texelFetch(sampler3D sampler, ivec3 P, int lod) { return default; }
        public static vec4 texelFetch(sampler2DRect sampler, ivec2 P) { return default; }
        public static vec4 texelFetch(sampler1DArray sampler, ivec2 P, int lod) { return default; }
        public static vec4 texelFetch(sampler2DArray sampler, ivec3 P, int lod) { return default; }
        public static vec4 texelFetch(samplerBuffer sampler, int P) { return default; }
        public static vec4 texelFetch(sampler2DMS sampler, ivec2 P, int sample) { return default; }
        public static vec4 texelFetch(sampler2DMSArray sampler, ivec3 P, int sample) { return default; }
        public static vec4 texelFetchOffset(sampler1D sampler, int P, int lod, int offset) { return default; }
        public static vec4 texelFetchOffset(sampler2D sampler, ivec2 P, int lod, ivec2 offset) { return default; }
        public static vec4 texelFetchOffset(sampler3D sampler, ivec3 P, int lod, ivec3 offset) { return default; }
        public static vec4 texelFetchOffset(sampler2DRect sampler, ivec2 P, ivec2 offset) { return default; }
        public static vec4 texelFetchOffset(sampler1DArray sampler, ivec2 P, int lod, int offset) { return default; }
        public static vec4 texelFetchOffset(sampler2DArray sampler, ivec3 P, int lod, ivec2 offset) { return default; }
        public static vec4 textureProjOffset(sampler1D sampler, vec2 P, int offset, Float bias = default) { return default; }
        public static vec4 textureProjOffset(sampler1D sampler, vec4 P, int offset, Float bias = default) { return default; }
        public static vec4 textureProjOffset(sampler2D sampler, vec3 P, ivec2 offset, Float bias = default) { return default; }
        public static vec4 textureProjOffset(sampler2D sampler, vec4 P, ivec2 offset, Float bias = default) { return default; }
        public static vec4 textureProjOffset(sampler3D sampler, vec4 P, ivec3 offset, Float bias = default) { return default; }
        public static vec4 textureProjOffset(sampler2DRect sampler, vec3 P, ivec2 offset) { return default; }
        public static vec4 textureProjOffset(sampler2DRect sampler, vec4 P, ivec2 offset) { return default; }
        public static Float textureProjOffset(sampler2DRectShadow sampler, vec4 P, ivec2 offset) { return default; }
        public static Float textureProjOffset(sampler1DShadow sampler, vec4 P, int offset, Float bias = default) { return default; }
        public static Float textureProjOffset(sampler2DShadow sampler, vec4 P, ivec2 offset, Float bias = default) { return default; }
        public static vec4 textureLodOffset(sampler1D sampler, Float P, Float lod, int offset) { return default; }
        public static vec4 textureLodOffset(sampler2D sampler, vec2 P, Float lod, ivec2 offset) { return default; }
        public static vec4 textureLodOffset(sampler3D sampler, vec3 P, Float lod, ivec3 offset) { return default; }
        public static Float textureLodOffset(sampler1DShadow sampler, vec3 P, Float lod, int offset) { return default; }
        public static Float textureLodOffset(sampler2DShadow sampler, vec3 P, Float lod, ivec2 offset) { return default; }
        public static vec4 textureLodOffset(sampler1DArray sampler, vec2 P, Float lod, int offset) { return default; }
        public static vec4 textureLodOffset(sampler2DArray sampler, vec3 P, Float lod, ivec2 offset) { return default; }
        public static Float textureLodOffset(sampler1DArrayShadow sampler, vec3 P, Float lod, int offset) { return default; }
        public static vec4 textureProjLod(sampler1D sampler, vec2 P, Float lod) { return default; }
        public static vec4 textureProjLod(sampler1D sampler, vec4 P, Float lod) { return default; }
        public static vec4 textureProjLod(sampler2D sampler, vec3 P, Float lod) { return default; }
        public static vec4 textureProjLod(sampler2D sampler, vec4 P, Float lod) { return default; }
        public static vec4 textureProjLod(sampler3D sampler, vec4 P, Float lod) { return default; }
        public static Float textureProjLod(sampler1DShadow sampler, vec4 P, Float lod) { return default; }
        public static Float textureProjLod(sampler2DShadow sampler, vec4 P, Float lod) { return default; }
        public static vec4 textureProjLodOffset(sampler1D sampler, vec2 P, Float lod, int offset) { return default; }
        public static vec4 textureProjLodOffset(sampler1D sampler, vec4 P, Float lod, int offset) { return default; }
        public static vec4 textureProjLodOffset(sampler2D sampler, vec3 P, Float lod, ivec2 offset) { return default; }
        public static vec4 textureProjLodOffset(sampler2D sampler, vec4 P, Float lod, ivec2 offset) { return default; }
        public static vec4 textureProjLodOffset(sampler3D sampler, vec4 P, Float lod, ivec3 offset) { return default; }
        public static Float textureProjLodOffset(sampler1DShadow sampler, vec4 P, Float lod, int offset) { return default; }
        public static Float textureProjLodOffset(sampler2DShadow sampler, vec4 P, Float lod, ivec2 offset) { return default; }
        public static vec4 textureGrad(sampler1D sampler, Float P, Float dPdx, Float dPdy) { return default; }
        public static vec4 textureGrad(sampler2D sampler, vec2 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureGrad(sampler3D sampler, vec3 P, vec3 dPdx, vec3 dPdy) { return default; }
        public static vec4 textureGrad(samplerCube sampler, vec3 P, vec3 dPdx, vec3 dPdy) { return default; }
        public static vec4 textureGrad(sampler2DRect sampler, vec2 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static Float textureGrad(sampler2DRectShadow sampler, vec3 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static Float textureGrad(sampler1DShadow sampler, vec3 P, Float dPdx, Float dPdy) { return default; }
        public static Float textureGrad(sampler2DShadow sampler, vec3 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static Float textureGrad(samplerCubeShadow sampler, vec4 P, vec3 dPdx, vec3 dPdy) { return default; }
        public static vec4 textureGrad(sampler1DArray sampler, vec2 P, Float dPdx, Float dPdy) { return default; }
        public static vec4 textureGrad(sampler2DArray sampler, vec3 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static Float textureGrad(sampler1DArrayShadow sampler, vec3 P, Float dPdx, Float dPdy) { return default; }
        public static Float textureGrad(sampler2DArrayShadow sampler, vec4 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureGrad(samplerCubeArray sampler, vec4 P, vec3 dPdx, vec3 dPdy) { return default; }
        public static vec4 textureGradOffset(sampler1D sampler, Float P, Float dPdx, Float dPdy, int offset) { return default; }
        public static vec4 textureGradOffset(sampler2D sampler, vec2 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureGradOffset(sampler3D sampler, vec3 P, vec3 dPdx, vec3 dPdy, ivec3 offset) { return default; }
        public static vec4 textureGradOffset(sampler2DRect sampler, vec2 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static Float textureGradOffset(sampler2DRectShadow sampler, vec3 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static Float textureGradOffset(sampler1DShadow sampler, vec3 P, Float dPdx, Float dPdy, int offset) { return default; }
        public static Float textureGradOffset(sampler2DShadow sampler, vec3 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureGradOffset(sampler1DArray sampler, vec2 P, Float dPdx, Float dPdy, int offset) { return default; }
        public static vec4 textureGradOffset(sampler2DArray sampler, vec3 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static Float textureGradOffset(sampler1DArrayShadow sampler, vec3 P, Float dPdx, Float dPdy, int offset) { return default; }
        public static Float textureGradOffset(sampler2DArrayShadow sampler, vec4 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureProjGrad(sampler1D sampler, vec2 P, Float dPdx, Float dPdy) { return default; }
        public static vec4 textureProjGrad(sampler1D sampler, vec4 P, Float dPdx, Float dPdy) { return default; }
        public static vec4 textureProjGrad(sampler2D sampler, vec3 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureProjGrad(sampler2D sampler, vec4 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureProjGrad(sampler3D sampler, vec4 P, vec3 dPdx, vec3 dPdy) { return default; }
        public static vec4 textureProjGrad(sampler2DRect sampler, vec3 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureProjGrad(sampler2DRect sampler, vec4 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static Float textureProjGrad(sampler2DRectShadow sampler, vec4 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static Float textureProjGrad(sampler1DShadow sampler, vec4 P, Float dPdx, Float dPdy) { return default; }
        public static Float textureProjGrad(sampler2DShadow sampler, vec4 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureProjGradOffset(sampler1D sampler, vec2 P, Float dPdx, Float dPdy, int offset) { return default; }
        public static vec4 textureProjGradOffset(sampler1D sampler, vec4 P, Float dPdx, Float dPdy, int offset) { return default; }
        public static vec4 textureProjGradOffset(sampler2D sampler, vec3 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureProjGradOffset(sampler2D sampler, vec4 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureProjGradOffset(sampler2DRect sampler, vec3 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureProjGradOffset(sampler2DRect sampler, vec4 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static Float textureProjGradOffset(sampler2DRectShadow sampler, vec4 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureProjGradOffset(sampler3D sampler, vec4 P, vec3 dPdx, vec3 dPdy, ivec3 offset) { return default; }
        public static Float textureProjGradOffset(sampler1DShadow sampler, vec4 P, Float dPdx, Float dPdy, int offset) { return default; }
        public static Float textureProjGradOffset(sampler2DShadow sampler, vec4 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        #endregion

        #region 8.9.3 Texture Gather Functions
        public static vec4 textureGather(sampler2D sampler, vec2 P, int comp = default) { return default; }
        public static vec4 textureGather(sampler2DArray sampler, vec3 P, int comp = default) { return default; }
        public static vec4 textureGather(samplerCube sampler, vec3 P, int comp = default) { return default; }
        public static vec4 textureGather(samplerCubeArray sampler, vec4 P, int comp = default) { return default; }
        public static vec4 textureGather(sampler2DRect sampler, vec2 P, int comp = default) { return default; }
        public static vec4 textureGather(sampler2DShadow sampler, vec2 P, Float refZ) { return default; }
        public static vec4 textureGather(sampler2DArrayShadow sampler, vec3 P, Float refZ) { return default; }
        public static vec4 textureGather(samplerCubeShadow sampler, vec3 P, Float refZ) { return default; }
        public static vec4 textureGather(samplerCubeArrayShadow sampler, vec4 P, Float refZ) { return default; }
        public static vec4 textureGather(sampler2DRectShadow sampler, vec2 P, Float refZ) { return default; }
        public static vec4 textureGatherOffset(sampler2D sampler, vec2 P, ivec2 offset, int comp = default) { return default; }
        public static vec4 textureGatherOffset(sampler2DArray sampler, vec3 P, ivec2 offset, int comp = default) { return default; }
        public static vec4 textureGatherOffset(sampler2DRect sampler, vec2 P, ivec2 offset, int comp = default) { return default; }
        public static vec4 textureGatherOffset(sampler2DShadow sampler, vec2 P, Float refZ, ivec2 offset) { return default; }
        public static vec4 textureGatherOffset(sampler2DArrayShadow sampler, vec3 P, Float refZ, ivec2 offset) { return default; }
        public static vec4 textureGatherOffset(sampler2DRectShadow sampler, vec2 P, Float refZ, ivec2 offset) { return default; }
        public static vec4 textureGatherOffsets(sampler2D sampler, vec2 P, ivec2[] offsets, int comp = default) { return default; }
        public static vec4 textureGatherOffsets(sampler2DArray sampler, vec3 P, ivec2[] offsets, int comp = default) { return default; }
        public static vec4 textureGatherOffsets(sampler2DRect sampler, vec2 P, ivec2[] offsets, int comp = default) { return default; }
        public static vec4 textureGatherOffsets(sampler2DShadow sampler, vec2 P, Float refZ, ivec2[] offsets) { return default; }
        public static vec4 textureGatherOffsets(sampler2DArrayShadow sampler, vec3 P, Float refZ, ivec2[] offsets) { return default; }
        public static vec4 textureGatherOffsets(sampler2DRectShadow sampler, vec2 P, Float refZ, ivec2[] offsets) { return default; }
        #endregion

        #region 8.9.4 Compatibility Profile Texture Functions
        public static vec4 texture1D(sampler1D sampler, Float coord, Float bias = default) { return default; }
        public static vec4 texture1DProj(sampler1D sampler, vec2 coord, Float bias = default) { return default; }
        public static vec4 texture1DProj(sampler1D sampler, vec4 coord, Float bias = default) { return default; }
        public static vec4 texture1DLod(sampler1D sampler, Float coord, Float lod) { return default; }
        public static vec4 texture1DProjLod(sampler1D sampler, vec2 coord, Float lod) { return default; }
        public static vec4 texture1DProjLod(sampler1D sampler, vec4 coord, Float lod) { return default; }
        public static vec4 texture2D(sampler2D sampler, vec2 coord, Float bias = default) { return default; }
        public static vec4 texture2DProj(sampler2D sampler, vec3 coord, Float bias = default) { return default; }
        public static vec4 texture2DProj(sampler2D sampler, vec4 coord, Float bias = default) { return default; }
        public static vec4 texture2DLod(sampler2D sampler, vec2 coord, Float lod) { return default; }
        public static vec4 texture2DProjLod(sampler2D sampler, vec3 coord, Float lod) { return default; }
        public static vec4 texture2DProjLod(sampler2D sampler, vec4 coord, Float lod) { return default; }
        public static vec4 texture3D(sampler3D sampler, vec3 coord, Float bias = default) { return default; }
        public static vec4 texture3DProj(sampler3D sampler, vec4 coord, Float bias = default) { return default; }
        public static vec4 texture3DLod(sampler3D sampler, vec3 coord, Float lod) { return default; }
        public static vec4 texture3DProjLod(sampler3D sampler, vec4 coord, Float lod) { return default; }
        public static vec4 textureCube(samplerCube sampler, vec3 coord, Float bias = default) { return default; }
        public static vec4 textureCubeLod(samplerCube sampler, vec3 coord, Float lod) { return default; }
        public static vec4 shadow1D(sampler1DShadow sampler, vec3 coord, Float bias = default) { return default; }
        public static vec4 shadow2D(sampler2DShadow sampler, vec3 coord, Float bias = default) { return default; }
        public static vec4 shadow1DProj(sampler1DShadow sampler, vec4 coord, Float bias = default) { return default; }
        public static vec4 shadow2DProj(sampler2DShadow sampler, vec4 coord, Float bias = default) { return default; }
        public static vec4 shadow1DLod(sampler1DShadow sampler, vec3 coord, Float lod) { return default; }
        public static vec4 shadow2DLod(sampler2DShadow sampler, vec3 coord, Float lod) { return default; }
        public static vec4 shadow1DProjLod(sampler1DShadow sampler, vec4 coord, Float lod) { return default; }
        public static vec4 shadow2DProjLod(sampler2DShadow sampler, vec4 coord, Float lod) { return default; }
        #endregion
    }
}
