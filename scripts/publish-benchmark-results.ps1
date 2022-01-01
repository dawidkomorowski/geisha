Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

if($args[0]) {
    $headSha = $args[0]
}
else {
    throw "Missing argument: commit SHA."
}

if($args[1]) {
    $githubToken = $args[1]
}
else {
    throw "Missing argument: github token."
}

$rawResults = Get-Content -Path "..\benchmark\Benchmark\bin\Release\netcoreapp3.1\BenchmarkResults--*" -Raw
$jsonResults = ConvertFrom-Json -InputObject $rawResults

$markdownOutput = "Benchmark|Fixed frames|Frames"

foreach($result in $jsonResults) {
    $markdownOutput = "$markdownOutput - $($result.BenchmarkName)|$($result.FixedFrames)|$($result.Frames)"
}

Write-Host "::notice title=Performance Benchmark Results::$markdownOutput"

$url = "https://api.github.com/repos/dawidkomorowski/geisha/check-runs"
$headers = @{
    Accept = 'application/vnd.github.antiope-preview+json'
    Authorization = "token $githubToken"
}
$body = @{
    name       = "Benchmark Results"
    head_sha   = $headSha
    status     = "completed"
    conclusion = "success"
    output     = @{
        title   = "Benchmark Results"
        summary = "This run completed at ``$([datetime]::Now)``"
        text    = "Markdown tests ___XYZ___"
    }
}
Invoke-WebRequest -Headers $headers $url -Method Post -Body ($body | ConvertTo-Json)