namespace Shaderlens.Extensions
{
    public static class InputBindingsExtensions
    {
        public static void AddRenderSpanStart(this IInputBindings inputBindings, IInputSpan? inputSpan, IApplication application, InputSpanEventHandler startAction)
        {
            inputBindings.AddSpan(inputSpan, startAction, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, true);
        }

        public static void AddRenderSpanStart(this IInputBindings inputBindings, IInputSpan? inputSpan, IApplication application, Action startAction)
        {
            inputBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { startAction(); e.Handled = true; } }, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, true);
        }


        public static void AddRenderSpanEnd(this IInputBindings inputBindings, IInputSpan? inputSpan, IApplication application, Action endAction, bool continuous = false)
        {
            inputBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, e => { if (application.IsFullyLoaded) { endAction(); e.Handled = true; } }, continuous);
        }

        public static void AddRenderSpanEnd(this IInputBindings inputBindings, IInputSpan? inputSpan, IApplication application, InputSpanEventHandler endAction, bool continuous = false)
        {
            inputBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { e.Handled = true; } }, endAction, continuous);
        }


        public static void AddRenderSpan(this IInputBindings inputBindings, IInputSpan? inputSpan, IApplication application, Action startAction, Action endAction, bool continuous = false)
        {
            inputBindings.AddSpan(inputSpan, e => { if (application.IsFullyLoaded) { startAction(); e.Handled = true; } }, e => { if (application.IsFullyLoaded) { endAction(); e.Handled = true; } }, continuous);
        }
    }
}
