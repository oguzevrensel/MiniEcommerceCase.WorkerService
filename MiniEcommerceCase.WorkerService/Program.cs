using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MiniEcommerceCase.WorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using MiniEcommerceCase.WorkerService.Interfaces;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build());

        services.AddSingleton<IRedisLogger, RedisLogger>(); 
        services.AddSingleton<RabbitMqListener>();           
    });

var host = builder.Build();
var listener = host.Services.GetRequiredService<RabbitMqListener>();
listener.StartListening();