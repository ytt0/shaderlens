namespace Shaderlens.Storage
{
    public interface IPathResolver
    {
        string ResolvePath(string path);
    }

    public partial class PathResolver : IPathResolver
    {
        private readonly IEnvironmentVariablesSource environmentVariables;
        private readonly string rootPath;

        public PathResolver(IEnvironmentVariablesSource environmentVariables, string rootPath)
        {
            this.environmentVariables = environmentVariables;
            this.rootPath = rootPath;
        }

        public string ResolvePath(string path)
        {
            path = EnvironmentVariableRegex().Replace(path, match => this.environmentVariables.GetEnvironmentVariable(match.Groups["name"].Value));
            path = path.NormalizeDirectorySeparator();
            path = Path.GetFullPath(path, this.rootPath);
            return path;
        }

        [GeneratedRegex("%(?<name>[\\w-]+)%")]
        private static partial Regex EnvironmentVariableRegex();
    }
}
