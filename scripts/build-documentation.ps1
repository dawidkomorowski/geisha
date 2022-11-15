Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

$docfxVersion = "2.59.4"

Write-Host "Building documentation."

if (-Not (Test-Path -Path bin\docfx\docfx.exe)) {
    Write-Warning "DocFX is not installed. Installing DocFX."

    if (-Not (Test-Path -Path bin)) {
        Write-Host "Creating bin directory."
        New-Item -ItemType Directory -Path bin | Out-Null
    }

    if (-Not (Test-Path -Path bin\docfx)) {
        Write-Host "Creating bin\docfx directory."
        New-Item -ItemType Directory -Path bin\docfx | Out-Null
    }

    $docfxUri = "https://github.com/dotnet/docfx/releases/download/v$docfxVersion/docfx.zip"
    Write-Host "Downloading DocFX from $docfxUri"
    Invoke-WebRequest -Uri $docfxUri -OutFile bin\docfx\docfx.zip

    Write-Host "Extracting bin\docfx\docfx.zip"
    Expand-Archive -Path bin\docfx\docfx.zip -DestinationPath bin\docfx
}

.\bin\docfx\docfx.exe ..\docs\docfx.json $args[0]