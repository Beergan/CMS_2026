using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Middleware
{
    public class DynamicPageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DynamicPageMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip if it's already a Razor Page or static file
            if (path.StartsWith("/admin") || 
                path.StartsWith("/Error") || 
                path.StartsWith("/_") ||
                path.Contains(".") && !path.EndsWith(".cshtml"))
            {
                await _next(context);
                return;
            }

            // Create scope to resolve scoped service
            using var scope = _serviceScopeFactory.CreateScope();
            var routingService = scope.ServiceProvider.GetRequiredService<PageRoutingService>();

            // Try to find page by path
            // Get langId from cookie, query string, or default to "vi" (similar to MyRouteTable logic)
            var langId = context.Request.Cookies["LangId"] 
                ?? context.Request.Query["lang"].FirstOrDefault() 
                ?? "vi";
            var page = routingService.FindPageByPath(path, langId);

            if (page != null)
            {
                // Check if language is enabled (similar to MyRouteTable logic)
                if (!RootService.Langs.ContainsKey(page.LangId) || 
                    !RootService.Langs[page.LangId].Enabled)
                {
                    await _next(context);
                    return;
                }

                // Store page info in context for use in Razor Pages
                context.Items["PageId"] = page.Id;
                context.Items["LangId"] = page.LangId;
                context.Items["ComptKey"] = page.ComptKey;
                context.Items["PP_Page"] = page;

                // Extract slug if pattern has {0} (for dynamic routes like /san-pham/{0})
                if (page.PathPattern.Contains("{0}"))
                {
                    var slug = routingService.ExtractSlug(page.PathPattern, path);
                    if (!string.IsNullOrEmpty(slug))
                    {
                        context.Items["NodeSlug"] = slug;
                    }
                }

                // Rewrite path to DynamicPage route
                context.Request.Path = new PathString("/DynamicPage");
            }

            await _next(context);
        }
    }
}

