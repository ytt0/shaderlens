namespace Shaderlens.Render
{
    public struct FrameIndex
    {
        public int Index;
        public long Time;
        public long Delta;
        public DateTime DateTime;
    }

    public interface IFrameIndexSource
    {
        FrameIndex GetNextFrame();
        void Restart();
        void PauseTime();
        void ResumeTime();
        void SetSpeed(double speed);
    }

    public class FrameIndexSource : IFrameIndexSource
    {
        private readonly object locker;

        private FrameIndex frame;
        private double speed;
        private bool isPaused;
        private long intervalStartTime;
        private long intervalOffset;

        public FrameIndexSource()
        {
            this.locker = new object();
            this.frame = new FrameIndex();
            this.speed = 1.0;
        }

        public void Restart()
        {
            lock (this.locker)
            {
                this.frame = default;
                this.intervalOffset = 0;
                this.intervalStartTime = 0;
            }
        }

        public void PauseTime()
        {
            lock (this.locker)
            {
                this.isPaused = true;
                this.frame.Delta = 0;
            }
        }

        public void ResumeTime()
        {
            lock (this.locker)
            {
                this.isPaused = false;
                this.intervalOffset = this.frame.Time;
                this.intervalStartTime = 0;
            }
        }

        public void SetSpeed(double speed)
        {
            lock (this.locker)
            {
                this.frame.Delta = 0;
                this.intervalOffset = this.frame.Time;
                this.intervalStartTime = 0;
                this.speed = speed;
            }
        }

        public FrameIndex GetNextFrame()
        {
            lock (this.locker)
            {
                var now = Stopwatch.GetTimestamp();

                if (this.intervalStartTime == 0)
                {
                    this.intervalStartTime = now;
                }
                else
                {
                    this.frame.Index++;

                    if (!this.isPaused)
                    {
                        var time = (long)(this.speed * (now - this.intervalStartTime)) + this.intervalOffset;

                        this.frame.Delta = time - this.frame.Time;
                        this.frame.Time = time;
                    }
                }

                if (!this.isPaused || this.frame.Index == 0)
                {
                    this.frame.DateTime = DateTime.Now;
                }

                return this.frame;
            }
        }
    }
}
