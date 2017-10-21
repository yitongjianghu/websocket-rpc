﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Net.WebSockets;
using WebSocketRPC;

namespace AspRpc
{
    class Startup
    {
        ReportingService reportingService = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

     
            app.UseMvc();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Site")),
                RequestPath = "/Site"
            });

            reportingService = new ReportingService();
            app.UseWebSockets();
            app.MapWebSocketRPC("/reportingService", (hc, c) => c.Bind<ReportingService, IClientUpdate>(reportingService));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            //generate js code
            File.WriteAllText($"./Site/{nameof(ReportingService)}.js", RPCJs.GenerateCallerWithDoc<ReportingService>());

            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .Build()
                   .Run();
        }
    }
}