namespace Shaderlens.Presentation.Elements
{
    public class MenuItemStyle : IStyle<MenuItem>
    {
        private class HighlightBehavior
        {
            public static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached("Behavior", typeof(HighlightBehavior), typeof(HighlightBehavior), new PropertyMetadata(null));
            public static readonly DependencyProperty ResourceValueProperty = DependencyProperty.RegisterAttached("ResourceValue", typeof(Brush), typeof(HighlightBehavior), new PropertyMetadata(null, OnResourceValueChanged));

            private readonly Border target;
            private bool isHighlighted;
            private bool isBorderChanging;

            private HighlightBehavior(Border target, IThemeResource<Brush> highlightBackground)
            {
                this.target = target;
                this.target.SetValue(BehaviorProperty, this);

                var property = DependencyPropertyDescriptor.FromProperty(Border.BorderBrushProperty, typeof(Border));

                this.isBorderChanging = false;
                property.AddValueChanged(target, (sender, e) =>
                {
                    this.isHighlighted = target.BorderBrush != Brushes.Transparent;
                    SetHighlightBackground();
                });

                target.SetReference(ResourceValueProperty, highlightBackground);
            }

            private static void OnResourceValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                ((HighlightBehavior)sender.GetValue(BehaviorProperty)).SetHighlightBackground();
            }

            private void SetHighlightBackground()
            {
                if (this.isHighlighted && !this.isBorderChanging)
                {
                    this.isBorderChanging = true;
                    this.target.SetCurrentValue(Border.BackgroundProperty, (Brush)this.target.GetValue(ResourceValueProperty));
                    this.isBorderChanging = false;
                }
            }

            public static HighlightBehavior Register(Border target, IThemeResource<Brush> highlightBackground)
            {
                return new HighlightBehavior(target, highlightBackground);
            }
        }

        private static readonly Lazy<Geometry> CheckmarkGeometry = new Lazy<Geometry>(() => Geometry.Parse("m21.303 11.758-7.424 7.424L10.697 16a.5.5 0 0 0-.707 0 .5.5 0 0 0 0 .707l3.535 3.535a.5.5 0 0 0 .707 0l7.778-7.777a.5.5 0 0 0 0-.707.5.5 0 0 0-.707 0z").WithFreeze());

        private readonly IMenuTheme theme;
        private readonly MenuContentStyle popupStyle;

        public MenuItemStyle(IMenuTheme theme)
        {
            this.theme = theme;
            this.popupStyle = new MenuContentStyle(theme);
        }

        public void Apply(MenuItem target)
        {
            var hasSubmenu = target is MenuItem menuItem && menuItem.Items.Count > 0;

            var descendants = target.GetVisualDescendants(10).OfType<FrameworkElement>().ToArray();

            var templateRoot = descendants.OfType<Border>().FirstOrDefault(element => element.Name == "templateRoot");
            var popup = descendants.OfType<Popup>().FirstOrDefault(element => element.Name == "PART_Popup");
            var icon = descendants.OfType<ContentPresenter>().FirstOrDefault(element => element.Name == "Icon");
            var glyphPanel = descendants.OfType<Border>().FirstOrDefault(element => element.Name == "GlyphPanel");
            var checkmarkPath = descendants.OfType<Path>().FirstOrDefault(element => element.Name == "Glyph");
            var rightArrowPath = descendants.OfType<Path>().FirstOrDefault(element => element.Name == "RightArrow");
            var grid = descendants.OfType<Grid>().FirstOrDefault(grid => grid.ColumnDefinitions.Count > 2);
            var gestureTextBlock = grid?.Children.OfType<TextBlock>().FirstOrDefault();

            if (templateRoot != null)
            {
                templateRoot.BorderThickness = new Thickness(0);
                templateRoot.CornerRadius = new CornerRadius(6);
                templateRoot.Padding = new Thickness(0, 4, 0, 4);
                HighlightBehavior.Register(templateRoot, this.theme.HighlightBackground);
            }

            if (popup != null)
            {
                popup.VerticalOffset = -6;
                this.popupStyle.Apply(popup);
            }

            if (icon != null)
            {
                icon.Margin = new Thickness(2, 0, 0, 0);
            }

            if (glyphPanel != null)
            {
                glyphPanel.Background = null;
                glyphPanel.BorderThickness = new Thickness(0);
                glyphPanel.Margin = new Thickness(4, 0, 0, 0);
            }

            if (checkmarkPath != null)
            {
                checkmarkPath.SetReference(Shape.FillProperty, this.theme.Checkmark);
                checkmarkPath.Data = CheckmarkGeometry.Value;
                checkmarkPath.HorizontalAlignment = HorizontalAlignment.Center;
                checkmarkPath.VerticalAlignment = VerticalAlignment.Center;
                checkmarkPath.Margin = new Thickness(-4);
                checkmarkPath.Width = 32;
                checkmarkPath.Height = 32;
            }

            rightArrowPath?.SetReference(Shape.FillProperty, this.theme.Arrow);

            if (grid != null)
            {
                grid.ColumnDefinitions[1].Width = new GridLength(8);
                grid.ColumnDefinitions[4].SharedSizeGroup = null;
                grid.Margin = new Thickness(-2);
            }

            if (gestureTextBlock != null)
            {
                gestureTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                gestureTextBlock.SetReference(TextElement.ForegroundProperty, this.theme.GestureForeground);
            }
        }
    }

    public class StyledMenuItem : MenuItem
    {
        private readonly IStyle<MenuItem> style;

        public StyledMenuItem(IMenuTheme theme) :
            this(new MenuItemStyle(theme))
        {
        }

        public StyledMenuItem(IStyle<MenuItem> style)
        {
            this.style = style;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);
        }
    }
}
