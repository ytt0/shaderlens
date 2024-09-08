namespace Shaderlens.Render
{
    public interface IUniformsViewBuilder
    {
        IDisposable AddGroup(ISettingsValue<bool> expandedSettingsValue, string displayName);
        void AddBoolElement(ISettingsValue<bool> settingsValue, string displayName);
        void AddFloatElement(ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step, int roundDecimals);
        void AddVectorElement(ISettingsValue<Vector<bool>> settingsValue, string displayName);
        void AddVectorElement(ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step, int roundDecimals);
        void AddColorElement(ISettingsValue<SrgbColor> settingsValue, bool editAlpha, string name, string displayName);
        void SetSettingsState();
    }

    public static class UniformsViewBuilderExtensions
    {
        private class IntAdapter : ISettingsValue<double>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public double Value
            {
                get { return this.settingsValue.Value; }
                set { this.settingsValue.Value = ToInt(value); }
            }

            public double DefaultValue { get { return this.settingsValue.DefaultValue; } }
            public double BaseValue { get { return this.settingsValue.BaseValue; } }

            private readonly ISettingsValue<int> settingsValue;

            public IntAdapter(ISettingsValue<int> settingsValue)
            {
                this.settingsValue = settingsValue;
            }
        }

        private class UIntAdapter : ISettingsValue<double>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public double Value
            {
                get { return this.settingsValue.Value; }
                set { this.settingsValue.Value = ToUInt(value); }
            }

            public double DefaultValue { get { return this.settingsValue.DefaultValue; } }
            public double BaseValue { get { return this.settingsValue.BaseValue; } }

            private readonly ISettingsValue<uint> settingsValue;

            public UIntAdapter(ISettingsValue<uint> settingsValue)
            {
                this.settingsValue = settingsValue;
            }
        }

        private class IntVectorAdapter : ISettingsValue<Vector<double>>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public Vector<double> Value
            {
                get { return ToFloatVector(this.settingsValue.Value); }
                set { this.settingsValue.Value = ToIntVector(value); }
            }

            public Vector<double> DefaultValue { get; }
            public Vector<double> BaseValue { get; }

            private readonly ISettingsValue<Vector<int>> settingsValue;

            public IntVectorAdapter(ISettingsValue<Vector<int>> settingsValue)
            {
                this.settingsValue = settingsValue;
                this.DefaultValue = ToFloatVector(settingsValue.DefaultValue);
                this.BaseValue = ToFloatVector(settingsValue.BaseValue);
            }
        }

        private class UIntVectorAdapter : ISettingsValue<Vector<double>>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public Vector<double> Value
            {
                get { return ToFloatVector(this.settingsValue.Value); }
                set { this.settingsValue.Value = ToUIntVector(value); }
            }

            public Vector<double> DefaultValue { get; }
            public Vector<double> BaseValue { get; }

            private readonly ISettingsValue<Vector<uint>> settingsValue;

            public UIntVectorAdapter(ISettingsValue<Vector<uint>> settingsValue)
            {
                this.settingsValue = settingsValue;
                this.DefaultValue = ToFloatVector(settingsValue.DefaultValue);
                this.BaseValue = ToFloatVector(settingsValue.BaseValue);
            }
        }

        private class SrgbColorAdapter : ISettingsValue<SrgbColor>
        {
            public string Name { get { return this.settingsValue.Name; } }

            public SrgbColor Value
            {
                get { return this.settingsValue.Value.ToSrgb(); }
                set { this.settingsValue.Value = value.ToLinearRgb(); }
            }

            public SrgbColor DefaultValue { get; }
            public SrgbColor BaseValue { get; }

            private readonly ISettingsValue<LinearRgbColor> settingsValue;

            public SrgbColorAdapter(ISettingsValue<LinearRgbColor> settingsValue)
            {
                this.settingsValue = settingsValue;
                this.DefaultValue = this.settingsValue.DefaultValue.ToSrgb();
                this.BaseValue = this.settingsValue.BaseValue.ToSrgb();
            }
        }

        public static void AddIntElement(this IUniformsViewBuilder builder, ISettingsValue<int> settingsValue, string displayName, int minValue, int maxValue, int step)
        {
            builder.AddFloatElement(new IntAdapter(settingsValue), displayName, minValue, maxValue, step, 0);
        }

        public static void AddUIntElement(this IUniformsViewBuilder builder, ISettingsValue<uint> settingsValue, string displayName, uint minValue, uint maxValue, uint step)
        {
            builder.AddFloatElement(new UIntAdapter(settingsValue), displayName, minValue, maxValue, step, 0);
        }

        public static void AddFloatElement(this IUniformsViewBuilder builder, ISettingsValue<double> settingsValue, string displayName, double minValue, double maxValue, double step)
        {
            builder.AddFloatElement(settingsValue, displayName, minValue, maxValue, step, 6);
        }

        public static void AddVectorElement(this IUniformsViewBuilder builder, ISettingsValue<Vector<double>> settingsValue, string displayName, Vector<double> minValue, Vector<double> maxValue, Vector<double> step)
        {
            builder.AddVectorElement(settingsValue, displayName, minValue, maxValue, step, 6);
        }

        public static void AddVectorElement(this IUniformsViewBuilder builder, ISettingsValue<Vector<int>> settingsValue, string displayName, Vector<int> minValue, Vector<int> maxValue, Vector<int> step)
        {
            builder.AddVectorElement(new IntVectorAdapter(settingsValue), displayName, ToFloatVector(minValue), ToFloatVector(maxValue), ToFloatVector(step), 0);
        }

        public static void AddVectorElement(this IUniformsViewBuilder builder, ISettingsValue<Vector<uint>> settingsValue, string displayName, Vector<uint> minValue, Vector<uint> maxValue, Vector<uint> step)
        {
            builder.AddVectorElement(new UIntVectorAdapter(settingsValue), displayName, ToFloatVector(minValue), ToFloatVector(maxValue), ToFloatVector(step), 0);
        }

        public static void AddColorElement(this IUniformsViewBuilder builder, ISettingsValue<LinearRgbColor> settingsValue, bool editAlpha, string name, string displayName)
        {
            builder.AddColorElement(new SrgbColorAdapter(settingsValue), editAlpha, name, displayName);
        }

        private static Vector<double> ToFloatVector(Vector<int> vector)
        {
            return Vector.Create(vector.Select(value => (double)value));
        }

        private static Vector<int> ToIntVector(Vector<double> vector)
        {
            return Vector.Create(vector.Select(ToInt));
        }

        private static Vector<double> ToFloatVector(Vector<uint> vector)
        {
            return Vector.Create(vector.Select(value => (double)value));
        }

        private static Vector<uint> ToUIntVector(Vector<double> vector)
        {
            return Vector.Create(vector.Select(ToUInt));
        }

        private static int ToInt(double value)
        {
            return (int)Math.Round(value);
        }

        private static uint ToUInt(double value)
        {
            return (uint)Math.Max(0.0, Math.Round(value));
        }
    }
}
