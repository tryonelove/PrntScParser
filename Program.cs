using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

namespace ImageParser
{
    class Program
    {
        static void Main(string[] args)
        {
            int imageCount;
            int threadCount;
            Console.WriteLine("How many images you'd like to download?");
            imageCount = int.Parse(Console.ReadLine());
            Console.WriteLine("How many threads you'd like to use?");
            threadCount = int.Parse(Console.ReadLine());

            // Create folder to save images
            string path = "Images";
            bool exists = System.IO.Directory.Exists(path);
            if(!exists)
                System.IO.Directory.CreateDirectory(path);

            RunParser(imageCount, threadCount);     
            Console.WriteLine("Done! Press any key to close app.");
            Console.ReadKey();
        }

        static void RunParser(int imageCount, int threadCount)
        {
            Parser parser = new Parser("https://prnt.sc/");
            int imagePerThread = imageCount / threadCount;
            List<Thread> threads = new List<Thread>();
            for(int i=0; i<threadCount && i<imageCount; i++)
            {
                Thread downloader = new Thread(() => parser.GetImages(imagePerThread == 0 ? 1 : imagePerThread));
                threads.Add(downloader);
                downloader.Start();
            }

            foreach(Thread t in threads)
            {
                t.Join();
            }
        }
    }
}
