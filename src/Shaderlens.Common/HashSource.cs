namespace Shaderlens
{
    public interface IHashSource
    {
        string GetHash(string value);
    }

    public class Sha256HashSource : IHashSource
    {
        public static readonly IHashSource Instance = new Sha256HashSource();

        private Sha256HashSource()
        {
        }

        public string GetHash(string value)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return String.Join(String.Empty, hash.Select(v => v.ToString("x2")));
        }
    }
}
