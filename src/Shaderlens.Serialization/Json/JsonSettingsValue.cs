namespace Shaderlens.Serialization.Json
{
    public interface IJsonSettingsValue
    {
        void SaveValue();
    }

    public interface IJsonSettingsValue<T> : IJsonSettingsValue
    {
        ISettingsValue<T> SettingsValue { get; }
    }

    public class JsonSettingsValue<T> : IJsonSettingsValue<T>
    {
        public ISettingsValue<T> SettingsValue { get; }

        private readonly IJsonSettings settings;
        private readonly IJsonSerializer<T> serializer;

        public JsonSettingsValue(IJsonSettings settings, IJsonSerializer<T> serializer, string name, T defaultValue, ISettingsValue<T>? previousValue)
        {
            this.settings = settings;
            this.serializer = serializer;

            var baseValue = settings.Get(serializer, name, defaultValue);
            var value = previousValue != null ? GetMergedValue(defaultValue, baseValue, previousValue) : baseValue;

            this.SettingsValue = new SettingsValue<T>(name, value, defaultValue, baseValue);
        }

        private static T GetMergedValue(T newDefaultValue, T newBaseValue, ISettingsValue<T> previousValue)
        {
            var isPreviousDefault = previousValue.IsDefaultValue();
            var isPreviousBase = previousValue.IsBaseValue();

            return
                // value was modified
                !isPreviousDefault && !isPreviousBase ? previousValue.Value :

                // default value was used
                isPreviousDefault && !isPreviousBase ? newDefaultValue :

                // base value was used
                !isPreviousDefault && isPreviousBase ? newBaseValue :

                // both default and base were the same, use the one that was changed
                !Equals(newBaseValue, previousValue.BaseValue) ? newBaseValue : newDefaultValue;
        }

        public void SaveValue()
        {
            if (this.SettingsValue.IsDefaultValue())
            {
                this.settings.Clear(this.SettingsValue.Name);
            }
            else
            {
                this.settings.Set(this.serializer, this.SettingsValue.Name, this.SettingsValue.Value);
            }
        }
    }
}
