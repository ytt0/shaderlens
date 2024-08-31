namespace Shaderlens.Presentation.Elements
{
    using static WinApi;

    public class NumberTextBox : FrameworkElement
    {
        private static readonly TimeSpan MaxClickTime = TimeSpan.FromSeconds(0.2);
        private const double DragSensitivityFactor = 0.05;

        protected override int VisualChildrenCount { get { return 1; } }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NumberTextBox));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private double value;
        public double Value
        {
            get { return this.value; }
            set
            {
                var isChanged = Math.Abs(this.value - value) > Double.Epsilon;

                this.value = Math.Abs(value) < Double.Epsilon ? 0.0 : value;

                if (!this.textBox.IsFocused)
                {
                    this.textBox.Text = this.value.ToString(this.valueFormat);
                }

                if (isChanged)
                {
                    InvalidateVisual();
                    RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                }
            }
        }

        public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxForeground()));
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty EditForegroundProperty = DependencyProperty.Register("EditForeground", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxForeground()));
        public Brush EditForeground
        {
            get { return (Brush)GetValue(EditForegroundProperty); }
            set { SetValue(EditForegroundProperty, value); }
        }

        public static readonly DependencyProperty DragForegroundProperty = DependencyProperty.Register("DragForeground", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.Inherits, (sender, e) => ((NumberTextBox)sender).SetTextBoxForeground()));
        public Brush DragForeground
        {
            get { return (Brush)GetValue(DragForegroundProperty); }
            set { SetValue(DragForegroundProperty, value); }
        }

        public static readonly DependencyProperty ProgressBrushProperty = DependencyProperty.Register("ProgressBrush", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        public Brush ProgressBrush
        {
            get { return (Brush)GetValue(ProgressBrushProperty); }
            set { SetValue(ProgressBrushProperty, value); }
        }

        public static readonly DependencyProperty ProgressTrackBrushProperty = DependencyProperty.Register("ProgressTrackBrush", typeof(Brush), typeof(NumberTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        public Brush ProgressTrackBrush
        {
            get { return (Brush)GetValue(ProgressTrackBrushProperty); }
            set { SetValue(ProgressTrackBrushProperty, value); }
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

        public static readonly DependencyProperty PaddingProperty = Control.PaddingProperty.AddOwner(typeof(NumberTextBox), new FrameworkPropertyMetadata((sender, e) => ((NumberTextBox)sender).textBox.Padding = (Thickness)e.NewValue));
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        private string? valueFormat;
        private double stepSize;
        public double StepSize
        {
            get { return this.stepSize; }
            set
            {
                this.stepSize = value;
                this.valueFormat = "f" + (int)Math.Ceiling(Math.Max(0.0, -Math.Log10(this.StepSize)));
                this.textBox.Text = this.value.ToString(this.valueFormat);
            }
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public double DragSensitivity { get; set; }

        public bool RequireScrollModifierKey { get; set; }

        private readonly TextBox textBox;
        private readonly double progressThickness;
        private POINT startPoint;
        private Point centerPoint;
        private double editStartValue;
        private DateTime dragStartTime;
        private double dragValue;
        private DateTime dragTime;
        private bool dragWithinBounds;

        public NumberTextBox(IApplicationTheme theme)
        {
            this.textBox = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = default,
                TextAlignment = TextAlignment.Center,
                Cursor = Cursors.Arrow,
            };

            this.textBox.TextChanged += (sender, e) =>
            {
                if (IsTextEditing() && Double.TryParse(this.textBox.Text, out var value))
                {
                    this.Value = value;
                }
            };

            this.textBox.GotFocus += (sender, e) =>
            {
                this.textBox.CaretBrush = this.Foreground;
                this.textBox.Foreground = this.EditForeground;

                e.Handled = true;
            };

            this.textBox.LostFocus += (sender, e) =>
            {
                this.textBox.CaretBrush = Brushes.Transparent;
                this.textBox.Foreground = this.Foreground;
                this.textBox.Select(this.textBox.Text.Length, 0);

                this.Value = Double.TryParse(this.textBox.Text, out var textEditValue) ? RoundValue(textEditValue, this.StepSize) : this.editStartValue;

                e.Handled = true;
            };

            AddVisualChild(this.textBox);

            this.ProgressTrackBrush = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            this.ProgressBrush = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0));
            this.progressThickness = 1.0;

            this.StepSize = 0.001;

            this.FocusVisualStyle = null;
            this.Focusable = true;

            this.Value = 0.0;

            this.MinValue = 0.0;
            this.MaxValue = 1.0;

            this.DragSensitivity = 1.0;

            this.Padding = new Thickness(0, 5, 0, 5);

            SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None);

            theme.TextProgressTrack.SetReference(this, ProgressTrackBrushProperty);
            theme.TextEditForeground.SetReference(this, EditForegroundProperty);
            theme.TextDragForeground.SetReference(this, DragForegroundProperty);
            theme.ControlHoveredBackground.SetReference(this, HoverBackgroundProperty);
            theme.ControlPressedBackground.SetReference(this, PressedBackgroundProperty);
            theme.TextSelectionBackground.SetReference(this, SelectionBrushProperty);
            theme.TextSelectionOpacity.SetReference(this, SelectionOpacityProperty);
            theme.CodeFontFamily.SetReference(this, TextElement.FontFamilyProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.textBox.Measure(availableSize);
            return this.textBox.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.textBox.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0: return this.textBox;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var background = IsDragging() ? this.PressedBackground :
                this.IsMouseOver || this.IsFocused ? this.HoverBackground : Brushes.Transparent;

            drawingContext.DrawRectangle(background, null, new Rect(this.RenderSize));
            drawingContext.DrawRectangle(this.ProgressTrackBrush, null, new Rect(0, this.RenderSize.Height - this.progressThickness, this.RenderSize.Width, this.progressThickness));

            if (this.MinValue > Double.MinValue && this.MaxValue < Double.MaxValue &&
                this.MinValue < this.MaxValue && this.MinValue > Double.MinValue && this.MaxValue < Double.MaxValue)
            {
                var progress = Math.Clamp(this.Value / (this.MaxValue - this.MinValue), 0.0, 1.0);
                drawingContext.DrawRectangle(this.ProgressBrush, null, new Rect(0, this.RenderSize.Height - this.progressThickness, this.RenderSize.Width * progress, this.progressThickness));
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            InvalidateVisual();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            InvalidateVisual();

            if (IsDragging())
            {
                DragEnd();
                e.Handled = true;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (Mouse.PrimaryDevice.Captured == null)
            {
                InvalidateVisual();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Mouse.PrimaryDevice.Captured == null)
            {
                InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (IsDragging())
            {
                if (e.ChangedButton != MouseButton.Left)
                {
                    RevertDrag();
                }
                else if (DateTime.Now - this.dragStartTime < MaxClickTime)
                {
                    RevertDrag();
                    this.textBox.Focus();
                    this.textBox.SelectAll();
                }
                else if (Math.Abs(this.editStartValue - this.Value) < Double.Epsilon)
                {
                    RevertDrag();
                    this.textBox.Focus();
                    this.textBox.Select(this.textBox.Text.Length, 0);
                }

                DragEnd();
                e.Handled = true;
            }
            else if (!IsTextEditing() && e.LeftButton == MouseButtonState.Pressed)
            {
                DragStart();
                e.Handled = true;
            }

            InvalidateVisual();
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (IsDragging() && DateTime.Now - this.dragStartTime > MaxClickTime)
            {
                DragEnd();
                e.Handled = true;
            }
            else if (!IsTextEditing())
            {
                RevertDrag();
                this.textBox.Focus();
                this.textBox.SelectAll();
                e.Handled = true;
            }

            InvalidateVisual();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsDragging())
            {
                var now = DateTime.Now;
                GetCursorPos(out var point);

                var distanceX = point.X - this.centerPoint.X;
                var distanceY = point.Y - this.centerPoint.Y;
                var distance = Math.Abs(distanceX) > Math.Abs(distanceY) ? distanceX : -distanceY;
                var time = (now - this.dragTime).TotalMilliseconds;
                var speed = distance / time;
                var stepModifier =
                        Keyboard.PrimaryDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) ? 100.0 :
                        Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control ? 10.0 :
                        Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Shift ? 0.1 : 1.0;

                var step = this.StepSize * stepModifier * speed * this.DragSensitivity * DragSensitivityFactor;
                this.dragValue += step;

                if (this.dragWithinBounds)
                {
                    this.dragValue = Math.Clamp(this.dragValue, this.MinValue, this.MaxValue);
                }

                this.Value = RoundValue(this.dragValue, this.StepSize * Math.Max(1.0, stepModifier));

                this.dragTime = now;
                SetCursorPos((int)this.centerPoint.X, (int)this.centerPoint.Y);

                if (Math.Abs(distance) > 0)
                {
                    this.textBox.Foreground = this.DragForeground;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (IsDragging())
            {
                if (e.SystemKey != Key.None ||
                    e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                    e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                    e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    return;
                }

                if (e.Key == Key.Escape)
                {
                    RevertDrag();
                    DragEnd();
                    e.Handled = true;
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                    e.Key >= Key.D0 && e.Key <= Key.D9 ||
                    e.Key == Key.OemMinus || e.Key == Key.Subtract ||
                    e.Key == Key.OemPeriod || e.Key == Key.Decimal)
                {
                    DragEnd();
                    TextEditStart(true);
                }
            }
            else if (IsTextEditing())
            {
                if (e.Key == Key.Enter)
                {
                    TextEditEnd();

                    this.Value = Double.TryParse(this.textBox.Text, out var textEditValue) ? RoundValue(textEditValue, this.StepSize) : this.editStartValue;

                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    TextEditEnd();

                    this.Value = this.editStartValue;

                    e.Handled = true;
                }
            }
            else if (e.Key == Key.C && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control && this.IsMouseOver)
            {
                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetText(this.textBox.Text);
                e.Handled = true;
            }
            else if (e.Key == Key.V && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control && this.IsMouseOver)
            {
                var clipboardText = System.Windows.Clipboard.GetText();
                if (Double.TryParse(clipboardText, out var clipboardValue))
                {
                    this.Value = RoundValue(clipboardValue, this.StepSize);
                }
                e.Handled = true;
            }
            else if (this.IsFocused && Mouse.PrimaryDevice.Captured is null)
            {
                if (e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Home || e.Key == Key.End)
                {
                    TextEditStart(false);
                    e.Handled = true;
                }

                if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                    e.Key >= Key.D0 && e.Key <= Key.D9 ||
                    e.Key == Key.OemMinus || e.Key == Key.Subtract ||
                    e.Key == Key.OemPeriod || e.Key == Key.Decimal)
                {
                    TextEditStart(true);
                    e.Handled = false;
                }
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (this.RequireScrollModifierKey && (Keyboard.PrimaryDevice.Modifiers & ModifierKeys.Alt) == 0)
            {
                return;
            }

            var stepModifier =
                    (Keyboard.PrimaryDevice.Modifiers & ModifierKeys.Control) != 0 ? 100.0 :
                    (Keyboard.PrimaryDevice.Modifiers & ModifierKeys.Shift) != 0 ? 10.0 : 1.0;

            var step = Math.Sign(e.Delta) * this.StepSize * stepModifier;
            var scrollValue = this.Value + step;

            if (this.MinValue <= this.Value && this.Value <= this.MaxValue)
            {
                scrollValue = Math.Clamp(scrollValue, this.MinValue, this.MaxValue);
            }

            this.Value = RoundValue(scrollValue, this.StepSize * Math.Max(1.0, stepModifier));

            e.Handled = true;
        }

        private void TextEditStart(bool clear)
        {
            this.textBox.Focus();

            if (clear)
            {
                this.textBox.Text = "";
            }
        }

        private void TextEditEnd()
        {
            Focus();
        }

        private bool IsTextEditing()
        {
            return this.textBox.IsFocused;
        }

        private bool IsDragging()
        {
            return Mouse.PrimaryDevice.Captured == this;
        }

        private void DragStart()
        {
            GetCursorPos(out this.startPoint);

            var window = Window.GetWindow(this);
            this.centerPoint = window.PointToScreen(new Point(window.Width / 2.0, window.Height / 2.0));

            SetCursorPos((int)this.centerPoint.X, (int)this.centerPoint.Y);
            var now = DateTime.Now;
            this.dragStartTime = now;
            this.editStartValue = this.Value;
            this.dragWithinBounds = this.MinValue <= this.Value && this.Value <= this.MaxValue;
            this.dragTime = now;
            this.dragValue = this.Value;
            this.Cursor = Cursors.None;
            InvalidateVisual();
            Focus();

            Mouse.PrimaryDevice.Capture(this);
        }

        private void DragEnd()
        {
            SetCursorPos(this.startPoint.X, this.startPoint.Y);
            this.Cursor = null;
            this.textBox.Foreground = this.Foreground;
            InvalidateVisual();
            Mouse.PrimaryDevice.Capture(null);
        }

        private void RevertDrag()
        {
            this.Value = this.editStartValue;
        }

        private void SetTextBoxForeground()
        {
            this.textBox.Foreground = this.textBox.IsFocused ? this.EditForeground : IsDragging() ? this.DragForeground : this.Foreground;
        }

        private static double RoundValue(double value, double stepSize)
        {
            return Math.Round(value / stepSize) * stepSize;
        }
    }
}
