using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CMS_2026.Services;

namespace CMS_2026.Middleware
{
    public class VisitCounterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public VisitCounterMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip admin and static files
            if (path.StartsWith("/admin") || 
                path.StartsWith("/_") ||
                path.Contains(".") && !path.EndsWith(".cshtml"))
            {
                await _next(context);
                return;
            }

            // Create scope to resolve scoped service
            using var scope = _serviceScopeFactory.CreateScope();
            var visitCounter = scope.ServiceProvider.GetRequiredService<VisitCounterService>();

            var sessionId = context.Session.Id;
            var currentUrl = context.Request.Path + context.Request.QueryString;
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                         ?? context.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
                         ?? context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var referer = context.Request.Headers["Referer"].ToString();
            var browser = "Unknown"; // Can be enhanced with browser detection

            // Handle session start
            if (!context.Session.Keys.Contains("VisitStarted"))
            {
                context.Session.SetString("VisitStarted", "true");
                visitCounter.OnSessionStart(sessionId, referer, ipAddress, userAgent, browser);
            }

            // Handle request begin (only for GET requests)
            if (context.Request.Method == "GET")
            {
                visitCounter.OnRequestBegin(sessionId, currentUrl, ipAddress, userAgent, browser);
            }

            await _next(context);
        }
    }
}

