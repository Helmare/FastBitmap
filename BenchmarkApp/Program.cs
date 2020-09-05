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
        public static void Main(string[] args)
        {
            Console.WriteLine("FastBitmap (v1.0.0) Benchmarking App");
            Console.WriteLine("------------------------------------------------");

            //
            // Single-Core Passthrough Benchmarks
            //
            BenchmarkTest pt = new BenchmarkTest
            {
                IterationCount = 3
            };
            
            // System.Drawing.Bitmap Coord Benchmark
            pt.AddBenchmark("SC Passthrough | SysBitmap Coord", () =>
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
            });

            // FastBitmap Coord Benchmark
            pt.AddBenchmark("SC Passthrough | FastBitmap Coord", () =>
            {
                using (FastBitmap src = FastBitmap.FromFile("image.jpg"))
                using (FastBitmap dst = new FastBitmap(src.Width, src.Height))
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (int x = 0; x < dst.Width; x++)
                    {
                        for (int y = 0; y < dst.Height; y++)
                        {
                            dst[x, y] = src[x, y];
                        }
                    }
                    sw.Stop();

                    dst.Save("FastBitmapCoord.jpg");
                    return src.Width * src.Height / sw.Elapsed.TotalSeconds;
                }
            });

            // Run Passthrough
            pt.Run();

            // Finished.
            Console.Write("\nAll Benchmarks Completed");
            Console.Read();
        }
    }
}