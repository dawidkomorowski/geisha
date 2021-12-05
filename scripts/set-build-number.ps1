Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"
$buildNumber = $args[0]

$projectVersion = Get-Content -Path "..\project.version" -Raw
$versionToSet = "$projectVersion+$buildNumber"

.\set-version.ps1 $versionToSet