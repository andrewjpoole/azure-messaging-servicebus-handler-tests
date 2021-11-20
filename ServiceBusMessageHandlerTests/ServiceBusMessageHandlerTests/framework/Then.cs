using Xunit;
using System.Linq;
using System.Collections.Generic;
using System;
using FluentAssertions;
using Moq;
using Azure.Messaging.ServiceBus;
using System.Threading;

namespace ServiceBusMessageHandlerTests.framework
{
    public class Then
    {
        private readonly TestHost _host;

        public Then(TestHost host)
        {
            _host = host;
        }

        public static Then OnThe(TestHost host) => new(host);
        public Then And() => this;

        public Then TheMessageWasCompleted()
        {
            _host.TestableQueue1MessageProcessor.MessageDeliveryAttempts.Last().WasCompleted.Should().BeTrue();
            return this;
        }

        public Then TheMessageWasDeadLettered()
        {
            _host.TestableQueue1MessageProcessor.MessageDeliveryAttempts.Last().WasDeadLettered.Should().BeTrue();
            return this;
        }

        public Then TheMessageWasRetried(int times)
        {
            _host.TestableQueue1MessageProcessor.MessageDeliveryAttempts.Count.Should().Be(times);
            return this;
        }

        public Then TheWidgetServiceWasCalled(int times) 
        {
            _host.MockWidgetService.Verify(x => x.DoSomething(It.IsAny<string>()), Times.Exactly(times));
            return this;
        }

        public Then AMessageWasSentToTestQueue2(int times = 1)
        {
            _host.MockTestQueue2Sender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(times));
            return this;
        }

        public Then TheRetriedMessagesHadIncreasingDelays()
        {
            var delays = new List<TimeSpan>();
            for (var i = 0; i < _host.TestableQueue1MessageProcessor.MessageDeliveryAttempts.Count - 1; i++)
            {
                delays.Add(_host.TestableQueue1MessageProcessor.MessageDeliveryAttempts[i + 1].Created -
                           _host.TestableQueue1MessageProcessor.MessageDeliveryAttempts[i].Created);
            }

            for (var i = 0; i < delays.Count - 1; i++)
            {
                Assert.True(delays[i + 1] > delays[i], $"the interval of retry{i+1}({delays[i+1].TotalMilliseconds}ms) was not larger than the previous retry({delays[i].TotalMilliseconds}ms)");
            }

            return this;
        }
    }
}