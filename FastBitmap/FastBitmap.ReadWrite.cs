using System;

namespace Hazdryx.Drawing
{
    //
    // Contains methods for buffer Read/Write
    //
    public partial class FastBitmap
    {
        /// <summary>
        ///     Reads pixel data from the bitmap into the buffer.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <param name="offset">Offset of the pixel buffer.</param>
        /// <param name="count">Number of pixels to be read.</param>
        /// <param name="position">Pixel position in the bitmap.</param>
        /// <returns></returns>
        public int Read(int[] buffer, int offset, int count, int position)
        {
            // Reduce the amount read to fit the buffer and bitmap.
            int read = count;
            if (read + offset > buffer.Length)
            {
                read = buffer.Length - offset;
            }
            if (read + position > Length)
            {
                read = Length - position;
            }

            // Block copy data to buffer.
            Buffer.BlockCopy(Data, position * 4, buffer, offset * 4, read * 4);
            return read;
        }
    }
}
