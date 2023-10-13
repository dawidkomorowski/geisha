Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Import-Module -Name .\modules\Benchmarks.psm1 -Force

$binPath = "bin"
$benchmarkAppPackageName = "benchmark-app.zip"

New-Item -ItemType Directory -Path $binPath -Force | Out-Null

if (-not (Test-Path -Path "$binPath\$benchmarkAppPackageName")) {
    Write-Warning "Benchmark app package is missing."

    $downloadPageLink = Get-LinkToArtifactDownloadPage
    Write-Host "Download '$benchmarkAppPackageName' from $downloadPageLink and place it in $(Resolve-Path -Path $binPath)"

    Write-Warning "Skipping benchmarks execution."
    return
}

if (-not (Test-Path -Path "$binPath\benchmark-app")) {
    Expand-Archive -Path $benchmarkAppPackageName
}