namespace Shaderlens.Presentation.Elements
{
    public class ComboBoxStyle : IStyle<StyledComboBox>
    {
        private static readonly Lazy<Geometry> ExpandGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 3,2 7,6 11,2").WithFreeze());

        private readonly IApplicationTheme theme;
        private readonly ScrollBarStyle scrollBarStyle;

        public ComboBoxStyle(IApplicationTheme theme)
        {
            this.theme = theme;
            this.scrollBarStyle = new ScrollBarStyle(this.theme.ScrollBar);
        }

        public void Apply(StyledComboBox target)
        {
            target.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            target.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            target.SetReference(Control.BorderBrushProperty, this.theme.ControlBorder);
            target.SetReference(StyledComboBox.HoveredBorderBrushProperty, this.theme.ControlHoveredBorder);
            target.SetReference(StyledComboBox.FocusedBorderBrushProperty, this.theme.ControlFocusedBorder);
            target.SetReference(StyledComboBox.HoveredBackgroundProperty, this.theme.ControlHoveredBackground);
            target.SetReference(StyledComboBox.PressedBackgroundProperty, this.theme.ControlPressedBackground);

            var toggleButton = target.GetVisualDescendants(3).OfType<ToggleButton>().FirstOrDefault();
            var path = toggleButton?.GetVisualDescendants(4).OfType<Path>().FirstOrDefault();
            var popup = target.GetVisualDescendants(3).OfType<Popup>().FirstOrDefault();
            var popupBoder = popup?.GetLogicalDescendants(3).OfType<Border>().FirstOrDefault();
            var scrollViewer = popup?.GetLogicalDescendants(4).OfType<ScrollViewer>().FirstOrDefault();
            var scrollViewerContent = scrollViewer?.GetVisualDescendants(2).OfType<Panel>().FirstOrDefault();
            var scrollBars = scrollViewer?.GetVisualDescendants(4).OfType<ScrollBar>().ToArray();
            var canvas = scrollViewerContent?.GetVisualDescendants(5).OfType<Canvas>().FirstOrDefault();

            if (path != null)
            {
                path.Data = ExpandGeometry.Value;
                path.Fill = null;
                path.StrokeThickness = 1.0;
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeEndLineCap = PenLineCap.Round;
                path.StrokeLineJoin = PenLineJoin.Round;
                path.Width = 14;
                path.Height = 8;
                ((FrameworkElement)path.Parent).Width = 24;

                path.SetReference(Shape.StrokeProperty, this.theme.WindowForeground);
            }

            if (scrollViewerContent != null)
            {
                scrollViewerContent.Background = Brushes.Transparent;
            }

            if (scrollBars != null)
            {
                foreach (var scrollBar in scrollBars)
                {
                    this.scrollBarStyle.Apply(scrollBar);
                }
            }

            canvas?.Children.Clear();

            if (popupBoder != null)
            {
                popupBoder.CornerRadius = new CornerRadius(0, 0, 4, 4);
                popupBoder.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            }
        }
    }

    public class StyledComboBox : ComboBox
    {
        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register("HoveredBackground", typeof(Brush), typeof(StyledComboBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBackground
        {
            get { return (Brush)GetValue(HoveredBackgroundProperty); }
            set { SetValue(HoveredBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(StyledComboBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoveredBorderBrushProperty = DependencyProperty.Register("HoveredBorderBrush", typeof(Brush), typeof(StyledComboBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBorderBrush
        {
            get { return (Brush)GetValue(HoveredBorderBrushProperty); }
            set { SetValue(HoveredBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty FocusedBorderBrushProperty = DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(StyledComboBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        private readonly IStyle<StyledComboBox> style;
        private Border? border;
        private FrameworkElement? popupContent;

        public StyledComboBox(IApplicationTheme theme) :
            this(new ComboBoxStyle(theme))
        {
        }

        public StyledComboBox(IStyle<StyledComboBox> style)
        {
            this.style = style;
            this.Height = 28;
            this.VerticalContentAlignment = VerticalAlignment.Center;
            this.Padding = new Thickness(6, 0, 6, 0);
            this.FocusVisualStyle = null;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (this.popupContent != null)
            {
                this.popupContent.MinWidth = sizeInfo.NewSize.Width;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var toggleButton = this.GetVisualDescendants(3).OfType<ToggleButton>().FirstOrDefault();

            this.style.Apply(this);

            this.border = toggleButton?.GetVisualDescendants(2).OfType<Border>().FirstOrDefault();
            SetBorderStyle();

            var popup = this.GetVisualDescendants(3).OfType<Popup>().FirstOrDefault();
            var popupShadow = popup?.Child as Decorator;
            this.popupContent = popupShadow?.Child as FrameworkElement;

            if (this.popupContent != null)
            {
                popupShadow!.Child = null;
                popup!.Child = this.popupContent;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            SetBorderStyle();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            SetBorderStyle();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            SetBorderStyle();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            SetBorderStyle();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            SetBorderStyle();
        }

        private void SetBorderStyle()
        {
            if (this.border != null)
            {
                this.border.Background =
                    this.IsMouseOver && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed ? this.PressedBackground :
                    this.IsMouseOver ? this.HoveredBackground :
                    this.Background;

                this.border.BorderBrush = this.IsEnabled && this.IsFocused ? this.FocusedBorderBrush :
                    this.IsEnabled && this.IsMouseOver ? this.HoveredBorderBrush :
                    this.BorderBrush;

                this.border.CornerRadius = this.IsDropDownOpen ? new CornerRadius(4, 4, 0, 0) : new CornerRadius(4);
            }
        }
    }

    public class ComboBoxItemStyle : IStyle<StyledComboBoxItem>
    {
        private readonly IApplicationTheme theme;

        public ComboBoxItemStyle(IApplicationTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(StyledComboBoxItem target)
        {
            target.SetReference(StyledComboBoxItem.HoveredBackgroundProperty, this.theme.ControlHoveredBackground);
            target.SetReference(StyledComboBoxItem.SelectedBackgroundProperty, this.theme.ControlSelectionBackground);
        }
    }

    public class StyledComboBoxItem : ComboBoxItem
    {
        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register("HoveredBackground", typeof(Brush), typeof(StyledComboBoxItem), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBackground
        {
            get { return (Brush)GetValue(HoveredBackgroundProperty); }
            set { SetValue(HoveredBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SelectedBackgroundProperty = DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(StyledComboBoxItem), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        private readonly IStyle<StyledComboBoxItem> style;
        private Border? border;

        public StyledComboBoxItem(IApplicationTheme theme) :
            this(new ComboBoxItemStyle(theme))
        {
        }

        public StyledComboBoxItem(IStyle<StyledComboBoxItem> style)
        {
            this.style = style;
            this.Padding = new Thickness(4, 4, 4, 4);
            this.BorderThickness = new Thickness();
            this.FocusVisualStyle = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);

            this.border = this.GetVisualDescendants(3).OfType<Border>().FirstOrDefault();
            if (this.border != null && this.Parent is ComboBox comboBox && comboBox.Items.IndexOf(this) == comboBox.Items.Count - 1)
            {
                this.border.CornerRadius = new CornerRadius(0, 0, 4, 4);
            }

            SetBorderStyle();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            SetBorderStyle();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            SetBorderStyle();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            SetBorderStyle();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            SetBorderStyle();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            SetBorderStyle();
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            SetBorderStyle();
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            SetBorderStyle();
        }

        private void SetBorderStyle()
        {
            if (this.border != null)
            {
                this.border.BorderThickness = new Thickness();
                this.border.Background =
                    this.IsSelected ? this.SelectedBackground :
                    this.IsMouseOver || this.IsFocused ? this.HoveredBackground :
                    this.Background;
            }
        }
    }
}
