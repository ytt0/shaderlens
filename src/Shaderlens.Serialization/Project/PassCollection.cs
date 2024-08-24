namespace Shaderlens.Serialization.Project
{
    public interface IPassCollection<out T> : IEnumerable<T>
    {
        T this[int index] { get; }
        T this[string key] { get; }
        T? Image { get; }
        IEnumerable<T> Buffers { get; }
        IEnumerable<string> BuffersKeys { get; }
        int Count { get; }

        T? TryGetBuffer(string key);
    }

    public static class PassCollection
    {
        private class EmptyPassCollection<T> : IPassCollection<T>
        {
            public static readonly IPassCollection<T> Instance = new EmptyPassCollection<T>();

            public T this[int index] { get { throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(PassCollection)} is empty"); } }
            public T this[string key] { get { throw new KeyNotFoundException($"{key} not found, {nameof(PassCollection)} is empty"); } }
            public T? Image { get; }
            public IEnumerable<T> Buffers { get; }
            public IEnumerable<string> BuffersKeys { get; }
            public int Count { get; }

            private EmptyPassCollection()
            {
                this.Buffers = Array.Empty<T>();
                this.BuffersKeys = Array.Empty<string>();
            }

            public T? TryGetBuffer(string key)
            {
                return default;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.Buffers.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static IPassCollection<T> Empty<T>()
        {
            return EmptyPassCollection<T>.Instance;
        }
    }

    public class PassCollection<T> : IPassCollection<T>
    {
        public T this[int index] { get { return this.all[index]; } }
        public T this[string key] { get { return this.buffers[key]; } }
        public T? Image { get; }
        public IEnumerable<T> Buffers { get; }
        public IEnumerable<string> BuffersKeys { get; }
        public int Count { get; }

        private readonly IReadOnlyDictionary<string, T> buffers;
        private readonly T[] all;

        public PassCollection(T image) :
            this(image, Dictionary.Empty<string, T>())
        {
        }

        public PassCollection(T image, IReadOnlyDictionary<string, T> buffers)
        {
            this.Image = image;
            this.buffers = buffers;
            this.Count = buffers.Count + 1;

            var pairs = buffers.OrderBy(pair => pair.Key).ToArray();
            this.BuffersKeys = pairs.Select(pair => pair.Key).ToArray();
            this.Buffers = pairs.Select(pair => pair.Value).ToArray();
            this.all = this.Buffers.Concat(image).ToArray();
        }

        public T? TryGetBuffer(string key)
        {
            return this.buffers.TryGetValue(key, out var buffer) ? buffer : default;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.all.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
