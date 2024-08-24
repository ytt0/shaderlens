namespace Shaderlens.Views.Menus
{
    public class ResolutionMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationCommands commands;

        public ResolutionMenuSource(IApplication application, IApplicationCommands commands)
        {
            this.application = application;
            this.commands = commands;
        }

        public void AddTo(IMenuBuilder builder)
        {
            this.commands.ResolutionFull.AddMenuItem(builder, "Full Resolution");
            builder.AddSeparator();
            this.commands.Resolution2.AddMenuItem(builder, "1/2 Resolution");
            this.commands.Resolution4.AddMenuItem(builder, "1/4 Resolution");
            this.commands.Resolution8.AddMenuItem(builder, "1/8 Resolution");
            this.commands.Resolution16.AddMenuItem(builder, "1/16 Resolution");
            this.commands.Resolution32.AddMenuItem(builder, "1/32 Resolution");
            this.commands.Resolution64.AddMenuItem(builder, "1/64 Resolution");

            var customResolutionHeader = new TextBlock();
            builder.AddSeparator();
            builder.AddItem(customResolutionHeader, null, null, null, () => { }, state =>
            {
                state.IsChecked = this.application.RenderDownscale > 64;
                state.IsVisible = this.application.RenderDownscale > 64;
                customResolutionHeader.Text = $"1/{this.application.RenderDownscale} Resolution";
            });
        }
    }
}
