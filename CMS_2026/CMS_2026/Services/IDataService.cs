using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CMS_2026.Data.Entities;
using CMS_2026.Data.Models;

namespace CMS_2026.Services
{
    public interface IDataService
    {
        // Basic CRUD operations - Async
        Task<T?> InsertAsync<T>(T model) where T : class;
        Task<T?> UpdateAsync<T>(T model) where T : class;
        Task<bool> DeleteAsync<T>(object key) where T : class;
        Task<bool> ExistsAsync<T>(object key) where T : class;
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> query) where T : class;
        Task<T?> GetOneAsync<T>(object key) where T : class;
        Task<T?> GetOneAsync<T>(Expression<Func<T, bool>> query) where T : class;
        Task<List<T>> GetListAsync<T>(string query) where T : class;
        Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>>? query = null) where T : class;
        IQueryable<T> Query<T>(Expression<Func<T, bool>>? query = null) where T : class;

        // Save changes - Async
        Task<int> SaveChangesAsync();

        // Special methods - Async
        Task<List<CategoryIndexer>> GetCategoryIndexesAsync();
        Task<List<Tuple<string, string>>> GetLinksAsync(string langId);
        Task<List<PP_Category>> GetCategoryMenuAsync(string langId, string? nodeType = null);
        Task RefreshVisitStatsAsync(DateTime now);
        Task<DashboardData> GetDashboardDataAsync();

        // Sync wrapper methods for Razor Views compatibility
        // These methods call async versions internally using GetAwaiter().GetResult()
        // WARNING: Only use in Razor views where await is not available
        T? GetOne<T>(object key) where T : class;
        T? GetOne<T>(Expression<Func<T, bool>> query) where T : class;
        List<T> GetList<T>(string query) where T : class;
        List<T> GetList<T>(Expression<Func<T, bool>>? query = null) where T : class;
        T? Insert<T>(T model) where T : class;
        T? Update<T>(T model) where T : class;
        bool Delete<T>(object key) where T : class;
        int SaveChanges();
        List<Tuple<string, string>> GetLinks(string langId);
    }

    public class DashboardData
    {
        public List<PP_Visit> RecentVisits { get; set; } = new();
        public List<PP_Order> RecentOrders { get; set; } = new();
    }
}

