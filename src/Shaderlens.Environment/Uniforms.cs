namespace Shaderlens
{
    public static partial class Environment
    {
        // image/buffer	The viewport resolution (z is pixel aspect ratio, usually 1.0)
        public static vec3 iResolution = default;
        // image/sound/buffer	Current time in seconds
        public static Float iTime = default;
        // image/buffer	Time it takes to render a frame, in seconds
        public static Float iTimeDelta = default;
        // image/buffer	Current frame
        public static int iFrame = default;
        // image/buffer	Number of frames rendered per second
        public static Float iFrameRate = default;
        // image/buffer	Time for channel (if video or sound), in seconds
        public static Float[] iChannelTime = default;
        // image/buffer	Duration for channel (if video or sound), in seconds
        public static Float[] iChannelDuration = default;
        // image/buffer/sound	Input texture resolution for each channel
        public static vec3[] iChannelResolution = default;
        // image/buffer	xy = current pixel coords (if LMB is down). zw = click pixel
        public static vec4 iMouse = default;
        // image/buffer/sound	Sampler for input textures i
        public static sampler3D iChannel0 = default;
        public static sampler3D iChannel1 = default;
        public static sampler3D iChannel2 = default;
        public static sampler3D iChannel3 = default;
        public static sampler3D iChannel4 = default;
        public static sampler3D iChannel5 = default;
        public static sampler3D iChannel6 = default;
        public static sampler3D iChannel7 = default;
        // image/buffer/sound	Year, month, day, time in seconds in .xyzw
        public static vec4 iDate = default;
        // image/buffer/sound	The sound sample rate (typically 44100)
        //public static Float iSampleRate = default;

        // viewer state
        public static Float iViewerScale = default;
        public static vec2 iViewerOffset = default;
    }
}
