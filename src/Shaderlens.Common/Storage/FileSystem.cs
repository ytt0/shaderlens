namespace Shaderlens.Storage
{
    public interface IFileSystem
    {
        FileResourceKey GetFileKey(string path);
        IEnumerable<string> GetSequencePaths(string pattern);
        IFileResource<string> ReadText(FileResourceKey key);
        IFileResource<byte[]> ReadBytes(FileResourceKey key);
    }

    public static class FileSystemExtensions
    {
        public static IFileResource<string> ReadText(this IFileSystem fileSystem, string path)
        {
            return fileSystem.ReadText(fileSystem.GetFileKey(path));
        }

        public static IFileResource<byte[]> ReadBytes(this IFileSystem fileSystem, string path)
        {
            return fileSystem.ReadBytes(fileSystem.GetFileKey(path));
        }

        public static bool TryReadText(this IFileSystem fileSystem, string path, [MaybeNullWhen(false)] out IFileResource<string> fileResource)
        {
            var key = fileSystem.GetFileKey(path);
            if (key.Exists)
            {
                fileResource = fileSystem.ReadText(key);
                return true;
            }

            fileResource = null;
            return false;
        }

        public static bool TryReadBytes(this IFileSystem fileSystem, string path, [MaybeNullWhen(false)] out IFileResource<byte[]> fileResource)
        {
            var key = fileSystem.GetFileKey(path);
            if (key.Exists)
            {
                fileResource = fileSystem.ReadBytes(key);
                return true;
            }

            fileResource = null;
            return false;
        }

        public static IFileResource<string> TryReadText(this IFileSystem fileSystem, string path)
        {
            var key = fileSystem.GetFileKey(path);
            return key.Exists ? fileSystem.ReadText(key) : new FileResource<string>(key, String.Empty);
        }
    }

    public class FileSystem : IFileSystem
    {
        public FileResourceKey GetFileKey(string path)
        {
            if (!Path.IsPathFullyQualified(path))
            {
                throw new Exception($"Absolute path is expected - {path}");
            }

            var fileInfo = new FileInfo(path);
            return new FileResourceKey(path, fileInfo.LastWriteTime, fileInfo.Exists);
        }

        public IEnumerable<string> GetSequencePaths(string pattern)
        {
            var directoryPath = Path.GetDirectoryName(pattern)!;

            if (!Path.IsPathFullyQualified(directoryPath))
            {
                throw new Exception($"Absolute path is expected: {directoryPath}");
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new StorageException($"Folder not found: {directoryPath}", new FileResourceKey(directoryPath, default, false));
            }

            var fileName = Path.GetFileName(pattern)!;
            if (fileName.Contains('*') || fileName.Contains('?'))
            {
                var paths = Directory.GetFiles(directoryPath, fileName, SearchOption.TopDirectoryOnly).ToList();
                paths.Sort();
                return paths;
            }

            return new[] { pattern };
        }

        public IFileResource<string> ReadText(FileResourceKey key)
        {
            if (!key.Exists)
            {
                throw new StorageException($"File not found: {key.AbsolutePath}", key);
            }

            return new FileResource<string>(key, File.ReadAllText(key.AbsolutePath));
        }

        public IFileResource<byte[]> ReadBytes(FileResourceKey key)
        {
            if (!key.Exists)
            {
                throw new StorageException($"File not found: {key.AbsolutePath}", key);
            }

            return new FileResource<byte[]>(key, File.ReadAllBytes(key.AbsolutePath));
        }
    }
}
