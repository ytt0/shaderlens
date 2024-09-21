namespace Shaderlens.Presentation.Input
{
    public interface IInputSettings
    {
        IInputSpan GetOrSetDefault(string name, IInputSpan defaultInputSpan);
        IInputSpanEvent GetOrSetDefault(string name, IInputSpanEvent defaultInputSpanEvent);
    }

    public static class InputSettingsExtensions
    {
        public static IInputSpan GetOrSetDefault(this IInputSettings settings, string name, IInputSpan defaultInputSpan1, IInputSpan defaultInputSpan2, params IInputSpan[] defaultInputSpans)
        {
            return settings.GetOrSetDefault(name, new AnyInputSpan(new[] { defaultInputSpan1, defaultInputSpan2 }.Concat(defaultInputSpans).ToArray()));
        }

        public static IInputSpanEvent GetOrSetDefault(this IInputSettings settings, string name, IInputSpanEvent defaultInputSpanEvent1, IInputSpanEvent defaultInputSpanEvent2, params IInputSpanEvent[] defaultInputSpanEvents)
        {
            return settings.GetOrSetDefault(name, new AnyInputSpanEvent(new[] { defaultInputSpanEvent1, defaultInputSpanEvent2 }.Concat(defaultInputSpanEvents).ToArray()));
        }
    }

    public class InputJsonSettings : IInputSettings
    {
        private readonly InputSpanJsonSerializer inputSpanSerializer;
        private readonly InputSpanEventJsonSerializer inputSpanEventSerializer;
        private readonly IJsonSettings settings;

        public InputJsonSettings(IJsonSettings settings, IInputValueSerializer valueSerializer, IInputSpanFactory factory)
        {
            this.inputSpanSerializer = new InputSpanJsonSerializer(valueSerializer, factory);
            this.inputSpanEventSerializer = new InputSpanEventJsonSerializer(valueSerializer, factory);
            this.settings = settings;
        }

        public IInputSpan GetOrSetDefault(string name, IInputSpan defaultInputSpan)
        {
            if (settings.TryGet(inputSpanSerializer, name, out var inputSpan))
            {
                return inputSpan;
            }

            settings.Set(inputSpanSerializer, name, defaultInputSpan ?? InputSpan.None);
            return defaultInputSpan ?? InputSpan.None;
        }

        public IInputSpanEvent GetOrSetDefault(string name, IInputSpanEvent defaultInputSpanEvent)
        {
            if (settings.TryGet(inputSpanEventSerializer, name, out var inputSpanEvent))
            {
                return inputSpanEvent;
            }

            settings.Set(inputSpanEventSerializer, name, defaultInputSpanEvent ?? InputSpanEvent.None);
            return defaultInputSpanEvent ?? InputSpanEvent.None;
        }
    }
}
