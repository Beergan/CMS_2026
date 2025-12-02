using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Database
{
    public class BuildModel : BaseAdminPageModel
    {
        private readonly DatabaseMigrationService _migrationService;

        public BuildModel(
            IDataService dataService,
            RootService rootService,
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache,
            PermissionService permissionService,
            DatabaseMigrationService migrationService)
            : base(dataService, rootService, cache, permissionService)
        {
            _migrationService = migrationService;
        }

        public DatabaseMigrationService.MigrationResult? Result { get; set; }
        public bool DatabaseExists { get; set; }
        public List<string> AppliedMigrations { get; set; } = new();
        public List<string> PendingMigrations { get; set; } = new();

        public void OnGet()
        {
            // Lấy thông tin hiện tại của database
            DatabaseExists = _migrationService.IsDatabaseExists();
            AppliedMigrations = _migrationService.GetAppliedMigrations().ToList();
            PendingMigrations = _migrationService.GetPendingMigrations().ToList();
        }

        public IActionResult OnPost(bool useMigrations = true)
        {
            try
            {
                // Build database
                Result = _migrationService.BuildDatabase(useMigrations);
                
                // Reload thông tin
                DatabaseExists = _migrationService.IsDatabaseExists();
                AppliedMigrations = _migrationService.GetAppliedMigrations().ToList();
                PendingMigrations = _migrationService.GetPendingMigrations().ToList();

                // Nếu thành công, reload StartupService để load dữ liệu
                if (Result.Success)
                {
                    var startupService = HttpContext.RequestServices.GetRequiredService<StartupService>();
                    startupService.Initialize();
                }

                return Page();
            }
            catch (Exception ex)
            {
                Result = new DatabaseMigrationService.MigrationResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
                return Page();
            }
        }
    }
}

