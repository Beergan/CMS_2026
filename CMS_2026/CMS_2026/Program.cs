using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using CMS_2026.Data;
using CMS_2026.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use port from launchSettings.json or environment
// This prevents binding to default port 5000 when launchSettings.json is not used
if (builder.Environment.IsDevelopment())
{
    // In development, use port from launchSettings.json (5050)
    builder.WebHost.UseUrls("http://localhost:5050");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add Entity Framework — dùng AddDbContextPool thay vì AddDbContext cho 10k+ users
// Pool tái sử dụng DbContext instances, giảm áp lực GC đáng kể
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(30);
    }), poolSize: 256);

// Add Data Service
builder.Services.AddScoped<IDataService, DataService>();

// Add Root Service
builder.Services.AddSingleton<RootService>();

// Add Page Routing Service
builder.Services.AddScoped<PageRoutingService>();

// Add Route Table Service (similar to MyRouteTable)
builder.Services.AddScoped<CMS_2026.Routing.RouteTableService>();

// Add Permission Service
builder.Services.AddScoped<PermissionService>();

// Add Visit Counter Service
builder.Services.AddScoped<VisitCounterService>();

// Add Payment Services
builder.Services.AddScoped<VietQRService>();
builder.Services.AddHttpClient<VietQRService>();

// Add Shopping Cart Service
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddHttpContextAccessor();

// Add Email Service
builder.Services.AddScoped<EmailService>();

// Add Startup Service
builder.Services.AddScoped<StartupService>();

// Add Database Migration Service
builder.Services.AddScoped<DatabaseMigrationService>();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Response Compression (Brotli + Gzip)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json",
        "application/xml",
        "text/xml",
        "image/svg+xml",
        "application/javascript",
        "text/javascript",
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Chỉ bật DeveloperExceptionPage trong Development — KHÔNG bao giờ để ngoài block này!
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Response Compression MUST be before static files and routing
app.UseResponseCompression();

// Static files MUST be before routing (avoids unnecessary routing overhead)
app.UseStaticFiles();

// Add Dynamic Page Middleware (MUST be before UseRouting to rewrite path correctly)
app.UseMiddleware<CMS_2026.Middleware.DynamicPageMiddleware>();

app.UseRouting();

app.UseSession();

// Add Visit Counter Middleware (after routing)
app.UseMiddleware<CMS_2026.Middleware.VisitCounterMiddleware>();

app.UseAuthorization();

// Auto-build database on startup using migrations
using (var scope = app.Services.CreateScope())
{
    try
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseMigrationService>();
        
        // Tự động apply migrations khi khởi động
        // Trong Production, nên đảm bảo connection string đúng và có quyền tạo database
        var result = migrationService.BuildDatabase(useMigrations: true);
        if (result.Success)
        {
            Console.WriteLine($"✅ {result.Message}");
            if (result.AppliedMigrations.Any())
            {
                Console.WriteLine($"   Đã apply {result.AppliedMigrations.Count} migration(s):");
                foreach (var migration in result.AppliedMigrations)
                {
                    Console.WriteLine($"     - {migration}");
                }
            }
        }
        else
        {
            Console.WriteLine($"⚠️ {result.Message}");
            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("   Có thể database chưa được tạo hoặc connection string chưa đúng.");
                Console.WriteLine("   Vui lòng kiểm tra connection string trong appsettings.json");
            }
            else
            {
                // Trong Production, chỉ check xem database có tồn tại không
                if (!migrationService.IsDatabaseExists())
                {
                    Console.WriteLine("⚠️ Database chưa được tạo. Vui lòng chạy migrations hoặc dùng admin page để build database.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Log error but continue - database might not be accessible
        Console.WriteLine($"⚠️ Warning: Could not check database: {ex.Message}");
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
        }
    }
}

// Initialize startup data
using (var scope = app.Services.CreateScope())
{
    var startupService = scope.ServiceProvider.GetRequiredService<StartupService>();
    startupService.InitializeAsync().GetAwaiter().GetResult();
}
app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
