namespace Shaderlens.Render
{
    using static OpenGL.Gl;

    public interface IBindingParameters
    {
        void SetParameters();
    }

    public class BindingParameters : IBindingParameters
    {
        public static readonly IBindingParameters Empty = new EmptyBindingParameters();

        private class EmptyBindingParameters : IBindingParameters
        {
            public void SetParameters()
            {
            }
        }

        public readonly int minFilter;
        public readonly int magFilter;
        public readonly int wrapS;
        public readonly int wrapT;
        private readonly bool mipmapAlreadyGenerated;

        public BindingParameters(BindingParametersSource source)
        {
            var textureFilter =
                source.TextureFilterMode == TextureFilterMode.Nearest ? GL_NEAREST :
                source.TextureFilterMode == TextureFilterMode.Linear ? GL_LINEAR :
                throw new NotSupportedException($"Unexpected {nameof(TextureFilterMode)} \"{source.TextureFilterMode}\"");

            var mipmapFilter =
                source.MipmapFilterMode == MipmapFilterMode.None ? 0 :
                source.MipmapFilterMode == MipmapFilterMode.Nearest ? GL_NEAREST :
                source.MipmapFilterMode == MipmapFilterMode.Linear ? GL_LINEAR :
                throw new NotSupportedException($"Unexpected {nameof(MipmapFilterMode)} \"{source.MipmapFilterMode}\"");

            this.wrapS =
                source.WrapSMode == ClampFilterMode.ClampToEdge ? GL_CLAMP_TO_EDGE :
                source.WrapSMode == ClampFilterMode.Repeat ? GL_REPEAT :
                source.WrapSMode == ClampFilterMode.MirroredRepeat ? GL_MIRRORED_REPEAT :
                throw new NotSupportedException($"Unexpected {nameof(ClampFilterMode)} \"{source.WrapSMode}\"");

            this.wrapT =
                source.WrapTMode == ClampFilterMode.ClampToEdge ? GL_CLAMP_TO_EDGE :
                source.WrapTMode == ClampFilterMode.Repeat ? GL_REPEAT :
                source.WrapTMode == ClampFilterMode.MirroredRepeat ? GL_MIRRORED_REPEAT :
                throw new NotSupportedException($"Unexpected {nameof(ClampFilterMode)} \"{source.WrapTMode}\"");

            this.magFilter = textureFilter;
            this.minFilter =
                textureFilter == GL_NEAREST && mipmapFilter == GL_NEAREST ? GL_NEAREST_MIPMAP_NEAREST :
                textureFilter == GL_NEAREST && mipmapFilter == GL_LINEAR ? GL_NEAREST_MIPMAP_LINEAR :
                textureFilter == GL_LINEAR && mipmapFilter == GL_NEAREST ? GL_LINEAR_MIPMAP_NEAREST :
                textureFilter == GL_LINEAR && mipmapFilter == GL_LINEAR ? GL_LINEAR_MIPMAP_LINEAR :
                textureFilter;

            this.mipmapAlreadyGenerated = source.MipmapAlreadyGenerated;
        }

        public void SetParameters()
        {
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, this.magFilter);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, this.minFilter);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, this.wrapS);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, this.wrapT);

            if (!this.mipmapAlreadyGenerated && this.minFilter >= GL_NEAREST_MIPMAP_NEAREST && this.minFilter <= GL_LINEAR_MIPMAP_LINEAR)
            {
                glGenerateMipmap(GL_TEXTURE_2D);
            }
        }
    }
}
