using Cympatic.Extensions.Http;
using Cympatic.Stub.Server.Containers;
using Cympatic.Stub.Server.Filters;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Cympatic.Stub.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.OutputFormatters.RemoveType<StreamOutputFormatter>();
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
                options.OperationFilter<StubServerHeaderOperationFilter>();

                // Configure Swagger to use the xml documentation file
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                options.IncludeXmlComments(xmlFile);
            });

            services.AddHttpContextAccessor();

            services.AddSingleton<RouteTransformer>();

            services.AddSingleton<IClientContainer, ClientContainer>();

            services.AddTransient<ResponseModelContainer>();
            services.AddTransient<RequestModelContainer>();

            services.AddSingleton<Func<ResponseModelContainer>>(serviceProvider => serviceProvider.GetRequiredService<ResponseModelContainer>);
            services.AddSingleton<Func<RequestModelContainer>>(serviceProvider => serviceProvider.GetRequiredService<RequestModelContainer>);
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
                endpoints.MapDynamicControllerRoute<RouteTransformer>("{controller}/{client}/{**slug}");
            });
        }
    }
}
