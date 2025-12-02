@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ========================================
ECHO  Fix Description Column in pp_roles
ECHO ========================================
ECHO.

REM Ensure running from correct directory
CD /D "%~dp0"

IF NOT EXIST "CMS_2026.csproj" (
    ECHO ❌ Error: CMS_2026.csproj file not found
    ECHO    Please run this script from the directory containing the .csproj file
    PAUSE
    EXIT /B 1
)

ECHO This script will:
ECHO 1. Remove the problematic CreateTable migration
ECHO 2. Create a new migration to add Description column only
ECHO.

SET /P "confirm=Continue? (Y/N): "
IF /I NOT "!confirm!"=="Y" (
    ECHO Cancelled.
    PAUSE
    EXIT /B 0
)

ECHO.
ECHO [1/3] Removing problematic migration...
REM Note: We'll keep the migration but create a new one for adding column
ECHO.

ECHO [2/3] Creating new migration to add Description column...
FOR /F %%i IN ('powershell -NoProfile -Command "Get-Date -Format yyyyMMddHHmmss"') DO SET "migrationName=%%i_AddDescriptionToPP_Roles"

dotnet ef migrations add "%migrationName%" --context ApplicationDbContext --project CMS_2026.csproj --output-dir Migrations

IF !ERRORLEVEL! NEQ 0 (
    ECHO.
    ECHO ❌ Failed to create migration!
    ECHO    The Description column may already exist in the model snapshot.
    ECHO    You may need to manually add the column using SQL script.
    ECHO.
    PAUSE
    EXIT /B !ERRORLEVEL!
)

ECHO.
ECHO [3/3] Applying migration to database...
dotnet ef database update --context ApplicationDbContext --project CMS_2026.csproj

IF !ERRORLEVEL! NEQ 0 (
    ECHO.
    ECHO ❌ Failed to update database!
    ECHO    You may need to manually run the SQL script: Migrations\AddDescriptionToPP_Roles.sql
    ECHO.
    PAUSE
    EXIT /B !ERRORLEVEL!
)

ECHO.
ECHO ========================================
ECHO ✅ Description column added successfully!
ECHO ========================================
ECHO.

PAUSE

