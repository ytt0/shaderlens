namespace Shaderlens.Presentation.Elements
{
    public class StyledToolTip : ToolTip
    {
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(StyledToolTip));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        protected override int VisualChildrenCount { get { return 1; } }

        private readonly Border border;
        private readonly ContentPresenter contentPresenter;

        public StyledToolTip(IApplicationTheme theme)
        {
            this.contentPresenter = new ContentPresenter();
            this.border = new Border { Child = this.contentPresenter };
            AddVisualChild(this.border);

            theme.WindowForeground.SetReference(this.border, ForegroundProperty);
            theme.WindowBackground.SetReference(this.border, BackgroundProperty);
            theme.ControlBorder.SetReference(this.border, BorderBrushProperty);
            theme.ToolTipOpacity.SetReference(this.border, OpacityProperty);

            this.Template = null;
            this.Padding = new Thickness(8, 4, 8, 4);
            this.CornerRadius = new CornerRadius(4);
            this.BorderThickness = new Thickness(1);
        }

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? this.border : throw new IndexOutOfRangeException();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PaddingProperty)
            {
                this.border.Padding = (Thickness)e.NewValue;
            }

            if (e.Property == BorderThicknessProperty)
            {
                this.border.BorderThickness = (Thickness)e.NewValue;
            }

            if (e.Property == CornerRadiusProperty)
            {
                this.border.CornerRadius = (CornerRadius)e.NewValue;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            this.contentPresenter.Content = newContent;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.border.Measure(constraint);
            return this.border.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.border.Arrange(new Rect(0.0, 0.0, Math.Max(0.0, arrangeBounds.Width), arrangeBounds.Height));
            return arrangeBounds;
        }
    }
}
