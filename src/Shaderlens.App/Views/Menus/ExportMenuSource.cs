namespace Shaderlens.Views.Menus
{
    public class ExportMenuSource : IMenuSource
    {
        private class RepeatSaveFrameHeader : VisualChildContainer
        {
            public string Header
            {
                get { return this.headerTextBlock.Text; }
                set { this.headerTextBlock.Text = value; }
            }

            public string? SaveFramePath
            {
                get { return this.pathTextBlock.Text; }
                set
                {
                    this.pathTextBlock.Text = Path.GetFileName(value);
                    this.pathBorder.ToolTip = value;
                }
            }

            protected override int VisualChildrenCount { get { return 1; } }

            private readonly TextBlock headerTextBlock;
            private readonly TextBlock pathTextBlock;
            private readonly Border pathBorder;
            private readonly StackPanel child;

            public RepeatSaveFrameHeader(IMenuTheme theme)
            {
                this.headerTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
                this.pathTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center }.WithReference(TextBlock.FontFamilyProperty, theme.CodeFontFamily);

                this.pathBorder = new Border
                {
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(8, 0, 0, 0),
                    Padding = new Thickness(4, 2, 4, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = this.pathTextBlock
                }.WithReference(Border.BorderBrushProperty, theme.ValueBorder);

                this.child = new StackPanel { Orientation = Orientation.Horizontal }.WithChildren(this.headerTextBlock, this.pathBorder);
            }

            protected override FrameworkElement GetChild()
            {
                return this.child;
            }
        }

        private readonly IApplication application;
        private readonly IApplicationCommands commands;
        private readonly IMenuTheme theme;

        public ExportMenuSource(IApplication application, IApplicationCommands commands, IMenuTheme theme)
        {
            this.application = application;
            this.commands = commands;
            this.theme = theme;
        }

        public void AddTo(IMenuBuilder builder)
        {
            var repeatSaveFrameHeader = new RepeatSaveFrameHeader(this.theme) { Header = "Save Current Frame" };

            this.commands.ExportFrameRepeat.AddMenuItem(builder, repeatSaveFrameHeader, null, null, state => repeatSaveFrameHeader.SaveFramePath = this.application.SaveFramePath);
            this.commands.ExportFrame.AddMenuItem(builder, "Save Current Frame As...");
            builder.AddSeparator();
            this.commands.ExportRenderSequence.AddMenuItem(builder, "Render Sequence...");
        }
    }
}