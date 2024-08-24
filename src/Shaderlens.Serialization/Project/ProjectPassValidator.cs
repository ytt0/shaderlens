namespace Shaderlens.Serialization.Project
{
    public interface IProjectPassValidator
    {
        bool PassDefined(string key);
    }

    public class ProjectPassValidator : IProjectPassValidator
    {
        private readonly HashSet<string> definedPasses;

        public ProjectPassValidator(IEnumerable<string> definedPasses)
        {
            this.definedPasses = new HashSet<string>(definedPasses);
        }

        public bool PassDefined(string key)
        {
            return this.definedPasses.Contains(key);
        }
    }
}
