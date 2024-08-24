namespace Shaderlens.Render.Project
{
    public class ProjectLoaderFileSystem : IFileSystem
    {
        private readonly IFileSystem fileSystem;
        private readonly IPathResolver pathResolver;
        private readonly IProjectResources resources;
        private readonly IFileMonitor fileMonitor;
        private readonly Dictionary<string, FileResourceKey> fileKeys;
        private readonly Dictionary<string, IEnumerable<string>> sequencesPaths;

        public ProjectLoaderFileSystem(IFileSystem fileSystem, IPathResolver pathResolver, IProjectResources resources, IFileMonitor fileMonitor)
        {
            this.fileSystem = fileSystem;
            this.pathResolver = pathResolver;
            this.resources = resources;
            this.fileMonitor = fileMonitor;
            this.fileKeys = new Dictionary<string, FileResourceKey>();
            this.sequencesPaths = new Dictionary<string, IEnumerable<string>>();
        }

        public FileResourceKey GetFileKey(string path)
        {
            path = this.pathResolver.ResolvePath(path);

            if (!this.fileKeys.TryGetValue(path, out var key))
            {
                key = this.fileSystem.GetFileKey(path);
                this.fileKeys.Add(path, key);
            }

            this.fileMonitor.Set(key);

            return key;
        }

        public IEnumerable<string> GetSequencePaths(string pattern)
        {
            pattern = this.pathResolver.ResolvePath(pattern);

            if (!this.sequencesPaths.TryGetValue(pattern, out var paths))
            {
                paths = this.fileSystem.GetSequencePaths(pattern);
                this.sequencesPaths.Add(pattern, paths);
            }

            return paths;
        }

        public IFileResource<string> ReadText(FileResourceKey key)
        {
            if (!this.resources.TryGetResource(key, out var resource))
            {
                resource = this.fileSystem.ReadText(key);
                this.resources.AddResource(key, resource);
            }

            return resource as IFileResource<string> ?? throw new StorageException($"{key} content has already been loaded as a non text type", key);
        }

        public IFileResource<byte[]> ReadBytes(FileResourceKey key)
        {
            if (!this.resources.TryGetResource(key, out var resource))
            {
                resource = this.fileSystem.ReadBytes(key);
                this.resources.AddResource(key, resource);
            }

            return resource as IFileResource<byte[]> ?? throw new StorageException($"{key} content has already been loaded as a non binary type", key);
        }
    }
}
