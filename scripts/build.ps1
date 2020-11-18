Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Write-Host "Step 1/3 - Build solution..." -ForegroundColor Cyan
dotnet build ..\Geisha.sln --configuration Debug

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Step 2/3 - Execute tests..." -ForegroundColor Cyan
dotnet test ..\Geisha.sln --configuration Debug

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Step 3/3 - Publish packages..." -ForegroundColor Cyan
dotnet publish ..\Geisha.sln --configuration Release

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Build completed." -ForegroundColor Cyan