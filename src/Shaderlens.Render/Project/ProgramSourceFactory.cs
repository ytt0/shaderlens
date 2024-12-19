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
        IProgramStageSource? Vertex { get; }
        IProgramStageSource? Fragment { get; }
        IProgramStageSource? Compute { get; }
    }

    public interface IProgramObjectsSourceFactory
    {
        IProgramSource CreateFragment(SourceLines common, SourceLines source);
        IProgramSource CreateCompute(SourceLines common, SourceLines source, Vector<int> workGroupSize);
    }

    public static class ProgramObjectsSourceFactoryExtensions
    {
        public static IProgramSource CreateFragment(this IProgramObjectsSourceFactory factory, SourceLines source)
        {
            return factory.CreateFragment(SourceLines.Empty, source);
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
            public IProgramStageSource? Vertex { get; }
            public IProgramStageSource? Fragment { get; }
            public IProgramStageSource? Compute { get; }

            private ProgramSource(IHashSource hashSource, IProgramStageSource? vertex, IProgramStageSource? fragment, IProgramStageSource? compute)
            {
                this.ResourceKey = hashSource.GetHash(new[] { nameof(ProgramSource), vertex?.ResourceKey, fragment?.ResourceKey, compute?.ResourceKey }.JoinLines());
                this.Vertex = vertex;
                this.Fragment = fragment;
                this.Compute = compute;
            }

            public static ProgramSource CreateFragment(IHashSource hashSource, IProgramStageSource vertex, IProgramStageSource fragment)
            {
                return new ProgramSource(hashSource, vertex, fragment, null);
            }

            public static ProgramSource CreateCompute(IHashSource hashSource, IProgramStageSource shader)
            {
                return new ProgramSource(hashSource, null, null, shader);
            }
        }

        private static readonly SourceLines VertexBodyLines = CreateSourceLines("Vertex (auto)",
@"layout (location = 0) in vec2 position;

void main()
{
    gl_Position = vec4(position.x, position.y, 0.0, 1.0);
}");

        private static readonly SourceLines UniformsLines = CreateSourceLines("Uniforms (auto)", @"
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
uniform vec2 iViewerOffset;");

        private static readonly SourceLines ComputeOutputsLines = CreateSourceLines("Compute Outputs (auto)", @"
layout(rgba32f, binding = 0) uniform writeonly image2D OutputTexture0;
layout(rgba32f, binding = 1) uniform writeonly image2D OutputTexture1;
layout(rgba32f, binding = 2) uniform writeonly image2D OutputTexture2;
layout(rgba32f, binding = 3) uniform writeonly image2D OutputTexture3;
layout(rgba32f, binding = 4) uniform writeonly image2D OutputTexture4;
layout(rgba32f, binding = 5) uniform writeonly image2D OutputTexture5;
layout(rgba32f, binding = 6) uniform writeonly image2D OutputTexture6;
layout(rgba32f, binding = 7) uniform writeonly image2D OutputTexture7;");

        private static readonly SourceLines FragmentOutputsLines = CreateSourceLines("Fragment Outputs (auto)", @"
layout(location = 0) out vec4 FragColor0;
layout(location = 1) out vec4 FragColor1;
layout(location = 2) out vec4 FragColor2;
layout(location = 3) out vec4 FragColor3;
layout(location = 4) out vec4 FragColor4;
layout(location = 5) out vec4 FragColor5;
layout(location = 6) out vec4 FragColor6;
layout(location = 7) out vec4 FragColor7;");

        private static readonly SourceLines FragmentEntryDeclarationLines = CreateSourceLines("Fragment Entry Declaration (auto)", @"
void mainImage(out vec4 fragColor, in vec2 fragCoord);");

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
        private readonly SourceLines computeHeaderLines;
        private readonly ProgramStageSource vertexStageSource;

        public ProgramSourceFactory(IHashSource hashSource, SourceLines vertexHeaderLines, SourceLines fragmentHeaderLines, SourceLines computeHeaderLines)
        {
            this.hashSource = hashSource;
            this.fragmentHeaderLines = fragmentHeaderLines;
            this.computeHeaderLines = computeHeaderLines;

            var vertexSourceLines = new SourceLines(new[] { vertexHeaderLines, VertexBodyLines });
            var vertexSource = vertexSourceLines.JoinLines();

            this.vertexStageSource = new ProgramStageSource(this.hashSource.GetHash(vertexSource), vertexSource, vertexSourceLines);
        }

        public IProgramSource CreateFragment(SourceLines common, SourceLines source)
        {
            var fragmentSourceLines = new SourceLines(new[] { this.fragmentHeaderLines, FragmentOutputsLines, UniformsLines, FragmentEntryDeclarationLines, common, source, FragmentEntryLines });
            var fragmentStageSource = CreateProgramStageSource(fragmentSourceLines);

            return ProgramSource.CreateFragment(this.hashSource, this.vertexStageSource, fragmentStageSource);
        }

        public IProgramSource CreateCompute(SourceLines common, SourceLines source, Vector<int> workGroupSize)
        {
            var workGroupSizeLines = CreateSourceLines("Compute WorkGroupSize (project)", $"layout(local_size_x = {workGroupSize[0]}, local_size_y = {workGroupSize[1]}, local_size_z = {workGroupSize[2]}) in;");
            var computeSourceLines = new SourceLines(new[] { this.computeHeaderLines, workGroupSizeLines, ComputeOutputsLines, UniformsLines, common, source });
            var computeStageSource = CreateProgramStageSource(computeSourceLines);

            return ProgramSource.CreateCompute(this.hashSource, computeStageSource);
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
