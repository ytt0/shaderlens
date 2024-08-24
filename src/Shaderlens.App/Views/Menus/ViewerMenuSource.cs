namespace Shaderlens.Views.Menus
{
    public class ViewerMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationCommands commands;
        private readonly IProjectSource? project;

        public ViewerMenuSource(IApplication application, IApplicationCommands commands, IProjectSource? project)
        {
            this.application = application;
            this.commands = commands;
            this.project = project;
        }

        public void AddTo(IMenuBuilder builder)
        {
            if (this.project == null)
            {
                builder.AddEmptyItem();
                return;
            }

            this.commands.ViewerNone.AddMenuItem(builder, "None");
            builder.AddSeparator();
            this.commands.ViewerValuesOverlay.AddMenuItem(builder, "Values Overlay");

            if (this.project?.Viewers.Any() == true)
            {
                foreach (var viewer in this.project.Viewers)
                {
                    var viewerPass = new ViewerPassSelection(viewer.Key);
                    builder.AddItem(viewer.Program.DisplayName, null, null, null, () => this.application.ViewerPass = viewerPass, state => state.IsChecked = this.application.ViewerPass.Equals(viewerPass));
                }
            }
        }
    }
}
