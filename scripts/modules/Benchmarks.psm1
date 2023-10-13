function Get-LinkToArtifactDownloadPage {
    $workflowName = "Build/Test/Publish"
    
    $workflowsJson = Invoke-WebRequest -Uri "https://api.github.com/repos/dawidkomorowski/geisha/actions/runs?branch=master"
    $workflows = ConvertFrom-Json $workflowsJson

    $workflow = $workflows.workflow_runs | Where-Object { $_.Name -EQ $workflowName } | Select-Object -First 1

    return $workflow.html_url
}