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

# Parse results
$parsedResults = @()
$rawResults = Get-ChildItem "..\..\benchmark-results\"
foreach ($rawResult in $rawResults) {
    $jsonResults = Get-Content -Path "$($rawResult.FullName)\BenchmarkResults--*" -Raw

    $parsedResult = [pscustomobject]@{
        BuildNumber   = [int]$rawResult.Name;
        ResultsObject = ConvertFrom-Json -InputObject $jsonResults
    }
    
    $parsedResults += $parsedResult
}

$parsedResults = $parsedResults | Sort-Object -Property BuildNumber -Descending

# Build table header
$outputText = "| Benchmark | Fixed frames |"

foreach ($parsedResult in $parsedResults) {
    $outputText = "$outputText Frames ($($parsedResult.BuildNumber)) |"
}

$outputText = "$outputText$([Environment]::NewLine)"
$outputText = "$outputText :- | -:" 

foreach ($parsedResult in $parsedResults) {
    $outputText = "$outputText | -:" 
}
$outputText = "$outputText$([Environment]::NewLine)"

# Build table rows
$currentResult = $parsedResults[0]

foreach ($result in $currentResult.ResultsObject) {
    $outputText = "$outputText$($result.BenchmarkName)|$($result.FixedFrames)"

    foreach ($parsedResult in $parsedResults) {
        $currentBenchmarkResults = $parsedResult.ResultsObject | Where-Object -Property BenchmarkName -EQ $result.BenchmarkName

        if ($currentBenchmarkResults) {
            $outputText = "$outputText|$($currentBenchmarkResults.Frames)"
        }
        else {
            $outputText = "$outputText|No data"
        }
    }

    $outputText = "$outputText$([Environment]::NewLine)"
}

# Post results
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
        summary = "Total benchmarks executed: $($currentResult.ResultsObject.Count)"
        text    = $outputText
    }
}
Invoke-WebRequest -Headers $headers $url -Method Post -Body ($body | ConvertTo-Json)