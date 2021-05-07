/*
    MIT License

    Copyright(c) 2021 Christopher Bishop

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hazdryx.Drawing
{
    /// <summary>
    ///     Wraps a System.Drawing.Bitmap and exposes direct access to the
    ///     pixel data.
    ///     
    ///     This class is based on the work from @SaxxonPike on Stackoverflow:
    ///     https://stackoverflow.com/questions/24701703/c-sharp-faster-alternatives-to-setpixel-and-getpixel-for-bitmaps-for-windows-f
    /// </summary>
    public class FastBitmap : ICloneable, IDisposable
    {
        private static int _defaultArgb = 0;
        private static Color _defaultColor = Color.FromArgb(_defaultArgb);
        /// <summary>
        ///     Gets or sets the color which is returned when a TryGet fails.
        /// </summary>
        public static Color DefaultColor
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                _defaultArgb = value.ToArgb();
            }
        }

        /// <summary>
        ///     Gets the array which allows direct access to pixel data in Int32 (ARGB) form.
        /// </summary>
        public int[] Data { get; }
        /// <summary>
        ///     Gets the Garbage Collector handle for the pixel data.
        /// </summary>
        protected GCHandle BitsHandle { get; }
        /// <summary>
        ///     A cache for the clear function.
        /// </summary>
        private int[] _clearCache = null;

        /// <summary>
        ///     Gets the underlying bitmap this object is wrapping.
        /// </summary>
        public Bitmap BaseBitmap { get; }
        /// <summary>
        ///     Gets the width of the bitmap.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        ///     Gets the height of the bitmap.
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        ///     Gets the length of the pixel data.
        /// </summary>
        public int Length => Data.Length;

        /// <summary>
        ///     Initializes a blank bitmap.
        /// </summary>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        public FastBitmap(int width, int height)
        {
            this.Data = new int[width * height];
            this.BitsHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            this.BaseBitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
            this.Width = width;
            this.Height = height;
        }
        /// <summary>
        ///     Initializes a bitmap and then draws the image using GDI+.
        /// </summary>
        /// <param name="image">The image which will be drawn to the bitmap.</param>
        public FastBitmap(Image image) : this(image.Width, image.Height)
        {
            Bitmap bmp = (Bitmap) image;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Marshal.Copy(bmpData.Scan0, Data, 0, Length);
            
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        ///     Gets the color of a pixel.
        /// </summary>
        /// <param name="index">Index of the pixel.</param>
        /// <returns></returns>
        public Color Get(int index) => Color.FromArgb(Data[index]);
        /// <summary>
        ///     Gets the color of a pixel.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        /// <returns>Whether the color was successfully obtained.</returns>
        public bool TryGet(int index, out Color color)
        {
            try
            {
                color = Color.FromArgb(Data[index]);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                color = _defaultColor;
                return false;
            }
        }

        /// <summary>
        ///     Sets the color of a pixel.
        /// </summary>
        /// <param name="index">Index of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        public void Set(int index, Color color) => Data[index] = color.ToArgb();
        /// <summary>
        ///     Sets the color of a pixel.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        /// <returns>Whether the color was set.</returns>
        public bool TrySet(int index, Color color)
        {
            try
            {
                Data[index] = color.ToArgb();
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Converts an X and Y coordinates to an index. If X and Y are out of bounds,
        ///     an exception is thrown.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int PointToIndex(int x, int y)
        {
            return (x < 0 || x >= Width || y < 0 || y >= Height) ? -1 : x + y * Width;
        }
        /// <summary>
        ///     Gets color of a pixel.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <returns></returns>
        public Color Get(int x, int y) => Color.FromArgb(Data[PointToIndex(x, y)]);

        /// <summary>
        ///     Gets color of a pixel.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <param name="color">Default color if out of range.</param>
        /// <returns>Whether the color was successfully obtained.</returns>
        public bool TryGet(int x, int y, out Color color)
        {
            try
            {
                color = Color.FromArgb(Data[PointToIndex(x, y)]);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                color = _defaultColor;
                return false;
            }
        }

        /// <summary>
        ///     Sets the color of a pixel.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        public void Set(int x, int y, Color color) => Data[PointToIndex(x, y)] = color.ToArgb();
        /// <summary>
        ///     Sets the color of a pixel.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        /// <returns>Whether the color was set.</returns>
        public bool TrySet(int x, int y, Color color)
        {
            try
            {
                Data[PointToIndex(x, y)] = color.ToArgb();
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="index">Index of the pixel.</param>
        /// <returns></returns>
        public int GetI(int index) => Data[index];
        /// <summary>
        ///     Gets color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="index">Index of the pixel.</param>
        /// <param name="color">Color of the pixel in ARGB32 form.</param>
        /// <returns>Whether the color was successfully obtained.</returns>
        public bool TryGetI(int index, out int color)
        {
            try
            {
                color = Data[index];
                return true;
            }
            catch(IndexOutOfRangeException)
            {
                color = _defaultArgb;
                return false;
            }
        }

        /// <summary>
        ///     Sets the color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="index">Index of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        public void SetI(int index, int color) => Data[index] = color;
        /// <summary>
        ///     Sets the color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="index">Index of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        /// <returns>Whether the color was set.</returns>
        public bool TrySetI(int index, int color)
        {
            try
            {
                Data[index] = color;
                return true;
            }
            catch(IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <returns></returns>
        public int GetI(int x, int y) => Data[PointToIndex(x, y)];
        /// <summary>
        ///     Gets color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <param name="color">Default color if out of range.</param>
        /// <returns>Whether the color was successfully obtained.</returns>
        public bool TryGetI(int x, int y, out int color)
        {
            try
            {
                color = Data[PointToIndex(x, y)];
                return true;
            }
            catch(IndexOutOfRangeException)
            {
                color = _defaultArgb;
                return false;
            }
        }

        /// <summary>
        ///     Sets the color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        public void SetI(int x, int y, int color) => Data[PointToIndex(x, y)] = color;
        /// <summary>
        ///     Sets the color of a pixel in ARGB32 form.
        /// </summary>
        /// <param name="x">X component of the pixel.</param>
        /// <param name="y">Y component of the pixel.</param>
        /// <param name="color">New color of the pixel.</param>
        /// <returns>Whether the color was set.</returns>
        public bool TrySetI(int x, int y, int color)
        {
            try
            {
                Data[PointToIndex(x, y)] = color;
                return true;
            }
            catch(IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Sets all pixels values to 0 (transparent).
        /// </summary>
        public void Clear()
        {
            if (_clearCache == null) _clearCache = new int[Length];
            Buffer.BlockCopy(_clearCache, 0, Data, 0, Length * 4);
        }

        /// <summary>
        ///     Copies a region of this bitmap to the destination bitmap.
        /// </summary>
        /// <param name="dst">Destination bitmap.</param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>The number of pixels copied.</returns>
        public int CopyTo(FastBitmap dst, int dstX, int dstY, int x, int y, int width, int height)
        {
            // Adjust source coordinates based on dst coodinates.
            if (dstX < 0)
            {
                x -= dstX;
                width += dstX;
                dstX = 0;
            }
            if (dstY < 0)
            {
                y -= dstY;
                height += dstY;
                dstY = 0;
            }
            // Adjust dst coordinates based on source coordinates.
            if (x < 0)
            {
                dstX -= x;
                width += x;
                x = 0;
            }
            if (y < 0)
            {
                dstY -= y;
                height += y;
                y = 0;
            }

            // Check if anything is being copied.
            if (width <= 0 || height <= 0 ||
                x >= Width || y >= Height ||
                dstX >= dst.Width || dstY >= dst.Height) return 0;

            // Adjust width to not go out of bounds.
            if (dst.Width < dstX + width) width = dst.Width - dstX;
            if (Width < x + width) width = Width - x;
            // Adjust height to not go out of bounds.
            if (dst.Height < dstY + height) height = dst.Height - dstY;
            if (Height < y + height) height = Height - y;

            // Copy each line.
            int srcOffset = PointToIndex(x, y);
            int dstOffset = dst.PointToIndex(dstX, dstY);
            for (int i = 0; i < height; i++)
            {
                Buffer.BlockCopy(Data, srcOffset * 4, dst.Data, dstOffset * 4, width * 4);
                srcOffset += Width;
                dstOffset += dst.Width;
            }
            return width * height;
        }
        /// <summary>
        ///     Copies a region of this bitmap to the destination bitmap.
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>The number of pixels copied.</returns>
        public int CopyTo(FastBitmap dst, int x, int y, int width, int height)
        {
            return CopyTo(dst, 0, 0, x, y, width, height);
        }
        /// <summary>
        ///     Copies a region of this bitmap with a rect of {x, y, dst.Width, dst.Height}
        ///     to the destination.
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="x">The X coordiante of the region.</param>
        /// <param name="y">The Y coordinate of the region.</param>
        /// <returns>The number of pixels copied.</returns>
        public int CopyTo(FastBitmap dst, int x, int y)
        {
            return CopyTo(dst, 0, 0, x, y, dst.Width, dst.Height);
        }
        /// <summary>
        ///     Copies all the color data to the new bitmap.
        ///     
        ///     Notice: This method can be called on an incompatable
        ///     bitmap and may have unwanted side effects.
        /// </summary>
        /// <param name="dst"></param>
        /// <returns>The number of pixels copied.</returns>
        public int CopyTo(FastBitmap dst)
        {
            Buffer.BlockCopy(Data, 0, dst.Data, 0, Length * 4);
            return Length;
        }

        /// <summary>
        ///     Clones the FastBitmap into another FastBitmap.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            FastBitmap clone = new FastBitmap(Width, Height);
            CopyTo(clone);
            return clone;
        }

        /// <summary>
        ///     Frees pinned resources and disposes base bitmap.
        /// </summary>
        public void Dispose()
        {
            BitsHandle.Free();
            BaseBitmap.Dispose();
        }

        /// <summary>
        ///     Saves the FastBitmap to a file.
        /// </summary>
        /// <param name="filename">The path of the image file.</param>
        public void Save(string filename) => BaseBitmap.Save(filename);
        /// <summary>
        ///     Saves the FastBitmap to a file using a specific format.
        /// </summary>
        /// <param name="filename">The path of the image file.</param>
        /// <param name="format">The format of the image file.</param>
        public void Save(string filename, ImageFormat format) => BaseBitmap.Save(filename, format);
        /// <summary>
        ///     Loads a image file into a FastBitmap.
        /// </summary>
        /// <param name="filename">The path of the image file.</param>
        /// <returns>The new FastBitmap</returns>
        public static FastBitmap FromFile(string filename) => new FastBitmap(Image.FromFile(filename));
    }
}
