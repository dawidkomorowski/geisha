Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Import-Module -Name ..\modules\Benchmarks.psm1 -Force

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

function Publish-CheckRun {
    param (
        $GitHubToken,
        $HeadSha,
        $Name,
        $Summary,
        $Text
    )

    $url = "https://api.github.com/repos/dawidkomorowski/geisha/check-runs"
    $headers = @{
        Accept        = 'application/vnd.github.antiope-preview+json'
        Authorization = "token $GitHubToken"
    }
    $body = @{
        name       = $Name
        head_sha   = $HeadSha
        status     = "completed"
        conclusion = "success"
        output     = @{
            title   = $Name
            summary = $Summary
            text    = $outputText
        }
    }
    Invoke-WebRequest -Headers $headers $url -Method Post -Body ($body | ConvertTo-Json)
}

$currentResults = Import-Results -Path "..\..\benchmark-app\current\Geisha.Benchmark.*\"
$masterResults = Import-Results -Path "..\..\benchmark-app\master\Geisha.Benchmark.*\"

$finalResults = Merge-Results -CurrentResults $currentResults -MasterResults $masterResults

$outputText = Format-Results -Results $finalResults

Publish-CheckRun -GitHubToken $githubToken -HeadSha $headSha -Name "Benchmark Results" -Summary "Total benchmarks executed: $($finalResults.Count)" -Text $outputText