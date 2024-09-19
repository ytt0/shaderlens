namespace Shaderlens.Presentation.Extensions
{
    public static class DrawingContextExtensions
    {
        public struct ScopeToken : IDisposable
        {
            private DrawingContext drawingContext;

            public ScopeToken(DrawingContext drawingContext)
            {
                this.drawingContext = drawingContext;
            }

            public void Dispose()
            {
                this.drawingContext.Pop();
            }
        }

        public static ScopeToken PushOpacityScope(this DrawingContext drawingContext, double opacity)
        {
            drawingContext.PushOpacity(opacity);
            return new ScopeToken(drawingContext);
        }

        public static ScopeToken PushTransformScope(this DrawingContext drawingContext, Transform transform)
        {
            drawingContext.PushTransform(transform);
            return new ScopeToken(drawingContext);
        }
    }
}
