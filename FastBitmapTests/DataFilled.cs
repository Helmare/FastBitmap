using System;

namespace Hazdryx.Drawing.FastBitmapTests
{
    public class DataFilled : FastBitmapTestBase
    {
        public static FastBitmap SetupBitmap()
        {
            FastBitmap bmp = new FastBitmap(3, 4);
            Array.Copy(ColorData, bmp.Data, 12);
            return bmp;
        }
        public DataFilled() : base(SetupBitmap())
        {
        }
    }
}