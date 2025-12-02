using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Data.Models;

namespace CMS_2026.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;

        public DataService(ApplicationDbContext context)
        {
            _context = context;
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
            _context.Set<T>().Add(model);
            _context.SaveChanges();
            return model;
        }

        public T? Update<T>(T model) where T : class
        {
            _context.Set<T>().Update(model);
            _context.SaveChanges();
            return model;
        }

        public bool Delete<T>(object key) where T : class
        {
            var entity = _context.Set<T>().Find(key);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
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
    }
}

