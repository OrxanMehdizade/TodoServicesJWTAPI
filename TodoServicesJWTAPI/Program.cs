using Microsoft.EntityFrameworkCore.Infrastructure;
using RabbitMQ.Client;
using Serilog;
using TodoServicesJWTAPI;
using TodoServicesJWTAPI.Configurations;
using TodoServicesJWTAPI.Services.BackgroundServices;
using TodoServicesJWTAPI.Services.Product;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwagger();
builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddMemoryCache();
builder.Services.AddTodoContext(builder.Configuration);
builder.Services.AddBackgroundServices();
builder.Host.ConfigureSerilog();


var section = builder.Configuration.GetSection("RabbitMQ");
var rabbitMQConfiguration = new RabbitMQConfiguration();
section.Bind(rabbitMQConfiguration);

builder.Services.AddSingleton(rabbitMQConfiguration);

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName=rabbitMQConfiguration.HostName, 
        UserName=rabbitMQConfiguration.UserName,
        Password = rabbitMQConfiguration.Password,
        Port =rabbitMQConfiguration.Port
    };
    return factory;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
