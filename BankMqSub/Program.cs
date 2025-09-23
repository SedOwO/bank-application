using BankMqSub;
using BankMqSub.Configs;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        var logger = NLog.LogManager.
            Setup()
            .LoadConfigurationFromFile("nlog.config")
            .GetCurrentClassLogger();
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        builder.Logging.AddNLog();

        // Register services below
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.AddHostedService<BankSubWorker>();

        var host = builder.Build();
        host.Run();

        NLog.LogManager.Shutdown();
    }
}