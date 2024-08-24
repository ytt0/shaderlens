namespace Shaderlens.Presentation.Elements
{
    public class CheckBoxStyle : IStyle<CheckBox>
    {
        private static readonly Lazy<Geometry> CheckmarkGeometry = new Lazy<Geometry>(() => Geometry.Parse("m 1.25,6.75 4,4 7.5,-7.5").WithFreeze());

        private readonly IApplicationTheme theme;

        public CheckBoxStyle(IApplicationTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(CheckBox target)
        {
            target.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            target.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            target.SetReference(Control.BorderBrushProperty, this.theme.ControlBorder);
            target.SetReference(StyledCheckBox.HoveredBorderBrushProperty, this.theme.ControlHoveredBorder);
            target.SetReference(StyledCheckBox.FocusedBorderBrushProperty, this.theme.ControlFocusedBorder);
            target.SetReference(StyledCheckBox.HoveredBackgroundProperty, this.theme.ControlHoveredBackground);
            target.SetReference(StyledCheckBox.PressedBackgroundProperty, this.theme.ControlPressedBackground);

            var checkBoxBorder = target.GetVisualDescendants(5).OfType<Border>().FirstOrDefault();
            var path = checkBoxBorder?.GetVisualDescendants(3).OfType<Path>().Where(path => path.Name == "optionMark").FirstOrDefault();
            var isEmpty = target.Content == null;

            if (checkBoxBorder != null)
            {
                checkBoxBorder.Padding = new Thickness(2);
                checkBoxBorder.Margin = isEmpty ? new Thickness(0, 2, 0, 2) : new Thickness(0, 2, 4, 2);
                checkBoxBorder.CornerRadius = new CornerRadius(4);

                if (isEmpty)
                {
                    checkBoxBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    checkBoxBorder.SetValue(Grid.ColumnSpanProperty, 2);
                }
            }


            if (path != null)
            {
                path.Data = CheckmarkGeometry.Value;

                path.Fill = null;
                path.StrokeThickness = 1.0;
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeEndLineCap = PenLineCap.Round;
                path.StrokeLineJoin = PenLineJoin.Round;
                path.Width = 14;
                path.Height = 14;
                path.SetReference(Shape.StrokeProperty, this.theme.WindowForeground);
            }
        }
    }

    public class StyledCheckBox : CheckBox
    {
        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register("HoveredBackground", typeof(Brush), typeof(StyledCheckBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBackground
        {
            get { return (Brush)GetValue(HoveredBackgroundProperty); }
            set { SetValue(HoveredBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(StyledCheckBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoveredBorderBrushProperty = DependencyProperty.Register("HoveredBorderBrush", typeof(Brush), typeof(StyledCheckBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBorderBrush
        {
            get { return (Brush)GetValue(HoveredBorderBrushProperty); }
            set { SetValue(HoveredBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty FocusedBorderBrushProperty = DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(StyledCheckBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        private readonly IStyle<CheckBox> style;
        private Border? border;

        public StyledCheckBox(IApplicationTheme theme) :
            this(new CheckBoxStyle(theme))
        {
        }

        public StyledCheckBox(IStyle<CheckBox> style)
        {
            this.style = style;
            this.FocusVisualStyle = null;
            this.VerticalContentAlignment = VerticalAlignment.Center;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);

            this.border = this.GetVisualDescendants(3).OfType<Border>().FirstOrDefault();
            SetBorderStyle();
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

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);
            SetBorderStyle();
        }

        private void SetBorderStyle()
        {
            if (this.border != null)
            {
                this.border.Background =
                    this.IsPressed ? this.PressedBackground :
                    this.IsMouseDirectlyOver ? this.HoveredBackground :
                    this.Background;

                this.border.BorderBrush = this.IsEnabled && this.IsFocused ? this.FocusedBorderBrush :
                    this.IsEnabled && this.IsMouseOver ? this.HoveredBorderBrush :
                    this.BorderBrush;
            }
        }
    }
}
