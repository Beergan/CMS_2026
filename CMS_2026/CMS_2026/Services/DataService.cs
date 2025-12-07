using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Data.Models;
using CMS_2026.Services;

namespace CMS_2026.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RootService? _rootService;
        private readonly ILogger<DataService>? _logger;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public DataService(
            ApplicationDbContext context,
            RootService? rootService = null,
            ILogger<DataService>? logger = null,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _context = context;
            _rootService = rootService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Exists<T>(object key) where T : class
        {
            var entity = _context.Set<T>().Find(key);
            return entity != null;
        }

        public bool Exists<T>(Expression<Func<T, bool>> query) where T : class
        {
            return _context.Set<T>().Any(query);
        }

        public T? GetOne<T>(object key) where T : class
        {
            return _context.Set<T>().Find(key);
        }

        public T? GetOne<T>(Expression<Func<T, bool>> query) where T : class
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(query);
        }

        public List<T> GetList<T>(string query) where T : class
        {
            // For raw SQL queries, we'll need to implement this differently
            // For now, return empty list - can be enhanced later
            return new List<T>();
        }

        public List<T> GetList<T>(Expression<Func<T, bool>>? query = null) where T : class
        {
            if (query != null)
            {
                return _context.Set<T>().AsNoTracking().Where(query).ToList();
            }
            else
            {
                return _context.Set<T>().AsNoTracking().ToList();
            }
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>>? query = null) where T : class
        {
            if (query != null)
            {
                return _context.Set<T>().AsNoTracking().Where(query);
            }
            else
            {
                return _context.Set<T>().AsNoTracking();
            }
        }

        public T? Insert<T>(T model) where T : class
        {
            var entityType = typeof(T).Name;
            _context.Set<T>().Add(model);
            var result = _context.SaveChanges();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database INSERT: {EntityType} - Cache version will be incremented", entityType);

                SaveDatabaseLog("INSERT", entityType, GetEntityId(model), _httpContextAccessor?.HttpContext);
            }

            return model;
        }

        public T? Update<T>(T model) where T : class
        {
            var entityType = typeof(T).Name;
            _context.Set<T>().Update(model);
            var result = _context.SaveChanges();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database UPDATE: {EntityType} - Cache version will be incremented", entityType);

                SaveDatabaseLog("UPDATE", entityType, GetEntityId(model), _httpContextAccessor?.HttpContext);

            }

            return model;
        }

        public bool Delete<T>(object key) where T : class
        {
            var entity = _context.Set<T>().Find(key);
            if (entity == null)
                return false;

            var entityType = typeof(T).Name;
            var entityId = GetEntityId(entity);
            _context.Set<T>().Remove(entity);
            var result = _context.SaveChanges();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database DELETE: {EntityType} (Key: {Key}) - Cache version will be incremented", entityType, key);

                SaveDatabaseLog("DELETE", entityType, entityId, _httpContextAccessor?.HttpContext);
            }

            return result > 0;
        }

        public int SaveChanges()
        {
            var result = _context.SaveChanges();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database SaveChanges: {Count} changes - Cache version will be incremented", result);
                SaveDatabaseLog("SAVECHANGES", "Multiple", null, _httpContextAccessor?.HttpContext, $"Total changes: {result}");
                _rootService?.IncrementCacheVersion();
            }

            return result;
        }

        public List<CategoryIndexer> GetCategoryIndexes()
        {
            // This will need to be implemented with raw SQL or a stored procedure
            // For now, return empty list
            return new List<CategoryIndexer>();
        }

        public List<Tuple<string, string>> GetLinks(string langId)
        {
            var pages = _context.PP_Pages
                .Where(p => p.LangId == langId && (p.PageType == null || !new[] { "item", "list" }.Contains(p.PageType)))
                .Select(p => new Tuple<string, string>(p.Title, "/" + p.PathPattern))
                .ToList();

            var categories = _context.PP_Categories
                .Where(c => c.LangId == langId)
                .Select(c => new Tuple<string, string>(c.Title, "/" + c.CategoryPath))
                .ToList();

            var nodes = _context.PP_Nodes
                .Where(n => n.LangId == langId)
                .Select(n => new Tuple<string, string>(n.Title, "/" + n.NodePath))
                .ToList();

            var products = _context.PP_Products
                .Where(p => p.LangId == langId)
                .Select(p => new Tuple<string, string>(p.Title, "/" + p.NodePath))
                .ToList();

            var result = new List<Tuple<string, string>>();
            result.AddRange(pages);
            result.AddRange(categories);
            result.AddRange(nodes);
            result.AddRange(products);

            return result;
        }

        public List<PP_Category> GetCategoryMenu(string langId, string? nodeType = null)
        {
            var query = _context.PP_Categories.Where(c => c.LangId == langId);

            if (!string.IsNullOrEmpty(nodeType))
            {
                query = query.Where(c => c.NodeType == nodeType);
            }

            return query.OrderBy(c => c.CategoryPath).ToList();
        }

        public void RefreshVisitStats(DateTime now)
        {
            // Implementation for refreshing visit stats
            // This will need to be implemented with raw SQL or a stored procedure
        }

        public DashboardData GetDashboardData()
        {
            var data = new DashboardData
            {
                RecentVisits = _context.PP_Visits
                    .OrderByDescending(v => v.Date)
                    .Take(5)
                    .ToList(),
                RecentOrders = _context.PP_Orders
                    .OrderByDescending(o => o.CreatedTime)
                    .Take(5)
                    .ToList()
            };

            return data;
        }

        /// <summary>
        /// Lấy ID từ entity (nếu entity có property Id)
        /// </summary>
        private int? GetEntityId<T>(T entity) where T : class
        {
            if (entity == null) return null;

            try
            {
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null && idProperty.PropertyType == typeof(int))
                {
                    var value = idProperty.GetValue(entity);
                    return value as int?;
                }
            }
            catch
            {
                // Ignore errors
            }

            return null;
        }

        /// <summary>
        /// Lưu log vào database về thao tác database
        /// Lưu log bất đồng bộ để không ảnh hưởng đến performance
        /// </summary>
        private void SaveDatabaseLog(string action, string entityType, int? entityId, HttpContext? httpContext, string? description = null)
        {
            if (httpContext == null)
                return;

            var userId = AuthenticationService.GetUserId(httpContext);
            var idUser = AuthenticationService.GetUserIdInt(httpContext);
            var displayName = AuthenticationService.GetDisplayName(httpContext);
            var ipAddress = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
                ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "Unknown";

            var connectionString = _context.Database.GetDbConnection().ConnectionString;

            Task.Run(async () =>
            {
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(connectionString);

                    using (var logContext = new ApplicationDbContext(optionsBuilder.Options))
                    {
                        var log = new PP_DatabaseLog
                        {
                            Action = action,
                            EntityType = entityType,
                            EntityId = entityId,
                            UserId = userId,
                            IdUser = idUser,
                            DisplayName = displayName,
                            IpAddress = ipAddress,
                            Description = description,
                            LogTime = DateTime.Now,
                            CreatedTime = DateTime.Now,
                            ModifiedTime = DateTime.Now,
                            CreatedBy = userId ?? "System",
                            ModifiedBy = userId ?? "System"
                        };

                        logContext.PP_DatabaseLog.Add(log);
                        logContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không throw để không ảnh hưởng đến flow chính
                    _logger?.LogError(ex, "[DatabaseLog] Error saving database log: {Action} - {EntityType}", action, entityType);
                    System.Diagnostics.Debug.WriteLine($"[DatabaseLog] Error: {ex.Message}");
                }
            });
        }
    }
}

