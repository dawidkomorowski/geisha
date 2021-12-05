Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"
$versionToSet = $args[0]

function Set-Version([string] $projectPath, [string] $version) {
    Write-Host "Setting version: $version for project: $projectPath."
    $content = Get-Content -Path $projectPath -Raw
    $content = $content -Replace "<Version>(\d+\.\d+\.\d+|\d+\.\d+\.\d+\.\d+)<\/Version>", "<Version>$version</Version>"
    Set-Content -Path $projectPath -Value $content -NoNewline

    $updatedContent = Get-Content -Path $projectPath -Raw
    if ($updatedContent -notmatch $version) {
        throw "Failed to set version for $projectPath."
    }
}

Set-Version "..\src\Geisha.Cli\Geisha.Cli.csproj" $versionToSet
Set-Version "..\src\Geisha.Common\Geisha.Common.csproj" $versionToSet
Set-Version "..\src\Geisha.Engine\Geisha.Engine.csproj" $versionToSet
Set-Version "..\src\Geisha.Engine.Audio.CSCore\Geisha.Engine.Audio.CSCore.csproj" $versionToSet
Set-Version "..\src\Geisha.Engine.Input.Windows\Geisha.Engine.Input.Windows.csproj" $versionToSet
Set-Version "..\src\Geisha.Engine.Rendering.DirectX\Geisha.Engine.Rendering.DirectX.csproj" $versionToSet
Set-Version "..\src\Geisha.Engine.Windows\Geisha.Engine.Windows.csproj" $versionToSet
Set-Version "..\src\Geisha.Tools\Geisha.Tools.csproj" $versionToSet