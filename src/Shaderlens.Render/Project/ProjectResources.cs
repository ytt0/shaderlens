namespace Shaderlens.Render.Project
{
    public interface IResourceBuildScope : IDisposable
    {
        void SetResource(object resource);
    }

    public interface IProjectResources : IDisposable
    {
        bool TryGetResource(object key, [MaybeNullWhen(false)] out object resource);
        void AddResource(object key, object resource);
        void SetUsedResource(object key);
        IResourceBuildScope PushResourceBuildScope(object key);

        void ResetResourcesUsage();
        void DisposeUnusedResources();
    }

    public readonly struct TypedResourceKey
    {
        public readonly Type Type;
        public readonly object Key;

        public TypedResourceKey(Type type, object key)
        {
            this.Type = type;
            this.Key = key;
        }
    }

    public static class ProjectResourcesExtensions
    {
        public static bool TryGetResource<T>(this IProjectResources resources, object key, [MaybeNullWhen(false)] out T resource)
        {
            if (resources.TryGetResource(key, out var resourceObject))
            {
                resource = (T)resourceObject;
                return true;
            }

            resource = default;
            return false;
        }
    }

    public class ProjectResources : IProjectResources
    {
        private class Scope : IResourceBuildScope
        {
            public IEnumerable<object> Keys { get { return this.keys; } }

            private readonly object key;
            private readonly IThreadAccess threadAccess;
            private readonly ProjectResources owner;
            private readonly Scope? collection;
            private readonly HashSet<object> keys;

            public Scope(ProjectResources owner, IThreadAccess threadAccess, Scope? collection, object key)
            {
                this.owner = owner;
                this.threadAccess = threadAccess;
                this.collection = collection;
                this.key = key;

                this.keys = new HashSet<object>();
            }

            public void Dispose()
            {
                this.owner.PopScope(this);
            }

            public void AddKey(object key)
            {
                this.threadAccess.Verify();

                if (key == this.key)
                {
                    throw new Exception("Scope key cannot be added to itself");
                }

                this.keys.Add(key);
                this.collection?.AddKey(key);
            }

            public void SetResource(object resource)
            {
                this.threadAccess.Verify();

                this.owner.SetScope(this.key, resource, this);
                this.collection?.AddKey(this.key);
            }
        }

        private readonly Dictionary<object, object> resources;
        private readonly Dictionary<object, Scope> scopes;

        private readonly HashSet<object> usedResourcesKeys;
        private readonly Stack<Scope?> scopesStack;
        private readonly IThreadAccess threadAccess;
        private Scope? scope;

        public ProjectResources(IThreadAccess threadAccess)
        {
            this.resources = new Dictionary<object, object>();
            this.scopes = new Dictionary<object, Scope>();
            this.usedResourcesKeys = new HashSet<object>();
            this.scopesStack = new Stack<Scope?>();
            this.threadAccess = threadAccess;
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            foreach (var resource in this.resources.Values.OfType<IDisposable>())
            {
                resource.Dispose();
            }

            this.resources.Clear();
        }

        public void DisposeUnusedResources()
        {
            this.threadAccess.Verify();

            foreach (var pair in this.resources.ToArray())
            {
                if (!this.usedResourcesKeys.Contains(pair.Key))
                {
                    this.resources.Remove(pair.Key);
                    (pair.Value as IDisposable)?.Dispose();
                }
            }

            foreach (var pair in this.scopes.ToArray())
            {
                if (!this.usedResourcesKeys.Contains(pair.Key))
                {
                    this.scopes.Remove(pair.Key);
                }
            }
        }

        public void ResetResourcesUsage()
        {
            this.threadAccess.Verify();

            if (this.scopesStack.Count > 0)
            {
                throw new Exception("Cannot reset resources usage, usage scopes were not disposed");
            }

            this.usedResourcesKeys.Clear();
        }

        public bool TryGetResource(object key, [MaybeNullWhen(false)] out object resource)
        {
            this.threadAccess.Verify();

            if (this.resources.TryGetValue(key, out resource))
            {
                SetUsedResource(key);
                return true;
            }

            resource = null;
            return false;
        }

        public void AddResource(object key, object resource)
        {
            this.threadAccess.Verify();

            this.resources.Add(key, resource);
            SetUsedResource(key);
        }

        public void SetUsedResource(object key)
        {
            this.threadAccess.Verify();

            this.usedResourcesKeys.Add(key);
            this.scope?.AddKey(key);

            if (this.scopes.TryGetValue(key, out var scope))
            {
                foreach (var childKey in scope.Keys)
                {
                    SetUsedResource(childKey);
                }
            }
        }

        private void SetScope(object key, object resource, Scope scope)
        {
            this.threadAccess.Verify();

            this.usedResourcesKeys.Add(key);
            this.resources.Add(key, resource);
            this.scopes.Add(key, scope);
        }

        public IResourceBuildScope PushResourceBuildScope(object key)
        {
            this.threadAccess.Verify();

            this.scopesStack.Push(this.scope);
            this.scope = new Scope(this, this.threadAccess, this.scope, key);
            return this.scope;
        }

        private void PopScope(Scope scope)
        {
            this.threadAccess.Verify();

            if (this.scope != scope)
            {
                throw new Exception("Cannot restore resources scope, scopes were disposed out of order");
            }

            this.scope = this.scopesStack.Pop();
        }
    }

    public class CachingProjectResources : IProjectResources
    {
        private readonly IProjectResources resources;
        private readonly int maxCachedResources;

        private readonly Dictionary<object, int> resourcesIndex;
        private readonly HashSet<object> usedResourcesKeys;
        private int index;

        public CachingProjectResources(IProjectResources resources, int maxCachedResources)
        {
            this.resources = resources;
            this.maxCachedResources = maxCachedResources;

            this.resourcesIndex = new Dictionary<object, int>();
            this.usedResourcesKeys = new HashSet<object>();
        }

        public void Dispose()
        {
            this.resources.Dispose();
        }

        public bool TryGetResource(object key, [MaybeNullWhen(false)] out object resource)
        {
            this.usedResourcesKeys.Add(key);
            this.resourcesIndex[key] = this.index++;
            return this.resources.TryGetResource(key, out resource);
        }

        public void AddResource(object key, object resource)
        {
            this.usedResourcesKeys.Add(key);
            this.resourcesIndex[key] = this.index++;
            this.resources.AddResource(key, resource);
        }

        public void SetUsedResource(object key)
        {
            this.usedResourcesKeys.Add(key);
            this.resourcesIndex[key] = this.index++;
            this.resources.SetUsedResource(key);
        }

        public IResourceBuildScope PushResourceBuildScope(object key)
        {
            this.usedResourcesKeys.Add(key);
            this.resourcesIndex[key] = this.index++;
            return this.resources.PushResourceBuildScope(key);
        }

        public void ResetResourcesUsage()
        {
            this.usedResourcesKeys.Clear();
            this.resources.ResetResourcesUsage();
        }

        public void DisposeUnusedResources()
        {
            var unusedResourcesIndex = this.resourcesIndex.Where(pair => !this.usedResourcesKeys.Contains(pair.Key)).OrderByDescending(pair => pair.Value).ToArray();

            foreach (var pair in unusedResourcesIndex.Take(this.maxCachedResources))
            {
                this.resources.SetUsedResource(pair.Key);
            }

            foreach (var pair in unusedResourcesIndex.Skip(this.maxCachedResources))
            {
                this.resourcesIndex.Remove(pair.Key);
            }

            this.resources.DisposeUnusedResources();
        }
    }
}
