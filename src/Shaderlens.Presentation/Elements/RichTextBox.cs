namespace Shaderlens.Presentation.Elements
{
    public class RichTextBoxStyle : IStyle<RichTextBox>
    {
        private readonly ScrollBarStyle scrollBarStyle;
        private readonly IApplicationTheme theme;

        public RichTextBoxStyle(IApplicationTheme theme)
        {
            this.theme = theme;
            this.scrollBarStyle = new ScrollBarStyle(theme.ScrollBar);
        }

        public void Apply(RichTextBox target)
        {
            target.BorderThickness = new Thickness(0);
            target.Cursor = Cursors.Arrow;
            target.SetReference(TextBoxBase.SelectionBrushProperty, this.theme.TextSelectionBackground);
            target.SetReference(TextBoxBase.SelectionOpacityProperty, this.theme.TextSelectionOpacity);

            var scrollBars = target.GetVisualDescendants(5).OfType<ScrollBar>().ToArray();
            foreach (var scrollBar in scrollBars)
            {
                scrollBar.ApplyTemplate();
                this.scrollBarStyle.Apply(scrollBar);
            }
        }
    }

    public class StyledRichTextBox : RichTextBox
    {
        private Transform scrollBarTransform;
        public Transform ScrollBarTransform
        {
            get { return this.scrollBarTransform; }
            set
            {
                this.scrollBarTransform = value;

                if (this.scrollBars != null)
                {
                    foreach (var scrollBar in this.scrollBars)
                    {
                        scrollBar.LayoutTransform = this.scrollBarTransform;
                    }
                }
            }
        }

        private readonly IStyle<RichTextBox> style;
        private ScrollBar[]? scrollBars;

        public StyledRichTextBox(IApplicationTheme theme) :
            this(new RichTextBoxStyle(theme))
        {
        }

        public StyledRichTextBox(IStyle<RichTextBox> style)
        {
            this.style = style;
            this.scrollBarTransform = Transform.Identity;

            this.IsInactiveSelectionHighlightEnabled = true;
            this.IsReadOnly = true;
            this.AutoWordSelection = false;
            this.IsUndoEnabled = false;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);

            this.scrollBars = this.GetVisualDescendants(5).OfType<ScrollBar>().ToArray();

            if (this.scrollBars != null)
            {
                foreach (var scrollBar in this.scrollBars)
                {
                    scrollBar.LayoutTransform = this.scrollBarTransform;
                }
            }

            var contentPresenter = this.GetVisualDescendants(5).OfType<ScrollContentPresenter>().First();
            var flowDocumentView = contentPresenter.GetVisualDescendants(2).OfType<FrameworkElement>().Skip(1).First();
            flowDocumentView.Margin = new Thickness(-5, 0, -5, 0);
        }
    }
}
