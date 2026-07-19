using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUI3_Senior_Gallery.Models;

/// <summary>Local, deterministic card used by the Gallery home and experiences pages.</summary>
public sealed class GalleryFeatureCard
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Route { get; init; }
    public required string Status { get; init; }
    public required string Category { get; init; }
    public string? Glyph { get; init; }
    public bool IsInteractive => !string.IsNullOrWhiteSpace(Route);
}

/// <summary>Small dashboard widget model that deliberately has no network/provider dependency.</summary>
public sealed class DashboardWidget : INotifyPropertyChanged
{
    private bool _isPinned = true;
    private bool _isExpanded = true;
    private int _refreshCount;

    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string Value { get; init; }
    public required string Accent { get; init; }
    public bool IsPinned
    {
        get => _isPinned;
        set => SetField(ref _isPinned, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetField(ref _isExpanded, value);
    }

    public int RefreshCount
    {
        get => _refreshCount;
        private set => SetField(ref _refreshCount, value);
    }

    public string RefreshDescription => $"本地刷新 {RefreshCount} 次";

    public void Refresh()
    {
        RefreshCount++;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RefreshDescription)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class PlaygroundScenario
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Contract { get; init; }
    public required string Status { get; init; }
    public bool IsReady => string.Equals(Status, "ready", StringComparison.OrdinalIgnoreCase);
}

public sealed class ExperienceDefinition
{
    public required string Name { get; init; }
    public required string Intent { get; init; }
    public required string Composition { get; init; }
    public required string Status { get; init; }
}

public sealed class GalleryPreference : INotifyPropertyChanged
{
    private bool _reducedMotion;
    private bool _showDiagnostics = true;
    private bool _rememberLastRoute = true;
    private string _density = "Default";
    private string _language = "中文（规范源）";

    public bool ReducedMotion { get => _reducedMotion; set => SetField(ref _reducedMotion, value); }
    public bool ShowDiagnostics { get => _showDiagnostics; set => SetField(ref _showDiagnostics, value); }
    public bool RememberLastRoute { get => _rememberLastRoute; set => SetField(ref _rememberLastRoute, value); }
    public string Density { get => _density; set => SetField(ref _density, value); }
    public string Language { get => _language; set => SetField(ref _language, value); }

    public GalleryPreference Clone() => new()
    {
        ReducedMotion = ReducedMotion,
        ShowDiagnostics = ShowDiagnostics,
        RememberLastRoute = RememberLastRoute,
        Density = Density,
        Language = Language,
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>Factory for all Gallery-only deterministic data. No cloud or file system access is performed.</summary>
public static class GalleryData
{
    public static ObservableCollection<GalleryFeatureCard> FeatureCards { get; } =
    [
        new() { Title = "CarouselView", Description = "虚拟化轮播、循环索引、转场与多输入实验台。", Route = "controls/carousel-view", Status = "in-progress · P0", Category = "Controls", Glyph = "Switch" },
        new() { Title = "AchievementToast", Description = "窗口本地成就队列、进度、超时和宿主销毁语义。", Route = "controls/achievement-toast", Status = "in-progress · P2", Category = "Controls", Glyph = "Like" },
        new() { Title = "OverlayMenu", Description = "模态/非模态层级菜单、遮罩和 Esc/Back 导航。", Route = "controls/overlay-menu", Status = "in-progress · P2", Category = "Controls", Glyph = "More" },
        new() { Title = "BigTitle", Description = "滚动感知的大标题、紧凑标题和主题字体插值。", Route = "controls/big-title", Status = "in-progress · P2", Category = "Controls", Glyph = "Font" },
        new() { Title = "ContentRail", Description = "横向内容轨道、相邻预览、分页移动和 See All 宿主命令。", Route = "controls/content-rail", Status = "in-progress · P1", Category = "Controls", Glyph = "View" },
        new() { Title = "MediaPlayerChrome", Description = "宿主会话驱动的播放控制与本地媒体调试。", Route = "media/media-player-chrome", Status = "in-progress · P0", Category = "Media", Glyph = "Play" },
        new() { Title = "MediaTimeline", Description = "VOD、Live DVR、缓冲、章节和禁用区间。", Route = "media/media-timeline", Status = "in-progress · P0", Category = "Media", Glyph = "Clock" },
        new() { Title = "TimedTextView", Description = "单行字幕、歌词、Karaoke 与双语模型。", Route = "media/timed-text-view", Status = "in-progress · P0", Category = "Media", Glyph = "Font" },
        new() { Title = "MediaCenterGrid", Description = "十英尺海报墙、方向焦点和手柄导航。", Route = "media/media-center-grid", Status = "in-progress · P2", Category = "Media", Glyph = "View" },
        new() { Title = "Media Workbench", Description = "在同一虚拟时钟下串联播放控制、时间轴和字幕实验。", Route = "media/workbench", Status = "local lab", Category = "Media", Glyph = "Play" },
        new() { Title = "Focus Session", Description = "专注、短休息、长休息与本地任务状态机。", Route = "experiences/focus-session", Status = "in-progress · P2", Category = "Experiences", Glyph = "Clock" },
        new() { Title = "TabbedShell", Description = "标签生命周期、重排、关闭守卫与拖出请求。", Route = "experiences/tabbed-shell", Status = "in-progress · P1", Category = "Experiences", Glyph = "AllApps" },
        new() { Title = "Quick Resume", Description = "最近会话、十英尺导航和宿主恢复请求。", Route = "experiences/quick-resume", Status = "in-progress · P2", Category = "Experiences", Glyph = "Play" },
        new() { Title = "Immersive Reader", Description = "阅读聚焦、文本缩放和宿主朗读请求。", Route = "experiences/immersive-reader", Status = "in-progress · P2", Category = "Experiences", Glyph = "Document" },
        new() { Title = "EditorCanvas", Description = "文档对象、缩放、平移、选择、吸附、撤销与压感笔画。", Route = "experiences/editor-canvas", Status = "in-progress · P2", Category = "Experiences", Glyph = "Edit" },
        new() { Title = "HubPanorama", Description = "Metro 横向章节、视口滚动、RTL 方向与循环选择。", Route = "experiences/hub-panorama", Status = "in-progress · P1", Category = "Experiences", Glyph = "View" },
        new() { Title = "PeopleCard", Description = "联系人摘要字段、状态和宿主动作事件。", Route = "experiences/people-card", Status = "in-progress · P2", Category = "Experiences", Glyph = "Contact" },
        new() { Title = "FileCard", Description = "文件预览元数据、共享状态和宿主动作请求。", Route = "experiences/file-card", Status = "in-progress · P2", Category = "Experiences", Glyph = "Document" },
        new() { Title = "Mixview", Description = "Radial related-content graph with host-owned node selection and live announcements.", Route = "experiences/mixview", Status = "in-progress · P2", Category = "Experiences", Glyph = "View" },
        new() { Title = "GuideMenu", Description = "Layered guide navigation with breadcrumb state and host-owned leaf actions.", Route = "experiences/guide-menu", Status = "in-progress · P2", Category = "Experiences", Glyph = "More" },
        new() { Title = "DetachablePlayerHost", Description = "Inline/detached player lifecycle with explicit host-owned floating requests.", Route = "experiences/detachable-player-host", Status = "in-progress · P1", Category = "Experiences", Glyph = "Play" },
        new() { Title = "GameBarWidget", Description = "Floating widget interaction mode with explicit recovery hotkey and host acknowledgement.", Route = "experiences/game-bar-widget", Status = "in-progress · P2", Category = "Experiences", Glyph = "Play" },
        new() { Title = "MediaCenterExperience", Description = "Ten-foot category browse, details overlay, and host-owned playback intent.", Route = "experiences/media-center", Status = "in-progress · P1", Category = "Experiences", Glyph = "View" },
        new() { Title = "CaptionsTranslation", Description = "Host-provided caption revisions, translation fallback, and TimedTextView composition.", Route = "experiences/captions-translation", Status = "in-progress · P2", Category = "Experiences", Glyph = "Font" },
        new() { Title = "Windowing Lab", Description = "无边框窗口、浮动小组件和停靠行为的实验入口。", Route = "windowing", Status = "backlog · specified", Category = "Windowing", Glyph = "View" },
        new() { Title = "Design Archaeology", Description = "从 Metro、Zune、Media Center 到 Fluent 的现代化重制。", Route = "archaeology", Status = "research catalog", Category = "Archaeology", Glyph = "Document" },
    ];

    public static ObservableCollection<DashboardWidget> Widgets { get; } =
    [
        new() { Id = "progress", Title = "P0 progress", Summary = "Ready for implementation", Value = "4 active", Accent = "#4CC2FF" },
        new() { Id = "catalog", Title = "Catalog units", Summary = "Specification work items", Value = "125", Accent = "#8A6CFF" },
        new() { Id = "runtime", Title = "Debug runtime", Summary = "Unpackaged Windows App SDK", Value = "x64", Accent = "#45D483" },
        new() { Id = "quality", Title = "Quality gate", Summary = "Tests intentionally deferred", Value = "paused", Accent = "#FFB84D" },
    ];

    public static ObservableCollection<PlaygroundScenario> Scenarios { get; } =
    [
        new() { Name = "Carousel / 1000 items", Description = "观察有限实现窗口、循环映射和用户输入。", Contract = "controls.carousel-view", Status = "ready" },
        new() { Name = "Media / fake VOD", Description = "使用确定性会话驱动播放状态，不读取本地文件。", Contract = "contracts.media-playback", Status = "in-progress" },
        new() { Name = "Timed text / revision", Description = "模拟字幕修订和当前段落焦点锚定。", Contract = "media.timed-text-view", Status = "in-progress" },
        new() { Name = "Focus / phase transitions", Description = "使用短虚拟时钟观察 Focus、ShortBreak、LongBreak 和任务完成。", Contract = "experiences.focus-session", Status = "in-progress" },
        new() { Name = "Windowing / host requests", Description = "仅观察请求模型，不创建或迁移窗口。", Contract = "windowing.presentation", Status = "specified" },
    ];

    public static ObservableCollection<ExperienceDefinition> Experiences { get; } =
    [
        new() { Name = "HomeScreen", Intent = "开始屏式自适应入口与最近内容", Composition = "AdaptiveGrid + WidgetCard + DynamicTile", Status = "specified" },
        new() { Name = "ImmersiveNowPlaying", Intent = "沉浸式专辑背景、歌词与播放控制", Composition = "MediaPlayerChrome + TimedTextView", Status = "in-progress" },
        new() { Name = "WidgetsBoard", Intent = "可折叠、可拖拽的 Dashboard 卡片", Composition = "WidgetCard + Windowing host", Status = "backlog" },
        new() { Name = "TabbedShell", Intent = "标签页容器与 tear-out 请求", Composition = "TabbedShell + TabTearOutBehavior", Status = "in-progress" },
        new() { Name = "QuickResumeExperience", Intent = "最近会话选择与恢复请求", Composition = "QuickResumePicker + host-owned restore", Status = "in-progress" },
        new() { Name = "ImmersiveReader", Intent = "行聚焦、文本缩放和朗读请求", Composition = "ImmersiveReader + host-owned speech", Status = "in-progress" },
        new() { Name = "FocusSession", Intent = "番茄钟、任务和专注状态", Composition = "FocusSession + host-owned task list", Status = "in-progress" },
        new() { Name = "EditorCanvas", Intent = "无限画布、缩放、平移、选择和撤销边界", Composition = "CanvasDocumentController + EditorCanvas", Status = "in-progress" },
        new() { Name = "PeopleCard", Intent = "联系人摘要和宿主动作", Composition = "PeopleCard + host-owned contact model", Status = "in-progress" },
        new() { Name = "FileCard", Intent = "文件元数据和预览请求", Composition = "FileCard + host-owned file descriptor", Status = "in-progress" },
        new() { Name = "Mixview", Intent = "Zune-era related-content radial graph with explicit host-owned selection", Composition = "MixviewExperience + host-owned node graph", Status = "in-progress" },
        new() { Name = "GuideMenu", Intent = "Layered guide navigation and host-owned leaf commands", Composition = "GuideMenuExperience + host-owned command handlers", Status = "in-progress" },
        new() { Name = "DetachablePlayerHost", Intent = "Player content lifecycle across inline and host-owned floating presentation", Composition = "DetachablePlayerHost + IFloatingWidgetHost", Status = "in-progress" },
        new() { Name = "GameBarWidget", Intent = "Floating widget interaction mode and recovery boundary", Composition = "GameBarWidgetExperience + IFloatingWidgetHost", Status = "in-progress" },
        new() { Name = "MediaCenterExperience", Intent = "Ten-foot media browse and detail-to-playback intent", Composition = "MediaCenterExperience + MediaCenterGrid", Status = "in-progress" },
        new() { Name = "CaptionsTranslationExperience", Intent = "Host-provided caption/translation revisions with monotonic ordering", Composition = "CaptionsTranslationExperience + TimedTextView", Status = "in-progress" },
        new() { Name = "CaptionsTranslationExperience", Intent = "ASR 与翻译的双语字幕工作流", Composition = "TimedTextView + provider abstractions", Status = "future" },
    ];

    public static GalleryPreference Preferences { get; } = new();
}
