function Get-VersionPrefix {
    # Retrieve the version prefix from Directory.Build.props
    if ((Get-Content -Path "$PSScriptRoot\..\..\Directory.Build.props" -Raw) -match '<VersionPrefix>(.*?)</VersionPrefix>') {
        $versionPrefix = $matches[1]
        Write-Debug "Version prefix: $versionPrefix"
    }
    else {
        Write-Error "No version prefix found."
    }

    return $versionPrefix
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

    # If the branch name is 'HEAD', use the name 'PullRequest'
    if ($branchName -eq "HEAD") {
        $branchName = "PullRequest"
    }

    # If the branch is not 'master', add the branch name to the build ID
    if ($branchName -ne "master") {
        
        $buildId = "$buildId.$branchName"
    }

    return $buildId
}

function Get-BuildVersion {
    $versionPrefix = Get-VersionPrefix
    $buildId = Get-BuildId
    try {
        $commitSha = (git rev-parse HEAD).Trim()
    }
    catch {
        Write-Error "Failed to retrieve the commit SHA. Ensure that 'git' is installed and this is a valid Git repository."
        return
    }

    return "$versionPrefix+$($buildId).$($commitSha)"
}

function Get-BuildFileVersion {
    $versionPrefix = Get-VersionPrefix
    $buildNumber = Get-BuildNumber

    return "$versionPrefix.$buildNumber"
}