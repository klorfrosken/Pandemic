using System;
using Pandemic.Managers;

namespace Pandemic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            new GameManager();

            Console.WriteLine("The Game has ended. Thanks for playing! \nGoodbye!");
        }
    }
}
