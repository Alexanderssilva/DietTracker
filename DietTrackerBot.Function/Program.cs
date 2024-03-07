using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Application;
using DietTrackerBot.Infra.Context;
using DietTrackerBot.Infra.Interfaces;
using DietTrackerBot.Infra;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<MongoContext>();
        services.AddSingleton<IDietTrackerApplication, DietTrackerApplication>();
        services.AddSingleton<IDietTrackerRepository, DietTrackerRepository>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
