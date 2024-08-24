namespace Shaderlens.Storage
{
    public interface IEnvironmentVariablesSource
    {
        string GetEnvironmentVariable(string name);
    }

    public class EnvironmentVariablesSource : IEnvironmentVariablesSource
    {
        public static readonly IEnvironmentVariablesSource Instance = new EnvironmentVariablesSource();

        private EnvironmentVariablesSource()
        {
        }

        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name) ?? throw new EnvironmentVariableException($"Environment variable \"{name}\" is not set");
        }
    }

    public class EnvironmentVariableException : Exception
    {
        public EnvironmentVariableException(string? message) :
            base(message)
        {
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
}
