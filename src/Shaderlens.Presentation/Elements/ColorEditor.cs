namespace Shaderlens.Presentation.Elements
{
    public class ColorEditor : FrameworkElement
    {
        private enum TextBoxMode { Okhsv, LinearRgb, Srgb }

        protected override int VisualChildrenCount { get { return this.visualChildren.Length; } }

        public event EventHandler? ColorChanged;
        private OkhsvColor color;
        public OkhsvColor Color
        {
            get { return this.color; }
            set
            {
                this.color = value;
                this.ColorChanged?.Invoke(this, EventArgs.Empty);

                if (!this.skipChangeEvent)
                {
                    try
                    {
                        this.skipChangeEvent = true;

                        this.colorPicker.Color = value;
                        SetTextBoxValues(value);
                        SetColorTextBoxProgressBrush(value);
                        SetTargetColorElement();
                    }
                    finally
                    {
                        this.skipChangeEvent = false;
                    }
                }
            }
        }

        public event EventHandler? AlphaChanged;
        private double alpha;
        public double Alpha
        {
            get { return this.alpha; }
            set
            {
                this.alpha = value;
                this.AlphaChanged?.Invoke(this, EventArgs.Empty);

                if (!this.skipChangeEvent)
                {
                    try
                    {
                        this.skipChangeEvent = true;
                        this.alphaTextBox.Value = value;
                        SetAlphaTextBoxProgressBrush();
                        SetTargetColorElement();
                    }
                    finally
                    {
                        this.skipChangeEvent = false;
                    }
                }
            }
        }

        private OkhsvColor sourceColor;
        public OkhsvColor SourceColor
        {
            get { return this.sourceColor; }
            set
            {
                this.sourceColor = value;
                this.sourceColorElement.Color = ToColor(this.sourceColor, this.IsAlphaVisible ? this.SourceAlpha : 1.0);
            }
        }

        private double sourceAlpha;
        public double SourceAlpha
        {
            get { return this.sourceAlpha; }
            set
            {
                this.sourceAlpha = value;
                this.sourceColorElement.Color = ToColor(this.SourceColor, this.IsAlphaVisible ? value : 1.0);
            }
        }

        private bool isAlphaVisible;
        public bool IsAlphaVisible
        {
            get { return this.isAlphaVisible; }
            set
            {
                this.isAlphaVisible = value;
                this.alphaTextBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                SetSourceColorElement();
                SetTargetColorElement();
                InvalidateArrange();
            }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = ImplicitButton.HoverBackgroundProperty.AddOwner(typeof(ColorEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var colorEditor = (ColorEditor)sender;
            var value = (Brush)e.NewValue;
            colorEditor.colorTextBox1.SetValue(ImplicitButton.HoverBackgroundProperty, value);
            colorEditor.colorTextBox2.SetValue(ImplicitButton.HoverBackgroundProperty, value);
            colorEditor.colorTextBox3.SetValue(ImplicitButton.HoverBackgroundProperty, value);
            colorEditor.alphaTextBox.SetValue(ImplicitButton.HoverBackgroundProperty, value);
        }));

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = ImplicitButton.PressedBackgroundProperty.AddOwner(typeof(ColorEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var colorEditor = (ColorEditor)sender;
            var value = (Brush)e.NewValue;
            colorEditor.colorTextBox1.SetValue(ImplicitButton.PressedBackgroundProperty, value);
            colorEditor.colorTextBox2.SetValue(ImplicitButton.PressedBackgroundProperty, value);
            colorEditor.colorTextBox3.SetValue(ImplicitButton.PressedBackgroundProperty, value);
            colorEditor.alphaTextBox.SetValue(ImplicitButton.PressedBackgroundProperty, value);
        }));

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty EditForegroundProperty = NumberTextBox.EditForegroundProperty.AddOwner(typeof(ColorEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var colorEditor = (ColorEditor)sender;
            var value = (Brush)e.NewValue;
            colorEditor.colorTextBox1.SetValue(NumberTextBox.EditForegroundProperty, value);
            colorEditor.colorTextBox2.SetValue(NumberTextBox.EditForegroundProperty, value);
            colorEditor.colorTextBox3.SetValue(NumberTextBox.EditForegroundProperty, value);
            colorEditor.alphaTextBox.SetValue(NumberTextBox.EditForegroundProperty, value);
        }));

        public Brush EditForeground
        {
            get { return (Brush)GetValue(EditForegroundProperty); }
            set { SetValue(EditForegroundProperty, value); }
        }

        public static readonly DependencyProperty DragForegroundProperty = NumberTextBox.DragForegroundProperty.AddOwner(typeof(ColorEditor), new FrameworkPropertyMetadata((sender, e) =>
        {
            var colorEditor = (ColorEditor)sender;
            var value = (Brush)e.NewValue;
            colorEditor.colorTextBox1.SetValue(NumberTextBox.DragForegroundProperty, value);
            colorEditor.colorTextBox2.SetValue(NumberTextBox.DragForegroundProperty, value);
            colorEditor.colorTextBox3.SetValue(NumberTextBox.DragForegroundProperty, value);
            colorEditor.alphaTextBox.SetValue(NumberTextBox.DragForegroundProperty, value);
        }));

        public Brush DragForeground
        {
            get { return (Brush)GetValue(DragForegroundProperty); }
            set { SetValue(DragForegroundProperty, value); }
        }

        public static readonly DependencyProperty TextFontFamilyProperty = DependencyProperty.Register("TextFontFamily", typeof(FontFamily), typeof(ColorEditor), new FrameworkPropertyMetadata(null, (sender, e) =>
        {
            if (e.NewValue != null)
            {
                var colorEditor = (ColorEditor)sender;
                var value = (FontFamily)e.NewValue;
                colorEditor.colorTextBox1.SetValue(TextElement.FontFamilyProperty, value);
                colorEditor.colorTextBox2.SetValue(TextElement.FontFamilyProperty, value);
                colorEditor.colorTextBox3.SetValue(TextElement.FontFamilyProperty, value);
                colorEditor.alphaTextBox.SetValue(TextElement.FontFamilyProperty, value);
            }
        }));

        public FontFamily TextFontFamily
        {
            get { return (FontFamily)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }

        public double dragSensitivity { get; set; }
        public double DragSensitivity
        {
            get { return this.dragSensitivity; }
            set
            {
                this.dragSensitivity = value;
                this.colorTextBox1.DragSensitivity = value;
                this.colorTextBox2.DragSensitivity = value;
                this.colorTextBox3.DragSensitivity = value;
                this.alphaTextBox.DragSensitivity = value;
            }
        }

        private readonly ColorPicker colorPicker;
        private readonly FrameworkElement valuesPanel;
        private readonly FrameworkElement[] visualChildren;
        private readonly TextBlock modeTextBlock;
        private readonly NumberTextBox colorTextBox1;
        private readonly NumberTextBox colorTextBox2;
        private readonly NumberTextBox colorTextBox3;
        private readonly NumberTextBox alphaTextBox;
        private readonly AlphaBackgroundView alphaBackgroundElement;
        private readonly ColorView sourceColorElement;
        private readonly ColorView targetColorElement;
        private readonly Clipboard clipboard;
        private readonly ColorTextSerializer colorTextSerializer;
        private bool skipChangeEvent;
        private OkhsvColor lastSourceColor;
        private OkhsvColor lastTargetColor;
        private double lastSourceAlpha;
        private double lastTargetAlpha;
        private TextBoxMode textBoxMode;

        public ColorEditor(IApplicationTheme theme)
        {
            this.colorPicker = new ColorPicker();

            this.colorTextBox1 = new NumberTextBox(theme) { StepSize = 0.001, MinValue = 0.0, MaxValue = 1.0 };
            this.colorTextBox2 = new NumberTextBox(theme) { StepSize = 0.001, MinValue = 0.0, MaxValue = 1.0 };
            this.colorTextBox3 = new NumberTextBox(theme) { StepSize = 0.001, MinValue = 0.0, MaxValue = 1.0 };
            this.alphaTextBox = new NumberTextBox(theme) { StepSize = 0.001, MinValue = 0.0, MaxValue = 1.0, Visibility = Visibility.Collapsed };
            this.dragSensitivity = this.colorTextBox1.DragSensitivity;

            this.textBoxMode = TextBoxMode.Okhsv;
            this.modeTextBlock = new TextBlock { Text = "Okhsv", TextAlignment = TextAlignment.Center };

            var modeButton = new ImplicitButton(theme)
            {
                Child = this.modeTextBlock
            };

            modeButton.Click += (sender, e) =>
            {
                ToggleTextBoxMode();
                e.Handled = true;
            };

            this.valuesPanel = CreateStackPanel(Orientation.Vertical, modeButton, this.colorTextBox1, this.colorTextBox2, this.colorTextBox3, this.alphaTextBox, new FrameworkElement { Height = 1 });

            this.alphaBackgroundElement = new AlphaBackgroundView();

            this.sourceColorElement = new ColorView().WithHandler(MouseDownEvent, (sender, e) => { ToggleTargetColor(); e.Handled = true; });
            this.targetColorElement = new ColorView().WithHandler(MouseDownEvent, (sender, e) => { ToggleSourceColor(); e.Handled = true; });

            this.visualChildren = new[]
            {
                this.colorPicker,
                this.valuesPanel,
                this.alphaBackgroundElement,
                this.sourceColorElement,
                this.targetColorElement,
            };

            foreach (var visualChild in this.visualChildren)
            {
                AddVisualChild(visualChild);
            }

            this.colorPicker.ColorChanged += ColorPickerColorChanged;
            this.colorTextBox1.ValueChanged += ColorTextBoxValueChanged;
            this.colorTextBox2.ValueChanged += ColorTextBoxValueChanged;
            this.colorTextBox3.ValueChanged += ColorTextBoxValueChanged;
            this.alphaTextBox.ValueChanged += AlphaTextBoxValueChanged;

            this.clipboard = new Clipboard();
            this.colorTextSerializer = new ColorTextSerializer();

            this.Focusable = true;
            this.FocusVisualStyle = null;

            theme.TextEditForeground.SetReference(this, NumberTextBox.EditForegroundProperty);
            theme.TextDragForeground.SetReference(this, NumberTextBox.DragForegroundProperty);
            theme.ControlPressedBackground.SetReference(this, ImplicitButton.PressedBackgroundProperty);
            theme.ControlHoveredBackground.SetReference(this, ImplicitButton.HoverBackgroundProperty);
            theme.IconForeground.SetReference(this, DockContainer.IconForegroundProperty);
            theme.CodeFontFamily.SetReference(this, TextFontFamilyProperty);
        }

        public void ResetLastColors()
        {
            this.lastTargetColor = this.Color;
            this.lastTargetAlpha = this.Alpha;
            this.lastSourceColor = this.SourceColor;
            this.lastSourceAlpha = this.SourceAlpha;
        }

        private void ToggleTextBoxMode()
        {
            this.textBoxMode = (TextBoxMode)(((int)this.textBoxMode + 1) % Enum.GetValues<TextBoxMode>().Length);
            switch (this.textBoxMode)
            {
                case TextBoxMode.Okhsv: this.modeTextBlock.Text = "Okhsv"; break;
                case TextBoxMode.LinearRgb: this.modeTextBlock.Text = "Linear RGB"; break;
                case TextBoxMode.Srgb: this.modeTextBlock.Text = "sRGB"; break;
                default: throw new NotSupportedException($"Unexpected {nameof(TextBoxMode)} \"{this.textBoxMode}\"");
            }

            try
            {
                this.skipChangeEvent = true;
                SetTextBoxValues(this.Color);
                SetColorTextBoxProgressBrush(this.Color);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void ColorPickerColorChanged(object? sender, EventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;

                var hsv = this.colorPicker.Color.Clamp();

                this.Color = hsv;
                SetTextBoxValues(hsv);
                SetColorTextBoxProgressBrush(hsv);
                SetTargetColorElement();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void ColorTextBoxValueChanged(object? sender, EventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;

                var hsv = GetTextBoxValues();

                this.Color = hsv;
                this.colorPicker.Color = hsv;

                SetColorTextBoxProgressBrush(hsv);
                SetTargetColorElement();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void AlphaTextBoxValueChanged(object? sender, EventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;

                this.Alpha = this.alphaTextBox.Value;

                SetAlphaTextBoxProgressBrush();
                SetTargetColorElement();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private OkhsvColor GetTextBoxValues()
        {
            if (this.textBoxMode == TextBoxMode.Okhsv)
            {
                return new OkhsvColor(this.colorTextBox1.Value, this.colorTextBox2.Value, this.colorTextBox3.Value);
            }

            if (this.textBoxMode == TextBoxMode.LinearRgb)
            {
                return OkhsvColor.FromLinearRgb(new LinearRgbColor(this.colorTextBox1.Value, this.colorTextBox2.Value, this.colorTextBox3.Value));
            }

            if (this.textBoxMode == TextBoxMode.Srgb)
            {
                return OkhsvColor.FromLinearRgb(new SrgbColor(this.colorTextBox1.Value, this.colorTextBox2.Value, this.colorTextBox3.Value).ToLinearRgb());
            }

            throw new NotSupportedException($"Unexpected {nameof(TextBoxMode)} \"{this.textBoxMode}\"");
        }

        private void SetTextBoxValues(OkhsvColor hsv)
        {
            if (this.textBoxMode == TextBoxMode.Okhsv)
            {
                this.colorTextBox1.Value = hsv.H;
                this.colorTextBox2.Value = hsv.S;
                this.colorTextBox3.Value = hsv.V;
                return;
            }

            if (this.textBoxMode == TextBoxMode.LinearRgb)
            {
                var rgb = hsv.ToLinearRgb().Clamp();
                this.colorTextBox1.Value = rgb.R;
                this.colorTextBox2.Value = rgb.G;
                this.colorTextBox3.Value = rgb.B;
                return;
            }

            if (this.textBoxMode == TextBoxMode.Srgb)
            {
                var rgb = hsv.ToLinearRgb().ToSrgb().Clamp();
                this.colorTextBox1.Value = rgb.R;
                this.colorTextBox2.Value = rgb.G;
                this.colorTextBox3.Value = rgb.B;
                return;
            }

            throw new NotSupportedException($"Unexpected {nameof(TextBoxMode)} \"{this.textBoxMode}\"");
        }

        private void SetColorTextBoxProgressBrush(OkhsvColor hsv)
        {
            if (this.textBoxMode == TextBoxMode.Okhsv)
            {
                this.colorTextBox1.ProgressBrush = new SolidColorBrush(ToColor(new OkhsvColor(hsv.H, 1.0, 1.0), 1.0));
                this.colorTextBox2.ProgressBrush = new SolidColorBrush(ToColor(new OkhsvColor(hsv.H, hsv.S, 1.0), 1.0));
                this.colorTextBox3.ProgressBrush = new SolidColorBrush(ToColor(new OkhsvColor(hsv.H, 1.0, hsv.V), 1.0));
            }

            if (this.textBoxMode == TextBoxMode.LinearRgb || this.textBoxMode == TextBoxMode.Srgb)
            {
                this.colorTextBox1.ProgressBrush = Brushes.Red;
                this.colorTextBox2.ProgressBrush = Brushes.Green;
                this.colorTextBox3.ProgressBrush = Brushes.Blue;
            }
        }

        private void SetAlphaTextBoxProgressBrush()
        {
            this.alphaTextBox.ProgressBrush = new SolidColorBrush(ToColor(this.Color, this.Alpha));
        }

        private void SetSourceColorElement()
        {
            this.sourceColorElement.Color = ToColor(this.SourceColor, this.isAlphaVisible ? this.SourceAlpha : 1.0);
        }

        private void SetTargetColorElement()
        {
            this.targetColorElement.Color = ToColor(this.Color, this.IsAlphaVisible ? this.Alpha : 1.0);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visualChildren[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.colorPicker.Measure(availableSize);
            this.valuesPanel.Measure(availableSize);
            this.sourceColorElement.Measure(availableSize);
            this.targetColorElement.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var pickerSize = finalSize.Height;
            var valuesWidth = Math.Max(0, finalSize.Width - pickerSize);

            this.colorPicker.Arrange(new Rect(0, 0, pickerSize, pickerSize));
            this.valuesPanel.Arrange(new Rect(pickerSize, 0, valuesWidth, this.valuesPanel.DesiredSize.Height));

            this.alphaBackgroundElement.Arrange(new Rect(finalSize.Width - valuesWidth / 2.0, this.valuesPanel.ActualHeight, valuesWidth / 2.0, Math.Max(0, finalSize.Height - this.valuesPanel.ActualHeight)));

            this.sourceColorElement.Arrange(new Rect(finalSize.Width - valuesWidth, this.valuesPanel.ActualHeight + (finalSize.Height - this.valuesPanel.ActualHeight) / 2, valuesWidth, Math.Max(0, (finalSize.Height - this.valuesPanel.ActualHeight) / 2)));
            this.targetColorElement.Arrange(new Rect(finalSize.Width - valuesWidth, this.valuesPanel.ActualHeight, valuesWidth, Math.Max(0, (finalSize.Height - this.valuesPanel.RenderSize.Height) / 2)));

            return finalSize;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                this.clipboard.SetText(this.colorTextSerializer.Serialize(this.Color.ToLinearRgb().ToSrgb()));
                e.Handled = true;
            }

            if (e.Key == Key.V && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                if (this.clipboard.TryGetText(out var text) && this.colorTextSerializer.TryDeserialize(text, out var color))
                {
                    this.Color = OkhsvColor.FromLinearRgb(color.ToLinearRgb());
                }

                e.Handled = true;
            }
        }

        private void ToggleSourceColor()
        {
            if (this.Color.Equals(this.SourceColor) && Math.Abs(this.Alpha - this.SourceAlpha) < 0.001)
            {
                this.SourceColor = this.lastSourceColor;
                this.SourceAlpha = this.lastSourceAlpha;
            }
            else
            {
                this.lastSourceColor = this.SourceColor;
                this.lastSourceAlpha = this.SourceAlpha;
                this.SourceColor = this.Color;
                this.SourceAlpha = this.Alpha;
            }
        }

        private void ToggleTargetColor()
        {
            if (this.Color.Equals(this.SourceColor) && Math.Abs(this.Alpha - this.SourceAlpha) < 0.001)
            {
                this.Color = this.lastTargetColor;
                this.Alpha = this.lastTargetAlpha;
            }
            else
            {
                this.lastTargetColor = this.Color;
                this.lastTargetAlpha = this.Alpha;
                this.Color = this.SourceColor;
                this.Alpha = this.SourceAlpha;
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

        private static Color ToColor(OkhsvColor color, double alpha)
        {
            var srgbColor = color.ToLinearRgb().ToSrgb();
            srgbColor.A = alpha;
            return srgbColor.ToColor();
        }
    }
}
