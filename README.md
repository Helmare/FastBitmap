# FastBitmap
A .NET Standard library which wraps the System.Drawing.Bitmap class for improved pixel read/write performance.

[![nuget](https://img.shields.io/nuget/v/Hazdryx.FastBitmap.svg)](https://www.nuget.org/packages/Hazdryx.FastBitmap/)

## Benchmarks
The benchmark graph below shows just how much faster FastBitmap is. When using the fastest mode it can see
speed increases up to 20,000% on a single core.
![](https://hazdryx.com/cdn/FastBitmap-Benchmarks.png)
These benchmarks were taken on 9/5/2020 using a Ryzen 9 3950X 16C/32T CPU

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
