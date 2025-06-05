
using System.Text.Json.Serialization;
using FoodOrdering.Data;
using FoodOrdering.Hubs;
using FoodOrdering.Workers;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace FoodOrdering;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        //     .AddJsonOptions(options =>
        //     {
        //         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //     });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=mydatabase.sqlite"));
        builder.Services.AddHostedService<SeedingWorker>();
        // builder.Services.AddMvc()
        //     .AddJsonOptions(options =>
        //         {
        //             options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //         });
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policyBuilder => 
                policyBuilder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
        });
        builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            })
            .AddJsonProtocol(opt =>
            {
                opt.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // for scaleout using redis, see: https://learn.microsoft.com/en-us/aspnet/core/signalr/redis-backplane?view=aspnetcore-9.0
            // .AddStackExchangeRedis("connectionString", options =>
            // {
            //     options.Configuration.ChannelPrefix = RedisChannel.Literal("MyApp");
            // });
        
        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseCors("CorsPolicy");

        app.UseAuthorization();
        
        app.MapControllers();

        app.MapHub<FoodHub>("/foodhub");

        app.Run();
    }
}
