namespace Shaderlens.Presentation.Elements
{
    public interface ITextBoxMenuResourcesFactory
    {
        FrameworkElement CreateSelectIcon();
        FrameworkElement CreateDeleteIcon();
        FrameworkElement CreateCutIcon();
        FrameworkElement CreatePasteIcon();
        FrameworkElement CreateCopyIcon();
    }

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

    public class TextBoxMenuResourcesFactory : ITextBoxMenuResourcesFactory
    {
        private static readonly Lazy<Geometry> SelectPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M24 17.5v2M22 24a2 2 0 0 0 2-2M8 17.5v2m2 4.5a2 2 0 0 1-2-2m9.5-14h2m4.5 4.5v2m-16-2v2M12.5 8h2M8 10a2 2 0 0 1 2-2m14 2a2 2 0 0 0-2-2m-4.5 16h2m-7 0h2").WithFreeze());
        private static readonly Lazy<Geometry> DeletePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("m21.5 10.5-11 11m0-11 11 11").WithFreeze());
        private static readonly Lazy<Geometry> CutPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("m12.82 19.564 3.08-4.55m3.28 4.55L10.5 6.75m6.776 6.235L21.5 6.75m2.5 15.5a3 3 0 0 1-3 3 3 3 0 0 1-3-3 3 3 0 0 1 3-3 3 3 0 0 1 3 3Zm-10 0a3 3 0 0 1-3 3 3 3 0 0 1-3-3 3 3 0 0 1 3-3 3 3 0 0 1 3 3Z").WithFreeze());
        private static readonly Lazy<Geometry> PastePathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M16.75 12.75h5c1.385 0 2.5 1.115 2.5 2.5v8c0 1.385-1.115 2.5-2.5 2.5h-5a2.495 2.495 0 0 1-2.5-2.5v-8c0-1.385 1.115-2.5 2.5-2.5zm1.8-5h1.2c1.108 0 2 .892 2 2V11M13 25.75H9.75c-1.108 0-2-.892-2-2v-14c0-1.108.892-2 2-2h1.3m1.7-1.5h4c.831 0 1.5.669 1.5 1.5v0c0 .831-.669 1.5-1.5 1.5h-4c-.831 0-1.5-.669-1.5-1.5v0c0-.831.669-1.5 1.5-1.5z").WithFreeze());
        private static readonly Lazy<Geometry> CopyPathGeometry = new Lazy<Geometry>(() => Geometry.Parse("M18 22c0 1.385-1.115 2.5-2.5 2.5h-5A2.495 2.495 0 0 1 8 22v-8c0-1.385 1.115-2.5 2.5-2.5h2m4-4h5c1.385 0 2.5 1.115 2.5 2.5v8c0 1.385-1.115 2.5-2.5 2.5h-5A2.495 2.495 0 0 1 14 18v-8c0-1.385 1.115-2.5 2.5-2.5z").WithFreeze());

        private readonly IMenuTheme theme;

        public TextBoxMenuResourcesFactory(IMenuTheme theme)
        {
            this.theme = theme;
        }

        public FrameworkElement CreateSelectIcon()
        {
            return CreatePath(SelectPathGeometry.Value);
        }

        public FrameworkElement CreateDeleteIcon()
        {
            return CreatePath(DeletePathGeometry.Value);
        }

        public FrameworkElement CreateCutIcon()
        {
            return CreatePath(CutPathGeometry.Value);
        }

        public FrameworkElement CreatePasteIcon()
        {
            return CreatePath(PastePathGeometry.Value);
        }

        public FrameworkElement CreateCopyIcon()
        {
            return CreatePath(CopyPathGeometry.Value);
        }

        private Path CreatePath(Geometry geometry)
        {
            return new Path
            {
                Data = geometry,
                StrokeThickness = 1.25,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-4),
                Width = 32,
                Height = 32,
            }.WithReference(Shape.StrokeProperty, this.theme.IconForeground);
        }
    }
}
