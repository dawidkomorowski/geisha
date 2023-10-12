function Get-BenchmarkAppFromGH {
    $artifactName = "benchmark-app"
    $artifactsJson = Invoke-WebRequest -Uri "https://api.github.com/repos/dawidkomorowski/geisha/actions/artifacts?name=$artifactName"
    $artifacts = ConvertFrom-Json $artifactsJson

    $artifact = $artifacts.artifacts | Where-Object { $_.workflow_run.head_branch -EQ "master" } | Select-Object -First 1

    Write-Host "Downloading '$artifactName' from latest '$($artifact.workflow_run.head_branch)' build SHA '$($artifact.workflow_run.head_sha)'"
}