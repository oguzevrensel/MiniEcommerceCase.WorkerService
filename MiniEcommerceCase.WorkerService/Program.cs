using Microsoft.Extensions.Configuration;
using MiniEcommerceCase.WorkerService.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var listener = new RabbitMqListener(configuration);
listener.StartListening();
