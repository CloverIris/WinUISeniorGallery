using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WinUI3.Senior.Controls;

public sealed record FileCardAction(string Id, string Label, object? Icon = null, bool IsEnabled = true, object? Tag = null)
{
    public FileCardAction Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("An action id is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(Label)) throw new ArgumentException("An action label is required.", nameof(Label));
        return this;
    }
}

public sealed record FileCardDescriptor(
    string Id,
    string Name,
    string Kind,
    long Size = 0,
    DateTimeOffset? Modified = null,
    object? Thumbnail = null,
    string? Sharing = null,
    IReadOnlyList<FileCardAction>? Actions = null,
    object? Tag = null)
{
    public FileCardDescriptor Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("A file id is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("A file name is required.", nameof(Name));
        if (string.IsNullOrWhiteSpace(Kind)) throw new ArgumentException("A file kind is required.", nameof(Kind));
        return this with { Size = Math.Max(0, Size), Actions = (Actions ?? Array.Empty<FileCardAction>()).Where(action => action is not null).Select(action => action.Normalize()).GroupBy(action => action.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray() };
    }

    public string SizeText => Size switch { < 1024 => $"{Size} B", < 1024 * 1024 => $"{Size / 1024d:0.0} KB", < 1024L * 1024 * 1024 => $"{Size / 1024d / 1024d:0.0} MB", _ => $"{Size / 1024d / 1024d / 1024d:0.0} GB" };
}

public sealed class FileCardActionInvokedEventArgs(FileCardDescriptor file, FileCardAction action) : EventArgs
{
    public FileCardDescriptor File { get; } = file ?? throw new ArgumentNullException(nameof(file));
    public FileCardAction Action { get; } = action ?? throw new ArgumentNullException(nameof(action));
}

public sealed class FileCardPreviewRequestedEventArgs(FileCardDescriptor file) : EventArgs
{
    public FileCardDescriptor File { get; } = file ?? throw new ArgumentNullException(nameof(file));
    public bool Handled { get; set; }
}

[TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Thumbnail", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "PART_Name", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Metadata", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Sharing", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Actions", Type = typeof(ItemsControl))]
public sealed class FileCard : Control
{
    private FrameworkElement? _root;
    private ContentPresenter? _thumbnail;
    private TextBlock? _name;
    private TextBlock? _metadata;
    private TextBlock? _sharing;
    private ItemsControl? _actions;
    private readonly ObservableCollection<FileCardAction> _actionItems = new();

    public static readonly DependencyProperty FileProperty = DependencyProperty.Register(nameof(File), typeof(FileCardDescriptor), typeof(FileCard), new PropertyMetadata(null, OnFileChanged));
    public static readonly DependencyProperty IsPreviewEnabledProperty = DependencyProperty.Register(nameof(IsPreviewEnabled), typeof(bool), typeof(FileCard), new PropertyMetadata(true));
    public static readonly DependencyProperty IsActionsVisibleProperty = DependencyProperty.Register(nameof(IsActionsVisible), typeof(bool), typeof(FileCard), new PropertyMetadata(true, OnVisualPropertyChanged));

    public FileCard() { DefaultStyleKey = typeof(FileCard); }
    public FileCardDescriptor? File { get => (FileCardDescriptor?)GetValue(FileProperty); set => SetValue(FileProperty, value); }
    public bool IsPreviewEnabled { get => (bool)GetValue(IsPreviewEnabledProperty); set => SetValue(IsPreviewEnabledProperty, value); }
    public bool IsActionsVisible { get => (bool)GetValue(IsActionsVisibleProperty); set => SetValue(IsActionsVisibleProperty, value); }
    public event EventHandler<FileCardActionInvokedEventArgs>? ActionInvoked;
    public event EventHandler<FileCardPreviewRequestedEventArgs>? PreviewRequested;

    public bool RequestPreview()
    {
        if (!IsPreviewEnabled || File is not { } file) return false;
        var args = new FileCardPreviewRequestedEventArgs(file);
        PreviewRequested?.Invoke(this, args);
        return args.Handled;
    }

    public bool InvokeAction(string id)
    {
        var file = File;
        var action = file?.Actions?.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
        if (file is null || action is null || !action.IsEnabled) return false;
        ActionInvoked?.Invoke(this, new FileCardActionInvokedEventArgs(file, action));
        return true;
    }

    protected override void OnApplyTemplate()
    {
        if (_actions is not null) _actions.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnActionClick));
        base.OnApplyTemplate();
        _root = GetTemplateChild("PART_Root") as FrameworkElement;
        _thumbnail = GetTemplateChild("PART_Thumbnail") as ContentPresenter;
        _name = GetTemplateChild("PART_Name") as TextBlock;
        _metadata = GetTemplateChild("PART_Metadata") as TextBlock;
        _sharing = GetTemplateChild("PART_Sharing") as TextBlock;
        _actions = GetTemplateChild("PART_Actions") as ItemsControl;
        if (_actions is not null) _actions.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnActionClick), true);
        UpdateTemplate();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);
    private static void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FileCard)d).UpdateTemplate();
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FileCard)d).UpdateTemplate();
    private void OnActionClick(object sender, RoutedEventArgs e) { if (e.OriginalSource is Button { Tag: string id }) { InvokeAction(id); e.Handled = true; } }

    private void UpdateTemplate()
    {
        var file = File?.Normalize();
        if (_root is not null) AutomationProperties.SetName(_root, file is null ? "File card" : $"File {file.Name}");
        if (_thumbnail is not null) _thumbnail.Content = file?.Thumbnail;
        if (_name is not null) _name.Text = file?.Name ?? string.Empty;
        if (_metadata is not null) _metadata.Text = file is null ? string.Empty : $"{file.Kind} · {file.SizeText} · {file.Modified?.ToLocalTime().ToString("g") ?? "unknown date"}";
        if (_sharing is not null) { _sharing.Text = file?.Sharing ?? string.Empty; _sharing.Visibility = string.IsNullOrWhiteSpace(file?.Sharing) ? Visibility.Collapsed : Visibility.Visible; }
        _actionItems.Clear();
        foreach (var action in file?.Actions ?? Array.Empty<FileCardAction>()) _actionItems.Add(action);
        if (_actions is not null) { _actions.ItemsSource = _actionItems; _actions.Visibility = IsActionsVisible && _actionItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
    }
}
