Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"

if($args[0]) {
    $buildArtifactsPath = $args[0]
}
else {
    throw "Missing argument: path to build artifacts."
}

$projectVersion = Get-Content -Path "..\project.version" -Raw

$buildArtifactsDirectoryPath = $((Get-Item $buildArtifactsPath ).DirectoryName)
$extractedBuildArtifactsPath = "$buildArtifactsDirectoryPath\$((Get-Item $buildArtifactsPath ).BaseName)"
Expand-Archive -Path $buildArtifactsPath -DestinationPath $extractedBuildArtifactsPath -Force

$releasePackageName = "GeishaSDK.$projectVersion"
$tempReleasePackagePath = "$buildArtifactsDirectoryPath\$releasePackageName"
New-Item -ItemType Directory -Path $tempReleasePackagePath
New-Item -ItemType Directory -Path "$tempReleasePackagePath\lib"
New-Item -ItemType Directory -Path "$tempReleasePackagePath\tools"

Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Cli\bin\Release\Geisha.Cli.*.nupkg" -Destination "$tempReleasePackagePath\tools"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Common\bin\Release\Geisha.Common.*.nupkg" -Destination "$tempReleasePackagePath\lib"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Engine\bin\Release\Geisha.Engine.*.nupkg" -Destination "$tempReleasePackagePath\lib"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Engine.Audio.CSCore\bin\Release\Geisha.Engine.Audio.CSCore.*.nupkg" -Destination "$tempReleasePackagePath\lib"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Engine.Input.Windows\bin\Release\Geisha.Engine.Input.Windows.*.nupkg" -Destination "$tempReleasePackagePath\lib"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Engine.Rendering.DirectX\bin\Release\Geisha.Engine.Rendering.DirectX.*.nupkg" -Destination "$tempReleasePackagePath\lib"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Engine.Windows\bin\Release\Geisha.Engine.Windows.*.nupkg" -Destination "$tempReleasePackagePath\lib"
Copy-Item -Path "$extractedBuildArtifactsPath\Geisha.Tools\bin\Release\Geisha.Tools.*.nupkg" -Destination "$tempReleasePackagePath\lib"

Compress-Archive -Path "$tempReleasePackagePath\*" -DestinationPath "$tempReleasePackagePath.zip" -CompressionLevel Optimal -Force

Remove-Item -Path $tempReleasePackagePath -Recurse