namespace Shaderlens.Serialization.Project
{
    public interface ISettingsValue<T>
    {
        string Name { get; }
        T Value { get; set; }
        T BaseValue { get; }
        T DefaultValue { get; }
    }

    public static class SettingsValueExtensions
    {
        public static bool IsDefaultValue<T>(this ISettingsValue<T> settingsValue)
        {
            return Equals(settingsValue.Value, settingsValue.DefaultValue);
        }

        public static bool IsBaseValue<T>(this ISettingsValue<T> settingsValue)
        {
            return Equals(settingsValue.Value, settingsValue.BaseValue);
        }

        public static void ResetValue<T>(this ISettingsValue<T> settingsValue)
        {
            settingsValue.Value = settingsValue.DefaultValue;
        }
    }

    public class SettingsValue<T> : ISettingsValue<T>
    {
        public string Name { get; }
        public T Value { get; set; }
        public T DefaultValue { get; }
        public T BaseValue { get; }

        public SettingsValue(string name, T value, T defaultValue, T baseValue)
        {
            this.Name = name;
            this.Value = value;
            this.DefaultValue = defaultValue;
            this.BaseValue = baseValue;
        }
    }
}