namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class ColorUniformElement : VisualChildContainer, IRowHeaderContainer
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ColorUniformElement));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ColorUniformElement));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public double HeaderWidth
        {
            get { return this.child.HeaderWidth; }
            set { this.child.HeaderWidth = value; }
        }

        private readonly UniformElement child;
        private readonly ColorView colorViewElement;
        private readonly ISettingsValue<SrgbColor> settingsValue;

        public ColorUniformElement(ISettingsValue<SrgbColor> settingsValue, string displayName, IApplicationTheme theme)
        {
            this.settingsValue = settingsValue;

            var colorViewUnderline = new Border
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
            }.WithReference(Border.BackgroundProperty, theme.TextProgressTrack).WithValue(Grid.ColumnSpanProperty, 2);

            var alphaBackgroundElement = new AlphaBackgroundView().WithValue(Grid.ColumnProperty, 1);

            this.colorViewElement = new ColorView().WithValue(Grid.ColumnSpanProperty, 2);

            var colorViewPanel = new Grid().WithChildren(alphaBackgroundElement, this.colorViewElement, colorViewUnderline);
            colorViewPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            colorViewPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            ClickBehavior.Register(colorViewPanel, (sender, e) => RaiseClickEvent());

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !settingsValue.IsDefaultValue(),
                ValueContent = colorViewPanel
            };

            this.child.ResetValue += (sender, e) =>
            {
                settingsValue.ResetValue();
                this.child.IsResetButtonVisible = false;
                SetColorView();
                RaiseChangedEvent();
            };

            SetColorView();
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        public void InvalidateValue()
        {
            SetColorView();
        }

        private void SetColorView()
        {
            this.colorViewElement.Color = this.settingsValue.Value.ToColor();
            this.child.IsResetButtonVisible = !this.settingsValue.IsDefaultValue();
        }

        private void RaiseChangedEvent()
        {
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        private void RaiseClickEvent()
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}
