namespace Shaderlens.Extensions
{
    public static class Dictionary
    {
        private class EmptyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        {
            public static readonly EmptyDictionary<TKey, TValue> Instance = new EmptyDictionary<TKey, TValue>();

            public TValue this[TKey key] { get { throw new KeyNotFoundException($"Dictionary is empty"); } }

            public IEnumerable<TKey> Keys { get; }
            public IEnumerable<TValue> Values { get; }
            public int Count { get; }

            public EmptyDictionary()
            {
                this.Keys = Array.Empty<TKey>();
                this.Values = Array.Empty<TValue>();
            }

            public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            {
                value = default;
                return false;
            }

            public bool ContainsKey(TKey key)
            {
                return false;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>()
        {
            return EmptyDictionary<TKey, TValue>.Instance;
        }
    }
}
