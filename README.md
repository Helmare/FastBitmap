# FastBitmap
A .NET Standard library which wraps the System.Drawing.Bitmap class for improved pixel read/write performance.

[![nuget](https://img.shields.io/nuget/v/Hazdryx.FastBitmap.svg)](https://www.nuget.org/packages/Hazdryx.FastBitmap/) 
![build](https://github.com/hazdryx/FastBitmap/actions/workflows/publish.yml/badge.svg)

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
	Color c = bmp.Get(0, 0);
	bmp.Set(1, 1, c);

	// Gets the color of pixel at index 1 and sets it to index 2.
	// Using indices is faster than coordinates.
	c = bmp.Get(1);
	bmp.Set(2, c);

	// Grabbing color from index 3 and setting it to index 4.
	// Using int (ARGB32) for color is much faster than the other methods.
	int color32 = bmp.GetI(3);
	bmp.SetI(4, color32);

	// Saves image to a file.
	bmp.Save("image0.png", ImageFormat.Png);
}
```

## Contribute
Want to help move this project forward? Consider contributing to the project. There are many different ways you can help out, even if you don't want to submit code changes.

### Use In Your Projects
The easiest way to contribute is to have this repo as a dependency in your projects. This contribution gives the project more recognition and likely to be seen by other developers, thereby growing the community.

### Submit Pull Requests
If you would like to make changes to the codebase or documentation, you can submit a pull request. Make sure to check out the [CONTRIBUTE.md](https://github.com/hazdryx/FastBitmap/blob/master/CONTRIBUTE.md) for pull request requirements.

### Donate
If you don't want to submit a pull request but still want to support my work further, consider sending me a donation. Donations help me spend more time on open-source projects so that they can be of the highest quality possible. You can send donations using the link(s) below.

[buymeacoffee.com](https://www.buymeacoffee.com/hazdryx)

## Known Issues
[#4](https://github.com/hazdryx/FastBitmap/issues/4) On some Linux distros, performing GDI+ (System.Drawing.Graphics) operations result in a desync from the BaseBitmap and FastBitmap.Data.
