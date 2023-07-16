using System;
using System.Drawing;
using System.Drawing.Imaging;
using Xunit;

namespace Hazdryx.Drawing.FastBitmapTests
{
    public abstract class FastBitmapTestBase
    {
        protected static readonly int[] ColorData = new int[]
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

            -1,         // White
            -16777216,  // Black
            -8421505,   // Grey
        };

        public readonly FastBitmap FastBmp;

        protected FastBitmapTestBase(FastBitmap fastBmp)
        {
            FastBmp = fastBmp;
        }

        #region GetI and TryGetI by index Tests
        [Theory]
        [InlineData(0, -65536)]
        [InlineData(5, -1)]
        [InlineData(8, 2131035647)]
        public void GetI_ShouldReturnCorrectValue(int index, int expected)
        {
            FastBitmap bmp = FastBmp;
            Assert.Equal(expected, bmp.GetI(index));
        }
        [Theory]
        [InlineData(-3)]
        [InlineData(100)]
        public void GetI_ShouldThrowException(int index)
        {
            FastBitmap bmp = FastBmp;
            Assert.Throws<IndexOutOfRangeException>(() => bmp.GetI(index));
        }
        [Theory]
        [InlineData(2, true, -16776961)]
        [InlineData(-5, false, 0)]
        public void TryGetI_ShouldReturnCorrectValues(int index, bool expected, int expectedColor)
        {
            FastBitmap bmp = FastBmp;
            Assert.Equal(expected, bmp.TryGetI(index, out int color));
            Assert.Equal(expectedColor, color);
        }
        #endregion

        #region SetI and TrySetI by index Tests
        [Theory]
        [InlineData(2, 255)]
        [InlineData(1, -65536)]
        public void SetI_ShouldCorrectlySetPixel(int index, int color)
        {
            FastBitmap bmp = FastBmp;
            bmp.SetI(index, color);
            Assert.Equal(color, bmp.Data[index]);
        }
        [Theory]
        [InlineData(-10, 240)]
        [InlineData(12, -50)]
        public void SetI_ShouldThrowException(int index, int color)
        {
            FastBitmap bmp = FastBmp;
            Assert.Throws<IndexOutOfRangeException>(() => bmp.SetI(index, color));
        }
        [Theory]
        [InlineData(5, 100, true)]
        [InlineData(-5, -23, false)]
        [InlineData(12, 10, false)]
        public void TrySetI_ShouldCorrectlySetPixel(int index, int color, bool expected)
        {
            FastBitmap bmp = FastBmp;
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
            FastBitmap bmp = FastBmp;
            Assert.Equal(expected, bmp.PointToIndex(x, y));
        }
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(10, 2)]
        [InlineData(1, -6)]
        [InlineData(2, 4)]
        public void PointToIndex_ShouldReturnNegativeOne(int x, int y)
        {
            FastBitmap bmp = FastBmp;
            Assert.Equal(-1, bmp.PointToIndex(x, y));
        }
        #endregion

        #region GetI and TryGetI by coord Tests
        [Theory]
        [InlineData(0, 0, -65536)]
        [InlineData(2, 1, -1)]
        [InlineData(1, 2, 2131099397)]
        public void GetI_Coord_ShouldReturnCorrectValue(int x, int y, int expected)
        {
            FastBitmap bmp = FastBmp;
            Assert.Equal(expected, bmp.GetI(x, y));
        }
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(4, 1)]
        [InlineData(0, -1)]
        [InlineData(2, 6)]
        public void GetI_Coord_ShouldThrowException(int x, int y)
        {
            FastBitmap bmp = FastBmp;
            Assert.Throws<IndexOutOfRangeException>(() => bmp.GetI(x, y));
        }
        [Theory]
        [InlineData(0, 0, true, -65536)]
        [InlineData(-1, 2, false, 0)]
        [InlineData(2, 10, false, 0)]
        public void TryGetI_Coord_ShouldReturnCorrectValue(int x, int y, bool expected, int expectedColor)
        {
            FastBitmap bmp = FastBmp;
            Assert.Equal(expected, bmp.TryGetI(x, y, out int color));
            Assert.Equal(expectedColor, color);
        }
        #endregion

        #region SetI and TrySetI by coord Tests
        [Theory]
        [InlineData(2, 1, 255)]
        [InlineData(1, 1, -65536)]
        public void SetI_Coord_ShouldCorrectlySetPixel(int x, int y, int color)
        {
            FastBitmap bmp = FastBmp;
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
            FastBitmap bmp = FastBmp;
            Assert.Throws<IndexOutOfRangeException>(() => bmp.SetI(x, y, -1));
        }
        [Theory]
        [InlineData(1, 1, 100, true)]
        [InlineData(-5, 0, -23, false)]
        [InlineData(0, 12, 10, false)]
        public void TrySetI_Coord_ShouldCorrectlySetPixel(int x, int y, int color, bool expected)
        {
            FastBitmap bmp = FastBmp;
            bool result = bmp.TrySetI(x, y, color);

            Assert.Equal(expected, result);
            Assert.Equal(color, expected ? bmp.GetI(x, y) : color);
        }
        #endregion

        #region Set then Get from BaseBitmap

        [Theory]
        [InlineData(2, 1, 255)]
        [InlineData(1, 1, -65536)]
        public void SetI_Coord_GetOnBitmap(int x, int y, int color)
        {
            FastBitmap bmp = FastBmp;
            bmp.SetI(x, y, color);
            Color setColor = Color.FromArgb(color);
            Color bmpColor = bmp.BaseBitmap.GetPixel(x, y);
            Assert.Equal(setColor, bmpColor);
        }

        #endregion

        #region Read and Write Tests
        [Theory]
        [InlineData(3, 0, 3, 0, 3, new int[] { -65536, -16711936, -16776961 })]
        [InlineData(4, 1, 3, 0, 3, new int[] { 0, -65536, -16711936, -16776961 })]
        [InlineData(3, 0, 2, 0, 2, new int[] { -65536, -16711936, 0 })]
        [InlineData(3, 0, 3, 3, 3, new int[] { -16777216, -8421505, -1 })]
        [InlineData(3, 1, 3, 0, 2, new int[] { 0, -65536, -16711936, })]
        [InlineData(3, 0, 3, 10, 2, new int[] { -16777216, -8421505, 0 })]
        [InlineData(3, 1, 3, 10, 2, new int[] { 0, -16777216, -8421505 })]
        [InlineData(3, 2, 3, 10, 1, new int[] { 0, 0, -16777216 })]
        public void Read_ShouldReadIntoBuffer(int bufferLength, int offset, int count, int position, int expectedRead, int[] expectedBuffer)
        {
            int[] buffer = new int[bufferLength];
            int read = FastBmp.Read(buffer, offset, count, position);

            Assert.Equal(expectedRead, read);
            for (int i = 0; i < buffer.Length; i++)
            {
                Assert.Equal(expectedBuffer[i], buffer[i]);
            }
        }

        [Theory]
        [InlineData(new int[] { -65536, -16711936, -16776961 }, 0, 3, 0, 3)]
        [InlineData(new int[] { -65536, -16711936, -16776961 }, 1, 3, 0, 2)]
        [InlineData(new int[] { -65536, -16711936, -16776961 }, 0, 3, 1, 3)]
        [InlineData(new int[] { -65536, -16711936, -16776961 }, 0, 2, 0, 2)]
        [InlineData(new int[] { -65536, -16711936, -16776961 }, 0, 3, 10, 2)]
        [InlineData(new int[] { -65536, -16711936, -16776961 }, 2, 3, 10, 1)]
        public void Write_ShouldWriteFromBuffer(int[] buffer, int offset, int count, int position, int expectedWrite)
        {
            int write = FastBmp.Write(buffer, offset, count, position);

            Assert.Equal(expectedWrite, write);
            for(int i = 0; i < expectedWrite; i++)
            {
                Assert.Equal(buffer[i + offset], FastBmp.GetI(i + position));
            }
        }
        #endregion

        #region Clear Tests
        [Fact]
        public void Clear_ShouldMakeAllPixelsZero()
        {
            FastBitmap bmp = FastBmp;
            bmp.Clear();

            for (int i = 0; i < bmp.Length; i++)
            {
                Assert.Equal(0, bmp.Data[i]);
            }
        }
        #endregion

        #region CopyTo Tests
        [Fact]
        public void CopyTo_ShouldCopyARegionWhichIsOffset()
        {
            FastBitmap src = FastBmp;
            FastBitmap dst = new FastBitmap(3, 3);

            src.CopyTo(dst, 1, 1, 1, 2, 2, 2);
            Assert.Equal(ColorData[7], dst.Data[4]);
            Assert.Equal(ColorData[8], dst.Data[5]);
            Assert.Equal(ColorData[10], dst.Data[7]);
            Assert.Equal(ColorData[11], dst.Data[8]);
        }
        [Theory]
        [InlineData(0, 0, -1, 0, 2, 2, 2, new int[] { 0, -65536, 0, -16777216 })]
        [InlineData(0, -1, 1, 1, 2, 2, 2, new int[] { 2131099397, 2131035647, 0, 0 })]
        [InlineData(-1, 0, -2, 2, 2, 2, 0, new int[] { 0, 0, 0, 0 })]
        [InlineData(1, 1, 0, 5, 2, 2, 0, new int[] { 0, 0, 0, 0 })]
        [InlineData(0, 0, -1, 1, 3, 4, 2, new int[] { 0, -16777216, 0, 2147419397 })]
        public void CopyTo_ShouldCopyAPartialRegion(int dstX, int dstY, int x, int y, int width, int height, int expected, int[] expectedData)
        {
            FastBitmap src = FastBmp;
            FastBitmap dst = new FastBitmap(2, 2);

            Assert.Equal(expected, src.CopyTo(dst, dstX, dstY, x, y, width, height));
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.Equal(expectedData[i], dst.Data[i]);
            }
        }
        [Fact]
        public void CopyTo_ShouldCopyARegion()
        {
            FastBitmap src = FastBmp;
            FastBitmap dst = new FastBitmap(3, 3);

            src.CopyTo(dst, 1, 2, 2, 2);
            Assert.Equal(ColorData[7], dst.Data[0]);
            Assert.Equal(ColorData[8], dst.Data[1]);
            Assert.Equal(ColorData[10], dst.Data[3]);
            Assert.Equal(ColorData[11], dst.Data[4]);
        }
        [Fact]
        public void CopyTo_ShouldCopyASimpleRegion()
        {
            FastBitmap src = FastBmp;
            FastBitmap dst = new FastBitmap(2, 2);

            src.CopyTo(dst, 1, 1);
            Assert.Equal(ColorData[4], dst.Data[0]);
            Assert.Equal(ColorData[5], dst.Data[1]);
            Assert.Equal(ColorData[7], dst.Data[2]);
            Assert.Equal(ColorData[8], dst.Data[3]);
        }
        [Fact]
        public void CopyTo_ShouldCopyAllPixels()
        {
            FastBitmap src = FastBmp;
            FastBitmap dst = new FastBitmap(src.Width, src.Height);

            src.CopyTo(dst);
            for (int i = 0; i < src.Length; i++)
            {
                Assert.Equal(src.Data[i], dst.Data[i]);
            }
        }
        #endregion

        #region Clone Tests
        [Fact]
        public void Clone_ShouldCreateNewCopy()
        {
            FastBitmap src = FastBmp;
            FastBitmap dst = (FastBitmap)src.Clone();

            Assert.Equal(src.Length, dst.Length);
            for (int i = 0; i < src.Length; i++)
            {
                Assert.Equal(src.Data[i], dst.Data[i]);
            }
        }
        #endregion

        #region Scan0 Should Be Identical
        [Fact]
        public void ShouldHaveIdenticalScan0()
        {
            IntPtr scan0 = FastBmp.Scan0;
            Rectangle rect = new Rectangle(0, 0, 1, 1);
            BitmapData bmpData = FastBmp.BaseBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Assert.Equal(scan0, bmpData.Scan0);

            FastBmp.BaseBitmap.UnlockBits(bmpData);
        }
        #endregion
    }
}
