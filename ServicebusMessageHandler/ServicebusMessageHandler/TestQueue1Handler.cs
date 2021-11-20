using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;

namespace ServicebusMessageHandler
{
    public class TestQueue1Handler : ServiceBusMessageHandlerWorker
    {
        private readonly IWidget _widget;
        private readonly ITestQueue2Sender _testQueue2Sender;
        private const string QueueName = "testQueue1";

        public TestQueue1Handler(
            ServiceBusClient serviceBusClient, 
            IWidget widget,
            ITestQueue2Sender testQueue2Sender,
            int intInitialBackoffInMs = 250)
            :base(serviceBusClient, QueueName, intInitialBackoffInMs)
        {
            _widget = widget;
            _testQueue2Sender = testQueue2Sender;
        }


        public override async Task OnReceiveMessage(ProcessMessageEventArgs args)
        {
            var eventPayload = JsonSerializer.Deserialize<TestQueueMessage>(Encoding.UTF8.GetString(args.Message.Body));

            if (eventPayload is null)
                throw new PermanentException("Payload is null!");

            _widget.DoSomething(eventPayload.Message); // simulate some processing in a service

            await _testQueue2Sender.SendMessage($"Received: {eventPayload.Message}");
        }        
    }
}
