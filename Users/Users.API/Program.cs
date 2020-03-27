using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Users.API
{
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
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                    builder.UseKestrel((context, options) =>
                    {
                        //Windows doesn't yet support TLS1.3
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            options.ConfigureHttpsDefaults(httpsOptions =>
                            {
                                httpsOptions.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                            });
                        }
                    });
                })
                .UseSerilog();
    }
}
