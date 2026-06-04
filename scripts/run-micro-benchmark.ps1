Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$projectPath = Resolve-Path -Path "..\benchmark\Geisha.MicroBenchmark\Geisha.MicroBenchmark.csproj"
dotnet run --project $projectPath --configuration Release -- @args
