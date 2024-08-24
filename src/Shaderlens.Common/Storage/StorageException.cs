namespace Shaderlens.Storage
{
    public class StorageException : Exception
    {
        public FileResourceKey Key { get; }

        public StorageException(string? message, FileResourceKey key, Exception? innerException = null) :
            base(message, innerException)
        {
            this.Key = key;
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
}
