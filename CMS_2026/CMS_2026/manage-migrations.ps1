# Script để quản lý Entity Framework Migrations
# Chạy script này từ thư mục CMS_2026/CMS_2026/CMS_2026

param(
    [Parameter(Position=0)]
    [ValidateSet("add", "update", "list", "remove", "script", "help")]
    [string]$Action = "help",
    
    [Parameter(Position=1)]
    [string]$MigrationName = "",
    
    [Parameter(Position=2)]
    [string]$OutputFile = ""
)

$ErrorActionPreference = "Stop"

# Get project directory
$projectDir = Get-Location
$projectFile = Join-Path $projectDir "CMS_2026.csproj"

if (-not (Test-Path $projectFile)) {
    Write-Host "❌ Error: CMS_2026.csproj not found in current directory" -ForegroundColor Red
    Write-Host "   Please run this script from CMS_2026/CMS_2026/CMS_2026 directory" -ForegroundColor Yellow
    exit 1
}

function Show-Help {
    Write-Host "`n📋 Entity Framework Migrations Manager" -ForegroundColor Cyan
    Write-Host "======================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\manage-migrations.ps1 [action] [options]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Actions:" -ForegroundColor Green
    Write-Host "  add <name>          - Tạo migration mới với tên được chỉ định" -ForegroundColor White
    Write-Host "  update               - Apply tất cả pending migrations vào database" -ForegroundColor White
    Write-Host "  list                 - Hiển thị danh sách migrations (applied và pending)" -ForegroundColor White
    Write-Host "  remove <name>        - Xóa migration (chưa được apply)" -ForegroundColor White
    Write-Host "  script [output]      - Tạo SQL script từ migrations (nếu có output file)" -ForegroundColor White
    Write-Host "  help                 - Hiển thị hướng dẫn này" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Green
    Write-Host "  .\manage-migrations.ps1 add AddUserTable" -ForegroundColor Gray
    Write-Host "  .\manage-migrations.ps1 update" -ForegroundColor Gray
    Write-Host "  .\manage-migrations.ps1 list" -ForegroundColor Gray
    Write-Host "  .\manage-migrations.ps1 script migrations.sql" -ForegroundColor Gray
    Write-Host ""
}

function Add-Migration {
    param([string]$Name)
    
    if ([string]::IsNullOrWhiteSpace($Name)) {
        Write-Host "❌ Error: Migration name is required" -ForegroundColor Red
        Write-Host "   Usage: .\manage-migrations.ps1 add <MigrationName>" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "`n🔄 Creating new migration: $Name" -ForegroundColor Cyan
    Write-Host "   Project: $projectFile" -ForegroundColor Gray
    Write-Host ""
    
    try {
        dotnet ef migrations add $Name --project $projectFile --context ApplicationDbContext
        if ($LASTEXITCODE -eq 0) {
            Write-Host "`n✅ Migration '$Name' created successfully!" -ForegroundColor Green
            Write-Host "   Next step: Run '.\manage-migrations.ps1 update' to apply it to database" -ForegroundColor Yellow
        } else {
            Write-Host "`n❌ Failed to create migration" -ForegroundColor Red
            exit 1
        }
    } catch {
        Write-Host "`n❌ Error: $_" -ForegroundColor Red
        exit 1
    }
}

function Update-Database {
    Write-Host "`n🔄 Applying migrations to database..." -ForegroundColor Cyan
    Write-Host ""
    
    try {
        dotnet ef database update --project $projectFile --context ApplicationDbContext
        if ($LASTEXITCODE -eq 0) {
            Write-Host "`n✅ Database updated successfully!" -ForegroundColor Green
        } else {
            Write-Host "`n❌ Failed to update database" -ForegroundColor Red
            Write-Host "   Please check your connection string in appsettings.json" -ForegroundColor Yellow
            exit 1
        }
    } catch {
        Write-Host "`n❌ Error: $_" -ForegroundColor Red
        exit 1
    }
}

function List-Migrations {
    Write-Host "`n📋 Migration Status" -ForegroundColor Cyan
    Write-Host "===================" -ForegroundColor Cyan
    Write-Host ""
    
    try {
        Write-Host "Applied Migrations:" -ForegroundColor Green
        $applied = dotnet ef migrations list --project $projectFile --context ApplicationDbContext --no-build 2>&1 | Where-Object { $_ -match "^\s*\[.*\]" -or $_ -match "^\s*\d{14}" }
        
        if ($applied) {
            $applied | ForEach-Object {
                if ($_ -match "\[.*\]") {
                    Write-Host "  ✅ $_" -ForegroundColor Green
                } else {
                    Write-Host "  ✅ $_" -ForegroundColor White
                }
            }
        } else {
            Write-Host "  (No migrations applied)" -ForegroundColor Gray
        }
        
        Write-Host ""
        Write-Host "Pending Migrations:" -ForegroundColor Yellow
        $pending = dotnet ef migrations list --project $projectFile --context ApplicationDbContext --no-build 2>&1 | Where-Object { $_ -notmatch "^\s*\[.*\]" -and $_ -match "^\s*\d{14}" }
        
        if ($pending) {
            $pending | ForEach-Object {
                Write-Host "  ⏳ $_" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  (No pending migrations)" -ForegroundColor Gray
        }
        
        Write-Host ""
    } catch {
        Write-Host "❌ Error listing migrations: $_" -ForegroundColor Red
        Write-Host "   Try running: dotnet ef migrations list --project $projectFile" -ForegroundColor Yellow
    }
}

function Remove-Migration {
    param([string]$Name)
    
    if ([string]::IsNullOrWhiteSpace($Name)) {
        Write-Host "❌ Error: Migration name is required" -ForegroundColor Red
        Write-Host "   Usage: .\manage-migrations.ps1 remove <MigrationName>" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "`n⚠️  Removing migration: $Name" -ForegroundColor Yellow
    Write-Host "   Note: Only migrations that haven't been applied can be removed" -ForegroundColor Gray
    Write-Host ""
    
    $confirm = Read-Host "Are you sure? (y/N)"
    if ($confirm -ne "y" -and $confirm -ne "Y") {
        Write-Host "Cancelled." -ForegroundColor Gray
        exit 0
    }
    
    try {
        dotnet ef migrations remove --project $projectFile --context ApplicationDbContext
        if ($LASTEXITCODE -eq 0) {
            Write-Host "`n✅ Migration removed successfully!" -ForegroundColor Green
        } else {
            Write-Host "`n❌ Failed to remove migration" -ForegroundColor Red
            exit 1
        }
    } catch {
        Write-Host "`n❌ Error: $_" -ForegroundColor Red
        exit 1
    }
}

function Generate-Script {
    param([string]$Output)
    
    Write-Host "`n📝 Generating SQL script from migrations..." -ForegroundColor Cyan
    Write-Host ""
    
    $scriptArgs = @("ef", "migrations", "script", "--project", $projectFile, "--context", "ApplicationDbContext")
    
    if (-not [string]::IsNullOrWhiteSpace($Output)) {
        $scriptArgs += "--output", $Output
        Write-Host "   Output file: $Output" -ForegroundColor Gray
    } else {
        Write-Host "   (No output file specified, will print to console)" -ForegroundColor Gray
    }
    
    try {
        & dotnet $scriptArgs
        if ($LASTEXITCODE -eq 0) {
            if (-not [string]::IsNullOrWhiteSpace($Output)) {
                Write-Host "`n✅ SQL script generated: $Output" -ForegroundColor Green
            } else {
                Write-Host "`n✅ SQL script generated" -ForegroundColor Green
            }
        } else {
            Write-Host "`n❌ Failed to generate script" -ForegroundColor Red
            exit 1
        }
    } catch {
        Write-Host "`n❌ Error: $_" -ForegroundColor Red
        exit 1
    }
}

# Main execution
switch ($Action.ToLower()) {
    "add" {
        Add-Migration -Name $MigrationName
    }
    "update" {
        Update-Database
    }
    "list" {
        List-Migrations
    }
    "remove" {
        Remove-Migration -Name $MigrationName
    }
    "script" {
        Generate-Script -Output $OutputFile
    }
    "help" {
        Show-Help
    }
    default {
        Write-Host "❌ Unknown action: $Action" -ForegroundColor Red
        Show-Help
        exit 1
    }
}

