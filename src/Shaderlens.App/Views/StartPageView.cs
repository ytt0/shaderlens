using System.Windows.Shapes;
using Microsoft.Win32;

namespace Shaderlens.Views
{
    using Path = System.Windows.Shapes.Path;

    public interface IStartPageView
    {
        void Show();
        void SetTheme(IApplicationTheme theme);
    }

    public partial class StartPageView : IStartPageView
    {
        private interface IProjectItem
        {
            FrameworkElement Content { get; }
            string Path { get; }
            bool IsVisible { get; }
            bool IsPinned { get; set; }
            bool IsSelected { get; set; }
            int RecentIndex { get; }

            void SetFilter(string filter);
        }

        private class ProjectItem : IProjectItem
        {
            public FrameworkElement Content { get { return this.content; } }
            public string Path { get; }
            public int RecentIndex { get; }
            public bool IsVisible { get; private set; }
            public bool IsPinned { get; set; }

            private bool isSelected;
            public bool IsSelected
            {
                get { return this.isSelected; }
                set
                {
                    this.isSelected = value;
                    if (this.isSelected)
                    {
                        this.content.SetReference(Border.BackgroundProperty, this.theme.ControlHoveredBackground);
                    }
                    else
                    {
                        this.content.Background = Brushes.Transparent;
                    }
                }
            }

            private readonly StartPageView view;
            private readonly IApplicationTheme theme;
            private readonly TextBlock headerTextBlock;
            private readonly TextBlock pathTextBlock;
            private readonly ImplicitButton content;
            private readonly string header;

            public ProjectItem(StartPageView view, IApplicationTheme theme, string path, int recentIndex, IMenuResourcesFactory resources)
            {
                this.view = view;
                this.theme = theme;

                this.Path = path;
                this.RecentIndex = recentIndex;
                this.header = String.Join('\\', path.Split('\\').TakeLast(2));

                this.headerTextBlock = new TextBlock { Text = this.header, Margin = new Thickness(4, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
                this.pathTextBlock = new TextBlock { Text = this.Path, Margin = new Thickness(28, 0, 0, 0) }.WithReference(TextBlock.ForegroundProperty, this.theme.CommentTextForeground);

                var icon = System.IO.Path.GetExtension(path).Equals(".json", StringComparison.InvariantCultureIgnoreCase) ? resources.CreateProjectIcon() : resources.CreateFileCodeIcon();

                this.content = new ImplicitButton(theme)
                {
                    Content = new StackPanel().WithChildren
                    (
                        new StackPanel { Orientation = Orientation.Horizontal }.WithChildren(icon, this.headerTextBlock),
                        this.pathTextBlock
                    ),
                    Background = Brushes.Transparent,
                    Padding = Spacing,
                    CornerRadius = new CornerRadius(4)
                };

                this.content.Click += OnClick;
                this.content.MouseUp += OnMouseUp;

                this.IsVisible = true;
            }

            private void OnMouseUp(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    this.view.OpenContextMenu(this);
                    e.Handled = true;
                }
            }

            private void OnClick(object sender, RoutedEventArgs e)
            {
                this.view.OpenProject(this.Path);
                e.Handled = true;
            }

            public void SetFilter(string filter)
            {
                if (String.IsNullOrWhiteSpace(filter))
                {
                    this.headerTextBlock.Text = this.header;
                    this.pathTextBlock.Text = this.Path;
                    this.IsVisible = true;
                    this.Content.Visibility = Visibility.Visible;
                    return;
                }

                var words = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries).OrderByDescending(word => word.Length).ToArray();
                if (words.All(word => this.Path.Contains(word, StringComparison.InvariantCultureIgnoreCase)))
                {
                    SetTextBlockHighlight(this.headerTextBlock, this.header, words);
                    SetTextBlockHighlight(this.pathTextBlock, this.Path, words);
                    this.IsVisible = true;
                    this.Content.Visibility = Visibility.Visible;
                    return;
                }

                this.IsVisible = false;
                this.Content.Visibility = Visibility.Collapsed;
            }

            private void SetTextBlockHighlight(TextBlock textBlock, string text, string[] words)
            {
                textBlock.Inlines.Clear();
                while (text.Length > 0)
                {
                    var pair = words.Select(word => (Word: word, Index: text.IndexOf(word, StringComparison.InvariantCultureIgnoreCase))).
                        OrderBy(pair => pair.Index).FirstOrDefault(pair => pair.Index != -1);

                    if (pair.Word != null)
                    {
                        textBlock.Inlines.Add(text.Substring(0, pair.Index));

                        textBlock.Inlines.Add(new Run
                        {
                            Text = text.Substring(pair.Index, pair.Word.Length)
                        }.
                        WithContentReference(TextElement.ForegroundProperty, this.theme.TextHighlightForeground).
                        WithContentReference(TextElement.BackgroundProperty, this.theme.TextHighlightBackground));

                        text = text.Substring(pair.Index + pair.Word.Length);
                        continue;
                    }

                    textBlock.Inlines.Add(text);
                    return;
                }
            }
        }

        private static readonly Lazy<Geometry> ClearGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 10,0 0,10 M 0,0 10,10").WithFreeze());
        private static readonly Thickness Spacing = new Thickness(10, 5, 10, 5);
        private static readonly Thickness VerticalSpacing = new Thickness(0, Spacing.Top, 0, Spacing.Bottom);
        private readonly Window window;
        private readonly IApplication application;
        private readonly IApplicationSettings settings;
        private readonly IApplicationTheme theme;
        private readonly OpenFileDialog openFileDialog;
        private readonly OpenFolderDialog openFolderDialog;
        private readonly WindowContainer windowContainer;
        private readonly List<IProjectItem> pinnedProjectsItems;
        private readonly List<IProjectItem> recentProjectsItems;
        private readonly StackPanel pinnedProjectsPanel;
        private readonly StackPanel recentProjectsPanel;
        private readonly TextBlock recentProjectsHeaderTextBlock;
        private readonly TextBlock pinnedProjectsHeaderTextBlock;
        private readonly StyledContextMenu contextMenu;
        private readonly StyledTextBox searchTextBox;
        private IProjectItem? selectedProjectItem;
        private IProjectItem[] visibleItems;
        private readonly Panel startPagePanel;
        private readonly Grid content;
        private IProjectCreationViewContent? projectCreationView;
        private readonly TextBlock headerTextBlock;

        public StartPageView(Window window, IApplication application, IApplicationSettings settings, IApplicationTheme theme, OpenFileDialog openFileDialog, OpenFolderDialog openFolderDialog)
        {
            this.window = window;
            this.application = application;
            this.settings = settings;
            this.theme = theme;
            this.openFileDialog = openFileDialog;
            this.openFolderDialog = openFolderDialog;

            this.windowContainer = new WindowContainer(window, theme, settings.StartPageWindowState, "Shaderlens");

            this.window.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape && this.window.Owner != null)
                {
                    this.window.Close();
                }
            };

            var resources = new MenuResourcesFactory(this.theme.Menu);

            this.pinnedProjectsItems = new List<IProjectItem>();
            this.recentProjectsItems = new List<IProjectItem>();

            this.pinnedProjectsPanel = new StackPanel { Margin = VerticalSpacing };
            this.pinnedProjectsHeaderTextBlock = new TextBlock { Text = "Pinned Projects", FontWeight = FontWeights.Bold, Margin = VerticalSpacing };
            foreach (var path in this.settings.PinnedProjects)
            {
                var item = new ProjectItem(this, this.theme, path, this.settings.RecentProjects.IndexOf(path), resources) { IsPinned = true };
                this.pinnedProjectsItems.Add(item);
                this.pinnedProjectsPanel.Children.Add(item.Content);
            }

            var recentIndex = 0;
            this.recentProjectsPanel = new StackPanel { Margin = VerticalSpacing };
            this.recentProjectsHeaderTextBlock = new TextBlock { Text = "Recent Projects", FontWeight = FontWeights.Bold, Margin = VerticalSpacing };
            foreach (var path in this.settings.RecentProjects.Except(this.settings.PinnedProjects))
            {
                var item = new ProjectItem(this, this.theme, path, recentIndex++, resources);
                this.recentProjectsItems.Add(item);
                this.recentProjectsPanel.Children.Add(item.Content);
            }

            SetVisibleItemsList();
            SetHeadersVisiblity();

            var clearSearchButton = new ImplicitButton(theme)
            {
                Content = CreatePath(ClearGeometry.Value, this.theme.IconForeground),
                Background = Brushes.Transparent,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Right,
                Visibility = Visibility.Collapsed,
                CornerRadius = new CornerRadius(4)
            };

            this.searchTextBox = new StyledTextBox(this.theme) { Padding = new Thickness(3, 0, 24, 0) };
            this.searchTextBox.TextChanged += (sender, e) =>
            {
                var text = this.searchTextBox.Text;

                foreach (var item in this.pinnedProjectsItems.Concat(this.recentProjectsItems))
                {
                    item.SetFilter(text);
                }

                clearSearchButton.Visibility = String.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;

                SetVisibleItemsList();
                SetHeadersVisiblity();
            };

            this.searchTextBox.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape && !String.IsNullOrEmpty(this.searchTextBox.Text))
                {
                    this.searchTextBox.Clear();
                    e.Handled = true;
                }

                if (e.Key == Key.Enter && this.selectedProjectItem != null)
                {
                    OpenProject(this.selectedProjectItem.Path);
                    e.Handled = true;
                }

                if (e.Key == Key.Up)
                {
                    MoveSelection(-1);
                    e.Handled = true;
                }

                if (e.Key == Key.Down)
                {
                    MoveSelection(1);
                    e.Handled = true;
                }

                if (e.Key == Key.PageUp)
                {
                    MoveSelection(-8);
                    e.Handled = true;
                }

                if (e.Key == Key.PageDown)
                {
                    MoveSelection(8);
                    e.Handled = true;
                }
            };

            clearSearchButton.Click += (sender, e) => this.searchTextBox.Clear();

            var logoImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Logo.png")),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            }.WithReference(FrameworkElement.HeightProperty, this.theme.WindowFontSize);

            GetAssemblyVersion(Assembly.GetExecutingAssembly(), out var semVer, out var informationalVersion);

            this.headerTextBlock = new TextBlock { Text = "Shaderlens", FontSize = this.theme.WindowFontSize.Value * 1.5, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center };
            var versionTextBlock = new TextBlock { Text = semVer, ToolTip = informationalVersion, Margin = Spacing, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom }.WithReference(TextElement.ForegroundProperty, theme.CommentTextForeground);

            var actionsPanel = new StackPanel().WithChildren
            (
                new StyledButton(this.theme) { Content = "New...", Margin = Spacing }.WithHandler(ButtonBase.ClickEvent, OnNewButtonClicked),
                new StyledButton(this.theme) { Content = "Open...", Margin = Spacing }.WithHandler(ButtonBase.ClickEvent, OnOpenButtonClicked)
            );

            this.contextMenu = new StyledContextMenu(this.theme.Menu) { LayoutTransform = this.windowContainer.Transform };

            this.theme.SetResources(this.contextMenu.Resources);
            MenuPreserveSelectionBehavior.Register(this.contextMenu);

            var menuBuilder = new MenuBuilder(this.contextMenu, this.theme.Menu);
            menuBuilder.AddItem("Open Project", null, resources.CreateProjectIcon(), null, () => OpenProject(this.selectedProjectItem!.Path), null);
            menuBuilder.AddSeparator();
            menuBuilder.AddItem("Open Folder", null, resources.CreateFolderIcon(), null, () => this.application.OpenExternalPath(System.IO.Path.GetDirectoryName(this.selectedProjectItem!.Path)!), null);
            menuBuilder.AddItem("Copy Path", null, resources.CreateCopyIcon(), null, () => this.application.Clipboard.SetText(this.selectedProjectItem!.Path));
            menuBuilder.AddSeparator();
            menuBuilder.AddItem("Pin Item", null, null, null, () => ToggleProjectItemPin(this.selectedProjectItem!), state => state.IsChecked = this.selectedProjectItem!.IsPinned);
            menuBuilder.AddSeparator();
            menuBuilder.AddItem("Remove Item", null, resources.CreateRemoveIcon(), null, () => RemoveProjectItem(this.selectedProjectItem!), null);

            if (settings.PinnedProjects.Any() || settings.RecentProjects.Any())
            {
                this.startPagePanel = new DockPanel { LastChildFill = true }.WithChildren
                (
                    new StackPanel { Orientation = Orientation.Horizontal, Margin = Spacing }.WithChildren
                    (
                        logoImage.WithValue(FrameworkElement.LayoutTransformProperty, new ScaleTransform(2.0, 2.0)).WithValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 8, 4)),
                        this.headerTextBlock
                    ).WithDock(Dock.Top),
                    actionsPanel.WithDock(Dock.Right),
                    new DockPanel { LastChildFill = true }.WithChildren
                    (
                        new StackPanel().WithDock(Dock.Top).WithChildren
                        (
                            new TextBlock { Text = "Open Recent Project", FontWeight = FontWeights.Bold, Margin = Spacing },
                            new Grid { Margin = Spacing }.WithChildren(this.searchTextBox, clearSearchButton)
                        ),
                        new StyledScrollViewer(this.theme.ScrollBar)
                        {
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                            Margin = Spacing,
                            Content = new StackPanel().WithChildren
                            (
                                this.pinnedProjectsHeaderTextBlock,
                                this.pinnedProjectsPanel,
                                this.recentProjectsHeaderTextBlock,
                                this.recentProjectsPanel
                            )
                        }
                    )
                );
            }
            else
            {
                this.startPagePanel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }.
                WithChildren
                (
                    logoImage.WithValue(FrameworkElement.LayoutTransformProperty, new ScaleTransform(8.0, 8.0)),
                    this.headerTextBlock.
                        WithValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center).
                        WithValue(FrameworkElement.MarginProperty, new Thickness(0, 10, 0, 10)),
                    actionsPanel
                );
            }

            this.startPagePanel = new Grid { Margin = Spacing }.WithChildren(this.startPagePanel, versionTextBlock);

            this.content = new Grid().WithChildren(this.startPagePanel);
            this.windowContainer.SetContent(this.content);
        }

        public void Show()
        {
            this.startPagePanel.Visibility = Visibility.Visible;

            if (this.projectCreationView != null)
            {
                this.projectCreationView.Content.Visibility = Visibility.Collapsed;
            }

            this.windowContainer.Show();
            this.searchTextBox.Focus();
        }

        public void SetTheme(IApplicationTheme theme)
        {
            this.windowContainer.SetTheme(theme);
            this.headerTextBlock.FontSize = this.theme.WindowFontSize.Value * 1.5;
        }

        [MemberNotNull(nameof(this.visibleItems))]
        private void SetVisibleItemsList()
        {
            this.visibleItems = this.pinnedProjectsItems.Concat(this.recentProjectsItems).Where(item => item.IsVisible).ToArray();
            if (this.visibleItems.Length == 1 || this.selectedProjectItem == null || !this.selectedProjectItem.IsVisible)
            {
                SetSelectedItem(this.visibleItems.FirstOrDefault());
            }
        }

        private void SetHeadersVisiblity()
        {
            var pinnedVisiblity = this.pinnedProjectsItems.Any(item => item.Content.Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
            var recentVisiblity = this.recentProjectsItems.Any(item => item.Content.Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
            this.pinnedProjectsHeaderTextBlock.Visibility = pinnedVisiblity;
            this.pinnedProjectsPanel.Visibility = pinnedVisiblity;
            this.recentProjectsHeaderTextBlock.Visibility = recentVisiblity;
            this.recentProjectsPanel.Visibility = recentVisiblity;
        }

        private void SetSettingsValue()
        {
            this.settings.PinnedProjects = this.pinnedProjectsItems.Select(item => item.Path).ToArray();
            this.settings.RecentProjects = this.recentProjectsItems.Select(item => item.Path).ToArray();
        }

        private void SetSelectedItem(IProjectItem? projectItem)
        {
            if (this.selectedProjectItem != null)
            {
                this.selectedProjectItem.IsSelected = false;
            }

            this.selectedProjectItem = projectItem;

            if (this.selectedProjectItem != null)
            {
                this.selectedProjectItem.IsSelected = true;
                this.selectedProjectItem.Content.BringIntoView();
            }
        }

        private void MoveSelection(int offset)
        {
            SetSelectedItem(
                this.visibleItems.ElementAtOrDefault(this.visibleItems.IndexOf(this.selectedProjectItem) + offset) ??
                (offset < 0 ? this.visibleItems.FirstOrDefault() : this.visibleItems.LastOrDefault()));
        }

        private void OpenProject(string path)
        {
            if (!this.application.ShowProjectCloseDialog())
            {
                return;
            }

            this.application.OpenProject(path);
            this.window.Hide();
        }

        private void ToggleProjectItemPin(IProjectItem projectItem)
        {
            if (projectItem.IsPinned)
            {
                this.pinnedProjectsItems.Remove(projectItem);
                this.pinnedProjectsPanel.Children.Remove(projectItem.Content);

                var insertIndex = this.recentProjectsItems.Count(item => item.RecentIndex < projectItem.RecentIndex);
                this.recentProjectsItems.Insert(insertIndex, projectItem);
                this.recentProjectsPanel.Children.Insert(insertIndex, projectItem.Content);
            }
            else
            {
                this.recentProjectsItems.Remove(projectItem);
                this.recentProjectsPanel.Children.Remove(projectItem.Content);

                this.pinnedProjectsItems.Insert(0, projectItem);
                this.pinnedProjectsPanel.Children.Insert(0, projectItem.Content);
            }

            projectItem.IsPinned = !projectItem.IsPinned;

            SetVisibleItemsList();
            SetHeadersVisiblity();
            SetSettingsValue();

            projectItem.Content.BringIntoView();
        }

        private void RemoveProjectItem(IProjectItem projectItem)
        {
            if (projectItem.IsPinned)
            {
                this.pinnedProjectsItems.Remove(projectItem);
                this.pinnedProjectsPanel.Children.Remove(projectItem.Content);
            }
            else
            {
                this.recentProjectsItems.Remove(projectItem);
                this.recentProjectsPanel.Children.Remove(projectItem.Content);
            }

            SetVisibleItemsList();
            SetHeadersVisiblity();
            SetSettingsValue();
        }

        private void OpenContextMenu(IProjectItem projectItem)
        {
            SetSelectedItem(projectItem);
            this.contextMenu.IsOpen = true;
        }

        private void OnNewButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.projectCreationView == null)
            {
                var templates = this.application.GetProjectTemplates();
                this.projectCreationView = new ProjectCreationViewContent(this.window, this.theme, this.openFolderDialog, templates, this.settings.CreateProjectTemplateIndex, this.settings.CreateProjectPath, this.settings.CreateProjectOpenFolder, true);
                this.projectCreationView.BackButtonClicked += OnBackButtonClicked;
                this.projectCreationView.CreateProjectRequested += OnProjectCreateRequested;
                this.content.Children.Add(this.projectCreationView.Content);
            }

            this.startPagePanel.Visibility = Visibility.Collapsed;
            this.projectCreationView.Content.Visibility = Visibility.Visible;
        }

        private void OnProjectCreateRequested(object? sender, CreateProjectRequestEventArgs e)
        {
            var projectPath = e.SelectedTemplate.CreateProject(e.TargetRootPath, e.TargetProjectName, e.Parameters);
            this.settings.CreateProjectPath = e.Location;
            this.settings.CreateProjectTemplateIndex = e.SelectedTemplateIndex;
            this.settings.CreateProjectOpenFolder = e.OpenFolder;

            if (e.OpenFolder)
            {
                this.application.OpenExternalPath(System.IO.Path.GetDirectoryName(projectPath)!);
            }

            OpenProject(projectPath);
        }

        private void OnBackButtonClicked(object? sender, EventArgs e)
        {
            this.startPagePanel.Visibility = Visibility.Visible;
            this.projectCreationView!.Content.Visibility = Visibility.Collapsed;
        }

        private void OnOpenButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this.window, this.settings.OpenProjectPath) == true)
            {
                var projectPath = this.openFileDialog.FileName;
                this.settings.OpenProjectPath = projectPath;
                OpenProject(projectPath);
            }
        }

        private static void GetAssemblyVersion(Assembly assembly, out string semVer, out string informationalVersion)
        {
            var assemblyVersion = assembly.GetName().Version?.ToString() ?? "unknown";
            var assemblyInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (assemblyInformationalVersion != null)
            {
                var match = InformationalVersionRegex().Match(assemblyInformationalVersion);
                semVer = match.Success ? match.Groups["semVer"].Value : assemblyVersion;

                informationalVersion = assemblyInformationalVersion;
            }
            else
            {
                semVer = assemblyVersion;
                informationalVersion = assemblyVersion;
            }
        }

        private static Path CreatePath(Geometry geometry, IThemeResource<Brush> stroke)
        {
            return new Path
            {
                Data = geometry,
                StrokeThickness = 1.0,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 10,
                Height = 10,
                Margin = new Thickness(8),
            }.WithReference(Shape.StrokeProperty, stroke);
        }

        [GeneratedRegex("^(?<semVer>.*?)(\\+|\\.Branch\\.|\\.Sha\\.)")]
        private static partial Regex InformationalVersionRegex();
    }
}
