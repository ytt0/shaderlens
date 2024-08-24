namespace Shaderlens.Render
{
    public interface IProjectLoadLogger : IDisposable
    {
        void LoadStarted();
        void SetState(string header, string? content = null);
        void AddFailure(string message, IFileResource<string>? source = null, int? lineIndex = null);
        void LoadCompleted(bool succeeded);
    }

    public static class ProjectLoadLoggerExtensions
    {
        public static void AddFailure(this IProjectLoadLogger logger, string message, SourceLine sourceLine)
        {
            logger.AddFailure(message, sourceLine.Source, sourceLine.Index);
        }
    }
}