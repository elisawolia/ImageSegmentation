using System;
using System.Drawing;

namespace Image_segmentation
{
    public class Histogram
    {
        private int[] HistR = new int[256];
        private int[] HistG = new int[256];
        private int[] HistB = new int[256];
        private int Total { get; set; }

        public Histogram()
        {
            init();
            Total = 0;
        }

        private void init()
        {
            for (int i = 0; i < 256; i++)
            {
                HistR[i] = 0;
                HistG[i] = 0;
                HistB[i] = 0;
            }
        }

        public void Add(Color color)
        {
            HistR[color.R]++;
            HistG[color.G]++;
            HistB[color.B]++;
            Total++;
        }
        
        static int countSeveral(int n, int [] hist)
        {
            int min;
            int max;
            int sum = 0;

            min = n - 10 <= 0 ? 0 : n - 10;
            max = n + 10 >= 255 ? 255 : n + 10;

            while (min <= max)
            {
                sum += hist[n];
                min++;
            }
            return sum;
        }

        public Double getProb(Color color)
        {
            double prob = ((double)countSeveral(color.R, HistR) / Total)
                          * ((double)countSeveral(color.G, HistG) / Total)
                          * ((double)countSeveral(color.B, HistB) / Total);
            return Math.Pow(prob, 1d / 3);
        }
    }
}