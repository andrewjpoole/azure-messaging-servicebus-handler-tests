using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace AzureMessagingServiceBusTestsSupport
{
    public class TestableServiceBusProcessor : ServiceBusProcessor
    {
        public List<TestableProcessMessageEventArgs> MessageDeliveryAttempts = new();

        public async Task SendMessageWithRetries<T>(T payload, int maxDeliveryCount = 5)
        {
            for (var attempt = 1; attempt <= maxDeliveryCount; attempt++)
            {
                // Don't send retry if the message was sent already and completed
                if (MessageDeliveryAttempts.Any() && MessageDeliveryAttempts.Last().WasCompleted)
                    return;

                await SendMessage(payload, attempt);

                // Simulate the message being deadlettered if max delivery count is hit
                if (attempt == maxDeliveryCount)
                    MessageDeliveryAttempts.Last().WasDeadLettered = true;

            }
        }

        public async Task SendMessage<T>(T payload, int attempt = 1)
        {
            var args = CreateMessageArgs(payload, attempt);
            MessageDeliveryAttempts.Add((TestableProcessMessageEventArgs)args);
            await base.OnProcessMessageAsync(args);
        }

        public ProcessMessageEventArgs CreateMessageArgs<T>(T payload, int deliveryCount = 1)
        {
            var payloadJson = JsonSerializer.Serialize(payload);

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(payloadJson),
                deliveryCount: deliveryCount);

            var args = new TestableProcessMessageEventArgs(message);

            return args;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task StartProcessingAsync(CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }
    }
}