@ECHO OFF
SETLOCAL

ECHO ========================================
ECHO  Update Database (Apply Migrations)
ECHO ========================================
ECHO.

ECHO Đang kiểm tra và apply các migrations chưa được apply...
ECHO.

REM Apply tất cả pending migrations
dotnet ef database update --context ApplicationDbContext --project CMS_2026.csproj

IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO ❌ Lỗi khi update database!
    ECHO    Vui lòng kiểm tra:
    ECHO    - Connection string trong appsettings.json
    ECHO    - SQL Server đã chạy chưa
    ECHO    - Database có tồn tại và có quyền truy cập
    ECHO.
    PAUSE
    EXIT /B %ERRORLEVEL%
)

ECHO.
ECHO ========================================
ECHO ✅ Database đã được cập nhật thành công!
ECHO ========================================
ECHO.

PAUSE

