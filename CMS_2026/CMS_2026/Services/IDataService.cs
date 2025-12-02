using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CMS_2026.Data.Entities;
using CMS_2026.Data.Models;

namespace CMS_2026.Services
{
    public interface IDataService
    {
        // Basic CRUD operations
        T? Insert<T>(T model) where T : class;
        T? Update<T>(T model) where T : class;
        bool Delete<T>(object key) where T : class;
        bool Exists<T>(object key) where T : class;
        bool Exists<T>(Expression<Func<T, bool>> query) where T : class;
        T? GetOne<T>(object key) where T : class;
        T? GetOne<T>(Expression<Func<T, bool>> query) where T : class;
        List<T> GetList<T>(string query) where T : class;
        List<T> GetList<T>(Expression<Func<T, bool>>? query = null) where T : class;
        IQueryable<T> Query<T>(Expression<Func<T, bool>>? query = null) where T : class;

        // Save changes
        int SaveChanges();

        // Special methods
        List<CategoryIndexer> GetCategoryIndexes();
        List<Tuple<string, string>> GetLinks(string langId);
        List<PP_Category> GetCategoryMenu(string langId, string? nodeType = null);
        void RefreshVisitStats(DateTime now);
        DashboardData GetDashboardData();
    }

    public class DashboardData
    {
        public List<PP_Visit> RecentVisits { get; set; } = new();
        public List<PP_Order> RecentOrders { get; set; } = new();
    }
}

