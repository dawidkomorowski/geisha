Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$rawResults = Get-Content -Path "..\benchmark\Benchmark\bin\Debug\netcoreapp3.1\BenchmarkResults-*" -Raw
$jsonResults = ConvertFrom-Json -InputObject $rawResults

$markdownOutput = "| Benchmark | Fixed frames | Frames |$([Environment]::NewLine)"
$markdownOutput = "$markdownOutput :- | -: | -:$([Environment]::NewLine)" 

foreach($result in $jsonResults) {
    $markdownOutput = "$markdownOutput$($result.BenchmarkName)|$($result.FixedFrames)|$($result.Frames)$([Environment]::NewLine)"
}

Write-Host "::notice title=Performance Benchmark Results::$markdownOutput"