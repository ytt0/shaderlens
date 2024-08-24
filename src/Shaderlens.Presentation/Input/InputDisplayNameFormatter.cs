namespace Shaderlens.Presentation.Input
{
    public interface IInputDisplayNameFormatter
    {
        string? GetDisplayName(IInputSpan? inputSpan);
    }

    public class InputDisplayNameFormatter : IInputDisplayNameFormatter
    {
        private readonly IInputValueSerializer serializer;

        public InputDisplayNameFormatter(IInputValueSerializer serializer)
        {
            this.serializer = serializer;
        }

        public string? GetDisplayName(IInputSpan? inputSpan)
        {
            if (inputSpan == null)
            {
                return null;
            }

            var writer = new InputSpanWriter(this.serializer);
            inputSpan.WriteTo(writer);
            return writer.GetResults().FirstOrDefault();
        }
    }
}