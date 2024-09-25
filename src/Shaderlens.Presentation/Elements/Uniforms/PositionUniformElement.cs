namespace Shaderlens.Presentation.Elements.Uniforms
{
    public interface IPositionUniformContext
    {
        ISettingsValue<Vector<double>> Uniform { get; }
        ISettingsValue<PositionEditMode> EditMode { get; }
        ISettingsValue<Vector<double>> ViewOffset { get; }
        ISettingsValue<double> ViewScale { get; }
        ISettingsValue<double> ColumnRatio { get; }
    }

    public class PositionUniformContext : IPositionUniformContext
    {
        public ISettingsValue<Vector<double>> Uniform { get; }
        public ISettingsValue<PositionEditMode> EditMode { get; }
        public ISettingsValue<Vector<double>> ViewOffset { get; }
        public ISettingsValue<double> ViewScale { get; }
        public ISettingsValue<double> ColumnRatio { get; }

        public PositionUniformContext(ISettingsValue<Vector<double>> uniformValue, ISettingsValue<PositionEditMode> editMode, ISettingsValue<Vector<double>> viewOffset, ISettingsValue<double> viewScale, ISettingsValue<double> columnRatio)
        {
            this.Uniform = uniformValue;
            this.EditMode = editMode;
            this.ViewScale = viewScale;
            this.ViewOffset = viewOffset;
            this.ColumnRatio = columnRatio;
        }
    }

    public class PositionUniformElement : VisualChildContainer, IRowHeaderContainer
    {
        private const int NormalDecimals = 3;

        public static readonly RoutedEvent ContextInvalidatedEvent = EventManager.RegisterRoutedEvent("ContextInvalidated", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(PositionUniformElement));
        public event RoutedEventHandler ContextInvalidated
        {
            add { AddHandler(ContextInvalidatedEvent, value); }
            remove { RemoveHandler(ContextInvalidatedEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(PositionUniformElement));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public Vector<int> ValueDisplayDecimals { get { return new Vector<int>(this.valuesTextBox.Select(textBox => textBox.ValueDisplayDecimals).ToArray()); } }

        public double HeaderWidth
        {
            get { return this.child.HeaderWidth; }
            set { this.child.HeaderWidth = value; }
        }

        private readonly IPositionUniformContext context;
        private readonly bool normalizeValue;
        private readonly bool is2D;

        private readonly UniformElement child;
        private readonly NumberTextBox[] valuesTextBox;
        private readonly PositionView positionView;
        private Vector<double> lastValueSign;
        private double[]? editStartValue;
        private int[]? editStartValueDisplayDecimals;
        private bool skipChangeEvent;

        public PositionUniformElement(IPositionUniformContext context, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, int roundDecimals, bool normalizeValue, double dragSensitivity, IClipboard clipboard, IApplicationTheme theme)
        {
            this.context = context;
            this.normalizeValue = normalizeValue;
            this.is2D = context.Uniform.Value.Count < 3;
            this.lastValueSign = Vector.Create(context.Uniform.Value.Count, 1.0);

            this.valuesTextBox = Enumerable.Range(0, context.Uniform.Value.Count).Select(index => new NumberTextBox(theme)
            {
                MinValue = minValue[index],
                MaxValue = maxValue[index],
                Step = step[index],
                RoundDecimals = roundDecimals,
                DragSensitivity = dragSensitivity,
                ScrollModifier = new ModifierKeyInputSpan(ModifierKey.Alt),
                HorizontalAlignment = HorizontalAlignment.Stretch
            }.
            WithHandler(NumberTextBox.RawValueEditStartedEvent, OnValueEditStarted).
            WithHandler(NumberTextBox.RawTextEditStartedEvent, OnValueEditStarted).
            WithHandler(NumberTextBox.ValueEditCommittedEvent, OnValueEditCommitted).
            WithHandler(NumberTextBox.ValueEditCanceledEvent, OnValueEditCanceled).
            WithHandler(NumberTextBox.ValueChangedEvent, OnValueChanged)).ToArray();

            this.positionView = new PositionView(theme) { Height = 26, Width = 26, Value = context.Uniform.Value, NormalBounds = normalizeValue, Step = step };
            this.positionView.Click += (sender, e) => RaiseClickEvent();

            this.child = new UniformElement(theme)
            {
                Header = displayName,
                IsResetButtonVisible = !context.Uniform.IsDefaultValue(),
                ValueContent = new DockPanel { LastChildFill = true }.WithChildren
                (
                    this.positionView.WithDock(Dock.Right),
                    new ColumnPanel().WithChildren(this.valuesTextBox)
                )
            };

            InvalidateValue();

            UniformElementResetValueBehavior.Register(this.child, context.Uniform, OnResetValue);
            UniformElementClipboardBehavior.Register(this.child, context.Uniform, clipboard, FixedSizeVectorTextSerializer.Create(VectorTextSerializer.Create(ValueTextSerializer.Double), this.context.Uniform.Value.Count), InvalidateValue);
        }

        protected override FrameworkElement GetChild()
        {
            return this.child;
        }

        private void OnValueEditStarted(object sender, RoutedEventArgs e)
        {
            this.editStartValue = this.valuesTextBox.Select(textBox => textBox.Value).ToArray();
            this.editStartValueDisplayDecimals = this.valuesTextBox.Select(textBox => textBox.ValueDisplayDecimals).ToArray();
        }

        private void OnValueEditCanceled(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < this.valuesTextBox.Length; i++)
            {
                this.valuesTextBox[i].EditValue(this.editStartValue![i], this.editStartValueDisplayDecimals![i]);
            }
        }

        private void OnValueEditCommitted(object sender, RoutedEventArgs e)
        {
            if (!this.normalizeValue)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;
                SetTextBoxValues(this.context.Uniform.Value, ((NumberTextBox)sender).ValueDisplayDecimals);
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (this.skipChangeEvent)
            {
                return;
            }

            try
            {
                this.skipChangeEvent = true;

                var values = this.valuesTextBox.Select(textBox => textBox.Value).ToArray();
                var valueDisplayDecimals = ((NumberTextBox)sender).ValueDisplayDecimals;

                if (this.editStartValue == null)
                {
                    this.editStartValue = values.ToArray();
                }

                this.lastValueSign = new Vector<double>(values.Select((v, i) => v != 0.0 ? Math.Sign(v) : this.lastValueSign[i]).ToArray());

                if (this.normalizeValue)
                {
                    values = values.Select(value => Math.Clamp(value, -1.0, 1.0)).ToArray();

                    if (this.is2D)
                    {
                        var editIndex = sender == this.valuesTextBox[0] ? 0 : 1;
                        var nonEditIndex = (editIndex + 1) % 2;

                        values[nonEditIndex] = this.lastValueSign[nonEditIndex] * GetNormalComponent(values[editIndex]);
                        this.valuesTextBox[nonEditIndex].EditValue(values[nonEditIndex], valueDisplayDecimals);
                    }
                    else
                    {
                        var nonEditIndex = this.context.EditMode.Value == PositionEditMode.XY ? 2 : this.context.EditMode.Value == PositionEditMode.XZ ? 1 : 0;
                        var editIndex0 = (nonEditIndex + 1) % 3;
                        var editIndex1 = (nonEditIndex + 2) % 3;

                        if (this.valuesTextBox[nonEditIndex] == sender)
                        {
                            Normalize(values[nonEditIndex], this.editStartValue[editIndex0], this.editStartValue[editIndex1], out values[editIndex0], out values[editIndex1]);
                        }
                        else
                        {
                            values[nonEditIndex] = this.lastValueSign[nonEditIndex] * GetNormalComponent(values[editIndex0], values[editIndex1]);
                        }

                        Normalize(values[0], values[1], values[2], out values[0], out values[1], out values[2]);

                        for (var i = 0; i < this.valuesTextBox.Length; i++)
                        {
                            if (this.valuesTextBox[i] != sender)
                            {
                                this.valuesTextBox[i].EditValue(values[i], valueDisplayDecimals);
                            }
                        }
                    }
                }

                var value = new Vector<double>(values);
                this.context.Uniform.Value = value;
                this.positionView.Value = value;
                this.child.IsResetButtonVisible = !this.context.Uniform.IsDefaultValue();

                RaiseContextInvalidatedEvent();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void OnResetValue()
        {
            InvalidateValue();
            RaiseContextInvalidatedEvent();
        }

        public void InvalidateValue()
        {
            try
            {
                this.skipChangeEvent = true;

                SetTextBoxValues(this.context.Uniform.Value);

                this.positionView.Value = this.context.Uniform.Value;

                this.child.IsResetButtonVisible = !this.context.Uniform.IsDefaultValue();
            }
            finally
            {
                this.skipChangeEvent = false;
            }
        }

        private void SetTextBoxValues(Vector<double> value, int decimals = -1)
        {
            if (decimals < 0 && this.normalizeValue)
            {
                decimals = NormalDecimals;
            }

            for (var i = 0; i < this.valuesTextBox.Length; i++)
            {
                this.valuesTextBox[i].EditValue(value[i], decimals);
            }
        }


        private void RaiseContextInvalidatedEvent()
        {
            RaiseEvent(new RoutedEventArgs(ContextInvalidatedEvent));
        }

        private void RaiseClickEvent()
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        private static double GetNormalComponent(double sourceX)
        {
            return Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX));
        }

        private static double GetNormalComponent(double sourceX, double sourceY)
        {
            return Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX - sourceY * sourceY));
        }

        private static void Normalize(double sourceX, double sourceY, double sourceZ, out double targetY, out double targetZ)
        {
            if (Math.Abs(sourceY) < Math.Abs(sourceZ))
            {
                var a = sourceY / sourceZ;
                targetZ = Math.Sqrt(Math.Max(0.0, (1.0 - sourceX * sourceX) / (1.0 + a * a)));
                targetY = GetNormalComponent(sourceX, targetZ);
                targetY *= Math.Sign(sourceY);
                targetZ *= Math.Sign(sourceZ);
            }
            else if (Math.Abs(sourceZ) < Math.Abs(sourceY))
            {
                var a = sourceZ / sourceY;
                targetY = Math.Sqrt(Math.Max(0.0, (1.0 - sourceX * sourceX) / (1.0 + a * a)));
                targetZ = GetNormalComponent(sourceX, targetY);
                targetY *= Math.Sign(sourceY);
                targetZ *= Math.Sign(sourceZ);
            }
            else
            {
                targetY = Math.Sqrt(Math.Max(0.0, 1.0 - sourceX * sourceX));
                targetZ = 0.0;
            }
        }

        private static void Normalize(double sourceX, double sourceY, double sourceZ, out double targetX, out double targetY, out double targetZ)
        {
            var length = Math.Sqrt(sourceX * sourceX + sourceY * sourceY + sourceZ * sourceZ);
            if (length > 0.0)
            {
                targetX = sourceX / length;
                targetY = sourceY / length;
                targetZ = sourceZ / length;
            }
            else
            {
                targetX = 1.0;
                targetY = 0.0;
                targetZ = 0.0;
            }
        }
    }
}
