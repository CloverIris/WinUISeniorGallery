[CmdletBinding()]
param([string]$RepositoryRoot)

$ErrorActionPreference = 'Stop'
if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
$catalogRoot = Join-Path $RepositoryRoot 'catalog'
$requiredDocs = @('README', 'SPEC', 'DESIGN', 'INTEGRATION', 'ACCEPTANCE')
$validKinds = @('foundation', 'control', 'behavior', 'service', 'experience', 'archaeology', 'page')
$validStatuses = @('proposed', 'specified', 'ready', 'in-progress', 'blocked', 'review', 'done')
$validMaturities = @('lab', 'preview', 'stable')
$validPriorities = @('P0', 'P1', 'P2', 'P3')
$validLicenseStates = @('not-required', 'pending', 'passed', 'blocked')
$allowedProperties = @('schema_version','id','display_name','kind','status','maturity','priority','package','namespace','gallery_route','source_language','owned_paths','depends_on','provides','archaeology_refs','historical_label','public_api_name','license_review')
$errors = [System.Collections.Generic.List[string]]::new()

if (-not (Test-Path -LiteralPath $catalogRoot)) {
    throw "Catalog root not found: $catalogRoot"
}

$items = @()
foreach ($manifestFile in Get-ChildItem -LiteralPath $catalogRoot -Filter feature.json -File -Recurse) {
    try {
        $manifest = Get-Content -LiteralPath $manifestFile.FullName -Raw | ConvertFrom-Json
    } catch {
        $errors.Add("Invalid JSON: $($manifestFile.FullName): $($_.Exception.Message)")
        continue
    }

    $required = @('schema_version','id','display_name','kind','status','maturity','priority','package','namespace','gallery_route','source_language','owned_paths','depends_on','provides','archaeology_refs','license_review')
    foreach ($name in $required) {
        if ($null -eq $manifest.PSObject.Properties[$name]) {
            $errors.Add("Missing '$name': $($manifestFile.FullName)")
        }
    }
    foreach ($name in $manifest.PSObject.Properties.Name) {
        if ($name -notin $allowedProperties) { $errors.Add("Unknown property '$name' in $($manifestFile.FullName)") }
    }
    foreach ($name in @('owned_paths','depends_on','provides','archaeology_refs')) {
        if ($null -ne $manifest.$name -and $manifest.$name -isnot [System.Array]) {
            $errors.Add("'$name' must be a JSON array in $($manifestFile.FullName)")
        }
    }

    if ($manifest.schema_version -ne 1) { $errors.Add("Unsupported schema_version in $($manifestFile.FullName)") }
    if ($manifest.id -notmatch '^[a-z0-9]+(?:[.-][a-z0-9]+)*$') { $errors.Add("Invalid id '$($manifest.id)'") }
    if ($manifest.kind -notin $validKinds) { $errors.Add("Invalid kind '$($manifest.kind)' in $($manifest.id)") }
    if ($manifest.status -notin $validStatuses) { $errors.Add("Invalid status '$($manifest.status)' in $($manifest.id)") }
    if ($manifest.maturity -notin $validMaturities) { $errors.Add("Invalid maturity '$($manifest.maturity)' in $($manifest.id)") }
    if ($manifest.priority -notin $validPriorities) { $errors.Add("Invalid priority '$($manifest.priority)' in $($manifest.id)") }
    if ($manifest.license_review -notin $validLicenseStates) { $errors.Add("Invalid license_review '$($manifest.license_review)' in $($manifest.id)") }
    if ($manifest.source_language -ne 'zh-CN') { $errors.Add("source_language must be zh-CN in $($manifest.id)") }
    if ($manifest.gallery_route -and $manifest.gallery_route -notmatch '^/') { $errors.Add("gallery_route must start with / in $($manifest.id)") }

    foreach ($doc in $requiredDocs) {
        $zh = Join-Path $manifestFile.DirectoryName "$doc.zh-CN.md"
        $en = Join-Path $manifestFile.DirectoryName "$doc.en-US.md"
        if (-not (Test-Path -LiteralPath $zh)) { $errors.Add("Missing $doc.zh-CN.md for $($manifest.id)") }
        if (-not (Test-Path -LiteralPath $en)) { $errors.Add("Missing $doc.en-US.md for $($manifest.id)") }
    }
    if ($manifest.kind -eq 'archaeology') {
        foreach ($language in @('zh-CN','en-US')) {
            if (-not (Test-Path -LiteralPath (Join-Path $manifestFile.DirectoryName "SOURCES.$language.md"))) {
                $errors.Add("Missing SOURCES.$language.md for $($manifest.id)")
            }
        }
    }

    $items += [pscustomobject]@{ File = $manifestFile.FullName; Manifest = $manifest }
}

foreach ($group in $items | Group-Object { $_.Manifest.id } | Where-Object Count -gt 1) {
    $errors.Add("Duplicate id '$($group.Name)'")
}
foreach ($group in $items | Where-Object { $_.Manifest.gallery_route } | Group-Object { $_.Manifest.gallery_route } | Where-Object Count -gt 1) {
    $errors.Add("Duplicate gallery_route '$($group.Name)'")
}
foreach ($group in $items | ForEach-Object { $item = $_; @($item.Manifest.provides) | ForEach-Object { [pscustomobject]@{ Value = $_; Item = $item.Manifest.id } } } | Group-Object Value | Where-Object Count -gt 1) {
    $errors.Add("Duplicate provided contract '$($group.Name)'")
}
foreach ($group in $items | ForEach-Object { $item = $_; @($item.Manifest.owned_paths) | ForEach-Object { [pscustomobject]@{ Value = $_; Item = $item.Manifest.id } } } | Group-Object Value | Where-Object Count -gt 1) {
    $errors.Add("Duplicate owned_path '$($group.Name)'")
}

$knownIds = @{}; foreach ($item in $items) { $knownIds[$item.Manifest.id] = $true }
foreach ($item in $items) {
    foreach ($dependency in @($item.Manifest.depends_on)) {
        if ($dependency -match '^(controls|media|windowing|experiences|pages|archaeology|foundation|future)\.' -and -not $knownIds.ContainsKey($dependency)) {
            $errors.Add("Unknown catalog dependency '$dependency' in $($item.Manifest.id)")
        }
    }
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ }
    throw "Catalog validation failed with $($errors.Count) error(s)."
}

Write-Host "Catalog validation passed: $($items.Count) work item(s)."
