namespace Shaderlens.Views.Menus
{
    public class FrameRateMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationCommands commands;

        public FrameRateMenuSource(IApplication application, IApplicationCommands commands)
        {
            this.application = application;
            this.commands = commands;
        }

        public void AddTo(IMenuBuilder builder)
        {
            this.commands.FrameRateFull.AddMenuItem(builder, "Full Rate", null, null, state => state.IsChecked = this.application.FrameRate == 1);
            builder.AddSeparator();
            this.commands.FrameRate2.AddMenuItem(builder, "1/2 Rate", null, null, state => state.IsChecked = this.application.FrameRate == 2);
            this.commands.FrameRate4.AddMenuItem(builder, "1/4 Rate", null, null, state => state.IsChecked = this.application.FrameRate == 4);
            this.commands.FrameRate8.AddMenuItem(builder, "1/8 Rate", null, null, state => state.IsChecked = this.application.FrameRate == 8);
            this.commands.FrameRate16.AddMenuItem(builder, "1/16 Rate", null, null, state => state.IsChecked = this.application.FrameRate == 16);

            var customFrameRateHeader = new TextBlock();
            builder.AddSeparator();
            builder.AddItem(customFrameRateHeader, null, null, null, () => { }, state =>
            {
                state.IsChecked = this.application.FrameRate > 16;
                state.IsVisible = this.application.FrameRate > 16;
                customFrameRateHeader.Text = $"1/{this.application.FrameRate} Rate";
            });
        }
    }
}