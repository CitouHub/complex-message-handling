using CMH.PriorityHandler.Infrastructure;
using CMH.PriorityHandler.Service;
using CMH.PriorityHandler.Tool;
using Microsoft.Extensions.Azure;

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

builder.Services.AddSingleton<IQueueCache, QueueCache>();
builder.Services.AddSingleton<Config>();

var app = builder.Build();

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