﻿namespace Shaderlens.Render.Project
{
    public interface IPipelineLoader
    {
        IRenderPipeline LoadPipeline(IProjectSource source, IUniform uniforms, IProjectResources resources, IShaderCache shaderCache, IFileSystem fileSystem, IProjectLoadLogger logger);
    }

    public class PipelineLoader : IPipelineLoader
    {
        private readonly IThreadAccess threadAccess;
        private readonly IProgramObjectsSourceFactory programSourceFactory;
        private readonly IShaderCompiler shaderCompiler;
        private readonly ICSharpTransformer csharpTransformer;
        private readonly ITextureReaderFactory textureReaderFactory;
        private readonly IKeyboardTextureResource keyboardTextureResource;
        private readonly IMouseStateSource mouseStateSource;
        private readonly IViewerSourceLines viewerSourceLines;
        private readonly IVertexArrayResource vertexArray;

        public PipelineLoader(IThreadAccess threadAccess, IProgramObjectsSourceFactory programSourceFactory, IShaderCompiler shaderCompiler, ICSharpTransformer csharpTransformer, ITextureReaderFactory textureReaderFactory, IKeyboardTextureResource keyboardTextureResource, IMouseStateSource mouseStateSource, IViewerSourceLines viewerSourceLines, IVertexArrayResource vertexArray)
        {
            this.threadAccess = threadAccess;
            this.programSourceFactory = programSourceFactory;
            this.shaderCompiler = shaderCompiler;
            this.csharpTransformer = csharpTransformer;
            this.textureReaderFactory = textureReaderFactory;
            this.keyboardTextureResource = keyboardTextureResource;
            this.mouseStateSource = mouseStateSource;
            this.viewerSourceLines = viewerSourceLines;
            this.vertexArray = vertexArray;
        }

        public IRenderPipeline LoadPipeline(IProjectSource source, IUniform uniforms, IProjectResources resources, IShaderCache shaderCache, IFileSystem fileSystem, IProjectLoadLogger logger)
        {
            var framebuffers = new PassCollection<IReadWriteFramebufferResources>(CreateFramebuffer(resources, "Image", source.Passes.Image!.Outputs), CreatePassFramebuffers(source, resources));
            var renderSizes = new PassCollection<IRenderSizeSource>(CreateRenderSizeSource(source, source.Passes.Image!), CreateBuffersRenderSizeSource(source));

            var passPrograms = new Dictionary<string, IRenderProgram>();
            foreach (var key in source.Passes.BuffersKeys)
            {
                passPrograms.Add(key, LoadRenderProgam(source, source.Passes[key], new PassDefaultFramebufferBinding(key), framebuffers, uniforms, shaderCache, resources, logger));
            }

            var imageProgram = LoadRenderProgam(source, source.Passes.Image!, ImageDefaultFramebufferBinding.Instance, framebuffers, uniforms, shaderCache, resources, logger);
            var passProgramsOrdered = passPrograms.OrderBy(pair => pair.Key).Select(pair => pair.Value).Concat(imageProgram).ToArray();

            var viewerPrograms = new Dictionary<string, IRenderProgram>();
            foreach (var viewer in source.Viewers)
            {
                var viewerProgramSource = CreateProgramSource(viewer.Program, source.Common);
                var viewerProgram = CreateProgram(viewer.Program, ChannelBindingSource.Empty, viewerProgramSource, framebuffers, uniforms, shaderCache, resources, logger);
                viewerPrograms.Add(viewer.Key, viewerProgram);
            }

            var clearViewerPass = CreateProgram("Clear Viewer", this.programSourceFactory.CreateFragment(this.viewerSourceLines.Clear), framebuffers, ChannelBindingSource.Empty, Uniform.Empty, null, null, shaderCache, resources, logger);
            var valuesOverlayViewerPass = CreateProgram("Values Overlay Viewer", this.programSourceFactory.CreateFragment(this.viewerSourceLines.ValuesOverlay), framebuffers, ChannelBindingSource.Empty, Uniform.Empty, null, null, shaderCache, resources, logger);
            var viewerPassCollection = new ViewerPassCollection(clearViewerPass, valuesOverlayViewerPass, viewerPrograms);

            var resourceKey = new TypedResourceKey(typeof(IFramebufferResource), "ViewerCopy");
            if (!resources.TryGetResource<IFramebufferResource>(resourceKey, out var viewerCopyFramebuffer))
            {
                viewerCopyFramebuffer = new FramebufferResource(this.threadAccess, 1);
                resources.AddResource(resourceKey, viewerCopyFramebuffer);
            }

            return new RenderPipeline(this.threadAccess, passProgramsOrdered, framebuffers.ToArray(), renderSizes.ToArray(), viewerPassCollection, viewerCopyFramebuffer, this.keyboardTextureResource, this.mouseStateSource);
        }

        private Dictionary<string, IReadWriteFramebufferResources> CreatePassFramebuffers(IProjectSource project, IProjectResources resources)
        {
            var framebuffers = new Dictionary<string, IReadWriteFramebufferResources>();

            foreach (var buffer in project.Passes.Buffers)
            {
                framebuffers[buffer.Key] = CreateFramebuffer(resources, buffer.Key, buffer.Outputs);
            }

            return framebuffers;
        }

        private IReadWriteFramebufferResources CreateFramebuffer(IProjectResources resources, string name, int texturesCount)
        {
            var resourceKey = new TypedResourceKey(typeof(IReadWriteFramebufferResources), $"{name},{texturesCount}");

            if (!resources.TryGetResource<IReadWriteFramebufferResources>(resourceKey, out var framebuffer))
            {
                framebuffer = new ReadWriteFramebufferResources(this.threadAccess, texturesCount);
                resources.AddResource(resourceKey, framebuffer);
            }

            return framebuffer;
        }

        private IRenderProgram LoadRenderProgam(IProjectSource project, IProjectPass pass, IChannelBindingSource defaultBinding, IPassCollection<IReadWriteFramebufferResources> framebuffers, IUniform uniforms, IShaderCache shaderCache, IProjectResources resources, IProjectLoadLogger logger)
        {
            var passProgramSource = CreateProgramSource(pass.Program, project.Common);
            return CreateProgram(pass.Program, defaultBinding, passProgramSource, framebuffers, uniforms, shaderCache, resources, logger);
        }

        private IRenderProgram CreateProgram(IProjectProgram renderProgram, IChannelBindingSource defaultBinding, IProgramSource programSource, IPassCollection<IReadWriteFramebufferResources> framebuffers, IUniform uniforms, IShaderCache shaderCache, IProjectResources resources, IProjectLoadLogger logger)
        {
            var binding = !renderProgram.Binding.IsEmpty ? renderProgram.Binding : defaultBinding;
            return CreateProgram(renderProgram.DisplayName, programSource, framebuffers, binding, uniforms, renderProgram.WorkGroupSize, renderProgram.WorkGroups, shaderCache, resources, logger);
        }

        private IRenderProgram CreateProgram(string displayName, IProgramSource programSource, IPassCollection<IReadWriteFramebufferResources> framebuffers, IChannelBindingSource binding, IUniform uniforms, Vector<int>? workGroupSize, Vector<int>? workGroups, IShaderCache shaderCache, IProjectResources resources, IProjectLoadLogger logger)
        {
            logger.SetState(displayName);

            var program = this.shaderCompiler.Compile(displayName, programSource, resources, shaderCache, logger);
            var readFramebuffers = new PassCollection<IFramebufferResource>(framebuffers.Image!.ReadFramebuffer, framebuffers.BuffersKeys.Select(key => new KeyValuePair<string, IFramebufferResource>(key, framebuffers[key].ReadFramebuffer)).ToDictionary());
            var channelBindingBuilder = new ChannelBindingBuilder(this.threadAccess, program.Id, readFramebuffers, resources, this.textureReaderFactory, this.keyboardTextureResource);
            channelBindingBuilder.SetViewerDefaultFramebufferBinding();

            binding.AddBinding(channelBindingBuilder);

            var uniformBindings = new UniformBindingCollection(
                channelBindingBuilder.Binding,
                new RenderContextUniformBinding(this.threadAccess, program.Id),
                uniforms.CreateBinding(program.Id));

            return programSource.Compute != null ?
                new ComputeRenderProgram(this.threadAccess, program, uniformBindings, workGroupSize, workGroups, displayName) :
                new FragmentRenderProgram(this.threadAccess, program, uniformBindings, this.vertexArray, displayName);
        }

        private IProgramSource CreateProgramSource(IProjectProgram program, IProjectProgramSource commonSource)
        {
            var common = program.IncludeCommonSource ? GetSourceLines(commonSource) : SourceLines.Empty;
            var source = GetSourceLines(program.Source);

            switch (program.Type)
            {
                case ProjectProgramType.Fragment: return this.programSourceFactory.CreateFragment(common, source);
                case ProjectProgramType.Compute: return this.programSourceFactory.CreateCompute(common, source, program.WorkGroupSize ?? Vector.Create(3, 1));
                default: throw new NotSupportedException($"Unexpected {nameof(ProjectProgramType)} \"{program.Type}\"");
            }
        }

        private SourceLines GetSourceLines(IProjectProgramSource resources)
        {
            return new SourceLines(resources.Select(GetSourceLines).ToArray());
        }

        private SourceLines GetSourceLines(IFileResource<string> resource)
        {
            var sourceLines = new SourceLines(resource);

            if (Path.GetExtension(resource.Key.AbsolutePath)!.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
            {
                sourceLines = this.csharpTransformer.Transform(sourceLines);
            }

            return sourceLines;
        }

        private static Dictionary<string, IRenderSizeSource> CreateBuffersRenderSizeSource(IProjectSource project)
        {
            var framebuffers = new Dictionary<string, IRenderSizeSource>();

            foreach (var buffer in project.Passes.Buffers)
            {
                framebuffers[buffer.Key] = CreateRenderSizeSource(project, buffer);
            }

            return framebuffers;
        }

        private static IRenderSizeSource CreateRenderSizeSource(IProjectSource project, IProjectPass pass)
        {
            var scalable = pass.Scalable ?? project.Scalable ?? false;
            var size = pass.RenderSize ?? project.RenderSize ?? null;

            return size == null ? ViewportRenderSizeSource.Instance :
                scalable ? new ScalableRenderSizeSource(size.Value.Width, size.Value.Height) :
                new FixedRenderSizeSource(size.Value.Width, size.Value.Height);
        }
    }
}
