using Hazdryx.Drawing;
using System;
using System.Drawing;
using Xunit;

namespace FastBitmapTests
{
    public class FastBitmapTests
    {
        public static readonly int[] ColorData = new int[]
        {
            Color.FromArgb(255, 0, 0).ToArgb(),
            Color.FromArgb(0, 255, 0).ToArgb(),
            Color.FromArgb(0, 0, 255).ToArgb(),

            Color.FromArgb(0, 0, 0).ToArgb(),
            Color.FromArgb(128, 128, 128).ToArgb(),
            Color.FromArgb(255, 255, 255).ToArgb(),

            Color.FromArgb(255, 5, 5, 128).ToArgb(),
            Color.FromArgb(5, 255, 5, 128).ToArgb(),
            Color.FromArgb(5, 5, 255, 128).ToArgb()
        };
        private static FastBitmap SetupBitmap()
        {
            FastBitmap bmp = new FastBitmap(3, 3);
            Array.Copy(ColorData, bmp.Data, 9);
            return bmp;
        }

        [Theory]
        [InlineData(2, 0, 2)]
        [InlineData(1, 1, 4)]
        [InlineData(0, 2, 6)]
        public void PointToIndexShouldCalculate(int x, int y, int expected)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Equal(expected, bmp.PointToIndex(x, y));
        }
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(10, 2)]
        [InlineData(1, -6)]
        [InlineData(2, 3)]
        public void PointToIndexShouldThrowException(int x, int y)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Throws<ArgumentOutOfRangeException>(() => bmp.PointToIndex(x, y));
        }


    }
}
