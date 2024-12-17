namespace Shaderlens.Serialization.Project
{
    public interface IChannelBindingBuilder
    {
        void SetImageDefaultFramebufferBinding();
        void SetImageFramebufferBinding(int channelIndex, int bufferTextureIndex, BindingParametersSource bindingParameters);
        void SetPassFramebufferBinding(int channelIndex, string key, int bufferTextureIndex, BindingParametersSource bindingParameters);
        void SetPassDefaultFramebufferBinding(string key);
        void SetViewerBufferBinding(int channelIndex);
        void SetViewerDefaultFramebufferBinding();
        void SetTextureBinding(int channelIndex, IFileResource<byte[]> resource, BindingParametersSource bindingParameters);
        void SetTextureSequenceBinding(int channelIndex, IEnumerable<IFileResource<byte[]>> resources, int frameRate, BindingParametersSource bindingParameters);
        void SetKeyboardBinding(int channelIndex);
    }
}
