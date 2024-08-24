namespace Shaderlens.Serialization
{
    public class ProjectLoadException : Exception
    {
        public ProjectLoadException(string message, Exception? innerException = null) :
            base(message, innerException)
        {
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
}
