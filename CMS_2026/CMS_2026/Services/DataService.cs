using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public async Task<bool> ExistsAsync<T>(object key) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(key);
            return entity != null;
        }

        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> query) where T : class
        {
            return await _context.Set<T>().AnyAsync(query);
        }

        public async Task<T?> GetOneAsync<T>(object key) where T : class
        {
            return await _context.Set<T>().FindAsync(key);
        }

        public async Task<T?> GetOneAsync<T>(Expression<Func<T, bool>> query) where T : class
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(query);
        }

        public async Task<List<T>> GetListAsync<T>(string query) where T : class
        {
            // For raw SQL queries, we'll need to implement this differently
            // For now, return empty list - can be enhanced later
            await Task.CompletedTask;
            return new List<T>();
        }

        public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>>? query = null) where T : class
        {
            if (query != null)
            {
                return await _context.Set<T>().AsNoTracking().Where(query).ToListAsync();
            }
            else
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
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

        public async Task<T?> InsertAsync<T>(T model) where T : class
        {
            var entityType = typeof(T).Name;
            _context.Set<T>().Add(model);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database INSERT: {EntityType} - Cache version will be incremented", entityType);

                await SaveDatabaseLogAsync("INSERT", entityType, GetEntityId(model), _httpContextAccessor?.HttpContext);
                
                // Only increment cache version for content-related entities, not for logs or stats
                if (!IsSystemEntity(entityType))
                {
                    _rootService?.IncrementCacheVersion();
                }
            }

            return model;
        }

        public async Task<T?> UpdateAsync<T>(T model) where T : class
        {
            var entityType = typeof(T).Name;
            _context.Set<T>().Update(model);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database UPDATE: {EntityType} - Cache version will be incremented", entityType);

                await SaveDatabaseLogAsync("UPDATE", entityType, GetEntityId(model), _httpContextAccessor?.HttpContext);
                
                // Only increment cache version for content-related entities, not for logs or stats
                if (!IsSystemEntity(entityType))
                {
                    _rootService?.IncrementCacheVersion();
                }
            }

            return model;
        }

        public async Task<bool> DeleteAsync<T>(object key) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(key);
            if (entity == null)
                return false;

            var entityType = typeof(T).Name;
            var entityId = GetEntityId(entity);
            _context.Set<T>().Remove(entity);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database DELETE: {EntityType} (Key: {Key}) - Cache version will be incremented", entityType, key);

                await SaveDatabaseLogAsync("DELETE", entityType, entityId, _httpContextAccessor?.HttpContext);
                
                // Only increment cache version for content-related entities, not for logs or stats
                if (!IsSystemEntity(entityType))
                {
                    _rootService?.IncrementCacheVersion();
                }
            }

            return result > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger?.LogInformation("[Cache] Database SaveChanges: {Count} changes - Cache version will be incremented", result);
                await SaveDatabaseLogAsync("SAVECHANGES", "Multiple", null, _httpContextAccessor?.HttpContext, $"Total changes: {result}");
                
                // Note: SaveChangesAsync is typically called directly for batch operations
                // Only increment cache if it's not already incremented by InsertAsync/UpdateAsync/DeleteAsync
                // This method should be used carefully to avoid double incrementing
                _rootService?.IncrementCacheVersion();
            }

            return result;
        }
        private List<int> GetAllDescendantIds(int parentId, List<PP_Category> allCategories)
{
            var result = new List<int>();

            var children = allCategories.Where(c => c.ParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child.Id);

                var descendants = GetAllDescendantIds(child.Id, allCategories);
                result.AddRange(descendants);
            }

            return result;
        }
        public async Task<List<CategoryIndexer>> GetCategoryIndexesAsync()
        {
            var categories = await _context.PP_Categories
                .ToListAsync(); 

            var result = new List<CategoryIndexer>();

            foreach (var category in categories) 
            {
                var descendantIds = GetAllDescendantIds(category.Id, categories);

                if (descendantIds.Any())
                {
                    result.Add(new CategoryIndexer
                    {
                        RootId = category.Id,
                        Array = string.Join(",", descendantIds)
                    });
                }
            }

            return result; 
        }

        public async Task<List<Tuple<string, string>>> GetLinksAsync(string langId)
        {
            var pages = await _context.PP_Pages
                .Where(p => p.LangId == langId && (p.PageType == null || !new[] { "item", "list" }.Contains(p.PageType)))
                .Select(p => new Tuple<string, string>(p.Title, "/" + p.PathPattern))
                .ToListAsync();

            var categories = await _context.PP_Categories
                .Where(c => c.LangId == langId)
                .Select(c => new Tuple<string, string>(c.Title, "/" + c.CategoryPath))
                .ToListAsync();

            var nodes = await _context.PP_Nodes
                .Where(n => n.LangId == langId)
                .Select(n => new Tuple<string, string>(n.Title, "/" + n.NodePath))
                .ToListAsync();

            var products = await _context.PP_Products
                .Where(p => p.LangId == langId)
                .Select(p => new Tuple<string, string>(p.Title, "/" + p.NodePath))
                .ToListAsync();

            var result = new List<Tuple<string, string>>();
            result.AddRange(pages);
            result.AddRange(categories);
            result.AddRange(nodes);
            result.AddRange(products);

            return result;
        }

        public async Task<List<PP_Category>> GetCategoryMenuAsync(string langId, string? nodeType = null)
        {
            var query = _context.PP_Categories.Where(c => c.LangId == langId);

            if (!string.IsNullOrEmpty(nodeType))
            {
                query = query.Where(c => c.NodeType == nodeType);
            }

            return await query.OrderBy(c => c.CategoryPath).ToListAsync();
        }

        public async Task RefreshVisitStatsAsync(DateTime now)
        {
            // Implementation for refreshing visit stats
            // This will need to be implemented with raw SQL or a stored procedure
            await Task.CompletedTask;
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            var data = new DashboardData
            {
                RecentVisits = await _context.PP_Visits
                    .OrderByDescending(v => v.Date)
                    .Take(5)
                    .ToListAsync(),
                RecentOrders = await _context.PP_Orders
                    .OrderByDescending(o => o.CreatedTime)
                    .Take(5)
                    .ToListAsync()
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
        /// Kiểm tra xem entity có phải là system entity (không cần invalidate cache) không
        /// System entities: DatabaseLog, Visit, Stats_Daily, etc.
        /// </summary>
        private bool IsSystemEntity(string entityType)
        {
            var systemEntities = new[] { 
                "PP_DatabaseLog", 
                "PP_Visit", 
                "PP_Stats_Daily",
                "PP_Json" // JSON data thường là system data
            };
            
            return systemEntities.Contains(entityType);
        }

        /// <summary>
        /// Lưu log vào database về thao tác database
        /// Lưu log bất đồng bộ để không ảnh hưởng đến performance
        /// </summary>
        private async Task SaveDatabaseLogAsync(string action, string entityType, int? entityId, HttpContext? httpContext, string? description = null)
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

            // Fire and forget - không chờ kết quả
            _ = Task.Run(async () =>
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
                        await logContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không throw để không ảnh hưởng đến flow chính
                    _logger?.LogError(ex, "[DatabaseLog] Error saving database log: {Action} - {EntityType}", action, entityType);
                    System.Diagnostics.Debug.WriteLine($"[DatabaseLog] Error: {ex.Message}");
                }
            });

            await Task.CompletedTask;
        }

        // =================================================================
        // Sync wrapper methods for Razor Views compatibility
        // WARNING: These methods block the thread. Only use in Razor views where await is not available.
        // In C# code files, always use the Async versions instead.
        // =================================================================

        public T? GetOne<T>(object key) where T : class
        {
            return GetOneAsync<T>(key).GetAwaiter().GetResult();
        }

        public T? GetOne<T>(Expression<Func<T, bool>> query) where T : class
        {
            return GetOneAsync<T>(query).GetAwaiter().GetResult();
        }

        public List<T> GetList<T>(string query) where T : class
        {
            return GetListAsync<T>(query).GetAwaiter().GetResult();
        }

        public List<T> GetList<T>(Expression<Func<T, bool>>? query = null) where T : class
        {
            return GetListAsync<T>(query).GetAwaiter().GetResult();
        }

        public T? Insert<T>(T model) where T : class
        {
            return InsertAsync(model).GetAwaiter().GetResult();
        }

        public T? Update<T>(T model) where T : class
        {
            return UpdateAsync(model).GetAwaiter().GetResult();
        }

        public bool Delete<T>(object key) where T : class
        {
            return DeleteAsync<T>(key).GetAwaiter().GetResult();
        }

        public int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        public List<Tuple<string, string>> GetLinks(string langId)
        {
            return GetLinksAsync(langId).GetAwaiter().GetResult();
        }
    }
}
