namespace Shaderlens.Render
{
    using static OpenGL.Gl;

    public interface IShaderCompiler
    {
        IRenderResource Compile(string displayName, IProgramSource source, IProjectResources resources, IShaderCache shaderCache, IProjectLoadLogger logger);
    }

    public class ShaderCompiler : IShaderCompiler
    {
        private readonly IThreadAccess threadAccess;
        private readonly IShaderStatusParser shaderStatusParser;

        public ShaderCompiler(IThreadAccess threadAccess, IShaderStatusParser shaderStatusParser)
        {
            this.threadAccess = threadAccess;
            this.shaderStatusParser = shaderStatusParser;
        }

        public IRenderResource Compile(string displayName, IProgramSource source, IProjectResources resources, IShaderCache shaderCache, IProjectLoadLogger logger)
        {
            this.threadAccess.Verify();

            if (resources.TryGetResource<IRenderResource>(source.ResourceKey, out var program))
            {
                return program;
            }

            using (var buildScope = resources.PushResourceBuildScope(source.ResourceKey))
            {
                var programId = glCreateProgram();

                logger.SetState("Compile", displayName);

                if (shaderCache.TryGetShaderBinary(source.ResourceKey, out var binaryFormat, out var binary))
                {
                    logger.SetState("Reading cached shader", displayName);

                    glProgramBinary(programId, binaryFormat, binary, binary.Length);

                    var linkStatus = GetProgramLinkStatus(programId);

                    if (linkStatus != null)
                    {
                        this.shaderStatusParser.LogLinkingErrors(logger, source, linkStatus);
                        glDeleteProgram(programId);

                        throw new ProjectLoadException("Cached shader linking failed");
                    }
                }
                else
                {
                    var vertexStage = CreateStageShader(source.Vertex, GL_VERTEX_SHADER, resources, logger);
                    var fragmentStage = CreateStageShader(source.Fragment, GL_FRAGMENT_SHADER, resources, logger);
                    var computeStage = CreateStageShader(source.Compute, GL_COMPUTE_SHADER, resources, logger);

                    logger.SetState("Link", displayName);

                    AttachShader(programId, vertexStage);
                    AttachShader(programId, fragmentStage);
                    AttachShader(programId, computeStage);

                    glLinkProgram(programId);
                    var linkStatus = GetProgramLinkStatus(programId);

                    DetachShader(programId, vertexStage);
                    DetachShader(programId, fragmentStage);
                    DetachShader(programId, computeStage);

                    if (linkStatus != null)
                    {
                        this.shaderStatusParser.LogLinkingErrors(logger, source, linkStatus);
                        glDeleteProgram(programId);

                        throw new ProjectLoadException("Linking failed");
                    }

                    if (source.ResourceKey != null)
                    {
                        var length = glGetProgramiv(programId, GL_PROGRAM_BINARY_LENGTH, 1)[0];
                        glGetProgramBinary(programId, length, out var length2, out binaryFormat, out var binaryArray);

                        if (length == length2)
                        {
                            shaderCache.SetShaderBinary(source.ResourceKey, binaryFormat, binaryArray);
                        }
                    }
                }

                program = new ProgramResource(this.threadAccess, programId);

                buildScope.SetResource(program);
                return program;
            }
        }

        private IRenderResource? CreateStageShader(IProgramStageSource? stageSource, int shaderType, IProjectResources resources, IProjectLoadLogger logger)
        {
            if (stageSource == null)
            {
                return null;
            }

            if (!resources.TryGetResource<IRenderResource>(stageSource.ResourceKey, out var stageShader))
            {
                stageShader = CompileProgramStage(shaderType, stageSource, logger);
                resources.AddResource(stageSource.ResourceKey, stageShader);
            }

            return stageShader;
        }

        private ShaderResource CompileProgramStage(int shaderType, IProgramStageSource stageSource, IProjectLoadLogger logger)
        {
            var shaderId = glCreateShader(shaderType);
            glShaderSource(shaderId, stageSource.Source);
            glCompileShader(shaderId);

            var compilationStatus = GetShaderCompileStatus(shaderId);

            if (compilationStatus != null)
            {
                this.shaderStatusParser.LogCompilationErrors(logger, stageSource, compilationStatus);
                glDeleteShader(shaderId);

                throw new ProjectLoadException("Compilation failed");
            }

            return new ShaderResource(this.threadAccess, shaderId);
        }

        private static void AttachShader(uint programId, IRenderResource? shader)
        {
            if (shader != null)
            {
                glAttachShader(programId, shader.Id);
            }
        }

        private static void DetachShader(uint programId, IRenderResource? shader)
        {
            if (shader != null)
            {
                glDetachShader(programId, shader.Id);
            }
        }

        private static string? GetShaderCompileStatus(uint shader)
        {
            var success = glGetShaderiv(shader, GL_COMPILE_STATUS, 1)[0];
            return success == 0 ? glGetShaderInfoLog(shader) : null;
        }

        private static string? GetProgramLinkStatus(uint programId)
        {
            var success = glGetProgramiv(programId, GL_LINK_STATUS, 1)[0];
            return success == 0 ? glGetProgramInfoLog(programId, 10240) : null;
        }
    }
}
