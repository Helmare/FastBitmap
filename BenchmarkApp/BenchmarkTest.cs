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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace Hazdryx.Drawing.Benchmark
{
    /// <summary>
    ///     A collection of relatable benchmarks which can be tested for comparing
    ///     similar technologies.
    /// </summary>
    public class BenchmarkTest
    {
        /// <summary>
        ///     Gets or sets the graph header.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     Gets a list of relatable benchmarks, which the first is the baseline.
        /// </summary>
        public List<Benchmark> Benchmarks { get; } = new List<Benchmark>();
        /// <summary>
        ///     Gets or sets the number of times each benchmark function is ran.
        /// </summary>
        public uint IterationCount { get; set; } = 1;

        /// <summary>
        ///     Instatiates a benchmark from the name and functions and adds
        ///     it to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Function"></param>
        public void AddBenchmark(string name, Func<double> function)
        {
            Benchmarks.Add(new Benchmark(name, function));
        }

        /// <summary>
        ///     Runs the benchmark and outputs results to the console.
        /// </summary>
        public void Run()
        {
            if (IterationCount == 0) throw new InvalidOperationException("Cannot run functions 0 or less times.");
            foreach (Benchmark bench in Benchmarks)
            {
                Console.WriteLine("\nRunning Benchmark: " + Name + " | " + bench.Name + "...");
                bench.Reset();
                for (int i = 0; i < IterationCount; i++)
                {
                    Console.Write("Iteration " + (i + 1) + ": ");
                    double result = bench.Run();
                    Console.WriteLine(result.ToString("N3") + " px/s");
                }

                double compare = 100 * (bench.Result / Benchmarks[0].Result - 1);
                Console.WriteLine(
                    "Average: " + bench.Result.ToString("N3") + " px/s (" + 
                    (compare > 0 ? "+" : "") + compare.ToString("N0") + "%)"
                );
            }
        }

        /// <summary>
        ///     Draws graph based on the current results in the benchmarks.
        /// </summary>
        /// <param name="max"></param>
        public FastBitmap DrawGraph()
        {
            // Create Image
            FastBitmap bmp = new FastBitmap(2880, 2160);

            // Create size refs
            Rectangle bounds = new Rectangle(150, 150, bmp.Width - 300, bmp.Height - 300);
            float barWidth = bmp.Width / 20;
            float barSpacing = bmp.Width / (float) Benchmarks.Count;
            float maxValue = 325000000;

            // Start Drawing.
            using (Graphics g = Graphics.FromImage(bmp.BaseBitmap))
            using (Font fntHeader = new Font("Consolas", 64))
            using (Font fnt = new Font("Consolas", 24))
            using (Pen pen = new Pen(Color.Gray, 4))
            {
                // Setup graphics
                g.Clear(Color.FromArgb(15, 15, 15));
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Draw Data
                for (int i = 1; i < Benchmarks.Count; i++)
                {
                    float startX = barSpacing * i;
                    // Draw BaseLine
                    DrawBar(g, Brushes.OrangeRed, bounds, startX - barWidth, barWidth, Benchmarks[0].Result, maxValue);

                    // Draw Benchmark
                    DrawBar(g, Brushes.RoyalBlue, bounds, startX, barWidth, Benchmarks[i].Result, maxValue);
                    SizeF labelSize = g.MeasureString(Benchmarks[i].Name, fnt);
                    g.DrawString(Benchmarks[i].Name, fnt, Brushes.White, startX - labelSize.Width / 2, bounds.Bottom + 20);
                }

                // Draw Header
                SizeF headerSize = g.MeasureString(Name, fntHeader);
                g.DrawString(Name, fntHeader, Brushes.White, (bmp.Width - headerSize.Width) / 2, bounds.Top - headerSize.Height);

                // Draw Outline.
                g.DrawLine(pen, bounds.Left - pen.Width / 2, bounds.Top, bounds.Left - pen.Width / 2, bounds.Bottom);
                g.DrawLine(pen, bounds.Left, bounds.Bottom + pen.Width / 2, bounds.Right, bounds.Bottom + pen.Width / 2);

                // Draw Max Value
                g.DrawString(maxValue.ToString("N0") + " px/s", fnt, Brushes.Gray, bounds.Left + 20, bounds.Top);

                // Draw Legend
                float lgndY = bmp.Height / 2 - 90;
                g.FillRectangle(Brushes.OrangeRed, bounds.Left + 20, lgndY, 35, 35);
                SizeF blstr = g.MeasureString(Benchmarks[0].Name, fnt);
                g.DrawString(Benchmarks[0].Name, fnt, Brushes.Gray, bounds.Left + 65, lgndY + 18f - blstr.Height / 2);

                g.FillRectangle(Brushes.RoyalBlue, bounds.Left + 20, lgndY + 55, 35, 35);
                SizeF obstr = g.MeasureString("Other Benchmarks", fnt);
                g.DrawString("Other Benchmarks", fnt, Brushes.Gray, bounds.Left + 65, lgndY + 73f - obstr.Height / 2);
            }
            return bmp;
        }
        private void DrawBar(Graphics g, Brush brush, Rectangle bounds, float x, float width, double value, double maxValue)
        {
            double height = value / maxValue;
            if (height > 1) height = 1;
            height *= bounds.Height;
            g.FillRectangle(brush, x, (float) (bounds.Top + bounds.Height - height), width, (float) height);
        }
    }
}
