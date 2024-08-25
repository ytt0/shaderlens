namespace Shaderlens
{
    public interface IVector<T> : IEnumerable<T>
    {
        T this[int index] { get; }
        int Length { get; }
    }
}
