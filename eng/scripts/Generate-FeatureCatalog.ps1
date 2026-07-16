[CmdletBinding()]
param(
    [string]$RepositoryRoot,
    [string]$OutputPath
)

if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
if (-not $OutputPath) {
    $OutputPath = Join-Path $RepositoryRoot 'docs\roadmap\FEATURE-CATALOG.md'
}

$items = Get-ChildItem (Join-Path $RepositoryRoot 'catalog') -Filter feature.json -File -Recurse |
    ForEach-Object { Get-Content $_.FullName -Raw | ConvertFrom-Json } |
    Sort-Object priority, kind, display_name

$lines = @('# Feature Catalog', '', '> Generated from `catalog/**/feature.json`. Do not edit manually.', '', '| Priority | Kind | Feature | Status | Package | Route |', '|---|---|---|---|---|---|')
foreach ($item in $items) {
    $lines += "| $($item.priority) | $($item.kind) | $($item.display_name) | $($item.status) | $($item.package) | $($item.gallery_route) |"
}
Set-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8
Write-Host "Generated $OutputPath"
