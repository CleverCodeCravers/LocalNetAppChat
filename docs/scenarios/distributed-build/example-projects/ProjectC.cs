using System;
using System.Text.Json;
using ProjectB;

namespace ProjectC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ProjectC - Calculator Test Application");
            
            var calculator = new Calculator();
            
            // Test calculations
            var testData = new
            {
                Addition = calculator.Add(10, 5),
                Subtraction = calculator.Subtract(10, 5),
                Multiplication = calculator.Multiply(10, 5),
                Division = calculator.Divide(10, 5),
                Average = calculator.CalculateAverage(10, 20, 30, 40, 50)
            };
            
            // Serialize to JSON
            var json = JsonSerializer.Serialize(testData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            Console.WriteLine("Calculation Results:");
            Console.WriteLine(json);
            
            Console.WriteLine("\nProjectC completed successfully!");
        }
    }
}