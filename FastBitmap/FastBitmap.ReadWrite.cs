using System;
using System.ComponentModel;

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
        /// <returns>How many pixels where read.</returns>
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
        /// <summary>
        ///     Reads pixel data from the bitmap into the buffer.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <param name="count">Number of pixels to be read.</param>
        /// <param name="position">Pixel position in the bitmap.</param>
        /// <returns>How many pixels where read.</returns>
        public int Read(int[] buffer, int count, int position)
        {
            return Read(buffer, 0, count, position);
        }
        /// <summary>
        ///     Reads pixel data from the bitmap into the entire buffer.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <param name="position">Pixel position in the bitmap.</param>
        /// <returns>How many pixels where read.</returns>
        public int Read(int[] buffer, int position)
        {
            return Read(buffer, 0, buffer.Length, position);
        }
        /// <summary>
        ///     Reads as much pixel data from the bitmap into the entire buffer.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <returns>How many pixels where read.</returns>
        public int Read(int[] buffer)
        {
            return Read(buffer, 0, buffer.Length, 0);
        }

        /// <summary>
        ///     Writes pixel data from the buffer to the bitmap.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <param name="offset">Offset of the pixel buffer.</param>
        /// <param name="count">Number of pixels to be read.</param>
        /// <param name="position">Pixel position in the bitmap.</param>
        /// <returns>How many pixels where written.</returns>
        public int Write(int[] buffer, int offset, int count, int position)
        {
            // Reduce the amount writen to fit the buffer and bitmap.
            int write = count;
            if (write + offset > buffer.Length)
            {
                write = buffer.Length - offset;
            }
            if (write + position > Length)
            {
                write = Length - position;
            }

            Buffer.BlockCopy(buffer, offset * 4, Data, position * 4, write * 4);
            return write;
        }
        /// <summary>
        ///     Writes pixel data from the buffer to the bitmap.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <param name="count">Number of pixels to be read.</param>
        /// <param name="position">Pixel position in the bitmap.</param>
        /// <returns>How many pixels where written.</returns>
        public int Write(int[] buffer, int count, int position)
        {
            return Write(buffer, 0, count, position);
        }
        /// <summary>
        ///     Writes pixel data from the buffer to the bitmap.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <param name="position">Pixel position in the bitmap.</param>
        /// <returns>How many pixels where written.</returns>
        public int Write(int[] buffer, int position)
        {
            return Write(buffer, 0, buffer.Length, position);
        }
        /// <summary>
        ///     Writes pixel data from the buffer to the bitmap.
        /// </summary>
        /// <param name="buffer">Pixel data buffer.</param>
        /// <returns>How many pixels where written.</returns>
        public int Write(int[] buffer)
        {
            return Write(buffer, 0, buffer.Length, 0);
        }
    }
}
