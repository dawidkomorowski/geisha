Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

if ($args[0]) {
    $headSha = $args[0]
}
else {
    throw "Missing argument: commit SHA."
}

if ($args[1]) {
    $githubToken = $args[1]
}
else {
    throw "Missing argument: github token."
}

$rawResults = Get-Content -Path "..\benchmark\Benchmark\bin\Release\netcoreapp3.1\BenchmarkResults--*" -Raw
$jsonResults = ConvertFrom-Json -InputObject $rawResults

$outputText = "| Benchmark | Fixed frames | Frames |$([Environment]::NewLine)"
$outputText = "$outputText :- | -: | -:$([Environment]::NewLine)" 

foreach ($result in $jsonResults) {
    $outputText = "$outputText$($result.BenchmarkName)|$($result.FixedFrames)|$($result.Frames)$([Environment]::NewLine)"
}

$url = "https://api.github.com/repos/dawidkomorowski/geisha/check-runs"
$headers = @{
    Accept        = 'application/vnd.github.antiope-preview+json'
    Authorization = "token $githubToken"
}
$body = @{
    name       = "Benchmark Results"
    head_sha   = $headSha
    status     = "completed"
    conclusion = "success"
    output     = @{
        title   = "Benchmark Results"
        summary = "Total benchmarks executed: $($jsonResults.Count)"
        text    = $outputText
    }
}
Invoke-WebRequest -Headers $headers $url -Method Post -Body ($body | ConvertTo-Json)