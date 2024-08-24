namespace Shaderlens
{
    public interface ITimeSource
    {
        long Now { get; }
        TimeSpan GetElapsedTime(long startTime);
    }

    public class StopwatchTimeSource : ITimeSource
    {
        public static readonly ITimeSource Instance = new StopwatchTimeSource();

        public long Now { get { return Stopwatch.GetTimestamp(); } }

        public TimeSpan GetElapsedTime(long startTime)
        {
            return Stopwatch.GetElapsedTime(startTime);
        }
    }
}
