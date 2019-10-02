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

namespace Hazdryx.Drawing
{
    /// <summary>
    ///     A class which stores information about the stream.
    /// </summary>
    public class ScanlineStreamInfo
    {
        /// <summary>
        ///     Gets the FastBitmap for the stream.
        /// </summary>
        public FastBitmap Bitmap { get; }
        /// <summary>
        ///     Gets the callback for the stream.
        /// </summary>
        public Action<FastBitmap, int> Callback { get; }

        /// <summary>
        ///     Gets the thread index the stream belongs to.
        /// </summary>
        public int Thread { get; }
        /// <summary>
        ///     Gets the number of threads.
        /// </summary>
        public int ThreadCount { get; }

        public ScanlineStreamInfo(FastBitmap bitmap, Action<FastBitmap, int> callback, int thread, int threadCount)
        {
            this.Bitmap = bitmap;
            this.Callback = callback;
            this.Thread = thread;
            this.ThreadCount = threadCount;
        }

        /// <summary>
        ///     Gets the action passed to the task.
        /// </summary>
        /// <returns>The aciton built.</returns>
        public Action GetAction()
        {
            return new Action(() =>
            {
                for (int y = Thread; y < Bitmap.Height; y += ThreadCount)
                {
                    Callback(Bitmap, y);
                }
            });
        }
    }
}
