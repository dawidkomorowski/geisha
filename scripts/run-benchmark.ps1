Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Set-Location -Path ..\benchmark\Geisha.Benchmark\bin\Release\net6.0-windows\win-x64\
.\Geisha.Benchmark.exe 2>&1 | Out-Default

Set-Location -Path $PSScriptRoot

New-Item -ItemType Directory -Path .\Benchmark.Artifacts -Force | Out-Null
Move-Item -Path ..\benchmark\Geisha.Benchmark\bin\Release\net6.0-windows\win-x64\BenchmarkResults*.json -Destination .\Benchmark.Artifacts