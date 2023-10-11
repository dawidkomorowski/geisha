Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Set-Location -Path ..\benchmark\Geisha.Benchmark\bin\Release\net6.0-windows\
.\Geisha.Benchmark.exe 2>&1 | Out-Default

Set-Location -Path $PSScriptRoot

New-Item -ItemType Directory -Path .\Benchmark.Artifacts -Force | Out-Null
Copy-Item -Path ..\benchmark\Geisha.Benchmark\bin\Release\net6.0-windows\BenchmarkResults*.json -Destination .\Benchmark.Artifacts