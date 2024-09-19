namespace Shaderlens
{
    public class Vector<T> : IReadOnlyList<T>, IEquatable<Vector<T>>
    {
        public T this[int index] { get { return this.values[index]; } }

        public int Count { get; }

        private readonly T[] values;

        public Vector(params T[] values) :
            this((IEnumerable<T>)values)
        {
        }

        public Vector(IEnumerable<T> values)
        {
            this.values = values.ToArray();
            this.Count = this.values.Length;
        }

        public override string? ToString()
        {
            return $"[{String.Join(", ", this.values)}]";
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            for (var i = 0; i < this.Count; i++)
            {
                hashCode.Add(this.values[i]);
            }

            return hashCode.ToHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Vector<T>);
        }

        public bool Equals(Vector<T>? other)
        {
            return other != null && this.SequenceEqual(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.values.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class Vector
    {
        public static Vector<T> Create<T>(IEnumerable<T> values)
        {
            return new Vector<T>(values);
        }

        public static Vector<T> Create<T>(int count, T value)
        {
            return new Vector<T>(Enumerable.Range(0, count).Select(i => value).ToArray());
        }
    }
}
