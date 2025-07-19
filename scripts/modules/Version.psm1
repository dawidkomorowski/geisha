function Get-SemVer {
    # Retrieve the semantic version from Directory.Build.props
    if ((Get-Content -Path "$PSScriptRoot\..\..\Directory.Build.props" -Raw) -match '<SemVer>(.*?)</SemVer>') {
        $semVer = $matches[1]
        Write-Debug "Semantic version: $semVer"
    }
    else {
        Write-Error "No semantic version found."
    }

    return $semVer
}

function Get-BuildNumber {
    # Retrieve the previous version tag from Directory.Build.props
    if ((Get-Content -Path "$PSScriptRoot\..\..\Directory.Build.props" -Raw) -match '<PreviousVersionTag>(.*?)</PreviousVersionTag>') {
        $previousVersionTag = $matches[1]
        Write-Debug "Previous version tag: $previousVersionTag"
    }
    else {
        Write-Error "No previous version tag found."
    }

    return (git rev-list --count "$previousVersionTag..HEAD") -as [int]
}

function Get-BuildId {
    $buildId = Get-BuildNumber

    # Get current branch name
    $branchName = (git rev-parse --abbrev-ref HEAD).Trim()
    Write-Debug "Current branch name: $branchName"

    # If the branch is not 'master', add the branch name to the build ID
    if ($branchName -ne "master") {
        $buildId = "$buildId.$branchName"
    }

    return $buildId
}

function Get-BuildVersion {
    $semVer = Get-SemVer
    $buildId = Get-BuildId
    $commitSha = (git rev-parse HEAD).Trim()

    return "$semVer+$($buildId).$($commitSha)"
}