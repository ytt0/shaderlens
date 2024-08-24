namespace Shaderlens.Threading
{
    public interface IThreadAccess
    {
        void Verify();
    }

    public class ThreadAccess : IThreadAccess
    {
        private readonly string threadName;
        private readonly int managedThreadId;

        public ThreadAccess(string threadName, int managedThreadId)
        {
            this.threadName = threadName;
            this.managedThreadId = managedThreadId;
        }

        public override string ToString()
        {
            return $"{this.threadName} thread access";
        }

        public void Verify()
        {
#if DEBUG
            if (this.managedThreadId != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException($"Method is expected to run on the {this.threadName} thread");
            }
#endif
        }
    }
}
