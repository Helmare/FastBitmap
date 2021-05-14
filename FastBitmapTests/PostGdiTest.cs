using System;
using System.Drawing;

namespace Hazdryx.Drawing.FastBitmapTests
{
    public class PostGdiTest : FastBitmapTestBase
    {
        public static FastBitmap SetupBitmap()
        {
            FastBitmap bmp = DataFilled.SetupBitmap();
            using (Graphics g = Graphics.FromImage(bmp.BaseBitmap))
            {
            }

            return bmp;
        }
        public PostGdiTest() : base(SetupBitmap())
        {
        }
    }
}
