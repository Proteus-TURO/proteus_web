namespace ProteusWeb.Middleware;

public class RedirectIfUnauthorizedMiddleware
{
    private const string LoginPath =  "/login";
    private readonly RequestDelegate _next;
    public RedirectIfUnauthorizedMiddleware(RequestDelegate next)  
    {  
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == 401)
        {
            context.Response.Redirect(LoginPath);
        }
    }
}
