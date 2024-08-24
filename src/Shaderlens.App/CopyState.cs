namespace Shaderlens
{
    public class CopySelection
    {
        public static readonly CopySelection CopyFrame = new CopySelection(null);
        public static readonly CopySelection CopyFrameWithAlpha = new CopySelection(null);

        public ICopyFormatter? CopyFormatter { get; private set; }

        public CopySelection(ICopyFormatter? copyFormatter)
        {
            this.CopyFormatter = copyFormatter;
        }
    }
}