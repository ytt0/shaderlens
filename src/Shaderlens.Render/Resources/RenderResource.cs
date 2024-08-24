namespace Shaderlens.Render.Resources
{
    public interface IRenderResource : IDisposable
    {
        uint Id { get; }
    }
}
