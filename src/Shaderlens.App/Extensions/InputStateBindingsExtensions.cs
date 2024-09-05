namespace Shaderlens.Extensions
{
    public static class InputStateBindingsExtensions
    {
        public static void AddRenderSpanStart(this IInputStateBindings inputStateBindings, IInputSpan? inputSpan, IApplication application, InputSpanEventHandler startAction)
        {
            inputStateBindings.AddSpan(inputSpan, startAction, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, true);
        }

        public static void AddRenderSpanStart(this IInputStateBindings inputStateBindings, IInputSpan? inputSpan, IApplication application, Action startAction)
        {
            inputStateBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { startAction(); e.Handled = true; } }, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, true);
        }


        public static void AddRenderSpanEnd(this IInputStateBindings inputStateBindings, IInputSpan? inputSpan, IApplication application, Action endAction, bool requireSpanStart = false)
        {
            inputStateBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, e => { if (application.IsFullyLoaded) { endAction(); e.Handled = true; } }, requireSpanStart);
        }

        public static void AddRenderSpanEnd(this IInputStateBindings inputStateBindings, IInputSpan? inputSpan, IApplication application, InputSpanEventHandler endAction, bool requireSpanStart = false)
        {
            inputStateBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, endAction, requireSpanStart);
        }


        public static void AddRenderSpan(this IInputStateBindings inputStateBindings, IInputSpan? inputSpan, IApplication application, Action startAction, Action endAction, bool requireSpanStart = false)
        {
            inputStateBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { startAction(); e.Handled = true; } }, e => { if (application.IsFullyLoaded) { endAction(); e.Handled = true; } }, requireSpanStart);
        }
    }
}
