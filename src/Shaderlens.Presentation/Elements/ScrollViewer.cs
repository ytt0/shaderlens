namespace Shaderlens.Presentation.Elements
{
    public class ScrollBarStyle : IStyle<ScrollBar>
    {
        private static readonly Lazy<Geometry> UpArrowGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 1,4 3,2 5,4").WithFreeze());
        private static readonly Lazy<Geometry> DownArrowGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 1,2 3,4 5,2").WithFreeze());
        private static readonly Lazy<Geometry> LeftArrowGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 4,1 2,3 4,5").WithFreeze());
        private static readonly Lazy<Geometry> RightArrowGeometry = new Lazy<Geometry>(() => Geometry.Parse("M 2,1 4,3 2,5").WithFreeze());

        private readonly IScrollBarTheme theme;

        public ScrollBarStyle(IScrollBarTheme theme)
        {
            this.theme = theme;
        }

        public void Apply(ScrollBar target)
        {
            var descendants = target.GetVisualDescendants(6).OfType<FrameworkElement>().ToArray();

            var borders = descendants.OfType<Border>().ToArray();
            var repeatButtons = descendants.OfType<RepeatButton>().ToArray();
            var paths = descendants.OfType<Path>().ToArray();
            var rectangles = descendants.OfType<Rectangle>().ToArray();

            target.BorderThickness = new Thickness();

            target.SetReference(Control.BackgroundProperty, this.theme.Track);
            target.SetReference(FrameworkElement.LayoutTransformProperty, this.theme.Scale);

            foreach (var border in borders)
            {
                border.BorderThickness = new Thickness(0);
                border.SetReference(Border.BackgroundProperty, this.theme.Track);
            }

            foreach (var repeatButton in repeatButtons)
            {
                repeatButton.Background = Brushes.Transparent;
            }

            if (paths.Length == 2)
            {
                if (target.Orientation == Orientation.Horizontal)
                {
                    paths[0].Data = LeftArrowGeometry.Value;
                    paths[1].Data = RightArrowGeometry.Value;
                }
                else
                {
                    paths[0].Data = UpArrowGeometry.Value;
                    paths[1].Data = DownArrowGeometry.Value;
                }

                foreach (var path in paths)
                {
                    path.Fill = null;
                    path.StrokeThickness = 1.0;
                    path.StrokeStartLineCap = PenLineCap.Round;
                    path.StrokeEndLineCap = PenLineCap.Round;
                    path.StrokeLineJoin = PenLineJoin.Round;
                    path.Width = 8;
                    path.Height = 8;
                    path.SetReference(Shape.StrokeProperty, this.theme.Arrow);
                }
            }


            foreach (var rectangle in rectangles)
            {
                if (target.Orientation == Orientation.Horizontal)
                {
                    rectangle.Height = 16;
                }
                else
                {
                    rectangle.Width = 16;
                }

                if (rectangle.Fill != Brushes.Transparent)
                {
                    rectangle.RadiusX = 5;
                    rectangle.RadiusY = 5;
                    rectangle.Stroke = Brushes.Transparent;
                    rectangle.StrokeThickness = 6;
                    rectangle.SetReference(Shape.FillProperty, this.theme.Thumb);
                }
            }
        }
    }

    public class ScrollViewerStyle : IStyle<ScrollViewer>
    {
        private readonly IScrollBarTheme theme;
        private readonly ScrollBarStyle scrollBarStyle;

        public ScrollViewerStyle(IScrollBarTheme theme)
        {
            this.theme = theme;
            this.scrollBarStyle = new ScrollBarStyle(theme);
        }

        public void Apply(ScrollViewer target)
        {
            var descendants = target.GetVisualDescendants(3).OfType<FrameworkElement>().ToArray();

            var rectangle = descendants.OfType<Rectangle>().FirstOrDefault();
            var scrollBar = descendants.OfType<ScrollBar>().FirstOrDefault();

            if (scrollBar != null)
            {
                this.scrollBarStyle.Apply(scrollBar);
            }

            rectangle?.SetReference(Shape.FillProperty, this.theme.Track);
        }
    }

    public class StyledScrollViewer : ScrollViewer
    {
        public Transform? scrollBarTransform;
        public Transform? ScrollBarTransform
        {
            get { return this.scrollBarTransform; }
            set
            {
                this.scrollBarTransform = value;

                if (this.scrollBars != null)
                {
                    foreach (var scrollBar in this.scrollBars)
                    {
                        scrollBar.LayoutTransform = this.scrollBarTransform ?? Transform.Identity;
                    }
                }
            }
        }

        private readonly IStyle<ScrollViewer> style;
        private ScrollBar[]? scrollBars;

        public StyledScrollViewer(IScrollBarTheme theme) :
            this(new ScrollViewerStyle(theme))
        {
        }

        public StyledScrollViewer(IStyle<ScrollViewer> style)
        {
            this.style = style;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);

            this.scrollBars = this.GetVisualDescendants(3).OfType<ScrollBar>().ToArray();

            if (this.scrollBars != null)
            {
                foreach (var scrollBar in this.scrollBars)
                {
                    scrollBar.LayoutTransform = this.scrollBarTransform ?? Transform.Identity;
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Keyboard.PrimaryDevice.Modifiers == ModifierKeys.None)
            {
                base.OnMouseWheel(e);
            }
        }
    }
}
