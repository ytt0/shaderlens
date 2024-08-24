namespace Shaderlens.Render.Project
{
    public interface IViewerPassCollection
    {
        IRenderProgram Clear { get; }
        IRenderProgram ValuesOverlay { get; }

        bool TryGetProgram(string key, [MaybeNullWhen(false)] out IRenderProgram program);
    }

    public class ViewerPassCollection : IViewerPassCollection
    {
        public IRenderProgram Clear { get; }
        public IRenderProgram ValuesOverlay { get; }

        private readonly IReadOnlyDictionary<string, IRenderProgram> programs;

        public ViewerPassCollection(IRenderProgram clearViewerProgram, IRenderProgram valuesOverlayViewerProgram, IReadOnlyDictionary<string, IRenderProgram> programs)
        {
            this.Clear = clearViewerProgram;
            this.ValuesOverlay = valuesOverlayViewerProgram;
            this.programs = programs;
        }

        public bool TryGetProgram(string key, [MaybeNullWhen(false)] out IRenderProgram program)
        {
            return this.programs.TryGetValue(key, out program);
        }
    }
}
