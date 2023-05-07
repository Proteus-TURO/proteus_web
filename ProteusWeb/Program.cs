using Microsoft.AspNetCore;
using ProteusWeb.Helper;
using Serilog;

namespace ProteusWeb;

public static class Program
{
    public static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        BuildWebHost().Run();
    }
    private static IWebHost BuildWebHost()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var signingKey = config.GetValue<string>("SigningKey");
        return WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>()
            .UseKestrel(options =>
            {
                options.ListenAnyIP(80);
                options.ListenAnyIP(443, listenOptions =>
                {
                    listenOptions.UseHttps("server.pfx", signingKey);
                });
            })
            .UseConfiguration(config)
            .Build();
    }
}