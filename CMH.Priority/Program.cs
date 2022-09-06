using Microsoft.Extensions.Azure;

using CMH.Priority.Infrastructure;
using CMH.Priority.Service;
using CMH.Priority.Util;
using CMH.Data.Repository;
using CMH.Data.Model;
using CMH.Common.Enum;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAzureClients(_ =>
{
    _.AddServiceBusClient(builder.Configuration.GetValue<string>("ServiceBus:ConnectionString"));
    _.AddServiceBusAdministrationClient(builder.Configuration.GetValue<string>("ServiceBus:ConnectionString"));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<MessageSeederService>();
builder.Services.AddHostedService<PriorityService>();
builder.Services.AddHostedService<QueueCacheService>();

builder.Services.AddSingleton<IRuntimeStatisticsRepository, RuntimeStatisticsRepository>();
builder.Services.AddSingleton<IMessageStatisticsRepository, MessageStatisticsRepository>();
builder.Services.AddSingleton<IDataSourceRepository, DataSourceRepository>();
builder.Services.AddSingleton<IQueueCache, QueueCache>();
builder.Services.AddSingleton<Config>();

var app = builder.Build();

InitiateDataSources(app, builder.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void InitiateDataSources(WebApplication app, ConfigurationManager configuration)
{
    using var scope = app.Services.CreateScope();
    var dataSourceRepository = scope.ServiceProvider.GetRequiredService<IDataSourceRepository>();

    var dataSourceDefaults = configuration.GetSection("Repository:DataSource:Default").Get<List<string>>();
    if (dataSourceDefaults != null && dataSourceDefaults.Any())
    {
        foreach (var dataSourceDefault in dataSourceDefaults)
        {
            var values = dataSourceDefault.Split(';');
            var dataSource = new DataSource()
            {
                Id = int.Parse(values.First(_ => _.StartsWith("Id=")).Split('=')[1] ?? "0"),
                FailRate = double.Parse(values.First(_ => _.StartsWith("FailRate=")).Split('=')[1] ?? "0.0", CultureInfo.InvariantCulture),
                MinProcessTime = int.Parse(values.First(_ => _.StartsWith("MinProcessTime=")).Split('=')[1] ?? "0"),
                MaxProcessTime = int.Parse(values.First(_ => _.StartsWith("MaxProcessTime=")).Split('=')[1] ?? "0"),
                ProcessChannel = Enum.Parse<ProcessChannel>(values.First(_ => _.StartsWith("ProcessChannel=")).Split('=')[1])
            };

            dataSourceRepository.Add(dataSource);
        }
    }
}