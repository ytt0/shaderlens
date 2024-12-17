namespace Shaderlens.Views.Menus
{
    public class BuffersMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationInputs inputs;
        private readonly IProjectSource? project;
        private readonly IMenuTheme theme;

        public BuffersMenuSource(IApplication application, IApplicationInputs inputs, IProjectSource? project, IMenuTheme theme)
        {
            this.application = application;
            this.inputs = inputs;
            this.project = project;
            this.theme = theme;
        }

        public void AddTo(IMenuBuilder builder)
        {
            if (this.project == null)
            {
                builder.AddEmptyItem();
                return;
            }

            AddItem(builder, project.Passes.Image, this.inputs.BufferImage, project.Passes.Count - 1);

            builder.AddSeparator();

            var buffers = project.Passes.Buffers.ToArray();

            for (var i = buffers.Length - 1; i >= 0; i--)
            {
                AddItem(builder, buffers[i], this.inputs.Buffer.ElementAtOrDefault(i), i);
            }
        }

        private void AddItem(IMenuBuilder builder, IProjectPass? pass, IInputSpanEvent? inputSpanEvent, int index)
        {
            if (pass == null)
            {
                return;
            }

            if (pass.Outputs == 1)
            {
                builder.AddItem(pass.Program.DisplayName, inputSpanEvent, null, null, () => this.application.SetViewerBufferIndex(index, 0), state => state.IsChecked = this.application.ViewerBufferIndex == index);
                return;
            }

            for (var i = 0; i < pass.Outputs; i++)
            {
                var textureIndex = i;
                builder.AddItem(CreateHeader(pass.Program.DisplayName, textureIndex, this.theme), inputSpanEvent, null, null, () => this.application.SetViewerBufferIndex(index, textureIndex), state => state.IsChecked = this.application.ViewerBufferIndex == index && this.application.ViewerBufferTextureIndex == textureIndex);
            }
        }

        private static FrameworkElement CreateHeader(string name, int index, IMenuTheme theme)
        {
            return new StackPanel { Orientation = Orientation.Horizontal }.WithChildren
            (
                new TextBlock
                {
                    Text = name,
                    VerticalAlignment = VerticalAlignment.Center
                },
                new Border
                {
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(8, 0, 0, 0),
                    Padding = new Thickness(4, 2, 4, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = new TextBlock
                    {
                        Text = index.ToString(),
                        VerticalAlignment = VerticalAlignment.Center
                    }.
                    WithReference(TextBlock.FontFamilyProperty, theme.CodeFontFamily).
                    WithReference(TextBlock.FontSizeProperty, theme.CodeFontSize)
                }.WithReference(Border.BorderBrushProperty, theme.ValueBorder)
            );
        }
    }
}
