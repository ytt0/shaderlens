namespace Shaderlens.Presentation.Elements
{
    using static WinApi;

    public partial class NumberTextBox : VisualChildContainer
    {
        private const int DragStartThreadhold = 4;
        private const double DragSensitivityFactor = 500.0;

        public static readonly RoutedEvent RawValueEditStartedEvent = EventManager.RegisterRoutedEvent("RawValueEditStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler RawValueEditStarted
        {
            add { AddHandler(RawValueEditStartedEvent, value); }
            remove { RemoveHandler(RawValueEditStartedEvent, value); }
        }

        public static readonly RoutedEvent RawTextEditStartedEvent = EventManager.RegisterRoutedEvent("RawTextEditStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler RawTextEditStarted
        {
            add { AddHandler(RawTextEditStartedEvent, value); }
            remove { RemoveHandler(RawTextEditStartedEvent, value); }
        }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public static readonly RoutedEvent RawValueChangedEvent = EventManager.RegisterRoutedEvent("RawValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler RawValueChanged
        {
            add { AddHandler(RawValueChangedEvent, value); }
            remove { RemoveHandler(RawValueChangedEvent, value); }
        }

        public static readonly RoutedEvent ValueEditCommittedEvent = EventManager.RegisterRoutedEvent("ValueEditCommitted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler ValueEditCommitted
        {
            add { AddHandler(ValueEditCommittedEvent, value); }
            remove { RemoveHandler(ValueEditCommittedEvent, value); }
        }

        public static readonly RoutedEvent ValueEditCanceledEvent = EventManager.RegisterRoutedEvent("ValueEditCanceled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler ValueEditCanceled
        {
            add { AddHandler(ValueEditCanceledEvent, value); }
            remove { RemoveHandler(ValueEditCanceledEvent, value); }
        }

        public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxStyle()));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty EditForegroundProperty = DependencyProperty.Register("EditForeground", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxStyle()));
        public Brush EditForeground
        {
            get { return (Brush)GetValue(EditForegroundProperty); }
            set { SetValue(EditForegroundProperty, value); }
        }

        public static readonly DependencyProperty DragForegroundProperty = DependencyProperty.Register("DragForeground", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxStyle()));
        public Brush DragForeground
        {
            get { return (Brush)GetValue(DragForegroundProperty); }
            set { SetValue(DragForegroundProperty, value); }
        }

        public static readonly DependencyProperty InvalidForegroundProperty = DependencyProperty.Register("InvalidForeground", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxStyle()));
        public Brush InvalidForeground
        {
            get { return (Brush)GetValue(InvalidForegroundProperty); }
            set { SetValue(InvalidForegroundProperty, value); }
        }

        public static readonly DependencyProperty ProgressBrushProperty = DependencyProperty.Register("ProgressBrush", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)), FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush ProgressBrush
        {
            get { return (Brush)GetValue(ProgressBrushProperty); }
            set { SetValue(ProgressBrushProperty, value); }
        }

        public static readonly DependencyProperty ProgressTrackBrushProperty = DependencyProperty.Register("ProgressTrackBrush", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)), FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush ProgressTrackBrush
        {
            get { return (Brush)GetValue(ProgressTrackBrushProperty); }
            set { SetValue(ProgressTrackBrushProperty, value); }
        }

        public static readonly DependencyProperty ProgressTrackThicknessProperty = DependencyProperty.Register("ProgressTrackThickness", typeof(double), typeof(NumberTextBox), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public double ProgressTrackThickness
        {
            get { return (double)GetValue(ProgressTrackThicknessProperty); }
            set { SetValue(ProgressTrackThicknessProperty, value); }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = ImplicitButton.HoverBackgroundProperty.AddOwner(typeof(NumberTextBox));
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = ImplicitButton.PressedBackgroundProperty.AddOwner(typeof(NumberTextBox));
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SelectionOpacityProperty = TextBoxBase.SelectionOpacityProperty.AddOwner(typeof(NumberTextBox), new FrameworkPropertyMetadata((sender, e) => ((NumberTextBox)sender).textBox.SelectionOpacity = (double)e.NewValue));
        public double SelectionOpacity
        {
            get { return (double)GetValue(SelectionOpacityProperty); }
            set { SetValue(SelectionOpacityProperty, value); }
        }

        public static readonly DependencyProperty SelectionBrushProperty = TextBoxBase.SelectionBrushProperty.AddOwner(typeof(NumberTextBox), new FrameworkPropertyMetadata((sender, e) => ((NumberTextBox)sender).textBox.SelectionBrush = (Brush)e.NewValue));
        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty = Control.PaddingProperty.AddOwner(typeof(NumberTextBox), new FrameworkPropertyMetadata(new Thickness(), (sender, e) => ((NumberTextBox)sender).textBox.Padding = (Thickness)e.NewValue));
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(NumberTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public int ValueDisplayDecimals { get; private set; }
        public double Value { get; private set; }

        private double rawValue;
        public double RawValue
        {
            get { return this.rawValue; }
            set
            {
                if (this.rawValue == value)
                {
                    return;
                }

                SetRawValue(value);
                SetValueDisplayDecimals();
                SetValue(this.rawValue);
                SetRawText();
                RaiseValueChanged();
            }
        }

        public bool IsRawTextValid { get; private set; }
        public string RawText
        {
            get { return this.textBox.Text; }
            set
            {
                if (this.textBox.Text == value)
                {
                    return;
                }

                if (TryParseExpressionValue(value, out var parsedValue))
                {
                    SetRawText(value, true);
                    SetRawValue(parsedValue);
                    SetValueDisplayDecimals();
                    SetValue(this.rawValue);
                }
                else
                {
                    SetRawText(value, false);
                }

                RaiseValueChanged();
            }
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        private int stepDecimalSize;
        private double step;
        public double Step
        {
            get { return this.step; }
            set
            {
                this.step = value;
                this.stepDecimalSize = GetDecimalSize(this.step, this.roundDecimals);
                SetValueDisplayDecimals();
                SetRawText();
                RaiseValueChanged();
            }
        }

        private int roundDecimals;
        public int RoundDecimals
        {
            get { return this.roundDecimals; }
            set
            {
                this.roundDecimals = value;
                this.stepDecimalSize = GetDecimalSize(this.step, this.roundDecimals);
                SetValueDisplayDecimals();
                SetValue(this.rawValue);
                SetRawText();
                RaiseValueChanged();
            }
        }

        public double DragSensitivity { get; set; }
        public double SmallStepFactor { get; set; }
        public double MediumStepFactor { get; set; }
        public double LargeStepFactor { get; set; }
        public IInputSpan SmallStepModifier { get; set; }
        public IInputSpan MediumStepModifier { get; set; }
        public IInputSpan LargeStepModifier { get; set; }
        public IInputSpan? ScrollModifier { get; set; }
        public IExpressionParser? ExpressionParser { get; set; }

        private readonly TextBox textBox;
        private bool isDragging;
        private bool skipTextBoxChangedEvent;
        private double startRawValue;
        private int startValueDisplayDecimals;
        private bool isRawTextEditing;
        private bool isRawValueEditing;
        private Point mouseDownPosition;
        private Point dragStartCursorPosition;
        private Point windowCenterPosition;
        private bool scrollWithinBounds;
        private long dragTime;
        private double dragValue;
        private bool rawValueChanged;
        private bool valueChanged;
        private bool handleMouseUp;

        public NumberTextBox(IApplicationTheme theme)
        {
            this.textBox = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = default,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Arrow,
                AllowDrop = false,
                Text = "0.000",
            };

            this.textBox.ContextMenu = new StyledTextBoxMenu(this.textBox, theme.Menu);

            this.textBox.TextChanged += OnTextBoxTextChanged;
            this.textBox.GotFocus += (sender, e) =>
            {
                StartRawTextEdit();
                SetTextBoxStyle();

                this.textBox.Dispatcher.InvokeAsync(() =>
                {
                    var caretIndex = textBox.CaretIndex - textBox.Text.Take(textBox.CaretIndex).Count(ch => ch == ',');
                    var selectionStart = textBox.SelectionStart - textBox.Text.Take(textBox.SelectionStart).Count(ch => ch == ',');
                    var selectionLength = textBox.SelectionLength - textBox.Text.Skip(textBox.SelectionStart).Take(textBox.SelectionLength).Count(ch => ch == ',');
                    textBox.Text = new string(textBox.Text.Where(ch => ch != ',').ToArray());
                    textBox.CaretIndex = caretIndex;
                    textBox.SelectionStart = selectionStart;
                    textBox.SelectionLength = selectionLength;
                });

                e.Handled = true;
            };

            this.textBox.LostFocus += (sender, e) =>
            {
                if (this.IsRawTextValid)
                {
                    CommitValueEdit();
                }
                else
                {
                    CancelValueEdit();
                }

                SetTextBoxStyle();
                this.textBox.Select(this.textBox.Text.Length, 0);
                e.Handled = true;
            };

            TextBoxSelectionBehavior.Register(this.textBox);

            this.rawValue = 0.0;
            this.RawText = "0.000";
            this.IsRawTextValid = true;
            this.MinValue = 0.0;
            this.MaxValue = 1.0;
            this.RoundDecimals = 6;
            this.Step = 0.001;
            this.Padding = new Thickness(0, 5, 0, 5);

            this.DragSensitivity = 1.0;
            this.SmallStepFactor = 0.1;
            this.MediumStepFactor = 10.0;
            this.LargeStepFactor = 100.0;
            this.SmallStepModifier = new ModifierKeyInputSpan(ModifierKey.Shift);
            this.MediumStepModifier = new ModifierKeyInputSpan(ModifierKey.Ctrl);
            this.LargeStepModifier = new AllInputSpans(new[] { new ModifierKeyInputSpan(ModifierKey.Ctrl), new ModifierKeyInputSpan(ModifierKey.Shift) });
            this.ExpressionParser = Shaderlens.Serialization.Text.ExpressionParser.Instance;

            this.FocusVisualStyle = null;
            this.Focusable = true;
            SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None);

            theme.TextProgress.SetReference(this, ProgressBrushProperty);
            theme.TextProgressTrack.SetReference(this, ProgressTrackBrushProperty);
            theme.TextEditForeground.SetReference(this, EditForegroundProperty);
            theme.TextDragForeground.SetReference(this, DragForegroundProperty);
            theme.TextInvalidForeground.SetReference(this, InvalidForegroundProperty);
            theme.ControlHoveredBackground.SetReference(this, HoverBackgroundProperty);
            theme.ControlPressedBackground.SetReference(this, PressedBackgroundProperty);
            theme.TextSelectionBackground.SetReference(this, SelectionBrushProperty);
            theme.TextSelectionOpacity.SetReference(this, SelectionOpacityProperty);
            theme.CodeFontFamily.SetReference(this, TextElement.FontFamilyProperty);
        }

        protected override FrameworkElement GetChild()
        {
            return this.textBox;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var background = this.isDragging ? this.PressedBackground : this.IsMouseOver || this.IsFocused || this.IsHighlighted ? this.HoverBackground : Brushes.Transparent;

            drawingContext.DrawRectangle(background, null, new Rect(this.RenderSize));
            drawingContext.DrawRectangle(this.ProgressTrackBrush, null, new Rect(0, this.RenderSize.Height - this.ProgressTrackThickness, this.RenderSize.Width, this.ProgressTrackThickness));

            if (this.MinValue > Double.MinValue && this.MaxValue < Double.MaxValue &&
                this.MinValue < this.MaxValue && this.MinValue > Double.MinValue && this.MaxValue < Double.MaxValue)
            {
                var progress = Math.Clamp((this.Value - this.MinValue) / (this.MaxValue - this.MinValue), 0.0, 1.0);
                drawingContext.DrawRectangle(this.ProgressBrush, null, new Rect(0, this.RenderSize.Height - this.ProgressTrackThickness, this.RenderSize.Width * progress, this.ProgressTrackThickness));
            }
        }

        public void StartRawValueEdit()
        {
            this.isRawValueEditing = true;
            this.startRawValue = this.RawValue;
            this.startValueDisplayDecimals = this.ValueDisplayDecimals;
            RaiseEvent(new RoutedEventArgs(RawValueEditStartedEvent));
        }

        public void StartRawTextEdit()
        {
            this.isRawTextEditing = true;
            this.startRawValue = this.RawValue;
            this.startValueDisplayDecimals = this.ValueDisplayDecimals;
            this.IsRawTextValid = true;
            RaiseEvent(new RoutedEventArgs(RawTextEditStartedEvent));
        }

        public void EditValue(double rawValue, int decimals)
        {
            SetRawValue(rawValue);
            this.ValueDisplayDecimals = Math.Max(decimals >= 0 ? Math.Min(decimals, this.RoundDecimals) : GetDecimalSize(this.rawValue, this.RoundDecimals), this.stepDecimalSize);
            SetValue(rawValue);
            SetRawText();
            InvalidateVisual();
            RaiseValueChanged();
        }

        public void EditValue(double rawValue, int decimals, string rawText, bool isRawTextValid)
        {
            SetRawValue(rawValue);
            this.ValueDisplayDecimals = decimals;
            SetRawText(rawText, isRawTextValid);
            SetValue(rawValue);
            InvalidateVisual();
            RaiseValueChanged();
        }

        public void CancelValueEdit()
        {
            if (this.isRawValueEditing || this.isRawTextEditing)
            {
                this.isRawValueEditing = false;
                this.isRawTextEditing = false;

                RaiseEvent(new RoutedEventArgs(ValueEditCanceledEvent));

                SetRawValue(this.startRawValue);
                this.ValueDisplayDecimals = this.startValueDisplayDecimals;
                SetValue(this.rawValue);
                SetRawText();

                if (this.textBox.IsFocused)
                {
                    Focus();
                }

                RaiseValueChanged();
            }
        }

        public void CommitValueEdit()
        {
            if (!this.isRawValueEditing && !this.isRawTextEditing)
            {
                return;
            }

            if (this.isRawTextEditing)
            {
                if (!this.IsRawTextValid)
                {
                    CancelValueEdit();
                    return;
                }

                SetValueDisplayDecimals();
            }

            this.isRawValueEditing = false;
            this.isRawTextEditing = false;

            RaiseEvent(new RoutedEventArgs(ValueEditCommittedEvent));

            if (this.textBox.IsFocused)
            {
                Focus();
            }

            SetRawValue(this.Value);
            SetRawText();

            RaiseValueChanged();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            CancelValueEdit();

            InvalidateVisual();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (this.isDragging)
            {
                EndDrag();
                CancelValueEdit();
                e.Handled = true;
                this.handleMouseUp = true;
                return;
            }

            if (this.isRawTextEditing && e.ClickCount == 3)
            {
                this.textBox.SelectAll();
                e.Handled = true;
                this.handleMouseUp = true;
                return;
            }

            if (this.isRawTextEditing || e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            if (e.ClickCount == 1)
            {
                Focus();
                this.mouseDownPosition = e.GetPosition(this);
                e.MouseDevice.Capture(this);
                e.Handled = true;
                this.handleMouseUp = true;
                return;
            }

            if (e.ClickCount == 2)
            {
                this.textBox.Focus();
                this.textBox.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton) { RoutedEvent = MouseDownEvent });
                e.Handled = true;
                this.handleMouseUp = true;
                return;
            }

            //if (e.ClickCount == 3)
            //{
            //    this.textBox.Focus();
            //    this.textBox.SelectAll();
            //    e.Handled = true;
            //    return;
            //}
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (this.handleMouseUp)
            {
                e.Handled = true;
                this.handleMouseUp = false;
            }

            if (e.MouseDevice.Captured != this)
            {
                return;
            }

            if (this.isDragging)
            {
                CommitValueEdit();
                EndDrag();
                e.Handled = true;
                return;
            }

            Mouse.PrimaryDevice.Capture(null);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.MouseDevice.Captured != this)
            {
                return;
            }

            if (this.isDragging)
            {
                GetCursorPos(out var position);
                var now = Stopwatch.GetTimestamp();

                var distanceX = position.X - this.windowCenterPosition.X;
                var distanceY = position.Y - this.windowCenterPosition.Y;
                var distance = Math.Abs(distanceX) > Math.Abs(distanceY) ? distanceX : -distanceY;

                if (Math.Abs(distance) < 1.0)
                {
                    e.Handled = true;
                    return;
                }

                var time = now - this.dragTime;
                var speed = distance / time;

                var stepFactor = GetStepFactor();
                var step = this.Step * stepFactor;

                this.dragValue += step * speed * this.DragSensitivity * DragSensitivityFactor;

                if (this.scrollWithinBounds)
                {
                    this.dragValue = Math.Clamp(this.dragValue, this.MinValue, this.MaxValue);
                }

                this.dragTime = now;
                SetCursorScreenPosition(this.windowCenterPosition);

                SetValueDisplayDecimals(Math.Min(this.Step, step));

                SetRawValue(GetStepValue(this.dragValue, step, this.MinValue, this.MaxValue));
                SetValue(this.rawValue);
                SetRawText();

                SetTextBoxStyle();
                InvalidateVisual();

                RaiseValueChanged();

                e.Handled = true;
                return;
            }

            if (e.MouseDevice.Captured == this)
            {
                var position = e.GetPosition(this);
                var distance = Math.Abs(position.X - this.mouseDownPosition.X) + Math.Abs(position.Y - this.mouseDownPosition.Y);
                if (distance > DragStartThreadhold)
                {
                    StartDrag();
                    StartRawValueEdit();
                    e.Handled = true;
                    return;
                }
            }
        }

        private void SetValueDisplayDecimals()
        {
            this.ValueDisplayDecimals = Math.Max(GetDecimalSize(this.rawValue, this.RoundDecimals), this.stepDecimalSize);
        }

        private void SetValueDisplayDecimals(double value)
        {
            this.ValueDisplayDecimals = GetDecimalSize(value, this.RoundDecimals);
        }

        private void SetValue(double rawValue)
        {
            var value = Math.Round(rawValue, this.roundDecimals);

            if (this.Value == value)
            {
                return;
            }

            this.Value = value;
            this.valueChanged = true;

            InvalidateVisual();
        }

        private void SetRawValue(double rawValue)
        {
            if (this.rawValue == rawValue)
            {
                return;
            }

            this.rawValue = rawValue;
            this.rawValueChanged = true;
        }

        private void SetRawText()
        {
            SetRawText(Math.Round(this.Value, this.ValueDisplayDecimals).ToString("n" + this.ValueDisplayDecimals), true);
        }

        private void SetRawText(string rawText, bool isValid)
        {
            if (this.textBox.Text == rawText && this.IsRawTextValid == isValid)
            {
                return;
            }

            this.IsRawTextValid = isValid;
            SetTextBoxText(rawText);
            SetTextBoxStyle();
            this.rawValueChanged = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.isRawValueEditing || this.isRawTextEditing)
            {
                if (e.Key == Key.Enter)
                {
                    if (this.IsRawTextValid)
                    {
                        EndDrag();
                        CommitValueEdit();
                    }

                    e.Handled = true;
                }

                if (e.Key == Key.Escape)
                {
                    EndDrag();
                    CancelValueEdit();
                    e.Handled = true;
                }

                return;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetText(this.Value.ToString("f" + GetDecimalSize(this.Value, this.RoundDecimals)));
                System.Windows.Clipboard.Flush();

                e.Handled = true;
                return;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                if (TryParseDoubleValue(System.Windows.Clipboard.GetText(), out var value))
                {
                    this.RawValue = value;
                }

                e.Handled = true;
                return;
            }

            if (!this.IsFocused && !this.textBox.IsFocused)
            {
                return;
            }

            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                this.textBox.Focus();
                this.textBox.CaretIndex = e.Key == Key.Left ? 0 : this.textBox.Text.Length;

                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != 0)
                {
                    this.textBox.SelectAll();
                }

                e.Handled = true;
                return;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {
                this.textBox.Focus();
                this.textBox.SelectAll();
                e.Handled = true;
                return;
            }

            if ((e.KeyboardDevice.Modifiers & ~ModifierKeys.Shift) == 0 &&
                TryGetKeyText(e.Key, e.KeyboardDevice.Modifiers, out var text, out var append))
            {
                this.textBox.Focus();

                if (append)
                {
                    this.textBox.Text += text;
                }
                else
                {
                    this.textBox.Text = text;
                }

                this.textBox.CaretIndex = this.textBox.Text.Length;

                if (Int32.TryParse(text, out var value))
                {
                    SetRawValue(value);
                    SetValue(this.rawValue);
                }

                RaiseValueChanged();

                e.Handled = true;
                return;
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (!this.isRawValueEditing && !this.isRawTextEditing && (this.ScrollModifier == null || PrimaryDeviceInputState.Instance.IsMatch(this.ScrollModifier)))
            {
                var scrollValue = this.RawValue;
                this.scrollWithinBounds = this.MinValue <= this.RawValue && this.RawValue <= this.MaxValue;

                var stepFactor = GetStepFactor();
                var step = this.Step * stepFactor;

                scrollValue += e.Delta > 0 ? step : -step;

                if (this.scrollWithinBounds)
                {
                    scrollValue = Math.Clamp(scrollValue, this.MinValue, this.MaxValue);
                }

                scrollValue = Math.Round(scrollValue / step) * step;

                StartRawValueEdit();

                SetRawValue(scrollValue);
                SetValueDisplayDecimals(Math.Min(this.Step, step));
                SetValue(this.rawValue);
                SetRawText();

                RaiseValueChanged();

                CommitValueEdit();

                InvalidateVisual();
                e.Handled = true;
            }
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.skipTextBoxChangedEvent)
            {
                return;
            }

            this.rawValueChanged = true;

            if (TryParseExpressionValue(this.textBox.Text, out var parsedValue))
            {
                this.IsRawTextValid = true;
                SetRawValue(parsedValue);
                SetValueDisplayDecimals(this.rawValue);
                SetValue(this.rawValue);
                InvalidateVisual();
            }
            else
            {
                this.IsRawTextValid = false;
            }

            SetTextBoxStyle();

            RaiseValueChanged();
        }

        private void SetTextBoxText(string value)
        {
            if (this.textBox.Text == value)
            {
                return;
            }

            this.skipTextBoxChangedEvent = true;

            try
            {
                this.textBox.Text = value;
            }
            finally
            {
                this.skipTextBoxChangedEvent = false;
            }
        }

        private void SetTextBoxStyle()
        {
            this.textBox.CaretBrush = this.textBox.IsFocused ? this.Foreground : Brushes.Transparent;
            this.textBox.Foreground =
                this.isRawTextEditing && !this.IsRawTextValid ? this.InvalidForeground :
                this.isRawTextEditing ? this.EditForeground :
                this.isDragging ? this.DragForeground :
                this.Foreground;
        }

        private void StartDrag()
        {
            this.isDragging = true;

            var now = Stopwatch.GetTimestamp();
            this.scrollWithinBounds = this.MinValue <= this.RawValue && this.RawValue <= this.MaxValue;
            this.dragTime = now;
            this.dragValue = this.rawValue;

            this.dragStartCursorPosition = GetCursorScreenPosition();

            var window = Window.GetWindow(this);
            this.windowCenterPosition = window.PointToScreen(new Point(window.Width / 2.0, window.Height / 2.0));

            SetCursorScreenPosition(this.windowCenterPosition);

            this.Cursor = Cursors.None;

            SetTextBoxStyle();
            InvalidateVisual();
        }

        private void EndDrag()
        {
            if (!this.isDragging)
            {
                return;
            }

            this.isDragging = false;

            Mouse.PrimaryDevice.Capture(null);

            SetCursorScreenPosition(this.dragStartCursorPosition);

            this.Cursor = null;

            SetTextBoxStyle();
            InvalidateVisual();
        }

        private void RaiseValueChanged()
        {
            if (this.rawValueChanged)
            {
                this.rawValueChanged = false;
                RaiseEvent(new RoutedEventArgs(RawValueChangedEvent));
            }

            if (this.valueChanged)
            {
                this.valueChanged = false;
                RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        private double GetStepFactor()
        {
            var smallMatch = this.SmallStepModifier.Match(PrimaryDeviceInputState.Instance);
            var mediumMatch = this.MediumStepModifier.Match(PrimaryDeviceInputState.Instance);
            var largeMatch = this.LargeStepModifier.Match(PrimaryDeviceInputState.Instance);

            return
                largeMatch > mediumMatch && largeMatch > smallMatch ? this.LargeStepFactor :
                mediumMatch > smallMatch ? this.MediumStepFactor :
                smallMatch > 0 ? this.SmallStepFactor : 1.0;
        }

        private bool TryParseExpressionValue(string rawText, out double value)
        {
            if (TryParseDoubleValue(rawText, out value))
            {
                return true;
            }

            if (this.ExpressionParser != null)
            {
                try
                {
                    value = this.ExpressionParser.Parse(rawText);
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        private static bool TryParseDoubleValue(string rawText, out double value)
        {
            return Double.TryParse(rawText, out value) && !rawText.Contains(',');
        }

        private static int GetDecimalSize(double value, int maxDecimalSize)
        {
            if (maxDecimalSize <= 0)
            {
                return 0;
            }

            value = Math.Abs(value);
            var s = (value - Math.Floor(value)).ToString("f" + maxDecimalSize).TrimEnd('0');
            return s != "0." ? s.Length - 2 : value > 0.0 && value < 1.0 ? maxDecimalSize : 0;
        }

        private static double GetStepValue(double value, double step, double minValue, double maxValue)
        {
            if (!Double.IsInfinity(minValue) && !Double.IsInfinity(maxValue) && minValue < maxValue)
            {
                step = Math.Min(step, maxValue - minValue);
            }

            return Math.Round(value / step) * step;
        }

        private static bool TryGetKeyText(Key key, ModifierKeys modifierKeys, [MaybeNullWhen(false)] out string text, out bool append)
        {
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                text = ((char)('0' + key - Key.NumPad0)).ToString();
                append = false;
                return true;
            }

            if (key >= Key.D0 && key <= Key.D9)
            {
                if (modifierKeys == ModifierKeys.None)
                {
                    text = ((char)('0' + key - Key.D0)).ToString();
                    append = false;
                    return true;
                }

                if (modifierKeys == ModifierKeys.Shift)
                {
                    text = ")!@#$%^&*("[key - Key.D0].ToString();
                    append = true;
                    return true;
                }
            }

            if (key >= Key.A && key <= Key.Z)
            {
                if (modifierKeys == ModifierKeys.None)
                {
                    text = ((char)('a' + key - Key.A)).ToString();
                    append = false;
                    return true;
                }

                if (modifierKeys == ModifierKeys.Shift)
                {
                    text = ((char)('A' + key - Key.A)).ToString();
                    append = false;
                    return true;
                }
            }

            if (key == Key.OemMinus || key == Key.Subtract)
            {
                text = "-";
                append = true;
                return true;
            }

            if (key == Key.OemPlus || key == Key.Add)
            {
                text = "+";
                append = true;
                return true;
            }

            if (key == Key.Multiply)
            {
                text = "*";
                append = true;
                return true;
            }

            if (key == Key.OemQuestion || key == Key.Divide)
            {
                text = "/";
                append = true;
                return true;
            }

            if (key == Key.OemPeriod || key == Key.Decimal)
            {
                text = ".";
                append = false;
                return true;
            }

            text = default;
            append = default;
            return false;
        }

        private static Point GetCursorScreenPosition()
        {
            GetCursorPos(out var point);
            return new Point(point.X, point.Y);
        }

        private static void SetCursorScreenPosition(Point point)
        {
            SetCursorPos((int)point.X, (int)point.Y);
        }
    }
}
