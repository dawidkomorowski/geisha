Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

# Parameters
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

# Functions
function Import-Results {
    param (
        $Path
    )
    
    $files = Get-ChildItem "$Path\BenchmarkResults--*" | Sort-Object -Property CreationTimeUtc

    $results = @()

    foreach ($file in $files) {
        $json = Get-Content -Path $file.FullName -Raw

        $result = [pscustomobject]@{
            RunNumber = $results.Count + 1;
            Data      = ConvertFrom-Json -InputObject $json
        }
    
        $results += $result
    }

    return $results
}

function Merge-Results {
    param (
        $CurrentResults,
        $MasterResults
    )

    if ($CurrentResults.Count -ne $MasterResults.Count) {
        throw "Mismatch in count of current and master results."
    }

    $results = @()

    $benchmarks = $CurrentResults[0].Data
    foreach ($benchmark in $benchmarks) {

        $current = @()
        foreach ($currentResult in $CurrentResults) {
            $benchmarkData = $currentResult.Data | Where-Object -Property BenchmarkName -EQ $benchmark.BenchmarkName

            $current += [pscustomobject]@{
                RunNumber = $currentResult.RunNumber;
                Frames    = [int]$benchmarkData.Frames
            }
        }

        $master = @()
        foreach ($masterResult in $MasterResults) {
            $benchmarkData = $masterResult.Data | Where-Object -Property BenchmarkName -EQ $benchmark.BenchmarkName

            if ($benchmarkData) {
                $master += [pscustomobject]@{
                    RunNumber = $masterResult.RunNumber;
                    Frames    = [int]$benchmarkData.Frames
                }
            }
            else {
                $master += [pscustomobject]@{
                    RunNumber = $masterResult.RunNumber;
                    Frames    = [int]-1
                }
            }            
        }

        $currentAvg = ($current | Select-Object -ExpandProperty Frames | Measure-Object -Average).Average
        $masterAvg = ($master | Select-Object -ExpandProperty Frames | Measure-Object -Average).Average

        $result = [pscustomobject]@{
            BenchmarkName = $benchmark.BenchmarkName;
            FixedFrames   = $benchmark.FixedFrames
            Current       = $current;
            CurrentAvg    = $currentAvg;
            Master        = $master;
            MasterAvg     = $masterAvg;
            AvgRatio      = $currentAvg / $masterAvg
        }

        $results += $result
    }

    return $results
}

function Format-Results {
    param (
        $Results
    )
    
    # Build table header
    $outputText = "| Benchmark | Fixed Frames | Avg Ratio |"

    foreach ($data in $Results[0].Current) {
        $outputText = "$outputText Frames ( Current $($data.RunNumber) ) |"
    }

    $outputText = "$outputText Frames ( Current Avg ) |"

    foreach ($data in $Results[0].Master) {
        $outputText = "$outputText Frames ( Master $($data.RunNumber) ) |"
    }

    $outputText = "$outputText Frames ( Master Avg ) |"

    $outputText = "$outputText$([Environment]::NewLine)"

    # Configure column alignment
    $outputText = "$outputText :- | -: | -:"

    foreach ($data in $Results[0].Current) {
        $outputText = "$outputText | -:"
    }

    $outputText = "$outputText | -:"

    foreach ($data in $Results[0].Master) {
        $outputText = "$outputText | -:"
    }

    $outputText = "$outputText | -:"

    $outputText = "$outputText$([Environment]::NewLine)"

    # Build table rows
    foreach ($result in $Results) {
        $outputText = "$outputText $($result.BenchmarkName)|$($result.FixedFrames)|$("{0:P2}" -f $result.AvgRatio)"

        foreach ($data in $result.Current) {
            $outputText = "$outputText|$($data.Frames)"
        }
    
        $outputText = "$outputText|$($result.CurrentAvg)"
    
        foreach ($data in $result.Master) {
            if ($data.Frames -eq -1) {
                $outputText = "$outputText|No data"
            }
            else {
                $outputText = "$outputText|$($data.Frames)"
            }
        }
    
        $outputText = "$outputText|$($result.MasterAvg)"
    
        $outputText = "$outputText$([Environment]::NewLine)"   
    }

    return $outputText
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
# End of Functions

$currentResults = Import-Results -Path "..\..\benchmark\Benchmark\bin\Release\net5.0-windows\"
$masterResults = Import-Results -Path "..\..\benchmark-bin\"

$finalResults = Merge-Results -CurrentResults $currentResults -MasterResults $masterResults

$outputText = Format-Results -Results $finalResults

Publish-CheckRun -GitHubToken $githubToken -HeadSha $headSha -Name "Benchmark Results" -Summary "Total benchmarks executed: $($finalResults.Count)" -Text $outputText