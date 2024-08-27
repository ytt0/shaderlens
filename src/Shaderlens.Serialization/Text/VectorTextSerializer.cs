namespace Shaderlens.Serialization.Text
{
    public partial class VectorTextSerializer<T> : ITextSerializer<Vector<T>>
    {
        private readonly ITextSerializer<T> serializer;

        public VectorTextSerializer(ITextSerializer<T> serializer)
        {
            this.serializer = serializer;
        }

        public string Serialize(Vector<T> value)
        {
            return $"[{String.Join(", ", value.Select(this.serializer.Serialize))}]";
        }

        public bool TryDeserialize(string value, [MaybeNullWhen(false)] out Vector<T> result)
        {
            var list = new List<T>();

            var serializedValues = value.TrimStart('[').TrimEnd(']').Split(',');

            if (serializedValues.Length > 0)
            {
                foreach (var serializedValue in serializedValues)
                {
                    if (!this.serializer.TryDeserialize(serializedValue, out var deserializedValue))
                    {
                        result = default;
                        return false;
                    }

                    list.Add(deserializedValue);
                }
            }

            result = Vector.Create(list);
            return true;
        }
    }

    public static class VectorTextSerializer
    {
        public static ITextSerializer<Vector<T>> Create<T>(ITextSerializer<T> serializer)
        {
            return new VectorTextSerializer<T>(serializer);
        }
    }

    public partial class FixedSizeVectorTextSerializer<T> : ITextSerializer<Vector<T>>
    {
        private readonly ITextSerializer<Vector<T>> serializer;
        private readonly int count;

        public FixedSizeVectorTextSerializer(ITextSerializer<Vector<T>> serializer, int count)
        {
            this.serializer = serializer;
            this.count = count;
        }

        public string Serialize(Vector<T> value)
        {
            return this.serializer.Serialize(value);
        }

        public bool TryDeserialize(string value, [MaybeNullWhen(false)] out Vector<T> result)
        {
            return this.serializer.TryDeserialize(value, out result) && result.Count == this.count;
        }
    }

    public static class FixedSizeVectorTextSerializer
    {
        public static ITextSerializer<Vector<T>> Create<T>(ITextSerializer<Vector<T>> serializer, int count)
        {
            return new FixedSizeVectorTextSerializer<T>(serializer, count);
        }
    }
}
