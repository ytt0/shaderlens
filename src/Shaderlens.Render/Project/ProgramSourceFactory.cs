namespace Shaderlens.Render.Project
{
    public interface IProgramStageSource
    {
        string ResourceKey { get; }
        string Source { get; }
        SourceLines SourceLines { get; }
    }

    public interface IProgramSource
    {
        string ResourceKey { get; }
        IProgramStageSource Vertex { get; }
        IProgramStageSource Fragment { get; }
    }

    public interface IProgramObjectsSourceFactory
    {
        IProgramSource Create(SourceLines common, SourceLines source);
    }

    public static class ProgramObjectsSourceFactoryExtensions
    {
        public static IProgramSource Create(this IProgramObjectsSourceFactory factory, SourceLines source)
        {
            return factory.Create(SourceLines.Empty, source);
        }
    }

    public class ProgramSourceFactory : IProgramObjectsSourceFactory
    {
        private class ProgramStageSource : IProgramStageSource
        {
            public string ResourceKey { get; }
            public string Source { get; }
            public SourceLines SourceLines { get; }

            public ProgramStageSource(string resourceKey, string source, SourceLines sourceLines)
            {
                this.ResourceKey = resourceKey;
                this.Source = source;
                this.SourceLines = sourceLines;
            }
        }

        private class ProgramSource : IProgramSource
        {
            public string ResourceKey { get; }
            public IProgramStageSource Vertex { get; }
            public IProgramStageSource Fragment { get; }

            public ProgramSource(string resourceKey, IProgramStageSource vertex, IProgramStageSource fragment)
            {
                this.ResourceKey = resourceKey;
                this.Vertex = vertex;
                this.Fragment = fragment;
            }
        }

        private static readonly SourceLines VertexBodyLines = CreateSourceLines("Vertex (auto)",
@"layout (location = 0) in vec2 position;

void main()
{
    gl_Position = vec4(position.x, position.y, 0.0, 1.0);
}");

        private static readonly SourceLines FragmentDeclarationLines = CreateSourceLines("Fragment Declaration (auto)",
@"layout(location = 0) out vec4 FragColor0;
layout(location = 1) out vec4 FragColor1;
layout(location = 2) out vec4 FragColor2;
layout(location = 3) out vec4 FragColor3;
layout(location = 4) out vec4 FragColor4;
layout(location = 5) out vec4 FragColor5;
layout(location = 6) out vec4 FragColor6;
layout(location = 7) out vec4 FragColor7;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;
uniform sampler2D iChannel4;
uniform sampler2D iChannel5;
uniform sampler2D iChannel6;
uniform sampler2D iChannel7;
uniform sampler2D iViewerChannel;

uniform vec3 iResolution;
uniform float iTime;
uniform vec4 iMouse;
uniform vec4 iDate;
uniform float iSampleRate;
uniform int iFrame;
uniform float iTimeDelta;
uniform float iFrameRate;

uniform float iChannelTime[8];
uniform float iChannelDuration[8];
uniform vec3 iChannelResolution[8];

uniform vec3 iViewerChannelResolution;
uniform float iViewerScale;
uniform vec2 iViewerOffset;

void mainImage(out vec4 fragColor, in vec2 fragCoord);
");

        private static readonly SourceLines FragmentEntryLines = CreateSourceLines("Fragment Entry (auto)",
@"
void main()
{
    vec4 fragColor;
    mainImage(fragColor, gl_FragCoord.xy);
    FragColor0 = fragColor;
}");

        private readonly IHashSource hashSource;
        private readonly SourceLines fragmentHeaderLines;
        private readonly ProgramStageSource vertexStageSource;

        public ProgramSourceFactory(IHashSource hashSource, SourceLines vertexHeaderLines, SourceLines fragmentHeaderLines)
        {
            this.hashSource = hashSource;
            this.fragmentHeaderLines = fragmentHeaderLines;

            var vertexSourceLines = new SourceLines(new[] { vertexHeaderLines, VertexBodyLines });
            var vertexSource = vertexSourceLines.JoinLines();

            this.vertexStageSource = new ProgramStageSource(this.hashSource.GetHash(vertexSource), vertexSource, vertexSourceLines);
        }

        public IProgramSource Create(SourceLines common, SourceLines source)
        {
            var fragmentSourceLines = new SourceLines(new[] { this.fragmentHeaderLines, FragmentDeclarationLines, common, source, FragmentEntryLines });
            var fragmentStageSource = CreateProgramStageSource(fragmentSourceLines);

            var resourceKey = this.hashSource.GetHash(new[] { this.vertexStageSource.ResourceKey, fragmentStageSource.ResourceKey }.JoinLines());
            return new ProgramSource(resourceKey, this.vertexStageSource, fragmentStageSource);
        }

        private ProgramStageSource CreateProgramStageSource(SourceLines sourceLines)
        {
            var source = sourceLines.JoinLines();
            var resourceKey = this.hashSource.GetHash(source);
            return new ProgramStageSource(resourceKey, source, sourceLines);
        }

        private static SourceLines CreateSourceLines(string displayName, string content)
        {
            return new SourceLines(new FileResource<string>(new FileResourceKey(displayName), content));
        }
    }
}
