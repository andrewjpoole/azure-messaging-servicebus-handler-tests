namespace ServicebusMessageHandler
{
    public class PermanentException : Exception
    {
        public PermanentException(string message) : base(message)
        {
        }
    }
}
