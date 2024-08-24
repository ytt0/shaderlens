namespace Shaderlens.Render
{
    using static OpenGL.Gl;

    public interface ISequenceRenderTask : IDisposable
    {
        // progress event
        // complete event

        void Start();
        void Stop();
        void Pause();
        void Resume();
    }

    public interface ISequenceRenderListener
    {
        void SetProgress(int index, ITextureBuffer? textureBuffer);
        void SetCompleted();
    }

    public class RenderSequenceTask : ISequenceRenderTask
    {
        private readonly ManualResetEventSlim startedEvent;
        private readonly ManualResetEventSlim stoppedEvent;
        private readonly ManualResetEventSlim waitingEvent;
        private readonly ManualResetEventSlim continueEvent;
        private readonly IRenderThread renderThread;
        private readonly IDispatcherThread viewThread;
        private readonly IRenderPipeline pipeline;
        private readonly ISequenceRenderListener listener;
        private readonly ITextureWriterFactory textureWriterFactory;
        private readonly RenderSequenceSettings settings;
        private readonly int endFrame;
        private readonly IFormattedIndexedPath targetPath;
        private readonly object locker;
        private bool isPaused;
        private bool isStarted;
        private bool isStopRequested;

        public RenderSequenceTask(IRenderThread renderThread, IDispatcherThread viewThread, IRenderPipeline pipeline, ISequenceRenderListener listener, ITextureWriterFactory textureWriterFactory, RenderSequenceSettings settings, string projectPath)
        {
            this.startedEvent = new ManualResetEventSlim(false);
            this.stoppedEvent = new ManualResetEventSlim(false);
            this.waitingEvent = new ManualResetEventSlim(false);
            this.continueEvent = new ManualResetEventSlim(false);

            this.locker = new object();
            this.renderThread = renderThread;
            this.viewThread = viewThread;
            this.pipeline = pipeline;
            this.listener = listener;
            this.textureWriterFactory = textureWriterFactory;
            this.settings = settings;
            this.endFrame = settings.StartFrame + settings.FrameCount - 1;
            this.targetPath = FormattedIndexedPath.Create(projectPath!, settings.TargetPath!, settings.StartFrame, settings.FrameCount);
        }

        public void Dispose()
        {
            Stop();

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

            this.renderThread.DispatchAsync(Run);
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
        }

        public void Pause()
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

        public void Resume()
        {
            lock (this.locker)
            {
                this.isPaused = false;
                this.continueEvent.Set();
            }
        }

        private void Run()
        {
            try
            {
                this.renderThread.VerifyAccess();

                this.pipeline.PushState();
                this.pipeline.ClearState();
                this.pipeline.SetViewportSize(this.settings.RenderSize.Width, this.settings.RenderSize.Height);
                this.pipeline.SetViewerBufferIndex(this.settings.BufferIndex);
                this.pipeline.SetViewerPass(ViewerPassSelection.None);
                this.pipeline.SetRenderDownscale(1);

                var startDateTime = DateTime.Now;
                var lastPreviewFrameTime = DateTime.MinValue;

                var index = this.settings.Prerender ? 0 : this.settings.StartFrame;
                var endIndex = this.settings.StartFrame + this.settings.FrameCount;
                var indexOffset = this.settings.RelativeIndex ? this.settings.StartFrame : 0;

                var delta = TimeSpan.TicksPerSecond / this.settings.FrameRate;

                this.pipeline.GetTextureSize(this.settings.BufferIndex, out var width, out var height);
                var buffer = new float[width * height * 4];
                var textureBuffer = new TextureBuffer(buffer, width, height);

                this.startedEvent.Set();

                Directory.CreateDirectory(Path.GetDirectoryName(this.settings.TargetPath)!);
                var extension = Path.GetExtension(this.settings.TargetPath!);

                while (index < endIndex)
                {
                    this.waitingEvent.Set();
                    this.continueEvent.Wait();

                    lock (this.locker)
                    {
                        if (this.isStopRequested)
                        {
                            break;
                        }

                        if (this.isPaused)
                        {
                            this.continueEvent.Reset();
                            continue;
                        }
                    }

                    var time = TimeSpan.FromSeconds((double)index / this.settings.FrameRate);

                    var frameIndex = new FrameIndex
                    {
                        Index = index,
                        Time = time.Ticks,
                        Delta = delta,
                        DateTime = startDateTime + time
                    };

                    this.pipeline.RenderFrame(frameIndex);
                    glFinish();

                    var now = DateTime.Now;
                    var readPreviewFrame = index == 0 || index == this.settings.StartFrame || index == this.endFrame || lastPreviewFrameTime.AddSeconds(1) < now;
                    var writeFrame = index >= this.settings.StartFrame;
                    var readFrame = readPreviewFrame || writeFrame;

                    if (readPreviewFrame)
                    {
                        lastPreviewFrameTime = now;
                    }

                    // test
                    //writeFrame = false;

                    if (readFrame)
                    {
                        this.pipeline.GetTexture(this.settings.BufferIndex, buffer);
                    }

                    if (writeFrame)
                    {
                        var path = this.targetPath.GetPath(index - indexOffset);
                        using (var fileStream = File.Open(path, this.settings.OverridePath ? FileMode.Create : FileMode.CreateNew, FileAccess.Write))
                        {
                            var textureWriter = this.textureWriterFactory.Create(extension);
                            textureWriter.Write(textureBuffer, fileStream);
                        }
                    }

                    var previewIndex = index;
                    var previewTextureBuffer = readPreviewFrame ? new TextureBuffer(textureBuffer.Buffer.ToArray(), textureBuffer.Width, textureBuffer.Height) : null;
                    this.viewThread.DispatchAsync(() => this.listener.SetProgress(previewIndex, previewTextureBuffer));

                    index++;

                    if (index % 10 == 0)
                    {
                        GC.Collect();
                    }
                }

                this.viewThread.DispatchAsync(this.listener.SetCompleted);
                this.pipeline.PopState();

                this.stoppedEvent.Set();
            }
            catch
            {
                this.startedEvent.Set();
                this.waitingEvent.Set();
                this.stoppedEvent.Set();
            }
        }
    }
}
