using System;
using Azure.Messaging.ServiceBus;
using ServicebusMessageHandler;
using AzureMessagingServiceBusTestsSupport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ServiceBusMessageHandlerTests.framework
{
    public class TestHost : IDisposable
    {
        private readonly IHost _server;
        public Mock<ServiceBusSender> MockTestQueue2Sender { get; } = new();
        public TestableServiceBusProcessor TestableQueue1MessageProcessor { get; } = new();
        public Mock<IWidget> MockWidgetService { get; } = new();
        public IServiceProvider Services => _server.Services;
        public int NumberOfSimulatedServiceBusMessageRetries = 5;
        public int InitialRetryDelayForServiceBusMessageRetriesInMs = 100;

        public TestHost()
        {
            var builder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer()
                        .ConfigureServices((context, services) => Program.ConfigureHost(services, context.Configuration))
                        .ConfigureTestServices((services) =>
                        {
                            var client = new Mock<ServiceBusClient>();

                            client.Setup(t => t.CreateSender(It.Is < string > (s => s == "testQueue2")))
                                .Returns(MockTestQueue2Sender.Object);

                            client.Setup(t => t.CreateProcessor(It.Is<string>(s => s == $"testQueue1")))
                                .Returns(TestableQueue1MessageProcessor);

                            services.AddSingleton(client.Object);

                            // register other test services here...
                            services.AddSingleton(MockWidgetService.Object);

                        }).Configure(app => { });
                });

            _server = builder.Build();
            _server.StartAsync();
        }

        public void Dispose()
        {
            _server?.StopAsync().GetAwaiter().GetResult();
            _server?.Dispose();
        }
    }
}