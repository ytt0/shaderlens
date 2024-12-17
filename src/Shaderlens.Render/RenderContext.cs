namespace Shaderlens.Render
{
    public interface IRenderContext
    {
        FrameIndex FrameIndex { get; }

        int MouseX { get; }
        int MouseY { get; }
        int MousePressedX { get; }
        int MousePressedY { get; }
        bool IsMouseDown { get; }
        bool IsMousePressed { get; }

        int ViewerBufferIndex { get; }
        int ViewerBufferTextureIndex { get; }
        float ViewerScale { get; }
        float ViewerOffsetX { get; }
        float ViewerOffsetY { get; }
    }

    public class RenderContext : IRenderContext
    {
        public FrameIndex FrameIndex { get; set; }

        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public int MousePressedX { get; set; }
        public int MousePressedY { get; set; }
        public bool IsMouseDown { get; set; }
        public bool IsMousePressed { get; set; }

        public int ViewerBufferIndex { get; set; }
        public int ViewerBufferTextureIndex { get; set; }
        public float ViewerScale { get; set; }
        public float ViewerOffsetX { get; set; }
        public float ViewerOffsetY { get; set; }
    }
}