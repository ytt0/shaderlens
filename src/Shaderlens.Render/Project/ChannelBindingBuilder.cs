namespace Shaderlens.Render.Project
{
    using static OpenGL.Gl;

    public class ChannelBindingBuilder : IChannelBindingBuilder
    {
        private class ChannelBinding : IUniformBinding
        {
            private const int MaxChannels = 8;

            private readonly IUniformBinding[] bindings;
            private readonly ITextureResource[] textures;
            private readonly IAnimatedTextureResource[] animatedTextures;
            private readonly IFramebufferResource[] framebuffers;
            private readonly float[] channelTime;
            private readonly float[] channelDuration;
            private readonly float[] channelResolution;

            private readonly int channelTimeLocation;
            private readonly int channelDurationLocation;
            private readonly int channelResolutionLocation;
            private readonly int viewerChannelResolutionLocation;

            private ITextureResource? viewerTexture;
            private IUniformBinding? viewerBinding;

            public ChannelBinding(uint programId)
            {
                this.bindings = new IUniformBinding[MaxChannels];
                this.textures = new ITextureResource[MaxChannels];
                this.animatedTextures = new IAnimatedTextureResource[MaxChannels];
                this.framebuffers = new IFramebufferResource[MaxChannels];
                this.channelTime = new float[MaxChannels];
                this.channelDuration = new float[MaxChannels];
                this.channelResolution = new float[MaxChannels * 3];

                this.channelTimeLocation = glGetUniformLocation(programId, "iChannelTime");
                this.channelDurationLocation = glGetUniformLocation(programId, "iChannelDuration");
                this.channelResolutionLocation = glGetUniformLocation(programId, "iChannelResolution");
                this.viewerChannelResolutionLocation = glGetUniformLocation(programId, "iViewerChannelResolution");
            }

            public void BindValue(IRenderContext context, IFramebufferResource framebuffer)
            {
                if (this.channelTimeLocation != -1)
                {
                    for (var i = 0; i < MaxChannels; i++)
                    {
                        if (this.animatedTextures[i] != null)
                        {
                            this.animatedTextures[i].SetFrameContent(context.FrameIndex);
                            this.channelTime[i] = (float)this.animatedTextures[i].Time / TimeSpan.TicksPerSecond;
                        }
                    }

                    glUniform1fv(this.channelTimeLocation, this.channelTime.Length, this.channelTime);
                }

                if (this.channelDurationLocation != -1)
                {
                    glUniform1fv(this.channelDurationLocation, this.channelDuration.Length, this.channelDuration);
                }

                if (this.channelResolutionLocation != -1)
                {
                    var j = 0;
                    for (var i = 0; i < MaxChannels; i++)
                    {
                        if (this.textures[i] != null)
                        {
                            this.channelResolution[j] = this.textures[i].Width;
                            this.channelResolution[j + 1] = this.textures[i].Height;
                        }
                        else if (this.framebuffers[i] != null)
                        {
                            this.channelResolution[j] = this.framebuffers[i].Width;
                            this.channelResolution[j + 1] = this.framebuffers[i].Height;
                        }

                        this.channelResolution[j + 2] = 1;
                        j += 3;
                    }

                    glUniform3fv(this.channelResolutionLocation, this.channelResolution.Length, this.channelResolution);
                }

                for (var i = 0; i < MaxChannels; i++)
                {
                    this.bindings[i]?.BindValue(context, framebuffer);
                }

                this.viewerBinding?.BindValue(context, framebuffer);

                if (this.viewerChannelResolutionLocation != -1 && this.viewerTexture != null)
                {
                    glUniform3f(this.viewerChannelResolutionLocation, this.viewerTexture.Width, this.viewerTexture.Height, 1);
                }
            }

            public void AddBinding(int index, IUniformBinding binding)
            {
                if (this.bindings[index] != null)
                {
                    throw new Exception($"Channel {index} binding has been set more than once");
                }

                this.bindings[index] = binding;
            }

            public void AddViewerBinding(IUniformBinding binding)
            {
                if (this.viewerBinding != null)
                {
                    throw new Exception($"Viewer channel binding has been set more than once");
                }

                this.viewerBinding = binding;
            }

            public void AddTexture(int index, ITextureResource texture)
            {
                if (this.textures[index] != null)
                {
                    throw new Exception($"Channel {index} binding has been set more than once");
                }

                this.textures[index] = texture;
            }

            public void AddViewerTexture(ITextureResource texture)
            {
                if (this.viewerTexture != null)
                {
                    throw new Exception($"Viewer channel binding has been set more than once");
                }

                this.viewerTexture = texture;
            }

            public void AddAnimatedTexture(int index, IAnimatedTextureResource animatedTexture)
            {
                if (this.textures[index] != null)
                {
                    throw new Exception($"Channel {index} binding has been set more than once");
                }

                this.textures[index] = animatedTexture;
                this.animatedTextures[index] = animatedTexture;
                this.channelDuration[index] = (float)animatedTexture.Duration / TimeSpan.TicksPerSecond;
            }

            public void AddFramebuffer(int index, IFramebufferResource framebuffer)
            {
                if (this.framebuffers[index] != null)
                {
                    throw new Exception($"Channel {index} binding has been set more than once");
                }

                this.framebuffers[index] = framebuffer;
            }
        }

        public IUniformBinding Binding { get; }

        private readonly IThreadAccess threadAccess;
        private readonly uint programId;
        private readonly IPassCollection<IFramebufferResource> framebuffers;
        private readonly IProjectResources resources;
        private readonly ITextureReaderFactory textureReaderFactory;
        private readonly IKeyboardTextureResource keyboardTextureResource;
        private readonly ChannelBinding channelBinding;
        private int textureUnit;

        public ChannelBindingBuilder(IThreadAccess threadAccess, uint programId, IPassCollection<IFramebufferResource> framebuffers, IProjectResources resources, ITextureReaderFactory textureReaderFactory, IKeyboardTextureResource keyboardTextureResource)
        {
            this.threadAccess = threadAccess;
            this.programId = programId;
            this.framebuffers = framebuffers;
            this.resources = resources;
            this.textureReaderFactory = textureReaderFactory;
            this.keyboardTextureResource = keyboardTextureResource;
            this.channelBinding = new ChannelBinding(programId);
            this.textureUnit = GL_TEXTURE0;

            this.Binding = this.channelBinding;
        }

        public void SetImageFramebufferBinding(int channelIndex, BindingParametersSource bindingParameters)
        {
            AddFramebufferBinding(channelIndex, this.framebuffers.Image!, new BindingParameters(bindingParameters));
        }

        public void SetImageDefaultFramebufferBinding()
        {
            if (this.framebuffers.Buffers.Any())
            {
                AddFramebufferBinding(0, this.framebuffers.Buffers.Last(), BindingParameters.Empty);
            }
        }

        public void SetPassFramebufferBinding(int channelIndex, string key, BindingParametersSource bindingParameters)
        {
            var framebuffer = this.framebuffers.TryGetBuffer(key) ?? throw new Exception($"Channel {channelIndex} binding cannot be resolved, pass {key} is missing");

            AddFramebufferBinding(channelIndex, framebuffer, new BindingParameters(bindingParameters));
        }

        public void SetPassDefaultFramebufferBinding(string key)
        {
            var previousKey = this.framebuffers.BuffersKeys.ElementAtOrDefault(this.framebuffers.BuffersKeys.IndexOf(key) - 1);
            if (previousKey != null)
            {
                AddFramebufferBinding(0, this.framebuffers[previousKey], BindingParameters.Empty);
            }
        }

        public void SetViewerFramebufferBinding(int channelIndex)
        {
            var texture = new ViewerBufferTexture(this.framebuffers.ToArray());

            this.channelBinding.AddTexture(channelIndex, texture);

            if (ViewerBufferUniformBinding.TryCreate(this.threadAccess, texture, BindingParameters.Empty, this.programId, GetChannelUniformName(channelIndex), this.textureUnit, out var binding))
            {
                this.channelBinding.AddBinding(channelIndex, binding);
                this.textureUnit++;
            }
        }

        public void SetViewerDefaultFramebufferBinding()
        {
            var texture = new ViewerBufferTexture(this.framebuffers.ToArray());

            this.channelBinding.AddViewerTexture(texture);

            if (ViewerBufferUniformBinding.TryCreate(this.threadAccess, texture, BindingParameters.Empty, this.programId, "iViewerChannel", this.textureUnit, out var binding))
            {
                this.channelBinding.AddViewerBinding(binding);
                this.textureUnit++;
            }
        }

        public void SetTextureBinding(int channelIndex, IFileResource<byte[]> resource, BindingParametersSource bindingParameters)
        {
            var texture = GetTextureResource(resource);

            if (texture is IAnimatedTextureResource animatedTexture)
            {
                AddAnimatedTextureBinding(channelIndex, animatedTexture, new BindingParameters(bindingParameters));
            }
            else
            {
                AddTextureBinding(channelIndex, texture, new BindingParameters(bindingParameters));
            }
        }

        public void SetTextureSequenceBinding(int channelIndex, IEnumerable<IFileResource<byte[]>> resources, int frameRate, BindingParametersSource bindingParameters)
        {
            AddAnimatedTextureBinding(channelIndex, GetTextureSequenceResource(resources, frameRate), new BindingParameters(bindingParameters));
        }

        public void SetKeyboardBinding(int channelIndex)
        {
            AddTextureBinding(channelIndex, this.keyboardTextureResource, BindingParameters.Empty);
        }

        private ITextureResource GetTextureResource(IFileResource<byte[]> resource)
        {
            var textureKey = new TypedResourceKey(typeof(ITextureResource), resource.Key);
            if (!this.resources.TryGetResource<ITextureResource>(textureKey, out var textureResource))
            {
                try
                {
                    var textureReader = this.textureReaderFactory.Create(Path.GetExtension(resource.Key.AbsolutePath));
                    using (var stream = new MemoryStream(resource.Value))
                    {
                        textureResource = textureReader.Read(stream);
                        this.resources.AddResource(textureKey, textureResource);
                    }
                }
                catch (Exception e)
                {
                    throw new StorageException($"{resource.Key}: {e.Message}", resource.Key, e);
                }
            }

            return textureResource;
        }

        private AnimatedTextureResource GetTextureSequenceResource(IEnumerable<IFileResource<byte[]>> resources, int frameRate)
        {
            var frames = new List<IAnimationFrameTextureResource>();

            var frameIndex = 1;
            foreach (var resource in resources)
            {
                var textureResource = GetTextureResource(resource);
                var frameEndTime = (int)(frameIndex * (1000.0 / frameRate));

                frames.Add(new AnimationFrameTextureResource(textureResource, frameEndTime));
                frameIndex++;
            }

            return new AnimatedTextureResource(frames);
        }

        private void AddTextureBinding(int channelIndex, ITextureResource texture, IBindingParameters bindingParameters)
        {
            this.channelBinding.AddTexture(channelIndex, texture);

            if (TextureUniformBinding.TryCreate(this.threadAccess, texture, bindingParameters, this.programId, GetChannelUniformName(channelIndex), this.textureUnit, out var binding))
            {
                this.channelBinding.AddBinding(channelIndex, binding);
                this.textureUnit++;
            }
        }

        private void AddFramebufferBinding(int channelIndex, IFramebufferResource framebuffer, IBindingParameters bindingParameters)
        {
            this.channelBinding.AddFramebuffer(channelIndex, framebuffer);

            if (FramebufferUniformBinding.TryCreate(this.threadAccess, framebuffer, 0, bindingParameters, this.programId, GetChannelUniformName(channelIndex), this.textureUnit, out var binding))
            {
                this.channelBinding.AddBinding(channelIndex, binding);
                this.textureUnit++;
            }
        }

        private void AddAnimatedTextureBinding(int channelIndex, IAnimatedTextureResource texture, IBindingParameters bindingParameters)
        {
            this.channelBinding.AddAnimatedTexture(channelIndex, texture);

            if (AnimatedTextureUniformBinding.TryCreate(this.threadAccess, texture, bindingParameters, this.programId, GetChannelUniformName(channelIndex), this.textureUnit, out var binding))
            {
                this.channelBinding.AddBinding(channelIndex, binding);
                this.textureUnit++;
            }
        }

        private static string GetChannelUniformName(int channelIndex)
        {
            return $"iChannel{channelIndex}";
        }
    }
}
