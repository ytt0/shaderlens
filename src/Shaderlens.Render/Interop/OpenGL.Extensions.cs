namespace OpenGL
{
    using static WinApi;

    [SuppressUnmanagedCodeSecurity]
    public static unsafe partial class Gl
    {
        public const int GL_PROGRAM_BINARY_LENGTH = 0x8741;
        public const int GL_DEBUG_SOURCE_APPLICATION = 0x824A;
        public const int GL_DEBUG_SOURCE_THIRD_PARTY = 0x8249;
        public const int GL_NUM_SHADING_LANGUAGE_VERSIONS = 0x82E9;
        public const int GL_COMPUTE_SHADER = 0x91B9;

        public const uint GL_SHADER_IMAGE_ACCESS_BARRIER_BIT = 0x00000020;

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglCreateContext(IntPtr hWnd);

        [DllImport("opengl32.dll")]
        public static extern bool wglDeleteContext(IntPtr hRC);

        [DllImport("opengl32.dll")]
        public static extern bool wglMakeCurrent(IntPtr hDC, IntPtr hRC);

        [DllImport("opengl32.dll")]
        public static extern bool wglSwapInterval(uint interval);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglGetProcAddress(string funcName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLGETPROGRAMBINARYPROC(uint program, int bufSize, int* length, uint* binaryFormat, void* binary);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLPROGRAMBINARYPROC(uint program, uint binaryFormat, void* binary, int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLPUSHDEBUGGROUP(int source, int id, int length, byte* message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLPOPDEBUGGROUP();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool PFNWGLSWAPINTERVALEXTPROC(int interval);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLDISPATCHCOMPUTEPROC(uint num_groups_x, uint num_groups_y, uint num_groups_z);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLBINDIMAGETEXTUREPROC(uint unit, uint texture, int level, bool layered, int layer, int access, int format);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PFNGLMEMORYBARRIERPROC(uint barriers);

        private static PFNGLGETPROGRAMBINARYPROC? _glGetProgramBinary;
        private static PFNGLPROGRAMBINARYPROC? _glProgramBinary;
        private static PFNGLPUSHDEBUGGROUP? _glPushDebugGroup;
        private static PFNGLPOPDEBUGGROUP? _glPopDebugGroup;
        private static PFNGLDISPATCHCOMPUTEPROC? _glDispatchCompute;
        private static PFNGLBINDIMAGETEXTUREPROC? _glBindImageTexture;
        private static PFNGLMEMORYBARRIERPROC? _glMemoryBarrier;
        private static PFNWGLSWAPINTERVALEXTPROC? _wglSwapIntervalEXT;

        public static void Import()
        {
            var opengl32Module = LoadLibraryA("opengl32.dll");
            if (opengl32Module == IntPtr.Zero)
            {
                throw new Exception($"Load opengl32.dll failed, error: {GetLastError()}");
            }

            var getAnyGLFuncAddress = new GetProcAddressHandler(name =>
            {
                // https://www.khronos.org/opengl/wiki/Load_OpenGL_Functions#Windows
                var procAddress = wglGetProcAddress(name);
                return procAddress != IntPtr.Zero ? procAddress : GetProcAddress(opengl32Module, name);
            });

            Import(getAnyGLFuncAddress);
            ImportExt(getAnyGLFuncAddress);
        }

        private static void ImportExt(GetProcAddressHandler loader)
        {
            _glGetProgramBinary = Marshal.GetDelegateForFunctionPointer<PFNGLGETPROGRAMBINARYPROC>(loader.Invoke("glGetProgramBinary"));
            _glProgramBinary = Marshal.GetDelegateForFunctionPointer<PFNGLPROGRAMBINARYPROC>(loader.Invoke("glProgramBinary"));
            _glPushDebugGroup = Marshal.GetDelegateForFunctionPointer<PFNGLPUSHDEBUGGROUP>(loader.Invoke("glPushDebugGroup"));
            _glPopDebugGroup = Marshal.GetDelegateForFunctionPointer<PFNGLPOPDEBUGGROUP>(loader.Invoke("glPopDebugGroup"));
            _glDispatchCompute = Marshal.GetDelegateForFunctionPointer<PFNGLDISPATCHCOMPUTEPROC>(loader.Invoke("glDispatchCompute"));
            _glBindImageTexture = Marshal.GetDelegateForFunctionPointer<PFNGLBINDIMAGETEXTUREPROC>(loader.Invoke("glBindImageTexture"));
            _glMemoryBarrier = Marshal.GetDelegateForFunctionPointer<PFNGLMEMORYBARRIERPROC>(loader.Invoke("glMemoryBarrier"));
            _wglSwapIntervalEXT = Marshal.GetDelegateForFunctionPointer<PFNWGLSWAPINTERVALEXTPROC>(loader.Invoke("wglSwapIntervalEXT"));
        }

        /// <summary>
        ///     Return a texture image.
        /// </summary>
        /// <param name="target">Specifies the target to which the texture is bound.</param>
        /// <param name="level">Specifies the level-of-detail number of the desired image. Level 0 is the base image level. Level n is the nth mipmap reduction image.</param>
        /// <param name="format">Specifies a pixel format for the returned data. </param>
        /// <param name="type">Specifies a pixel type for the returned data.</param>
        /// <param name="pixels">Returns the texture image. Should be a pointer to an array of the type specified by type.</param>
        public static void glGetTexImage<T>(int target, int level, int format, int type, [NotNull] T[] pixels) where T : struct
        {
            var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            var ptr = handle.AddrOfPinnedObject();
            _glGetTexImage(target, level, format, type, ptr.ToPointer());
            handle.Free();
        }

        /// <summary>
        /// Return a single parameter of a query object.
        /// </summary>
        /// <param name="id">Specifies the name of a query object.</param>
        /// <param name="pname">Specifies the symbolic name of a query object parameter.<para>Accepted values are GL_QUERY_RESULT or GL_QUERY_RESULT_AVAILABLE.</para></param>
        /// <returns>The retrieved values.</returns>
        public static int glGetQueryObject(uint id, int pname)
        {
            int result;
            _glGetQueryObjectiv(id, pname, &result);
            return result;
        }

        public static void glGetProgramBinary(uint program, int bufSize, out int length, out uint binaryFormat, out byte[] binary)
        {
            binary = new byte[bufSize];

            fixed (int* _length = &length)
            fixed (uint* _binaryFormat = &binaryFormat)
            fixed (byte* _binary = &binary[0])
            {
                _glGetProgramBinary!(program, bufSize, _length, _binaryFormat, _binary);
            }
        }

        public static void glProgramBinary(uint program, uint binaryFormat, byte[] binary, int length)
        {
            fixed (byte* _binary = &binary[0])
            {
                _glProgramBinary!(program, binaryFormat, _binary, length);
            }
        }

        public static void glPushDebugGroup(int source, int id, int length, byte* message)
        {
#if DEBUG
            _glPushDebugGroup!(source, id, length, message);
#endif
        }

        public static void glPushDebugGroup(int source, int id, string message)
        {
#if DEBUG
            var buffer = Encoding.UTF8.GetBytes(message);
            fixed (byte* p = &buffer[0])
            {
                var length = buffer.Length;
                glPushDebugGroup(source, id, length, p);
            }
#endif
        }

        public static void glPopDebugGroup()
        {
#if DEBUG
            _glPopDebugGroup!();
#endif
        }

        public static void glDispatchCompute(uint num_groups_x, uint num_groups_y, uint num_groups_z)
        {
            _glDispatchCompute!(num_groups_x, num_groups_y, num_groups_z);
        }

        public static void glBindImageTexture(uint unit, uint texture, int level, bool layered, int layer, int access, int format)
        {
            _glBindImageTexture!(unit, texture, level, layered, layer, access, format);
        }

        public static void glMemoryBarrier(uint barriers)
        {
            _glMemoryBarrier!(barriers);
        }

        public static bool wglSwapIntervalEXT(int interval)
        {
            return _wglSwapIntervalEXT!(interval);
        }
    }
}
