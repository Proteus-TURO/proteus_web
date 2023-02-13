using Microsoft.Extensions.FileProviders;

namespace ProteusWeb;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
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