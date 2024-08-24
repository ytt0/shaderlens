namespace Shaderlens.Render.Interop
{
    public static class MarshalExtension
    {
        public static IntPtr AllocHGlobal(byte[] source)
        {
            var handle = Marshal.AllocHGlobal(source.Length * sizeof(byte));
            Marshal.Copy(source, 0, handle, source.Length);
            return handle;
        }

        public static IntPtr AllocHGlobal(float[] source)
        {
            var handle = Marshal.AllocHGlobal(source.Length * sizeof(float));
            Marshal.Copy(source, 0, handle, source.Length);
            return handle;
        }
    }
}
