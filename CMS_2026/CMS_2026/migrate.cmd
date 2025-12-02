@ECHO OFF
SETLOCAL

ECHO ========================================
ECHO  Entity Framework Migration Tool
ECHO ========================================
ECHO.

REM Kiểm tra xem có tên migration được truyền vào không
IF "%~1"=="" (
    ECHO [1/2] Tạo migration mới với tên tự động (timestamp)...
    FOR /F %%i IN ('powershell -NoProfile -Command "Get-Date -Format yyyyMMddHHmmss"') DO SET "name=%%i"
    ECHO    Tên migration: %name%
) ELSE (
    SET "name=%~1"
    ECHO [1/2] Tạo migration mới: %name%
)

ECHO.

REM Tạo migration mới
dotnet ef migrations add "%name%" --context ApplicationDbContext --project CMS_2026.csproj --output-dir Migrations

IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO ❌ Lỗi khi tạo migration!
    ECHO.
    PAUSE
    EXIT /B %ERRORLEVEL%
)

ECHO.
ECHO ✅ Migration đã được tạo thành công!
ECHO.
ECHO [2/2] Đang apply migration vào database...
ECHO.

REM Apply migration vào database
dotnet ef database update --context ApplicationDbContext --project CMS_2026.csproj

IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO ❌ Lỗi khi update database!
    ECHO    Vui lòng kiểm tra connection string trong appsettings.json
    ECHO.
    PAUSE
    EXIT /B %ERRORLEVEL%
)

ECHO.
ECHO ========================================
ECHO ✅ Hoàn thành! Database đã được cập nhật.
ECHO ========================================
ECHO.

PAUSE

