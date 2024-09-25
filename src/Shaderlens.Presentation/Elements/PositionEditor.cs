namespace Shaderlens.Presentation.Elements
{
    public enum PositionEditMode { XY, XZ, YZ }

    public class PositionEditor : FrameworkElement
    {
        private static readonly Geometry FocusViewGeometry = Geometry.Parse("M20.5 17h-17M7 3.498V20.5m8-8.5c.16 2.796-3.913 4.12-5.427 1.763-1.977-2.408 1.25-5.949 3.821-4.42A3.009 3.009 0 0 1 15 12Z").WithFreeze();
        private static readonly Geometry ResetViewGeometry = Geometry.Parse("M20.5 12h-17M12 3.5v17M20 7a3 3 0 0 1-3 3 3 3 0 0 1-3-3 3 3 0 0 1 3-3 3 3 0 0 1 3 3z").WithFreeze();
        private static readonly Brush AxisXBrush = new SolidColorBrush(Color.FromRgb(255, 20, 20));
        private static readonly Brush AxisYBrush = new SolidColorBrush(Color.FromRgb(20, 200, 20));
        private static readonly Brush AxisZBrush = new SolidColorBrush(Color.FromRgb(20, 120, 255));
        private const int ButtonsMagin = 4;
        private const int MaxComponents = 3;
        private const int NormalDecimals = 3;

        protected override int VisualChildrenCount { get { return this.visualChildren.Length; } }

        private bool is2D;
        public bool Is2D
        {
            get { return this.is2D; }
            set
            {
                this.is2D = value;
                this.valuesTextBox[2].Visibility = this.is2D ? Visibility.Collapsed : Visibility.Visible;
                this.positionGraph.NormalizeValue = this.is2D && this.NormalizeValue;
                InvalidateArrange();
            }
        }

        public event EventHandler? EditModeChanged;
        private PositionEditMode editMode;
        public PositionEditMode EditMode
        {
            get { return this.editMode; }
            set
            {
                if (this.editMode != value)
                {
                    this.editMode = value;
                    this.EditModeChanged?.Invoke(this, EventArgs.Empty);

                    if (!this.skipChangeEvent)
                    {
                        try
                        {
                            this.skipChangeEvent = true;

                            SetGraphEditMode();
                        }
                        finally
                        {
                            this.skipChangeEvent = false;
                        }
                    }
                }
            }
        }

        public event EventHandler? ValueChanged;
        private Vector<double> value;
        public Vector<double> Value
        {
            get { return this.value; }
            set
            {
                if (this.value.Equals(value))
                {
                    return;
                }

                this.value = GetFixedSizeVector(value);
                this.lastValueSign = new Vector<double>(this.value.Select((v, i) => v != 0.0 ? Math.Sign(v) : this.lastValueSign[i]).ToArray());

                this.ValueChanged?.Invoke(this, EventArgs.Empty);

                if (!this.skipChangeEvent)
                {
                    try
                    {
                        this.skipChangeEvent = true;

                        SetTextBoxValues(this.Value);
                        SetGraphEditMode();
                    }
                    finally
                    {
                        this.skipChangeEvent = false;
                    }
                }
            }
        }

        private Vector<double> sourceValue;
        public Vector<double> SourceValue
        {
            get { return this.sourceValue; }
            set
            {
                this.sourceValue = GetFixedSizeVector(value);
                if (!this.skipChangeEvent)
                {
                    try
                    {
                        this.skipChangeEvent = true;

                        SetGraphEditMode();
                    }
                    finally
                    {
                        this.skipChangeEvent = false;
                    }
                }
            }
        }

        public event EventHandler? OffsetChanged;
        private Vector<double> offset;
        public Vector<double> Offset
        {
            get { return this.offset; }
            set
            {
                this.offset = GetFixedSizeVector(value);
                this.positionGraph.Offset = ToGraphValue(this.offset, this.EditMode);
            }
        }

        public event EventHandler? ScaleChanged;
        public double Scale
        {
            get { return this.positionGraph.Scale; }
            set { this.positionGraph.Scale = value; }
        }

        private bool normalizeValue;
        public bool NormalizeValue
        {
            get { return this.normalizeValue; }
            set
            {
                this.normalizeValue = value;
                this.positionGraph.NormalBounds = value;
                this.positionGraph.NormalizeValue = this.is2D && this.normalizeValue;
                SetTextBoxValues(this.Value);
            }
        }

        private Vector<double> minValue;
        public Vector<double> MinValue
        {
            get { return this.minValue; }
            set
            {
                this.minValue = GetFixedSizeVector(value);

                for (var i = 0; i < MaxComponents; i++)
                {
                    this.valuesTextBox[i].MinValue = this.minValue[i];
                }

                this.positionGraph.MinValue = ToGraphValue(this.minValue, this.EditMode);
            }
        }

        private Vector<double> maxValue;
        public Vector<double> MaxValue
        {
            get { return this.maxValue; }
            set
            {
                this.maxValue = GetFixedSizeVector(value);

                for (var i = 0; i < MaxComponents; i++)
                {
                    this.valuesTextBox[i].MaxValue = this.maxValue[i];
                }

                this.positionGraph.MaxValue = ToGraphValue(this.maxValue, this.EditMode);
            }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = ImplicitButton.HoverBackgroundProperty.AddOwner(typeof(PositionEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var valueEditor = (PositionEditor)sender;
            var value = (Brush)e.NewValue;

            for (var i = 0; i < MaxComponents; i++)
            {
                valueEditor.valuesTextBox[i].SetValue(ImplicitButton.HoverBackgroundProperty, value);
            }
        }));

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = ImplicitButton.PressedBackgroundProperty.AddOwner(typeof(PositionEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var valueEditor = (PositionEditor)sender;
            var value = (Brush)e.NewValue;

            for (var i = 0; i < MaxComponents; i++)
            {
                valueEditor.valuesTextBox[i].SetValue(ImplicitButton.PressedBackgroundProperty, value);
            }
        }));

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty EditForegroundProperty = NumberTextBox.EditForegroundProperty.AddOwner(typeof(PositionEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var valueEditor = (PositionEditor)sender;
            var value = (Brush)e.NewValue;

            for (var i = 0; i < MaxComponents; i++)
            {
                valueEditor.valuesTextBox[i].SetValue(NumberTextBox.EditForegroundProperty, value);
            }
        }));

        public Brush EditForeground
        {
            get { return (Brush)GetValue(EditForegroundProperty); }
            set { SetValue(EditForegroundProperty, value); }
        }

        public static readonly DependencyProperty DragForegroundProperty = NumberTextBox.DragForegroundProperty.AddOwner(typeof(PositionEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var valueEditor = (PositionEditor)sender;
            var value = (Brush)e.NewValue;

            for (var i = 0; i < MaxComponents; i++)
            {
                valueEditor.valuesTextBox[i].SetValue(NumberTextBox.DragForegroundProperty, value);
            }

        }));

        public Brush DragForeground
        {
            get { return (Brush)GetValue(DragForegroundProperty); }
            set { SetValue(DragForegroundProperty, value); }
        }

        public static readonly DependencyProperty TextFontFamilyProperty = DependencyProperty.Register("TextFontFamily", typeof(FontFamily), typeof(PositionEditor), new FrameworkPropertyMetadata(null, (sender, e) =>
        {
            if (e.NewValue != null)
            {
                var valueEditor = (PositionEditor)sender;
                var value = (FontFamily)e.NewValue;

                for (var i = 0; i < MaxComponents; i++)
                {
                    valueEditor.valuesTextBox[i].SetValue(TextElement.FontFamilyProperty, value);
                }
            }
        }));

        public FontFamily TextFontFamily
        {
            get { return (FontFamily)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }

        private double dragSensitivity;
        public double DragSensitivity
        {
            get { return this.dragSensitivity; }
            set
            {
                this.dragSensitivity = value;

                for (var i = 0; i < MaxComponents; i++)
                {
                    this.valuesTextBox[i].DragSensitivity = this.dragSensitivity;
                }
            }
        }

        public Vector<double> step;
        public Vector<double> Step
        {
            get { return this.step; }
            set
            {
                this.step = GetFixedSizeVector(value);

                for (var i = 0; i < MaxComponents; i++)
                {
                    this.valuesTextBox[i].Step = this.step[i];
                }

                this.positionGraph.Step = ToGraphValue(this.step, this.EditMode);
            }
        }

        public int roundDecimals;
        public int RoundDecimals
        {
            get { return this.roundDecimals; }
            set
            {
                this.roundDecimals = value;

                for (var i = 0; i < MaxComponents; i++)
                {
                    this.valuesTextBox[i].RoundDecimals = this.roundDecimals;
                }

                this.positionGraph.RoundDecimals = this.roundDecimals;
            }
        }

        public event EventHandler? ColumnRatioChanged;
        public double ColumnRatio
        {
            get { return this.splitterHandle.Ratio; }
            set { this.splitterHandle.Ratio = value; }
        }

        private readonly GeometryButton focusViewButton;
        private readonly GeometryButton resetViewButton;
        private readonly IClipboard clipboard;
        private readonly ITextSerializer<Vector<double>> valueTextSerializer;
        private readonly PositionGraph positionGraph;
        private readonly TextBlock scaleTextBlock;
        private readonly FrameworkElement valuesPanel;
        private readonly SplitterHandle splitterHandle;
        private readonly FrameworkElement[] visualChildren;
        private readonly TextBlock modeTextBlock;
        private readonly NumberTextBox[] valuesTextBox;
        private bool skipChangeEvent;
        private Vector<double>? lastSourceValue;
        private Vector<double>? lastTargetValue;
        private Vector<double> lastValueSign;
        private double[]? editStartValue;
        private int[]? editStartValueDisplayDecimals;

        public PositionEditor(IClipboard clipboard, IPositionGraphInputs inputs, IApplicationTheme theme)
        {
            this.clipboard = clipboard;
            this.valueTextSerializer = VectorTextSerializer.Create(ValueTextSerializer.Double);
            this.positionGraph = new PositionGraph(inputs, theme.Graph);

            this.scaleTextBlock = new TextBlock { IsHitTestVisible = false }.
                WithReference(TextElement.FontFamilyProperty, theme.CodeFontFamily).
                WithReference(TextElement.FontSizeProperty, theme.CodeFontSize);

            var axisBrush = new[] { AxisXBrush, AxisYBrush, AxisZBrush };
            this.valuesTextBox = Enumerable.Range(0, MaxComponents).Select(i => new NumberTextBox(theme)
            {
                MinValue = Double.MinValue,
                MaxValue = Double.MaxValue,
                ProgressBrush = axisBrush[i]
            }.
            WithHandler(NumberTextBox.RawValueEditStartedEvent, OnValueEditStarted).
            WithHandler(NumberTextBox.RawTextEditStartedEvent, OnValueEditStarted).
            WithHandler(NumberTextBox.ValueEditCommittedEvent, OnValueEditCommitted).
            WithHandler(NumberTextBox.ValueEditCanceledEvent, OnValueEditCanceled).
            WithHandler(NumberTextBox.ValueChangedEvent, OnValueChanged)).ToArray();

            this.dragSensitivity = this.valuesTextBox[0].DragSensitivity;

            this.focusViewButton = new GeometryButton(FocusViewGeometry, theme) { BorderThickness = new Thickness(1) }.
                WithReference(Control.BackgroundProperty, theme.WindowBackground).
                WithReference(Control.BorderBrushProperty, theme.ControlBorder).
                WithHandler(ButtonBase.ClickEvent, (sender, e) => this.positionGraph.FocusView());

            this.resetViewButton = new GeometryButton(ResetViewGeometry, theme) { BorderThickness = new Thickness(1) }.
                WithReference(Control.BackgroundProperty, theme.WindowBackground).
                WithReference(Control.BorderBrushProperty, theme.ControlBorder).
                WithHandler(ButtonBase.ClickEvent, (sender, e) => this.positionGraph.ResetView());

            this.modeTextBlock = new TextBlock { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center, Padding = new Thickness(0, 5, 0, 5) };

            var modeButton = new ImplicitButton(theme)
            {
                Content = this.modeTextBlock
            };

            modeButton.Click += (sender, e) =>
            {
                this.EditMode = this.Is2D ? PositionEditMode.XY : ((PositionEditMode)(((int)this.EditMode + 1) % Enum.GetValues<PositionEditMode>().Length));
                e.Handled = true;
            };

            this.valuesPanel = CreateStackPanel(Orientation.Vertical, modeButton, this.valuesTextBox[0], this.valuesTextBox[1], this.valuesTextBox[2], new FrameworkElement { Height = 1 });
            MultiNumberTextBoxEditBehavior.Register(this.valuesPanel);

            this.splitterHandle = new SplitterHandle { Ratio = 0.5 };
            this.splitterHandle.RatioChanged += (sender, e) =>
            {
                this.ColumnRatioChanged?.Invoke(this, EventArgs.Empty);
                InvalidateMeasure();
            };

            this.visualChildren = new[]
            {
                this.positionGraph,
                this.scaleTextBlock,
                this.valuesPanel,
                this.focusViewButton,
                this.resetViewButton,
                this.splitterHandle,
            };

            foreach (var visualChild in this.visualChildren)
            {
                AddVisualChild(visualChild);
            }

            this.positionGraph.ScaleChanged += (sender, e) => OnPositionGraphScaleChanged();
            this.positionGraph.OffsetChanged += (sender, e) => OnPositionGraphOffsetChanged();
            this.positionGraph.ValueChanged += OnPositionGraphValueChanged;
            this.positionGraph.ToggleTargetValueRequested += OnPositionGraphToggleTargetValueRequested;
            this.positionGraph.ToggleSourceValueRequested += OnPositionGraphToggleSourceValueRequested;

            this.Focusable = true;
            this.FocusVisualStyle = null;

            theme.TextEditForeground.SetReference(this, NumberTextBox.EditForegroundProperty);
            theme.TextDragForeground.SetReference(this, NumberTextBox.DragForegroundProperty);
            theme.ControlPressedBackground.SetReference(this, ImplicitButton.PressedBackgroundProperty);
            theme.ControlHoveredBackground.SetReference(this, ImplicitButton.HoverBackgroundProperty);
            theme.CodeFontFamily.SetReference(this, TextFontFamilyProperty);

            this.minValue = Vector.Create(MaxComponents, Double.MinValue);
            this.maxValue = Vector.Create(MaxComponents, Double.MaxValue);
            this.offset = Vector.Create(MaxComponents, 0.0);
            this.value = Vector.Create(MaxComponents, 0.0);
            this.sourceValue = Vector.Create(MaxComponents, 0.0);
            this.step = Vector.Create(MaxComponents, 0.001);
            this.lastValueSign = Vector.Create(MaxComponents, 1.0);

            OnPositionGraphScaleChanged();
            SetGraphEditMode();
        }

        public void ResetLastValues()
        {
            this.lastTargetValue = null;
            this.lastSourceValue = null;
            this.lastValueSign = Vector.Create(MaxComponents, 1.0);
        }

        public void ResetView()
        {
            this.positionGraph.ResetView();
        }

        private void SetGraphEditMode()
        {
            switch (this.EditMode)
            {
                case PositionEditMode.XY:
                    this.modeTextBlock.Text = "XY";
                    this.valuesTextBox[0].Opacity = 1.0;
                    this.valuesTextBox[1].Opacity = 1.0;
                    this.valuesTextBox[2].Opacity = 0.3;
                    this.positionGraph.AxisXStroke = AxisXBrush;
                    this.positionGraph.AxisYStroke = AxisYBrush;
                    break;

                case PositionEditMode.XZ:
                    this.modeTextBlock.Text = "XZ";
                    this.valuesTextBox[0].Opacity = 1.0;
                    this.valuesTextBox[1].Opacity = 0.3;
                    this.valuesTextBox[2].Opacity = 1.0;
                    this.positionGraph.AxisXStroke = AxisXBrush;
                    this.positionGraph.AxisYStroke = AxisZBrush;
                    break;

                case PositionEditMode.YZ:
                    this.modeTextBlock.Text = "YZ";
                    this.valuesTextBox[0].Opacity = 0.3;
                    this.valuesTextBox[1].Opacity = 1.0;
                    this.valuesTextBox[2].Opacity = 1.0;
                    this.positionGraph.AxisXStroke = AxisZBrush;
                    this.positionGraph.AxisYStroke = AxisYBrush;
                    break;

                default:
                    throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{this.EditMode}\"");
            }

            this.positionGraph.Value = ToGraphValue(this.Value, this.EditMode);
            this.positionGraph.SourceValue = ToGraphValue(this.SourceValue, this.EditMode);
            this.positionGraph.MinValue = ToGraphValue(this.minValue, this.EditMode);
            this.positionGraph.MaxValue = ToGraphValue(this.maxValue, this.EditMode);
            this.positionGraph.Offset = ToGraphValue(this.offset, this.EditMode);
            this.positionGraph.Step = ToGraphValue(this.step, this.EditMode);
        }

        private void OnPositionGraphScaleChanged()
        {
            var decimals = Math.Floor(Math.Log10(this.positionGraph.Scale));
            this.scaleTextBlock.Text = $"Scale: {Math.Pow(10.0, -decimals - 1).ToString("f" + Math.Max(1, decimals + 1))}";
            this.ScaleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnPositionGraphOffsetChanged()
        {
            this.offset = FromGraphValue(this.positionGraph.Offset, this.offset, this.EditMode);
            this.OffsetChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnPositionGraphToggleTargetValueRequested(object? sender, EventArgs e)
        {
            try
            {
                this.skipChangeEvent = true;

                if (this.lastTargetValue != null)
                {
                    this.Value = this.lastTargetValue;
                    this.lastTargetValue = null;
                }
                else
                {
                    this.lastTargetValue = this.Value;
                    this.Value = this.SourceValue;
                }

                SetTextBoxValues(this.value);
                this.positionGraph.Value = ToGraphValue(this.Value, this.EditMode);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnPositionGraphToggleSourceValueRequested(object? sender, EventArgs e)
        {
            try
            {
                this.skipChangeEvent = true;

                if (this.lastSourceValue != null)
                {
                    this.SourceValue = this.lastSourceValue;
                    this.lastSourceValue = null;
                }
                else
                {
                    this.lastSourceValue = this.SourceValue;
                    this.SourceValue = this.Value;
                }

                this.positionGraph.SourceValue = ToGraphValue(this.SourceValue, this.EditMode);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnPositionGraphValueChanged(object? sender, EventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;

                this.lastSourceValue = null;
                this.lastTargetValue = null;

                var x = this.positionGraph.Value.X;
                var y = this.positionGraph.Value.Y;

                switch (this.EditMode)
                {
                    case PositionEditMode.XY:
                        this.Value = new Vector<double>(
                            x,
                            y,
                            !this.Is2D && this.NormalizeValue ? this.lastValueSign[2] * GetNormalComponent(x, y) : this.Value[2]);
                        break;
                    case PositionEditMode.XZ:
                        this.Value = new Vector<double>(
                            x,
                            !this.Is2D && this.NormalizeValue ? this.lastValueSign[1] * GetNormalComponent(x, y) : this.Value[1],
                            y);
                        break;
                    case PositionEditMode.YZ:
                        this.Value = new Vector<double>(
                            !this.Is2D && this.NormalizeValue ? this.lastValueSign[0] * GetNormalComponent(x, y) : this.Value[0],
                            y,
                            x);
                        break;
                    default: throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{this.EditMode}\"");
                }

                SetTextBoxValues(this.Value, this.positionGraph.ValueDisplayDecimals);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnValueEditStarted(object sender, RoutedEventArgs e)
        {
            this.editStartValue = this.valuesTextBox.Select(textBox => textBox.Value).ToArray();
            this.editStartValueDisplayDecimals = this.valuesTextBox.Select(textBox => textBox.ValueDisplayDecimals).ToArray();
        }

        private void OnValueEditCanceled(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < MaxComponents; i++)
            {
                this.valuesTextBox[i].EditValue(this.editStartValue![i], this.editStartValueDisplayDecimals![i]);
            }
        }

        private void OnValueEditCommitted(object sender, RoutedEventArgs e)
        {
            if (!this.NormalizeValue)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;
                SetTextBoxValues(this.Value, ((NumberTextBox)sender).ValueDisplayDecimals);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;

                var values = this.valuesTextBox.Select(textBox => textBox.Value).ToArray();
                var valueDisplayDecimals = ((NumberTextBox)sender).ValueDisplayDecimals;

                if (this.editStartValue == null)
                {
                    this.editStartValue = this.value.ToArray();
                }

                if (this.NormalizeValue)
                {
                    values = values.Select(value => Math.Clamp(value, -1.0, 1.0)).ToArray();

                    if (this.Is2D)
                    {
                        var editIndex = sender == this.valuesTextBox[0] ? 0 : 1;
                        var nonEditIndex = (editIndex + 1) % 2;

                        values[nonEditIndex] = this.lastValueSign[nonEditIndex] * GetNormalComponent(values[editIndex]);
                        this.valuesTextBox[nonEditIndex].EditValue(values[nonEditIndex], valueDisplayDecimals);
                    }
                    else
                    {
                        var nonEditIndex = this.editMode == PositionEditMode.XY ? 2 : this.editMode == PositionEditMode.XZ ? 1 : 0;
                        var editIndex0 = (nonEditIndex + 1) % 3;
                        var editIndex1 = (nonEditIndex + 2) % 3;

                        if (this.valuesTextBox[nonEditIndex] == sender)
                        {
                            Normalize(values[nonEditIndex], this.editStartValue[editIndex0], this.editStartValue[editIndex1], out values[editIndex0], out values[editIndex1]);
                        }
                        else
                        {
                            values[nonEditIndex] = this.lastValueSign[nonEditIndex] * GetNormalComponent(values[editIndex0], values[editIndex1]);
                        }

                        Normalize(values[0], values[1], values[2], out values[0], out values[1], out values[2]);

                        for (var i = 0; i < MaxComponents; i++)
                        {
                            if (this.valuesTextBox[i] != sender)
                            {
                                this.valuesTextBox[i].EditValue(values[i], valueDisplayDecimals);
                            }
                        }
                    }
                }

                this.Value = new Vector<double>(values);
                this.positionGraph.Value = ToGraphValue(this.Value, this.EditMode);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void SetTextBoxValues(Vector<double> value, int decimals = -1)
        {
            if (decimals < 0 && this.NormalizeValue)
            {
                decimals = NormalDecimals;
            }

            for (var i = 0; i < MaxComponents; i++)
            {
                this.valuesTextBox[i].EditValue(value[i], decimals);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visualChildren[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.positionGraph.Measure(availableSize);
            this.valuesPanel.Measure(availableSize);
            this.scaleTextBlock.Measure(availableSize);
            this.focusViewButton.Measure(availableSize);
            this.resetViewButton.Measure(availableSize);
            this.splitterHandle.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var pickerWidth = finalSize.Width * this.splitterHandle.Ratio;

            this.positionGraph.Arrange(new Rect(0, 0, pickerWidth, finalSize.Height));
            this.scaleTextBlock.Arrange(new Rect(4, finalSize.Height - this.scaleTextBlock.DesiredSize.Height, Math.Min(this.scaleTextBlock.DesiredSize.Width, Math.Max(0, pickerWidth - 4)), this.scaleTextBlock.DesiredSize.Height));
            this.valuesPanel.Arrange(new Rect(pickerWidth, 0, finalSize.Width - pickerWidth, this.valuesPanel.DesiredSize.Height));
            this.focusViewButton.Arrange(new Rect(new Point(finalSize.Width - this.focusViewButton.DesiredSize.Width - this.resetViewButton.DesiredSize.Width - ButtonsMagin - ButtonsMagin, finalSize.Height - this.focusViewButton.DesiredSize.Height - ButtonsMagin), this.focusViewButton.DesiredSize));
            this.resetViewButton.Arrange(new Rect(new Point(finalSize.Width - this.resetViewButton.DesiredSize.Width - ButtonsMagin, finalSize.Height - this.resetViewButton.DesiredSize.Height - ButtonsMagin), this.resetViewButton.DesiredSize));
            this.splitterHandle.Arrange(new Rect(finalSize));

            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                this.clipboard.SetText(this.valueTextSerializer.Serialize(this.Value));
                e.Handled = true;
            }

            if (e.Key == Key.V && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                if (this.clipboard.TryGetText(out var text) && this.valueTextSerializer.TryDeserialize(text, out var value))
                {
                    this.Value = value;
                }

                e.Handled = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        private static Point ToGraphValue(Vector<double> value, PositionEditMode mode)
        {
            switch (mode)
            {
                case PositionEditMode.XY: return new Point(value[0], value[1]);
                case PositionEditMode.XZ: return new Point(value[0], value[2]);
                case PositionEditMode.YZ: return new Point(value[2], value[1]);
                default: throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{mode}\"");
            }
        }

        private static Vector<double> FromGraphValue(Point editedValue, Vector<double> source, PositionEditMode mode)
        {
            switch (mode)
            {
                case PositionEditMode.XY: return new Vector<double>(editedValue.X, editedValue.Y, source[2]);
                case PositionEditMode.XZ: return new Vector<double>(editedValue.X, source[1], editedValue.Y);
                case PositionEditMode.YZ: return new Vector<double>(source[0], editedValue.Y, editedValue.X);
                default: throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{mode}\"");
            }
        }

        private static Vector<int> FromGraphValue(Point editedValue, Vector<int> source, PositionEditMode mode)
        {
            switch (mode)
            {
                case PositionEditMode.XY: return new Vector<int>((int)editedValue.X, (int)editedValue.Y, source[2]);
                case PositionEditMode.XZ: return new Vector<int>((int)editedValue.X, source[1], (int)editedValue.Y);
                case PositionEditMode.YZ: return new Vector<int>(source[0], (int)editedValue.Y, (int)editedValue.X);
                default: throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{mode}\"");
            }
        }

        private static double GetNormalComponent(double sourceX)
        {
            return Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX));
        }

        private static double GetNormalComponent(double sourceX, double sourceY)
        {
            return Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX - sourceY * sourceY));
        }

        private static void Normalize(double sourceX, double sourceY, double sourceZ, out double targetY, out double targetZ)
        {
            if (Math.Abs(sourceY) < Math.Abs(sourceZ))
            {
                var a = sourceY / sourceZ;
                targetZ = Math.Sqrt(Math.Max(0.0, (1.0 - sourceX * sourceX) / (1.0 + a * a)));
                targetY = GetNormalComponent(sourceX, targetZ);
                targetY *= Math.Sign(sourceY);
                targetZ *= Math.Sign(sourceZ);
            }
            else if (Math.Abs(sourceZ) < Math.Abs(sourceY))
            {
                var a = sourceZ / sourceY;
                targetY = Math.Sqrt(Math.Max(0.0, (1.0 - sourceX * sourceX) / (1.0 + a * a)));
                targetZ = GetNormalComponent(sourceX, targetY);
                targetY *= Math.Sign(sourceY);
                targetZ *= Math.Sign(sourceZ);
            }
            else
            {
                targetY = Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX));
                targetZ = 0.0;
            }
        }

        private static void Normalize(double sourceX, double sourceY, double sourceZ, out double targetX, out double targetY, out double targetZ)
        {
            var length = Math.Sqrt(sourceX * sourceX + sourceY * sourceY + sourceZ * sourceZ);
            if (length > 0.0)
            {
                targetX = sourceX / length;
                targetY = sourceY / length;
                targetZ = sourceZ / length;
            }
            else
            {
                targetX = 1.0;
                targetY = 0.0;
                targetZ = 0.0;
            }
        }

        private static Vector<double> GetFixedSizeVector(Vector<double> value)
        {
            return value.Count == MaxComponents ? value : Vector.Create(value.ToArray().Take(MaxComponents).Concat(Enumerable.Range(0, MaxComponents - value.Count).Select(i => 0.0)).ToArray());
        }

        private static Grid CreateColumnPanel(params FrameworkElement[] children)
        {
            var grid = new Grid();

            var i = 0;
            foreach (var child in children)
            {
                child.SetValue(Grid.ColumnProperty, i++);
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.Children.Add(child);
            }

            return grid;
        }

        private static StackPanel CreateStackPanel(Orientation orientation, params FrameworkElement[] children)
        {
            var stackPanel = new StackPanel { Orientation = orientation };

            foreach (var child in children)
            {
                stackPanel.Children.Add(child);
            }

            return stackPanel;
        }

        private static DockPanel CreateDockPanel(Dock dock, params FrameworkElement[] children)
        {
            var dockPanel = new DockPanel { LastChildFill = true };

            foreach (var child in children)
            {
                child.SetValue(DockPanel.DockProperty, dock);
                dockPanel.Children.Add(child);
            }

            return dockPanel;
        }

        private static TextBlock CreateTextBlock(string text, double width)
        {
            return new TextBlock { Text = text, Width = width, TextAlignment = TextAlignment.Center };
        }
    }
}
