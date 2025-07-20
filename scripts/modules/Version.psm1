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

    try {
        $gitOutput = git rev-list --count "$previousVersionTag..HEAD"
        if (-not $gitOutput) {
            throw "Git command returned no output."
        }
        return $gitOutput -as [int]
    }
    catch {
        Write-Error "Failed to retrieve build number using git: $_"
        return 0
    }
}

function Get-BuildId {
    $buildId = Get-BuildNumber

    # Get current branch name
    try {
        $branchName = (git rev-parse --abbrev-ref HEAD).Trim()
        if (-not $branchName) {
            throw "Failed to retrieve branch name. Repository might be in a detached HEAD state."
        }
        Write-Debug "Current branch name: $branchName"
    }
    catch {
        Write-Error "Error retrieving branch name: $_"
        throw
    }

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