Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

if ($args[0]) {
    $buildNumber = $args[0]
}
else {
    throw "Missing argument: build number."
}

$projectVersion = Get-Content -Path "..\..\project.version" -Raw

$sdkPackageName = "GeishaSDK.$projectVersion"
$sdkPackagePath = "..\..\$sdkPackageName"
New-Item -ItemType Directory -Path $sdkPackagePath


# Package libs
New-Item -ItemType Directory -Path "$sdkPackagePath\lib"

Copy-Item -Path "..\..\src\Geisha.Common\bin\Release\Geisha.Common.*.nupkg" -Destination "$sdkPackagePath\lib"
Copy-Item -Path "..\..\src\Geisha.Engine\bin\Release\Geisha.Engine.*.nupkg" -Destination "$sdkPackagePath\lib"
Copy-Item -Path "..\..\src\Geisha.Engine.Audio.CSCore\bin\Release\Geisha.Engine.Audio.CSCore.*.nupkg" -Destination "$sdkPackagePath\lib"
Copy-Item -Path "..\..\src\Geisha.Engine.Input.Windows\bin\Release\Geisha.Engine.Input.Windows.*.nupkg" -Destination "$sdkPackagePath\lib"
Copy-Item -Path "..\..\src\Geisha.Engine.Rendering.DirectX\bin\Release\Geisha.Engine.Rendering.DirectX.*.nupkg" -Destination "$sdkPackagePath\lib"
Copy-Item -Path "..\..\src\Geisha.Engine.Windows\bin\Release\Geisha.Engine.Windows.*.nupkg" -Destination "$sdkPackagePath\lib"
Copy-Item -Path "..\..\src\Geisha.Tools\bin\Release\Geisha.Tools.*.nupkg" -Destination "$sdkPackagePath\lib"

# Package tools
New-Item -ItemType Directory -Path "$sdkPackagePath\tools"
Copy-Item -Path "..\..\src\Geisha.Cli\bin\Release\Geisha.Cli.*.nupkg" -Destination "$sdkPackagePath\tools"

# Packege misc
New-Item -ItemType File -Path "$sdkPackagePath" -Name ".version" -Value "$projectVersion+$buildNumber"
Copy-Item -Path "..\..\sdk\install-geisha-cli.ps1" -Destination "$sdkPackagePath"


Compress-Archive -Path "$sdkPackagePath\*" -DestinationPath "$sdkPackagePath.zip" -CompressionLevel Optimal -Force

Remove-Item -Path $sdkPackagePath -Recurse