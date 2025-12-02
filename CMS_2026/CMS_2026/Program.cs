using Microsoft.EntityFrameworkCore;
using CMS_2026.Data;
using CMS_2026.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

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

app.UseHttpsRedirection();

// Add Dynamic Page Middleware (MUST be before UseRouting to rewrite path correctly)
app.UseMiddleware<CMS_2026.Middleware.DynamicPageMiddleware>();

app.UseRouting();

// Static files should be after routing to allow controllers to handle requests first
app.UseStaticFiles();

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
    startupService.Initialize();
}
app.UseDeveloperExceptionPage();
app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
