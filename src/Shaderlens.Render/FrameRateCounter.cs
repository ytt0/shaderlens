namespace Shaderlens.Render
{
    public interface IFrameRateCounter
    {
        void RecordFrame(FrameIndex frameIndex, int totalRunTime, bool isPaused);
    }

    public interface IFrameRateTarget
    {
        void SetFrameRate(FrameIndex frameIndex, double rate, double average);
    }

    public class FrameRateCounter : IFrameRateCounter
    {
        private class EmptyFrameRateCounter : IFrameRateCounter
        {
            public void RecordFrame(FrameIndex frameIndex, int totalRunTime, bool isPaused)
            {
            }
        }

        public static readonly IFrameRateCounter Empty = new EmptyFrameRateCounter();

        private readonly long updateIntervalTicks;
        private readonly IFrameRateTarget target;
        private long startTime;
        private int index;
        private int totalRunTime;
        private int startIndex;

        public FrameRateCounter(IFrameRateTarget target, TimeSpan updateInterval)
        {
            this.updateIntervalTicks = updateInterval.Ticks;
            this.index = 0;
            this.startIndex = 0;
            this.startTime = 0L;
            this.target = target;
        }

        public void RecordFrame(FrameIndex frameIndex, int frameRunTime, bool isPaused)
        {
            this.index += frameRunTime > 0 ? 1 : 0;
            this.totalRunTime += frameRunTime;

            var now = Stopwatch.GetTimestamp();

            if (now > this.startTime + this.updateIntervalTicks || isPaused)
            {
                var count = this.index - this.startIndex;

                var rate = count > 0 ? count / TimeSpan.FromTicks(now - this.startTime).TotalSeconds : 0;
                var average = count > 0 ? this.totalRunTime / count : 0;

                this.startIndex = this.index;
                this.startTime = now;
                this.totalRunTime = 0;

                this.target.SetFrameRate(frameIndex, rate, average);
            }
        }
    }
}
