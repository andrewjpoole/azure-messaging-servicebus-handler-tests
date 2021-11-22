using Moq;
using ServicebusMessageHandler;
using System;

namespace ServiceBusMessageHandlerTests.framework
{
    public class Given
    {
        private readonly TestHost _host;

        public Given(TestHost host)
        {
            _host = host;
        }

        public static Given OnThe(TestHost host) => new(host);
        public Given And() => this;

        public Given ATestPayloadIsGenerated(out string testPayload)
        {
            testPayload = Guid.NewGuid().ToString();

            return this;
        }
        
        public Given TheWidgetWillThrowAPermanentException()
        {
            _host.MockWidgetService.Setup(x => x.DoSomething(It.IsAny<string>())).Callback(() => 
            {
                throw new PermanentException("something permanent has gone wrong!");
            });

            return this;
        }

        public Given TheWidgetWillThrowANumberOfTransientExceptions(int times = 1)
        {
            var callbackExecutionCount = 0;
            var sequence = new MockSequence();
            for (int i = 1; i <= times; i++)
            {
                _host.MockWidgetService.InSequence(sequence)
                    .Setup(x => x.DoSomething(It.IsAny<string>()))
                    .Callback(() =>
                    {
                        callbackExecutionCount++;

                        if (callbackExecutionCount <= times)
                            throw new Exception("something transient has gone wrong!");

                        Console.WriteLine("All fine this time!");
                    });
            }

            return this;
        }
    }
}