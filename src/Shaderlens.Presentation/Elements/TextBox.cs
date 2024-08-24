namespace Shaderlens.Presentation.Elements
{
    public class TextBoxStyle : IStyle<TextBox>
    {
        private readonly IApplicationTheme theme;

        public TextBoxStyle(IApplicationTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(TextBox target)
        {
            target.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            target.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            target.SetReference(Control.BorderBrushProperty, this.theme.ControlBorder);
            target.SetReference(StyledTextBox.HoveredBorderBrushProperty, this.theme.ControlHoveredBorder);
            target.SetReference(StyledTextBox.FocusedBorderBrushProperty, this.theme.ControlFocusedBorder);
            target.SetReference(TextBoxBase.SelectionOpacityProperty, this.theme.TextSelectionOpacity);
            target.SetReference(TextBoxBase.SelectionBrushProperty, this.theme.TextSelectionBackground);
        }
    }

    public class StyledTextBox : TextBox
    {
        public static readonly DependencyProperty HoveredBorderBrushProperty = DependencyProperty.Register("HoveredBorderBrush", typeof(Brush), typeof(StyledTextBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBorderBrush
        {
            get { return (Brush)GetValue(HoveredBorderBrushProperty); }
            set { SetValue(HoveredBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty FocusedBorderBrushProperty = DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(StyledTextBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        private readonly IStyle<TextBox> style;
        private Border? border;

        public StyledTextBox(IApplicationTheme theme) :
            this(new TextBoxStyle(theme))
        {
        }

        public StyledTextBox(IStyle<TextBox> style)
        {
            this.style = style;
            this.Height = 28;
            this.VerticalContentAlignment = VerticalAlignment.Center;
            this.Padding = new Thickness(3, 0, 3, 0);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.border = this.GetVisualDescendants(2).OfType<Border>().FirstOrDefault();
            if (this.border != null)
            {
                this.border.CornerRadius = new CornerRadius(4);
            }

            this.style.Apply(this);
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
                this.border.BorderBrush = this.IsFocused ? this.FocusedBorderBrush : this.IsMouseOver ? this.HoveredBorderBrush : this.BorderBrush;
            }
        }
    }
}
