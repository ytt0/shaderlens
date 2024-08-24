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

            public ResetButton()
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
        private readonly List<FrameworkElement> headers;

        private Panel target;
        private bool isValueChanging;
        private ISettingsValue<SrgbColor>? editedSettingsValue;
        private ColorView? editedColorViewElement;
        private FrameworkElement? editedColorResetButton;
        private readonly StyledScrollViewer scrollViewer;

        public UniformsViewBuilder(IApplication application, IProjectSettings projectSettings, IApplicationTheme theme, Transform scrollBarTransform, IClipboard clipboard, double dragSensitivity)
        {
            this.application = application;
            this.projectSettings = projectSettings;
            this.theme = theme;
            this.clipboard = clipboard;
            this.dragSensitivity = dragSensitivity;
            this.dockContainer = new DockContainer
            {
                Visibility = Visibility.Collapsed,
                Height = this.projectSettings.UniformDockHeight.Value,
            }.
            WithValue(DockPanel.DockProperty, Dock.Bottom).
            WithValue(Panel.ZIndexProperty, 1).
            WithReference(ImplicitButton.HoverBackgroundProperty, theme.ControlHoveredBackground).
            WithReference(Border.BorderBrushProperty, theme.Separator).
            WithReference(DockContainer.IconForegroundProperty, theme.IconForeground);

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

            this.colorEditor = new ColorEditor { DragSensitivity = this.dragSensitivity };
            this.colorEditor.SetReference(NumberTextBox.EditForegroundProperty, theme.TextEditForeground);
            this.colorEditor.SetReference(NumberTextBox.DragForegroundProperty, theme.TextDragForeground);
            this.colorEditor.SetReference(ImplicitButton.PressedBackgroundProperty, theme.ControlPressedBackground);
            this.colorEditor.SetReference(ImplicitButton.HoverBackgroundProperty, theme.ControlHoveredBackground);
            this.colorEditor.SetReference(DockContainer.IconForegroundProperty, theme.IconForeground);
            this.colorEditor.SetReference(ColorEditor.TextFontFamilyProperty, theme.CodeFontFamily);

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

            this.headers = new List<FrameworkElement>();
            this.target = this.uniformsPanel;
        }

        public IDisposable AddGroup(ISettingsValue<bool> expandedSettingsValue, string displayName)
        {
            var groupElement = new StackPanel();
            var groupContent = new RowsPanel { Margin = new Thickness(0, 4, 0, 0) };

            var groupHeader = new GroupHeader
            {
                IsExpanded = expandedSettingsValue.Value,
                Child = new TextBlock { Text = displayName, FontWeight = FontWeights.Bold },
            }.
            WithReference(Icon.ForegroundProperty, this.theme.IconForeground).
            WithReference(Border.BackgroundProperty, this.theme.GroupBackground).
            WithReference(ImplicitButton.HoverBackgroundProperty, this.theme.ControlHoveredBackground).
            WithReference(ImplicitButton.PressedBackgroundProperty, this.theme.ControlPressedBackground);

            groupHeader.MouseDown += (sender, e) =>
            {
                expandedSettingsValue.Value = !expandedSettingsValue.Value;

                groupHeader.IsExpanded = expandedSettingsValue.Value;
                groupContent.Visibility = expandedSettingsValue.Value ? Visibility.Visible : Visibility.Collapsed;
            };

            groupElement.Children.Add(groupHeader);
            groupElement.Children.Add(groupContent);

            groupContent.Visibility = expandedSettingsValue.Value ? Visibility.Visible : Visibility.Collapsed;

            this.target.Children.Add(groupElement);

            var previousTarget = this.target;
            this.target = groupContent;

            return new DisposableAction(() => this.target = previousTarget);
        }

        public void AddBoolElement(ISettingsValue<bool> settingsValue, string displayName)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);
            var valueCheckBox = new StyledCheckBox(this.theme) { IsChecked = settingsValue.Value, HorizontalAlignment = HorizontalAlignment.Stretch };

            void ValueChanged(object sender, RoutedEventArgs e)
            {
                settingsValue.Value = valueCheckBox.IsChecked == true;
                resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                this.application.SetProjectChanged();
            }

            void ResetValue()
            {
                ClearTextBoxKeyboardFocus();
                settingsValue.ResetValue();
                valueCheckBox.IsChecked = settingsValue.Value;
                resetButton.Visibility = Visibility.Collapsed;
                this.application.SetProjectChanged();
            }

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetValue();
                e.Handled = true;
            };

            valueCheckBox.Click += ValueChanged;

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);
            var uniformValueElement = new ColumnPanel().WithChildren(valueCheckBox);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, uniformValueElement);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetValue();
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);
        }

        public void AddIntElement(ISettingsValue<int> settingsValue, string displayName, int minValue, int maxValue, int step)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);
            var valueTextBox = CreateNumberTextBox(settingsValue.Value, minValue, maxValue, step);

            void ValueChanged(object sender, RoutedEventArgs e)
            {
                settingsValue.Value = (int)Math.Round(valueTextBox.Value);
                resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                this.application.SetProjectChanged();
            }

            void ResetValue()
            {
                ClearTextBoxKeyboardFocus();
                settingsValue.ResetValue();
                valueTextBox.Value = settingsValue.Value;
                resetButton.Visibility = Visibility.Collapsed;
                this.application.SetProjectChanged();
            };

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetValue();
                e.Handled = true;
            };

            valueTextBox.ValueChanged += ValueChanged;

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);
            var uniformValueElement = new ColumnPanel().WithChildren(valueTextBox);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, uniformValueElement);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetValue();
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);
        }

        public void AddFloatElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);
            var valueTextBox = CreateNumberTextBox(settingsValue.Value, minValue, maxValue, step);

            void ValueChanged(object sender, RoutedEventArgs e)
            {
                if (!this.isValueChanging)
                {
                    settingsValue.Value = valueTextBox.Value;
                    resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                    this.application.SetProjectChanged();
                }
            }

            void ResetValue()
            {
                this.isValueChanging = true;
                try
                {
                    ClearTextBoxKeyboardFocus();
                    settingsValue.ResetValue();
                    valueTextBox.Value = settingsValue.Value;
                    resetButton.Visibility = Visibility.Collapsed;
                    this.application.SetProjectChanged();
                }
                finally
                {
                    this.isValueChanging = false;
                }
            };

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetValue();
                e.Handled = true;
            };

            valueTextBox.ValueChanged += ValueChanged;

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);
            var uniformValueElement = new ColumnPanel().WithChildren(valueTextBox);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, uniformValueElement);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetValue();
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);
        }

        public void AddVec2Element(ISettingsValue<Vec2> settingsValue, string displayName, Vec2 minValue, Vec2 maxValue, Vec2 step)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);
            var value1TextBox = CreateNumberTextBox(settingsValue.Value.X, minValue.X, maxValue.X, step.X);
            var value2TextBox = CreateNumberTextBox(settingsValue.Value.Y, minValue.Y, maxValue.Y, step.Y);

            var isValueChanging = false;

            void ValueChanged(object sender, RoutedEventArgs e)
            {
                if (!isValueChanging)
                {
                    settingsValue.Value = new Vec2(value1TextBox.Value, value2TextBox.Value);
                    resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                    this.application.SetProjectChanged();
                }
            };

            void ResetValue()
            {
                isValueChanging = true;
                try
                {
                    ClearTextBoxKeyboardFocus();
                    settingsValue.ResetValue();
                    value1TextBox.Value = settingsValue.Value.X;
                    value2TextBox.Value = settingsValue.Value.Y;
                    resetButton.Visibility = Visibility.Collapsed;
                    this.application.SetProjectChanged();
                }
                finally
                {
                    isValueChanging = false;
                }
            };

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetValue();
                e.Handled = true;
            };

            value1TextBox.ValueChanged += ValueChanged;
            value2TextBox.ValueChanged += ValueChanged;

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);
            var uniformValueElement = new ColumnPanel().WithChildren(value1TextBox, value2TextBox);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, uniformValueElement);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetValue();
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);
        }

        public void AddVec3Element(ISettingsValue<Vec3> settingsValue, string displayName, Vec3 minValue, Vec3 maxValue, Vec3 step)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);
            var value1TextBox = CreateNumberTextBox(settingsValue.Value.X, minValue.X, maxValue.X, step.X);
            var value2TextBox = CreateNumberTextBox(settingsValue.Value.Y, minValue.Y, maxValue.Y, step.Y);
            var value3TextBox = CreateNumberTextBox(settingsValue.Value.Z, minValue.Z, maxValue.Z, step.Z);

            var isValueChanging = false;

            void ValueChanged(object sender, RoutedEventArgs e)
            {
                if (!isValueChanging)
                {
                    settingsValue.Value = new Vec3(value1TextBox.Value, value2TextBox.Value, value3TextBox.Value);
                    resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                    this.application.SetProjectChanged();
                }
            }

            void ResetValue()
            {
                isValueChanging = true;
                try
                {
                    ClearTextBoxKeyboardFocus();
                    settingsValue.ResetValue();
                    value1TextBox.Value = settingsValue.Value.X;
                    value2TextBox.Value = settingsValue.Value.Y;
                    value3TextBox.Value = settingsValue.Value.Z;
                    resetButton.Visibility = Visibility.Collapsed;
                    this.application.SetProjectChanged();
                }
                finally
                {
                    isValueChanging = false;
                }
            }

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetValue();
                e.Handled = true;
            };

            value1TextBox.ValueChanged += ValueChanged;
            value2TextBox.ValueChanged += ValueChanged;
            value3TextBox.ValueChanged += ValueChanged;

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);
            var uniformValueElement = new ColumnPanel().WithChildren(value1TextBox, value2TextBox, value3TextBox);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, uniformValueElement);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetValue();
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);
        }

        public void AddVec4Element(ISettingsValue<Vec4> settingsValue, string displayName, Vec4 minValue, Vec4 maxValue, Vec4 step)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);
            var value1TextBox = CreateNumberTextBox(settingsValue.Value.X, minValue.X, maxValue.X, step.X);
            var value2TextBox = CreateNumberTextBox(settingsValue.Value.Y, minValue.Y, maxValue.Y, step.Y);
            var value3TextBox = CreateNumberTextBox(settingsValue.Value.Z, minValue.Z, maxValue.Z, step.Z);
            var value4TextBox = CreateNumberTextBox(settingsValue.Value.W, minValue.W, maxValue.W, step.W);

            var isValueChanging = false;

            void ValueChanged(object sender, RoutedEventArgs e)
            {
                if (!isValueChanging)
                {
                    settingsValue.Value = new Vec4(value1TextBox.Value, value2TextBox.Value, value3TextBox.Value, value4TextBox.Value);
                    resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                    this.application.SetProjectChanged();
                }
            }

            void ResetValue()
            {
                isValueChanging = true;
                try
                {
                    ClearTextBoxKeyboardFocus();
                    settingsValue.ResetValue();
                    value1TextBox.Value = settingsValue.Value.X;
                    value2TextBox.Value = settingsValue.Value.Y;
                    value3TextBox.Value = settingsValue.Value.Z;
                    value4TextBox.Value = settingsValue.Value.W;
                    resetButton.Visibility = Visibility.Collapsed;
                    this.application.SetProjectChanged();
                }
                finally
                {
                    isValueChanging = false;
                }
            };

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetValue();
                e.Handled = true;
            };

            value1TextBox.ValueChanged += ValueChanged;
            value2TextBox.ValueChanged += ValueChanged;
            value3TextBox.ValueChanged += ValueChanged;
            value4TextBox.ValueChanged += ValueChanged;

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);
            var uniformValueElement = new ColumnPanel().WithChildren(value1TextBox, value2TextBox, value3TextBox, value4TextBox);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, uniformValueElement);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetValue();
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);
        }

        public void AddColorElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, string name, string displayName)
        {
            var resetButton = CreateResetButton(!settingsValue.IsDefaultValue);

            var colorViewUnderline = new Border
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
            }.WithReference(Border.BackgroundProperty, this.theme.TextProgressTrack).WithValue(Grid.ColumnSpanProperty, 2);

            var alphaBackgroundElement = new AlphaBackgroundView().WithValue(Grid.ColumnProperty, 1);

            var colorViewElement = new ColorView { Color = settingsValue.Value.ToColor() }.WithValue(Grid.ColumnSpanProperty, 2);

            var colorViewPanel = new Grid().WithChildren(alphaBackgroundElement, colorViewElement, colorViewUnderline);
            colorViewPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            colorViewPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            ClickBehavior.Register(colorViewPanel, (sender, e) =>
            {
                ToggleColorEdit(settingsValue, editAlpha, colorViewElement, resetButton);
                e.Handled = true;
            });

            colorViewPanel.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.C && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
                {
                    this.clipboard.SetColor(settingsValue.Value);
                    e.Handled = true;
                }

                if (e.Key == Key.V && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
                {
                    if (this.clipboard.TryGetColor(System.Windows.Clipboard.GetText(), out var srgbColor))
                    {
                        if (!editAlpha)
                        {
                            srgbColor.A = 1.0;
                        }

                        SetColor(settingsValue, colorViewElement, resetButton, srgbColor);
                    }

                    e.Handled = true;
                }
            };

            resetButton.PreviewMouseDown += (sender, e) =>
            {
                ResetColor(settingsValue, colorViewElement, resetButton);
                e.Handled = true;
            };

            var uniformHeaderElement = CreateUniformHeaderElement(displayName, resetButton);

            var uniformRowElement = CreateUniformRowElement(uniformHeaderElement, colorViewPanel);
            uniformRowElement.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && uniformRowElement.IsMouseOver)
                {
                    ResetColor(settingsValue, colorViewElement, resetButton);
                    e.Handled = true;
                }
            };

            this.headers.Add(uniformHeaderElement);
            this.target.Children.Add(uniformRowElement);

            if (this.projectSettings.EditedColor.Value == name)
            {
                ToggleColorEdit(settingsValue, editAlpha, colorViewElement, resetButton);
            }
        }

        public void SetSettingsState()
        {
            this.scrollViewer.ScrollToVerticalOffset(this.projectSettings.UniformScroll.Value);
        }

        private void SetColor(ISettingsValue<SrgbColor> settingsValue, ColorView colorViewElement, FrameworkElement resetButton, SrgbColor color)
        {
            this.isValueChanging = true;
            try
            {
                settingsValue.Value = color;
                resetButton.Visibility = settingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                colorViewElement.Color = color.ToColor();

                if (this.editedSettingsValue == settingsValue)
                {
                    var linearRgb = settingsValue.Value.ToLinearRgb();

                    this.colorEditor.Color = OkhsvColor.FromLinearRgb(linearRgb);
                    this.colorEditor.Alpha = linearRgb.A;
                }

                this.application.SetProjectChanged();
            }
            finally
            {
                this.isValueChanging = false;
            }
        }

        private void ResetColor(ISettingsValue<SrgbColor> settingsValue, ColorView colorViewElement, FrameworkElement resetButton)
        {
            this.isValueChanging = true;
            try
            {
                ClearTextBoxKeyboardFocus();
                settingsValue.ResetValue();
                resetButton.Visibility = Visibility.Collapsed;
                colorViewElement.Color = settingsValue.Value.ToColor();

                if (this.editedSettingsValue == settingsValue)
                {
                    this.colorEditor.Color = OkhsvColor.FromLinearRgb(settingsValue.Value.ToLinearRgb());
                    this.colorEditor.Alpha = settingsValue.Value.A;
                }

                this.application.SetProjectChanged();
            }
            finally
            {
                this.isValueChanging = false;
            }
        }

        private void OnColorEdited()
        {
            if (!this.isValueChanging && this.editedSettingsValue is not null)
            {
                var color = this.colorEditor.Color.ToLinearRgb().Round(0.001).ToSrgb();
                color.A = this.colorEditor.Alpha;

                this.editedSettingsValue.Value = color;
                this.editedColorViewElement!.Color = color.ToColor();
                this.editedColorResetButton!.Visibility = this.editedSettingsValue.IsDefaultValue ? Visibility.Collapsed : Visibility.Visible;
                this.application.SetProjectChanged();
            }
        }

        private void ToggleColorEdit(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, ColorView colorViewElement, FrameworkElement resetButton)
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
                this.editedColorViewElement = colorViewElement;
                this.editedColorResetButton = resetButton;
                this.colorEditor.Color = hsv;
                this.colorEditor.SourceColor = hsv;
                this.colorEditor.Alpha = alpha;
                this.colorEditor.SourceAlpha = alpha;
                this.colorEditor.IsAlphaVisible = editAlpha;
                this.colorEditor.ResetLastColors();

                this.dockContainer.Child = this.colorEditor;
                this.dockContainer.Visibility = Visibility.Visible;

                this.projectSettings.EditedColor.Value = settingsValue.Name;

                colorViewElement.BringIntoView();
            }
            finally
            {
                this.isValueChanging = false;
            }
        }

        private void SetColumnSize()
        {
            var width = this.target.ActualWidth * this.columnSplit.Ratio;
            foreach (var header in this.headers)
            {
                header.Width = width;
            }
        }

        private NumberTextBox CreateNumberTextBox(double value, double minValue, double maxValue, double step)
        {
            return new NumberTextBox
            {
                MinValue = minValue,
                MaxValue = maxValue,
                StepSize = step,
                Value = value,
                DragSensitivity = this.dragSensitivity,
                RequireScrollModifierKey = true,
                HorizontalAlignment = HorizontalAlignment.Stretch
            }.
            WithReference(NumberTextBox.ProgressTrackBrushProperty, this.theme.TextProgressTrack).
            WithReference(NumberTextBox.EditForegroundProperty, this.theme.TextEditForeground).
            WithReference(NumberTextBox.DragForegroundProperty, this.theme.TextDragForeground).
            WithReference(NumberTextBox.HoverBackgroundProperty, this.theme.ControlHoveredBackground).
            WithReference(NumberTextBox.PressedBackgroundProperty, this.theme.ControlPressedBackground).
            WithReference(TextElement.FontFamilyProperty, this.theme.CodeFontFamily);
        }

        private ResetButton CreateResetButton(bool isVisible)
        {
            return new ResetButton
            {
                Margin = new Thickness(4, -8, 4, -8),
                Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed,
            }.
            WithReference(ResetButton.ForegroundProperty, this.theme.IconForeground).
            WithReference(ImplicitButton.HoverBackgroundProperty, this.theme.ControlHoveredBackground);
        }

        private static DockPanel CreateUniformHeaderElement(string displayName, FrameworkElement resetButton)
        {
            return new DockPanel { LastChildFill = true }.WithChildren
            (
                resetButton.WithDock(Dock.Right),
                new TextBlock { Text = displayName, TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center }
            );
        }

        private static DockPanel CreateUniformRowElement(FrameworkElement headerElement, FrameworkElement valueElement)
        {
            return new DockPanel { LastChildFill = true, Focusable = false, Margin = new Thickness(4, 0, 4, 0) }.WithChildren
            (
                headerElement,
                new FrameworkElement { Width = 4 },
                valueElement
            );
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
