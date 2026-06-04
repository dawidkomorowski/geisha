Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Import-Module -Name .\modules\Benchmarks.psm1 -Force
Import-Module -Name .\modules\Windows.psm1 -Force

$binPath = Resolve-Path -Path "bin"
$benchmarkAppPackageName = "benchmark-app.zip"
$currentBenchmarkProjectPath = Resolve-Path -Path "..\benchmark\Geisha.Benchmark\Geisha.Benchmark.csproj"
$currentBenchmarkProjectDir = Split-Path -Path $currentBenchmarkProjectPath -Parent
$currentBenchmarkOutputDir = dotnet msbuild $currentBenchmarkProjectPath -nologo -p:Configuration=Release -getProperty:TargetDir
$currentBenchmarkOutputDir = $currentBenchmarkOutputDir.Trim()
$currentBenchmarkDir = "$binPath\current"

### Prepare Geisha.Benchmark binaries ###

New-Item -ItemType Directory -Path $binPath -Force | Out-Null

if (-not (Test-Path -PathType Leaf -Path "$binPath\$benchmarkAppPackageName")) {
    Write-Warning "Benchmark app package is missing."

    $downloadPageLink = Get-LinkToArtifactDownloadPage
    Write-Host "Download '$benchmarkAppPackageName' from $downloadPageLink and place it in $(Resolve-Path -Path $binPath)"

    Write-Warning "Skipping benchmarks execution."
    return
}

if (-not (Test-Path -PathType Container -Path "$binPath\benchmark-app")) {
    Write-Host "Unpacking $binPath\$benchmarkAppPackageName"
    Expand-Archive -Path "$binPath\$benchmarkAppPackageName" -DestinationPath "$binPath\benchmark-app"
}

$zip = Resolve-Path -Path "$binPath\benchmark-app\Geisha.Benchmark.*.zip"
$dir = (Get-Item $zip).BaseName
$masterBenchmarkDir = "$binPath\benchmark-app\$dir"

if (-not (Test-Path -PathType Container -Path $masterBenchmarkDir)) {
    Write-Host "Unpacking $zip"
    Expand-Archive -Path $zip -DestinationPath $masterBenchmarkDir
}

New-Item -ItemType Directory -Path $currentBenchmarkDir -Force | Out-Null

### Run Benchmarks ###

Write-Host "Removing stale benchmark results."
Remove-Item -Path "$currentBenchmarkDir\BenchmarkResults*.json" -ErrorAction SilentlyContinue
Remove-Item -Path "$currentBenchmarkOutputDir\BenchmarkResults*.json" -ErrorAction SilentlyContinue
Remove-Item -Path "$masterBenchmarkDir\BenchmarkResults*.json" -ErrorAction SilentlyContinue

$startTime = Get-Date

Set-ThreadExecutionState($ES_DISPLAY_REQUIRED)

Write-Host "Execute performance benchmarks (current 1/3)" -ForegroundColor Cyan
Set-Location -Path $currentBenchmarkProjectDir
dotnet run --project $currentBenchmarkProjectPath --configuration Release 2>&1 | Out-Default
Move-Item -Path "$currentBenchmarkOutputDir\BenchmarkResults*.json" -Destination $currentBenchmarkDir -Force
Set-Location -Path $PSScriptRoot

Set-ThreadExecutionState($ES_DISPLAY_REQUIRED)

Write-Host "Execute performance benchmarks (master 1/3)" -ForegroundColor Cyan
Set-Location -Path $masterBenchmarkDir
.\Geisha.Benchmark.exe 2>&1 | Out-Default
Set-Location -Path $PSScriptRoot

Set-ThreadExecutionState($ES_DISPLAY_REQUIRED)

Write-Host "Execute performance benchmarks (current 2/3)" -ForegroundColor Cyan
Set-Location -Path $currentBenchmarkProjectDir
dotnet run --project $currentBenchmarkProjectPath --configuration Release 2>&1 | Out-Default
Move-Item -Path "$currentBenchmarkOutputDir\BenchmarkResults*.json" -Destination $currentBenchmarkDir -Force
Set-Location -Path $PSScriptRoot

Set-ThreadExecutionState($ES_DISPLAY_REQUIRED)

Write-Host "Execute performance benchmarks (master 2/3)" -ForegroundColor Cyan
Set-Location -Path $masterBenchmarkDir
.\Geisha.Benchmark.exe 2>&1 | Out-Default
Set-Location -Path $PSScriptRoot

Set-ThreadExecutionState($ES_DISPLAY_REQUIRED)

Write-Host "Execute performance benchmarks (current 3/3)" -ForegroundColor Cyan
Set-Location -Path $currentBenchmarkProjectDir
dotnet run --project $currentBenchmarkProjectPath --configuration Release 2>&1 | Out-Default
Move-Item -Path "$currentBenchmarkOutputDir\BenchmarkResults*.json" -Destination $currentBenchmarkDir -Force
Set-Location -Path $PSScriptRoot

Set-ThreadExecutionState($ES_DISPLAY_REQUIRED)

Write-Host "Execute performance benchmarks (master 3/3)" -ForegroundColor Cyan
Set-Location -Path $masterBenchmarkDir
.\Geisha.Benchmark.exe 2>&1 | Out-Default
Set-Location -Path $PSScriptRoot

$endTime = Get-Date
$duration = $endTime - $startTime
Write-Host "Benchmarks completed in: $duration" -ForegroundColor Green

### Generate Report ###

Write-Host "Generating benchmark results report..."

$currentResults = Import-Results -Path $currentBenchmarkDir
$masterResults = Import-Results -Path $masterBenchmarkDir
$finalResults = Merge-Results -CurrentResults $currentResults -MasterResults $masterResults
$resultsTableMardown = Format-Results -Results $finalResults

$dateForFileName = $startTime.ToString("yyyy-MM-dd--HH-mm-ss")
$dateSortable = $startTime.ToString("s")

$resultsFile = ".\Benchmark.Artifacts\BenchmarkResults--$dateForFileName.md"
New-Item -ItemType Directory -Path .\Benchmark.Artifacts -Force | Out-Null
New-Item -ItemType File -Path $resultsFile -Force | Out-Null

Add-Content -Path $resultsFile -Value "## Performance Benchmark Results $dateSortable$([Environment]::NewLine)"

$computerInfo = Get-ComputerInfo
$firstProcessor = $computerInfo.CsProcessors[0]
Add-Content -Path $resultsFile -Value "### System Info"
Add-Content -Path $resultsFile -Value "| Property | Value |"
Add-Content -Path $resultsFile -Value "| :- | -: |"
Add-Content -Path $resultsFile -Value "OS Name|$($computerInfo.OsName)"
Add-Content -Path $resultsFile -Value "CPU|$($firstProcessor.Name)"
Add-Content -Path $resultsFile -Value "CPU Physical Cores|$($firstProcessor.NumberOfCores)"
Add-Content -Path $resultsFile -Value "CPU Logical Cores|$($firstProcessor.NumberOfLogicalProcessors)"
Add-Content -Path $resultsFile -Value "RAM|$((Get-CimInstance Win32_PhysicalMemory | Measure-Object -Property capacity -Sum).sum /1gb)GB"
Add-Content -Path $resultsFile -Value "GPU|$((Get-WmiObject Win32_VideoController).Name)"

Add-Content -Path $resultsFile -Value "### Benchmark Results"
Add-Content -Path $resultsFile -Value $resultsTableMardown

Write-Host "Benchmark results report created: $($resultsFile)" -ForegroundColor Green