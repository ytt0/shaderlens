namespace Shaderlens.Serialization.Project
{
    public readonly struct BindingParametersSource
    {
        public TextureFilterMode TextureFilterMode { get; }
        public MipmapFilterMode MipmapFilterMode { get; }
        public ClampFilterMode WrapSMode { get; }
        public ClampFilterMode WrapTMode { get; }
        public bool MipmapAlreadyGenerated { get; }

        public BindingParametersSource(TextureFilterMode textureFilterMode, MipmapFilterMode mipmapFilterMode, ClampFilterMode wrapSMode, ClampFilterMode wrapTMode, bool mipmapAlreadyGenerated)
        {
            this.TextureFilterMode = textureFilterMode;
            this.MipmapFilterMode = mipmapFilterMode;
            this.WrapSMode = wrapSMode;
            this.WrapTMode = wrapTMode;
            this.MipmapAlreadyGenerated = mipmapAlreadyGenerated;
        }
    }
}
