namespace Shaderlens.Serialization.Text
{
    public interface ITextSerializer<T>
    {
        string Serialize(T value);
        bool TryDeserialize(string value, [MaybeNullWhen(false)] out T result);
    }
}
