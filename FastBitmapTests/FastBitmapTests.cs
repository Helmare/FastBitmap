using Hazdryx.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace FastBitmapTests
{
    public class FastBitmapTests
    {
        private static readonly int[] ColorData = new int[]
        {
            -65536,     // Red
            -16711936,  // Green
            -16776961,  // Blue

            -16777216,  // Black
            -8421505,   // Gray
            -1,         // White

            2147419397, // Mild Red
            2131099397, // Mild Green
            2131035647, // Mild Blue
        };
        private static FastBitmap SetupBitmap()
        {
            FastBitmap bmp = new FastBitmap(3, 3);
            Array.Copy(ColorData, bmp.Data, 9);
            return bmp;
        }
        
        #region GetI and TryGetI by index Tests
        [Theory]
        [InlineData(0, -65536)]
        [InlineData(5, -1)]
        [InlineData(8, 2131035647)]
        public void GetI_ShouldReturnCorrectValue(int index, int expected)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Equal(expected, bmp.GetI(index));
        }
        [Theory]
        [InlineData(-3)]
        [InlineData(100)]
        public void GetI_ShouldThrowException(int index)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Throws<IndexOutOfRangeException>(() => bmp.GetI(index));
        }
        [Theory]
        [InlineData(2, true, -16776961)]
        [InlineData(-5, false, 0)]
        public void TryGetI_ShouldReturnCorrectValues(int index, bool expected, int expectedColor)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Equal(expected, bmp.TryGetI(index, out int color));
            Assert.Equal(expectedColor, color);
        }
        #endregion

        #region SetI and TrySetI by index Tests
        [Theory]
        [InlineData(2, 255)]
        public void SetI_ShouldCorrectlySetPixel(int index, int color)
        {
            FastBitmap bmp = SetupBitmap();
            bmp.SetI(index, color);
            Assert.Equal(color, bmp.Data[index]);
        }
        [Theory]
        [InlineData(-10, 240)]
        [InlineData(9, -50)]
        public void SetI_ShouldThrowException(int index, int color)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Throws<IndexOutOfRangeException>(() => bmp.SetI(index, color));
        }
        [Theory]
        [InlineData(5, 100, true)]
        [InlineData(-5, -23, false)]
        [InlineData(12, 10, false)]
        public void TrySetI_ShouldCorrectlySetPixel(int index, int color, bool expected)
        {
            FastBitmap bmp = SetupBitmap();
            bool result = bmp.TrySetI(index, color);

            Assert.Equal(expected, result);
            Assert.Equal(color, expected ? bmp.Data[index] : color);
        }
        #endregion

        #region PointToIndex Tests
        [Theory]
        [InlineData(2, 0, 2)]
        [InlineData(1, 1, 4)]
        [InlineData(0, 2, 6)]
        public void PointToIndex_ShouldCalculate(int x, int y, int expected)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Equal(expected, bmp.PointToIndex(x, y));
        }
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(10, 2)]
        [InlineData(1, -6)]
        [InlineData(2, 3)]
        public void PointToIndex_ShouldThrowException(int x, int y)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Throws<ArgumentOutOfRangeException>(() => bmp.PointToIndex(x, y));
        }
        #endregion

        #region GetI and TryGetI by coord Tests
        [Theory]
        [InlineData(0, 0, -65536)]
        [InlineData(2, 1, -1)]
        [InlineData(1, 2, 2131099397)]
        public void GetI_Coord_ShouldReturnCorrectValue(int x, int y, int expected)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Equal(expected, bmp.GetI(x, y));
        }
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(4, 1)]
        [InlineData(0, -1)]
        [InlineData(2, 6)]
        public void GetI_Coord_ShouldThrowException(int x, int y)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Throws<ArgumentOutOfRangeException>(() => bmp.GetI(x, y));
        }
        [Theory]
        [InlineData(0, 0, true, -65536)]
        [InlineData(-1, 2, false, 0)]
        [InlineData(2, 10, false, 0)]
        public void TryGetI_Coord_ShouldReturnCorrectValue(int x, int y, bool expected, int expectedColor)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Equal(expected, bmp.TryGetI(x, y, out int color));
            Assert.Equal(expectedColor, color);
        }
        #endregion

        #region SetI and TrySetI by coord Tests
        [Theory]
        [InlineData(2, 1, 255)]
        public void SetI_Coord_ShouldCorrectlySetPixel(int x, int y, int color)
        {
            FastBitmap bmp = SetupBitmap();
            bmp.SetI(x, y, color);
            Assert.Equal(color, bmp.GetI(x, y));
        }
        [Theory]
        [InlineData(0, -1)]
        [InlineData(1, 4)]
        [InlineData(-1, 0)]
        [InlineData(6, 2)]
        public void SetI_Coord_ShouldThrowException(int x, int y)
        {
            FastBitmap bmp = SetupBitmap();
            Assert.Throws<ArgumentOutOfRangeException>(() => bmp.SetI(x, y, -1));
        }
        [Theory]
        [InlineData(1, 1, 100, true)]
        [InlineData(-5, 0, -23, false)]
        [InlineData(0, 12, 10, false)]
        public void TrySetI_Coord_ShouldCorrectlySetPixel(int x, int y, int color, bool expected)
        {
            FastBitmap bmp = SetupBitmap();
            bool result = bmp.TrySetI(x, y, color);

            Assert.Equal(expected, result);
            Assert.Equal(color, expected ? bmp.GetI(x, y) : color);
        }
        #endregion

        [Fact]
        public void CopyTo_ShouldCopyAllPixelsToFastBitmap()
        {
            FastBitmap src = SetupBitmap();
            FastBitmap dst = new FastBitmap(src.Width, src.Height);

            src.CopyTo(dst);
            for (int i = 0; i < src.Length; i++)
            {
                Assert.Equal(src.Data[i], dst.Data[i]);
            }
        }
        [Fact]
        public void CopyRegionTo_ShouldCopyARegionOfPixelsToFastBitmap()
        {
            FastBitmap src = SetupBitmap();
            FastBitmap dst = new FastBitmap(2, 2);

            src.CopyRegionTo(dst, 1, 1);
            Assert.Equal(ColorData[4], dst.Data[0]);
            Assert.Equal(ColorData[5], dst.Data[1]);
            Assert.Equal(ColorData[7], dst.Data[2]);
            Assert.Equal(ColorData[8], dst.Data[3]);
        }

        [Fact]
        public void Clone_ShouldCreateNewCopy()
        {
            FastBitmap src = SetupBitmap();
            FastBitmap dst = (FastBitmap) src.Clone();

            Assert.Equal(src.Length, dst.Length);
            for (int i = 0; i < src.Length; i++)
            {
                Assert.Equal(src.Data[i], dst.Data[i]);
            }
        }
    }
}