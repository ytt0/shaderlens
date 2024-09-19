namespace Shaderlens.Presentation.Input
{
    public interface IInputSettings
    {
        IInputSpan GetOrSetDefault(string name, IInputSpan defaultInputSpan);
    }

    public static class InputSettingsExtensions
    {
        public static IInputSpan GetOrSetDefault(this IInputSettings settings, string name, params IInputSpan[] defaultInputSpans)
        {
            var defaultInputSpan =
                defaultInputSpans.Length == 0 ? InputSpan.None :
                defaultInputSpans.Length == 1 ? defaultInputSpans[0] :
                new AnyInputSpan(defaultInputSpans);

            return settings.GetOrSetDefault(name, defaultInputSpan);
        }
    }

    public class InputJsonSettings : IInputSettings
    {
        private readonly IJsonSerializer<IInputSpan> serializer;
        private readonly IJsonSettings settings;

        public InputJsonSettings(IJsonSettings settings, IJsonSerializer<IInputSpan> serializer)
        {
            this.serializer = serializer;
            this.settings = settings;
        }

        public IInputSpan GetOrSetDefault(string name, IInputSpan defaultInputSpan)
        {
            if (settings.TryGet(serializer, name, out var inputSpan))
            {
                return inputSpan;
            }

            settings.Set(serializer, name, defaultInputSpan ?? InputSpan.None);
            return defaultInputSpan ?? InputSpan.None;
        }
    }
}
