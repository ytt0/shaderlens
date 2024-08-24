namespace Shaderlens.Resources
{
    public class ViewerSourceLines : IViewerSourceLines
    {
        private static readonly Lazy<string> DefaultSource = new Lazy<string>(() => GetEmbeddedResource("Shaderlens.Resources.Viewer.Default.glsl"));
        private static readonly Lazy<string> OverlaySource = new Lazy<string>(() => GetEmbeddedResource("Shaderlens.Resources.Viewer.Overlay.glsl"));

        public SourceLines Clear { get; }
        public SourceLines ValuesOverlay { get; }

        public ViewerSourceLines(double gridVisibleScale, double valueVisibleScale, double fontScale)
        {
            var overlayVersionDefines = new StringBuilder();
            overlayVersionDefines.AppendLine($"#define OVERLAY_GRID_VISIBLE_SCALE {Math.Max(1.0, gridVisibleScale):0.0####}");
            overlayVersionDefines.AppendLine($"#define OVERLAY_VALUE_VISIBLE_SCALE {Math.Max(1.0, valueVisibleScale):0.0####}");
            overlayVersionDefines.AppendLine($"#define OVERLAY_FONT_SCALE {fontScale:0.0####}");

            this.Clear = new SourceLines(new FileResource<string>(new FileResourceKey("Viewer (auto)"), DefaultSource.Value));
            this.ValuesOverlay = new SourceLines(new FileResource<string>(new FileResourceKey("Viewer Overlay (auto)"), SetVersionDefines(overlayVersionDefines.ToString(), OverlaySource.Value)));
        }

        private static string SetVersionDefines(string versionDefines, string code)
        {
            return code.Replace("#VERSION_DEFINES", versionDefines);
        }

        private static string GetEmbeddedResource(string name)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(name)!))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
