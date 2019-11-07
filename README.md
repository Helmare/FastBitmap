# FastBitmap
A wrapper for System.Drawing.Bitmap which allows for faster read/write pixel operations.

## Example
```C#
// Loads image from file.
using (FastBitmap bmp = FastBitmap.FromFile("image.png"))
{
	// Gets the color of pixel 0, 0 and sets it to pixel 1, 1.
	Color c = bmp[0, 0];
	bmp[1, 1] = c;

	// Gets the color of pixel at index 1 and sets it to index 2.
	// Using indices is faster than coordinates.
	c = bmp[1];
	bmp[2] = c;

	// Grabbing color from index 3 and setting it to index 4.
	// Using int (ARGB32) for color is much faster than the other methods.
	int color32 = bmp.Data[3];
	bmp.Data[4] = color32;

	// Saves image to a file.
	bmp.Save("image0.png", ImageFormat.Png);
}
```
