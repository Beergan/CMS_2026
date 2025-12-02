using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CMS_2026.Data;

namespace CMS_2026.Services
{
    /// <summary>
    /// Service để tự động build database và apply migrations
    /// </summary>
    public class DatabaseMigrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseMigrationService>? _logger;

        public DatabaseMigrationService(ApplicationDbContext context, ILogger<DatabaseMigrationService>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tự động build database và apply tất cả migrations
        /// </summary>
        /// <returns>True nếu thành công, False nếu có lỗi</returns>
        public bool MigrateDatabase()
        {
            try
            {
                _logger?.LogInformation("Bắt đầu migrate database...");

                // Kiểm tra database có tồn tại không
                var canConnect = _context.Database.CanConnect();
                _logger?.LogInformation($"Database có thể kết nối: {canConnect}");

                // Lấy danh sách migrations pending
                var pendingMigrations = _context.Database.GetPendingMigrations().ToList();
                
                if (pendingMigrations.Any())
                {
                    _logger?.LogInformation($"Tìm thấy {pendingMigrations.Count} migration(s) chưa được apply:");
                    foreach (var migration in pendingMigrations)
                    {
                        _logger?.LogInformation($"  - {migration}");
                    }

                    // Apply tất cả pending migrations
                    _context.Database.Migrate();
                    _logger?.LogInformation("Đã apply tất cả migrations thành công!");
                }
                else
                {
                    _logger?.LogInformation("Không có migration nào cần apply. Database đã được cập nhật.");
                }

                // Kiểm tra lại sau khi migrate
                var appliedMigrations = _context.Database.GetAppliedMigrations().ToList();
                _logger?.LogInformation($"Tổng số migrations đã được apply: {appliedMigrations.Count}");

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi migrate database: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Tạo database nếu chưa tồn tại (không dùng migrations)
        /// Chỉ nên dùng trong môi trường development
        /// </summary>
        /// <returns>True nếu thành công</returns>
        public bool EnsureDatabaseCreated()
        {
            try
            {
                _logger?.LogInformation("Đang tạo database (nếu chưa tồn tại)...");

                var created = _context.Database.EnsureCreated();
                
                if (created)
                {
                    _logger?.LogInformation("Database đã được tạo mới.");
                }
                else
                {
                    _logger?.LogInformation("Database đã tồn tại.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi tạo database: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem database đã được tạo chưa
        /// </summary>
        public bool IsDatabaseExists()
        {
            try
            {
                return _context.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách migrations đã được apply
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> GetAppliedMigrations()
        {
            try
            {
                return _context.Database.GetAppliedMigrations();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Lấy danh sách migrations chưa được apply
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> GetPendingMigrations()
        {
            try
            {
                return _context.Database.GetPendingMigrations();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Build database một cách tự động:
        /// - Nếu có migrations: apply migrations
        /// - Nếu không có migrations: tạo database bằng EnsureCreated (chỉ cho dev)
        /// </summary>
        /// <param name="useMigrations">True = dùng migrations, False = dùng EnsureCreated</param>
        /// <returns>Thông tin kết quả</returns>
        public MigrationResult BuildDatabase(bool useMigrations = true)
        {
            var result = new MigrationResult
            {
                Success = false,
                Message = ""
            };

            try
            {
                if (useMigrations)
                {
                    // Luôn ưu tiên dùng migrations nếu được chọn
                    // Kiểm tra xem có migrations trong code không
                    try
                    {
                        var pendingMigrations = _context.Database.GetPendingMigrations().ToList();
                        var appliedMigrations = _context.Database.GetAppliedMigrations().ToList();
                        
                        // Nếu có migrations (pending hoặc applied) hoặc database chưa tồn tại
                        if (pendingMigrations.Any() || appliedMigrations.Any() || !IsDatabaseExists())
                        {
                            // Dùng migrations
                            result.Success = MigrateDatabase();
                            if (result.Success)
                            {
                                var applied = _context.Database.GetAppliedMigrations().ToList();
                                result.Message = applied.Any() 
                                    ? $"Database đã được build thành công bằng migrations. ({applied.Count} migration(s) đã được apply)" 
                                    : "Database đã được build thành công bằng migrations.";
                            }
                            else
                            {
                                result.Message = "Lỗi khi apply migrations. Vui lòng kiểm tra connection string và quyền truy cập database.";
                            }
                        }
                        else
                        {
                            // Database đã tồn tại và không có migrations, có thể database đã được tạo bằng cách khác
                            result.Success = true;
                            result.Message = "Database đã tồn tại và không có migrations nào cần apply.";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Nếu không thể check migrations (database chưa tồn tại), thử tạo database bằng migrations
                        _logger?.LogWarning(ex, "Không thể check migrations, sẽ thử tạo database bằng migrations");
                        result.Success = MigrateDatabase();
                        result.Message = result.Success 
                            ? "Database đã được build thành công bằng migrations." 
                            : $"Lỗi khi tạo database bằng migrations: {ex.Message}";
                    }
                }
                else
                {
                    // Dùng EnsureCreated (chỉ cho dev)
                    result.Success = EnsureDatabaseCreated();
                    result.Message = result.Success 
                        ? "Database đã được tạo thành công." 
                        : "Lỗi khi tạo database.";
                }

                if (result.Success)
                {
                    result.AppliedMigrations = GetAppliedMigrations().ToList();
                    result.PendingMigrations = GetPendingMigrations().ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi: {ex.Message}";
                _logger?.LogError(ex, "Lỗi khi build database");
                return result;
            }
        }

        /// <summary>
        /// Kết quả của quá trình migration
        /// </summary>
        public class MigrationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public System.Collections.Generic.List<string> AppliedMigrations { get; set; } = new();
            public System.Collections.Generic.List<string> PendingMigrations { get; set; } = new();
        }
    }
}

