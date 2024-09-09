namespace Shaderlens.Views
{
    public class UniformsViewBuilder : IUniformsViewBuilder
    {
        private class DisposableAction : IDisposable
        {
            private readonly Action action;

            public DisposableAction(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                this.action();
            }
        }

        public FrameworkElement ViewElement { get { return this.dockPanel; } }

        private readonly IApplication application;
        private readonly IProjectSettings projectSettings;
        private readonly IApplicationTheme theme;
        private readonly IClipboard clipboard;
        private readonly double dragSensitivity;
        private readonly DockContainer dockContainer;
        private readonly Grid dockContainerPanel;
        private readonly Panel uniformsPanel;
        private readonly ColumnSplitContainer columnSplit;
        private readonly DockPanel dockPanel;
        private readonly List<FrameworkElement> headerElements;
        private readonly List<IRowHeaderContainer> rowHeaderContainers;
        private readonly StyledScrollViewer scrollViewer;

        private Panel targetPanel;

        private ColorUniformElement? colorUniformElement;
        private ColorDockUniformElement? colorDockUniformElement;
        private ISettingsValue<SrgbColor>? colorSettingsValue;

        public UniformsViewBuilder(IApplication application, IProjectSettings projectSettings, IApplicationTheme theme, Transform scrollBarTransform, IClipboard clipboard, double dragSensitivity)
        {
            this.application = application;
            this.projectSettings = projectSettings;
            this.theme = theme;
            this.clipboard = clipboard;
            this.dragSensitivity = dragSensitivity;
            this.dockContainerPanel = new Grid();
            this.dockContainer = new DockContainer(theme)
            {
                Visibility = Visibility.Collapsed,
                Height = this.projectSettings.UniformDockHeight.Value,
                Child = this.dockContainerPanel,
            }.
            WithValue(DockPanel.DockProperty, Dock.Bottom).
            WithValue(Panel.ZIndexProperty, 1);

            this.dockContainer.Closed += (sender, e) =>
            {
                this.projectSettings.UniformDockName.Value = null;
            };

            this.dockContainer.SizeChanged += (sender, e) =>
            {
                this.projectSettings.UniformDockHeight.Value = (int)this.dockContainer.Height;
            };

            this.uniformsPanel = new RowsPanel { Background = Brushes.Transparent };
            this.uniformsPanel.SizeChanged += (sender, e) => SetColumnSize();

            MultiNumberTextBoxEditBehavior.Register(this.uniformsPanel);
            MultiToggleButtonClickBehavior.Register(this.uniformsPanel);
            MultiButtonClickBehavior.Register(this.uniformsPanel);

            this.columnSplit = new ColumnSplitContainer { Child = this.uniformsPanel, Ratio = this.projectSettings.UniformColumnRatio.Value };
            this.columnSplit.SetReference(ColumnSplitContainer.HoverBrushProperty, theme.ControlHoveredBackground);
            this.columnSplit.RatioChanged += (sender, e) =>
            {
                this.projectSettings.UniformColumnRatio.Value = this.columnSplit.Ratio;
                SetColumnSize();
            };

            this.scrollViewer = new StyledScrollViewer(theme.ScrollBar)
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                ScrollBarTransform = scrollBarTransform,
                Content = new Border
                {
                    Child = this.columnSplit,
                    Padding = new Thickness(8, 0, 8, 0),
                }
            };
            this.scrollViewer.ScrollChanged += (sender, e) => projectSettings.UniformScroll.Value = (int)this.scrollViewer.VerticalOffset;

            this.dockPanel = new DockPanel { LastChildFill = true, Background = Brushes.Transparent };
            this.dockPanel.Children.Add(this.dockContainer);
            this.dockPanel.Children.Add(this.scrollViewer);

            this.headerElements = new List<FrameworkElement>();
            this.rowHeaderContainers = new List<IRowHeaderContainer>();
            this.targetPanel = this.uniformsPanel;
        }

        public IDisposable AddGroup(ISettingsValue<bool> expandedSettingsValue, string displayName)
        {
            var groupElement = new UniformGroupElement(expandedSettingsValue, displayName, this.theme);
            this.targetPanel.Children.Add(groupElement);

            var previousTargetPanel = this.targetPanel;
            this.targetPanel = groupElement.Content;

            return new DisposableAction(() => this.targetPanel = previousTargetPanel);
        }

        public void AddBoolElement(ISettingsValue<bool> settingsValue, string displayName)
        {
            var element = new BoolUniformElement(settingsValue, displayName, this.clipboard, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.targetPanel.Children.Add(element);
        }

        public void AddFloatElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step, int roundDecimals)
        {
            var element = new FloatUniformElement(settingsValue, displayName, minValue, maxValue, step, roundDecimals, this.dragSensitivity, this.clipboard, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.targetPanel.Children.Add(element);
        }

        public void AddVectorElement(ISettingsValue<Vector<bool>> settingsValue, string displayName)
        {
            var element = new BoolVectorUniformElement(settingsValue, displayName, this.clipboard, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.targetPanel.Children.Add(element);
        }

        public void AddVectorElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, int roundDecimals)
        {
            var element = new VectorUniformElement(settingsValue, displayName, minValue, maxValue, step, roundDecimals, this.dragSensitivity, this.clipboard, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.targetPanel.Children.Add(element);
        }

        public void AddColorElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, string name, string displayName)
        {
            var element = new ColorUniformElement(settingsValue, displayName, this.clipboard, this.theme) { Height = 28 };
            element.ValueChanged += (sender, e) =>
            {
                if (this.colorDockUniformElement != null && this.colorSettingsValue == settingsValue)
                {
                    this.colorDockUniformElement.InvalidateValue();
                }

                this.application.SetProjectChanged();
                this.application.RenderFrame();
            };

            element.Click += (sender, e) =>
            {
                ToggleDockElement(settingsValue, editAlpha, element);
                e.Handled = true;
            };

            this.rowHeaderContainers.Add(element);
            this.targetPanel.Children.Add(element);

            if (this.projectSettings.UniformDockName.Value == name)
            {
                ToggleDockElement(settingsValue, editAlpha, element);
            }
        }

        public void SetSettingsState()
        {
            this.scrollViewer.ScrollToVerticalOffset(this.projectSettings.UniformScroll.Value);
        }

        private void OnUniformElementValueChanged(object sender, RoutedEventArgs e)
        {
            this.application.SetProjectChanged();
            this.application.RenderFrame();
        }

        private void SetColumnSize()
        {
            var width = this.targetPanel.ActualWidth * this.columnSplit.Ratio;
            foreach (var header in this.headerElements)
            {
                header.Width = width;
            }

            foreach (var header in this.rowHeaderContainers)
            {
                header.HeaderWidth = width;
            }
        }

        public void ToggleDockElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, ColorUniformElement element)
        {
            if (this.colorSettingsValue == settingsValue && this.dockContainer.Visibility != Visibility.Collapsed)
            {
                this.dockContainer.Visibility = Visibility.Collapsed;
                this.projectSettings.UniformDockName.Value = null;
                return;
            }

            this.colorSettingsValue = settingsValue;

            if (this.colorDockUniformElement == null)
            {
                this.colorDockUniformElement = CreateColorDockUniformElement();
                this.dockContainerPanel.Children.Add(this.colorDockUniformElement);
            }

            this.colorUniformElement = element;
            this.colorUniformElement.BringIntoView();
            this.colorDockUniformElement.SetValue(settingsValue, editAlpha);
            this.projectSettings.UniformDockName.Value = settingsValue.Name;

            SetDockElementsVisibility(this.colorDockUniformElement);
            this.dockContainer.Visibility = Visibility.Visible;
        }

        private ColorDockUniformElement CreateColorDockUniformElement()
        {
            return new ColorDockUniformElement(this.clipboard, this.theme, this.dragSensitivity).WithHandler(ColorDockUniformElement.ValueChangedEvent, (sender, e) =>
            {
                if (this.colorUniformElement != null)
                {
                    this.colorUniformElement.BringIntoView();
                    this.colorUniformElement.InvalidateValue();
                    this.application.SetProjectChanged();
                    this.application.RenderFrame();
                }
            });
        }

        private void SetDockElementsVisibility(FrameworkElement visibleElement)
        {
            foreach (var child in this.dockContainerPanel.Children.OfType<FrameworkElement>())
            {
                child.Visibility = child == visibleElement ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
