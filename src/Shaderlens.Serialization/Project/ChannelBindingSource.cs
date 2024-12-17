namespace Shaderlens.Serialization.Project
{
    public interface IChannelBindingSource
    {
        bool IsEmpty { get; }
        void AddBinding(IChannelBindingBuilder builder);
    }

    public static class ChannelBindingSource
    {
        private class EmptyChannelBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return true; } }

            public void AddBinding(IChannelBindingBuilder builder)
            {
            }
        }

        public static readonly IChannelBindingSource Empty = new EmptyChannelBindingSource();
    }

    public class ChannelBindingSourceCollection : IChannelBindingSource
    {
        public bool IsEmpty { get { return this.bindings.All(binding => binding.IsEmpty); } }

        private readonly IEnumerable<IChannelBindingSource> bindings;

        public ChannelBindingSourceCollection(IEnumerable<IChannelBindingSource> bindings)
        {
            this.bindings = bindings;
        }

        public void AddBinding(IChannelBindingBuilder builder)
        {
            foreach (var binding in this.bindings)
            {
                binding.AddBinding(builder);
            }
        }
    }

    public class ImageDefaultFramebufferBinding : IChannelBindingSource
    {
        public static readonly IChannelBindingSource Instance = new ImageDefaultFramebufferBinding();

        public bool IsEmpty { get { return false; } }

        private ImageDefaultFramebufferBinding()
        {
        }

        public void AddBinding(IChannelBindingBuilder builder)
        {
            builder.SetImageDefaultFramebufferBinding();
        }
    }

    public class PassDefaultFramebufferBinding : IChannelBindingSource
    {
        private readonly string key;

        public bool IsEmpty { get { return false; } }

        public PassDefaultFramebufferBinding(string key)
        {
            this.key = key;
        }

        public void AddBinding(IChannelBindingBuilder builder)
        {
            builder.SetPassDefaultFramebufferBinding(this.key);
        }
    }

    public enum TextureFilterMode { Nearest, Linear }
    public enum MipmapFilterMode { None, Nearest, Linear }
    public enum ClampFilterMode { ClampToEdge, Repeat, MirroredRepeat }

    public partial class ChannelBindingSourceJsonSerializer : IJsonSerializer<IChannelBindingSource>
    {
        private class KeyboardBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return false; } }

            private readonly int channelIndex;

            public KeyboardBindingSource(int channelIndex)
            {
                this.channelIndex = channelIndex;
            }

            public void AddBinding(IChannelBindingBuilder builder)
            {
                builder.SetKeyboardBinding(this.channelIndex);
            }
        }

        private class ImageFramebufferBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return false; } }

            private readonly int channelIndex;
            private readonly int bufferTextureIndex;
            private readonly BindingParametersSource bindingParameters;

            public ImageFramebufferBindingSource(int channelIndex, int bufferTextureIndex, BindingParametersSource bindingParameters)
            {
                this.channelIndex = channelIndex;
                this.bufferTextureIndex = bufferTextureIndex;
                this.bindingParameters = bindingParameters;
            }

            public void AddBinding(IChannelBindingBuilder builder)
            {
                builder.SetImageFramebufferBinding(this.channelIndex, this.bufferTextureIndex, this.bindingParameters);
            }
        }

        private class PassFramebufferBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return false; } }

            private readonly int channelIndex;
            private readonly string key;
            private readonly int bufferTextureIndex;
            private readonly BindingParametersSource bindingParameters;

            public PassFramebufferBindingSource(int channelIndex, string key, int bufferTextureIndex, BindingParametersSource bindingParameters)
            {
                this.channelIndex = channelIndex;
                this.key = key;
                this.bufferTextureIndex = bufferTextureIndex;
                this.bindingParameters = bindingParameters;
            }

            public void AddBinding(IChannelBindingBuilder builder)
            {
                builder.SetPassFramebufferBinding(this.channelIndex, this.key, this.bufferTextureIndex, this.bindingParameters);
            }
        }

        private class ViewerBufferBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return false; } }

            private readonly int channelIndex;

            public ViewerBufferBindingSource(int channelIndex)
            {
                this.channelIndex = channelIndex;
            }

            public void AddBinding(IChannelBindingBuilder builder)
            {
                builder.SetViewerBufferBinding(this.channelIndex);
            }
        }

        private class TextureBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return false; } }

            private readonly int channelIndex;
            private readonly IFileResource<byte[]> resource;
            private readonly BindingParametersSource bindingParameters;

            public TextureBindingSource(int channelIndex, IFileResource<byte[]> resource, BindingParametersSource bindingParameters)
            {
                this.channelIndex = channelIndex;
                this.resource = resource;
                this.bindingParameters = bindingParameters;
            }

            public void AddBinding(IChannelBindingBuilder builder)
            {
                builder.SetTextureBinding(this.channelIndex, this.resource, this.bindingParameters);
            }
        }

        private class TextureSequenceBindingSource : IChannelBindingSource
        {
            public bool IsEmpty { get { return false; } }

            private readonly int channelIndex;
            private readonly IEnumerable<IFileResource<byte[]>> resources;
            private readonly int frameRate;
            private readonly BindingParametersSource bindingParameters;

            public TextureSequenceBindingSource(int channelIndex, IEnumerable<IFileResource<byte[]>> resources, int frameRate, BindingParametersSource bindingParameters)
            {
                this.channelIndex = channelIndex;
                this.resources = resources;
                this.frameRate = frameRate;
                this.bindingParameters = bindingParameters;
            }

            public void AddBinding(IChannelBindingBuilder builder)
            {
                builder.SetTextureSequenceBinding(this.channelIndex, this.resources, this.frameRate, this.bindingParameters);
            }
        }


        private readonly int channelIndex;
        private readonly IChannelBindingValidator validator;
        private readonly IFileSystem fileSystem;

        public ChannelBindingSourceJsonSerializer(int channelIndex, IChannelBindingValidator validator, IFileSystem fileSystem)
        {
            this.channelIndex = channelIndex;
            this.validator = validator;
            this.fileSystem = fileSystem;
        }

        public IChannelBindingSource Deserialize(JsonNode? sourceNode)
        {
            if (sourceNode == null)
            {
                return ChannelBindingSource.Empty;
            }

            if (sourceNode.TryGetStringValue(out var value))
            {
                if (value == "Keyboard")
                {
                    return new KeyboardBindingSource(this.channelIndex);
                }

                if (value == "Viewer")
                {
                    return new ViewerBufferBindingSource(this.channelIndex);
                }

                var match = PassKeyRegex().Match(value);
                if (match.Success)
                {
                    var key = match.Groups["key"].Value;
                    var index = match.Groups["index"].Success ? Int32.Parse(match.Groups["index"].Value) : 0;

                    this.validator.Validate(key, index, sourceNode);

                    if (key == "Image")
                    {
                        return new ImageFramebufferBindingSource(this.channelIndex, index, default);
                    }

                    return new PassFramebufferBindingSource(this.channelIndex, key, index, default);
                }

                return new TextureBindingSource(this.channelIndex, this.fileSystem.ReadBytes(value), default);
            }

            if (sourceNode.GetValueKind() == JsonValueKind.Object)
            {
                var jsonObject = sourceNode.AsObject();

                var type = ReadStringProperty(jsonObject, "Type");
                var source = ReadStringProperty(jsonObject, "Source") ?? throw new JsonSourceException("Binding \"Source\" value is missing", sourceNode);
                var textureFilterString = ReadStringProperty(jsonObject, "TextureFilter");
                var mipmapFilterString = ReadStringProperty(jsonObject, "MipmapFilter");
                var wrapString = ReadStringProperty(jsonObject, "Wrap");
                var wrapSString = ReadStringProperty(jsonObject, "WrapS") ?? wrapString;
                var wrapTString = ReadStringProperty(jsonObject, "WrapT") ?? wrapString;


                var textureFilterMode =
                    textureFilterString == null || textureFilterString == "Nearest" ? TextureFilterMode.Nearest :
                    textureFilterString == "Linear" ? TextureFilterMode.Linear :
                    throw new JsonSourceException($"Binding texture filter value \"{textureFilterString}\" is invalid, expected values are: Nearest (default), Linear", sourceNode["TextureFilter"]!);

                var mipmapFilterMode =
                    mipmapFilterString == null || mipmapFilterString == "None" ? MipmapFilterMode.None :
                    mipmapFilterString == "Nearest" ? MipmapFilterMode.Nearest :
                    mipmapFilterString == "Linear" ? MipmapFilterMode.Linear :
                    throw new JsonSourceException($"Binding mipmap filter value \"{mipmapFilterString}\" is invalid, expected values are: None (default), Nearest, Linear", sourceNode["MipmapFilter"]!);

                var wrapSMode =
                    wrapSString == "ClampToEdge" || wrapSString == null ? ClampFilterMode.ClampToEdge :
                    wrapSString == "Repeat" ? ClampFilterMode.Repeat :
                    wrapSString == "MirroredRepeat" ? ClampFilterMode.MirroredRepeat :
                    throw new JsonSourceException($"Binding clamp filter value \"{wrapSString}\" is invalid, expected values are: ClampToEdge (default), Repeat, MirroredRepeat", sourceNode["WrapS"] ?? sourceNode["Wrap"]!);

                var wrapTMode =
                    wrapTString == "ClampToEdge" || wrapTString == null ? ClampFilterMode.ClampToEdge :
                    wrapTString == "Repeat" ? ClampFilterMode.Repeat :
                    wrapTString == "MirroredRepeat" ? ClampFilterMode.MirroredRepeat :
                    throw new JsonSourceException($"Binding clamp filter value \"{wrapTString}\" is invalid, expected values are: ClampToEdge (default), Repeat, MirroredRepeat", sourceNode["WrapT"] ?? sourceNode["Wrap"]!);

                var mipmapAlreadyGenerated = type == "Texture" || type == "TextureSequence";

                var bindingParameters = new BindingParametersSource(textureFilterMode, mipmapFilterMode, wrapSMode, wrapTMode, mipmapAlreadyGenerated);

                if (type == "Framebuffer" || type == null)
                {
                    if (source == "Viewer")
                    {
                        return new ViewerBufferBindingSource(this.channelIndex);
                    }

                    var match = PassKeyRegex().Match(source);
                    if (match.Success)
                    {
                        var key = match.Groups["key"].Value;
                        var index = match.Groups["index"].Success ? Int32.Parse(match.Groups["index"].Value) : 0;

                        this.validator.Validate(key, index, sourceNode["Source"]!);

                        if (key == "Image")
                        {
                            return new ImageFramebufferBindingSource(this.channelIndex, index, bindingParameters);
                        }

                        return new PassFramebufferBindingSource(this.channelIndex, key, index, bindingParameters);
                    }

                    throw new JsonSourceException($"Framebuffer binding source value \"{source}\" is invalid, expected values are: Image, Buffer#, set Type to \"Texture\" to bind to a texture file", sourceNode["Source"]!);
                }

                if (type == "Texture")
                {
                    if (source == "Keyboard")
                    {
                        return new KeyboardBindingSource(this.channelIndex);
                    }

                    return new TextureBindingSource(this.channelIndex, this.fileSystem.ReadBytes(source), bindingParameters);
                }

                if (type == "TextureSequence")
                {
                    if (!(sourceNode["FrameRate"]?.TryGetIntValue(out var frameRate) ?? throw new JsonSourceException("TextureSequence binding FrameRate value is missing", sourceNode)))
                    {
                        throw new JsonSourceException($"TextureSequence binding FrameRate value is invalid{Environment.NewLine}{sourceNode["FrameRate"]!.ToJsonString()}", sourceNode["FrameRate"]!);
                    }

                    var resources = this.fileSystem.GetSequencePaths(source).Select(path => this.fileSystem.ReadBytes(path)).ToArray();

                    return new TextureSequenceBindingSource(this.channelIndex, resources, frameRate, bindingParameters);
                }

                throw new JsonSourceException($"Invalid binding definition, \"{type}\" is invalid, expected types are: Framebuffer (default), Texture, TextureSequence", jsonObject["Type"]!);
            }

            throw new JsonSourceException($"Invalid binding definition, string or object value is expected{Environment.NewLine}{sourceNode.ToJsonString()}", sourceNode);
        }

        public JsonNode? Serialize(IChannelBindingSource value)
        {
            throw new NotImplementedException();
        }

        private static string? ReadStringProperty(JsonObject source, string name)
        {
            var sourceNode = source[name];
            var kind = sourceNode?.GetValueKind();

            return sourceNode == null ? null : kind == JsonValueKind.String ? sourceNode.GetValue<string>() :
                kind == JsonValueKind.Object || kind == JsonValueKind.Array ?
                    throw new JsonSourceException($"Binding \"{name}\" {kind} value is invalid, a string value is expected:{Environment.NewLine}{sourceNode!.ToJsonString()}", sourceNode) :
                    throw new JsonSourceException($"Binding \"{name}\" {kind} value \"{sourceNode!.ToJsonString()}\" is invalid, a string value is expected", sourceNode);
        }

        [GeneratedRegex("^(?<key>Image|(Buffer([0-9]+|[A-Z])))(\\[(?<index>[0-9]+)\\])?$")]
        private static partial Regex PassKeyRegex();
    }

    public partial class ChannelBindingSourceCollectionJsonSerializer : IJsonSerializer<IChannelBindingSource>
    {
        private readonly IChannelBindingValidator validator;
        private readonly IFileSystem fileSystem;

        public ChannelBindingSourceCollectionJsonSerializer(IChannelBindingValidator validator, IFileSystem fileSystem)
        {
            this.validator = validator;
            this.fileSystem = fileSystem;
        }

        public IChannelBindingSource Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return ChannelBindingSource.Empty;
            }

            if (source.GetValueKind() != JsonValueKind.Object)
            {
                throw new JsonSourceException($"Failed to deserialize element of type {source.GetValueKind()}, a json object is expected", source);
            }

            var bindings = new List<IChannelBindingSource>();
            foreach (var property in source.AsObject())
            {
                var match = ChannelKeyRegex().Match(property.Key);
                if (match.Success && property.Value != null)
                {
                    var channelIndex = Int32.Parse(match.Groups["key"].Value);
                    if (channelIndex > 7)
                    {
                        throw new JsonSourceException($"Framebuffer binding target \"{property.Key}\" is invalid, channel index should be between 0 and 7", property.Value);
                    }

                    var serializer = JsonSerializerContext.Create(new ChannelBindingSourceJsonSerializer(channelIndex, this.validator, this.fileSystem));
                    bindings.Add(serializer.Deserialize(property.Value));
                }
            }

            return new ChannelBindingSourceCollection(bindings);
        }

        public JsonNode? Serialize(IChannelBindingSource value)
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex("^Channel(?<key>[0-9]+)$")]
        private static partial Regex ChannelKeyRegex();
    }
}
