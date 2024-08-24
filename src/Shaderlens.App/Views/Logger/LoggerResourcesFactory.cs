using System.Windows.Shapes;

namespace Shaderlens.Views.Logger
{
    using Path = System.Windows.Shapes.Path;

    public interface ILoggerResourcesFactory
    {
        FrameworkElement CreateLinkIcon();
        FrameworkElement CreateProgressIcon();
        FrameworkElement CreateSuccessIcon();
        FrameworkElement CreateFailureIcon();
    }

    public class LoggerResourcesFactory : ILoggerResourcesFactory
    {
        private static readonly Lazy<Geometry> FailureGeometry = new Lazy<Geometry>(() => Geometry.Parse("m19.822 12.145-7.644 7.71m0-7.71 7.644 7.71").WithFreeze());
        private static readonly Lazy<Geometry> SuccessGeometry = new Lazy<Geometry>(() => Geometry.Parse("m21.734 12.145-7.645 7.71v0L10.266 16").WithFreeze());
        private static readonly Lazy<Geometry> ProgressGeometry = new Lazy<Geometry>(() => Geometry.Parse("M14.765 19.947a.911.911 0 0 1-1.367-.789v-6.315c0-.702.76-1.141 1.368-.79l5.47 3.159a.91.91 0 0 1 0 1.578z").WithFreeze());
        private static readonly Lazy<Geometry> LinkGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 5.75,0.75 10.25,0.75 V 5 M 4,7 10.25,0.75 M 3.5,2 H 0.75 v 8.25 H 9 V 7.5").WithFreeze());

        private readonly ILogTheme theme;

        public LoggerResourcesFactory(ILogTheme theme)
        {
            this.theme = theme;
        }

        public FrameworkElement CreateLinkIcon()
        {
            return new Path
            {
                Data = LinkGeometry.Value,
                StrokeThickness = 1.2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
            }.WithReference(Shape.StrokeProperty, this.theme.LinkForeground);
        }

        public FrameworkElement CreateProgressIcon()
        {
            return CreatePath(ProgressGeometry.Value, this.theme.ProgressTrack);
        }

        public FrameworkElement CreateSuccessIcon()
        {
            return CreatePath(SuccessGeometry.Value, this.theme.SuccessProgressTrack);
        }

        public FrameworkElement CreateFailureIcon()
        {
            return CreatePath(FailureGeometry.Value, this.theme.FailureProgressTrack);
        }

        private static Path CreatePath(Geometry geometry, IThemeResource<Brush> stroke)
        {
            return new Path
            {
                Data = geometry,
                StrokeThickness = 1.5,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-4),
                Width = 32,
                Height = 32,
            }.WithReference(Shape.StrokeProperty, stroke);
        }
    }
}
