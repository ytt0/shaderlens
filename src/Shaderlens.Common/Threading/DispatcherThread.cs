namespace Shaderlens.Threading
{
    public interface IDispatcherThread : IDisposable
    {
        IThreadAccess Access { get; }
        void DispatchAsync(Action action);
    }

    public static class DispatcherThreadExtensions
    {
        public static void VerifyAccess(this IDispatcherThread dispatcherThread)
        {
            dispatcherThread.Access.Verify();
        }

        public static void Dispatch(this IDispatcherThread dispatcherThread, Action action)
        {
            Exception exception = null!;

            using (var completeEvent = new ManualResetEventSlim(false))
            {
                dispatcherThread.DispatchAsync(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }

                    completeEvent.Set();
                });

                completeEvent.Wait();
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public static T Dispatch<T>(this IDispatcherThread dispatcherThread, Func<T> func)
        {
            T result = default!;

            dispatcherThread.Dispatch(new Action(() => { result = func(); }));

            return result;
        }
    }

    public class DispatcherThread : IDispatcherThread
    {
        [AllowNull]
        public IThreadAccess Access { get; private set; }

        private readonly ManualResetEventSlim startedEvent;
        private readonly ManualResetEventSlim stoppedEvent;
        private readonly ManualResetEventSlim continueEvent;
        private readonly Thread thread;
        private readonly object locker;
        private readonly List<Action> actions;
        private readonly string name;
        private bool isStopRequested;
        private bool isStarted;

        public DispatcherThread(string name)
        {
            this.name = name;
            this.startedEvent = new ManualResetEventSlim(false);
            this.stoppedEvent = new ManualResetEventSlim(false);
            this.continueEvent = new ManualResetEventSlim(false);
            this.thread = new Thread(Run) { Name = name };

            this.locker = new object();
            this.actions = new List<Action>();
        }

        public void Dispose()
        {
            if (this.thread.IsAlive)
            {
                throw new Exception($"Thread {this.name} has not been stopped");
            }

            this.startedEvent.Dispose();
            this.stoppedEvent.Dispose();
            this.continueEvent.Dispose();
        }

        public void Start()
        {
            lock (this.locker)
            {
                if (this.isStarted || this.isStopRequested)
                {
                    return;
                }

                this.isStarted = true;
            }

            this.thread.Start();
            this.continueEvent.Set();
            this.startedEvent.Wait();
        }

        public void Stop()
        {
            lock (this.locker)
            {
                if (!this.isStarted || this.isStopRequested)
                {
                    return;
                }

                this.isStopRequested = true;
                this.continueEvent.Set();
            }

            this.stoppedEvent.Wait();
            this.thread.Join();
        }

        public void DispatchAsync(Action action)
        {
            if (this.thread.ManagedThreadId == Environment.CurrentManagedThreadId)
            {
                action();
                return;
            }

            lock (this.locker)
            {
                if (!this.isStarted || this.isStopRequested)
                {
                    throw new Exception($"Thread {this.name} is not running");
                }

                this.actions.Add(action);
                this.continueEvent.Set();
            }
        }

        private void Run()
        {
            try
            {
                this.Access = new ThreadAccess(this.name, Environment.CurrentManagedThreadId);

                this.startedEvent.Set();

                while (true)
                {
                    this.continueEvent.Wait();

                    Action[]? actions = null;

                    lock (this.locker)
                    {
                        if (this.isStopRequested)
                        {
                            break;
                        }

                        if (this.actions.Count > 0)
                        {
                            actions = this.actions.ToArray();
                            this.actions.Clear();
                        }

                        this.continueEvent.Reset();
                    }

                    if (actions != null)
                    {
                        foreach (var action in actions)
                        {
                            action();
                        }

                        continue;
                    }
                }

                this.stoppedEvent.Set();
            }
            catch
            {
                //this.OnException?.Invoke(e);
                this.startedEvent.Set();
                this.stoppedEvent.Set();
            }
        }
    }
}
