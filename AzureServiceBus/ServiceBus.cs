using Azure.Core;
using Azure.Messaging.ServiceBus;
using System;
using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;

namespace AzureServiceBus
{
    public class ServiceBus
    {
        public ServiceBusClient client;

        public ServiceBusSender sender;
        public ServiceBusProcessor processor;

        const int numOfMessages = 3;

        public ServiceBus()
        {
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            client = new ServiceBusClient("Endpoint=sb://crreembolso.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=e8Ec6CbL58Y8f1S3Cyq+2lvf3nadJY3e3+ASbPE9VbM=", clientOptions);
            sender = client.CreateSender("consultaticketzendesk");
            processor = client.CreateProcessor("consultaticketzendesk", new ServiceBusProcessorOptions());

        }

        public async void CreateMessage()
        {
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            string guid = Guid.NewGuid().ToString();
            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {guid}")))
            {
                // if it is too large for the batch
                throw new Exception($"The message {guid} is too large to fit in the batch.");
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"{guid}");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

        }



        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // complete the message. message is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        // handle any errors when receiving messages
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }


        public async void GetMessage()
        {
            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }

        }


    }
}
