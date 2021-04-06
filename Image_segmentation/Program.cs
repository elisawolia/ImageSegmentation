using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image_segmentation
{
    class Program
    {
        static double GetDist(Color one, Color two)
        {
            return Math.Sqrt(
                Math.Pow(one.R - two.R, 2) +
                Math.Pow(one.G - two.G, 2) +
                Math.Pow(one.B - two.B, 2));
        }
        
        static void Main(string[] args)
        {
            Histogram oneHist = new Histogram();
            Histogram twoHist = new Histogram();


            int width = 0;
            int height = 0;
            
            
            Image image = Image.FromFile("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/test2.JPG");
            Image brash = Image.FromFile("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/test2_brash.png");
            
            Bitmap bitmap = new Bitmap(image); 
            Bitmap bitmapBrash = new Bitmap(brash);
            
            width = bitmap.Width; 
            height = bitmap.Height;
            
            for (int i = 0; i < bitmap.Height; i++)
            { 
                for (int j = 0; j < bitmap.Width; j++)
                { 
                    byte red = bitmapBrash.GetPixel(j, i).R;
                    byte blue = bitmapBrash.GetPixel(j, i).B;
                    
                    if (blue == 255)
                    {
                        twoHist.Add(bitmap.GetPixel(j, i));
                    } else if (red == 255)
                    {
                        oneHist.Add(bitmap.GetPixel(j, i));
                    }
                }
            }
            
            bitmapBrash.Dispose();
                
            Bitmap maskOne = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap maskTwo = new Bitmap(bitmap.Width, bitmap.Height);
                
            double[][] maskTOne = new double[bitmap.Height][];
            double[][] maskTTwo = new double[bitmap.Height][];

            for (int i = 0; i < bitmap.Height; i++)
            { 
                maskTOne[i] = new Double[bitmap.Width]; 
                maskTTwo[i] = new Double[bitmap.Width]; 
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Color o = bitmap.GetPixel(j, i);
                    double probOne = oneHist.getProb(o);
                    double probTwo = twoHist.getProb(o);

                    maskTOne[i][j] = probOne; 
                    maskTTwo[i][j] = probTwo;
                        
                    Color one = Color.FromArgb((byte) (probOne * 255), (byte) (probOne * 255),
                            (byte) (probOne * 255));
                    Color two = Color.FromArgb((byte) (probTwo * 255), (byte) (probTwo * 255),
                            (byte) (probTwo * 255));
                        
                    maskOne.SetPixel(j, i, one);
                    maskTwo.SetPixel(j, i, two);
                }
            }
            
            maskOne.Save("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/maskOne.png", ImageFormat.Png);
            maskTwo.Save("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/maskTwo.png", ImageFormat.Png);

            Bitmap resMask = new Bitmap(width, height);
            
            byte [][] mask = new byte[height][];

            for (int i = 0; i < height; i++)
            { 
                mask[i] = new byte[width];
                for (int j = 0; j < width; j++)
                { 
                    byte m = maskTTwo[i][j].CompareTo(maskTOne[i][j]) >= 0? (byte) 0 : (byte) 1;
                    mask[i][j] = m;
                //    resMask.SetPixel(j, i, Color.FromArgb((byte) (m * 255), (byte) (m * 255), (byte) (m * 255)));
                }
            }

            double energy = 0;
            
            double diff;
            double w = 1;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    diff = 0;
                    energy -= (double.IsFinite(Math.Log(maskTOne[i][j]))
                                  ? Math.Log(maskTOne[i][j])
                                  : Math.Log(Double.Epsilon)) * mask[i][j]
                              + (double.IsFinite(Math.Log(maskTTwo[i][j]))
                                  ? Math.Log(maskTTwo[i][j])
                                  : Math.Log(Double.Epsilon)) * (1 - mask[i][j]);
                    if (i != 0)
                    {
                        diff += Math.Abs(mask[i][j] - mask[i - 1][j]);
                    }
                    if (j != 0)
                    {
                        diff += Math.Abs(mask[i][j] - mask[i][j - 1]);
                    } 
                    if (j != width - 1)
                    {
                        diff += Math.Abs(mask[i][j] - mask[i][j + 1]);
                    }
                    if (i != height - 1)
                    {
                        diff += Math.Abs(mask[i][j] - mask[i + 1][j]);
                    }
            
                    energy += w * diff;
                }
            }
            
            double temp;
            double energyNew = energy;
            
            for (int i = 0; i < height; i++)
            {
                diff = 0;
                for (int j = 0; j < width; j++)
                {
                    energyNew = energy;

                    for (int k = (i > 0 ? i - 1 : i); k <= (i < height - 1 ? i + 1 : i); k++)
                    {
                        for (int l = (j > 0 ? j - 1 : j); l <= (j < width - 1 ? j + 1 : j); l++)
                        {
                            diff = 0;
                            energyNew += (double.IsFinite(Math.Log(maskTOne[k][l]))
                                             ? Math.Log(maskTOne[k][l])
                                             : Math.Log(Double.Epsilon)) * mask[k][l]
                                         + (double.IsFinite(Math.Log(maskTTwo[k][l]))
                                             ? Math.Log(maskTTwo[k][l])
                                             : Math.Log(Double.Epsilon)) * (1 - mask[k][l]);
                            if (k != 0)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k - 1][l]);
                            }
                            if (l != 0)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k][l - 1]);
                            } 
                            if (l != width - 1)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k][l + 1]);
                            }
                            if (k != height - 1)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k + 1][l]);
                            }
                    
                            energyNew -= w * diff;
                        }
                    }
                    
                    
                    mask[i][j] = (mask[i][j] == 0) ? (byte) 1 : (byte) 0;
                    
                    for (int k = (i > 0 ? i - 1 : i); k <= (i < height - 1 ? i + 1 : i); k++)
                    {
                        for (int l = (j > 0 ? j - 1 : j); l <= (j < width - 1 ? j + 1 : j); l++)
                        {
                            diff = 0;
                            energyNew -= (double.IsFinite(Math.Log(maskTOne[k][l]))
                                             ? Math.Log(maskTOne[k][l])
                                             : Math.Log(Double.Epsilon)) * mask[k][l]
                                         + (double.IsFinite(Math.Log(maskTTwo[k][l]))
                                             ? Math.Log(maskTTwo[k][l])
                                             : Math.Log(Double.Epsilon)) * (1 - mask[k][l]);
                            if (k != 0)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k - 1][l]);
                            }
                            if (l != 0)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k][l - 1]);
                            } 
                            if (l != width - 1)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k][l + 1]);
                            }
                            if (k != height - 1)
                            {
                                diff += Math.Abs(mask[k][l] - mask[k + 1][l]);
                            }
                    
                            energyNew += w * diff;
                        }
                    }
                    
                    
                    if (energy < energyNew)
                    {
                        mask[i][j] = (mask[i][j] == 0) ? (byte) 1 : (byte) 0;
                    }
                    else
                    {
                        energy = energyNew;
                    }
                }
            }
            
            //
            // for (int i = 0; i < height; i++)
            // {
            //     for (int j = 0; j < width; j++)
            //     {
            //         Color c = bitmap.GetPixel(j, i);
            //         bitmap.SetPixel(j, i, Color.FromArgb(c.R * mask[i][j], c.G * mask[i][j], c.B * mask[i][j]));
            //     }
            // }
            
            for (int i = 0; i < height; i++)
            { 
                for (int j = 0; j < width; j++)
                { 
                    byte m = mask[i][j];
                    resMask.SetPixel(j, i, Color.FromArgb((byte) (m * 255), (byte) (m * 255), (byte) (m * 255)));
                }
            }

            // bitmap.Save("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/maskAfter.png", ImageFormat.Png);
            resMask.Save("/Users/elizavetavolianica/RiderProjects/ImageSegmentation/Image_segmentation/mask.png", ImageFormat.Png);
            bitmap.Dispose();
        }
    }
}