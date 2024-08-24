namespace Shaderlens.Serialization.Json
{
    public interface IJsonSettings
    {
        IJsonSettings GetScope(string key);
        bool TryGet<T>(IJsonSerializer<T> serializer, string key, [MaybeNullWhen(false)] out T value);
        void Set<T>(IJsonSerializer<T> serializer, string key, T value);
        void Clear(string key);
        void ClearUnusedValues();
    }

    public static class JsonSettingsExtensions
    {
        public static T Get<T>(this IJsonSettings settings, IJsonSerializer<T> serializer, string name, T defaultValue)
        {
            return settings.TryGet(serializer, name, out var value) ? value : defaultValue;
        }

        public static IEnumerable<T> Get<T>(this IJsonSettings settings, IJsonSerializer<T> serializer, string name, IEnumerable<T> defaultValue)
        {
            return settings.TryGet(new ArrayJsonSerializer<T>(serializer), name, out var value) ? value : defaultValue;
        }

        public static void Set<T>(this IJsonSettings settings, IJsonSerializer<T> serializer, string name, IEnumerable<T> value)
        {
            settings.Set(new ArrayJsonSerializer<T>(serializer), name, value);
        }

        public static T GetOrSetDefault<T>(this IJsonSettings settings, IJsonSerializer<T> serializer, string name, T defaultValue)
        {
            if (settings.TryGet(serializer, name, out var value))
            {
                return value;
            }

            settings.Set(serializer, name, defaultValue);
            return defaultValue;
        }

        public static IEnumerable<T> GetOrSetDefault<T>(this IJsonSettings settings, IJsonSerializer<T> serializer, string name, IEnumerable<T> defaultValue)
        {
            if (settings.TryGet(new ArrayJsonSerializer<T>(serializer), name, out var value))
            {
                return value;
            }

            settings.Set(serializer, name, defaultValue);
            return defaultValue;
        }
    }

    public class JsonSettings : IJsonSettings
    {
        private readonly JsonObject element;
        private readonly HashSet<string> usedKeys;
        private readonly List<JsonSettings> scopes;

        public JsonSettings(JsonObject element)
        {
            this.element = element;
            this.usedKeys = new HashSet<string>();
            this.scopes = new List<JsonSettings>();
        }

        public IJsonSettings GetScope(string key)
        {
            JsonObject scopeElement;

            if (this.element.TryGetPropertyValue(key, out var propertyValue) && propertyValue?.GetValueKind() == JsonValueKind.Object)
            {
                scopeElement = propertyValue.AsObject();
            }
            else
            {
                scopeElement = new JsonObject();
                this.element[key] = scopeElement;
            }

            var scope = new JsonSettings(scopeElement);

            if (this.usedKeys.Add(key))
            {
                this.scopes.Add(scope);
            }

            return scope;
        }

        public bool TryGet<T>(IJsonSerializer<T> serializer, string key, [MaybeNullWhen(false)] out T value)
        {
            this.usedKeys.Add(key);

            if (this.element.TryGetPropertyValue(key, out var propertyValue) && propertyValue != null)
            {
                return serializer.TryDeserialize(propertyValue, out value);
            }

            value = default;
            return false;
        }

        public void Set<T>(IJsonSerializer<T> serializer, string key, T value)
        {
            this.usedKeys.Add(key);

            this.element[key] = serializer.Serialize(value);
        }

        public void Clear(string key)
        {
            this.element.Remove(key);
        }

        public void ClearUnusedValues()
        {
            foreach (var key in this.element.Select(property => property.Key).ToArray())
            {
                if (!this.usedKeys.Contains(key))
                {
                    this.element.Remove(key);
                }
            }

            foreach (var scope in this.scopes)
            {
                scope.ClearUnusedValues();
            }
        }
    }
}
