namespace Shaderlens.Serialization.Project
{
    public interface ISettingsValue
    {
        bool IsDefaultValue { get; }
        void ResetValue();
        void SaveValue();
    }

    public interface ISettingsValue<T> : ISettingsValue
    {
        string Name { get; }
        T Value { get; set; }
        T GetMergedValue(T newSettingsValue, T newDefaultValue);
    }

    public class SettingsValue<T> : ISettingsValue<T>
    {
        public string Name { get; }
        public T Value { get; set; }
        public bool IsDefaultValue { get { return Equals(this.Value, this.defaultValue); } }

        private readonly IJsonSettings settings;
        private readonly IJsonSerializer<T> serializer;
        private readonly T defaultValue;
        private readonly T settingsValue;

        public SettingsValue(IJsonSettings settings, IJsonSerializer<T> serializer, string name, T defaultValue, ISettingsValue<T>? previousValue)
        {
            this.settings = settings;
            this.serializer = serializer;
            this.Name = name;
            this.defaultValue = defaultValue;
            this.settingsValue = settings.Get(serializer, name, defaultValue);
            this.Value = previousValue != null ? previousValue.GetMergedValue(this.settingsValue, this.defaultValue) : this.settingsValue;
        }

        public T GetMergedValue(T newSettingsValue, T newDefaultValue)
        {
            var isSettingsValue = Equals(this.settingsValue, this.Value); // value came from settings
            var settingsValueChanged = !Equals(this.settingsValue, newSettingsValue);

            return
                // value was modified
                !this.IsDefaultValue && !isSettingsValue ? this.Value :

                // value was reset to default
                this.IsDefaultValue && !isSettingsValue ? newDefaultValue :

                // value came from settings
                !this.IsDefaultValue && isSettingsValue ? newSettingsValue :

                // both default and settings were the same
                settingsValueChanged ? newSettingsValue : newDefaultValue;
        }

        public void SaveValue()
        {
            if (this.IsDefaultValue)
            {
                this.settings.Clear(this.Name);
            }
            else
            {
                this.settings.Set(this.serializer, this.Name, this.Value);
            }
        }

        public void ResetValue()
        {
            this.Value = this.defaultValue;
        }
    }
}