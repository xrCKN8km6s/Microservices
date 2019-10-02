using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Orders.API
{
    [UsedImplicitly]
    public static class Program
    {
        public static int Main(string[] args)
        {
            static IConfiguration BuildConfiguration(string[] a)
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                        true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(a)
                    .Build();
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(BuildConfiguration(args))
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); })
                .UseSerilog();
    }
}
