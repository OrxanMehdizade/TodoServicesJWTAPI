using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;
using TodoServicesJWTAPI;
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


Log.Logger=new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

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
