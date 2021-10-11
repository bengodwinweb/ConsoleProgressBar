using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgressBar
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var progressBar = new ConsoleProgressBar())
            {
                Console.Write("Performing task... ");

                for (int i = 0; i < 500; i++)
                {
                    progressBar.Report((i + 1) / 500d);
                    Thread.Sleep(10);
                }
            }
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
