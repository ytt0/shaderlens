namespace Shaderlens.Views.Menus
{
    public class BuffersMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationInputs inputs;
        private readonly string? imageName;
        private readonly string[]? buffersNames;

        public BuffersMenuSource(IApplication application, IApplicationInputs inputs, IProjectSource? project)
        {
            this.application = application;
            this.inputs = inputs;
            this.imageName = project?.Passes.Image?.Program.DisplayName;
            this.buffersNames = project?.Passes.Buffers.Select(buffer => buffer.Program.DisplayName).ToArray();
        }

        public void AddTo(IMenuBuilder builder)
        {
            if (this.imageName == null)
            {
                builder.AddEmptyItem();
                return;
            }

            builder.AddItem(this.imageName, this.inputs.BufferImage, null, null, () => this.application.ViewerBufferIndex = this.application.ViewerBuffersCount - 1, state => state.IsChecked = this.application.ViewerBufferIndex == this.application.ViewerBuffersCount - 1);
            builder.AddSeparator();

            for (var i = this.buffersNames!.Length - 1; i >= 0; i--)
            {
                var bufferIndex = i;
                builder.AddItem(this.buffersNames[bufferIndex], this.inputs.Buffer.ElementAtOrDefault(bufferIndex), null, null, () => this.application.ViewerBufferIndex = bufferIndex, state => state.IsChecked = this.application.ViewerBufferIndex == bufferIndex);
            }
        }
    }
}
