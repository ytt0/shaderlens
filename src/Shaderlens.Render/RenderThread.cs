namespace Shaderlens.Render
{
    using static OpenGL.Gl;
    using static WinApi;

    public interface IRenderThread : IDispatcherThread
    {
        string? RenderInformation { get; }

        void Start();
        void Stop();

        void ClearPipeline();
        void SetPipeline(IRenderPipeline pipeline);
        void SetFrameRate(int frameRate);
        void PausePipeline();
        void ResumePipeline();
        void RenderFrame();
        void RenderViewerFrame();
    }

    public class RenderThread : IRenderThread
    {
        private class RenderThreadAccess : IThreadAccess
        {
            private readonly string threadName;
            private ThreadAccess? threadAccess;

            public RenderThreadAccess(string threadName)
            {
                this.threadName = threadName;
            }

            public void Initialize(int managedThreadId)
            {
                this.threadAccess = new ThreadAccess(this.threadName, managedThreadId);
            }

            public void Verify()
            {
#if DEBUG
                if (this.threadAccess == null)
                {
                    throw new InvalidOperationException($"{this.threadName} thread has not been started");
                }

                this.threadAccess.Verify();
#endif
            }
        }

        private const int QueriesCount = 20;

        public event EventHandler<Exception>? OnException;

        public IThreadAccess Access { get { return this.threadAccess; } }

        public string? RenderInformation { get; private set; }

        private readonly ManualResetEventSlim startedEvent;
        private readonly ManualResetEventSlim stoppedEvent;
        private readonly ManualResetEventSlim waitingEvent;
        private readonly ManualResetEventSlim continueEvent;
        private readonly Thread thread;
        private readonly RenderThreadAccess threadAccess;
        private readonly IntPtr hWindow;
        private readonly IFrameIndexSource frameIndexSource;
        private readonly IFrameRateCounter counter;
        private readonly object locker;
        private IRenderPipeline? pipeline;
        private readonly List<Action> actions;
        private bool isStopRequested;
        private bool isPaused;
        private bool isStarted;
        private bool isViewerFrame;
        private bool isFullFrame;
        private int frameRate;

        public RenderThread(IntPtr hWindow, IFrameIndexSource frameIndexSource, IFrameRateCounter counter, string name)
        {
            this.hWindow = hWindow;
            this.frameIndexSource = frameIndexSource;
            this.counter = counter;

            this.startedEvent = new ManualResetEventSlim(false);
            this.stoppedEvent = new ManualResetEventSlim(false);
            this.waitingEvent = new ManualResetEventSlim(false);
            this.continueEvent = new ManualResetEventSlim(false);

            this.thread = new Thread(Run) { Name = name };
            this.threadAccess = new RenderThreadAccess(name);
            this.frameRate = 1;

            this.locker = new object();
            this.actions = new List<Action>();

        }

        public void Dispose()
        {
            if (this.thread.IsAlive)
            {
                throw new Exception("Thread has not been stopped");
            }

            this.startedEvent.Dispose();
            this.stoppedEvent.Dispose();
            this.waitingEvent.Dispose();
            this.continueEvent.Dispose();
        }

        public void Start()
        {
            lock (this.locker)
            {
                if (this.isStarted || this.isStopRequested)
                {
                    return;
                }

                this.isStarted = true;
            }

            this.thread.Start();
            this.continueEvent.Set();
            this.startedEvent.Wait();
        }

        public void Stop()
        {
            lock (this.locker)
            {
                if (!this.isStarted || this.isStopRequested)
                {
                    return;
                }

                this.isStopRequested = true;
                this.continueEvent.Set();
            }

            this.stoppedEvent.Wait();
            this.thread.Join();
        }

        public void DispatchAsync(Action action)
        {
            if (this.thread.ManagedThreadId == Environment.CurrentManagedThreadId)
            {
                action();
                return;
            }

            lock (this.locker)
            {
                if (!this.isStarted || this.isStopRequested)
                {
                    throw new Exception("RenderThread is not running");
                }

                this.actions.Add(action);
                this.continueEvent.Set();
            }
        }

        public void ClearPipeline()
        {
            lock (this.locker)
            {
                this.pipeline = null;
            }
        }

        public void SetPipeline(IRenderPipeline pipeline)
        {
            lock (this.locker)
            {
                this.pipeline = pipeline;
                this.continueEvent.Set();
            }
        }

        public void PausePipeline()
        {
            lock (this.locker)
            {
                if (this.isPaused)
                {
                    return;
                }

                this.isPaused = true;
                this.waitingEvent.Reset();
            }

            this.waitingEvent.Wait();
        }

        public void ResumePipeline()
        {
            lock (this.locker)
            {
                this.isPaused = false;
                this.continueEvent.Set();
            }
        }

        public void RenderFrame()
        {
            lock (this.locker)
            {
                this.isFullFrame = true;
                this.continueEvent.Set();
            }
        }

        public void RenderViewerFrame()
        {
            lock (this.locker)
            {
                this.isViewerFrame = true;
                this.continueEvent.Set();
            }
        }

        public void SetFrameRate(int frameRate)
        {
            lock (this.locker)
            {
                if (this.frameRate != frameRate)
                {
                    this.frameRate = frameRate;
                    DispatchAsync(() => wglSwapIntervalEXT(this.frameRate));
                }
            }
        }

        private void Run()
        {
            try
            {
                var pfd = new PIXELFORMATDESCRIPTOR
                {
                    nSize = (short)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>(), // size of this pfd
                    nVersion = 1, // version number
                    dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER, // support window | support OpenGL | double buffered
                    iPixelType = PFD_TYPE_RGBA, // RGBA type
                    cColorBits = 24, // 24-bit color depth
                    cRedBits = 0, // color bits ignored
                    cRedShift = 0,
                    cGreenBits = 0,
                    cGreenShift = 0,
                    cBlueBits = 0,
                    cBlueShift = 0,
                    cAlphaBits = 0, // no alpha buffer
                    cAlphaShift = 0, // shift bit ignored
                    cAccumBits = 0, // no accumulation buffer
                    cAccumRedBits = 0, // accum bits ignored
                    cAccumGreenBits = 0,
                    cAccumBlueBits = 0,
                    cAccumAlphaBits = 0,
                    cDepthBits = 32, // 32-bit z-buffer
                    cStencilBits = 0, // no stencil buffer
                    cAuxBuffers = 0, // no auxiliary buffer
                    iLayerType = PFD_MAIN_PLANE, // main layer
                    bReserved = 0, // reserved
                    dwLayerMask = 0, // layer masks ignored
                    dwVisibleMask = 0,
                    dwDamageMask = 0
                };

                this.threadAccess.Initialize(Environment.CurrentManagedThreadId);

                var hDC = ValidateResult(GetDC(this.hWindow));
                var format = ChoosePixelFormat(hDC, ref pfd);
                ValidateResult(SetPixelFormat(hDC, format, ref pfd));

                var hRC = ValidateResult(wglCreateContext(hDC));
                ValidateResult(wglMakeCurrent(hDC, hRC));

                hRC = TryReplaceContext(hDC, hRC, 4, 5);

                Import();

                this.RenderInformation = GetRenderInformation();

                wglSwapIntervalEXT(this.frameRate);

                this.startedEvent.Set();

                var queries = glGenQueries(QueriesCount);
                var queryIndex = 0;

                while (true)
                {
                    this.waitingEvent.Set();
                    this.continueEvent.Wait();

                    IRenderPipeline? pipeline = null;
                    Action[]? actions = null;
                    var isFullFrame = false;
                    var isViewerFrame = false;
                    var isPaused = false;

                    lock (this.locker)
                    {
                        if (this.isStopRequested)
                        {
                            break;
                        }

                        pipeline = this.pipeline;

                        if (this.actions.Count > 0)
                        {
                            actions = this.actions.ToArray();
                            this.actions.Clear();
                        }

                        if (this.isPaused || this.pipeline == null)
                        {
                            this.isPaused = true;
                            this.continueEvent.Reset();
                        }

                        isFullFrame = this.isFullFrame && this.isPaused;
                        isViewerFrame = this.isViewerFrame && !this.isFullFrame && this.isPaused;
                        isPaused = this.isPaused;

                        this.isViewerFrame = false;
                        this.isFullFrame = false;
                    }

                    if (actions != null)
                    {
                        foreach (var action in actions)
                        {
                            action();
                        }
                    }

                    if (pipeline != null && (!isPaused || isFullFrame || isViewerFrame))
                    {
                        if (isViewerFrame)
                        {
                            pipeline.RenderViewerFrame();
                        }
                        else
                        {
                            var frameIndex = this.frameIndexSource.GetNextFrame();

                            glPushDebugGroup(GL_DEBUG_SOURCE_APPLICATION, 0, "Pipeline");
                            glBeginQuery(GL_TIME_ELAPSED, queries[queryIndex]);

                            pipeline.RenderFrame(frameIndex);

                            glEndQuery(GL_TIME_ELAPSED);
                            glPopDebugGroup();

                            if (isPaused)
                            {
                                glFinish();
                            }
                            else
                            {
                                queryIndex = (queryIndex + 1) % QueriesCount;
                            }

                            var elapsedTime = glGetQueryObject(queries[queryIndex], GL_QUERY_RESULT_AVAILABLE) == 1 ?
                                glGetQueryObject(queries[queryIndex], GL_QUERY_RESULT) : 0;
                            this.counter.RecordFrame(frameIndex, elapsedTime / (int)TimeSpan.NanosecondsPerTick, isPaused);
                        }

                        SwapBuffers(hDC);
                    }
                }

                foreach (var query in queries)
                {
                    glDeleteQuery(query);
                }

                wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                ReleaseDC(hDC, this.hWindow);
                wglDeleteContext(hRC);

                this.stoppedEvent.Set();
            }
            catch (Exception e)
            {
                this.OnException?.Invoke(this, e);
                this.startedEvent.Set();
                this.waitingEvent.Set();
                this.stoppedEvent.Set();
            }
        }

        private static IntPtr TryReplaceContext(IntPtr hDC, IntPtr hRC, int majorVersion, int minorVersion)
        {
            var attribList = new int[]
            {
                WGL_CONTEXT_MAJOR_VERSION_ARB, majorVersion,
                WGL_CONTEXT_MINOR_VERSION_ARB, minorVersion,
                WGL_CONTEXT_PROFILE_MASK_ARB,  WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
                0,
            };

            var delegatePtr = wglGetProcAddress("wglCreateContextAttribsARB");
            if (delegatePtr != IntPtr.Zero)
            {
                var wglCreateContextAttribsARB = Marshal.GetDelegateForFunctionPointer<PFNWGLCREATECONTEXTATTRIBSARB>(delegatePtr);

                var newContext = wglCreateContextAttribsARB(hDC, IntPtr.Zero, attribList);
                if (newContext != IntPtr.Zero)
                {
                    wglDeleteContext(hRC);
                    ValidateResult(wglMakeCurrent(hDC, newContext));
                    return newContext;
                }
            }

            return hRC;
        }

        private static string GetRenderInformation()
        {
            var renderInformation = new StringBuilder();

            renderInformation.AppendLine($"Vendor: {glGetString(GL_VENDOR)}");
            renderInformation.AppendLine($"Renderer: {glGetString(GL_RENDERER)}");
            renderInformation.AppendLine($"Version: {glGetString(GL_VERSION)}");
            renderInformation.AppendLine($"ShadingLanguageVersion: {glGetString(GL_SHADING_LANGUAGE_VERSION)}");

            return renderInformation.ToString();
        }
    }
}
