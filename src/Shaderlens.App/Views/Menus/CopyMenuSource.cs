namespace Shaderlens.Views.Menus
{
    public interface ICopyMenuState
    {
        void ResetCopySource(int x, int y);
        ICopySource GetCopySource();
    }

    public class CopyMenuState : ICopyMenuState
    {
        private readonly IApplication application;
        private ICopySource? copySource;
        private int x;
        private int y;

        public CopyMenuState(IApplication application)
        {
            this.application = application;
        }

        public void ResetCopySource(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.copySource = null;
        }

        public ICopySource GetCopySource()
        {
            if (this.copySource == null)
            {
                this.copySource = new CopySource(this.application.GetSelectedBufferTexture(), this.x, this.y);
            }

            return this.copySource;
        }
    }

    public class CopyMenuSource : IMenuSource
    {
        private interface ICopyMenuItem
        {
            void AddTo(IMenuBuilder builder);
        }

        private interface IRepeatCopyMenuItem : ICopyMenuItem
        {
            void SetCopyValue(string copyValue);
        }

        private class CopyMenuItemHeader : VisualChildContainer
        {
            public string Header
            {
                get { return this.headerTextBlock.Text; }
                set { this.headerTextBlock.Text = value; }
            }

            public string? Value
            {
                get { return this.valueTextBlock.Text; }
                set
                {
                    this.valueTextBlock.Text = value;
                    this.valueBorder.Visibility = String.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            protected override int VisualChildrenCount { get { return 1; } }

            private readonly TextBlock headerTextBlock;
            private readonly TextBlock valueTextBlock;
            private readonly Border valueBorder;
            private readonly StackPanel child;

            public CopyMenuItemHeader(IMenuTheme theme)
            {
                this.headerTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
                this.valueTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center }.
                    WithReference(TextBlock.FontFamilyProperty, theme.CodeFontFamily).
                    WithReference(TextBlock.FontSizeProperty, theme.CodeFontSize);

                this.valueBorder = new Border
                {
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(8, 0, 0, 0),
                    Padding = new Thickness(4, 2, 4, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = this.valueTextBlock
                }.WithReference(Border.BorderBrushProperty, theme.ValueBorder);

                this.child = new StackPanel { Orientation = Orientation.Horizontal }.WithChildren(this.headerTextBlock, this.valueBorder);
            }

            protected override FrameworkElement GetChild()
            {
                return this.child;
            }
        }

        private class CopyMenuItem : ICopyMenuItem
        {
            private readonly CopyMenuItemHeader headerElement;
            private readonly IInputSpan? inputSpan;
            private readonly ICopyFormatter formatter;
            private readonly IApplication application;
            private readonly IRepeatCopyMenuItem repeatCopyMenuItem;
            private readonly ICopyMenuState state;
            private string? displayedValue;

            public CopyMenuItem(string header, IInputSpan? inputSpan, ICopyFormatter formatter, IApplication application, ICopyMenuState state, IRepeatCopyMenuItem repeatCopyMenuItem, IMenuTheme theme)
            {
                this.headerElement = new CopyMenuItemHeader(theme) { Header = header };
                this.inputSpan = inputSpan;
                this.formatter = formatter;
                this.application = application;
                this.state = state;
                this.repeatCopyMenuItem = repeatCopyMenuItem;
            }

            public void AddTo(IMenuBuilder builder)
            {
                builder.AddItem(this.headerElement, this.inputSpan, null, null, Copy, SetState);
            }

            private void SetState(IMenuItemState state)
            {
                var selection = this.state.GetCopySource();
                this.displayedValue = selection != null ? this.formatter.FormatString(selection) : String.Empty;

                this.headerElement.Value = this.displayedValue;

                if (this.application.CopySelection?.CopyFormatter == this.formatter)
                {
                    this.repeatCopyMenuItem.SetCopyValue(this.displayedValue);
                }
            }

            private void Copy()
            {
                this.application.CopyValue(this.state.GetCopySource(), this.formatter);
            }
        }

        private class RepeatCopyMenuItem : IRepeatCopyMenuItem
        {
            private readonly CopyMenuItemHeader headerElement;
            private readonly IInputSpan? inputSpan;
            private readonly IApplication application;
            private readonly ICopyMenuState state;
            private string? copyValue;

            public RepeatCopyMenuItem(IInputSpan? inputSpan, IApplication application, ICopyMenuState state, IMenuTheme theme)
            {
                this.headerElement = new CopyMenuItemHeader(theme) { Header = "Copy" };
                this.inputSpan = inputSpan;
                this.application = application;
                this.state = state;
            }

            public void AddTo(IMenuBuilder builder)
            {
                builder.AddItem(this.headerElement, this.inputSpan, null, null, () => this.application.CopyRepeat(this.state.GetCopySource()), SetState);
            }

            private void SetState(IMenuItemState state)
            {
                state.IsVisible = this.application.CopySelection != null;
                this.headerElement.Value = this.application.CopySelection?.CopyFormatter != null ? this.copyValue : null;
                this.headerElement.Header =
                    this.application.CopySelection == CopySelection.CopyFrame ? "Repeat Copy Frame" :
                    this.application.CopySelection == CopySelection.CopyFrameWithAlpha ? "Repeat Copy Frame With Alpha Channel" :
                    "Repeat Copy";
            }

            public void SetCopyValue(string copyValue)
            {
                this.copyValue = copyValue;
            }
        }

        private readonly IApplication application;
        private readonly IApplicationInputs inputs;
        private readonly ICopyMenuState state;
        private readonly IMenuTheme theme;

        public CopyMenuSource(IApplication application, IApplicationInputs inputs, ICopyMenuState state, IMenuTheme theme)
        {
            this.application = application;
            this.state = state;
            this.inputs = inputs;
            this.theme = theme;
        }

        public void AddTo(IMenuBuilder builder)
        {
            if (!this.application.IsFullyLoaded)
            {
                builder.AddEmptyItem();
                return;
            }

            var repeatCopy = new RepeatCopyMenuItem(this.inputs.CopyRepeat, this.application, this.state, this.theme);
            repeatCopy.AddTo(builder);

            builder.AddSeparator();

            foreach (var formatter in this.application.CopyFormatters)
            {
                AddCopyMenuItem(builder, formatter.DisplayName, null, formatter, repeatCopy);
            }

            builder.AddSeparator();

            builder.AddItem("Copy Frame", this.inputs.CopyFrame, null, null, () => this.application.CopyFrame(this.state.GetCopySource(), false));
            builder.AddItem("Copy Frame With Alpha Channel", this.inputs.CopyFrameWithAlpha, null, null, () => this.application.CopyFrame(this.state.GetCopySource(), true));
        }

        private void AddCopyMenuItem(IMenuBuilder builder, string header, IInputSpan? inputSpan, ICopyFormatter formatter, IRepeatCopyMenuItem repeatCopy)
        {
            var item = new CopyMenuItem(header, inputSpan, formatter, this.application, this.state, repeatCopy, this.theme);
            item.AddTo(builder);
        }
    }
}