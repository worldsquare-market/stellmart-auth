using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Stellmart.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                            .ConfigureAppConfiguration((context, config) =>
                            {
                                config.SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: false)
                                    .AddEnvironmentVariables();

                                var builtConfig = config.Build();

                                config.AddAzureKeyVault(
                                    $"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
                                    builtConfig["KeyVault:ClientId"],
                                    builtConfig["KeyVault:ClientSecret"]);
                            })
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}
