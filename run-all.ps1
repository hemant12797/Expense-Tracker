# SpendSmart Full Stack Startup Script
# Run this from: c:\Projects\Expense_Tracker\
# Usage: .\run-all.ps1

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backendDir = Join-Path $rootDir "Backend"
$frontendDir = Join-Path $rootDir "Frontend"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   SpendSmart Full Stack Launcher       " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kill any old dotnet processes on our ports
Write-Host "Cleaning up old processes..." -ForegroundColor Yellow
$ports = @(5290, 5025, 5149, 5254, 5207, 5220, 5252, 4200)
foreach ($port in $ports) {
    $processId = (netstat -ano 2>$null | Select-String ":$port " | ForEach-Object { ($_ -split '\s+')[-1] } | Select-Object -First 1)
    if ($processId -and $processId -match '^\d+$' -and $processId -ne '0') {
        try { Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue } catch {}
    }
}
Start-Sleep -Seconds 1

# Services to start: [ServiceDir, Port, Name]
$services = @(
    @{ Dir = "SpendSmart.Auth.API";         Port = 5290; Name = "Auth API" }
    @{ Dir = "SpendSmart.Expense.API";      Port = 5025; Name = "Expense API" }
    @{ Dir = "SpendSmart.Income.API";       Port = 5252; Name = "Income API"}
    @{ Dir = "SpendSmart.Budget.API";       Port = 5149; Name = "Budget API" }
    @{ Dir = "SpendSmart.Category.API";     Port = 5254; Name = "Category API" }
    @{ Dir = "SpendSmart.Notification.API"; Port = 5207; Name = "Notification API" }
    @{ Dir = "SpendSmart.Report.API";       Port = 5220; Name = "Report API" }
)

Write-Host "Starting Backend Microservices..." -ForegroundColor Cyan
Write-Host ""

foreach ($svc in $services) {
    $svcPath = Join-Path $backendDir $svc.Dir
    $title = $svc.Name + " :" + $svc.Port
    Write-Host "  -> $($svc.Name) on http://localhost:$($svc.Port)" -ForegroundColor Green
    Start-Process powershell -ArgumentList "-NoLogo", "-NoExit", "-Command", "cd '$svcPath'; `$host.UI.RawUI.WindowTitle = '$title'; dotnet run" -WindowStyle Minimized
    Start-Sleep -Milliseconds 500
}

Write-Host ""
Write-Host "Starting Angular Frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoLogo", "-NoExit", "-Command", "cd '$frontendDir'; `$host.UI.RawUI.WindowTitle = 'SpendSmart Frontend :4200'; npm start"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host " All services started!" -ForegroundColor Green
Write-Host ""
Write-Host " Frontend:          http://localhost:4200" -ForegroundColor White
Write-Host " Auth API:          http://localhost:5290" -ForegroundColor Gray
Write-Host " Expense API:       http://localhost:5025" -ForegroundColor Gray
Write-Host " Budget API:        http://localhost:5149" -ForegroundColor Gray
Write-Host " Category API:      http://localhost:5254" -ForegroundColor Gray
Write-Host " Notification API:  http://localhost:5207" -ForegroundColor Gray
Write-Host " Report API:        http://localhost:5220" -ForegroundColor Gray
Write-Host ""
Write-Host " Wait ~30 seconds for APIs to fully initialize," -ForegroundColor Yellow
Write-Host " then open: http://localhost:4200" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Green
