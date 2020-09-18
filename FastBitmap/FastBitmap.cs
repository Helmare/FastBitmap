/*
    MIT License

    Copyright(c) 2020 Christopher Bishop

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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
        protected GCHandle BitsHandle { get; }

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
            using (Graphics g = Graphics.FromImage(BaseBitmap))
            {
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }
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
            catch (ArgumentOutOfRangeException)
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
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private int PointToIndex(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException();
            else
                return x + y * Width;
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
            catch (ArgumentOutOfRangeException)
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
        public bool TrySetI(int x, int y, Color color)
        {
            try
            {
                Data[PointToIndex(x, y)] = color.ToArgb();
                return true;
            }
            catch (ArgumentOutOfRangeException)
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
            catch(ArgumentOutOfRangeException)
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
            catch(ArgumentOutOfRangeException)
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
            catch(ArgumentOutOfRangeException)
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
            catch(ArgumentOutOfRangeException)
            {
                return false;
            }
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
        ///     Streams each Y value to the callback.
        /// </summary>
        /// <param name="callback">
        ///     The action called for each horizontal line. 
        ///     The first argument is the FastBitmap and the next is the Y value.
        /// </param>
        /// <param name="threads">The number of threads to use.</param>
        [Obsolete("Method has been moved to FastBitmapExt and will be removed in the future.")]
        public void StreamByLine(Action<FastBitmap, int> callback, int threads)
        {
            Task[] tasks = new Task[threads];
            for (int i = 0; i < threads; i++)
            {
                tasks[i] = new Task(new ScanlineStreamInfo(this, callback, i, threads).GetAction());
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        ///     Streams each X and Y value to the callback.
        /// </summary>
        /// <param name="callback">
        ///     The action called for each horizontal line. 
        ///     The first argument is the FastBitmap and the next two are X and Y.
        /// </param>
        /// <param name="threads">The number threads to use.</param>
        [Obsolete("Method has been moved to FastBitmapExt and will be removed in the future.")]
        public void StreamByPixel(Action<FastBitmap, int, int> callback, int threads)
        {
            StreamByLine(new Action<FastBitmap, int>((bmp, y) =>
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    callback(bmp, x, y);
                }
            }), threads);
        }

        /// <summary>
        ///     Clones the FastBitmap into another FastBitmap.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            FastBitmap clone = new FastBitmap(Width, Height);
            Buffer.BlockCopy(Data, 0, clone.Data, 0, Length);
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
        ///     Loads a image file into a FastBitmap.
        /// </summary>
        /// <param name="filename">The path of the image file.</param>
        /// <returns>The new FastBitmap</returns>
        public static FastBitmap FromFile(string filename) => new FastBitmap(Image.FromFile(filename));
    }
}
