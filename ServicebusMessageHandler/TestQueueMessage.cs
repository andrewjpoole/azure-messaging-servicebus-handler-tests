namespace ServicebusMessageHandler
{
    public class TestQueueMessage
    {
        public string Message { get; }

        public TestQueueMessage(string message)
        {
            Message = message;
        }
    }
}