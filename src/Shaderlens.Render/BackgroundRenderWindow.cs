namespace Shaderlens.Render
{
    using static WinApi;

    public interface IBackgroundRenderWindow : IDisposable
    {
        IntPtr Handle { get; }
    }

    public class BackgroundRenderWindow : IBackgroundRenderWindow
    {
        private class WindowClassRegistration : IDisposable
        {
            public string Name { get; }

            private readonly WNDPROC wndProcDelegate;
            private bool isDisposed;

            public WindowClassRegistration(string name)
            {
                this.Name = name;
                this.wndProcDelegate = new WNDPROC(DefWindowProc);

                var wndClass = new WNDCLASSEX
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                    //hInstance = GetModuleHandleA(null),
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate(this.wndProcDelegate),
                    lpszClassName = name,
                };

                ValidateResult(RegisterClassEx(ref wndClass));
            }

            public void Dispose()
            {
                if (!this.isDisposed)
                {
                    this.isDisposed = true;
                    UnregisterClassA(this.Name, IntPtr.Zero);
                }
            }
        }

        private static readonly Lazy<WindowClassRegistration> Registration = new Lazy<WindowClassRegistration>(() => new WindowClassRegistration(nameof(BackgroundRenderWindow)));

        public IntPtr Handle { get; }

        private bool isDisposed;

        public BackgroundRenderWindow()
        {
            this.Handle = ValidateResult(CreateWindowExA(0, Registration.Value.Name, null!, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                ValidateResult(DestroyWindow(this.Handle));
            }
        }
    }
}
