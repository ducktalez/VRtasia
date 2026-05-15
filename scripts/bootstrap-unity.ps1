<#
.SYNOPSIS
    Re-creates the VRtasia Unity project at the canonical path using the pinned Unity Editor version.

.DESCRIPTION
    Intended for disaster recovery or clean resets. Day-to-day developers should NOT run this —
    just clone the repo and open `unity/VRtasia/` via Unity Hub.

.PARAMETER Force
    Overwrite a non-empty target directory.

.EXAMPLE
    ./scripts/bootstrap-unity.ps1

.EXAMPLE
    $env:UNITY_EDITOR_PATH = 'D:\Unity\6000.4.7f1\Editor\Unity.exe'
    ./scripts/bootstrap-unity.ps1 -Force
#>
[CmdletBinding()]
param(
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$PinnedUnityVersion = '6000.4.7f1'
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$DefaultEditor = "C:\Program Files\Unity\Hub\Editor\$PinnedUnityVersion\Editor\Unity.exe"

$EditorPath = if ($env:UNITY_EDITOR_PATH) { $env:UNITY_EDITOR_PATH } else { $DefaultEditor }
$TargetDir  = if ($env:VRTASIA_UNITY_DIR) { $env:VRTASIA_UNITY_DIR } else { Join-Path $RepoRoot 'unity\VRtasia' }

Write-Host "Repo root   : $RepoRoot"
Write-Host "Editor      : $EditorPath"
Write-Host "Target dir  : $TargetDir"
Write-Host "Pinned vers.: $PinnedUnityVersion"
Write-Host ''

if (-not (Test-Path -LiteralPath $EditorPath)) {
    throw "Unity Editor not found at '$EditorPath'. Install Unity $PinnedUnityVersion via Unity Hub or set `$env:UNITY_EDITOR_PATH."
}

if (Test-Path -LiteralPath $TargetDir) {
    $existing = Get-ChildItem -Force -LiteralPath $TargetDir
    if ($existing.Count -gt 0 -and -not $Force) {
        throw "Target '$TargetDir' is not empty. Re-run with -Force to overwrite (DESTROYS existing project)."
    }
    if ($Force) {
        Write-Warning "Removing existing target '$TargetDir'..."
        Remove-Item -Recurse -Force -LiteralPath $TargetDir
    }
}

New-Item -ItemType Directory -Path (Split-Path -Parent $TargetDir) -Force | Out-Null

Write-Host "Invoking Unity headlessly... (this can take several minutes)"
$args = @('-batchmode', '-nographics', '-createProject', $TargetDir, '-quit', '-logFile', '-')
& $EditorPath @args
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    throw "Unity exited with code $exitCode."
}

Write-Host ''
Write-Host "Done. Plain 3D project scaffolded at '$TargetDir'." -ForegroundColor Green
Write-Host 'NOTE: The committed project uses URP. To convert this scaffold:'
Write-Host '      Window > Package Manager > Universal RP > Install,'
Write-Host '      then Assets > Create > Rendering > URP Asset.'
Write-Host 'Next: see INSTALLATION.md section 5 (XR Setup pass).'

