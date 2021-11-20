using ServiceBusMessageHandlerTests.framework;
using Xunit;

namespace ServiceBusMessageHandlerTests
{
    public class MessageHandlerTests
    {
        [Fact]
        public void a_message_sent_to_the_queue_is_handled_and_completed()
        {
            using var host = new TestHost();

            Given.OnThe(host)
                .ATestPayloadIsGenerated(out var testPayload);

            When.OnThe(host)
                .AMessageIsSentToTestQueue1(testPayload);

            Then.OnThe(host)
                .TheWidgetServiceWasCalled(times: 1)
                .And().AMessageWasSentToTestQueue2()
                .And().TheMessageWasCompleted();
        }

        [Fact]
        public void a_message_sent_to_the_queue_is_handled_and_when_permanentException_is_thrown_is_dead_lettered()
        {
            using var host = new TestHost();

            Given.OnThe(host)
                .ATestPayloadIsGenerated(out var testPayload)
                .And().TheWidgetWillThrowAPermanentException();

            When.OnThe(host)
                .AMessageIsSentToTestQueue1(testPayload);

            Then.OnThe(host)
                .AMessageWasSentToTestQueue2(times: 0)
                .And().TheMessageWasDeadLettered();
        }

        [Fact]
        public void a_message_sent_to_the_queue_is_handled_and_when_transientException_is_thrown_is_retried_and_is_eventually_completed()
        {
            using var host = new TestHost();

            Given.OnThe(host)
                .ATestPayloadIsGenerated(out var testPayload)
                .And().TheWidgetWillThrowANumberOfTransientExceptions(times:3);

            When.OnThe(host)
                .AMessageIsSentToTestQueue1(testPayload, simulateRetries: true);

            Then.OnThe(host)
                .TheMessageWasRetried(times:4)
                .And().TheWidgetServiceWasCalled(times:4)
                .And().TheRetriedMessagesHadIncreasingDelays()
                .And().AMessageWasSentToTestQueue2(times:1)
                .And().TheMessageWasCompleted();
        }

        [Fact]
        public void a_message_sent_to_the_queue_is_handled_and_when_5_transientExceptions_are_thrown_is_eventually_deadlettered()
        {
            using var host = new TestHost();

            Given.OnThe(host)
                .ATestPayloadIsGenerated(out var testPayload)
                .And().TheWidgetWillThrowANumberOfTransientExceptions(times: 5);

            When.OnThe(host)
                .AMessageIsSentToTestQueue1(testPayload, simulateRetries: true);

            Then.OnThe(host)
                .TheMessageWasRetried(times: 5)
                .And().AMessageWasSentToTestQueue2(times:0)
                .And().TheMessageWasDeadLettered();
        }
    }
}