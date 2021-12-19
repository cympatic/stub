using Cympatic.Extensions.Http;
using Cympatic.Stub.Example.Api.Filters;
using Cympatic.Stub.Example.Api.Services;
using Cympatic.Stub.Example.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = AppDomain.CurrentDomain.FriendlyName
        });
        options.OperationFilter<ExampleIdentifierHeaderOperationFilter>();

        // Configure Swagger to use the xml documentation file
        var xmlFile = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
        options.IncludeXmlComments(xmlFile);
    });

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddOptions<ExternalApiServiceSettings>()
    .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(ExternalApiServiceSettings)).Bind(options));

builder.Services.AddHttpClient<ExternalApiService>((serviceProvider, config) =>
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


var app = builder.Build();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(app.Configuration).CreateLogger();

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

app.Run();
