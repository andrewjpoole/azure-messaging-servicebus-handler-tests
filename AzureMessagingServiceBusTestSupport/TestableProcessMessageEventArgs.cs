using Azure.Messaging.ServiceBus;

namespace AzureMessagingServiceBusTestsSupport
{
    public class TestableProcessMessageEventArgs : ProcessMessageEventArgs
    {
        public bool WasCompleted;
        public bool WasDeadLettered;
        public DateTime Created;

        public TestableProcessMessageEventArgs(ServiceBusReceivedMessage message) : base(message, null, CancellationToken.None)
        {
            Created = DateTime.Now;
        }

        public override Task CompleteMessageAsync(ServiceBusReceivedMessage message,
            CancellationToken cancellationToken = new CancellationToken())
        {
            WasCompleted = true;
            return Task.CompletedTask;
        }

        public override Task DeadLetterMessageAsync(ServiceBusReceivedMessage message, string deadLetterReason,
            string deadLetterErrorDescription = null, CancellationToken cancellationToken = new())
        {
            WasDeadLettered = true;
            return Task.CompletedTask;
        }

        public override Task AbandonMessageAsync(ServiceBusReceivedMessage message, IDictionary<string, object> propertiesToModify = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }
    }
}