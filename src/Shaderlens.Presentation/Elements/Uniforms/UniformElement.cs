namespace Shaderlens.Presentation.Elements.Uniforms
{
    public interface IRowHeaderContainer
    {
        double HeaderWidth { get; set; }
    }

    public class UniformElement : VisualChildContainer, IRowHeaderContainer
    {
        private class ResetValueButton : ImplicitButton
        {
            private const double DrawingSize = 24;
            private static readonly Geometry ResetValueGeometry = Geometry.Parse("M14.394 5.422a7 7 0 0 1 4.44 8.093 7 7 0 0 1-7.444 5.458 7 7 0 0 1-6.383-6.668 7 7 0 0 1 5.777-7.199M14 10V5h5").WithFreeze();

            public static readonly DependencyProperty ForegroundProperty = Icon.ForegroundProperty.AddOwner(typeof(ResetValueButton), new FrameworkPropertyMetadata((sender, e) => ((ResetValueButton)sender).pen.Brush = (Brush)e.NewValue));
            public Brush Foreground
            {
                get { return (Brush)GetValue(ForegroundProperty); }
                set { SetValue(ForegroundProperty, value); }
            }

            private readonly Pen pen;
            private readonly Geometry geometry;

            public ResetValueButton(IApplicationTheme theme) :
                base(theme)
            {
                this.pen = new Pen(this.Foreground, 1.25) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
                this.geometry = ResetValueGeometry;
                this.Width = DrawingSize;
                this.Height = DrawingSize;
                this.CornerRadius = new CornerRadius(4);
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                drawingContext.DrawGeometry(null, this.pen, this.geometry);
            }
        }

        public static readonly RoutedEvent ResetValueEvent = EventManager.RegisterRoutedEvent("ResetValue", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(UniformElement));
        public event RoutedEventHandler ResetValue
        {
            add { AddHandler(ResetValueEvent, value); }
            remove { RemoveHandler(ResetValueEvent, value); }
        }

        public string Header
        {
            get { return this.headerTextBlock.Text; }
            set { this.headerTextBlock.Text = value; }
        }

        public double HeaderWidth
        {
            get { return this.headerContainer.Width; }
            set { this.headerContainer.Width = value; }
        }

        public UIElement ValueContent
        {
            get { return this.valueContentContainer.Child; }
            set { this.valueContentContainer.Child = value; }
        }

        public bool IsResetButtonVisible
        {
            get { return this.resetButton.Visibility == Visibility.Visible; }
            set { this.resetButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        private readonly TextBlock headerTextBlock;
        private readonly ResetValueButton resetButton;
        private readonly Decorator valueContentContainer;
        private readonly DockPanel headerContainer;
        private readonly DockPanel child;

        public UniformElement(IApplicationTheme theme)
        {
            this.headerTextBlock = new TextBlock { TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center };

            this.resetButton = new ResetValueButton(theme) { Margin = new Thickness(4, -8, 4, -8), }.
                WithReference(ResetValueButton.ForegroundProperty, theme.IconForeground).
                WithReference(ImplicitButton.HoverBackgroundProperty, theme.ControlHoveredBackground);

            this.resetButton.PreviewMouseDown += (sender, e) =>
            {
                ClearTextBoxKeyboardFocus();
                RaiseResetValueEvent();
            };

            this.headerContainer = new DockPanel { LastChildFill = true }.WithChildren
            (
                this.resetButton.WithDock(Dock.Right),
                this.headerTextBlock
            );

            this.valueContentContainer = new Decorator();

            this.child = new DockPanel { LastChildFill = true, Focusable = false, Margin = new Thickness(4, 0, 4, 0) }.WithChildren
            (
                this.headerContainer,
                new FrameworkElement { Width = 4 },
                this.valueContentContainer
            );

            this.child.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back && this.child.IsMouseOver)
                {
                    ClearTextBoxKeyboardFocus();
                    RaiseResetValueEvent();
                }
            };
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        protected virtual void OnResetValue(RoutedEventArgs e)
        {
        }

        private void RaiseResetValueEvent()
        {
            var resetValueEvent = new RoutedEventArgs { RoutedEvent = ResetValueEvent };
            OnResetValue(resetValueEvent);
            RaiseEvent(resetValueEvent);
        }

        private static void ClearTextBoxKeyboardFocus()
        {
            if (Keyboard.FocusedElement is TextBox textBox)
            {
                var scope = FocusManager.GetFocusScope(textBox);
                FocusManager.SetFocusedElement(scope, textBox.GetAncestor<FrameworkElement>());
            }
        }
    }
}
