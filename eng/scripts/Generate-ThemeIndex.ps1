[CmdletBinding()]
param([string]$RepositoryRoot)

if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}

$themeFiles = Get-ChildItem (Join-Path $RepositoryRoot 'src') -Filter '*.xaml' -File -Recurse |
    Where-Object { $_.FullName -match '[\\/]Themes[\\/]' -and $_.Name -ne 'Generic.xaml' }

$duplicates = $themeFiles | Group-Object Name | Where-Object Count -gt 1
if ($duplicates) {
    $names = ($duplicates.Name -join ', ')
    throw "Duplicate theme dictionary file names: $names"
}
Write-Host "Theme dictionaries discovered: $($themeFiles.Count). Aggregation will be enabled when component implementation begins."
