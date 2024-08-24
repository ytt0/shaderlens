namespace Shaderlens.Presentation.Extensions
{
    public static class FreezableExtensions
    {
        public static T WithFreeze<T>(this T freezable) where T : Freezable
        {
            freezable.Freeze();
            return freezable;
        }
    }
}
