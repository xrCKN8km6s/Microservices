// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;

namespace IdentityServer
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            static IConfiguration BuildConfiguration(string[] a)
            {
                return new ConfigurationBuilder()
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
                })
                .UseSerilog();
    }
}