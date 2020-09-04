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

    ------------------------------------------------------------------------------    

    The image.jpg file was created by scratsmacker and can be downloaded from:
    https://pixabay.com/photos/new-york-brooklyn-bridge-nyc-city-5173657/
*/
using System;
using System.Diagnostics;
using System.Drawing;

namespace Hazdryx.Drawing.Benchmark
{
    /// <summary>
    ///     A console program which compares the pixel read/write speeds of
    ///     System.Drawing.Bitmap and FastBitmap.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     The number of times each benchmark is ran.
        /// </summary>
        public const int IterationCount = 3;

        public static void Main(string[] args)
        {
            Console.WriteLine("Running benchmark for FastBitmap v1.0.0");

            Console.WriteLine("FastBitmap Coord:");
            double fbcoord = TestFastBitmapCoord();
            Console.WriteLine("\t" + (fbcoord / 1000).ToString("N2") + " mp/s");

            Console.WriteLine("SystemBitmap Coord:");
            double sbcoord = TestSystemBitmap();
            Console.WriteLine("\t" + (sbcoord / 1000).ToString("N2") + " mp/s");

            Console.WriteLine("\nFastBitmap is " + (100 * ((fbcoord / sbcoord) - 1)).ToString("N2") + "% faster.");

            Console.Write("Finished All Benchmarks");
            Console.Read();
        }

        private static double TestFastBitmapCoord()
        {
            using (FastBitmap src = FastBitmap.FromFile("image.jpg"))
            using (FastBitmap dst = new FastBitmap(src.Width, src.Height))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int x = 0; x < dst.Width; x++)
                {
                    for(int y = 0; y < dst.Height; y++)
                    {
                        dst[x, y] = src[x, y];
                    }
                }
                sw.Stop();

                dst.Save("FastBitmapCoord.jpg");
                return src.Width * src.Height / sw.Elapsed.TotalSeconds;
            }
        }
        private static double TestSystemBitmap()
        {
            using (Bitmap src = new Bitmap(Image.FromFile("image.jpg")))
            using (Bitmap dst = new Bitmap(src.Width, src.Height))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int x = 0; x < dst.Width; x++)
                {
                    for (int y = 0; y < dst.Height; y++)
                    {
                        dst.SetPixel(x, y, src.GetPixel(x, y));
                    }
                }
                sw.Stop();

                dst.Save("SystemBitmap.jpg");
                return src.Width * src.Height / sw.Elapsed.TotalSeconds;
            }
        }
    }
}