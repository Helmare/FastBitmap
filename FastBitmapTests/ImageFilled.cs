using System.Drawing;

namespace Hazdryx.Drawing.FastBitmapTests
{
    public class ImageFilled : FastBitmapTestBase
    {
        public static FastBitmap SetupBitmap()
        {
            Bitmap bmp = new Bitmap(3, 4);
            int index = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(ColorData[index]));
                    index++;
                }
            }

            FastBitmap fastBmp = new FastBitmap(bmp);

            return fastBmp;
        }
        public ImageFilled() : base(SetupBitmap())
        {
        }
    }
}