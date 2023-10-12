Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Import-Module -Name .\modules\Benchmarks.psm1 -Force

Write-Host "Test"
Get-BenchmarkAppFromGH