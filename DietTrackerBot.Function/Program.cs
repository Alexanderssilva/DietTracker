using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Application;
using DietTracker.Infra.Context;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<MongoContext>();
        services.AddSingleton<IDietTrackerApplication, DietTrackerApplication>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
