namespace Shaderlens.Render.Uniforms
{
    using static OpenGL.Gl;

    public interface IUniformBinding
    {
        void BindValue(IRenderContext context, IFramebufferResource framebuffer);
    }

    public static class UniformBinding
    {
        private class EmptyUniformBinding : IUniformBinding
        {
            public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
            {
            }
        }

        public static readonly IUniformBinding Empty = new EmptyUniformBinding();
    }

    public class RenderContextUniformBinding : IUniformBinding
    {
        // uniform vec3      iResolution
        // uniform float     iTime
        // uniform float     iChannelTime[4]
        // uniform vec4      iMouse
        // uniform vec4      iDate
        // uniform float     iSampleRate
        // uniform vec3      iChannelResolution[4]
        // uniform int       iFrame
        // uniform float     iTimeDelta
        // uniform float     iFrameRate

        private readonly IThreadAccess threadAccess;

        private readonly int resolutionLocation;
        private readonly int timeLocation;
        private readonly int mouseLocation;
        private readonly int dateLocation;
        private readonly int frameLocation;
        private readonly int frameRateLocation;
        private readonly int timeDeltaLocation;

        private readonly int viewerScaleLocation;
        private readonly int viewerOffsetLocation;

        public RenderContextUniformBinding(IThreadAccess threadAccess, uint programId)
        {
            threadAccess.Verify();
            this.threadAccess = threadAccess;

            this.resolutionLocation = glGetUniformLocation(programId, "iResolution");
            this.timeLocation = glGetUniformLocation(programId, "iTime");
            this.mouseLocation = glGetUniformLocation(programId, "iMouse");
            this.dateLocation = glGetUniformLocation(programId, "iDate");
            this.frameLocation = glGetUniformLocation(programId, "iFrame");
            this.frameRateLocation = glGetUniformLocation(programId, "iFrameRate");
            this.timeDeltaLocation = glGetUniformLocation(programId, "iTimeDelta");

            this.viewerScaleLocation = glGetUniformLocation(programId, "iViewerScale");
            this.viewerOffsetLocation = glGetUniformLocation(programId, "iViewerOffset");
        }

        public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.threadAccess.Verify();

            glUniform3f(this.resolutionLocation, framebuffer.Width, framebuffer.Height, 1.0f);
            glUniform1f(this.timeLocation, (float)context.FrameIndex.Time / TimeSpan.TicksPerSecond);
            glUniform4f(this.mouseLocation, context.MouseX, context.MouseY,
                context.IsMouseDown ? Math.Max(0.0001f, context.MousePressedX) : -context.MousePressedX,
                context.IsMousePressed ? Math.Max(0.0001f, context.MousePressedY) : -context.MousePressedY);
            glUniform4f(this.dateLocation, context.FrameIndex.DateTime.Year, context.FrameIndex.DateTime.Month, context.FrameIndex.DateTime.Day, (float)(context.FrameIndex.DateTime - context.FrameIndex.DateTime.Date).TotalSeconds);
            glUniform1i(this.frameLocation, context.FrameIndex.Index);
            glUniform1f(this.frameRateLocation, context.FrameIndex.Delta < TimeSpan.TicksPerMillisecond ? 0 : (float)TimeSpan.TicksPerSecond / context.FrameIndex.Delta);
            glUniform1f(this.timeDeltaLocation, (float)context.FrameIndex.Delta / TimeSpan.TicksPerSecond);

            glUniform1f(this.viewerScaleLocation, context.ViewerScale);
            glUniform2f(this.viewerOffsetLocation, context.ViewerOffsetX, context.ViewerOffsetY);
        }
    }

    public class TextureUniformBinding : IUniformBinding
    {
        private readonly IThreadAccess threadAccess;
        private readonly ITextureResource texture;
        private readonly IBindingParameters bindingParameters;
        private readonly int textureUnit;

        private TextureUniformBinding(IThreadAccess threadAccess, ITextureResource texture, IBindingParameters bindingParameters, int textureUnit)
        {
            this.threadAccess = threadAccess;
            this.texture = texture;
            this.textureUnit = textureUnit;
            this.bindingParameters = bindingParameters;
        }

        public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.threadAccess.Verify();
            glActiveTexture(this.textureUnit);
            glBindTexture(GL_TEXTURE_2D, this.texture.Id);

            this.bindingParameters.SetParameters();
        }

        public static bool TryCreate(IThreadAccess threadAccess, ITextureResource texture, IBindingParameters bindingParameters, uint programId, string name, int textureUnit, [MaybeNullWhen(false)] out IUniformBinding uniformBinding)
        {
            threadAccess.Verify();

            if (textureUnit < GL_TEXTURE0 || textureUnit > GL_TEXTURE31)
            {
                throw new ArgumentOutOfRangeException(nameof(textureUnit), "Texture unit should be between GL_TEXTURE0 and GL_TEXTURE31");
            }

            var location = glGetUniformLocation(programId, name);

            if (location == -1)
            {
                uniformBinding = null;
                return false;
            }

            glUseProgram(programId);
            glUniform1i(location, textureUnit - GL_TEXTURE0);
            glUseProgram(0);

            uniformBinding = new TextureUniformBinding(threadAccess, texture, bindingParameters, textureUnit);
            return true;
        }
    }

    public class FramebufferUniformBinding : IUniformBinding
    {
        private readonly IThreadAccess threadAccess;
        private readonly IFramebufferResource framebuffer;
        private readonly int framebufferTextureIndex;
        private readonly IBindingParameters bindingParameters;
        private readonly int textureUnit;

        private FramebufferUniformBinding(IThreadAccess threadAccess, IFramebufferResource framebuffer, int framebufferTextureIndex, IBindingParameters bindingParameters, int textureUnit)
        {
            this.threadAccess = threadAccess;
            this.framebuffer = framebuffer;
            this.framebufferTextureIndex = framebufferTextureIndex;
            this.bindingParameters = bindingParameters;
            this.textureUnit = textureUnit;
        }

        public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.threadAccess.Verify();
            glActiveTexture(this.textureUnit);
            glBindTexture(GL_TEXTURE_2D, this.framebuffer.GetTextureId(this.framebufferTextureIndex));

            this.bindingParameters.SetParameters();
        }

        public static bool TryCreate(IThreadAccess threadAccess, IFramebufferResource framebuffer, int framebufferTextureIndex, IBindingParameters bindingParameters, uint programId, string name, int textureUnit, [MaybeNullWhen(false)] out IUniformBinding uniformBinding)
        {
            threadAccess.Verify();

            if (textureUnit < GL_TEXTURE0 || textureUnit > GL_TEXTURE31)
            {
                throw new ArgumentOutOfRangeException(nameof(textureUnit), "Texture unit should be between GL_TEXTURE0 and GL_TEXTURE31");
            }

            var location = glGetUniformLocation(programId, name);

            if (location == -1)
            {
                uniformBinding = null;
                return false;
            }

            glUseProgram(programId);
            glUniform1i(location, textureUnit - GL_TEXTURE0);
            glUseProgram(0);

            uniformBinding = new FramebufferUniformBinding(threadAccess, framebuffer, framebufferTextureIndex, bindingParameters, textureUnit);
            return true;
        }
    }

    public class AnimatedTextureUniformBinding : IUniformBinding
    {
        private readonly IThreadAccess threadAccess;
        private readonly IAnimatedTextureResource texture;
        private readonly IBindingParameters bindingParameters;
        private readonly int textureUnit;

        private AnimatedTextureUniformBinding(IThreadAccess threadAccess, IAnimatedTextureResource texture, IBindingParameters bindingParameters, int textureUnit)
        {
            this.threadAccess = threadAccess;
            this.texture = texture;
            this.bindingParameters = bindingParameters;
            this.textureUnit = textureUnit;
        }

        public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.threadAccess.Verify();
            this.texture.SetFrameContent(context.FrameIndex);
            glActiveTexture(this.textureUnit);
            glBindTexture(GL_TEXTURE_2D, this.texture.Id);

            this.bindingParameters.SetParameters();
        }

        public static bool TryCreate(IThreadAccess threadAccess, IAnimatedTextureResource texture, IBindingParameters bindingParameters, uint programId, string name, int textureUnit, [MaybeNullWhen(false)] out IUniformBinding uniformBinding)
        {
            threadAccess.Verify();

            if (textureUnit < GL_TEXTURE0 || textureUnit > GL_TEXTURE31)
            {
                throw new ArgumentOutOfRangeException(nameof(textureUnit), "Texture unit should be between GL_TEXTURE0 and GL_TEXTURE31");
            }

            var location = glGetUniformLocation(programId, name);
            if (location == -1)
            {
                uniformBinding = null;
                return false;
            }

            glUseProgram(programId);
            glUniform1i(location, textureUnit - GL_TEXTURE0);
            glUseProgram(0);

            uniformBinding = new AnimatedTextureUniformBinding(threadAccess, texture, bindingParameters, textureUnit);
            return true;
        }
    }

    public class ViewerBufferUniformBinding : IUniformBinding
    {
        private readonly IViewerBufferTexture texture;
        private readonly IUniformBinding uniformBinding;

        private ViewerBufferUniformBinding(IViewerBufferTexture texture, IUniformBinding uniformBinding)
        {
            this.texture = texture;
            this.uniformBinding = uniformBinding;
        }

        public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
        {
            this.texture.SetIndex(context.ViewerBufferIndex, 0);
            this.uniformBinding.BindValue(context, framebuffer);
        }

        public static bool TryCreate(IThreadAccess threadAccess, IViewerBufferTexture texture, IBindingParameters bindingParameters, uint programId, string name, int textureUnit, [MaybeNullWhen(false)] out IUniformBinding uniformBinding)
        {
            if (TextureUniformBinding.TryCreate(threadAccess, texture, bindingParameters, programId, name, textureUnit, out var textureUniformBinding))
            {
                uniformBinding = new ViewerBufferUniformBinding(texture, textureUniformBinding);
                return true;
            }

            uniformBinding = null;
            return false;
        }
    }

    public class UniformBindingCollection : IUniformBinding
    {
        private readonly IUniformBinding[] bindings;

        public UniformBindingCollection(params IUniformBinding[] bindings)
        {
            this.bindings = bindings.ToArray();
        }

        public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
        {
            for (var i = 0; i < this.bindings.Length; i++)
            {
                this.bindings[i].BindValue(context, framebuffer);
            }
        }
    }
}
