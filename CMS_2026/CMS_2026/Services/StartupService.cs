using System;
using System.Linq;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CMS_2026.Services
{
    public class StartupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataService _dataService;
        private readonly RootService _rootService;
        private readonly PageRoutingService _routingService;

        public StartupService(
            ApplicationDbContext context,
            IDataService dataService,
            RootService rootService,
            PageRoutingService routingService)
        {
            _context = context;
            _dataService = dataService;
            _rootService = rootService;
            _routingService = routingService;
        }

        public void Initialize()
        {
            try
            {
                // Load languages
                var langs = _dataService.GetList<PP_Lang>();
                _rootService.ReloadLangs(langs);

                // Load configs
                var configs = _dataService.GetList<PP_Config>();
                _rootService.RefreshConfigs(configs);

                // Load pages and refresh routes
                var pages = _dataService.GetList<PP_Page>()
                    .OrderByDescending(t => t.PathPattern)
                    .ToList();
                _routingService.RefreshRoutes();

                // Load category indexes
                _rootService.RefreshCategoryIndexes();

                // Load JSON data (like VisitCounter.UrlStats)
                var jsons = _dataService.GetList<PP_Json>();
                foreach (var json in jsons)
                {
                    if (json.JsonKey == "UrlStats")
                    {
                        // Can be used for VisitCounter initialization
                    }
                }

                // Initialize permissions
                if (!GlobalPermissions.Dictionary.Any())
                {
                    GlobalPermissions.RangerRegister(typeof(Management));
                }

                // Initialize default admin user if not exists
                var users = _dataService.GetList<PP_User>();
                if (!users.Any())
                {
                    var adminUser = new PP_User
                    {
                        UserId = "admin",
                        DisplayName = "ADMIN",
                        Email = "infor@gmail.com",
                        Enabled = true,
                        Password = "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92", // SHA256 of "123456"
                        CreatedTime = DateTime.Now,
                        ModifiedTime = DateTime.Now
                    };
                    _dataService.Insert(adminUser);

                    var adminRole = new PP_Roles
                    {
                        Name = "ADMIN",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = 1
                    };
                    _dataService.Insert(adminRole);

                    var userRole = new PP_UserRoles
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id
                    };
                    _dataService.Insert(userRole);

                    // Assign all permissions to admin role
                    var listPermissions = GlobalPermissions.Dictionary;
                    foreach (var item in listPermissions)
                    {
                        var roleClaim = new PP_RoleClaims
                        {
                            RoleId = adminRole.Id,
                            ClaimType = item.Key.Name ?? string.Empty,
                            ClaimValue = item.Value.Sum(x => x.Item1)
                        };
                        _dataService.Insert(roleClaim);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Startup initialization error: {ex.Message}");
            }
        }
    }
}

