namespace Shaderlens.Presentation.Behaviors
{
    public interface IScaleBehavior : IDisposable
    {
        Transform Transform { get; }
        Transform InverseTransform { get; }
        double Scale { get; set; }
        bool IsEnabled { get; set; }
    }

    public class ScaleBehavior : IScaleBehavior
    {
        public Transform Transform { get { return this.transform; } }
        public Transform InverseTransform { get { return this.inverseTransform; } }

        public bool IsEnabled { get; set; }

        private double scale;
        public double Scale
        {
            get { return this.scale; }
            set
            {
                if (value < 0.0001 || value > 10000.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Scale));
                }

                this.scale = value;
                this.transform.ScaleX = this.scale;
                this.transform.ScaleY = this.scale;
                this.inverseTransform.ScaleX = 1.0 / this.scale;
                this.inverseTransform.ScaleY = 1.0 / this.scale;
            }
        }

        private readonly ScaleTransform transform;
        private readonly ScaleTransform inverseTransform;
        private readonly FrameworkElement target;
        private readonly double scaleFactor;
        private readonly double defaultScale;

        private ScaleBehavior(FrameworkElement target, double scaleFactor, double defaultScale)
        {
            this.target = target;
            this.scaleFactor = scaleFactor;
            this.defaultScale = defaultScale;
            this.IsEnabled = true;

            this.transform = new ScaleTransform();
            this.inverseTransform = new ScaleTransform();
            this.Scale = defaultScale;

            this.target.PreviewMouseWheel += OnMouseWheel;
            this.target.PreviewKeyDown += OnKeyDown;
        }

        public void Dispose()
        {
            this.target.PreviewMouseWheel -= OnMouseWheel;
            this.target.PreviewKeyDown -= OnKeyDown;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.IsEnabled && Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Control)
            {
                this.Scale *= e.Delta > 0 ? this.scaleFactor : 1.0 / this.scaleFactor;

                e.Handled = true;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.IsEnabled || Keyboard.PrimaryDevice.Modifiers != ModifierKeys.Control)
            {
                return;
            }

            if (e.Key == Key.D0 || e.Key == Key.NumPad0)
            {
                this.Scale = this.defaultScale;
                e.Handled = true;
            }

            if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                this.Scale *= this.scaleFactor;
                e.Handled = true;
            }

            if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                this.Scale /= this.scaleFactor;
                e.Handled = true;
            }
        }

        public static IScaleBehavior Register(FrameworkElement target)
        {
            return Register(target, 1.1, 1.0);
        }

        public static IScaleBehavior Register(FrameworkElement target, double scaleFactor, double defaultScale)
        {
            return new ScaleBehavior(target, scaleFactor, defaultScale);
        }
    }
}
