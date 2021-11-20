using Azure.Messaging.ServiceBus;

namespace ServicebusMessageHandler
{
    public class TestQueue2Sender : ITestQueue2Sender
    {
        private const string QueueName = "testQueue2";
        private readonly ServiceBusSender _sender;

        public TestQueue2Sender(ServiceBusClient serviceBusClient)
        {
            _sender = serviceBusClient.CreateSender(QueueName);
        }

        public async Task SendMessage(string message)
        {
            var serviceBusMessage = new ServiceBusMessage(message)
            {
                ContentType = "application/json"
            };

            await _sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
