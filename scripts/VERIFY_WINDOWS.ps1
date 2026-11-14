# Run from the ZIP project root in PowerShell. Requires .NET 10 SDK.
$ErrorActionPreference = "Stop"
Set-Location (Split-Path -Parent $PSScriptRoot)

function Run-Step {
    param([string]$Description, [scriptblock]$Command)
    Write-Host "`n=== $Description ===" -ForegroundColor Cyan
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Description failed with exit code $LASTEXITCODE" }
}

Run-Step "Restore" { dotnet restore .\MoodLog.slnx }
Run-Step "Release build" { dotnet build .\MoodLog.slnx -c Release --no-restore }
Run-Step "Unit/integration tests + coverage" { dotnet test .\tests\MoodLog.Tests\MoodLog.Tests.csproj -c Release --no-build --collect:"XPlat Code Coverage" }
Run-Step "Build browser tests" { dotnet build .\tests\MoodLog.E2E\MoodLog.E2E.csproj -c Release }
$playwrightInstall = Join-Path $PWD "tests\MoodLog.E2E\bin\Release\net10.0\playwright.ps1"
if (-not (Test-Path $playwrightInstall)) { throw "Playwright installation script not found: $playwrightInstall" }
Run-Step "Install Chromium" { & $playwrightInstall install chromium }
Run-Step "Browser tests" { dotnet test .\tests\MoodLog.E2E\MoodLog.E2E.csproj -c Release --no-build }
Write-Host "`nAll automated checks passed. Run the manual QA checklist before submission." -ForegroundColor Green
