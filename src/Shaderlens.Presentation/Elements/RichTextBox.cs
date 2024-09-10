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
        private readonly IStyle<RichTextBox> style;

        public StyledRichTextBox(IApplicationTheme theme) :
            this(new RichTextBoxStyle(theme), new ContextMenuStyle(theme.Menu), theme.Menu)
        {
        }

        public StyledRichTextBox(IStyle<RichTextBox> style, IStyle<ContextMenu> menuStyle, IMenuTheme menuTheme)
        {
            this.style = style;
            this.IsInactiveSelectionHighlightEnabled = true;
            this.IsReadOnly = true;
            this.AutoWordSelection = false;
            this.IsUndoEnabled = false;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.ContextMenu = new StyledRichTextBoxMenu(this, menuTheme, menuStyle);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.style.Apply(this);

            var contentPresenter = this.GetVisualDescendants(5).OfType<ScrollContentPresenter>().First();
            var flowDocumentView = contentPresenter.GetVisualDescendants(2).OfType<FrameworkElement>().Skip(1).First();
            flowDocumentView.Margin = new Thickness(-5, 0, -5, 0);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right || this.Selection?.IsEmpty == false)
            {
                base.OnMouseDown(e);
            }
        }
    }

    public class StyledRichTextBoxMenu : StyledContextMenu
    {
        private readonly RichTextBox target;
        private readonly IMenuTheme theme;
        private bool isInitialized;

        public StyledRichTextBoxMenu(RichTextBox target, IMenuTheme theme) :
            this(target, theme, new ContextMenuStyle(theme))
        {
        }

        public StyledRichTextBoxMenu(RichTextBox target, IMenuTheme theme, IStyle<ContextMenu> style) :
            base(style)
        {
            this.target = target;
            this.theme = theme;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!this.isInitialized)
            {
                this.isInitialized = true;

                var resources = new TextBoxMenuResourcesFactory(this.theme);
                var menuBuilder = new MenuBuilder(this, this.theme);
                menuBuilder.AddItem("Copy", new AllInputSpans(new IInputSpan[] { new ModifierKeyInputSpan(ModifierKey.Ctrl), new KeyInputSpan(Key.C) }), resources.CreateCopyIcon(), null, this.target.Copy, state => state.IsEnabled = this.target.Selection?.Text?.Length > 0);
                menuBuilder.AddSeparator();
                menuBuilder.AddItem("Select All", new AllInputSpans(new IInputSpan[] { new ModifierKeyInputSpan(ModifierKey.Ctrl), new KeyInputSpan(Key.A) }), resources.CreateSelectIcon(), null, this.target.SelectAll, state => state.IsEnabled = this.target.Document.Blocks.Sum(block => (block as Paragraph)?.Inlines.Count ?? 0) > 0);
            }
        }
    }
}
