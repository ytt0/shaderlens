namespace Shaderlens.Presentation.Themes
{
    public interface IThemeResources
    {
        void SetResources(ResourceDictionary target);
    }

    public class ThemeResources : IThemeResources
    {
        private readonly IJsonSettings settings;
        private readonly string? scope;
        private readonly List<IThemeResource> resources;

        public ThemeResources(IJsonSettings settings, string? scope = null)
        {
            this.settings = settings;
            this.scope = scope;
            this.resources = new List<IThemeResource>();
        }

        public IThemeResource<T> AddResource<T>(IThemeResource<T> resource)
        {
            this.resources.Add(resource);
            return resource;
        }

        public IThemeResource<T> AddResource<T>(string key, IJsonSerializer<T> serializer, T defaultValue)
        {
            if (this.scope != null)
            {
                key = $"{this.scope}.{key}";
            }

            var value = this.settings.GetOrSetDefault(serializer, key, defaultValue);
            var resource = new ThemeResource<T>(key, value);
            this.resources.Add(resource);
            return resource;
        }

        public void SetResources(ResourceDictionary target)
        {
            foreach (var resource in this.resources)
            {
                resource.SetResource(target);
            }
        }
    }
}
