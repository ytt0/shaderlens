namespace Shaderlens.Presentation.Elements.Uniforms
{
    public class PositionDockUniformElement : VisualChildContainer
    {
        public static readonly RoutedEvent ContextInvalidatedEvent = EventManager.RegisterRoutedEvent("ContextInvalidated", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(PositionDockUniformElement));
        public event RoutedEventHandler ContextInvalidated
        {
            add { AddHandler(ContextInvalidatedEvent, value); }
            remove { RemoveHandler(ContextInvalidatedEvent, value); }
        }

        private readonly PositionEditor positionEditor;
        private IPositionUniformContext? context;
        private bool skipChangeEvent;

        public PositionDockUniformElement(IClipboard clipboard, IPositionGraphInputs inputs, IApplicationTheme theme, double dragSensitivity)
        {
            this.positionEditor = new PositionEditor(clipboard, inputs, theme) { DragSensitivity = dragSensitivity };
            this.positionEditor.ValueChanged += OnEditorChanged;

            this.positionEditor.EditModeChanged += (sender, e) => { if (this.context != null && !this.skipChangeEvent) { this.context.EditMode.Value = this.positionEditor.EditMode; } };
            this.positionEditor.OffsetChanged += (sender, e) => { if (this.context != null && !this.skipChangeEvent) { this.context.ViewOffset.Value = this.positionEditor.Offset; } };
            this.positionEditor.ScaleChanged += (sender, e) => { if (this.context != null && !this.skipChangeEvent) { this.context.ViewScale.Value = this.positionEditor.Scale; } };
            this.positionEditor.ColumnRatioChanged += (sender, e) => { if (this.context != null && !this.skipChangeEvent) { this.context.ColumnRatio.Value = this.positionEditor.ColumnRatio; } };
        }

        protected override FrameworkElement GetChild()
        {
            return this.positionEditor;
        }

        public void SetContext(IPositionUniformContext context, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, int roundDecimals, bool normalizeValue)
        {
            this.skipChangeEvent = true;
            try
            {
                this.context = context;

                var is2D = this.context.Uniform.Value.Count < 3;
                this.positionEditor.Is2D = is2D;
                this.positionEditor.EditMode = is2D ? PositionEditMode.XY : this.context.EditMode.Value;
                this.positionEditor.RoundDecimals = roundDecimals;
                this.positionEditor.Step = step;
                this.positionEditor.NormalizeValue = normalizeValue;
                this.positionEditor.Value = context.Uniform.Value;
                this.positionEditor.SourceValue = context.Uniform.Value;
                this.positionEditor.MinValue = minValue;
                this.positionEditor.MaxValue = maxValue;

                this.positionEditor.ColumnRatio = this.context.ColumnRatio.Value;

                if (this.context.ViewScale.Value > 0)
                {
                    this.positionEditor.Offset = this.context.ViewOffset.Value;
                    this.positionEditor.Scale = this.context.ViewScale.Value;
                }
                else
                {
                    this.positionEditor.ResetView();
                }

                this.positionEditor.ResetLastValues();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        public void InvalidateContext()
        {
            if (this.context != null)
            {
                this.skipChangeEvent = true;
                try
                {
                    this.positionEditor.Value = this.context.Uniform.Value;
                }
                finally
                {
                    this.skipChangeEvent = false;
                }
            }
        }

        private void OnEditorChanged(object? sender, EventArgs e)
        {
            if (!this.skipChangeEvent && this.context != null)
            {
                this.context.Uniform.Value = this.positionEditor.Is2D ? new Vector<double>(this.positionEditor.Value[0], this.positionEditor.Value[1]) : this.positionEditor.Value;
                RaiseEvent(new RoutedEventArgs(ContextInvalidatedEvent));
            }
        }
    }
}
