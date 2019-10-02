/*
    MIT License

    Copyright(c) 2019 Christopher Bishop

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
    public class FastBitmap : IDisposable
    {
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
        ///     Gets or sets whether to ignore IndexOutOfRangeException (default is true).
        ///     
        ///     If this is set to true, getting a color will return <code>DefaultColor</code>
        ///     if the index is out of range. Setting a color won't do anything.
        /// </summary>
        public bool IgnoreOutOfRange { get; set; } = true;
        /// <summary>
        ///     Gets or sets the color used when getting a pixel which is out of range.
        /// </summary>
        public Color DefaultColor { get; set; } = Color.Transparent;

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
        ///     Gets or sets the color of a pixel at the index.
        /// </summary>
        /// <param name="index">The index of the pixel.</param>
        /// <returns>The color of the pixel at the index.</returns>
        public Color this[int index]
        {
            get
            {
                if (index < 0 || index >= Data.Length)
                {
                    if (IgnoreOutOfRange) return DefaultColor;
                    else throw new ArgumentOutOfRangeException();
                }
                else return Color.FromArgb(Data[index]);
            }
            set
            {
                if (index < 0 || index >= Data.Length && !IgnoreOutOfRange)
                    throw new ArgumentOutOfRangeException();
                Data[index] = value.ToArgb();
            }
        }

        /// <summary>
        ///     Gets or sets the color of a pixel at the X and Y coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the pixel.</param>
        /// <param name="y">The Y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at the X and Y coordinates.</returns>
        public Color this[int x, int y]
        {
            get
            {
                try
                {
                    return this[PointToIndex(x, y)];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (IgnoreOutOfRange) return DefaultColor;
                    else throw e;
                }
            }
            set
            {
                try
                {
                    this[PointToIndex(x, y)] = value;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (!IgnoreOutOfRange) throw e;
                }
            }
        }
        private int PointToIndex(int x, int y)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException();
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException();
            return x + y * Width;
        }

        /// <summary>
        ///     Gets or sets the color of a pixel at a specific point.
        /// </summary>
        /// <param name="pt">The point of the pixel.</param>
        /// <returns>The color of the pixel at a specific point.</returns>
        public Color this[Point pt]
        {
            get { return this[pt.X, pt.Y]; }
            set { this[pt.X, pt.Y] = value; }
        }

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
