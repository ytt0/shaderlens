namespace Shaderlens.Views.Menus
{
    public class SpeedMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationCommands commands;

        public SpeedMenuSource(IApplication application, IApplicationCommands commands)
        {
            this.application = application;
            this.commands = commands;
        }

        public void AddTo(IMenuBuilder builder)
        {
            this.commands.Speed1_16.AddMenuItem(builder, "1/16x");
            this.commands.Speed1_8.AddMenuItem(builder, "1/8x");
            this.commands.Speed1_4.AddMenuItem(builder, "1/4x");
            this.commands.Speed1_2.AddMenuItem(builder, "1/2x");
            builder.AddSeparator();
            this.commands.SpeedNormal.AddMenuItem(builder, "Normal");
            builder.AddSeparator();
            this.commands.Speed2.AddMenuItem(builder, "2x");
            this.commands.Speed4.AddMenuItem(builder, "4x");
            this.commands.Speed8.AddMenuItem(builder, "8x");
            this.commands.Speed16.AddMenuItem(builder, "16x");

            var customSpeedHeader = new TextBlock();
            builder.AddSeparator();
            builder.AddItem(customSpeedHeader, null, null, null, () => { }, state =>
            {
                state.IsChecked = this.application.Speed < 1.0 / 16 || this.application.Speed > 16;
                state.IsVisible = this.application.Speed < 1.0 / 16 || this.application.Speed > 16;
                customSpeedHeader.Text = this.application.Speed > 1.0 ? $"{(int)Math.Round(this.application.Speed)}x" : $"1/{(int)Math.Round(1.0 / this.application.Speed)}x";
            });
        }
    }
}
