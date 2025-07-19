using System;

namespace ProjectA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ProjectA - Example Application");
            Console.WriteLine($"Built at: {DateTime.Now}");
            
            // Simulate some work
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Processing item {i + 1}...");
                System.Threading.Thread.Sleep(100);
            }
            
            Console.WriteLine("ProjectA completed successfully!");
        }
    }
}