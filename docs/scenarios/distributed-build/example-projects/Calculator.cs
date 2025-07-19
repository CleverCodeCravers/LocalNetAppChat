using System;

namespace ProjectB
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public double Divide(int a, int b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero");
            }
            return (double)a / b;
        }

        public double CalculateAverage(params int[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }

            int sum = 0;
            foreach (int num in numbers)
            {
                sum += num;
            }

            return (double)sum / numbers.Length;
        }
    }
}