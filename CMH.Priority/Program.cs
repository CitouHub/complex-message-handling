using System.Globalization;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Swashbuckle.AspNetCore.SwaggerUI;

using CMH.Priority.Service;
using CMH.Priority.Util;
using CMH.Data.Repository;
using CMH.Data.Model;
using CMH.Common.Variable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAzureClients(_ =>
{
    _.AddServiceBusClient(builder.Configuration.GetValue<string>("ServiceBus:ConnectionString"));
    _.AddServiceBusAdministrationClient(builder.Configuration.GetValue<string>("ServiceBus:ConnectionString"));
});

builder.Services.AddHttpClient("Function", _ =>
{
    _.BaseAddress = new Uri($"{builder.Configuration.GetValue<string>("Function:API")}");
    _.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<PriorityService>();

builder.Services.AddSingleton<IRuntimeStatisticsRepository, RuntimeStatisticsRepository>();
builder.Services.AddSingleton<IMessageStatisticsRepository, MessageStatisticsRepository>();
builder.Services.AddSingleton<IDataSourceRepository, DataSourceRepository>();
builder.Services.AddSingleton<IProcessChannelPolicyRepository, ProcessChannelPolicyRepository>();
builder.Services.AddSingleton<Config>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("CMH.Service initializing!");

InitiatePriorityQueues(app, builder.Configuration, scope, logger);
InitiateDataSources(app, builder.Configuration, scope, logger);
InitiateProcessChannelPolicies(app, builder.Configuration, scope, logger);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(_ => _.DocExpansion(DocExpansion.None));

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

logger.LogInformation("CMH.Service started!");

app.Run();

static void InitiatePriorityQueues(WebApplication app, ConfigurationManager configuration, IServiceScope scope, ILogger<Program> logger)
{
    var config = scope.ServiceProvider.GetRequiredService<Config>();
    var connectionString = configuration.GetValue<string>("ServiceBus:ConnectionString");
    var serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);

    logger.LogInformation("InitiatePriorityQueues starting");

    foreach (var priorityQueue in config.Priority.Queues)
    {
        if (serviceBusAdministrationClient.QueueExistsAsync(priorityQueue).Result == false)
        {
            serviceBusAdministrationClient.CreateQueueAsync(priorityQueue).Wait();
        }
    }

    logger.LogInformation("InitiatePriorityQueues finished");
}

static void InitiateDataSources(WebApplication app, ConfigurationManager configuration, IServiceScope scope, ILogger<Program> logger)
{
    var dataSourceRepository = scope.ServiceProvider.GetRequiredService<IDataSourceRepository>();

    logger.LogInformation("InitiateDataSources starting");

    var dataSourceDefaults = configuration.GetSection("Repository:DataSource:Default").Get<List<string>>();
    if (dataSourceDefaults != null && dataSourceDefaults.Any())
    {
        foreach (var dataSourceDefault in dataSourceDefaults)
        {
            var values = dataSourceDefault.Split(';');
            var dataSource = new DataSource()
            {
                Id = short.Parse(values.First(_ => _.StartsWith("Id=")).Split('=')[1]),
                FailRate = double.Parse(values.First(_ => _.StartsWith("FailRate=")).Split('=')[1], CultureInfo.InvariantCulture),
                MinProcessTime = int.Parse(values.First(_ => _.StartsWith("MinProcessTime=")).Split('=')[1]),
                MaxProcessTime = int.Parse(values.First(_ => _.StartsWith("MaxProcessTime=")).Split('=')[1]),
                ProcessChannel = Enum.Parse<ProcessChannel>(values.First(_ => _.StartsWith("ProcessChannel=")).Split('=')[1])
            };

            dataSourceRepository.Add(dataSource);
        }
    }

    logger.LogInformation("InitiateDataSources finished");
}

static void InitiateProcessChannelPolicies(WebApplication app, ConfigurationManager configuration, IServiceScope scope, ILogger<Program> logger)
{
    var processChannelPolicyRepository = scope.ServiceProvider.GetRequiredService<IProcessChannelPolicyRepository>();

    logger.LogInformation("InitiateProcessChannelPolicies starting");

    var processChannelPolicies = configuration.GetSection("Repository:ProcessChannelPolicies:Default").Get<List<string>>();
    if (processChannelPolicies != null && processChannelPolicies.Any())
    {
        var connectionString = configuration.GetValue<string>("ServiceBus:ConnectionString");
        var serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);

        foreach (var policy in processChannelPolicies)
        {
            var values = policy.Split(';');
            var processChannelPolicy = new ProcessChannelPolicy()
            {
                Name = Enum.Parse<ProcessChannel>(values.First(_ => _.StartsWith("Name=")).Split('=')[1]).ToString(),
                Tries = short.Parse(values.First(_ => _.StartsWith("Tries=")).Split('=')[1]),
                InitialSleepTime = short.Parse(values.First(_ => _.StartsWith("InitialSleepTime=")).Split('=')[1]),
                BackoffFactor = double.Parse(values.First(_ => _.StartsWith("BackoffFactor=")).Split('=')[1], CultureInfo.InvariantCulture)
            };

            var processChannelQueueName = $"{Queue.ProcessQueuePrefix}{processChannelPolicy.Name}";
            if (serviceBusAdministrationClient.QueueExistsAsync(processChannelQueueName).Result == false)
            {
                serviceBusAdministrationClient.CreateQueueAsync(processChannelQueueName).Wait();
            }

            processChannelPolicyRepository.Add(processChannelPolicy);
        }
    }

    logger.LogInformation("InitiateProcessChannelPolicies finished");
}