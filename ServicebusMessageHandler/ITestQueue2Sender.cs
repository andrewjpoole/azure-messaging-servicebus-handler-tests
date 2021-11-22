namespace ServicebusMessageHandler
{
    public interface ITestQueue2Sender 
    {
        Task SendMessage(string message);
    }
}
