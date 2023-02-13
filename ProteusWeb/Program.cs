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
        return WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
    }
}