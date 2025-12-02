@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

REM Ensure running from correct directory (directory containing .csproj)
CD /D "%~dp0"

REM Check if .csproj file exists
IF NOT EXIST "CMS_2026.csproj" (
    ECHO ❌ Error: CMS_2026.csproj file not found
    ECHO    Please run this script from the directory containing the .csproj file
    ECHO    Current directory: %CD%
    PAUSE
    EXIT /B 1
)

ECHO ========================================
ECHO  Auto Update Database Migration Tool
ECHO ========================================
ECHO.
ECHO Working directory: %CD%
ECHO.

ECHO [1/3] Checking for model changes...
ECHO.

REM Generate automatic migration name
FOR /F %%i IN ('powershell -NoProfile -Command "Get-Date -Format yyyyMMddHHmmss"') DO SET "migrationName=%%i_AutoUpdate"

ECHO    Migration name will be: %migrationName%
ECHO    Attempting to create migration...

REM Try to create migration to check for model changes
dotnet ef migrations add "%migrationName%" --context ApplicationDbContext --project CMS_2026.csproj --output-dir Migrations >temp_migration_output.txt 2>&1
SET "migrationResult=!ERRORLEVEL!"

REM Check if migration file was created
IF EXIST "Migrations\%migrationName%.cs" (
    REM Migration created successfully
    ECHO ✅ Model changes detected!
    ECHO    Created migration: %migrationName%
    ECHO    File: Migrations\%migrationName%.cs
    ECHO.
    
    REM Check if Designer file exists
    IF EXIST "Migrations\%migrationName%.Designer.cs" (
        ECHO    ✅ Designer file created
    )
    
    REM Check if Snapshot was updated
    IF EXIST "Migrations\ApplicationDbContextModelSnapshot.cs" (
        ECHO    ✅ Snapshot updated
    )
    
    ECHO.
    SET "hasChanges=1"
    DEL temp_migration_output.txt >nul 2>&1
) ELSE (
    REM Check output to see if it's "No changes"
    findstr /C:"No changes" temp_migration_output.txt >nul 2>&1
    SET "noChanges=!ERRORLEVEL!"
    
    IF "!noChanges!"=="0" (
        REM No model changes
        ECHO ℹ️  No model changes detected.
        ECHO.
        SET "hasChanges=0"
        DEL temp_migration_output.txt >nul 2>&1
    ) ELSE (
        REM Other error - display error
        ECHO ⚠️  Unable to create migration:
        TYPE temp_migration_output.txt | findstr /V /C:"Build started" /C:"Build succeeded" /C:"Build failed" /C:"dotnet" /C:"Microsoft"
        ECHO.
        ECHO    Will continue to update database with existing migrations...
        ECHO.
        SET "hasChanges=0"
        DEL temp_migration_output.txt >nul 2>&1
    )
)

ECHO [2/3] Applying migrations to database...
ECHO.

REM Apply migrations to database
dotnet ef database update --context ApplicationDbContext --project CMS_2026.csproj
SET "updateResult=!ERRORLEVEL!"

IF NOT "!updateResult!"=="0" (
    ECHO.
    ECHO ❌ Error updating database!
    ECHO    Please check:
    ECHO    - Connection string in appsettings.json
    ECHO    - SQL Server is running
    ECHO    - Database exists and has proper access
    ECHO.
    
    REM If migration was created but update failed, ask if user wants to remove it
    IF "!hasChanges!"=="1" (
        IF EXIST "Migrations\%migrationName%.cs" (
            ECHO ⚠️  Migration was created but not applied: %migrationName%
            ECHO    File: Migrations\%migrationName%.cs
            ECHO.
            ECHO    Do you want to remove this migration? (Y/N)
            SET /P "deleteChoice="
            IF /I "!deleteChoice!"=="Y" (
                ECHO    Removing migration...
                dotnet ef migrations remove --context ApplicationDbContext --project CMS_2026.csproj --force >nul 2>&1
                ECHO    ✅ Migration removed.
            ) ELSE (
                ECHO    Migration kept. You can fix the error and run update-db.cmd again
            )
        )
    )
    
    PAUSE
    EXIT /B !updateResult!
)

ECHO.
ECHO [3/3] ✅ Completed!
ECHO.

IF "!hasChanges!"=="1" (
    REM Check again if migration file exists
    IF EXIST "Migrations\%migrationName%.cs" (
        ECHO 📝 Created and applied new migration: %migrationName%
        ECHO    ✅ Migration file saved at: Migrations\%migrationName%.cs
        ECHO.
        ECHO    Migration files list:
        DIR /B "Migrations\%migrationName%*" 2>nul
    ) ELSE (
        ECHO ⚠️  Migration was applied but file not found!
        ECHO    Migration may have been applied but file was deleted or not created correctly.
        ECHO    Please check the Migrations\ directory
    )
) ELSE (
    ECHO 📝 Checked and applied pending migrations
)

ECHO.
ECHO ========================================
ECHO ✅ Database updated successfully!
ECHO ========================================
ECHO.

PAUSE

