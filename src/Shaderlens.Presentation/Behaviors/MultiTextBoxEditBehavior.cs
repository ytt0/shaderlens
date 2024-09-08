using System.Windows.Controls;

namespace Shaderlens.Presentation.Behaviors
{
    public class MultiNumberTextBoxEditBehavior : IDisposable
    {
        private const int TargetsDepth = 20;

        private readonly FrameworkElement container;
        private readonly RoutedEventHandler rawValueEditStartedHandler;
        private readonly RoutedEventHandler rawTextEditStartedHandler;
        private readonly RoutedEventHandler rawValueChangedHandler;
        private readonly RoutedEventHandler valueEditCommittedHandler;
        private readonly RoutedEventHandler valueEditCanceledHandler;

        private IEnumerable<NumberTextBox>? targets;
        private bool isEditing;
        private Point startPosition;
        private bool isRawValueEdited;
        private bool isRawTextEdited;
        private bool isProcessing;
        private NumberTextBox? editedTarget;

        public MultiNumberTextBoxEditBehavior(FrameworkElement container)
        {
            this.container = container;

            this.rawValueEditStartedHandler = new RoutedEventHandler(OnRawValueEditStarted);
            this.rawTextEditStartedHandler = new RoutedEventHandler(OnRawTextEditStarted);
            this.rawValueChangedHandler = new RoutedEventHandler(OnRawValueChanged);
            this.valueEditCommittedHandler = new RoutedEventHandler(OnValueEditCommitted);
            this.valueEditCanceledHandler = new RoutedEventHandler(OnValueEditCanceled);

            this.container.PreviewMouseDown += OnPreviewMouseDown;
            this.container.MouseDown += OnMouseDown;
            this.container.MouseMove += OnMouseMove;
            this.container.MouseUp += OnMouseUp;
            this.container.KeyDown += OnKeyDown;
            this.container.PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
            this.container.AddHandler(NumberTextBox.RawValueEditStartedEvent, this.rawValueEditStartedHandler);
            this.container.AddHandler(NumberTextBox.RawTextEditStartedEvent, this.rawTextEditStartedHandler);
            this.container.AddHandler(NumberTextBox.RawValueChangedEvent, this.rawValueChangedHandler);
            this.container.AddHandler(NumberTextBox.ValueEditCommittedEvent, this.valueEditCommittedHandler);
            this.container.AddHandler(NumberTextBox.ValueEditCanceledEvent, this.valueEditCanceledHandler);
        }

        public void Dispose()
        {
            this.container.PreviewMouseDown -= OnPreviewMouseDown;
            this.container.MouseDown -= OnMouseDown;
            this.container.MouseMove -= OnMouseMove;
            this.container.MouseUp -= OnMouseUp;
            this.container.KeyDown -= OnKeyDown;
            this.container.PreviewGotKeyboardFocus -= OnPreviewGotKeyboardFocus;
            this.container.RemoveHandler(NumberTextBox.RawValueEditStartedEvent, this.rawValueEditStartedHandler);
            this.container.RemoveHandler(NumberTextBox.RawTextEditStartedEvent, this.rawTextEditStartedHandler);
            this.container.RemoveHandler(NumberTextBox.RawValueChangedEvent, this.rawValueChangedHandler);
            this.container.RemoveHandler(NumberTextBox.ValueEditCommittedEvent, this.valueEditCommittedHandler);
            this.container.RemoveHandler(NumberTextBox.ValueEditCanceledEvent, this.valueEditCanceledHandler);
        }

        private IEnumerable<NumberTextBox> GetTargets()
        {
            if (this.targets == null)
            {
                this.targets = this.container.GetVisualDescendants(TargetsDepth).OfType<NumberTextBox>().ToArray();
            }

            return this.targets;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearTargets();
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.ChangedButton == MouseButton.Left)
            {
                if (this.isRawValueEdited || this.isRawTextEdited)
                {
                    ProcessTargets(null, target => target.CancelValueEdit());
                    this.isRawValueEdited = false;
                    this.isRawTextEdited = false;
                }

                this.isEditing = true;
                this.startPosition = Mouse.PrimaryDevice.GetPosition(this.container);
                e.MouseDevice.Capture(this.container);
                e.Handled = true;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.Captured == this.container)
            {
                var endPosition = Mouse.PrimaryDevice.GetPosition(this.container);
                var selectionRect = new Rect(this.startPosition, endPosition);

                foreach (var target in GetTargets())
                {
                    var targetRect = new Rect(target.TranslatePoint(new Point(), this.container), target.TranslatePoint(new Point(target.RenderSize.Width, target.RenderSize.Height), this.container));
                    target.IsHighlighted = selectionRect.IntersectsWith(targetRect);
                }

                if (this.editedTarget?.IsHighlighted != true)
                {
                    this.editedTarget = GetTargets().FirstOrDefault(target => target.IsHighlighted);
                }

                if (this.editedTarget?.IsFocused == false)
                {
                    this.editedTarget.Focus();
                }

                e.Handled = true;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.Captured == this.container)
            {
                e.MouseDevice.Capture(null);
                e.Handled = true;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (this.isEditing && (e.Key == Key.Enter || e.Key == Key.Escape))
            {
                ClearTargets();

                e.Handled = true;
            }
        }

        private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.container.Dispatcher.InvokeAsync(() =>
            {
                if (!GetTargets().Any(target => target.IsHighlighted && target.IsKeyboardFocusWithin))
                {
                    ClearTargets();
                }
            }, DispatcherPriority.Background);
        }

        private void OnRawValueEditStarted(object sender, RoutedEventArgs e)
        {
            if (this.isProcessing)
            {
                return;
            }

            if (!((NumberTextBox)e.OriginalSource).IsHighlighted)
            {
                ClearTargets();
            }

            this.isRawValueEdited = true;
            this.isRawTextEdited = false;
            ProcessTargets(e.OriginalSource, target => target.StartRawValueEdit());
        }

        private void OnRawTextEditStarted(object sender, RoutedEventArgs e)
        {
            if (this.isProcessing)
            {
                return;
            }

            if (!((NumberTextBox)e.OriginalSource).IsHighlighted)
            {
                ClearTargets();
            }

            this.isRawTextEdited = true;
            this.isRawValueEdited = false;
            ProcessTargets(e.OriginalSource, target => target.StartRawTextEdit());
        }

        private void OnRawValueChanged(object sender, RoutedEventArgs e)
        {
            if (this.isProcessing)
            {
                return;
            }

            var source = (NumberTextBox)e.OriginalSource;

            if (this.isRawValueEdited)
            {
                ProcessTargets(source, target => target.EditValue(source.RawValue, source.ValueDisplayDecimals));
            }

            if (this.isRawTextEdited)
            {
                ProcessTargets(source, target => target.EditValue(source.RawValue, source.ValueDisplayDecimals, source.RawText, source.IsRawTextValid));
            }
        }

        private void OnValueEditCommitted(object sender, RoutedEventArgs e)
        {
            if (this.isProcessing)
            {
                return;
            }

            this.isRawValueEdited = false;
            this.isRawTextEdited = false;
            ProcessTargets(e.OriginalSource, target => target.CommitValueEdit());
        }

        private void OnValueEditCanceled(object sender, RoutedEventArgs e)
        {
            if (this.isProcessing)
            {
                return;
            }

            this.isRawValueEdited = false;
            this.isRawTextEdited = false;
            ProcessTargets(e.OriginalSource, target => target.CancelValueEdit());
        }

        private void ProcessTargets(object? source, Action<NumberTextBox> action)
        {
            if (this.isProcessing)
            {
                return;
            }

            this.isProcessing = true;
            try
            {
                foreach (var target in GetTargets())
                {
                    if (target.IsHighlighted && target != source)
                    {
                        action(target);
                    }
                }
            }
            finally
            {
                this.isProcessing = false;
            }
        }

        private void ClearTargets()
        {
            this.isEditing = false;

            if (this.isRawValueEdited || this.isRawTextEdited)
            {
                ProcessTargets(null, target => target.CommitValueEdit());
                this.isRawValueEdited = false;
                this.isRawTextEdited = false;
            }

            foreach (var target in GetTargets())
            {
                target.IsHighlighted = false;
            }
        }

        public static IDisposable Register(FrameworkElement container)
        {
            return new MultiNumberTextBoxEditBehavior(container);
        }
    }
}
