namespace Shaderlens.Render
{
    using static OpenGL.Gl;

    public interface IRenderPipeline
    {
        int ViewerBufferWidth { get; }
        int ViewerBufferHeight { get; }

        void ClearState();
        void PushState();
        void PopState();

        void SetViewportSize(int viewportWidth, int viewportHeight);
        void SetRenderDownscale(int renderDownscale);
        void SetViewerBufferIndex(int index, int textureIndex);
        void SetViewerPass(ViewerPassSelection selection);

        void SetMouseState(bool isDown);
        void SetViewerTransform(double scale, double offsetX, double offsetY);

        void RenderFrame(FrameIndex frameIndex);
        void RenderViewerFrame();

        void GetTextureSize(int bufferIndex, out int width, out int height);
        void GetTexture(int bufferIndex, int bufferTextureIndex, float[] target);
        void GetViewerTexture(float[] target);
    }

    public class RenderPipeline : IRenderPipeline
    {
        private class ViewportFramebufferResource : IFramebufferResource
        {
            public uint Id { get; }

            public int TexturesCount { get { return 1; } }

            public int Width { get; private set; }
            public int Height { get; private set; }

            public void Dispose()
            {
            }

            public uint GetTextureId(int index)
            {
                return 0;
            }

            public void SetSize(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }

            public void PopState()
            {
            }

            public void PushState()
            {
            }

            public void ClearState()
            {
            }
        }

        private readonly struct State
        {
            public readonly int ViewportWidth;
            public readonly int ViewportHeight;
            public readonly int RenderDownscale;
            public readonly int ViewerBufferIndex;
            public readonly int ViewerBufferTextureIndex;
            public readonly double ViewerScale;
            public readonly double ViewerOffsetX;
            public readonly double ViewerOffsetY;
            public readonly ViewerPassSelection ViewerPassSelection;

            public State(int viewportWidth, int viewportHeight, int renderDownscale, int viewerBufferIndex, int viewerBufferTextureIndex, double viewerScale, double viewerOffsetX, double viewerOffsetY, ViewerPassSelection viewerPassSelection)
            {
                this.ViewportWidth = viewportWidth;
                this.ViewportHeight = viewportHeight;
                this.RenderDownscale = renderDownscale;
                this.ViewerBufferIndex = viewerBufferIndex;
                this.ViewerBufferTextureIndex = viewerBufferTextureIndex;
                this.ViewerScale = viewerScale;
                this.ViewerOffsetX = viewerOffsetX;
                this.ViewerOffsetY = viewerOffsetY;
                this.ViewerPassSelection = viewerPassSelection;
            }
        }

        public int ViewerBufferWidth { get; private set; }
        public int ViewerBufferHeight { get; private set; }

        private readonly IThreadAccess threadAccess;
        private readonly IRenderProgram[] passPrograms;
        private readonly IViewerPassCollection viewerPassPrograms;
        private readonly IFramebufferResource viewerCopyFramebuffer;
        private readonly IKeyboardTextureResource keyboardTextureResource;
        private readonly IMouseStateSource mouseStateSource;
        private readonly RenderContext renderContext;
        private readonly IReadWriteFramebufferResources[] framebuffers;
        private readonly IRenderSizeSource[] framebufferSizes;
        private readonly Queue<State> states;
        private readonly ViewportFramebufferResource viewportFramebuffer;
        private readonly object locker;
        private IRenderProgram viewerPass;
        private bool clearState;
        private bool sizeChanged;
        private bool viewerTransformChanged;
        private int viewportWidth;
        private int viewportHeight;
        private int renderDownscale;
        private double viewerScale;
        private double viewerOffsetX;
        private double viewerOffsetY;
        private bool isMouseDown;
        private bool viewerBufferIndexChanged;
        private int viewerBufferIndex;
        private int viewerBufferTextureIndex;
        private bool viewerPassSelectionChanged;
        private ViewerPassSelection viewerPassSelection;
        private int framebuffersStateDepth;

        public RenderPipeline(IThreadAccess threadAccess, IRenderProgram[] passPrograms, IReadWriteFramebufferResources[] framebuffers, IRenderSizeSource[] framebufferSizes, IViewerPassCollection viewerPassPrograms, IFramebufferResource viewerCopyFramebuffer, IKeyboardTextureResource keyboardTextureResource, IMouseStateSource mouseStateSource)
        {
            this.locker = new object();
            this.threadAccess = threadAccess;
            this.passPrograms = passPrograms;
            this.framebuffers = framebuffers;
            this.framebufferSizes = framebufferSizes;
            this.viewerPassPrograms = viewerPassPrograms;
            this.viewerCopyFramebuffer = viewerCopyFramebuffer;
            this.keyboardTextureResource = keyboardTextureResource;
            this.mouseStateSource = mouseStateSource;
            this.renderContext = new RenderContext();
            this.viewerPass = viewerPassPrograms.ValuesOverlay;
            this.viewportFramebuffer = new ViewportFramebufferResource();
            this.states = new Queue<State>();
            this.renderDownscale = 1;
            this.viewerPassSelection = ViewerPassSelection.ValuesOverlay;
        }

        public void ClearState()
        {
            lock (this.locker)
            {
                this.clearState = true;
            }
        }

        public void PushState()
        {
            lock (this.locker)
            {
                this.states.Enqueue(new State(this.viewportWidth, this.viewportHeight, this.renderDownscale, this.viewerBufferIndex, this.viewerBufferTextureIndex, this.viewerScale, this.viewerOffsetX, this.viewerOffsetY, this.viewerPassSelection));
            }
        }

        public void PopState()
        {
            lock (this.locker)
            {
                var state = this.states.Dequeue();

                SetViewportSize(state.ViewportWidth, state.ViewportHeight);
                SetRenderDownscale(state.RenderDownscale);
                SetViewerBufferIndex(state.ViewerBufferIndex, state.ViewerBufferTextureIndex);
                SetViewerTransform(state.ViewerScale, state.ViewerOffsetX, state.ViewerOffsetY);
                SetViewerPass(state.ViewerPassSelection);
            }
        }

        public void SetViewportSize(int viewportWidth, int viewportHeight)
        {
            lock (this.locker)
            {
                this.sizeChanged = true;
                this.viewerTransformChanged = true;
                this.viewportWidth = viewportWidth;
                this.viewportHeight = viewportHeight;
                this.viewportFramebuffer.SetSize(viewportWidth, viewportHeight);
                SetSize();
            }
        }

        public void SetRenderDownscale(int renderDownscale)
        {
            lock (this.locker)
            {
                this.sizeChanged = true;
                this.viewerTransformChanged = true;
                this.renderDownscale = renderDownscale;
                SetSize();
            }
        }

        public void SetViewerBufferIndex(int index, int textureIndex)
        {
            lock (this.locker)
            {
                this.viewerBufferIndexChanged = true;
                this.viewerBufferIndex = index;
                this.viewerBufferTextureIndex = textureIndex;
                SetSize();
            }
        }

        public void SetMouseState(bool isDown)
        {
            lock (this.locker)
            {
                this.isMouseDown = isDown;
            }
        }

        public void SetViewerTransform(double scale, double offsetX, double offsetY)
        {
            lock (this.locker)
            {
                this.viewerTransformChanged = true;
                this.viewerScale = scale;
                this.viewerOffsetX = offsetX;
                this.viewerOffsetY = offsetY;
            }
        }

        public void SetViewerPass(ViewerPassSelection selection)
        {
            lock (this.locker)
            {
                this.viewerPassSelectionChanged = true;
                this.viewerPassSelection = selection;
            }
        }

        public void RenderFrame(FrameIndex frameIndex)
        {
            this.threadAccess.Verify();

            SetState();

            this.keyboardTextureResource.SetFrameContent(frameIndex);

            glViewport(0, 0, this.viewportWidth, this.viewportHeight);

            this.renderContext.FrameIndex = frameIndex;

            for (var i = 0; i < this.passPrograms.Length; i++)
            {
                var program = this.passPrograms[i];
                var framebuffer = this.framebuffers[i];

                program.Render(this.renderContext, framebuffer.WriteFramebuffer);
                framebuffer.SwapBuffers();
            }

            this.viewerPass.Render(this.renderContext, this.viewportFramebuffer);
        }

        public void RenderViewerFrame()
        {
            this.threadAccess.Verify();

            SetState();

            glViewport(0, 0, this.viewportWidth, this.viewportHeight);

            this.viewerPass.Render(this.renderContext, this.viewportFramebuffer);
        }

        public void GetTextureSize(int bufferIndex, out int width, out int height)
        {
            lock (this.locker)
            {
                this.framebufferSizes[bufferIndex].GetSize(this.viewportWidth, this.viewportHeight, this.renderDownscale, out width, out height);
            }
        }

        public void GetTexture(int bufferIndex, int bufferTextureIndex, float[] target)
        {
            this.threadAccess.Verify();

            var source = this.framebuffers[bufferIndex].ReadFramebuffer;

            glBindTexture(GL_TEXTURE_2D, source.GetTextureId(bufferTextureIndex));
            glGetTexImage(GL_TEXTURE_2D, 0, GL_RGBA, GL_FLOAT, target);
            glBindTexture(GL_TEXTURE_2D, 0);
        }

        public void GetViewerTexture(float[] target)
        {
            this.threadAccess.Verify();

            lock (this.locker)
            {
                this.viewerCopyFramebuffer.SetSize(this.viewportWidth, this.viewportHeight);
                this.viewerPass.Render(this.renderContext, this.viewerCopyFramebuffer);
            }

            glBindTexture(GL_TEXTURE_2D, this.viewerCopyFramebuffer.GetTextureId(0));
            glGetTexImage(GL_TEXTURE_2D, 0, GL_RGBA, GL_FLOAT, target);
            glBindTexture(GL_TEXTURE_2D, 0);
        }

        private void SetState()
        {
            lock (this.locker)
            {
                while (this.framebuffersStateDepth < this.states.Count)
                {
                    for (var i = 0; i < this.framebuffers.Length; i++)
                    {
                        this.framebuffers[i].PushState();
                    }

                    this.mouseStateSource.PushState();
                    this.keyboardTextureResource.PushState();

                    this.framebuffersStateDepth++;
                }

                while (this.framebuffersStateDepth > this.states.Count)
                {
                    for (var i = 0; i < this.framebuffers.Length; i++)
                    {
                        this.framebuffers[i].PopState();
                    }

                    this.mouseStateSource.PopState();
                    this.keyboardTextureResource.PopState();

                    this.framebuffersStateDepth--;
                }

                if (this.clearState)
                {
                    for (var i = 0; i < this.framebuffers.Length; i++)
                    {
                        this.framebuffers[i].ClearState();
                    }

                    this.mouseStateSource.ClearState();
                    this.keyboardTextureResource.ClearState();
                }

                if (this.sizeChanged)
                {
                    this.mouseStateSource.TranslatePosition(this.viewportWidth, this.viewportHeight, this.renderDownscale);

                    for (var i = 0; i < this.framebuffers.Length; i++)
                    {
                        this.framebufferSizes[i].GetSize(this.viewportWidth, this.viewportHeight, this.renderDownscale, out var width, out var height);
                        this.framebuffers[i].SetSize(width, height);
                    }
                }

                this.mouseStateSource.SetNextState(this.isMouseDown);
                this.renderContext.MouseX = this.mouseStateSource.MouseX;
                this.renderContext.MouseY = this.mouseStateSource.MouseY;
                this.renderContext.MousePressedX = this.mouseStateSource.MousePressedX;
                this.renderContext.MousePressedY = this.mouseStateSource.MousePressedY;
                this.renderContext.IsMouseDown = this.mouseStateSource.IsMouseDown;
                this.renderContext.IsMousePressed = this.mouseStateSource.IsMousePressed;

                if (this.viewerTransformChanged)
                {
                    this.renderContext.ViewerScale = (float)this.viewerScale;
                    this.renderContext.ViewerOffsetX = (float)this.viewerOffsetX;
                    this.renderContext.ViewerOffsetY = (float)this.viewerOffsetY;
                }

                if (this.viewerBufferIndexChanged)
                {
                    this.renderContext.ViewerBufferIndex = this.viewerBufferIndex;
                    this.renderContext.ViewerBufferTextureIndex = this.viewerBufferTextureIndex;
                }

                if (this.viewerPassSelectionChanged)
                {
                    this.viewerPass =
                        this.viewerPassSelection.Key != null && this.viewerPassPrograms.TryGetProgram(this.viewerPassSelection.Key, out var program) ? program :
                        this.viewerPassSelection == ViewerPassSelection.None ? this.viewerPassPrograms.Clear : this.viewerPassPrograms.ValuesOverlay;
                }

                this.clearState = false;
                this.sizeChanged = false;
                this.viewerTransformChanged = false;
                this.viewerBufferIndexChanged = false;
                this.viewerPassSelectionChanged = false;
            }
        }

        private void SetSize()
        {
            lock (this.locker)
            {
                this.framebufferSizes[this.viewerBufferIndex].GetSize(this.viewportWidth, this.viewportHeight, this.renderDownscale, out var width, out var height);
                this.ViewerBufferWidth = width;
                this.ViewerBufferHeight = height;
            }
        }
    }
}
