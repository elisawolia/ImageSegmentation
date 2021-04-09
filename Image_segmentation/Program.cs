using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image_segmentation
{
    class Program
    {
        static void PrintHelp()
        {
            Console.WriteLine("To use this program you need to prepare two images: the image and the \"brush\". The seconds image is an image on the white background with two selected segments. You have to draw on the \"figure\" with the red color (r=255,g=0,b=0) and on the background you have to draw with blue color (r=0,g=0,b=255). Place images in the \"Input\" directory. The results will be in the \"Output\" directory. If you want to see the intermedia results, turn on the \"Debug\" option.");
        }
        static void PrintMenu()
        {
            Console.WriteLine("=== MENU ===");
            Console.WriteLine("1. Help");
            Console.WriteLine("2. Debug");
            Console.WriteLine("3. Set weights");
            Console.WriteLine("4. Run");
            Console.WriteLine("5. Load the image");
            Console.WriteLine("6. Load the brush");
            Console.WriteLine("7. Exit");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("IMAGE SEGMENTATION PROGRAM");
            Console.WriteLine("Author: Elizaveta Volyanitsa 4310");
            PrintHelp();
            PrintMenu();
            
            int option;
            
            ImageBuilder imageBuilder = new ImageBuilder();
            do
            {
                Console.WriteLine("\nEnter:");
                string line = Console.ReadLine();
                
                if (!Int32.TryParse(line, out option))
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }
                
                switch (option)
                {
                    case 1:
                        PrintHelp();
                        break;
                    case 2:
                        if (imageBuilder.turnDebug())
                        {
                            Console.WriteLine("Debug is turned on.");
                            Console.WriteLine("Run the program to see the debug files.");
                        }
                        else
                        {
                            Console.WriteLine("Debug is tuned off.");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Enter weight:");
                        line = Console.ReadLine();
                        int weight;
                        if (!Int32.TryParse(line, out weight))
                        {
                            Console.WriteLine("Weight should be an integer!");
                            break;
                        }

                        if (weight < 0)
                        {
                            Console.WriteLine("Negative value is not allowed! Default value is to be set.");
                        }
                        imageBuilder.setWeight(weight);
                        Console.WriteLine("Weight is set!");
                        break;
                    case 4:
                        Console.WriteLine("Running the program...");
                        if (imageBuilder.run())
                            Console.WriteLine("Saved to output!");
                        break;
                    case 5:
                        Console.WriteLine("Enter name of the file:");
                        String image = Console.ReadLine();
                        if (imageBuilder.loadImage(image))
                            Console.WriteLine("Successfully loaded!");
                        break;
                    case 6:
                        Console.WriteLine("Enter name of the brush file:");
                        String brush = Console.ReadLine();
                        if (imageBuilder.loadBrush(brush))
                            Console.WriteLine("Successfully loaded!");
                        break;
                    case 7:
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Not a valid option!");
                        PrintMenu();
                        break;
                }
            } while (option != 7);
        }
    }
}