namespace Shaderlens.Storage
{
    public interface IFileMonitor : IDisposable
    {
        event EventHandler? Changed;

        void Clear();
        void Set(FileResourceKey fileKey);
    }

    public class FileMonitor : IFileMonitor
    {
        public event EventHandler? Changed;

        private readonly IDispatcherThread thread;
        private readonly IFileSystem fileSystem;
        private readonly Dictionary<string, FileResourceKey> files;
        private readonly Dictionary<string, FileSystemWatcher> directoryWatchers;
        private readonly object locker;

        public FileMonitor(IDispatcherThread thread, IFileSystem fileSystem)
        {
            this.thread = thread;
            this.fileSystem = fileSystem;
            this.directoryWatchers = new Dictionary<string, FileSystemWatcher>();
            this.files = new Dictionary<string, FileResourceKey>();
            this.locker = new object();
        }

        public void Dispose()
        {
            lock (this.locker)
            {
                foreach (var directoryWatcher in this.directoryWatchers.Values)
                {
                    directoryWatcher.Dispose();
                }
            }
        }

        public void Clear()
        {
            lock (this.locker)
            {
                this.files.Clear();

                foreach (var directoryWatcher in this.directoryWatchers.Values)
                {
                    directoryWatcher.Dispose();
                }

                this.directoryWatchers.Clear();
            }
        }

        public void Set(FileResourceKey key)
        {
            lock (this.locker)
            {
                var directory = Path.GetDirectoryName(key.AbsolutePath)!;
                if (!this.directoryWatchers.ContainsKey(directory) && Directory.Exists(directory))
                {
                    var directoryWatcher = new FileSystemWatcher(directory)
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                        IncludeSubdirectories = true,
                        EnableRaisingEvents = true,
                    };

                    directoryWatcher.Changed += (sender, e) => OnWatcherChanged();
                    directoryWatcher.Created += (sender, e) => OnWatcherChanged();
                    directoryWatcher.Deleted += (sender, e) => OnWatcherChanged();
                    directoryWatcher.Renamed += (sender, e) => OnWatcherChanged();

                    this.directoryWatchers.Add(directory, directoryWatcher);
                }

                this.files[key.AbsolutePath] = key;
            }
        }

        private void OnWatcherChanged()
        {
            Thread.Sleep(100);

            this.thread.DispatchAsync(() =>
            {
                if (TrySetFilesKey())
                {
                    this.Changed?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        private bool TrySetFilesKey()
        {
            lock (this.locker)
            {
                var isChanged = false;

                foreach (var pair in this.files.ToArray())
                {
                    var fileKey = this.fileSystem.GetFileKey(pair.Key);

                    if (!Equals(pair.Value, fileKey))
                    {
                        this.files[pair.Key] = fileKey;
                        isChanged = true;
                    }
                }

                return isChanged;
            }
        }
    }
}
