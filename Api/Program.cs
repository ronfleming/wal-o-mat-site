using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString")
            ?? "UseDevelopmentStorage=true";
        var tableClient = new TableClient(connectionString, "QuizResults");
        tableClient.CreateIfNotExists();
        services.AddSingleton(tableClient);
    })
    .Build();

host.Run();

