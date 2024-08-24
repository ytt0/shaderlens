namespace Shaderlens.Render
{
    public interface ITextureWriter
    {
        void Write(ITextureBuffer source, Stream target);
    }

    public interface ITextureWriterFactory
    {
        IEnumerable<string> SupportedExtensions { get; }
        ITextureWriter Create(string extension);
    }
}
