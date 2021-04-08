using System;
using System.Drawing;
using System.IO;

namespace Image_segmentation
{
    public class ImageBuilder
    {
        private readonly String inputPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + "/Input/";
        private readonly String outputPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + "/Output/";
        private readonly String debugPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + "/Debug/";

        private Mask mask;
        private Bitmap maskRes;

        private String image = "test.jpeg";
        private String brush = "testBrush.png";
        
        private int weight = -1;
        private bool debug;
        public bool loadImage(String image)
        {
            if (!image.Contains(".png") && !image.Contains(".jpeg") && !image.Contains(".JPG"))
            {
                Console.WriteLine("Not a valid image. Please attach a .png, .jpeg or .JPG file.");
                return false;
            }

            this.image = image;
            return true;
        }

        public bool loadBrush(String brush)
        {
            if (!brush.Contains(".png") && !brush.Contains(".jpeg") && !brush.Contains(".JPG"))
            {
                Console.WriteLine("Not a valid brush image. Please attach a .png, .jpeg or .JPG file.");
                return false;
            }
            this.brush = brush;
            return true;
        }

        public void setWeight(int weight)
        {
            this.weight = (weight < 0) ? -1 : weight;
        }

        public bool turnDebug()
        {
            debug = !debug;
            return debug;
        }

        public bool run()
        {
            Image imageIm;
            Image brushIm;

            try
            {
                imageIm = Image.FromFile(inputPath + image);
                brushIm = Image.FromFile(inputPath + brush);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error! Please check the correctness of the paths. Files are not found!");
                Console.WriteLine("Image: " + inputPath + image);
                Console.WriteLine("Brush image: " + inputPath + brush);
                
                return false; 
            }

            if (imageIm.Height != brushIm.Height || imageIm.Width != brushIm.Width)
            {
                Console.WriteLine("Please check if the pair - image and brush image - is valid! " + 
                                  "Sizes are not the same");
                imageIm.Dispose();
                brushIm.Dispose();
                return false;
            }

            mask = new Mask(imageIm, brushIm, weight);
            maskRes = mask.getImageWMask();

            if (debug)
            {
                Bitmap maskRes = mask.getMask();
                Bitmap redProb = mask.getProbMaskRed();
                Bitmap blueProb = mask.getProbMaskBlue();
                
                save(maskRes, debugPath, "Mask.png");
                save(redProb, debugPath, "RedMask.png");
                save(blueProb, debugPath, "BlueMask.png");
                
                maskRes.Dispose();
                redProb.Dispose();
                blueProb.Dispose();
            }
            
            save(maskRes, outputPath, image);

            imageIm.Dispose();
            brushIm.Dispose();
            mask.close();
            return true;
        }

        private void save(Bitmap bitmap, String path, String name)
        {
            Directory.CreateDirectory(path);
            bitmap.Save(path + name);
        }
    }
}