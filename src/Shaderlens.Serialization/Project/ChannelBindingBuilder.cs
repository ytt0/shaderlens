namespace Shaderlens.Serialization.Project
{
    public interface IChannelBindingBuilder
    {
        void SetImageFramebufferBinding(int channelIndex, BindingParametersSource bindingParameters);
        void SetImageDefaultFramebufferBinding();
        void SetPassFramebufferBinding(int channelIndex, string key, BindingParametersSource bindingParameters);
        void SetPassDefaultFramebufferBinding(string key);
        void SetViewerFramebufferBinding(int channelIndex);
        void SetTextureBinding(int channelIndex, IFileResource<byte[]> resource, BindingParametersSource bindingParameters);
        void SetTextureSequenceBinding(int channelIndex, IEnumerable<IFileResource<byte[]>> resources, int frameRate, BindingParametersSource bindingParameters);
        void SetKeyboardBinding(int channelIndex);
    }
}
