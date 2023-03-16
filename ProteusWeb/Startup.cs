using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProteusWeb.Database;
using Serilog;

namespace ProteusWeb;

public class Startup
{
    public static IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        //---------------------------------------------------------------------------------
        services.AddCors();
        //---------------------------------------------------------------------------------
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Proteus API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        var connectionString = Configuration.GetValue<string>("ConnectionString:DefaultConnection");
        if (connectionString == null)
        {
            // TODO: Error
            return;
        }

        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion("8.0.21-mysql"));
        });
        services.AddScoped<UserService>();
        services.AddScoped<ArticleService>();


        var signingKey = Configuration.GetValue<string>("SigningKey");

        if (signingKey == null)
        {
            // TODO: Error
            return;
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)),
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.Cookie.Name = "Proteus.Cookie";
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
        });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme,
                    CookieAuthenticationDefaults.AuthenticationScheme)
                .Build();
        });
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proteus API"); });
        app.UseAuthentication();
        app.UseAuthorization();

        //---------------------------------------------------------------------------------
        app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
        //---------------------------------------------------------------------------------

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/private"))
            {
                if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            await next();
        });
        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
            RequestPath = "",
            EnableDefaultFiles = true,
            StaticFileOptions =
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                    ctx.Context.Response.Headers["Pragma"] = "no-cache";
                    ctx.Context.Response.Headers["Expires"] = "-1";
                }
            }
        });
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}