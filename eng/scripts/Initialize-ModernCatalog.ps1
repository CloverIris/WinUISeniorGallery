[CmdletBinding()]
param([string] $RepositoryRoot)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    $RepositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
}

$records = @'
Foundation/ThemeSystem|foundation.theme-system|ThemeSystem|foundation|P0|WinUI3.Senior.Core||contracts.theme
Foundation/MotionSystem|foundation.motion-system|MotionSystem|foundation|P0|WinUI3.Senior.Core||contracts.motion
Foundation/InputSystem|foundation.input-system|InputSystem|foundation|P0|WinUI3.Senior.Core||contracts.input
Foundation/AccessibilitySystem|foundation.accessibility-system|AccessibilitySystem|foundation|P0|WinUI3.Senior.Core||contracts.accessibility
Foundation/NavigationCatalog|foundation.navigation-catalog|NavigationCatalog|service|P1|WinUI3.Senior.Core||contracts.navigation
Foundation/LocalizationService|foundation.localization-service|LocalizationService|service|P1|WinUI3.Senior.Core||contracts.localization
Foundation/ResourceCatalog|foundation.resource-catalog|ResourceCatalog|service|P1|WinUI3.Senior.Core||contracts.resources
Controls/Navigation/PivotView|controls.pivot-view|PivotView|control|P1|WinUI3.Senior.Controls|/controls/pivot-view|foundation.input-system;foundation.motion-system
Controls/Navigation/SemanticZoomView|controls.semantic-zoom-view|SemanticZoomView|control|P1|WinUI3.Senior.Controls|/controls/semantic-zoom-view|foundation.input-system
Controls/Navigation/BreadcrumbBarEx|controls.breadcrumb-bar-ex|BreadcrumbBarEx|control|P2|WinUI3.Senior.Controls|/controls/breadcrumb-bar-ex|foundation.navigation-catalog
Controls/Input/SearchBoxEx|controls.search-box-ex|SearchBoxEx|control|P2|WinUI3.Senior.Controls|/controls/search-box-ex|foundation.accessibility-system
Controls/Navigation/WizardStepper|controls.wizard-stepper|WizardStepper|control|P2|WinUI3.Senior.Controls|/controls/wizard-stepper|foundation.navigation-catalog
Controls/Layout/AdaptiveGrid|controls.adaptive-grid|AdaptiveGrid|control|P1|WinUI3.Senior.Controls|/controls/adaptive-grid|foundation.accessibility-system
Controls/Collections/ContentRail|controls.content-rail|ContentRail|control|P1|WinUI3.Senior.Controls|/controls/content-rail|foundation.input-system
Controls/Layout/WidgetCard|controls.widget-card|WidgetCard|control|P1|WinUI3.Senior.Controls|/controls/widget-card|foundation.theme-system
Controls/Collections/DynamicTile|controls.dynamic-tile|DynamicTile|control|P1|WinUI3.Senior.Controls|/controls/dynamic-tile|foundation.motion-system
Controls/Layout/BigTitle|controls.big-title|BigTitle|control|P2|WinUI3.Senior.Controls|/controls/big-title|foundation.theme-system
Controls/Overlays/DragOverlay|controls.drag-overlay|DragOverlay|control|P2|WinUI3.Senior.Controls|/controls/drag-overlay|foundation.input-system
Controls/Overlays/AchievementToast|controls.achievement-toast|AchievementToast|control|P2|WinUI3.Senior.Controls|/controls/achievement-toast|foundation.motion-system
Controls/Overlays/OverlayMenu|controls.overlay-menu|OverlayMenu|control|P2|WinUI3.Senior.Controls|/controls/overlay-menu|foundation.accessibility-system
Controls/Commands/ExpandableCommandBar|controls.expandable-command-bar|ExpandableCommandBar|control|P2|WinUI3.Senior.Controls|/controls/expandable-command-bar|foundation.input-system
Controls/Commands/CommandRibbon|controls.command-ribbon|CommandRibbon|control|P2|WinUI3.Senior.Controls|/controls/command-ribbon|foundation.accessibility-system
Controls/Input/ColorPickerEx|controls.color-picker-ex|ColorPickerEx|control|P2|WinUI3.Senior.Controls|/controls/color-picker-ex|foundation.accessibility-system
Controls/Input/FontPicker|controls.font-picker|FontPicker|control|P2|WinUI3.Senior.Controls|/controls/font-picker|foundation.localization-service
Controls/Input/IconPicker|controls.icon-picker|IconPicker|control|P2|WinUI3.Senior.Controls|/controls/icon-picker|foundation.resource-catalog
Controls/Data/PropertyGrid|controls.property-grid|PropertyGrid|control|P2|WinUI3.Senior.Controls|/controls/property-grid|foundation.accessibility-system
Controls/Data/TreeDataGrid|controls.tree-data-grid|TreeDataGrid|control|P2|WinUI3.Senior.Controls|/controls/tree-data-grid|foundation.accessibility-system
Media/MiniPlayerHost|media.mini-player-host|MiniPlayerHost|control|P1|WinUI3.Senior.Media|/media/mini-player-host|media.media-player-chrome
Media/MediaCenterGrid|media.media-center-grid|MediaCenterGrid|control|P1|WinUI3.Senior.Media|/media/media-center-grid|controls.content-rail
Windowing/WindowChrome|windowing.window-chrome|WindowChrome|control|P1|WinUI3.Senior.Windowing|/windowing/window-chrome|contracts.windowing
Windowing/TitleBarHost|windowing.title-bar-host|TitleBarHost|control|P1|WinUI3.Senior.Windowing|/windowing/title-bar-host|contracts.windowing
Windowing/CompactOverlayHost|windowing.compact-overlay-host|CompactOverlayHost|service|P1|WinUI3.Senior.Windowing|/windowing/compact-overlay-host|contracts.windowing
Windowing/FloatingWidgetHost|windowing.floating-widget-host|FloatingWidgetHost|service|P1|WinUI3.Senior.Windowing|/windowing/floating-widget-host|contracts.windowing
Windowing/EdgeCommandPanel|windowing.edge-command-panel|EdgeCommandPanel|control|P2|WinUI3.Senior.Windowing|/windowing/edge-command-panel|contracts.windowing
Windowing/SettingsPanel|windowing.settings-panel|SettingsPanel|control|P2|WinUI3.Senior.Windowing|/windowing/settings-panel|contracts.windowing
Windowing/DockLayoutPreview|windowing.dock-layout-preview|DockLayoutPreview|control|P2|WinUI3.Senior.Windowing|/windowing/dock-layout-preview|contracts.windowing
Windowing/TabTearOutBehavior|windowing.tab-tear-out-behavior|TabTearOutBehavior|behavior|P2|WinUI3.Senior.Windowing|/windowing/tab-tear-out-behavior|contracts.windowing
Windowing/RevealBorderBehavior|windowing.reveal-border-behavior|RevealBorderBehavior|behavior|P2|WinUI3.Senior.Windowing|/windowing/reveal-border-behavior|contracts.motion
Windowing/ConnectedTransitionBehavior|windowing.connected-transition-behavior|ConnectedTransitionBehavior|behavior|P2|WinUI3.Senior.Windowing|/windowing/connected-transition-behavior|contracts.motion
Experiences/HomeScreen|experiences.home-screen|HomeScreen|experience|P1|WinUI3.Senior.Archaeology|/experiences/home-screen|controls.adaptive-grid;controls.widget-card
Experiences/HubPanorama|experiences.hub-panorama|HubPanorama|experience|P1|WinUI3.Senior.Archaeology|/experiences/hub-panorama|controls.pivot-view
Experiences/DetachablePlayerHost|experiences.detachable-player-host|DetachablePlayerHost|experience|P1|WinUI3.Senior.Archaeology|/experiences/detachable-player-host|media.media-player-chrome;windowing.compact-overlay-host
Experiences/ImmersiveNowPlaying|experiences.immersive-now-playing|ImmersiveNowPlaying|experience|P1|WinUI3.Senior.Archaeology|/experiences/immersive-now-playing|media.media-player-chrome;media.timed-text-view
Experiences/MediaCenterExperience|experiences.media-center-experience|MediaCenterExperience|experience|P1|WinUI3.Senior.Archaeology|/experiences/media-center-experience|media.media-center-grid
Experiences/DynamicTileBoard|experiences.dynamic-tile-board|DynamicTileBoard|experience|P1|WinUI3.Senior.Archaeology|/experiences/dynamic-tile-board|controls.dynamic-tile
Experiences/WidgetsBoard|experiences.widgets-board|WidgetsBoard|experience|P1|WinUI3.Senior.Archaeology|/experiences/widgets-board|controls.widget-card;controls.adaptive-grid
Experiences/TabbedShell|experiences.tabbed-shell|TabbedShell|experience|P1|WinUI3.Senior.Archaeology|/experiences/tabbed-shell|windowing.tab-tear-out-behavior
Experiences/FocusSession|experiences.focus-session|FocusSession|experience|P2|WinUI3.Senior.Archaeology|/experiences/focus-session|controls.snackbar-host
Experiences/GameBarWidgetExperience|experiences.game-bar-widget|GameBarWidgetExperience|experience|P2|WinUI3.Senior.Archaeology|/experiences/game-bar-widget|windowing.floating-widget-host
Experiences/ImmersiveReader|experiences.immersive-reader|ImmersiveReader|experience|P2|WinUI3.Senior.Archaeology|/experiences/immersive-reader|media.timed-text-view
Experiences/EditorCanvas|experiences.editor-canvas|EditorCanvas|experience|P2|WinUI3.Senior.Archaeology|/experiences/editor-canvas|future.canvas-winui
Experiences/CaptionsTranslationExperience|experiences.captions-translation|CaptionsTranslationExperience|experience|P2|WinUI3.Senior.Archaeology|/experiences/captions-translation|media.timed-text-view;future.captions-abstractions
Experiences/MixviewExperience|experiences.mixview|MixviewExperience|experience|P2|WinUI3.Senior.Archaeology|/experiences/mixview|foundation.motion-system
Experiences/QuickResumeExperience|experiences.quick-resume|QuickResumeExperience|experience|P2|WinUI3.Senior.Archaeology|/experiences/quick-resume|controls.content-rail
Experiences/GuideMenuExperience|experiences.guide-menu|GuideMenuExperience|experience|P2|WinUI3.Senior.Archaeology|/experiences/guide-menu|windowing.edge-command-panel
Experiences/PeopleCard|experiences.people-card|PeopleCard|experience|P2|WinUI3.Senior.Archaeology|/experiences/people-card|controls.overlay-menu
Experiences/FileCard|experiences.file-card|FileCard|experience|P2|WinUI3.Senior.Archaeology|/experiences/file-card|controls.widget-card
Experiences/Future/CaptionsAbstractions|future.captions-abstractions|Captions.Abstractions|service|P3|WinUI3.Senior.Core||contracts.timed-text
Experiences/Future/AsrProvider|future.asr-provider|ASR Provider|service|P3|WinUI3.Senior.Core||future.captions-abstractions
Experiences/Future/TranslationProvider|future.translation-provider|Translation Provider|service|P3|WinUI3.Senior.Core||future.captions-abstractions
Experiences/Future/CanvasAbstractions|future.canvas-abstractions|Canvas.Abstractions|service|P3|WinUI3.Senior.Core||foundation.input-system
Experiences/Future/CanvasWinUI|future.canvas-winui|Canvas.WinUI|control|P3|WinUI3.Senior.Controls||future.canvas-abstractions;future.canvas-native
Experiences/Future/CanvasNative|future.canvas-native|Canvas.Native|service|P3|WinUI3.Senior.Core||future.canvas-abstractions
'@

function Decode([string] $value) { [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($value)) }
$zhReadme = Decode '6L+Z5piv5Lit5paH6KeE6IyD5rqQ44CC5b2T5YmN5Li6IHByb3Bvc2VkIGJhY2tsb2fvvIzkuI3lhYHorrjov5vlhaXlrp7njrDjgII='
$zhSpec = Decode '55uu5qCH77ya5a6a5LmJ5Y+v5aSN55So6IGM6LSj44CB54q25oCB5ZKM6L6555WM44CC5q2j5byPIEFQSeOAgeaooeadv+mDqOS7tuOAgeWksei0peaooeW8j+WSjOaAp+iDvemihOeul+WwmuacqumUgeWumu+8m+WujOaIkOS4k+mhueivhOWuoeWQjuaWueWPr+i/m+WFpSByZWFkeeOAgg=='
$zhDesign = Decode '6K6+6K6h5b+F6aG76KaG55uW5ZON5bqU5byP5biD5bGA44CBTGlnaHQvRGFyay9IaWdoIENvbnRyYXN044CBUmVkdWNlZCBNb3Rpb27jgIHplK7nm5jjgIHpvKDmoIfjgIHop6bmkbjjgIFOYXJyYXRvcuOAgeaWh+acrOe8qeaUvuOAgeS4reaWh+OAgeiLseaWh+WSjCBSVEzjgILljoblj7LmnaXmupDnlLHlr7nlupTogIPlj6Tlt6XkvZzljZXlhYPnu7TmiqTvvIzkuI3lpI3liLblj5fniYjmnYPkv53miqTntKDmnZDjgII='
$zhIntegration = Decode '5LiN5b6X6YeN5aSN5a6a5LmJ5YWo5bGAIENvbnRyYWN0IOaIlui1hOa6kOmUruOAguW8guatpeaTjeS9nOW/hemhu+WPr+WPlua2iO+8m+Wuv+S4u+mUgOavgeWQjuS4jeW+l+e7p+e7reWbnuiwg+OAgum7mOiupOS4jeWjsOaYjumineWkluadg+mZkO+8jOS4jeaUtumbhumBpea1i+aIlueUqOaIt+WGheWuueOAgg=='
$zhAcceptance = Decode '6aqM5pS26Zeo56aB77yacHJvcG9zZWQg5bel5L2c5Y2V5YWD5LiN5b6X5a6e546w44CC5Lit6Iux5paH5qCH6aKY57uT5p6E44CB56iz5a6aIElEIOWSjCBBUEkg5ZCN56ew5b+F6aG75ZCM5q2l77ybQVBJ44CB54q25oCB44CB6ZSZ6K+v5ZKM5oCn6IO96aKE566X6ZSB5a6a5ZCO5omN5Y+v6L+b5YWlIHJlYWR544CC'

$count = 0
foreach ($line in ($records -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $parts = $line.Split('|')
    $path,$id,$name,$kind,$priority,$package,$route,$dependencyText = $parts
    [object[]] $dependencies = if ($dependencyText) { @($dependencyText.Split(';')) } else { @() }
    $directory = Join-Path (Join-Path $RepositoryRoot 'catalog') $path
    $manifestPath = Join-Path $directory 'feature.json'
    if (Test-Path -LiteralPath $manifestPath) { continue }
    New-Item -ItemType Directory -Force -Path $directory | Out-Null
    $ownedPath = 'catalog/' + $path
    [ordered]@{
        schema_version=1; id=$id; display_name=$name; kind=$kind; status='proposed'; maturity='lab'; priority=$priority
        package=$package; namespace=$package; gallery_route=$route; source_language='zh-CN'; owned_paths=@($ownedPath)
        depends_on=[object[]]$dependencies; provides=[object[]]@($kind + ':' + $id); archaeology_refs=[object[]]@(); license_review='not-required'
    } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding utf8

    $zhDocs = @{
        README = "# $name`n`n$zhReadme`n`n## Status`n`nproposed / lab / $priority`n`n## Documents`n`n- SPEC.zh-CN.md`n- DESIGN.zh-CN.md`n- INTEGRATION.zh-CN.md`n- ACCEPTANCE.zh-CN.md`n`n## Agent ownership`n`n$ownedPath"
        SPEC = "# $name Specification`n`n## Goal`n`n$zhSpec`n`n## Non-goals`n`nNo implementation while proposed.`n`n## Public API`n`nNot locked.`n`n## State model`n`nNot locked.`n`n## Template parts and visual tree`n`nNot locked.`n`n## Behavior and failure modes`n`nFollow referenced contracts.`n`n## Open Decisions`n`nAPI, template parts, defaults, and performance budgets require specification review."
        DESIGN = "# $name Design`n`n## Visuals and interaction`n`n$zhDesign`n`n## Responsiveness`n`nNarrow, regular, and wide layouts require review.`n`n## Theme, motion, input, and accessibility`n`nFollow global contracts.`n`n## Modernization tradeoffs`n`nArchaeology owns historical provenance."
        INTEGRATION = "# $name Integration`n`n## Dependencies`n`n$($dependencies -join ', ')`n`n## Global contracts and resources`n`n$zhIntegration`n`n## Platform APIs and capabilities`n`nNo extra capability by default.`n`n## Lifecycle and threading`n`nCancellation and host destruction must be handled."
        ACCEPTANCE = "# $name Acceptance`n`n## Current gate`n`n$zhAcceptance`n`n## Common matrix`n`nLight, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL."
    }
    $enDocs = @{
        README = "# $name`n`nSpecification work item for $name.`n`n## Status`n`nproposed / lab / $priority. Not eligible for implementation.`n`n## Documents`n`n- SPEC.en-US.md`n- DESIGN.en-US.md`n- INTEGRATION.en-US.md`n- ACCEPTANCE.en-US.md`n`n## Agent ownership`n`n$ownedPath"
        SPEC = "# $name Specification`n`n## Goal`n`nDefine reusable responsibilities, state, and boundaries.`n`n## Non-goals`n`nNo implementation while proposed.`n`n## Public API`n`nNot locked.`n`n## State model`n`nNot locked.`n`n## Template parts and visual tree`n`nNot locked.`n`n## Behavior and failure modes`n`nFollow referenced contracts.`n`n## Open Decisions`n`nAPI, template parts, defaults, and performance budgets require specification review."
        DESIGN = "# $name Design`n`n## Visuals and interaction`n`nUse modern Fluent semantics without copying protected assets.`n`n## Responsiveness`n`nNarrow, regular, and wide layouts require review.`n`n## Theme, motion, input, and accessibility`n`nFollow global contracts.`n`n## Modernization tradeoffs`n`nArchaeology owns historical provenance."
        INTEGRATION = "# $name Integration`n`n## Dependencies`n`n$($dependencies -join ', ')`n`n## Global contracts and resources`n`nDo not redefine global contracts or shared resource keys.`n`n## Platform APIs and capabilities`n`nNo extra capability by default.`n`n## Lifecycle and threading`n`nCancellation and host destruction must be handled."
        ACCEPTANCE = "# $name Acceptance`n`n## Current gate`n`nProposed work items must not be implemented. Chinese and English heading structures, stable IDs, and API names remain synchronized. APIs, state, errors, and performance budgets must be locked before ready.`n`n## Common matrix`n`nLight, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL."
    }
    foreach ($document in @('README','SPEC','DESIGN','INTEGRATION','ACCEPTANCE')) {
        Set-Content -LiteralPath (Join-Path $directory "$document.zh-CN.md") -Value $zhDocs[$document] -Encoding utf8
        Set-Content -LiteralPath (Join-Path $directory "$document.en-US.md") -Value $enDocs[$document] -Encoding utf8
    }
    $count++
}
Write-Host "Created $count modern catalog work items."
