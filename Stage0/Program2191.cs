using System;
namespace Targil0 {

    partial class Program
{
     static void Main(string[] args)
        {
            Welcom2191();
            Welcom0767();

            Console.ReadKey();
        }
         static partial void Welcom0767();

        private static void Welcom2191()
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Enter your name: ");
            string UserName = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", UserName);
        }
    }
}