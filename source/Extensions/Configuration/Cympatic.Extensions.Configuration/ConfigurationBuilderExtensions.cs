using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Cympatic.Extensions.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAvailableConfigurations(this IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentJsonFile()
                .AddUserJsonFile()
                .AddUserEnvironmentJsonFile();

            return builder;
        }

        public static IConfigurationBuilder AddUserJsonFile(this IConfigurationBuilder builder)
        {
            if (TryGetSolutionItemsDirectoryInfo(out var solutionItemsFolder))
            {
                var userName = Environment.UserName;

                var userJsonFile = Path.Combine(solutionItemsFolder.FullName, $"appsettings.{userName}.json");

                builder.AddJsonFile(userJsonFile, optional: true, reloadOnChange: true);
            }

            return builder;
        }

        public static IConfigurationBuilder AddUserEnvironmentJsonFile(this IConfigurationBuilder builder)
        {
            var environmentName = builder.Build()["Environment"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrEmpty(environmentName) && TryGetSolutionItemsDirectoryInfo(out var solutionItemsFolder))
            {
                var userName = Environment.UserName;

                var userEnvironmentJsonFile = Path.Combine(solutionItemsFolder.FullName, $"appsettings.{userName}.{environmentName}.json");

                builder.AddJsonFile(userEnvironmentJsonFile, optional: true, reloadOnChange: true);
            }

            return builder;
        }

        public static IConfigurationBuilder AddEnvironmentJsonFile(this IConfigurationBuilder builder)
        {
            var environmentName = builder.Build()["Environment"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrEmpty(environmentName))
            {
                builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            }

            return builder;
        }

        private static bool TryGetSolutionItemsDirectoryInfo(out DirectoryInfo directory)
        {
            directory = null;

            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directoryInfo != null)
            {
                if (directoryInfo.GetDirectories("Solution Items").Any())
                {
                    directory = directoryInfo.GetDirectories("Solution Items").First();
                    return true;
                }
                directoryInfo = directoryInfo.Parent;
            }

            return false;
        }
    }
}
