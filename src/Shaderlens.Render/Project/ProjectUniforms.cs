namespace Shaderlens.Render.Project
{
    public interface IProjectUniforms
    {
        string Key { get; }
        IUniform Uniforms { get; }
    }

    public interface IProjectUniformsBuilder
    {
        void AddUniforms(SourceLines sourceLines);
        string GetUniformsKey();
        IProjectUniforms CreateProjectUniforms(IProjectSettings settings);
    }

    public partial class ProjectUniformsBuilder : IProjectUniformsBuilder
    {
        private interface ISourceUniform
        {
            void AddTo(ISourceUniformBuilder builder);
        }

        private class SourceUniform : ISourceUniform
        {
            private readonly SourceLine uniformSourceLine;
            private readonly SourceLine? annotationSourceLine;
            private readonly string type;
            private readonly string name;
            private readonly string value;

            public SourceUniform(SourceLine uniformSourceLine, SourceLine? annotationSourceLine, string type, string name, string value)
            {
                this.uniformSourceLine = uniformSourceLine;
                this.annotationSourceLine = annotationSourceLine;
                this.type = type;
                this.name = name;
                this.value = value;
            }

            public void AddTo(ISourceUniformBuilder builder)
            {
                builder.AddUniform(this.uniformSourceLine, this.annotationSourceLine, this.type, this.name, this.value);
            }
        }

        private class SourceUniformGroup : ISourceUniform
        {
            private readonly SourceLine annotationSourceLine;

            public SourceUniformGroup(SourceLine annotationSourceLine)
            {
                this.annotationSourceLine = annotationSourceLine;
            }

            public void AddTo(ISourceUniformBuilder builder)
            {
                builder.AddGroup(this.annotationSourceLine);
            }
        }

        private class ProjectUniforms : IProjectUniforms
        {
            public string Key { get; }
            public IUniform Uniforms { get; }

            public ProjectUniforms(string key, IUniform uniforms)
            {
                this.Key = key;
                this.Uniforms = uniforms;
            }
        }

        private readonly IHashSource hashSource;

        private readonly List<List<ISourceUniform>> sourceGroups;
        private readonly List<string> lines;
        private readonly SourceUniformFactory factory;

        public ProjectUniformsBuilder(IDispatcherThread renderThread, IHashSource hashSource)
        {
            this.hashSource = hashSource;

            this.sourceGroups = new List<List<ISourceUniform>>();
            this.lines = new List<string>();
            this.factory = new SourceUniformFactory(renderThread);
        }

        public void AddUniforms(SourceLines sourceLines)
        {
            var sourceGroup = new List<ISourceUniform>();

            SourceLine? annotationSourceLine = null;

            foreach (var sourceLine in sourceLines)
            {
                if (String.IsNullOrWhiteSpace(sourceLine.Line))
                {
                    continue;
                }

                var match = AnnotationTypeRegex().Match(sourceLine.Line);
                if (match.Success)
                {
                    if (match.Groups["type"].Value == "uniform-group")
                    {
                        sourceGroup.Add(new SourceUniformGroup(sourceLine));
                        this.lines.Add(sourceLine.Line.Trim());
                        annotationSourceLine = null;
                        continue;
                    }

                    annotationSourceLine = sourceLine;
                    continue;
                }

                match = UniformRegex().Match(sourceLine.Line);
                if (match.Success)
                {
                    if (annotationSourceLine != null)
                    {
                        this.lines.Add(annotationSourceLine.Value.Line.Trim());
                    }

                    this.lines.Add(sourceLine.Line.Trim());

                    sourceGroup.Add(new SourceUniform(sourceLine, annotationSourceLine, match.Groups["type"].Value, match.Groups["name"].Value, match.Groups["value"].Value.Trim()));
                }

                annotationSourceLine = null;
            }

            this.sourceGroups.Add(sourceGroup);
        }

        public string GetUniformsKey()
        {
            return this.hashSource.GetHash(String.Join(Environment.NewLine, this.lines));
        }

        public IProjectUniforms CreateProjectUniforms(IProjectSettings settings)
        {
            return new ProjectUniforms(GetUniformsKey(), CreateUniform(settings));
        }

        private IUniform CreateUniform(IProjectSettings settings)
        {
            var builder = new SourceUniformBuilder(this.factory, settings);

            foreach (var sourceGroup in this.sourceGroups)
            {
                builder.SetRootTargetGroup();

                foreach (var source in sourceGroup)
                {
                    source.AddTo(builder);
                }
            }

            return builder.CreateUniform();
        }

        [GeneratedRegex("^uniform(\\s+(?<type>\\w+))(\\s+(?<name>\\w+))(\\s*=\\s*(?<value>[^;]+))?\\s*;")]
        private static partial Regex UniformRegex();

        [GeneratedRegex("^\\s*//@\\s*(?<type>[\\w-]+)\\b")]
        private static partial Regex AnnotationTypeRegex();
    }

}
