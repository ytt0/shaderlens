using System.Windows.Shapes;

namespace Shaderlens.Views.Menus
{
    using Path = System.Windows.Shapes.Path;

    public interface IMenuResourcesFactory
    {
        FrameworkElement CreatePlayIcon();
        FrameworkElement CreatePauseIcon();
        FrameworkElement CreateStepIcon();
        FrameworkElement CreateRestartIcon();
        FrameworkElement CreateUniformsIcon();
        FrameworkElement CreateStartPageIcon();
        FrameworkElement CreateNewIcon();
        FrameworkElement CreateOpenIcon();
        FrameworkElement CreateRecentIcon();
        FrameworkElement CreateReloadIcon();
        FrameworkElement CreateSaveIcon();
        FrameworkElement CreateFilesIcon();
        FrameworkElement CreateExportIcon();
        FrameworkElement CreateCopyIcon();
        FrameworkElement CreateBuffersIcon();
        FrameworkElement CreateResolutionIcon();
        FrameworkElement CreateFrameRateIcon();
        FrameworkElement CreateSpeedIcon();
        FrameworkElement CreateViewerIcon();
        FrameworkElement CreateOptionsIcon();
        FrameworkElement CreateHelpIcon();
        FrameworkElement CreateFolderIcon();
        FrameworkElement CreateFileCodeIcon();
        FrameworkElement CreateFileSettingsIcon();
        FrameworkElement CreateFileProjectIcon();
        FrameworkElement CreateFileUniformsIcon();
        FrameworkElement CreateProjectIcon();
        FrameworkElement CreateRemoveIcon();
    }

    public class MenuResourcesFactory : IMenuResourcesFactory
    {
        private static readonly Lazy<Geometry> PlayPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M10.969 10.758v10.428c.001 1.249 1.38 2.005 2.434 1.333l8.18-5.215a1.583 1.583 0 0 0 0-2.67l-8.18-5.212c-1.15-.562-2.41.17-2.434 1.336Z").WithFreeze());
        private static readonly Lazy<Geometry> PausePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M11.567 9.033h1.266c.702 0 1.267.565 1.267 1.267v11.4c0 .702-.565 1.267-1.267 1.267h-1.266A1.264 1.264 0 0 1 10.3 21.7V10.3c0-.702.565-1.267 1.267-1.267zm7.6 0h1.266c.702 0 1.267.565 1.267 1.267v11.4c0 .702-.565 1.267-1.267 1.267h-1.266A1.264 1.264 0 0 1 17.9 21.7V10.3c0-.702.565-1.267 1.267-1.267z").WithFreeze());
        private static readonly Lazy<Geometry> StepPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M9.518 10.76v10.428c.001 1.249 1.225 2.005 2.16 1.333l7.255-5.215c.866-.622.866-2.047 0-2.67l-7.255-5.212c-1.02-.562-2.139.17-2.16 1.336Zm11.697-1.727h0c.702 0 1.267.565 1.267 1.267v11.4c0 .702-.565 1.267-1.267 1.267h0A1.264 1.264 0 0 1 19.95 21.7V10.3c0-.702.565-1.267 1.266-1.267z").WithFreeze());
        private static readonly Lazy<Geometry> RestartPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M24.728 10.743v10.428c-.001 1.25-.866 2.005-1.525 1.334l-5.124-5.216c-.61-.622-.61-2.046 0-2.669l5.124-5.213c.72-.561 1.51.17 1.525 1.336zm-7.473 0v10.428c-.001 1.25-.865 2.005-1.525 1.334l-5.124-5.216c-.61-.622-.61-2.046 0-2.669l5.124-5.213c.72-.561 1.51.17 1.525 1.336zm-8.716-1.71h0c.702 0 1.267.565 1.267 1.267v11.4c0 .702-.565 1.267-1.267 1.267h0A1.264 1.264 0 0 1 7.272 21.7V10.3c0-.702.565-1.267 1.267-1.267z").WithFreeze());
        private static readonly Lazy<Geometry> UniformsPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M7 22.5h6m8 0h4m-7 0a2.5 2.5 0 0 1-2.5 2.5 2.5 2.5 0 0 1-2.5-2.5 2.5 2.5 0 0 1 2.5-2.5 2.5 2.5 0 0 1 2.5 2.5zM7 16h1.5m8 0H25m-11.5 0a2.5 2.5 0 0 1-2.5 2.5A2.5 2.5 0 0 1 8.5 16a2.5 2.5 0 0 1 2.5-2.5 2.5 2.5 0 0 1 2.5 2.5ZM7 9.5h8.5m5 0a2.5 2.5 0 1 1-5 0 2.5 2.5 0 0 1 5 0zm3 0H25").WithFreeze());
        private static readonly Lazy<Geometry> StartPagePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M18.571 24.64v-5.46c0-.987-.795-1.782-1.781-1.782h-1.58c-.986 0-1.78.795-1.78 1.782v5.46m-2.82.227a1.778 1.778 0 0 1-1.781-1.783v-5.686c-.856 0-1.545-.69-1.545-1.546 0-.455.175-.911.524-1.26l6.929-6.935c.349-.349.805-.524 1.263-.524.459 0 .915.175 1.264.524l6.929 6.934c.349.35.523.806.523 1.261 0 .857-.688 1.546-1.544 1.546v5.686c0 .988-.794 1.783-1.781 1.783z").WithFreeze());
        private static readonly Lazy<Geometry> NewPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M22.967 13.467H17.9a1.264 1.264 0 0 1-1.267-1.267V7.133M22.967 23.6c0 .702-.565 1.267-1.267 1.267H10.3A1.264 1.264 0 0 1 9.033 23.6V8.4c0-.702.565-1.267 1.267-1.267h6.967l5.7 5.7V23.6z").WithFreeze());
        private static readonly Lazy<Geometry> OpenPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M11.987 12.822H23.3c1.404 0 2.322 1.11 2.058 2.488l-1.23 6.435c-.263 1.379-1.605 2.488-3.008 2.488H9.806c-1.403 0-2.321-1.11-2.058-2.488l1.23-6.435c.263-1.378 1.605-2.488 3.009-2.488zm12.343.009a2.528 2.528 0 0 0-2.533-2.531h-6.333a2.528 2.528 0 0 0-2.534-2.533h-3.8A2.528 2.528 0 0 0 6.597 10.3V21.7a2.528 2.528 0 0 0 2.533 2.534h11.99").WithFreeze());
        private static readonly Lazy<Geometry> RecentPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M24.867 16a8.867 8.867 0 1 1-17.734 0 8.867 8.867 0 0 1 17.734 0zM19.8 16H16v-5.7").WithFreeze());
        private static readonly Lazy<Geometry> ReloadPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M17.912 13.817h5.066V8.75m-.164 8.897a6.967 6.967 0 0 1-1.903 3.566 6.967 6.967 0 1 1 0-9.852c.43.432.802.92 1.105 1.45").WithFreeze());
        private static readonly Lazy<Geometry> SavePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M22.016 24.218v-6.967c0-.702-.565-1.266-1.266-1.266h-9.5c-.702 0-1.267.564-1.267 1.266v6.967M11.25 7.766v3.785c0 .702.565 1.267 1.267 1.267h6.333c.701 0 1.266-.565 1.266-1.267V7.766m-10.133 0A2.528 2.528 0 0 0 7.45 10.3v11.4a2.528 2.528 0 0 0 2.533 2.534h11.4c1.403 0 3.167-1.13 3.167-2.534V10.978L21.34 7.766Z").WithFreeze());
        private static readonly Lazy<Geometry> FilesPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M12.833 12.833h6.334c.701 0 1.266.565 1.266 1.267v8.867c0 .701-.565 1.266-1.266 1.266h-6.334a1.264 1.264 0 0 1-1.266-1.266V14.1c0-.702.565-1.267 1.266-1.267zm7.6 6.334h2.533c.702 0 1.267-.565 1.267-1.267V9.033c0-.701-.565-1.266-1.267-1.266h-6.333c-.702 0-1.266.565-1.266 1.266v3.8m0-2.533H9.033c-.702 0-1.266.565-1.266 1.267v8.866c0 .702.564 1.267 1.266 1.267h2.534").WithFreeze());
        private static readonly Lazy<Geometry> ExportPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M19.198 6.53v3.737h0a8.867 8.867 0 0 0-8.866 8.867c.002.424.034.848.097 1.268a10.133 10.133 0 0 1 8.769-5.068h0v3.736l6.27-6.27zM13.08 9.003h-2.748a2.528 2.528 0 0 0-2.534 2.533v11.4a2.528 2.528 0 0 0 2.534 2.534h11.4a2.528 2.528 0 0 0 2.533-2.534v-4.453").WithFreeze());
        private static readonly Lazy<Geometry> CopyPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M15.05 7.767h7.6c.702 0 1.267.565 1.267 1.266v10.134c0 .701-.565 1.266-1.267 1.266h-7.6a1.264 1.264 0 0 1-1.267-1.266V9.033c0-.701.565-1.266 1.267-1.266zm-3.166 3.8H9.35c-.702 0-1.267.565-1.267 1.266v10.134c0 .702.565 1.266 1.267 1.266h7.6c.702 0 1.267-.564 1.267-1.266v-.634").WithFreeze());
        private static readonly Lazy<Geometry> BuffersPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("m7.642 18.937-.503.254c-.61.308-.61.804 0 1.112l7.76 3.914c.61.308 1.592.308 2.202 0l7.76-3.914c.61-.308.61-.804 0-1.112l-.502-.253M7.642 15.19l-.503.254c-.61.308-.61.804 0 1.111l7.76 3.915c.61.308 1.592.308 2.202 0l7.76-3.915c.61-.307.61-.803 0-1.111l-.503-.253m-7.257-7.408 7.76 3.914c.61.308.61.803 0 1.111l-7.76 3.914c-.61.308-1.592.308-2.202 0l-7.76-3.914c-.61-.308-.61-.803 0-1.11l7.76-3.915c.61-.308 1.592-.308 2.202 0z").WithFreeze());
        private static readonly Lazy<Geometry> ResolutionPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M16.5 19v-3.5H13m3.5 0-7 6.958M9 13h8c1.108 0 2 .892 2 2v8c0 1.108-.892 2-2 2H9c-1.108 0-2-.892-2-2v-8c0-1.108.892-2 2-2zm.5-6H9c-1.108 0-2 .892-2 2v1m11.5-3h-5M25 10V9c0-1.108-.892-2-2-2h-1m3 11.5v-5M22 25h1c1.108 0 2-.892 2-2v-1M9.5 19v3.5H13").WithFreeze());
        private static readonly Lazy<Geometry> FrameRatePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M14.28 14.242v3.498c0 .42.448.673.79.447l2.656-1.75a.54.54 0 0 0 0-.895l-2.656-1.748c-.373-.189-.782.056-.79.448zm5.52 6.191h2.533m-7.6 0h2.534m-7.6 0H12.2m7.6-8.866h2.533m-7.6 0h2.534m-7.6 0H12.2M8.4 9.033h15.2c.702 0 1.267.565 1.267 1.267v11.4c0 .702-.565 1.267-1.267 1.267H8.4A1.264 1.264 0 0 1 7.133 21.7V10.3c0-.702.565-1.267 1.267-1.267z").WithFreeze());
        private static readonly Lazy<Geometry> SpeedPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M13.537 13.062v5.845c0 .7.764 1.124 1.347.748l4.527-2.924a.894.894 0 0 0 0-1.496l-4.527-2.922c-.636-.315-1.334.094-1.347.749zm1.15-6.471a9.5 9.5 0 0 0-4.49 1.862m-1.792 1.79a9.5 9.5 0 0 0-1.862 4.493m0 2.533a9.5 9.5 0 0 0 1.862 4.491m1.792 1.791a9.5 9.5 0 0 0 4.49 1.862m2.536 0a9.5 9.5 0 0 0 8.234-9.41 9.5 9.5 0 0 0-8.234-9.416").WithFreeze());
        private static readonly Lazy<Geometry> ViewerPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M23 25H13c-1.108 0-2-.892-2-2V13c0-1.108.892-2 2-2h10c1.108 0 2 .892 2 2v10c0 1.108-.892 2-2 2zm-6.5-4h-2M12 21H9c-1.108 0-2-.892-2-2V9c0-1.108.892-2 2-2h10c1.108 0 2 .892 2 2v3m0 2.5v2m0 2.5v0m0 0c0 1.108-.892 2-2 2v0").WithFreeze());
        private static readonly Lazy<Geometry> OptionsPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M18.75 16A2.75 2.75 0 0 1 16 18.75 2.75 2.75 0 0 1 13.25 16 2.75 2.75 0 0 1 16 13.25 2.75 2.75 0 0 1 18.75 16ZM16 25.497c-3.303 0-.969-1.142-3.83-2.808-2.86-1.665-2.674.945-4.325-1.94-1.652-2.885.496-1.418.496-4.749 0-3.331-2.148-1.863-.496-4.749 1.651-2.885 1.465-.275 4.326-1.94 2.86-1.666.526-2.808 3.829-2.808s.969 1.142 3.83 2.808c2.86 1.665 2.674-.945 4.325 1.94 1.652 2.886-.496 1.418-.496 4.749 0 3.331 2.148 1.864.496 4.749-1.651 2.885-1.465.275-4.326 1.94-2.86 1.666-.526 2.808-3.829 2.808z").WithFreeze());
        private static readonly Lazy<Geometry> HelpPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M16.032 20.377a.2.2 0 1 1 0-.4.2.2 0 0 1 0 .4m-2.612-7.246c.293-1.71 1.678-2.197 2.58-2.232 1.715-.066 3.012 1.354 2.557 2.978-.455 1.623-2.592 2.13-2.557 3.904M25.5 16a9.5 9.5 0 0 1-9.5 9.5A9.5 9.5 0 0 1 6.5 16 9.5 9.5 0 0 1 16 6.5a9.5 9.5 0 0 1 9.5 9.5Z").WithFreeze());
        private static readonly Lazy<Geometry> FolderPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M22.335 24.233a2.528 2.528 0 0 0 2.531-2.533v-8.87a2.528 2.528 0 0 0-2.533-2.53H16a2.528 2.528 0 0 0-2.534-2.533h-3.8A2.528 2.528 0 0 0 7.135 10.3v11.4a2.528 2.528 0 0 0 2.533 2.533z").WithFreeze());
        private static readonly Lazy<Geometry> FileCodePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("m15.08 19.787 1.84-6.574m1.819 5.256 2.026-1.969-2.026-1.968m-5.478 3.937L11.235 16.5l2.026-1.968m9.706 9.068c0 .702-.565 1.267-1.267 1.267H10.3A1.264 1.264 0 0 1 9.033 23.6V8.4c0-.702.565-1.267 1.267-1.267h6.967l5.7 5.7V23.6z").WithFreeze());
        private static readonly Lazy<Geometry> FileSettingsPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M17.239 16.5A1.239 1.245 0 0 1 16 17.745a1.239 1.245 0 0 1-1.239-1.245A1.239 1.245 0 0 1 16 15.255a1.239 1.245 0 0 1 1.239 1.245ZM16 20.8c-1.488 0-.436-.517-1.725-1.271-1.289-.754-1.205.427-1.949-.879-.744-1.305.224-.641.224-2.149s-.968-.843-.224-2.15c.744-1.305.66-.124 1.949-.878 1.289-.754.237-1.27 1.725-1.27s.436.516 1.725 1.27c1.289.754 1.205-.427 1.949.879s-.224.641-.224 2.15c0 1.507.968.843.224 2.148-.744 1.306-.66.125-1.949.879-1.289.754-.237 1.27-1.725 1.27Zm6.967 2.801c0 .702-.565 1.267-1.267 1.267H10.3A1.264 1.264 0 0 1 9.033 23.6V8.4c0-.702.565-1.267 1.267-1.267h6.967l5.7 5.7V23.6z").WithFreeze());
        private static readonly Lazy<Geometry> FileProjectPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M22.967 23.6c0 .702-.565 1.267-1.267 1.267H10.3A1.264 1.264 0 0 1 9.033 23.6V8.4c0-.702.565-1.267 1.267-1.267h6.967l5.7 5.7V23.6zM16 12l-3.905 2.25m7.81 4.5L16 21.002m3.905-6.75L16 16.5m3.905-2.25L16 12m0 9.001-3.905-2.25M16 16.5l-3.905-2.25m7.81 0v4.5m-7.81-4.5v4.5M16 16.5V21").WithFreeze());
        private static readonly Lazy<Geometry> FileUniformsPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M12 18.6h2.75m2.25 0a1 1 0 0 1-1 1 1 1 0 0 1-1-1 1 1 0 0 1 1-1 1 1 0 0 1 1 1zm-5-4h5.75m2.25 0a1 1 0 0 1-1 1 1 1 0 0 1-1-1 1 1 0 0 1 1-1 1 1 0 0 1 1 1zm2.967 9c0 .702-.565 1.267-1.267 1.267H10.3A1.264 1.264 0 0 1 9.033 23.6V8.4c0-.702.565-1.267 1.267-1.267h6.967l5.7 5.7V23.6z").WithFreeze());
        private static readonly Lazy<Geometry> ProjectPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("m16 7.5-7.428 4.25m14.856 8.5L16 24.5m7.428-12.75L16 16m7.428-4.25L16 7.5m0 17-7.428-4.25M16 16l-7.428-4.25m14.856 0v8.5m-14.856-8.5v8.5M16 16v8.5").WithFreeze());
        private static readonly Lazy<Geometry> RemovePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 11,16 H 21").WithFreeze());

        private readonly IMenuTheme theme;

        public MenuResourcesFactory(IMenuTheme theme)
        {
            this.theme = theme;
        }

        public FrameworkElement CreatePlayIcon()
        {
            return CreatePath(PlayPathGeometry.Value);
        }

        public FrameworkElement CreatePauseIcon()
        {
            return CreatePath(PausePathGeometry.Value);
        }

        public FrameworkElement CreateStepIcon()
        {
            return CreatePath(StepPathGeometry.Value);
        }

        public FrameworkElement CreateRestartIcon()
        {
            return CreatePath(RestartPathGeometry.Value);
        }

        public FrameworkElement CreateUniformsIcon()
        {
            return CreatePath(UniformsPathGeometry.Value);
        }

        public FrameworkElement CreateStartPageIcon()
        {
            return CreatePath(StartPagePathGeometry.Value);
        }

        public FrameworkElement CreateNewIcon()
        {
            return CreatePath(NewPathGeometry.Value);
        }

        public FrameworkElement CreateOpenIcon()
        {
            return CreatePath(OpenPathGeometry.Value);
        }

        public FrameworkElement CreateRecentIcon()
        {
            return CreatePath(RecentPathGeometry.Value);
        }

        public FrameworkElement CreateReloadIcon()
        {
            return CreatePath(ReloadPathGeometry.Value);
        }

        public FrameworkElement CreateSaveIcon()
        {
            return CreatePath(SavePathGeometry.Value);
        }

        public FrameworkElement CreateFilesIcon()
        {
            return CreatePath(FilesPathGeometry.Value);
        }

        public FrameworkElement CreateExportIcon()
        {
            return CreatePath(ExportPathGeometry.Value);
        }

        public FrameworkElement CreateCopyIcon()
        {
            return CreatePath(CopyPathGeometry.Value);
        }

        public FrameworkElement CreateBuffersIcon()
        {
            return CreatePath(BuffersPathGeometry.Value);
        }

        public FrameworkElement CreateResolutionIcon()
        {
            return CreatePath(ResolutionPathGeometry.Value);
        }

        public FrameworkElement CreateFrameRateIcon()
        {
            return CreatePath(FrameRatePathGeometry.Value);
        }

        public FrameworkElement CreateSpeedIcon()
        {
            return CreatePath(SpeedPathGeometry.Value);
        }

        public FrameworkElement CreateViewerIcon()
        {
            return CreatePath(ViewerPathGeometry.Value);
        }

        public FrameworkElement CreateOptionsIcon()
        {
            return CreatePath(OptionsPathGeometry.Value);
        }

        public FrameworkElement CreateHelpIcon()
        {
            return CreatePath(HelpPathGeometry.Value);
        }

        public FrameworkElement CreateFolderIcon()
        {
            return CreatePath(FolderPathGeometry.Value);
        }

        public FrameworkElement CreateFileCodeIcon()
        {
            return CreatePath(FileCodePathGeometry.Value);
        }

        public FrameworkElement CreateFileSettingsIcon()
        {
            return CreatePath(FileSettingsPathGeometry.Value);
        }

        public FrameworkElement CreateFileProjectIcon()
        {
            return CreatePath(FileProjectPathGeometry.Value);
        }

        public FrameworkElement CreateFileUniformsIcon()
        {
            return CreatePath(FileUniformsPathGeometry.Value);
        }

        public FrameworkElement CreateProjectIcon()
        {
            return CreatePath(ProjectPathGeometry.Value);
        }

        public FrameworkElement CreateRemoveIcon()
        {
            return CreatePath(RemovePathGeometry.Value);
        }

        private Path CreatePath(Geometry geometry)
        {
            return new Path
            {
                Data = geometry,
                StrokeThickness = 1.25,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-4),
                Width = 32,
                Height = 32,
            }.WithReference(Shape.StrokeProperty, this.theme.IconForeground);
        }
    }
}
