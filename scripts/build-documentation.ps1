Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$serve = $false

if ($args[0]) {
    if ($args[0] -eq "--serve") {
        $serve = $true
    }
    else {
        Write-Error "Unknown parameter: $($args[0])"
    }
}

Write-Host "Restore local dotnet tools."

dotnet tool restore

Write-Host "Building documentation."

if ($serve) {
    dotnet tool run docfx ..\docs\docfx.json --serve
}
else {
    dotnet tool run docfx ..\docs\docfx.json
}
