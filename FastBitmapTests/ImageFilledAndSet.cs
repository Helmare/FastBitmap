namespace Hazdryx.Drawing.FastBitmapTests
{
    public class ImageFilledAndSet : FastBitmapTestBase
    {
        public static FastBitmap SetupBitmap()
        {
            FastBitmap fastBmp = ImageFilled.SetupBitmap();

            for (int i = 0; i < fastBmp.Length; i++)
            {
                fastBmp.SetI(i, ColorData[i]);
            }
            return fastBmp;
        }
        public ImageFilledAndSet() : base(SetupBitmap())
        {
        }
    }
}