namespace Shaderlens
{
    public class sampler2D { }
    public class image2D { }
    public struct atomic_uint { }
    public struct Double { }

    public static partial class Environment
    {
        public static bool _bool(int v) { return default; }
        public static bool _bool(uint v) { return default; }
        public static bool _bool(Float v) { return default; }
        public static bool bool_(int v) { return default; }
        public static bool bool_(uint v) { return default; }
        public static bool bool_(Float v) { return default; }

        public static int _int(bool v) { return default; }
        public static int _int(uint v) { return default; }
        public static int _int(Float v) { return default; }
        public static int int_(bool v) { return default; }
        public static int int_(uint v) { return default; }
        public static int int_(Float v) { return default; }

        public static uint _uint(bool v) { return default; }
        public static uint _uint(int v) { return default; }
        public static uint _uint(Float v) { return default; }
        public static uint uint_(bool v) { return default; }
        public static uint uint_(int v) { return default; }
        public static uint uint_(Float v) { return default; }

        public static Float _float(bool v) { return default; }
        public static Float _float(int v) { return default; }
        public static Float _float(uint v) { return default; }
        public static Float float_(bool v) { return default; }
        public static Float float_(int v) { return default; }
        public static Float float_(uint v) { return default; }

        public static int length<T>(this T[] array) { return default; }
    }
}
