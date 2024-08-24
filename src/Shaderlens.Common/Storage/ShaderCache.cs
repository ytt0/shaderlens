

namespace Shaderlens.Storage
{
    public interface IShaderCache
    {
        bool TryGetShaderBinary(string key, out uint binaryFormat, [MaybeNullWhen(false)] out byte[] binary);
        void SetShaderBinary(string key, uint binaryFormat, byte[] binary);
    }

    public class EmptyShaderCache : IShaderCache
    {
        public void SetShaderBinary(string key, uint binaryFormat, byte[] binary)
        {
        }

        public bool TryGetShaderBinary(string key, out uint binaryFormat, [MaybeNullWhen(false)] out byte[] binary)
        {
            binaryFormat = 0;
            binary = null;
            return false;
        }
    }

    public class MemoryShaderCache : IShaderCache
    {
        private struct Entry
        {
            public uint BinaryFormat;
            public byte[] Binary;
        }

        private readonly Dictionary<string, Entry> entries;
        private readonly Queue<string> keys;
        private readonly int maxCapacity;

        public MemoryShaderCache(int maxCapacity = 100)
        {
            this.entries = new Dictionary<string, Entry>();
            this.keys = new Queue<string>();
            this.maxCapacity = maxCapacity;
        }

        public bool TryGetShaderBinary(string key, out uint binaryFormat, [MaybeNullWhen(false)] out byte[] binary)
        {
            if (this.entries.TryGetValue(key, out var entry))
            {
                binaryFormat = entry.BinaryFormat;
                binary = entry.Binary;
                return true;
            }

            binaryFormat = 0;
            binary = null;
            return false;
        }

        public void SetShaderBinary(string key, uint binaryFormat, byte[] binary)
        {
            if (!this.entries.ContainsKey(key))
            {
                this.entries.Add(key, new Entry { BinaryFormat = binaryFormat, Binary = binary });
                this.keys.Enqueue(key);

                while (this.keys.Count > this.maxCapacity)
                {
                    this.entries.Remove(this.keys.Dequeue());
                }
            }
        }
    }

    public class FileSystemShaderCache : IShaderCache
    {
        private readonly string path;
        private bool isPathValidated;

        public FileSystemShaderCache(string path)
        {
            this.path = path;
        }

        public bool TryGetShaderBinary(string key, out uint binaryFormat, [MaybeNullWhen(false)] out byte[] binary)
        {
            var shaderPath = Path.Combine(this.path, key);

            if (File.Exists(shaderPath))
            {
                var source = File.ReadAllBytes(Path.Combine(this.path, key));

                binaryFormat = BitConverter.ToUInt32(source.Take(4).ToArray(), 0);
                binary = source.Skip(4).ToArray();
                return true;
            }

            binaryFormat = 0;
            binary = null;
            return false;
        }

        public void SetShaderBinary(string key, uint binaryFormat, byte[] binary)
        {
            if (!this.isPathValidated)
            {
                Directory.CreateDirectory(this.path);
                this.isPathValidated = true;
            }

            var shaderPath = Path.Combine(this.path, key);
            var target = new byte[binary.Length + 4];

            var formatBytes = BitConverter.GetBytes(binaryFormat);
            formatBytes.CopyTo(target, 0);
            binary.CopyTo(target, 4);

            File.WriteAllBytes(shaderPath, target);
        }
    }

    public class ShaderCacheCollection : IShaderCache
    {
        private readonly IShaderCache[] shaderCaches;

        public ShaderCacheCollection(params IShaderCache[] shaderCaches)
        {
            this.shaderCaches = shaderCaches;
        }

        public void SetShaderBinary(string key, uint binaryFormat, byte[] binary)
        {
            foreach (var shaderCache in this.shaderCaches)
            {
                shaderCache.SetShaderBinary(key, binaryFormat, binary);
            }
        }

        public bool TryGetShaderBinary(string key, out uint binaryFormat, [MaybeNullWhen(false)] out byte[] binary)
        {
            foreach (var shaderCache in this.shaderCaches)
            {
                if (shaderCache.TryGetShaderBinary(key, out binaryFormat, out binary))
                {
                    return true;
                }
            }

            binaryFormat = 0;
            binary = null;
            return false;
        }
    }

    public class ReadOnlyShaderCache : IShaderCache
    {
        private readonly IShaderCache shaderCache;

        public ReadOnlyShaderCache(IShaderCache shaderCache)
        {
            this.shaderCache = shaderCache;
        }

        public void SetShaderBinary(string key, uint binaryFormat, byte[] binary)
        {
        }

        public bool TryGetShaderBinary(string key, out uint binaryFormat, [MaybeNullWhen(false)] out byte[] binary)
        {
            return this.shaderCache.TryGetShaderBinary(key, out binaryFormat, out binary);
        }
    }
}
