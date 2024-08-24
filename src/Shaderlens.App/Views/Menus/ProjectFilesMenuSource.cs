namespace Shaderlens.Views.Menus
{
    public class ProjectFilesMenuSource : IMenuSource
    {
        private readonly IApplication application;
        private readonly IApplicationInputs inputs;
        private readonly IProjectSource? project;
        private readonly IMenuResourcesFactory resources;

        public ProjectFilesMenuSource(IApplication application, IApplicationInputs inputs, IMenuResourcesFactory resources, IProjectSource? project)
        {
            this.application = application;
            this.inputs = inputs;
            this.resources = resources;
            this.project = project;
        }

        public void AddTo(IMenuBuilder builder)
        {
            if (this.project == null)
            {
                builder.AddEmptyItem();
                return;
            }

            var basePath = this.project.Source?.Key.AbsolutePath != null ?
                Path.GetDirectoryName(this.project.Source.Key.AbsolutePath)! :
                Path.GetDirectoryName(this.project.Passes.Image!.Program.Source.First().Key.AbsolutePath)!;

            AddItem(builder, "Open Containing Folder", basePath, this.inputs.ProjectOpenFolder, this.resources.CreateFolderIcon());
            builder.AddSeparator();

            // program

            var addedPaths = new HashSet<string>();

            AddProgramSourceLocation(builder, basePath, addedPaths, this.project.Common);
            foreach (var pass in this.project.Passes)
            {
                AddProgramSourceLocation(builder, basePath, addedPaths, pass.Program.Source);
            }
            builder.AddSeparator();

            // viewer program

            foreach (var viewer in this.project.Viewers)
            {
                AddProgramSourceLocation(builder, basePath, addedPaths, viewer.Program.Source);
            }
            builder.AddSeparator();

            // project

            if (this.project.Source != null)
            {
                TryAddRelativeItem(builder, basePath, addedPaths, this.project.Source, this.resources.CreateFileProjectIcon());
            }
            builder.AddSeparator();

            // settings

            TryAddRelativeItem(builder, basePath, addedPaths, this.project.UniformsSettings, this.resources.CreateFileUniformsIcon());
        }

        private void AddProgramSourceLocation(IMenuBuilder builder, string basePath, HashSet<string> addedPaths, IProjectProgramSource? projectProgramSource)
        {
            if (projectProgramSource != null)
            {
                foreach (var source in projectProgramSource)
                {
                    TryAddRelativeItem(builder, basePath, addedPaths, source, this.resources.CreateFileCodeIcon());
                }
            }
        }

        private void TryAddRelativeItem(IMenuBuilder builder, string basePath, HashSet<string> addedPaths, IFileResource<string>? resource, object? icon)
        {
            if (resource != null && addedPaths.Add(resource.Key.AbsolutePath))
            {
                AddItem(builder, Path.GetRelativePath(basePath, resource.Key.AbsolutePath), resource.Key.AbsolutePath, null, icon);
            }
        }

        private void AddItem(IMenuBuilder builder, string header, string absolutePath, IInputSpan? inputSpan, object? icon)
        {
            builder.AddItem(header.Replace("_", "__"), inputSpan, icon, absolutePath, () => this.application.OpenExternalPath(absolutePath), state => state.SetIsEnabled(Path.Exists(absolutePath)));
        }
    }
}