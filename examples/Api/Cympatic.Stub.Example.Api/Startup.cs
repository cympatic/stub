using Cympatic.Extensions.Http;
using Cympatic.Stub.Example.Api.Filters;
using Cympatic.Stub.Example.Api.Services;
using Cympatic.Stub.Example.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Cympatic.Stub.Example.Api
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
                options.OperationFilter<ExampleIdentifierHeaderOperationFilter>();

                // Configure Swagger to use the xml documentation file
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                options.IncludeXmlComments(xmlFile);
            });

            services.AddHttpContextAccessor();

            services
                .AddOptions<ExternalApiServiceSettings>()
                .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(ExternalApiServiceSettings)).Bind(options));

            services.AddHttpClient<ExternalApiService>((serviceProvider, config) =>
            {
                var externalApiServiceSettings = serviceProvider.GetRequiredService<IOptions<ExternalApiServiceSettings>>().Value;

                if (string.IsNullOrWhiteSpace(externalApiServiceSettings.Url))
                {
                    throw new ArgumentException($"{nameof(externalApiServiceSettings.Url)} must be provided");
                }

                config.BaseAddress = new Uri(externalApiServiceSettings.Url);
                config.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
                if (httpContext != null)
                {
                    foreach (var item in httpContext.Request.Headers.Where(header => !config.DefaultRequestHeaders.Contains(header.Key)))
                    {
                        config.DefaultRequestHeaders.Add(item.Key, item.Value as IEnumerable<string>);
                    }
                }
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

            app.UseDeveloperLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
