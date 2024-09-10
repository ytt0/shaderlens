using Microsoft.Win32;

namespace Shaderlens.Views
{
    public interface IProjectCreationView
    {
        void Show();
        void SetTheme(IApplicationTheme theme);
    }

    public interface IProjectCreationViewContent
    {
        event EventHandler<CreateProjectRequestEventArgs>? CreateProjectRequested;
        event EventHandler? BackButtonClicked;
        FrameworkElement Content { get; }
    }

    public class CreateProjectRequestEventArgs : EventArgs
    {
        public IProjectTemplate SelectedTemplate { get; }
        public int SelectedTemplateIndex { get; }
        public string Location { get; }
        public string TargetRootPath { get; }
        public string TargetProjectName { get; }
        public bool OpenFolder { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        public CreateProjectRequestEventArgs(IProjectTemplate selectedTemplate, int selectedTemplateIndex, string location, string targetRootPath, string targetProjectName, bool openFolder, Dictionary<string, string> parameters)
        {
            this.SelectedTemplate = selectedTemplate;
            this.SelectedTemplateIndex = selectedTemplateIndex;
            this.Location = location;
            this.TargetRootPath = targetRootPath;
            this.TargetProjectName = targetProjectName;
            this.OpenFolder = openFolder;
            this.Parameters = parameters;
        }
    }

    public class ProjectCreationView : IProjectCreationView
    {
        private readonly WindowContainer windowContainer;

        public ProjectCreationView(Window window, IApplication application, IApplicationSettings settings, IApplicationTheme theme, OpenFolderDialog openFolderDialog, IEnumerable<IProjectTemplate> templates, int selectedTemplateIndex, string location)
        {
            this.windowContainer = new WindowContainer(window, theme, settings.CreateProjectWindowState, "New Project");

            var viewContent = new ProjectCreationViewContent(window, theme, openFolderDialog, templates, selectedTemplateIndex, location, settings.CreateProjectOpenFolder, false);
            viewContent.CreateProjectRequested += (sender, e) =>
            {
                if (!application.ShowProjectCloseDialog())
                {
                    return;
                }

                var projectPath = e.SelectedTemplate.CreateProject(e.TargetRootPath, e.TargetProjectName, e.Parameters);
                settings.CreateProjectPath = e.Location;
                settings.CreateProjectTemplateIndex = e.SelectedTemplateIndex;
                settings.CreateProjectOpenFolder = e.OpenFolder;

                if (e.OpenFolder)
                {
                    application.OpenExternalPath(Path.GetDirectoryName(projectPath)!);
                }

                application.OpenProject(projectPath);
                window.Hide();
            };

            this.windowContainer.SetContent(viewContent.Content);
        }

        public void Show()
        {
            this.windowContainer.Show();
        }

        public void SetTheme(IApplicationTheme theme)
        {
            this.windowContainer.SetTheme(theme);
        }
    }

    public class ProjectCreationViewContent : IProjectCreationViewContent
    {
        private readonly struct SelectionViewItem : IComparable<SelectionViewItem>
        {
            private readonly IProjectTemplate template;
            private readonly string displayName;
            private readonly string? description;
            private readonly int index;
            private readonly IApplicationTheme theme;

            public SelectionViewItem(IProjectTemplate template, string displayName, string? description, int index, IApplicationTheme theme)
            {
                this.template = template;
                this.displayName = displayName;
                this.description = description;
                this.index = index;
                this.theme = theme;
            }

            public int CompareTo(SelectionViewItem other)
            {
                var result = this.index.CompareTo(other.index);

                if (result == 0)
                {
                    result = this.displayName.CompareTo(other.displayName);
                }

                return result;
            }

            public readonly void AddTo(Selector target)
            {
                target.Items.Add(new StyledComboBoxItem(this.theme) { Content = this.displayName, ToolTip = this.description }.WithValue(TemplateValueProperty, this.template));
            }
        }

        private class SelectionViewBuilder : IProjectTemplateSelectionViewBuilder
        {
            private readonly List<SelectionViewItem> items;
            private readonly IApplicationTheme theme;

            public SelectionViewBuilder(IApplicationTheme theme)
            {
                this.items = new List<SelectionViewItem>();
                this.theme = theme;
            }

            public void AddTemplate(IProjectTemplate template, string displayName, string? description, int index)
            {
                this.items.Add(new SelectionViewItem(template, displayName, description, index, this.theme));
            }

            public void AddTo(Selector target)
            {
                this.items.Sort();

                foreach (var item in this.items)
                {
                    item.AddTo(target);
                }
            }
        }

        private class ParametersViewBuilder : IProjectTemplateParametersViewBuilder
        {
            private readonly ProjectCreationViewContent view;

            public ParametersViewBuilder(ProjectCreationViewContent view)
            {
                this.view = view;
            }

            public void SetProjectName(string name)
            {
                this.view.projectNameTextBox.Text = name;
            }

            public void SetCreateDirectory(bool createDirectory)
            {
                this.view.createDirectoryCheckBox.IsChecked = createDirectory;
            }

            public void AddParameter(string replaceSource, string value)
            {
                this.view.parameters[replaceSource] = value;
            }

            public void AddParameter(string replaceSource, string displayName, string defaultValue)
            {
                var parameterTextBox = new StyledTextBox(this.view.theme);
                parameterTextBox.TextChanged += (sender, e) =>
                {
                    this.view.parameters[replaceSource] = parameterTextBox.Text;
                    this.view.InvalidatePaths();
                    this.view.ValidateValues();
                };

                parameterTextBox.Text = defaultValue;

                this.view.parametersPanel.Children.Add(new DockPanel { LastChildFill = true, Margin = Spacing }.WithChildren
                (
                    new TextBlock { Text = displayName, Width = ColumnWidth, VerticalAlignment = VerticalAlignment.Center },
                    parameterTextBox
                ));
            }
        }

        private static readonly DependencyProperty TemplateValueProperty = DependencyProperty.RegisterAttached("TemplateValue", typeof(IProjectTemplate), typeof(ProjectCreationViewContent));

        private static readonly Thickness Spacing = new Thickness(10, 5, 10, 5);

        private const int ColumnWidth = 200;

        public event EventHandler<CreateProjectRequestEventArgs>? CreateProjectRequested;
        public event EventHandler? BackButtonClicked;

        public FrameworkElement Content { get; }

        private readonly Window window;
        private readonly IApplicationTheme theme;
        private readonly IEnumerable<IProjectTemplate> templates;
        private readonly Dictionary<string, string> parameters;
        private readonly DispatcherTimer pathsPreviewTimer;
        private readonly Selector templateSelector;
        private readonly TextBox locationTextBox;
        private readonly TextBox projectNameTextBox;
        private readonly StyledCheckBox createDirectoryCheckBox;
        private readonly StyledCheckBox openDirectoryCheckBox;
        private readonly StackPanel parametersPanel;
        private readonly StackPanel pathsPanel;
        private readonly TextBlock statusTextBlock;
        private readonly StyledButton createButton;
        private readonly OpenFolderDialog openFolderDialog;
        private readonly Paragraph pathsParagraph;
        private readonly StyledRichTextBox pathsDocumentView;
        private readonly HashSet<string> existingPaths;
        private readonly bool isInitialized;
        private IProjectTemplate? selectedTemplate;
        private bool targetPathExists;
        private bool pathsValidated;
        private bool isValid;

        public ProjectCreationViewContent(Window window, IApplicationTheme theme, OpenFolderDialog openFolderDialog, IEnumerable<IProjectTemplate> templates, int selectedTemplateIndex, string location, bool openContainingFolder, bool addBackButton)
        {
            this.window = window;
            this.theme = theme;
            this.openFolderDialog = openFolderDialog;
            this.templates = templates;
            this.parameters = new Dictionary<string, string>();

            this.pathsPreviewTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5), IsEnabled = false };
            this.pathsPreviewTimer.Tick += (sender, e) =>
            {
                ValidatePaths();
                ValidateValues();
            };

            this.templateSelector = new StyledComboBox(theme) { IsEditable = false };
            this.existingPaths = new HashSet<string>();

            var builder = new SelectionViewBuilder(theme);
            foreach (var template in this.templates)
            {
                template.AddTemplateSelectionView(builder);
            }
            builder.AddTo(this.templateSelector);

            this.templateSelector.SelectedIndex = selectedTemplateIndex;
            this.templateSelector.SelectionChanged += (sender, e) => SetTemplateSelection();

            this.locationTextBox = new StyledTextBox(theme) { Text = Path.Exists(location) ? Path.GetFullPath(location) : Environment.CurrentDirectory };
            this.locationTextBox.TextChanged += (sender, e) =>
            {
                InvalidatePaths();
                ValidateValues();
            };

            this.projectNameTextBox = new StyledTextBox(theme);
            this.projectNameTextBox.TextChanged += (sender, e) =>
            {
                InvalidatePaths();
                ValidateValues();
            };

            this.createDirectoryCheckBox = new StyledCheckBox(theme)
            {
                Content = "Create Project Directory",
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(ColumnWidth + Spacing.Left, Spacing.Top, Spacing.Right, Spacing.Bottom),
            }.WithReference(Control.ForegroundProperty, theme.WindowForeground);

            this.createDirectoryCheckBox.Click += (sender, e) =>
            {
                InvalidatePaths();
                ValidateValues();
            };

            this.openDirectoryCheckBox = new StyledCheckBox(theme)
            {
                Content = "Open Project Containing Directory",
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(ColumnWidth + Spacing.Left, Spacing.Top, Spacing.Right, Spacing.Bottom),
                IsChecked = openContainingFolder,
            }.WithReference(Control.ForegroundProperty, theme.WindowForeground);

            this.pathsParagraph = new Paragraph();
            this.pathsDocumentView = new StyledRichTextBox(theme)
            {
                Document = new FlowDocument(this.pathsParagraph),
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = Spacing
            }.
            WithReference(Control.ForegroundProperty, theme.WindowForeground).
            WithReference(Control.BackgroundProperty, theme.WindowBackground).
            WithReference(TextElement.FontSizeProperty, theme.CodeFontSize).
            WithReference(TextElement.FontFamilyProperty, theme.CodeFontFamily);

            this.parametersPanel = new StackPanel();
            this.pathsPanel = new StackPanel().WithChildren
            (
                new Border { Height = 1, Margin = new Thickness(-Spacing.Left, Spacing.Top, -Spacing.Right, Spacing.Bottom) }.WithReference(Border.BackgroundProperty, theme.Separator),
                new TextBlock { Text = "Target Paths", Margin = Spacing },
                this.pathsDocumentView
            );

            this.statusTextBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = Spacing
            }.WithReference(TextElement.ForegroundProperty, theme.WarningTextForeground);

            this.createButton = new StyledButton(theme)
            {
                Content = "Create",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = Spacing,
                Padding = new Thickness(8, 2, 8, 2),
                IsEnabled = false,
            }.WithHandler(ButtonBase.ClickEvent, OnCreateClicked);

            var backButton = addBackButton ? new StyledButton(theme)
            {
                Content = "Back",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = Spacing,
                Padding = new Thickness(8, 2, 8, 2),
            }.WithHandler(ButtonBase.ClickEvent, OnBackButtonClicked) : null;

            this.Content = new DockPanel
            {
                LastChildFill = true,
            }.
            WithReference(TextElement.ForegroundProperty, theme.WindowForeground).
            WithReference(Control.BackgroundProperty, theme.WindowBackground).
            WithChildren
            (
                new DockPanel { LastChildFill = true, Margin = Spacing }.WithValue(DockPanel.DockProperty, Dock.Bottom).WithChildren
                (
                    this.createButton.WithValue(DockPanel.DockProperty, Dock.Right),
                    backButton?.WithValue(DockPanel.DockProperty, Dock.Right),
                    this.statusTextBlock
                ),
                new StyledScrollViewer(theme.ScrollBar)
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Content = new StackPanel { Margin = Spacing }.WithChildren
                    (
                        new DockPanel { LastChildFill = true, Margin = Spacing }.WithChildren
                        (
                            new TextBlock { Text = "Template", Width = ColumnWidth, VerticalAlignment = VerticalAlignment.Center },
                            this.templateSelector
                        ),
                        new DockPanel { LastChildFill = true, Margin = Spacing }.WithChildren
                        (
                            new TextBlock { Text = "Project Name", Width = ColumnWidth, VerticalAlignment = VerticalAlignment.Center },
                            this.projectNameTextBox
                        ),
                        new StackPanel().WithChildren
                        (
                            new DockPanel { LastChildFill = true, Margin = Spacing }.WithChildren
                            (
                                new TextBlock { Text = "Location", Width = ColumnWidth, VerticalAlignment = VerticalAlignment.Center }.WithDock(Dock.Left),
                                new StyledButton(theme) { Content = "...", Width = 30, Padding = new Thickness(8, 2, 8, 2), Margin = new Thickness(Spacing.Top * 2, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center }.WithDock(Dock.Right).WithHandler(ButtonBase.ClickEvent, OnBrowseClicked),
                                this.locationTextBox
                            ),
                            this.createDirectoryCheckBox.WithDock(Dock.Bottom),
                            this.openDirectoryCheckBox.WithDock(Dock.Bottom)
                        ),
                        this.parametersPanel,
                        this.pathsPanel
                    )
                }
            );

            this.Content.IsVisibleChanged += (sender, e) =>
            {
                if (this.Content.IsVisible)
                {
                    ValidatePaths();
                    this.window.Dispatcher.InvokeAsync(ValidateValues, DispatcherPriority.Render);
                }
            };

            SetTemplateSelection();

            this.isInitialized = true;
        }

        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            this.BackButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SetTemplateSelection()
        {
            this.selectedTemplate = (this.templateSelector.SelectedItem as DependencyObject)?.GetValue(TemplateValueProperty) as IProjectTemplate;

            this.parameters.Clear();
            this.parametersPanel.Children.Clear();
            this.pathsParagraph.Inlines.Clear();

            if (this.selectedTemplate != null)
            {
                var builder = new ParametersViewBuilder(this);
                this.selectedTemplate.AddTemplateParametersView(builder);
            }

            InvalidatePaths();
            ValidateValues();
        }

        private void OnCreateClicked(object sender, RoutedEventArgs e)
        {
            ValidatePaths();
            ValidateValues();

            if (this.isValid)
            {
                var selectedTemplateIndex = this.templateSelector.SelectedIndex;
                var location = this.locationTextBox.Text;
                var targetProjectName = this.projectNameTextBox.Text;
                var targetRootPath = GetTargetPath(targetProjectName);

                this.CreateProjectRequested?.Invoke(this, new CreateProjectRequestEventArgs(this.selectedTemplate!, selectedTemplateIndex, location, targetRootPath, targetProjectName, this.openDirectoryCheckBox.IsChecked == true, this.parameters));
            }
        }

        private void OnBrowseClicked(object sender, RoutedEventArgs e)
        {
            var path = Path.GetFullPath(this.locationTextBox.Text, Environment.CurrentDirectory);

            if (this.openFolderDialog.ShowDialog(this.window, path) == true)
            {
                this.locationTextBox.Text = this.openFolderDialog.FolderName;
            }
        }

        private void InvalidatePaths()
        {
            this.pathsValidated = false;
            this.existingPaths.Clear();

            this.pathsPreviewTimer.Stop();
            this.pathsPreviewTimer.Start();
        }

        private void ValidatePaths()
        {
            this.pathsPreviewTimer.Stop();

            this.pathsValidated = true;
            this.existingPaths.Clear();

            if (this.selectedTemplate != null)
            {
                var targetRootPath = GetTargetPath(this.projectNameTextBox.Text);
                this.targetPathExists = Directory.Exists(targetRootPath);

                if (this.targetPathExists)
                {
                    var relativePaths = this.selectedTemplate.GetTemplateRelativePaths(this.projectNameTextBox.Text, this.parameters).ToArray();

                    foreach (var relativePath in relativePaths)
                    {
                        var absolutePath = Path.Combine(targetRootPath, relativePath);

                        if (Path.Exists(absolutePath))
                        {
                            this.existingPaths.Add(absolutePath);
                        }
                    }
                }
            }
        }

        private void ValidateValues()
        {
            if (!this.isInitialized || this.selectedTemplate == null)
            {
                return;
            }

            this.isValid = false;
            this.createButton.IsEnabled = false;
            this.statusTextBlock.Text = null;
            this.statusTextBlock.Visibility = Visibility.Visible;

            this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            this.projectNameTextBox.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            this.pathsParagraph.Inlines.Clear();

            if (!PathExtensions.IsPathValid(this.locationTextBox.Text))
            {
                this.statusTextBlock.Text = "Location path contains invalid characters";
                this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (!Path.IsPathFullyQualified(this.locationTextBox.Text))
            {
                this.statusTextBlock.Text = "Location path is not fully qualified";
                this.locationTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (!PathExtensions.IsFileNameValid(this.projectNameTextBox.Text))
            {
                this.statusTextBlock.Text = "Project name contains invalid characters";
                this.projectNameTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            var pathsExist = false;
            var pathsInvalid = false;

            if (this.selectedTemplate != null)
            {
                var targetRootPath = GetTargetPath(this.projectNameTextBox.Text);
                var relativePaths = this.selectedTemplate.GetTemplateRelativePaths(this.projectNameTextBox.Text, this.parameters).ToArray();

                foreach (var relativePath in relativePaths)
                {
                    var absolutePath = Path.Combine(targetRootPath, relativePath);

                    if (this.existingPaths.Contains(absolutePath))
                    {
                        this.pathsParagraph.Inlines.Add(new Run($"{absolutePath} (exists)").WithContentReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground));
                        pathsExist = true;
                    }
                    else if (!PathExtensions.IsPathValid(absolutePath))
                    {
                        this.pathsParagraph.Inlines.Add(new Run($"{absolutePath} (invalid)").WithContentReference(TextElement.ForegroundProperty, this.theme.WarningTextForeground));
                        pathsInvalid = true;
                    }
                    else
                    {
                        this.pathsParagraph.Inlines.Add(new Run(absolutePath));
                    }

                    this.pathsParagraph.Inlines.Add(new LineBreak());
                }
            }

            if (this.createDirectoryCheckBox.IsChecked == true && this.pathsValidated && this.targetPathExists)
            {
                this.statusTextBlock.Text = "Project directory already exists";
                this.projectNameTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (pathsInvalid)
            {
                this.statusTextBlock.Text = "Target path contains invalid characters";
                this.projectNameTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (pathsExist)
            {
                this.statusTextBlock.Text = "Target path already exists";
                this.projectNameTextBox.SetReference(Control.ForegroundProperty, this.theme.WarningTextForeground);
                return;
            }

            if (!this.pathsValidated)
            {
                return;
            }

            this.isValid = true;
            this.createButton.IsEnabled = true;
            this.statusTextBlock.Visibility = Visibility.Collapsed;
        }

        private string GetTargetPath(string targetProjectName)
        {
            var targetRootPath = Path.GetFullPath(this.locationTextBox.Text, Environment.CurrentDirectory);

            if (this.createDirectoryCheckBox.IsChecked == true)
            {
                targetRootPath = Path.Combine(targetRootPath, targetProjectName);
            }

            return targetRootPath;
        }
    }
}
