namespace Shaderlens
{
    public static partial class Environment
    {
        #region 7.1.5. Fragment Shader Special Variables

        public static readonly vec4 gl_FragCoord;
        public static readonly bool gl_FrontFacing;
        public static readonly float[] gl_ClipDistance;
        public static readonly float[] gl_CullDistance;
        public static readonly vec2 gl_PointCoord;
        public static readonly int gl_PrimitiveID;
        public static readonly int gl_SampleID;
        public static readonly vec2 gl_SamplePosition;
        public static readonly int[] gl_SampleMaskIn;
        public static readonly int gl_Layer;
        public static readonly int gl_ViewportIndex;
        public static readonly bool gl_HelperInvocation;
        public static float gl_FragDepth;
        public static int[] gl_SampleMask;

        #endregion

        #region 7.1.6. Compute Shader Special Variables

        // workgroup dimensions
        public static readonly uvec3 gl_NumWorkGroups;
        public static readonly uvec3 gl_WorkGroupSize;

        // workgroup and invocation IDs
        public static readonly uvec3 gl_WorkGroupID;
        public static readonly uvec3 gl_LocalInvocationID;

        // derived variables
        public static readonly uvec3 gl_GlobalInvocationID;
        public static readonly uint gl_LocalInvocationIndex;

        #endregion
    }
}
