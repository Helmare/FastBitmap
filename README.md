# FastBitmap
A .NET Standard library which wraps the System.Drawing.Bitmap class for improved pixel read/write performance.

[![nuget](https://img.shields.io/nuget/v/Hazdryx.FastBitmap.svg)](https://www.nuget.org/packages/Hazdryx.FastBitmap/) [![build](https://github.com/hazdryx/FastBitmap/actions/workflows/publish.yml/badge.svg)]

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
Want to help make FastBitmap the go to pixel manipulation library for .NET? 
Why not contibute to the project! There are multiple ways you can contribute to this
project.

### Use FastBitmap in your Project
Using the library in your next project will help by getting more downloads an more recognition.
It's easy to get started and provides fast image manipulation functionality.

### Submit Pull Requests
You could submit a pull request with bug fixes, new features, examples, or anything else that
comes to mind. Just make sure you read the CONTRIBUTE.md for pull request requirements.

### Donate
If you don't want to submit a pull request, consider donating. This will help me continue to
work on the project in my free time.

[buymecoffee.com](https://www.buymeacoffee.com/hazdryx)