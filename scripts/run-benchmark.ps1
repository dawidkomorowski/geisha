Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$projectPath = Resolve-Path -Path "..\benchmark\Geisha.Benchmark\Geisha.Benchmark.csproj"
$projectDir = Split-Path -Path $projectPath -Parent

Push-Location -Path $projectDir
try {
	dotnet run --project $projectPath --configuration Release 2>&1 | Out-Default
}
finally {
	Pop-Location
}

$outputPath = dotnet msbuild $projectPath -nologo -p:Configuration=Release -getProperty:TargetDir
if (-not $outputPath) {
	throw "Failed to resolve benchmark output directory."
}

$outputPath = $outputPath.Trim()
if (-not (Test-Path -Path $outputPath)) {
	throw "Benchmark output directory does not exist: $outputPath"
}

New-Item -ItemType Directory -Path .\Benchmark.Artifacts -Force | Out-Null
Move-Item -Path (Join-Path $outputPath "BenchmarkResults*.json") -Destination .\Benchmark.Artifacts -Force