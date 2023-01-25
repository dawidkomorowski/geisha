Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

if ($args[0]) {
    $buildNumber = $args[0]
}
else {
    throw "Missing argument: build number."
}

$projectVersion = Get-Content -Path "..\..\project.version" -Raw

$packageName = "Geisha.Demo.$projectVersion"
$packagePath = "..\..\$packageName"
New-Item -ItemType Directory -Path $packagePath


# Package published content
Copy-Item -Path "..\..\demo\Geisha.Demo\bin\Release\net5.0-windows\win-x64\publish\*" -Destination "$packagePath" -Recurse -Exclude @("*.pdb", "*.xml")

# Packege misc
New-Item -ItemType File -Path "$packagePath" -Name ".version" -Value "$projectVersion+$buildNumber"
Copy-Item -Path "..\..\LICENSE" -Destination "$packagePath"

Compress-Archive -Path "$packagePath\*" -DestinationPath "$packagePath.zip" -CompressionLevel Optimal -Force

Remove-Item -Path $packagePath -Recurse