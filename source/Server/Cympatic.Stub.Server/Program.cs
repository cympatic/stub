using Cympatic.Extensions.Http;
using Cympatic.Stub.Server;
using Cympatic.Stub.Server.Containers;
using Cympatic.Stub.Server.Filters;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog();

builder.Services
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

builder.Services
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = AppDomain.CurrentDomain.FriendlyName
        });
        options.OperationFilter<StubServerHeaderOperationFilter>();

        // Configure Swagger to use the xml documentation file
        var xmlFile = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
        options.IncludeXmlComments(xmlFile);
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<RouteTransformer>();

builder.Services.AddSingleton<IClientContainer, ClientContainer>();

builder.Services.AddTransient<ResponseModelContainer>();
builder.Services.AddTransient<RequestModelContainer>();

builder.Services.AddSingleton<Func<ResponseModelContainer>>(serviceProvider => serviceProvider.GetRequiredService<ResponseModelContainer>);
builder.Services.AddSingleton<Func<RequestModelContainer>>(serviceProvider => serviceProvider.GetRequiredService<RequestModelContainer>);


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
    endpoints.MapDynamicControllerRoute<RouteTransformer>("{controller}/{client}/{**slug}");
});

app.Run();