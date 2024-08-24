namespace Shaderlens.Render
{
    public interface IStatisticsFormatter
    {
        string GetStatistics(FrameIndex frameIndex, double rate, double average, int bufferWidth, int bufferHeight, bool roundSeconds);
    }

    public class StatisticsFormatter : IStatisticsFormatter
    {
        private readonly string separator;
        private readonly char[] buffer;

        public StatisticsFormatter(string separator)
        {
            this.buffer = new char[200];
            this.separator = separator;
        }

        public string GetStatistics(FrameIndex frameIndex, double rate, double average, int bufferWidth, int bufferHeight, bool includeMilliseconds)
        {
            var handler = new DefaultInterpolatedStringHandler(this.buffer.Length, 20, null, this.buffer.AsSpan());

            handler.AppendLiteral("Frame ");
            handler.AppendFormatted(frameIndex.Index, "n0");
            handler.AppendLiteral(this.separator);
            handler.AppendLiteral("Time ");
            handler.AppendFormatted(TimeSpan.FromTicks(frameIndex.Time), includeMilliseconds ? "m':'ss'.'fff" : "m':'ss");
            handler.AppendLiteral(this.separator);
            handler.AppendLiteral("FPS ");
            handler.AppendFormatted(rate, "n0");

            if (average > 0)
            {
                handler.AppendLiteral(this.separator);
                handler.AppendLiteral("Render ");
                handler.AppendFormatted(average / TimeSpan.TicksPerMillisecond, "n4");
                handler.AppendLiteral("ms");
                handler.AppendLiteral(this.separator);
                handler.AppendLiteral("Utilization ");
                handler.AppendFormatted(100.0 * Math.Min(1.0, rate * average / TimeSpan.TicksPerSecond), "0");
                handler.AppendLiteral("%");
                handler.AppendLiteral(this.separator);
                handler.AppendLiteral("Max FPS ");
                handler.AppendFormatted(TimeSpan.TicksPerSecond / average, "n0");
            }

            handler.AppendLiteral(this.separator);
            handler.AppendLiteral("Resolution ");
            handler.AppendFormatted(bufferWidth);
            handler.AppendLiteral("\u00d7"); //x
            handler.AppendFormatted(bufferHeight);

            return handler.ToString();
        }
    }
}
