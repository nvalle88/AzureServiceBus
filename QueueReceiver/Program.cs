using AzureServiceBus;

namespace QueueReceiver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bus = new ServiceBusReceived();
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Procesar");
            bus.GetMessage();
            Console.ReadLine();
        }
    }
}