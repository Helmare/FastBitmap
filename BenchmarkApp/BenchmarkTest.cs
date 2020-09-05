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
using System.Collections.Generic;

namespace Hazdryx.Drawing.Benchmark
{
    /// <summary>
    ///     A collection of relatable benchmarks which can be tested for comparing
    ///     similar technologies.
    /// </summary>
    public class BenchmarkTest
    {
        /// <summary>
        ///     Gets a list of relatable benchmarks.
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
                Console.WriteLine("\nRunning Benchmark: " + bench.Name + "...");
                bench.Reset();
                for (int i = 0; i < IterationCount; i++)
                {
                    Console.Write("Iteration " + (i + 1) + ": ");
                    double result = bench.Run();
                    Console.WriteLine(result.ToString("N3") + " px/s");
                }
                Console.WriteLine("Average: " + bench.Result.ToString("N3") + " px/s");
            }
        }
    }
}
