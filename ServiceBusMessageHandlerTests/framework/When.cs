using ServicebusMessageHandler;

namespace ServiceBusMessageHandlerTests.framework
{
    public class When
    {
        private readonly TestHost _host;

        public When(TestHost host)
        {
            _host = host;
        }

        public static When OnThe(TestHost host) => new(host);
        public When And() => this;

        public When AMessageIsSentToTestQueue1(string testPayload, bool simulateRetries = false)
        {
            var payload = new TestQueueMessage(testPayload);

            if (simulateRetries)
                _host.TestableQueue1MessageProcessor.SendMessageWithRetries(payload, _host.NumberOfSimulatedServiceBusMessageRetries).GetAwaiter().GetResult();
            else
                _host.TestableQueue1MessageProcessor.SendMessage(payload).GetAwaiter().GetResult();

            return this;
        }
    }
}