namespace Shaderlens.Extensions
{
    public static class ArrayExtensions
    {
        public static void FlipBufferRows(this Array buffer, int rows)
        {
            var rowLength = buffer.Length / rows;
            var swapRow = new byte[rowLength];

            for (var i = 0; i < rows / 2; i++)
            {
                var sourceRow = i;
                var targetRow = rows - 1 - i;

                Array.Copy(buffer, targetRow * rowLength, swapRow, 0, rowLength);
                Array.Copy(buffer, sourceRow * rowLength, buffer, targetRow * rowLength, rowLength);
                Array.Copy(swapRow, 0, buffer, sourceRow * rowLength, rowLength);
            }
        }

        public static void FlipBgraRgba(this byte[] buffer)
        {
            for (var i = 0; i < buffer.Length - 2; i += 4)
            {
                (buffer[i + 2], buffer[i]) = (buffer[i], buffer[i + 2]);
            }
        }

        public static void RemoveAlphaChannel(this byte[] buffer)
        {
            for (var i = 3; i < buffer.Length; i += 4)
            {
                buffer[i] = 255;
            }
        }
    }
}
