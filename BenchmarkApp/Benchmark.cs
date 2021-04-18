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

namespace Hazdryx.Drawing.Benchmark
{
    /// <summary>
    ///     A benchmark function which keeps track of results
    ///     for future use.
    /// </summary>
    public class Benchmark
    {
        /// <summary>
        ///     Gets the benchmark function which returns a value
        ///     in pixels per second (px/s).
        /// </summary>
        private Func<double> Function { get; }
        /// <summary>
        ///     Gets the name of the benchmark.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the average result from the times it was ran.
        /// </summary>
        public double Result { get; private set; }
        /// <summary>
        ///     Gets the number of times this bencmark has been ran.
        /// </summary>
        public int Iterations { get; private set; }

        public Benchmark(string name, Func<double> function)
        {
            this.Name = name;
            this.Function = function;
        }

        /// <summary>
        ///     Runs the benchmark and updates the result.
        /// </summary>
        /// <returns>The result from this run.</returns>
        public double Run()
        {
            double result = Function();
            Result = ((Result * Iterations) + result) / ++Iterations;
            return result;
        }
        /// <summary>
        ///     Resets the results.
        /// </summary>
        public void Reset()
        {
            Result = 0;
            Iterations = 0;
        }
    }
}
