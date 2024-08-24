namespace Shaderlens.Presentation.Elements
{
    public class ButtonStyle : IStyle<StyledButton>
    {
        private readonly IApplicationTheme theme;

        public ButtonStyle(IApplicationTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(StyledButton target)
        {
            target.SetReference(Control.ForegroundProperty, this.theme.WindowForeground);
            target.SetReference(Control.BackgroundProperty, this.theme.WindowBackground);
            target.SetReference(Control.BorderBrushProperty, this.theme.ControlBorder);
            target.SetReference(StyledButton.HoveredBorderBrushProperty, this.theme.ControlHoveredBorder);
            target.SetReference(StyledButton.FocusedBorderBrushProperty, this.theme.ControlFocusedBorder);
            target.SetReference(StyledButton.HoveredBackgroundProperty, this.theme.ControlHoveredBackground);
            target.SetReference(StyledButton.PressedBackgroundProperty, this.theme.ControlPressedBackground);
        }
    }

    public class StyledButton : ButtonBase
    {
        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register("HoveredBackground", typeof(Brush), typeof(StyledButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBackground
        {
            get { return (Brush)GetValue(HoveredBackgroundProperty); }
            set { SetValue(HoveredBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(StyledButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoveredBorderBrushProperty = DependencyProperty.Register("HoveredBorderBrush", typeof(Brush), typeof(StyledButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush HoveredBorderBrush
        {
            get { return (Brush)GetValue(HoveredBorderBrushProperty); }
            set { SetValue(HoveredBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty FocusedBorderBrushProperty = DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(StyledButton), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        private readonly IStyle<StyledButton> style;
        private ContentPresenter? contentPresenter;

        static StyledButton()
        {
            IsEnabledProperty.OverrideMetadata(typeof(StyledButton), new FrameworkPropertyMetadata((d, e) => ((StyledButton)d).OnIsEnabledChanged(e)));
        }

        public StyledButton(IApplicationTheme theme) :
            this(new ButtonStyle(theme))
        {
        }

        public StyledButton(IStyle<StyledButton> style)
        {
            this.style = style;
            this.Height = 28;
            this.Width = 130;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentPresenter = this.GetVisualDescendants(2).OfType<ContentPresenter>().LastOrDefault();
            if (this.contentPresenter != null)
            {
                this.contentPresenter.Opacity = this.IsEnabled ? 1.0 : 0.5;
            }

            this.style.Apply(this);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.contentPresenter != null)
            {
                this.contentPresenter.Measure(constraint);
                var size = this.contentPresenter.DesiredSize;
                var padding = this.Padding;
                size.Width += padding.Left + padding.Right;
                size.Height += padding.Top + padding.Bottom;

                return size;
            }

            return default;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (this.contentPresenter != null)
            {
                var size = this.contentPresenter.DesiredSize;
                this.contentPresenter.Arrange(new Rect((arrangeBounds.Width - size.Width) / 2, (arrangeBounds.Height - size.Height) / 2, size.Width, size.Height));
            }

            return arrangeBounds;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var background =
                this.IsPressed ? this.PressedBackground :
                this.IsMouseDirectlyOver ? this.HoveredBackground :
                this.Background;

            var border = this.IsEnabled && this.IsFocused ? this.FocusedBorderBrush :
                this.IsEnabled && this.IsMouseOver ? this.HoveredBorderBrush :
                this.BorderBrush;

            drawingContext.DrawRoundedRectangle(background, new Pen(border, 1), new Rect(this.RenderSize), 4, 4);
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
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            InvalidateVisual();
        }

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);
            InvalidateVisual();
        }

        private void OnIsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.contentPresenter != null)
            {
                this.contentPresenter.Opacity = (bool)e.NewValue ? 1.0 : 0.5;
            }
        }
    }
}
