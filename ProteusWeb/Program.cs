using Microsoft.AspNetCore;
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
                options.ListenAnyIP(12345);
                options.ListenAnyIP(12346, listenOptions =>
                {
                    listenOptions.UseHttps("server.pfx", signingKey);
                });
            })
            .UseConfiguration(config)
            .Build();
    }
}