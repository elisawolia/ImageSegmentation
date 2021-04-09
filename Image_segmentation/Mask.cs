using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image_segmentation
{
    public class Mask
    {
        private Bitmap image;
        private Bitmap brush;

        private int height;
        private int width;
        
        private Histogram redHist = new Histogram();
        private Histogram blueHist = new Histogram();

        private double[][] redProbMask;
        private double[][] blueProbMask;

        private byte[][] mask;
        private double weight; 
        public Mask (Image image, Image brush, int weight)
        {
            this.image = new Bitmap(image);
            this.brush = new Bitmap(brush);

            this.weight = weight == -1 ? 0 : weight;

            height = image.Height;
            width = image.Width;
            
            buildHist();
            buildProbMasks();
            buildMask();
        }

        private void buildHist()
        {
            for (int i = 0; i < height; i++)
            { 
                for (int j = 0; j < width; j++)
                { 
                    byte red = brush.GetPixel(j, i).R;
                    byte blue = brush.GetPixel(j, i).B;
                    
                    if (blue == 255)
                        blueHist.Add(image.GetPixel(j, i));
                    else if (red == 255)
                        redHist.Add(image.GetPixel(j, i));
                }
            }
        }

        private void buildProbMasks()
        {
            redProbMask = new double[height][];
            blueProbMask = new double[height][];
            
            for (int i = 0; i < height; i++)
            { 
                redProbMask[i] = new Double[width]; 
                blueProbMask[i] = new Double[width]; 
                for (int j = 0; j < width; j++)
                {
                    Color o = image.GetPixel(j, i);
                    double probOne = redHist.getProb(o);
                    double probTwo = blueHist.getProb(o);

                    redProbMask[i][j] = probOne;
                    blueProbMask[i][j] = probTwo;
                }
            }
        }

        private void buildMask()
        {
            mask = new byte[height][];
            
            for (int i = 0; i < height; i++)
            { 
                mask[i] = new byte[width];
                for (int j = 0; j < width; j++)
                { 
                    byte m = blueProbMask[i][j].CompareTo(redProbMask[i][j]) >= 0? (byte) 0 : (byte) 1;
                    mask[i][j] = m;
                    
                    byte red = brush.GetPixel(j, i).R;
                    byte blue = brush.GetPixel(j, i).B;
                    
                    if (blue == 255)
                    {
                        mask[i][j] = 0;
                    } else if (red == 255)
                    {
                        mask[i][j] = 1;
                    }
                }
            }

            optimization();
        }

        private double pixelEnergy(int i, int j)
        {
            return (double.IsFinite(Math.Log(redProbMask[i][j]))
                ? Math.Log(redProbMask[i][j])
                : Math.Log(Double.Epsilon)) * mask[i][j]
                + (double.IsFinite(Math.Log(blueProbMask[i][j]))
                    ? Math.Log(blueProbMask[i][j])
                    : Math.Log(Double.Epsilon)) * (1 - mask[i][j]);
        }

        private double pixelDiff(int i, int j)
        {
            int diff = 0;

            if (i != 0)
                diff += Math.Abs(mask[i][j] - mask[i - 1][j]);
            if (j != 0)
                diff += Math.Abs(mask[i][j] - mask[i][j - 1]);
            if (j != width - 1)
                diff += Math.Abs(mask[i][j] - mask[i][j + 1]);
            if (i != height - 1)
                diff += Math.Abs(mask[i][j] - mask[i + 1][j]);

            return diff;
        }
        private void optimization()
        {
            double energy = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    energy -= pixelEnergy(i,j);
                    energy += weight * pixelDiff(i, j);
                }
            }

            double newEnergy = energy;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newEnergy = energy;
                    for (int k = (i > 0 ? i - 1 : i); k <= (i < height - 1 ? i + 1 : i); k++)
                    {
                        for (int l = (j > 0 ? j - 1 : j); l <= (j < width - 1 ? j + 1 : j); l++)
                        {
                            newEnergy += pixelEnergy(k, l);

                            newEnergy -= weight * pixelDiff(k, l);
                        }
                    }

                    mask[i][j] = (mask[i][j] == 0) ? (byte) 1 : (byte) 0;
                    
                    for (int k = (i > 0 ? i - 1 : i); k <= (i < height - 1 ? i + 1 : i); k++)
                    {
                        for (int l = (j > 0 ? j - 1 : j); l <= (j < width - 1 ? j + 1 : j); l++)
                        {
                            newEnergy -= pixelEnergy(k, l);

                            newEnergy += weight * pixelDiff(k, l);
                        }
                    }
                    
                    
                    if (energy < newEnergy)
                        mask[i][j] = (mask[i][j] == 0) ? (byte) 1 : (byte) 0;
                    else
                        energy = newEnergy;
                }
            }
        }

        public Bitmap getMask()
        {
            Bitmap resMask =  new Bitmap(width, height);
            
            for (int i = 0; i < height; i++)
            { 
                for (int j = 0; j < width; j++)
                { 
                    byte m = mask[i][j];
                    
                    resMask.SetPixel(j, i, Color.FromArgb((byte) (m * 255), (byte) (m * 255), (byte) (m * 255)));
                }
            }

            return resMask;
        }
        
        public Bitmap getProbMaskRed()
        {
            Bitmap redMask =  new Bitmap(width, height);
            
            for (int i = 0; i < height; i++)
            { 
                for (int j = 0; j < width; j++)
                {
                    double red = redProbMask[i][j];
                    
                    redMask.SetPixel(j, i, Color.FromArgb((byte) (red * 255),
                        (byte) (red * 255),
                        (byte) (red * 255)));
                }
            }

            return redMask;
        }
        
        public Bitmap getProbMaskBlue()
        {
            Bitmap blueMask =  new Bitmap(width, height);
            
            for (int i = 0; i < height; i++)
            { 
                for (int j = 0; j < width; j++)
                {
                    double blue = blueProbMask[i][j];

                    blueMask.SetPixel(j, i, Color.FromArgb((byte) (blue * 255),
                        (byte) (blue * 255),
                        (byte) (blue * 255)));
                }
            }

            return blueMask;
        }
        
        public Bitmap getImageWMask()
        {
            Bitmap resMask =  new Bitmap(width, height);
            
            for (int i = 0; i < height; i++)
            { 
                for (int j = 0; j < width; j++)
                { 
                    Color c = image.GetPixel(j, i);
                    resMask.SetPixel(j, i, Color.FromArgb(c.R * mask[i][j], c.G * mask[i][j], c.B * mask[i][j]));
                }
            }

            return resMask;
        }

        public void close()
        {
            image.Dispose();
            brush.Dispose();
        }
    }
}