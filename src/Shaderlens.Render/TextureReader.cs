namespace Shaderlens.Render
{
    public interface ITextureReader
    {
        ITextureResource Read(Stream source);
    }

    public interface ITextureReaderFactory
    {
        ITextureReader Create(string extension);
    }
}
