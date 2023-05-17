using System;
using System.IO;
using System.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Mockus.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = GetLogger<Program>(GetConfig());
            var logger = Log.ForContext<Program>();
            logger.Information("Starting Mockus...Have a great mocks!");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static ILogger GetLogger<TProgram>(IConfigurationRoot config) where TProgram : class
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", typeof(TProgram).Assembly.GetName().Name)
                .CreateLogger();
        }

        private static IConfigurationRoot GetConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
            return config;
        }
    }
}