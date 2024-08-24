namespace Shaderlens.Presentation.Threading
{
    public class ViewDispatcherThread : IDispatcherThread
    {
        public IThreadAccess Access { get; }

        private readonly Dispatcher dispatcher;

        public ViewDispatcherThread(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.Access = new ThreadAccess("view", dispatcher.Thread.ManagedThreadId);
        }

        public void Dispose()
        {
        }

        public void DispatchAsync(Action action)
        {
            if (this.dispatcher.Thread.ManagedThreadId == Environment.CurrentManagedThreadId)
            {
                action();
            }
            else
            {
                this.dispatcher.InvokeAsync(action);
            }
        }
    }
}
