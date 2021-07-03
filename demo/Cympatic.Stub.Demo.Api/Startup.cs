using Cympatic.Stub.Demo.Api.Filters;
using Cympatic.Stub.Demo.Api.Services;
using Cympatic.Stub.Demo.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace Cympatic.Stub.Demo.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = AppDomain.CurrentDomain.FriendlyName
                });
                options.OperationFilter<DemoIdentifierHeaderOperationFilter>();

                // Configure Swagger to use the xml documentation file
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                options.IncludeXmlComments(xmlFile);
            });


            services
                .AddOptions<DemoSettings>()
                .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(DemoSettings)).Bind(options));

            services.AddHttpClient<DemoApiService>((serviceProvider, config) =>
            {
                var demoSettings = serviceProvider.GetRequiredService<IOptions<DemoSettings>>().Value;

                if (string.IsNullOrWhiteSpace(demoSettings.DemoBaseUrl))
                {
                    throw new ArgumentException($"{nameof(demoSettings.DemoBaseUrl)} must be provided");
                }

                config.BaseAddress = new Uri(demoSettings.DemoBaseUrl);
                config.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", AppDomain.CurrentDomain.FriendlyName);
                c.DisplayRequestDuration();
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
