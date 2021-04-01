using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image_segmentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Histogram oneHist = new Histogram();
            Histogram twoHist = new Histogram();


            int width = 0;
            int height = 0;
            
            
            Image image = Image.FromFile("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/test2.JPG");
            Image brash = Image.FromFile("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/test2_brash.png");
            
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
                        
                    // Color one = Color.FromArgb((byte) (probOne * 255), (byte) (probOne * 255),
                    //         (byte) (probOne * 255));
                    // Color two = Color.FromArgb((byte) (probTwo * 255), (byte) (probTwo * 255),
                    //         (byte) (probTwo * 255));
                    //     
                    // maskOne.SetPixel(j, i, one);
                    // maskTwo.SetPixel(j, i, two);
                }
            }

            // maskOne.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/maskOne.png", ImageFormat.Png);
            // maskTwo.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/maskTwo.png", ImageFormat.Png);
            //
            // Подсчитываем функцию разделения

            double funcRasdelOne = 0d;
            double funcRasdelTwo = 0d;

            double [][] priorOne = new double[height][];
            double [][] priorTwo = new double[height][];
            
            double tOne;
            double tTwo;
            
            for (int i = 0; i < height; i++)
            {
                tOne = 1;
                tTwo = 1;
                priorOne[i] = new double[width];
                priorTwo[i] = new double[width];
                for (int j = 0; j < width; j++)
                {
                    if (i != 0)
                    {
                        tOne += Math.Exp(-Math.Abs(maskTOne[i][j] - maskTOne[i - 1][j]));
                        tTwo += Math.Exp(-Math.Abs(maskTTwo[i][j] - maskTTwo[i - 1][j]));
                    }
                    if (j != 0)
                    {
                        tOne += Math.Exp(-Math.Abs(maskTOne[i][j] - maskTOne[i][j - 1]));
                        tTwo += Math.Exp(-Math.Abs(maskTTwo[i][j] - maskTTwo[i][j - 1]));
                    }
                    // if (j != 0 && i != 0)
                    // {
                    //     tOne += Math.Abs(maskTOne[i][j] - maskTOne[i - 1][j - 1]);
                    //     tTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i - 1][j - 1]);
                    // }
                    // if (j != 0 && i != height - 1)
                    // {
                    //     tOne += Math.Abs(maskTOne[i][j] - maskTOne[i + 1][j - 1]);
                    //     tTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i + 1][j - 1]);
                    // }
                    if (j != width - 1)
                    {
                        tOne += Math.Exp(-Math.Abs(maskTOne[i][j] - maskTOne[i][j + 1]));
                        tTwo += Math.Exp(-Math.Abs(maskTTwo[i][j] - maskTTwo[i][j + 1]));
                    }
                    if (i != height - 1)
                    {
                        tOne += Math.Exp(-Math.Abs(maskTOne[i][j] - maskTOne[i + 1][j]));
                        tTwo += Math.Exp(-Math.Abs(maskTTwo[i][j] - maskTTwo[i + 1][j]));
                    }
                    // if (i != height - 1 && j != width - 1)
                    // {
                    //     tOne += Math.Abs(maskTOne[i][j] - maskTOne[i + 1][j + 1]);
                    //     tTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i + 1][j + 1]);
                    // }
                    // if (j != width - 1 && i != 0)
                    // {
                    //     tOne += Math.Abs(maskTOne[i][j] - maskTOne[i - 1][j + 1]);
                    //     tTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i - 1][j + 1]);
                    // }

                    priorOne[i][j] = tOne;
                    priorTwo[i][j] = tTwo;
                    
                    funcRasdelOne += tOne;
                    funcRasdelTwo += tTwo;
                }
            }

            double w = 10000;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    priorOne[i][j] /= funcRasdelOne;
                    priorTwo[i][j] /= funcRasdelTwo;
                    
                    maskTOne[i][j] += w * priorOne[i][j]; 
                    maskTTwo[i][j] += w * priorTwo[i][j];
                    
                    Color one = Color.FromArgb((byte) (maskTOne[i][j] * 255), (byte) (maskTOne[i][j] * 255),
                        (byte) (maskTOne[i][j] * 255));
                    Color two = Color.FromArgb((byte) (maskTTwo[i][j] * 255), (byte) (maskTTwo[i][j] * 255),
                        (byte) (maskTTwo[i][j] * 255));
                        
                    maskOne.SetPixel(j, i, one);
                    maskTwo.SetPixel(j, i, two);
                }
            }
            
            maskOne.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/maskOne.png", ImageFormat.Png);
            maskTwo.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/maskTwo.png", ImageFormat.Png);

            Bitmap resMask = new Bitmap(width, height);
            
            byte [][] mask = new byte[height][];

            for (int i = 0; i < height; i++)
            { 
                mask[i] = new byte[width];
                for (int j = 0; j < width; j++)
                { 
                    byte m = maskTTwo[i][j].CompareTo(maskTOne[i][j]) >= 0? (byte) 0 : (byte) 1;
                    mask[i][j] = m;
                    resMask.SetPixel(j, i, Color.FromArgb((byte) (m * 255), (byte) (m * 255), (byte) (m * 255)));
                }
            }
            
            
            //Console.WriteLine(aprioaProb);
            //
            // double EnergyOne = 0;
            // double EnergyTwo = 0;
            //
            // double AllEner = 0;
            //
            // double[][] Energes = new double[height][];
            //
            // double w = 100;
            // double tempOne;
            // double tempTwo;
            // double temp;
            //
            // for (int i = 0; i < height; i++)
            // {
            //     tempOne = 0;
            //     tempTwo = 0;
            //     temp = 0;
            //     Energes[i] = new double[width];
            //     for (int j = 0; j < width; j++)
            //     {
            //         if (i != 0)
            //         {
            //             tempOne += Math.Abs(maskTOne[i][j] - maskTOne[i - 1][j]);
            //             tempTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i - 1][j]);
            //             temp += Math.Abs(mask[i][j] - mask[i - 1][j]);
            //         }
            //         if (j != 0)
            //         {
            //             tempOne += Math.Abs(maskTOne[i][j] - maskTOne[i][j - 1]);
            //             tempTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i][j - 1]);
            //             temp += Math.Abs(mask[i][j] - mask[i][j - 1]);
            //         }
            //
            //         if (j != width - 1)
            //         {
            //             tempOne += Math.Abs(maskTOne[i][j] - maskTOne[i][j + 1]);
            //             tempTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i][j + 1]);
            //             temp += Math.Abs(mask[i][j] - mask[i][j + 1]);
            //         }
            //
            //         if (i != height - 1)
            //         {
            //             tempOne += Math.Abs(maskTOne[i][j] - maskTOne[i + 1][j]);
            //             tempTwo += Math.Abs(maskTTwo[i][j] - maskTTwo[i + 1][j]);
            //             temp += Math.Abs(mask[i][j] - mask[i + 1][j]);
            //         }
            //         Energes[i][j] = w * temp;
            //
            //         EnergyOne += w * tempOne;
            //         double mOne = maskTOne[i][j];
            //         if (mOne != 0)
            //         {
            //             EnergyOne -= Math.Log(mOne) * mOne;
            //             EnergyOne -= Math.Log(mOne) * (1 - mOne);
            //             
            //             Energes[i][j] -= Math.Log(mOne);
            //         }
            //         // EnergyOne -= Math.Log(maskTOne[i][j]) * maskTOne[i][j];
            //         // EnergyOne -= Math.Log(maskTOne[i][j]) * (1 - maskTOne[i][j]);
            //         
            //         EnergyTwo += w * tempTwo;
            //         double mTwo = maskTTwo[i][j];
            //         if (mTwo != 0)
            //         {
            //             EnergyTwo -= Math.Log(mTwo) * mTwo;
            //             EnergyTwo -= Math.Log(mTwo) * (1 - mTwo);
            //             
            //             Energes[i][j] -= Math.Log(mTwo) * (1 - mask[i][j]);
            //         }
            //         // EnergyTwo -= Math.Log(maskTTwo[i][j]) * maskTTwo[i][j];
            //         // EnergyTwo -= Math.Log(maskTTwo[i][j]) * (1 - maskTTwo[i][j]);
            //         AllEner += Energes[i][j];
            //     }
            // }
            //
            // double TotalEnergy = EnergyTwo + EnergyOne;
            //
            // Console.WriteLine("TotalEnergy: " + TotalEnergy);
            //
            // double [][] ProbEnergy = new double[height][];
            // Bitmap EnergyMask = new Bitmap(bitmap.Width, bitmap.Height);
            //
            // for (int i = 0; i < height; i++)
            // { 
            //     ProbEnergy[i] = new Double[width];
            //     for (int j = 0; j < width; j++)
            //     {
            //         ProbEnergy[i][j] = Energes[i][j] / AllEner;
            //         
            //         Color one = Color.FromArgb((byte) (ProbEnergy[i][j] * 255), (byte) (ProbEnergy[i][j] * 255),
            //             (byte) (ProbEnergy[i][j] * 255));
            //
            //         EnergyMask.SetPixel(j, i, one);
            //     }
            // }
            // EnergyMask.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/EnergyMask.png", ImageFormat.Png);
            //
            //

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color c = bitmap.GetPixel(j, i);
                    bitmap.SetPixel(j, i, Color.FromArgb(c.R * mask[i][j], c.G * mask[i][j], c.B * mask[i][j]));
                }
            }
            

            bitmap.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/maskAfter.png", ImageFormat.Png);
            resMask.Save("/Users/elizavetavolianica/RiderProjects/Image_segmentation/Image_segmentation/mask.png", ImageFormat.Png);
            bitmap.Dispose();
        }
    }
}