
using BLOG.Data;
using BLOG.Models;
using BLOG.Services;
using BLOG.Services.Helpers;
using BLOG.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BLOG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            /*//logs only run in console
            builder.Host.ConfigureLogging(loggingprovider =>
            {
                loggingprovider.ClearProviders();
                loggingprovider.AddConsole();
                loggingprovider.AddDebug();
            });*/
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IPostService, PostService>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {          
                // DateTime parsing in ISO 8601 (default)
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            
                // Optional: Customize DateTime globally
                options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter("yyyy-MM-ddTHH:mm:ssZ")); // adjust as needed
            });
            builder.Services.AddDbContext<Data.BloggingContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
           /* app.Logger.LogDebug("Log-debug");
            app.Logger.LogInformation("log-information");
            app.Logger.LogCritical("Log-critical");
            app.Logger.LogWarning("log-warning");*/
            
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
