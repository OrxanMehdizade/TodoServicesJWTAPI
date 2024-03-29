﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TodoServicesJWTAPI.Auth;
using TodoServicesJWTAPI.Data;
using TodoServicesJWTAPI.Models.Entities;
using TodoServicesJWTAPI.Providers;
using TodoServicesJWTAPI.Services.BackgroundServices;
using TodoServicesJWTAPI.Services.Product;
using TodoServicesJWTAPI.Services.RabbitMQ;
using TodoServicesJWTAPI.Services.Todo;

namespace TodoServicesJWTAPI
{
    public static class DI
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title= "My Api - V1",
                        Version="v1",

                    });
                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name="Authorization",
                    Type=SecuritySchemeType.ApiKey,
                    Scheme= "Bearer",
                    BearerFormat="JWT",
                    In=ParameterLocation.Header,
                    Description= "Standard Auth header using the Bearer scheme (\"Bearer {token}\")"
                });


            });
            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<TodoDbContext>();

            var jwtConfig = new JwtConfig();
            configuration.GetSection("JWT").Bind(jwtConfig);
            services.AddSingleton(jwtConfig);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, setup =>
            {
                setup.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer= true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience=jwtConfig.Audience,
                    ValidIssuer=jwtConfig.Issuer,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))

                };
            });

            services.AddAuthorization();
            return services;

        }

        public static IServiceCollection AddTodoContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TodoDbContext>(op => op.UseSqlServer(configuration.GetConnectionString("TodoConStr")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped <IJwtService, JwtService>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddScoped<IProductService , ProductService>();
            services.AddScoped<IRequestUserProvider, RequestUserProvider>();

            return services;
        }

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<TodoBackgroundService>();
            return services;
        }

        public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureLogging((context, loggingBuilder) =>
            {
                Log.Logger=new LoggerConfiguration()
                .ReadFrom.Configuration(context.Configuration)
                .CreateLogger();
                loggingBuilder.AddSerilog(dispose:true);
            });
            return hostBuilder;
        }
    }
}
