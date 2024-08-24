namespace Shaderlens.Presentation.Extensions
{
    public static class ColorExtensions
    {
        public static Color ToColor(this SrgbColor color)
        {
            return Color.FromArgb(
                (byte)Math.Clamp(255.0 * color.A, 0.0, 255.0),
                (byte)Math.Clamp(255.0 * color.R, 0.0, 255.0),
                (byte)Math.Clamp(255.0 * color.G, 0.0, 255.0),
                (byte)Math.Clamp(255.0 * color.B, 0.0, 255.0));
        }

        public static SrgbColor ToSrgbColor(this Color color)
        {
            return new SrgbColor(
                color.R / 255.0,
                color.G / 255.0,
                color.B / 255.0,
                color.A / 255.0);
        }
    }
}
