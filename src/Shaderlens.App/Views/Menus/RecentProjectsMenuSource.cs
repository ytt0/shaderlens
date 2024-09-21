namespace Shaderlens.Views.Menus
{
    public class RecentProjectsMenuSource : IMenuSource
    {
        private const int MaxItemsCount = 20;

        private readonly IApplication application;
        private readonly IApplicationInputs inputs;
        private readonly IMenuResourcesFactory resources;

        public RecentProjectsMenuSource(IApplication application, IApplicationInputs inputs, IMenuResourcesFactory resources)
        {
            this.application = application;
            this.inputs = inputs;
            this.resources = resources;
        }

        public void AddTo(IMenuBuilder builder)
        {
            var maxPinnedItemsCount = MaxItemsCount - Math.Min(10, this.application.RecentProjects.Count());

            var addedPinnedItemsCount = 0;
            var addedRecentItemsCount = 0;

            foreach (var path in this.application.PinnedProjects)
            {
                AddProjectItem(builder, path, this.inputs.PinnedProject.ElementAtOrDefault(addedPinnedItemsCount));
                addedPinnedItemsCount++;

                if (addedPinnedItemsCount == maxPinnedItemsCount)
                {
                    break;
                }
            }

            builder.AddSeparator();

            foreach (var path in this.application.RecentProjects)
            {
                AddProjectItem(builder, path, this.inputs.RecentProject.ElementAtOrDefault(addedRecentItemsCount));
                addedRecentItemsCount++;

                if (addedPinnedItemsCount + addedRecentItemsCount == MaxItemsCount)
                {
                    break;
                }
            }

            if (addedPinnedItemsCount == 0 && addedRecentItemsCount == 0)
            {
                builder.AddEmptyItem();
            }
        }

        private void AddProjectItem(IMenuBuilder builder, string path, IInputSpanEvent? inputSpanEvent)
        {
            var displayName = path.Contains('\\') ? path.Substring(path.Substring(0, path.LastIndexOf('\\')).LastIndexOf('\\') + 1) : path;
            var icon = Path.GetExtension(path).Equals(".json", StringComparison.InvariantCultureIgnoreCase) ? this.resources.CreateProjectIcon() : this.resources.CreateFileCodeIcon();
            builder.AddItem(displayName, inputSpanEvent, icon, path, () => this.application.OpenProject(path));
        }
    }
}
