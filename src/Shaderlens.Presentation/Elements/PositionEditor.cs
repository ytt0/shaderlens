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

        protected override int VisualChildrenCount { get { return this.visualChildren.Length; } }

        private PositionEditMode editMode;
        public PositionEditMode EditMode
        {
            get { return this.editMode; }
            set
            {
                if (this.editMode != value)
                {
                    this.editMode = value;
                    SetGraphEditMode();
                }
            }
        }

        public event EventHandler? ValueChanged;
        private Vector<double> valueSign;
        private Vector<double> value;
        public Vector<double> Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.valueSign = new Vector<double>(
                    this.value[0] != 0 ? Math.Sign(this.value[0]) : this.valueSign[0],
                    this.value[1] != 0 ? Math.Sign(this.value[1]) : this.valueSign[1],
                    this.value[2] != 0 ? Math.Sign(this.value[2]) : this.valueSign[2]);

                this.ValueChanged?.Invoke(this, EventArgs.Empty);

                if (!this.skipChangeEvent)
                {
                    this.skipChangeEvent = true;
                    try
                    {
                        SetTextBoxValues(value, 3);
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
                this.sourceValue = value;
                if (!this.skipChangeEvent)
                {
                    SetGraphEditMode();
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
                this.offset = value;
                this.OffsetChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? ScaleChanged;
        public double Scale
        {
            get { return this.positionGraph.Scale; }
            set { this.positionGraph.Scale = value; }
        }

        private bool isZAxisVisible;
        public bool IsZAxisVisible
        {
            get { return this.isZAxisVisible; }
            set
            {
                this.isZAxisVisible = value;
                this.zTextBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                InvalidateArrange();
            }
        }

        private bool normalizeValue;
        public bool NormalizeValue
        {
            get { return this.normalizeValue; }
            set
            {
                this.normalizeValue = value;
                this.positionGraph.NormalBounds = value;
                this.positionGraph.NormalizeValue = value && !this.isZAxisVisible;
            }
        }

        private Vector<double> minValue;
        public Vector<double> MinValue
        {
            get { return this.minValue; }
            set
            {
                this.minValue = value;
                this.positionGraph.MinValue = GetEditValue(this.minValue, this.EditMode);
            }
        }

        private Vector<double> maxValue;
        public Vector<double> MaxValue
        {
            get { return this.maxValue; }
            set
            {
                this.maxValue = value;
                this.positionGraph.MaxValue = GetEditValue(this.maxValue, this.EditMode);
            }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = ImplicitButton.HoverBackgroundProperty.AddOwner(typeof(PositionEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var valueEditor = (PositionEditor)sender;
            var value = (Brush)e.NewValue;
            valueEditor.xTextBox.SetValue(ImplicitButton.HoverBackgroundProperty, value);
            valueEditor.yTextBox.SetValue(ImplicitButton.HoverBackgroundProperty, value);
            valueEditor.zTextBox.SetValue(ImplicitButton.HoverBackgroundProperty, value);
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
            valueEditor.xTextBox.SetValue(ImplicitButton.PressedBackgroundProperty, value);
            valueEditor.yTextBox.SetValue(ImplicitButton.PressedBackgroundProperty, value);
            valueEditor.zTextBox.SetValue(ImplicitButton.PressedBackgroundProperty, value);
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
            valueEditor.xTextBox.SetValue(NumberTextBox.EditForegroundProperty, value);
            valueEditor.yTextBox.SetValue(NumberTextBox.EditForegroundProperty, value);
            valueEditor.zTextBox.SetValue(NumberTextBox.EditForegroundProperty, value);
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
            valueEditor.xTextBox.SetValue(NumberTextBox.DragForegroundProperty, value);
            valueEditor.yTextBox.SetValue(NumberTextBox.DragForegroundProperty, value);
            valueEditor.zTextBox.SetValue(NumberTextBox.DragForegroundProperty, value);
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
                valueEditor.xTextBox.SetValue(TextElement.FontFamilyProperty, value);
                valueEditor.yTextBox.SetValue(TextElement.FontFamilyProperty, value);
                valueEditor.zTextBox.SetValue(TextElement.FontFamilyProperty, value);
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
                this.xTextBox.DragSensitivity = value;
                this.yTextBox.DragSensitivity = value;
                this.zTextBox.DragSensitivity = value;
            }
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
        private readonly NumberTextBox xTextBox;
        private readonly NumberTextBox yTextBox;
        private readonly NumberTextBox zTextBox;
        private bool skipChangeEvent;
        private Vector<double>? lastSourceValue;
        private Vector<double>? lastTargetValue;
        private double xEditStartValue;
        private double yEditStartValue;
        private double zEditStartValue;
        private int xEditStartValueDecimals;
        private int yEditStartValueDecimals;
        private int zEditStartValueDecimals;

        public PositionEditor(IClipboard clipboard, IPositionGraphInputs inputs, IApplicationTheme theme)
        {
            this.clipboard = clipboard;
            this.valueTextSerializer = VectorTextSerializer.Create(ValueTextSerializer.Double);
            this.positionGraph = new PositionGraph(inputs, theme.Graph);

            this.scaleTextBlock = new TextBlock { IsHitTestVisible = false }.
                WithReference(TextElement.FontFamilyProperty, theme.CodeFontFamily).
                WithReference(TextElement.FontSizeProperty, theme.CodeFontSize);

            this.xTextBox = new NumberTextBox(theme) { Step = 0.001, MinValue = -1.0, MaxValue = 1.0, ProgressBrush = AxisXBrush };
            this.yTextBox = new NumberTextBox(theme) { Step = 0.001, MinValue = -1.0, MaxValue = 1.0, ProgressBrush = AxisYBrush };
            this.zTextBox = new NumberTextBox(theme) { Step = 0.001, MinValue = -1.0, MaxValue = 1.0, ProgressBrush = AxisZBrush };
            this.dragSensitivity = this.xTextBox.DragSensitivity;
            this.isZAxisVisible = true;
            this.valueSign = new Vector<double>(1.0, 1.0, 1.0);

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
                this.EditMode = (PositionEditMode)(((int)this.EditMode + 1) % Enum.GetValues<PositionEditMode>().Length);
                e.Handled = true;
            };

            this.valuesPanel = CreateStackPanel(Orientation.Vertical, modeButton, this.xTextBox, this.yTextBox, this.zTextBox, new FrameworkElement { Height = 1 });
            MultiNumberTextBoxEditBehavior.Register(this.valuesPanel);

            this.splitterHandle = new SplitterHandle { Ratio = 0.5 };
            this.splitterHandle.RatioChanged += (sender, e) => InvalidateMeasure();

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
            this.xTextBox.ValueChanged += OnValueTextBoxValueChanged;
            this.yTextBox.ValueChanged += OnValueTextBoxValueChanged;
            this.zTextBox.ValueChanged += OnValueTextBoxValueChanged;
            this.xTextBox.RawValueEditStarted += OnValueEditStarted;
            this.yTextBox.RawValueEditStarted += OnValueEditStarted;
            this.zTextBox.RawValueEditStarted += OnValueEditStarted;
            this.xTextBox.RawTextEditStarted += OnValueEditStarted;
            this.yTextBox.RawTextEditStarted += OnValueEditStarted;
            this.zTextBox.RawTextEditStarted += OnValueEditStarted;
            this.yTextBox.ValueEditCommitted += OnValueEditCommitted;
            this.xTextBox.ValueEditCommitted += OnValueEditCommitted;
            this.zTextBox.ValueEditCommitted += OnValueEditCommitted;
            this.yTextBox.ValueEditCanceled += OnValueEditCanceled;
            this.xTextBox.ValueEditCanceled += OnValueEditCanceled;
            this.zTextBox.ValueEditCanceled += OnValueEditCanceled;

            this.Focusable = true;
            this.FocusVisualStyle = null;

            theme.TextEditForeground.SetReference(this, NumberTextBox.EditForegroundProperty);
            theme.TextDragForeground.SetReference(this, NumberTextBox.DragForegroundProperty);
            theme.ControlPressedBackground.SetReference(this, ImplicitButton.PressedBackgroundProperty);
            theme.ControlHoveredBackground.SetReference(this, ImplicitButton.HoverBackgroundProperty);
            theme.CodeFontFamily.SetReference(this, TextFontFamilyProperty);

            this.minValue = new Vector<double>(Double.MinValue, Double.MinValue, Double.MinValue);
            this.maxValue = new Vector<double>(Double.MaxValue, Double.MaxValue, Double.MaxValue);
            this.offset = new Vector<double>(0.0, 0.0, 0.0);
            this.value = new Vector<double>(0.0, 0.0, 0.0);
            this.sourceValue = new Vector<double>(0.0, 0.0, 0.0);

            //this.minValue = new Vector<double>(-100.0, -100.0, -100.0);
            //this.maxValue = new Vector<double>(100.0, 100.0, 100.0);

            OnPositionGraphScaleChanged();
            SetTextBoxValues(this.value, 3);
            SetGraphEditMode();

            //this.NormalizeValue = true;
        }

        public void ResetLastValues()
        {
            this.lastTargetValue = null;
            this.lastSourceValue = null;
        }

        private void SetGraphEditMode()
        {
            switch (this.EditMode)
            {
                case PositionEditMode.XY:
                    this.modeTextBlock.Text = "XY";
                    this.xTextBox.Opacity = 1.0;
                    this.yTextBox.Opacity = 1.0;
                    this.zTextBox.Opacity = 0.3;
                    this.positionGraph.AxisXStroke = AxisXBrush;
                    this.positionGraph.AxisYStroke = AxisYBrush;
                    break;

                case PositionEditMode.XZ:
                    this.modeTextBlock.Text = "XZ";
                    this.xTextBox.Opacity = 1.0;
                    this.yTextBox.Opacity = 0.3;
                    this.zTextBox.Opacity = 1.0;
                    this.positionGraph.AxisXStroke = AxisXBrush;
                    this.positionGraph.AxisYStroke = AxisZBrush;
                    break;

                case PositionEditMode.YZ:
                    this.modeTextBlock.Text = "YZ";
                    this.xTextBox.Opacity = 0.3;
                    this.yTextBox.Opacity = 1.0;
                    this.zTextBox.Opacity = 1.0;
                    this.positionGraph.AxisXStroke = AxisZBrush;
                    this.positionGraph.AxisYStroke = AxisYBrush;
                    break;

                default:
                    throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{this.EditMode}\"");
            }

            this.positionGraph.Value = GetEditValue(this.Value, this.EditMode);
            this.positionGraph.SourceValue = GetEditValue(this.SourceValue, this.EditMode);
            this.positionGraph.MinValue = GetEditValue(this.minValue, this.EditMode);
            this.positionGraph.MaxValue = GetEditValue(this.maxValue, this.EditMode);
            this.positionGraph.Offset = GetEditValue(this.offset, this.EditMode);
        }

        private void OnPositionGraphScaleChanged()
        {
            var decimals = Math.Floor(Math.Log10(this.positionGraph.Scale));
            this.scaleTextBlock.Text = $"Scale: {Math.Pow(10.0, -decimals - 1).ToString("f" + Math.Max(1, decimals + 1))}";
            this.ScaleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnPositionGraphOffsetChanged()
        {
            this.offset = GetEditedValue(this.positionGraph.Offset, this.offset, this.EditMode);
        }

        private void OnPositionGraphToggleTargetValueRequested(object? sender, EventArgs e)
        {
            this.skipChangeEvent = true;
            try
            {
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

                SetTextBoxValues(this.value, 3);
                this.positionGraph.Value = GetEditValue(this.Value, this.EditMode);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnPositionGraphToggleSourceValueRequested(object? sender, EventArgs e)
        {
            this.skipChangeEvent = true;
            try
            {
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

                this.positionGraph.SourceValue = GetEditValue(this.SourceValue, this.EditMode);
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

            this.skipChangeEvent = true;
            try
            {
                this.lastSourceValue = null;
                this.lastTargetValue = null;

                Normalize(this.positionGraph.Value.X, this.positionGraph.Value.Y, out var normalizedComponent);

                switch (this.EditMode)
                {
                    case PositionEditMode.XY:
                        this.Value = new Vector<double>(
                            this.positionGraph.Value.X,
                            this.positionGraph.Value.Y,
                            this.NormalizeValue ? this.valueSign[2] * normalizedComponent : this.value[2]);
                        break;
                    case PositionEditMode.XZ:
                        this.Value = new Vector<double>(
                            this.positionGraph.Value.X,
                            this.NormalizeValue ? this.valueSign[1] * normalizedComponent : this.value[1],
                            this.positionGraph.Value.Y);
                        break;
                    case PositionEditMode.YZ:
                        this.Value = new Vector<double>(
                            this.NormalizeValue ? this.valueSign[0] * normalizedComponent : this.value[0],
                            this.positionGraph.Value.Y,
                            this.positionGraph.Value.X);
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
            this.xEditStartValue = this.xTextBox.Value;
            this.yEditStartValue = this.yTextBox.Value;
            this.zEditStartValue = this.zTextBox.Value;
            this.xEditStartValueDecimals = this.xTextBox.ValueDisplayDecimals;
            this.yEditStartValueDecimals = this.yTextBox.ValueDisplayDecimals;
            this.zEditStartValueDecimals = this.zTextBox.ValueDisplayDecimals;
        }

        private void OnValueEditCanceled(object sender, RoutedEventArgs e)
        {
            this.xTextBox.EditValue(this.xEditStartValue, this.xEditStartValueDecimals);
            this.yTextBox.EditValue(this.yEditStartValue, this.yEditStartValueDecimals);
            this.zTextBox.EditValue(this.zEditStartValue, this.zEditStartValueDecimals);
        }

        private void OnValueEditCommitted(object sender, RoutedEventArgs e)
        {
            if (!this.NormalizeValue)
            {
                return;
            }

            this.skipChangeEvent = true;
            try
            {
                var xValue = this.xTextBox.Value;
                var yValue = this.yTextBox.Value;
                var zValue = this.zTextBox.Value;
                Normalize(xValue, yValue, zValue, out xValue, out yValue, out zValue);

                this.Value = new Vector<double>(xValue, yValue, zValue);
                this.positionGraph.Value = GetEditValue(this.Value, this.EditMode);
                SetTextBoxValues(this.Value, ((NumberTextBox)sender).ValueDisplayDecimals);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnValueTextBoxValueChanged(object sender, EventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            this.skipChangeEvent = true;
            try
            {
                var xValue = this.xTextBox.Value;
                var yValue = this.yTextBox.Value;
                var zValue = this.zTextBox.Value;

                var xChanged = sender == this.xTextBox;
                var yChanged = sender == this.yTextBox;
                var zChanged = sender == this.zTextBox;

                var valueDisplayDecimals = ((NumberTextBox)sender).ValueDisplayDecimals;

                if (this.NormalizeValue)
                {
                    switch (this.editMode)
                    {
                        case PositionEditMode.XY:
                            if (zChanged)
                            {
                                Normalize(zValue, this.xEditStartValue, this.yEditStartValue, out xValue, out yValue);
                            }
                            else
                            {
                                Normalize(xValue, yValue, out zValue);
                            }
                            break;
                        case PositionEditMode.XZ:
                            if (yChanged)
                            {
                                Normalize(yValue, this.xEditStartValue, this.zEditStartValue, out xValue, out zValue);
                            }
                            else
                            {
                                Normalize(xValue, zValue, out yValue);
                            }
                            break;
                        case PositionEditMode.YZ:
                            if (xChanged)
                            {
                                Normalize(xValue, this.yEditStartValue, this.zEditStartValue, out yValue, out zValue);
                            }
                            else
                            {
                                Normalize(yValue, zValue, out xValue);
                            }
                            break;
                    }

                    Normalize(xValue, yValue, zValue, out xValue, out yValue, out zValue);
                }

                if (!xChanged)
                {
                    this.xTextBox.EditValue(xValue, valueDisplayDecimals);
                }

                if (!yChanged)
                {
                    this.yTextBox.EditValue(yValue, valueDisplayDecimals);
                }

                if (!zChanged)
                {
                    this.zTextBox.EditValue(zValue, valueDisplayDecimals);
                }

                this.Value = new Vector<double>(xValue, yValue, zValue);
                this.positionGraph.Value = GetEditValue(this.Value, this.EditMode);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void SetTextBoxValues(Vector<double> value, int decimals)
        {
            this.xTextBox.EditValue(value[0], decimals);
            this.yTextBox.EditValue(value[1], decimals);
            this.zTextBox.EditValue(value[2], decimals);
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

            //this.positionGraph.Arrange(new Rect(30, 30, Math.Max(0, pickerWidth - 30), Math.Max(0, finalSize.Height - 60)));
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

        private static Point GetEditValue(Vector<double> value, PositionEditMode mode)
        {
            switch (mode)
            {
                case PositionEditMode.XY: return new Point(value[0], value[1]);
                case PositionEditMode.XZ: return new Point(value[0], value[2]);
                case PositionEditMode.YZ: return new Point(value[2], value[1]);
                default: throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{mode}\"");
            }
        }

        private static Vector<double> GetEditedValue(Point editedValue, Vector<double> source, PositionEditMode mode)
        {
            switch (mode)
            {
                case PositionEditMode.XY: return new Vector<double>(editedValue.X, editedValue.Y, source[2]);
                case PositionEditMode.XZ: return new Vector<double>(editedValue.X, source[1], editedValue.Y);
                case PositionEditMode.YZ: return new Vector<double>(source[0], editedValue.Y, editedValue.X);
                default: throw new NotSupportedException($"Unexpected {nameof(PositionEditMode)} \"{mode}\"");
            }
        }

        private static void Normalize(double sourceX, double sourceY, out double targetZ)
        {
            targetZ = Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX - sourceY * sourceY));
        }

        private static void Normalize(double sourceX, double sourceY, double sourceZ, out double targetY, out double targetZ)
        {
            if (Math.Abs(sourceY) < Math.Abs(sourceZ))
            {
                var a = sourceY / sourceZ;
                targetZ = Math.Sqrt((1.0 - sourceX * sourceX) / (1.0 + a * a));
                Normalize(sourceX, targetZ, out targetY);
                targetY *= Math.Sign(sourceY);
                targetZ *= Math.Sign(sourceZ);
            }
            else if (Math.Abs(sourceZ) < Math.Abs(sourceY))
            {
                var a = sourceZ / sourceY;
                targetY = Math.Sqrt((1.0 - sourceX * sourceX) / (1.0 + a * a));
                Normalize(sourceX, targetY, out targetZ);
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
            var length2 = sourceX * sourceX + sourceY * sourceY + sourceZ * sourceZ;
            if (length2 > 0.0)
            {
                targetX = sourceX / length2;
                targetY = sourceY / length2;
                targetZ = sourceZ / length2;
            }
            else
            {
                targetX = 1.0;
                targetY = 0.0;
                targetZ = 0.0;
            }
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
