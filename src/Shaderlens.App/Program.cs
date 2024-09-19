namespace Shaderlens
{
    public static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            Exception? unhandledException = null;
            string? renderInformation = null;

            try
            {
                var applicationPath = Assembly.GetEntryAssembly()!.Location;
                var settings = CreateSettings(applicationPath);
                var inputs = CreateApplicationInputs(applicationPath);

                var lightThemeSettings = JsonSettingsFile.Load(Path.Combine(Path.GetDirectoryName(applicationPath) ?? String.Empty, "Theme.Light.json"));
                var lightTheme = new LightApplicationTheme(lightThemeSettings.Content, lightThemeSettings.Path);
                lightThemeSettings.Save();

                var darkThemeSettings = JsonSettingsFile.Load(Path.Combine(Path.GetDirectoryName(applicationPath) ?? String.Empty, "Theme.Dark.json"));
                var darkTheme = new DarkApplicationTheme(darkThemeSettings.Content, darkThemeSettings.Path);
                darkThemeSettings.Save();

                var application = new System.Windows.Application { ShutdownMode = ShutdownMode.OnMainWindowClose };
                var viewerApplication = new Application(application, settings, inputs, darkTheme, lightTheme);

                application.Startup += (sender, e) =>
                {
                    var arg = args.FirstOrDefault();

                    if (settings.RecentProjects.Any())
                    {
                        viewerApplication.ShowViewport();
                    }

                    if (arg != null)
                    {
                        viewerApplication.OpenProject(arg);
                    }
                    else if (settings.ShowStartPage)
                    {
                        viewerApplication.ShowStartPage();
                    }
                };

                application.Run();

                unhandledException = viewerApplication.UnhandledException;
                renderInformation = viewerApplication.RenderInformation;
            }
            catch (Exception e)
            {
                unhandledException = e;
            }

            if (unhandledException != null)
            {
                ReportException(unhandledException, renderInformation);
                return 1;
            }

            return 0;
        }

        private static ApplicationSettings CreateSettings(string applicationPath)
        {
            var path = Path.Combine(Path.GetDirectoryName(applicationPath) ?? String.Empty, Path.GetFileNameWithoutExtension(applicationPath) + ".settings.json");
            return new ApplicationSettings(JsonSettingsFile.Load(path));
        }

        private static ApplicationInputs CreateApplicationInputs(string applicationPath)
        {
            var path = Path.Combine(Path.GetDirectoryName(applicationPath) ?? String.Empty, Path.GetFileNameWithoutExtension(applicationPath) + ".inputs.json");
            var jsonSettingsFile = JsonSettingsFile.Load(path);
            var valueSerializer = new InputValueSerializer();
            var serializer = new InputSpanJsonSerializer(valueSerializer);
            var inputJsonSettings = new InputJsonSettings(jsonSettingsFile.Content, serializer);
            var applicationInputs = new ApplicationInputs(inputJsonSettings, jsonSettingsFile.Path);

            jsonSettingsFile.Content.ClearUnusedValues();
            jsonSettingsFile.Save();

            return applicationInputs;
        }

        private static void ReportException(Exception e, string? renderInformation)
        {
            string? reportPath;

            try
            {
                var assembly = Assembly.GetEntryAssembly()!;
                var assemblyInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                reportPath = Path.Combine(Path.GetDirectoryName(assembly.Location)!, "CrashReport.txt");

                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"Version: {assemblyInformationalVersion}");
                stringBuilder.AppendLine($"Time: {DateTime.Now:O}");

                if (renderInformation != null)
                {
                    stringBuilder.AppendLine(renderInformation);
                }

                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Exception:");
                AppendException(stringBuilder, e);

                File.WriteAllText(reportPath, stringBuilder.ToString());
            }
            catch
            {
                reportPath = null;
            }

            var message = new StringBuilder();
            message.AppendLine("An unhandled exception has occurred:");

            if (reportPath != null)
            {
                message.AppendLine(String.Join(Environment.NewLine, e.Message));
                message.AppendLine();
                message.AppendLine("See more details at:");
                message.AppendLine(reportPath);
            }
            else
            {
                message.AppendLine(String.Join(Environment.NewLine, e.ToString().Split("\r\n").Take(20)));
            }

            MessageBox.Show(message.ToString(), "Shaderlens - Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void AppendException(StringBuilder stringBuilder, Exception e)
        {
            stringBuilder.AppendLine("   " + e.GetType().FullName);
            stringBuilder.AppendLine("   " + e.Message);

            if (e.StackTrace != null)
            {
                stringBuilder.AppendLine("StackTrace:");
                stringBuilder.AppendLine(e.StackTrace);
            }

            if (e.InnerException != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("InnerException:");
                AppendException(stringBuilder, e.InnerException);
            }
        }
    }
}
