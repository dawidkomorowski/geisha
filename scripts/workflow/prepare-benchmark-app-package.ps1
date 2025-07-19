Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

Import-Module -Name ..\modules\Version.psm1 -Force

$packageName = "Geisha.Benchmark.$(Get-SemVer)"
$packagePath = "..\..\$packageName"
New-Item -ItemType Directory -Path $packagePath

# Package published content
Copy-Item -Path "..\..\benchmark\Geisha.Benchmark\bin\Release\net6.0-windows\win-x64\publish\*" -Destination "$packagePath" -Recurse -Exclude @("*.pdb", "*.xml")

# Packege misc
New-Item -ItemType File -Path "$packagePath" -Name ".version" -Value (Get-BuildVersion)
Copy-Item -Path "..\..\LICENSE" -Destination "$packagePath"

Compress-Archive -Path "$packagePath\*" -DestinationPath "$packagePath.zip" -CompressionLevel Optimal -Force

Remove-Item -Path $packagePath -Recurse