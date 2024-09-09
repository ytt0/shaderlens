namespace Shaderlens.Presentation.Elements
{
    public class DockContainer : Decorator
    {
        private const double BorderCaptureThickness = 10;
        private const double BorderThickness = 1;

        private class CloseButton : ImplicitButton
        {
            public static readonly DependencyProperty IconForegroundProperty = Icon.ForegroundProperty.AddOwner(typeof(CloseButton), new FrameworkPropertyMetadata((sender, e) => ((CloseButton)sender).pen.Brush = (Brush)e.NewValue));
            public Brush IconForeground
            {
                get { return (Brush)GetValue(IconForegroundProperty); }
                set { SetValue(IconForegroundProperty, value); }
            }

            private const double DrawingSize = 8;

            private readonly Pen pen;

            public CloseButton(IApplicationTheme theme) :
                base(theme)
            {
                this.pen = new Pen(theme.IconForeground.Value, 1);
                theme.IconForeground.SetReference(this, Icon.ForegroundProperty);
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                var drawingRect = new Rect((this.RenderSize.Width - DrawingSize) / 2, (this.RenderSize.Height - DrawingSize) / 2, DrawingSize, DrawingSize);

                drawingContext.DrawLine(this.pen, drawingRect.TopLeft, drawingRect.BottomRight);
                drawingContext.DrawLine(this.pen, drawingRect.TopRight, drawingRect.BottomLeft);
            }
        }

        private class ResizeBorder : FrameworkElement
        {
            public static readonly DependencyProperty HoverBorderBrushProperty = DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(ResizeBorder), new PropertyMetadata());
            public Brush HoverBorderBrush
            {
                get { return (Brush)GetValue(HoverBorderBrushProperty); }
                set { SetValue(HoverBorderBrushProperty, value); }
            }

            public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof(ResizeBorder));
            public Brush BorderBrush
            {
                get { return (Brush)GetValue(BorderBrushProperty); }
                set { SetValue(BorderBrushProperty, value); }
            }

            private readonly FrameworkElement target;
            private Point resizeStartPosition;
            private double resizeStartSize;

            public ResizeBorder(FrameworkElement target, IApplicationTheme theme)
            {
                this.target = target;
                theme.ControlBorder.SetReference(this, BorderBrushProperty);
                theme.ControlHoveredBorder.SetReference(this, HoverBorderBrushProperty);
            }

            protected override void OnQueryCursor(QueryCursorEventArgs e)
            {
                e.Cursor = Cursors.SizeNS;
                e.Handled = true;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, -BorderCaptureThickness / 2, this.RenderSize.Width, BorderCaptureThickness));
                drawingContext.DrawRectangle(this.IsMouseOver ? this.HoverBorderBrush : this.BorderBrush, null, new Rect(0, -BorderThickness / 2, this.RenderSize.Width, BorderThickness));
            }

            protected override void OnMouseEnter(MouseEventArgs e)
            {
                InvalidateVisual();
            }

            protected override void OnMouseLeave(MouseEventArgs e)
            {
                InvalidateVisual();
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.Key == Key.Escape && IsResizing())
                {
                    CancelResize();
                    e.Handled = true;
                }
            }

            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    StartResize();
                    e.Handled = true;
                }
                else if (IsResizing())
                {
                    CancelResize();
                    e.Handled = true;
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (IsResizing())
                {
                    var offset = PointFromScreen(this.resizeStartPosition).Y - Mouse.PrimaryDevice.GetPosition(this).Y;

                    this.target.Height = Math.Max(this.target.MinHeight, this.resizeStartSize + offset);
                    e.Handled = true;
                }
            }

            protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
            {
                if (IsResizing())
                {
                    EndResize();
                    e.Handled = true;
                }
            }

            private bool IsResizing()
            {
                return Mouse.PrimaryDevice.Captured == this;
            }

            private void StartResize()
            {
                Mouse.PrimaryDevice.Capture(this);
                this.resizeStartPosition = PointToScreen(Mouse.PrimaryDevice.GetPosition(this));
                this.resizeStartSize = this.target.Height;
            }

            private void CancelResize()
            {
                this.target.Height = this.resizeStartSize;
                Mouse.PrimaryDevice.Capture(null);
            }

            private static void EndResize()
            {
                Mouse.PrimaryDevice.Capture(null);
            }
        }

        public event EventHandler? Closed;
        public event EventHandler? Resized;

        protected override int VisualChildrenCount { get { return (this.Child != null ? 1 : 0) + 2; } }

        private readonly CloseButton closeButton;
        private readonly ResizeBorder resizeBorder;
        private readonly FrameworkElement[] visualChildren;

        public DockContainer(IApplicationTheme theme)
        {
            this.Height = 200;
            this.MinHeight = 200;

            this.closeButton = new CloseButton(theme) { Width = 28, Height = 28 };
            this.closeButton.Click += (sender, e) =>
            {
                this.Visibility = Visibility.Collapsed;
                this.Closed?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            };

            this.resizeBorder = new ResizeBorder(this, theme);
            this.resizeBorder.SizeChanged += (sender, e) => this.Resized?.Invoke(this, EventArgs.Empty);

            AddVisualChild(this.resizeBorder);
            AddVisualChild(this.closeButton);

            this.visualChildren = new FrameworkElement[] { this.resizeBorder, this.closeButton };
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.Child == null ? this.visualChildren[index] :
                index == 0 ? this.Child : this.visualChildren[index - 1];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);

            this.closeButton.Measure(constraint);
            this.resizeBorder.Measure(constraint);

            return this.resizeBorder.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            this.closeButton.Arrange(new Rect(arrangeSize.Width - this.closeButton.DesiredSize.Width, BorderThickness / 2.0, this.closeButton.DesiredSize.Width, this.closeButton.DesiredSize.Height));
            this.resizeBorder.Arrange(new Rect(arrangeSize));

            return arrangeSize;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }
    }
}
