using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PositionManagement.Service.Api;
using PositionManagement.Service.Core;
using PositionManagement.Service.Data;
using PositionManagement.Service.Impl;

namespace PositionManagement.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TradingDbContext>(options =>
            {
                options.UseInMemoryDatabase("TradingDb");
            });

            builder.Services.AddSingleton<INetQuantityCalculator, NetQuantityCalculatorImpl>();
            builder.Services.AddScoped<IPositionService, PositionServiceImpl>();
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("NgClient", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();
            app.UseCors("NgClient");
            app.MapHub<SignalRHub>("/signalrhub");
            app.Run();
        }
    }
}
