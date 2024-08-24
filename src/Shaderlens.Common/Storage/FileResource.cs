namespace Shaderlens.Storage
{
    public readonly struct FileResourceKey
    {
        public readonly string? DisplayName;
        public readonly string AbsolutePath;
        public readonly DateTime LastWriteTime;
        public readonly bool Exists;

        public FileResourceKey(string displayName)
        {
            this.DisplayName = displayName;
            this.AbsolutePath = null!;
        }

        public FileResourceKey(string absolutePath, DateTime lastWriteTime, bool exists)
        {
            this.AbsolutePath = absolutePath;
            this.LastWriteTime = lastWriteTime;
            this.Exists = exists;
        }

        public override string ToString()
        {
            return this.DisplayName ?? this.AbsolutePath;
        }
    }

    public interface IFileResource<T>
    {
        FileResourceKey Key { get; }
        T Value { get; }
    }

    public class FileResource<T> : IFileResource<T>
    {
        public FileResourceKey Key { get; }
        public T Value { get; }

        public FileResource(FileResourceKey key, T value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
