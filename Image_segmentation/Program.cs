using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image_segmentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Image image = Image.FromFile("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/eight.jpeg");
            Image brash = Image.FromFile("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/mask_eight.png");
            
            
            Mask mask = new Mask(image, brash, 10000);

            Bitmap resMask = mask.getMask();
            Bitmap bitmap = mask.getImageWMask();
        
            bitmap.Save("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/maskAfter.png", ImageFormat.Png);
            resMask.Save("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/mask.png", ImageFormat.Png);
            bitmap.Dispose();
        }
    }
}