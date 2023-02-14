using Microsoft.Extensions.FileProviders;
using ProteusWeb.Database;

namespace ProteusWeb;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var connectionString = Configuration.GetValue<string>("ConnectionString:DefaultConnection");
        if (connectionString == null)
        {
            // TODO: Error
            return;
        }
        services.Add(new ServiceDescriptor(typeof(DatabaseController), new DatabaseController(connectionString)));
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseFileServer(new FileServerOptions  
        {  
            FileProvider = new PhysicalFileProvider(  
                Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),  
            RequestPath = "",  
            EnableDefaultFiles = true
        }) ; 
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}