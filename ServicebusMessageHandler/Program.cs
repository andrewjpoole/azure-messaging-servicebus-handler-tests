using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServicebusMessageHandler // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to start service {ex}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, collection) => ConfigureHost(collection, context.Configuration));

        public static void ConfigureHost(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp => new ServiceBusClient("connectionString"));
            services.AddHostedService<TestQueue1Handler>();
            services.AddSingleton<ITestQueue2Sender, TestQueue2Sender>();
        }
    }
}




