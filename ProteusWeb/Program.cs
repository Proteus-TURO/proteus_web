using Microsoft.AspNetCore;

namespace ProteusWeb;

public static class Program
{
    public static void Main()
    {
        BuildWebHost().Run();
    }

    private static IWebHost BuildWebHost()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        return WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>()
            .UseConfiguration(config)
            .Build();
    }
}