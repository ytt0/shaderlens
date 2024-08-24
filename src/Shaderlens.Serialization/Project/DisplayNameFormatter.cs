namespace Shaderlens.Serialization.Project
{
    public interface IDisplayNameFormatter
    {
        string GetDisplayName(string name);
    }

    public partial class DisplayNameFormatter : IDisplayNameFormatter
    {
        public string GetDisplayName(string name)
        {
            var matches = new List<Match>();

            var value = name;

            foreach (var pattern in GetRegexPatterns())
            {
                value = pattern.Replace(value, match =>
                {
                    matches.Add(match);
                    return new string(' ', match.Length);
                });
            }

            return String.Join(' ', matches.OrderBy(match => match.Index).Select(match => match.Value).Select(CapitalizeWord));
        }

        private static string CapitalizeWord(string word)
        {
            return Char.ToUpper(word[0]) + word.Substring(1);
        }

        private static IEnumerable<Regex> GetRegexPatterns()
        {
            yield return CapitalizedWordRegex();
            yield return LowerCaseWordRegex();
            yield return UpperCaseWordRegex();
            yield return NumberRegex();
        }

        [GeneratedRegex("[A-Z][a-z]+")]
        private static partial Regex CapitalizedWordRegex();

        [GeneratedRegex("[a-z]+")]
        private static partial Regex LowerCaseWordRegex();

        [GeneratedRegex("[A-Z]+")]
        private static partial Regex UpperCaseWordRegex();

        [GeneratedRegex("[0-9]+")]
        private static partial Regex NumberRegex();
    }
}
