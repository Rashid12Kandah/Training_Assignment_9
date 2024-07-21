using System;
using System.Drawing;
using System.Drawing.Imaging;
using dipp;

public class UnsafeImageResizer
{
    public static Bitmap To_8Bit(Bitmap image)
    {
        if(image.PixelFormat != PixelFormat.Format1bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb )
        {
            throw new Exception("Image format is not supported only 24-bit and 8-bit images are supported");
        }else
        {
            return GrayScaleConvert.Conv_24to8bits(image);
        }
    }
    public static Bitmap BilinearResize(Bitmap _originalImg,double ScalingFactor)
    {
        int oldH = _originalImg.Height;
        int oldW = _originalImg.Width;
        Bitmap originalImg = new Bitmap(oldW, oldH, PixelFormat.Format8bppIndexed);
        ColorPalette grayscalePalette = originalImg.Palette;
        for (int i = 0; i < 256; i++)
        {
            grayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
        }
        originalImg.Palette = grayscalePalette;

        if(_originalImg.PixelFormat != PixelFormat.Format8bppIndexed)
        {
            originalImg = To_8Bit(_originalImg);
        }
        else
        {
            originalImg = (Bitmap)_originalImg.Clone();
        }

        int newH = (int)(oldH * ScalingFactor), newW = (int)(oldW * ScalingFactor);

        Bitmap resized = new Bitmap(newW, newH, PixelFormat.Format8bppIndexed);

        resized.Palette = grayscalePalette;

    

        double wScaleFactor = newW != 0 ? (double)oldW / newW : 0;
        double hScaleFactor = newH != 0 ? (double)oldH / newH : 0;
        

        BitmapData originalData = originalImg.LockBits(new Rectangle(0, 0, oldW, oldH), ImageLockMode.ReadOnly, originalImg.PixelFormat);
        BitmapData resizedData = resized.LockBits(new Rectangle(0, 0, newW, newH), ImageLockMode.WriteOnly, resized.PixelFormat);

        unsafe
        {
            byte* originalPtr = (byte*)originalData.Scan0;
            byte* resizedPtr = (byte*)resizedData.Scan0;

            for (int i = 0; i < newH; i++)
            {
                for (int j = 0; j < newW; j++)
                {
                    double x = i * hScaleFactor;
                    double y = j * wScaleFactor;

                    int xFloor = (int)Math.Floor(x);
                    int xCeil = Math.Min(oldH - 1, (int)Math.Ceiling(x));
                    int yFloor = (int)Math.Floor(y);
                    int yCeil = Math.Min(oldW - 1, (int)Math.Ceiling(y));

                    byte q;

                    if (xCeil == xFloor && yCeil == yFloor)
                    {
                        q = originalPtr[xFloor * originalData.Stride + yFloor];
                    }
                    else if (xCeil == xFloor)
                    {
                        byte q1 = originalPtr[xFloor * originalData.Stride + yFloor];
                        byte q2 = originalPtr[xFloor * originalData.Stride + yCeil];
                        q = (byte)(q1 * (yCeil - y) + q2 * (y - yFloor));
                    }
                    else if (yCeil == yFloor)
                    {
                        byte q1 = originalPtr[xFloor * originalData.Stride + yFloor];
                        byte q2 = originalPtr[xCeil * originalData.Stride + yFloor];
                        q = (byte)(q1 * (xCeil - x) + q2 * (x - xFloor));
                    }
                    else
                    {
                        byte v1 = originalPtr[xFloor * originalData.Stride + yFloor];
                        byte v2 = originalPtr[xFloor * originalData.Stride + yCeil];
                        byte v3 = originalPtr[xCeil * originalData.Stride + yFloor];
                        byte v4 = originalPtr[xCeil * originalData.Stride + yCeil];

                        double q1 = v1 * (yCeil - y) + v2 * (y - yFloor);
                        double q2 = v3 * (yCeil - y) + v4 * (y - yFloor);
                        q = (byte)(q1 * (xCeil - x) + q2 * (x - xFloor));
                    }

                    resizedPtr[i * resizedData.Stride + j] = q;
                }
            }
        }

        originalImg.UnlockBits(originalData);
        resized.UnlockBits(resizedData);

        return resized;
    }

    public static void Verify(Bitmap img1, Bitmap img2, double ScalingFactor)
    {
        int img1W = img1.Width, img1H = img1.Height;
        int img2W = img2.Width, img2H = img2.Height;
        Console.WriteLine("Verifying images...");
        Console.WriteLine("Original Image Size: " + img1W + "x" + img1H);
        Console.WriteLine("Original Image Pixel Format: " + img1.PixelFormat);
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine("Resized Image Size: " + img2W + "x" + img2H);
        Console.WriteLine("Resized Image Pixel Format: " + img2.PixelFormat);
        Console.WriteLine("--------------------------------------------");
        if(img1W != img2W && img1H != img2H)
        {
            if((int)(img1W*ScalingFactor) == img2W && (int)(img1H*ScalingFactor) == img2H)
            {
                Console.WriteLine("Images have different dimensions.");
                Console.WriteLine("Image have been resized successfully.");
            }
            else
            {
                Console.WriteLine("Image has not been resized correctly.");
            }
        }
        else
        {
            Console.WriteLine("Images have same dimensions.");
        }
    }

    public static void Main(string[] args)
    {
        string inputPath = args[0];
        double ScalingFactor = double.Parse(args[1]);
        Bitmap originalImg = new Bitmap(inputPath);
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
    
        Bitmap resizedImg = BilinearResize(originalImg, ScalingFactor);
        watch.Stop();
        
        resizedImg.Save("Rescaled_Husky_0.5.jpeg", ImageFormat.Jpeg);

        Verify(originalImg, resizedImg, ScalingFactor);

        Console.WriteLine("--------------------------------------------");
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}