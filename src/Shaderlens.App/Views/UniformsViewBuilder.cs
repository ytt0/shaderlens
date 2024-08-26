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

        private class ResetButton : ImplicitButton
        {
            private const double DrawingSize = 24;
            private static readonly Geometry ResetGeometry = Geometry.Parse("M14.394 5.422a7 7 0 0 1 4.44 8.093 7 7 0 0 1-7.444 5.458 7 7 0 0 1-6.383-6.668 7 7 0 0 1 5.777-7.199M14 10V5h5").WithFreeze();

            public static readonly DependencyProperty ForegroundProperty = Icon.ForegroundProperty.AddOwner(typeof(ResetButton), new FrameworkPropertyMetadata((sender, e) => ((ResetButton)sender).pen.Brush = (Brush)e.NewValue));
            public Brush Foreground
            {
                get { return (Brush)GetValue(ForegroundProperty); }
                set { SetValue(ForegroundProperty, value); }
            }

            private readonly Pen pen;
            private readonly Geometry geometry;

            public ResetButton(IApplicationTheme theme) :
                base(theme)
            {
                this.pen = new Pen(this.Foreground, 1.25) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
                this.geometry = ResetGeometry;
                this.Width = DrawingSize;
                this.Height = DrawingSize;
                this.CornerRadius = new CornerRadius(4);
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                drawingContext.DrawGeometry(null, this.pen, this.geometry);
            }
        }

        public FrameworkElement ViewElement { get { return this.dockPanel; } }

        private readonly IApplication application;
        private readonly IProjectSettings projectSettings;
        private readonly IApplicationTheme theme;
        private readonly IClipboard clipboard;
        private readonly double dragSensitivity;
        private readonly DockContainer dockContainer;
        private readonly Panel uniformsPanel;
        private readonly ColumnSplitContainer columnSplit;
        private readonly ColorEditor colorEditor;
        private readonly DockPanel dockPanel;
        private readonly List<FrameworkElement> headerElements;
        private readonly List<IRowHeaderContainer> rowHeaderContainers;
        private Panel target;
        private bool isValueChanging;
        private ISettingsValue<SrgbColor>? editedSettingsValue;
        private ColorUniformElement? editedColorUniformElement;
        private readonly StyledScrollViewer scrollViewer;

        public UniformsViewBuilder(IApplication application, IProjectSettings projectSettings, IApplicationTheme theme, Transform scrollBarTransform, IClipboard clipboard, double dragSensitivity)
        {
            this.application = application;
            this.projectSettings = projectSettings;
            this.theme = theme;
            this.clipboard = clipboard;
            this.dragSensitivity = dragSensitivity;
            this.dockContainer = new DockContainer(theme)
            {
                Visibility = Visibility.Collapsed,
                Height = this.projectSettings.UniformDockHeight.Value,
            }.
            WithValue(DockPanel.DockProperty, Dock.Bottom).
            WithValue(Panel.ZIndexProperty, 1);

            this.dockContainer.Closed += (sender, e) =>
            {
                this.projectSettings.EditedColor.Value = null;
            };

            this.dockContainer.SizeChanged += (sender, e) =>
            {
                this.projectSettings.UniformDockHeight.Value = (int)this.dockContainer.Height;
            };

            this.uniformsPanel = new RowsPanel { Background = Brushes.Transparent };
            this.uniformsPanel.SizeChanged += (sender, e) => SetColumnSize();

            this.columnSplit = new ColumnSplitContainer { Child = this.uniformsPanel, Ratio = this.projectSettings.UniformColumnRatio.Value };
            this.columnSplit.SetReference(ColumnSplitContainer.HoverBrushProperty, theme.ControlHoveredBackground);
            this.columnSplit.RatioChanged += (sender, e) =>
            {
                this.projectSettings.UniformColumnRatio.Value = this.columnSplit.Ratio;
                SetColumnSize();
            };

            this.colorEditor = new ColorEditor(this.theme) { DragSensitivity = this.dragSensitivity };

            this.colorEditor.ColorChanged += (sender, e) => OnColorEdited();
            this.colorEditor.AlphaChanged += (sender, e) => OnColorEdited();

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
            this.target = this.uniformsPanel;
        }

        public IDisposable AddGroup(ISettingsValue<bool> expandedSettingsValue, string displayName)
        {
            var groupElement = new UniformGroupElement(expandedSettingsValue, displayName, this.theme);
            this.target.Children.Add(groupElement);

            var previousTarget = this.target;
            this.target = groupElement.Content;

            return new DisposableAction(() => this.target = previousTarget);
        }

        public void AddBoolElement(ISettingsValue<bool> settingsValue, string displayName)
        {
            var element = new BoolUniformElement(settingsValue, displayName, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.target.Children.Add(element);
        }

        public void AddFloatElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step)
        {
            var element = new FloatUniformElement(settingsValue, displayName, minValue, maxValue, step, this.dragSensitivity, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.target.Children.Add(element);
        }

        public void AddVectorElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step)
        {
            var element = new VectorUniformElement(settingsValue, displayName, minValue, maxValue, step, this.dragSensitivity, this.theme);
            element.ValueChanged += OnUniformElementValueChanged;

            this.rowHeaderContainers.Add(element);
            this.target.Children.Add(element);
        }

        public void AddColorElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, string name, string displayName)
        {
            var element = new ColorUniformElement(settingsValue, displayName, this.theme);
            element.ValueChanged += (sender, e) =>
            {
                if (this.editedSettingsValue == settingsValue && !this.isValueChanging)
                {
                    this.isValueChanging = true;
                    try
                    {
                        this.colorEditor.Color = OkhsvColor.FromLinearRgb(settingsValue.Value.ToLinearRgb());
                        ClearTextBoxKeyboardFocus();
                        this.application.SetProjectChanged();
                    }
                    finally
                    {
                        this.isValueChanging = false;
                    }
                }
            };

            element.Click += (sender, e) =>
            {
                ToggleColorEdit(settingsValue, editAlpha, element);
                e.Handled = true;
            };

            this.rowHeaderContainers.Add(element);
            this.target.Children.Add(element);

            if (this.projectSettings.EditedColor.Value == name)
            {
                ToggleColorEdit(settingsValue, editAlpha, element);
            }
        }

        public void SetSettingsState()
        {
            this.scrollViewer.ScrollToVerticalOffset(this.projectSettings.UniformScroll.Value);
        }

        private void OnUniformElementValueChanged(object sender, RoutedEventArgs e)
        {
            ClearTextBoxKeyboardFocus();
            this.application.SetProjectChanged();
        }

        private void OnColorEdited()
        {
            if (!this.isValueChanging && this.editedSettingsValue != null && this.editedColorUniformElement != null)
            {
                this.isValueChanging = true;
                try
                {
                    var color = this.colorEditor.Color.ToLinearRgb().Round(0.001).ToSrgb();
                    color.A = this.colorEditor.Alpha;

                    this.editedSettingsValue.Value = color;
                    this.editedColorUniformElement.InvalidateValue();
                    this.application.SetProjectChanged();
                }
                finally
                {
                    this.isValueChanging = false;
                }
            }
        }

        private void ToggleColorEdit(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, ColorUniformElement colorUniformElement)
        {
            if (this.editedSettingsValue == settingsValue && this.dockContainer.Visibility == Visibility.Visible)
            {
                this.editedSettingsValue = null;
                this.dockContainer.Visibility = Visibility.Collapsed;
                this.projectSettings.EditedColor.Value = null;
                return;
            }

            this.isValueChanging = true;
            try
            {
                var hsv = OkhsvColor.FromLinearRgb(settingsValue.Value.ToLinearRgb());
                var alpha = settingsValue.Value.A;

                this.editedSettingsValue = settingsValue;
                this.editedColorUniformElement = colorUniformElement;
                this.colorEditor.Color = hsv;
                this.colorEditor.SourceColor = hsv;
                this.colorEditor.Alpha = alpha;
                this.colorEditor.SourceAlpha = alpha;
                this.colorEditor.IsAlphaVisible = editAlpha;
                this.colorEditor.ResetLastColors();

                this.dockContainer.Child = this.colorEditor;
                this.dockContainer.Visibility = Visibility.Visible;

                this.projectSettings.EditedColor.Value = settingsValue.Name;

                colorUniformElement.BringIntoView();
            }
            finally
            {
                this.isValueChanging = false;
            }
        }

        private void SetColumnSize()
        {
            var width = this.target.ActualWidth * this.columnSplit.Ratio;
            foreach (var header in this.headerElements)
            {
                header.Width = width;
            }

            foreach (var header in this.rowHeaderContainers)
            {
                header.HeaderWidth = width;
            }
        }

        private static void ClearTextBoxKeyboardFocus()
        {
            if (Keyboard.FocusedElement is TextBox textBox)
            {
                var scope = FocusManager.GetFocusScope(textBox);
                FocusManager.SetFocusedElement(scope, textBox.GetAncestor<FrameworkElement>());
            }
        }
    }
}
