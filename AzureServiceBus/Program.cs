using AzureServiceBus;
using System;
using System.Threading.Tasks;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine("Fin");

        for (int i = 0; i < 100; i++)
        {
            var bus = new ServiceBus();
            bus.CreateMessage();
            Task.Delay(5);
        }

        Console.ReadLine();
        

    }
}