namespace Shaderlens.Render.Project
{
    public interface ISourceUniformBuilder
    {
        void AddGroup(SourceLine annotationSourceLine);
        void AddUniform(SourceLine uniformSourceLine, SourceLine? annotationSourceLine, string type, string name, string value);
        void SetRootTargetGroup();
        IUniform CreateUniform();
    }

    public partial class SourceUniformBuilder : ISourceUniformBuilder
    {
        private interface IUniformSource
        {
            IUniform CreateUniform(ISourceUniformFactory factory, IProjectSettings settings);
        }

        private interface IUniformGroupSource : IUniformSource
        {
            void AddUniform(IUniformSource uniform, int sourceIndex, int? index);
            void AddGroup(IUniformGroupSource groupSource, int sourceIndex, int? index);
            bool IsContainedBy(IUniformGroupSource group);
        }

        private class IndexedItem<T> : IComparable<IndexedItem<T>>
        {
            public T Item { get; }

            private readonly int sourceIndex;
            private readonly int? index;

            public IndexedItem(T item, int sourceIndex, int? index)
            {
                this.Item = item;
                this.sourceIndex = sourceIndex;
                this.index = index;
            }

            public int CompareTo(IndexedItem<T>? other)
            {
                if (other == null)
                {
                    return 1;
                }

                // put positive indexes first, negative indexes last, and undefined indexes in the middle
                // 0, 1, 2, 3, null, null, null, -3, -2, -1

                var thisIndexGroup = this.index >= 0 ? 0 : this.index < 0 ? 2 : 1;
                var otherIndexGroup = other.index >= 0 ? 0 : other.index < 0 ? 2 : 1;

                var result = thisIndexGroup.CompareTo(otherIndexGroup);
                if (result == 0)
                {
                    result = (this.index ?? 0).CompareTo(other.index ?? 0);
                }

                if (result == 0)
                {
                    result = this.sourceIndex.CompareTo(other.sourceIndex);
                }

                return result;
            }
        }

        private class UniformSource : IUniformSource
        {
            private readonly string name;
            private readonly string type;

            private bool isInitialized;
            private string value;
            private SourceLine sourceLine;
            private int sourceIndex;
            private IUniformGroupSource collection;
            private int? index;
            private ISourceLineAnnotationReader? annotationReader;

            public UniformSource(SourceLine sourceLine, int sourceIndex, string name, string type, string value, IUniformGroupSource collection)
            {
                this.sourceLine = sourceLine;
                this.sourceIndex = sourceIndex;
                this.name = name;
                this.type = type;
                this.value = value;
                this.collection = collection;
            }

            public void Initialize(SourceLine sourceLine, int sourceIndex, IUniformGroupSource collection, int? index, string type, string value, ISourceLineAnnotationReader? annotationReader)
            {
                if (!this.isInitialized)
                {
                    if (this.type != type)
                    {
                        throw new SourceLineException($"Uniform \"{this.name}\" has already been defined at {this.sourceLine.Source.Key}:{this.sourceLine.Index} with a different type ({this.type})", sourceLine);
                    }

                    if (!String.IsNullOrEmpty(this.value) && this.value != value)
                    {
                        throw new SourceLineException($"Uniform \"{this.name}\" has already been defined at {this.sourceLine.Source.Key}:{this.sourceLine.Index} with a different default value ({this.value})", sourceLine);
                    }

                    this.value = value;
                    this.sourceLine = sourceLine;
                    this.sourceIndex = sourceIndex;
                    this.collection = collection;
                    this.index = index;
                    this.annotationReader = annotationReader;
                    this.isInitialized = true;
                }
                else if (this.sourceLine.Index != sourceLine.Index || this.sourceLine.Source != sourceLine.Source)
                {
                    throw new SourceLineException($"Uniform \"{this.name}\" has already been defined at {this.sourceLine.Source.Key}:{this.sourceLine.Index}, only one of these definitions should have a default value and annotation properties", annotationReader?.SourceLine ?? this.sourceLine);
                }
            }

            public IUniform CreateUniform(ISourceUniformFactory factory, IProjectSettings settings)
            {
                return factory.CreateUniform(this.name, this.type, this.value, this.annotationReader ?? SourceLineAnnotationReader.Empty, this.sourceLine, settings);
            }

            public void AddToParent()
            {
                this.collection.AddUniform(this, this.sourceIndex, this.index);
            }
        }

        private class UniformGroupSource : IUniformGroupSource
        {
            private readonly string name;
            private readonly List<IndexedItem<IUniformSource>> uniforms;
            private readonly List<IndexedItem<IUniformSource>> groups;

            private bool isInitialized;
            private SourceLine sourceLine;
            private int sourceIndex;
            private string? displayName;
            private IUniformGroupSource parent;
            private int? index;

            public UniformGroupSource(SourceLine sourceLine, int sourceIndex, string name, IUniformGroupSource parent)
            {
                this.sourceLine = sourceLine;
                this.name = name;
                this.parent = parent;
                this.sourceIndex = sourceIndex;
                this.uniforms = new List<IndexedItem<IUniformSource>>();
                this.groups = new List<IndexedItem<IUniformSource>>();
            }

            public void Initialize(SourceLine sourceLine, int sourceIndex, string? displayName, IUniformGroupSource parent, int? index)
            {
                if (!this.isInitialized)
                {
                    this.isInitialized = true;
                    this.sourceLine = sourceLine;
                    this.sourceIndex = sourceIndex;
                    this.displayName = displayName;
                    this.parent = parent;
                    this.index = index;
                }
                else if (this.sourceLine.Index != sourceLine.Index || this.sourceLine.Source != sourceLine.Source)
                {
                    throw new SourceLineException($"Group \"{this.name}\" has already been defined at {this.sourceLine.Source.Key}:{this.sourceLine.Index}, only one of these definitions should have annotation properties (other than name)", sourceLine);
                }
            }

            public IUniform CreateUniform(ISourceUniformFactory factory, IProjectSettings settings)
            {
                this.uniforms.Sort();
                this.groups.Sort();

                var items = this.uniforms.Concat(this.groups).Select(indexedItem => indexedItem.Item.CreateUniform(factory, settings)).ToArray();
                return factory.CreateUniformGroup(this.name, this.displayName, items, this.sourceLine, settings);
            }

            public void AddToParent()
            {
                if (this.parent.IsContainedBy(this))
                {
                    throw new SourceLineException($"Group \"{this.name}\" collection cannot be set, as it would cause a circular reference", this.sourceLine);
                }

                this.parent.AddGroup(this, this.sourceIndex, this.index);
            }

            public void AddUniform(IUniformSource uniform, int sourceIndex, int? index)
            {
                this.uniforms.Add(new IndexedItem<IUniformSource>(uniform, sourceIndex, index));
            }

            public void AddGroup(IUniformGroupSource group, int sourceIndex, int? index)
            {
                this.groups.Add(new IndexedItem<IUniformSource>(group, sourceIndex, index));
            }

            public bool IsContainedBy(IUniformGroupSource group)
            {
                return group == this || this.parent.IsContainedBy(group);
            }
        }

        private class RootGroupSource : IUniformGroupSource
        {
            private readonly List<IndexedItem<IUniformSource>> uniforms;
            private readonly List<IndexedItem<IUniformSource>> groups;

            public RootGroupSource()
            {
                this.uniforms = new List<IndexedItem<IUniformSource>>();
                this.groups = new List<IndexedItem<IUniformSource>>();
            }

            public IUniform CreateUniform(ISourceUniformFactory factory, IProjectSettings settings)
            {
                this.uniforms.Sort();
                this.groups.Sort();

                var items = this.uniforms.Concat(this.groups).Select(indexedItem => indexedItem.Item.CreateUniform(factory, settings)).ToArray();
                return factory.CreateRootUniformGroup(items);
            }

            public void AddUniform(IUniformSource uniform, int sourceIndex, int? index)
            {
                this.uniforms.Add(new IndexedItem<IUniformSource>(uniform, sourceIndex, index));
            }

            public void AddGroup(IUniformGroupSource group, int sourceIndex, int? index)
            {
                this.groups.Add(new IndexedItem<IUniformSource>(group, sourceIndex, index));
            }

            public bool IsContainedBy(IUniformGroupSource collection)
            {
                return false;
            }
        }

        private readonly ISourceUniformFactory factory;
        private readonly IProjectSettings settings;
        private readonly SourceLineAnnotationParser annotationParser;
        private readonly Dictionary<string, UniformGroupSource> groups;
        private readonly Dictionary<string, UniformSource> uniforms;
        private readonly RootGroupSource rootGroup;
        private IUniformGroupSource targetGroup;
        private int sourceIndex;

        public SourceUniformBuilder(ISourceUniformFactory factory, IProjectSettings settings)
        {
            this.annotationParser = new SourceLineAnnotationParser(false);
            this.groups = new Dictionary<string, UniformGroupSource>();
            this.uniforms = new Dictionary<string, UniformSource>();
            this.rootGroup = new RootGroupSource();
            this.targetGroup = this.rootGroup;
            this.factory = factory;
            this.settings = settings;
        }

        public void AddGroup(SourceLine sourceLine)
        {
            ParseGroupAnnotation(sourceLine, out var name, out var displayName, out var parentName, out var index);

            if (!this.groups.TryGetValue(name, out var group))
            {
                group = new UniformGroupSource(sourceLine, this.sourceIndex, name, this.rootGroup);
                this.groups.Add(name, group);
            }

            UniformGroupSource? parent = null;
            if (parentName != null && !this.groups.TryGetValue(parentName, out parent))
            {
                parent = new UniformGroupSource(sourceLine, this.sourceIndex, parentName, this.rootGroup);
                this.groups.Add(parentName, parent);
            }

            if (index != null || parent != null || displayName != null)
            {
                group.Initialize(sourceLine, this.sourceIndex, displayName, parent ?? (IUniformGroupSource)this.rootGroup, index);
            }

            this.targetGroup = group;
            this.sourceIndex++;
        }

        public void AddUniform(SourceLine uniformSourceLine, SourceLine? annotationSourceLine, string type, string name, string value)
        {
            if (!this.uniforms.TryGetValue(name, out var uniform))
            {
                uniform = new UniformSource(uniformSourceLine, this.sourceIndex, name, type, value, this.targetGroup);
                this.uniforms.Add(name, uniform);
            }

            if (annotationSourceLine != null)
            {
                try
                {
                    var annotation = this.annotationParser.Parse(annotationSourceLine.Value);
                    var annotationReader = new SourceLineAnnotationReader(annotation);

                    int? index = null;
                    if (annotationReader.TryGetValue("index", out var indexRawValue))
                    {
                        index = Int32.TryParse(indexRawValue, out var indexValue) ? indexValue : throw new SourceLineException($"Uniform \"{name}\" index property \"{indexRawValue}\" is invalid", annotationSourceLine.Value);
                    }

                    uniform.Initialize(uniformSourceLine, this.sourceIndex, this.targetGroup, index, type, value, annotationReader);
                }
                catch (ParseException e)
                {
                    throw new SourceLineException(e.Description, annotationSourceLine.Value, e);
                }
            }

            this.sourceIndex++;
        }

        public void SetRootTargetGroup()
        {
            this.targetGroup = this.rootGroup;
        }

        public IUniform CreateUniform()
        {
            foreach (var uniform in this.uniforms.Values)
            {
                uniform.AddToParent();
            }

            foreach (var group in this.groups.Values)
            {
                group.AddToParent();
            }

            return this.rootGroup.CreateUniform(this.factory, this.settings);
        }

        private void ParseGroupAnnotation(SourceLine sourceLine, out string name, out string? displayName, out string? parentName, out int? index)
        {
            try
            {
                index = null;

                var annotation = this.annotationParser.Parse(sourceLine);
                var annotationReader = new SourceLineAnnotationReader(annotation);

                if (!annotationReader.TryGetValue("name", out var nameValue) && !annotationReader.TryGetValue(out nameValue))
                {
                    throw new SourceLineException("Group name property is missing", annotation.SourceLine);
                }

                if (!IdentifierRegex().IsMatch(nameValue))
                {
                    throw new SourceLineException($"Value \"{nameValue}\" is an invalid group name, to specify a display name, set both \"name\", and \"display-name\" properties (//@uniform-group[, name]: <identifier>, display-name: \"<string>\"))", annotation.SourceLine);
                }

                name = nameValue.ToString()!;
                displayName = annotationReader.TryGetValue("display-name", out var displayNameValue) ? displayNameValue?.ToString() : null;
                parentName = annotationReader.TryGetValue("parent", out var parentNameValue) ? parentNameValue.ToString() : null;

                if (annotationReader.TryGetValue("index", out var indexRawValue))
                {
                    index = Int32.TryParse(indexRawValue, out var indexValue) ? indexValue : throw new SourceLineException($"Group \"{name}\" index property \"{indexRawValue}\" is invalid", annotation.SourceLine);
                }
            }
            catch (ParseException e)
            {
                throw new SourceLineException(e.Message, sourceLine, e);
            }
        }

        [GeneratedRegex(@"^[a-zA-Z_][\w-]*")]
        private static partial Regex IdentifierRegex();
    }
}
