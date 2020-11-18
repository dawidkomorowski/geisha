Set-Location -Path $PSScriptRoot
$ErrorActionPreference = "Stop"
$currentStep = 0
$totalSteps = 4

function Write-Step([string] $stepName) {
    $script:currentStep++
    Write-Host "Step $script:currentStep/$totalSteps - $stepName" -ForegroundColor Cyan
    
    if($currentStep -gt $totalSteps) {
        throw "Unexpected number of steps."
    }
}

Write-Step "Build solution..."
dotnet build ..\Geisha.sln --configuration Debug

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Step "Execute tests..."
dotnet test ..\Geisha.sln --configuration Debug

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Step "Publish packages..."
dotnet publish ..\Geisha.sln --configuration Release

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Step "Copy nuget packages to publish directory..."
New-Item -Path ".." -Name "publish" -ItemType "Directory" -Force
Copy-Item -Path "..\src\Geisha.Common\bin\Release\Geisha.Common.*.nupkg" -Destination "..\publish"
Copy-Item -Path "..\src\Geisha.Engine\bin\Release\Geisha.Engine.*.nupkg" -Destination "..\publish"
Copy-Item -Path "..\src\Geisha.Engine.Audio.CSCore\bin\Release\Geisha.Engine.Audio.CSCore.*.nupkg" -Destination "..\publish"
Copy-Item -Path "..\src\Geisha.Engine.Input.Windows\bin\Release\Geisha.Engine.Input.Windows.*.nupkg" -Destination "..\publish"
Copy-Item -Path "..\src\Geisha.Engine.Rendering.DirectX\bin\Release\Geisha.Engine.Rendering.DirectX.*.nupkg" -Destination "..\publish"
Copy-Item -Path "..\src\Geisha.Engine.Windows\bin\Release\Geisha.Engine.Windows.*.nupkg" -Destination "..\publish"

if($currentStep -ne $totalSteps) {
    throw "Unexpected number of steps."
}

Write-Host "Build completed." -ForegroundColor Cyan