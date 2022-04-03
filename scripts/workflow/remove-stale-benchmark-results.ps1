Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$numberOfBenchmarksToKeep = 5

$parsedResults = @()
$results = Get-ChildItem "..\..\benchmark-results\"
foreach ($result in $results) {
    $parsedResult = [pscustomobject]@{
        BuildNumber = [int]$result.Name;
        Path        = $result.FullName
    }
    
    $parsedResults += $parsedResult
}

$parsedResults = $parsedResults | Sort-Object -Property BuildNumber -Descending
$resultsToRemove = $parsedResults | Select-Object -Skip $numberOfBenchmarksToKeep

foreach ($resultToRemove in $resultsToRemove) {
    Remove-Item -Path $resultToRemove.Path -Recurse
}