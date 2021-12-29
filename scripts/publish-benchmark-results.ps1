Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$rawResults = Get-Content -Path "..\benchmark\Benchmark\bin\Release\netcoreapp3.1\BenchmarkResults--*" -Raw
$jsonResults = ConvertFrom-Json -InputObject $rawResults

$markdownOutput = "Benchmark|Fixed frames|Frames"

foreach($result in $jsonResults) {
    $markdownOutput = "$markdownOutput - $($result.BenchmarkName)|$($result.FixedFrames)|$($result.Frames)"
}

Write-Host "::notice title=Performance Benchmark Results::$markdownOutput"