namespace Shaderlens.Presentation.Elements
{
    public interface IMenuSource
    {
        void AddTo(IMenuBuilder builder);
    }

    public class HeaderedMenuSource : IMenuSource
    {
        private readonly IMenuSource menuSource;
        private readonly object header;
        private readonly object? icon;

        public HeaderedMenuSource(IMenuSource menuSource, object header, object? icon)
        {
            this.menuSource = menuSource;
            this.header = header;
            this.icon = icon;
        }

        public void AddTo(IMenuBuilder builder)
        {
            builder.AddHeader(this.header, this.icon);
            this.menuSource.AddTo(builder);
        }
    }
}
