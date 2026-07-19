using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace WinUI3.Senior.Media;

public enum QuickResumeInputDeviceKind
{
    Unknown,
    Mouse,
    Touch,
    Keyboard,
    GameController,
}

public sealed record QuickResumeEntry(
    string Id,
    string Title,
    TimeSpan Position,
    TimeSpan Duration,
    DateTimeOffset LastActive,
    object? Thumbnail = null,
    object? Tag = null)
{
    public double Progress => Duration <= TimeSpan.Zero ? 0 : Math.Clamp(Position.TotalMilliseconds / Duration.TotalMilliseconds, 0, 1);
    public bool CanResume => Duration > TimeSpan.Zero && Position < Duration;
}

public sealed class QuickResumeItemInvokedEventArgs(QuickResumeEntry entry, int index, QuickResumeInputDeviceKind inputDeviceKind) : EventArgs
{
    public QuickResumeEntry Entry { get; } = entry ?? throw new ArgumentNullException(nameof(entry));
    public int Index { get; } = index;
    public QuickResumeInputDeviceKind InputDeviceKind { get; } = inputDeviceKind;
}

public sealed class QuickResumeRemoveRequestedEventArgs(QuickResumeEntry entry) : EventArgs
{
    public QuickResumeEntry Entry { get; } = entry ?? throw new ArgumentNullException(nameof(entry));
    public bool Cancel { get; set; }
}

/// <summary>
/// Ten-foot friendly recent-session picker. It owns ordering and focus semantics only;
/// the host decides how a selected entry is restored and whether removal is persisted.
/// </summary>
[TemplatePart(Name = "PART_ListView", Type = typeof(ListView))]
public sealed class QuickResumePicker : ListView
{
    public static readonly DependencyProperty IsResumeActionEnabledProperty = DependencyProperty.Register(
        nameof(IsResumeActionEnabled), typeof(bool), typeof(QuickResumePicker), new PropertyMetadata(true));
    public static readonly DependencyProperty IsRemoveActionEnabledProperty = DependencyProperty.Register(
        nameof(IsRemoveActionEnabled), typeof(bool), typeof(QuickResumePicker), new PropertyMetadata(true));
    public static readonly DependencyProperty IsWrapNavigationEnabledProperty = DependencyProperty.Register(
        nameof(IsWrapNavigationEnabled), typeof(bool), typeof(QuickResumePicker), new PropertyMetadata(true));
    public static readonly DependencyProperty MaximumItemsProperty = DependencyProperty.Register(
        nameof(MaximumItems), typeof(int), typeof(QuickResumePicker), new PropertyMetadata(12, OnMaximumItemsChanged));

    public QuickResumePicker()
    {
        DefaultStyleKey = typeof(QuickResumePicker);
        SelectionMode = ListViewSelectionMode.Single;
        IsItemClickEnabled = true;
        ItemClick += OnItemClick;
        KeyDown += OnKeyDown;
    }

    public bool IsResumeActionEnabled { get => (bool)GetValue(IsResumeActionEnabledProperty); set => SetValue(IsResumeActionEnabledProperty, value); }
    public bool IsRemoveActionEnabled { get => (bool)GetValue(IsRemoveActionEnabledProperty); set => SetValue(IsRemoveActionEnabledProperty, value); }
    public bool IsWrapNavigationEnabled { get => (bool)GetValue(IsWrapNavigationEnabledProperty); set => SetValue(IsWrapNavigationEnabledProperty, value); }
    public int MaximumItems { get => (int)GetValue(MaximumItemsProperty); set => SetValue(MaximumItemsProperty, Math.Clamp(value, 1, 100)); }

    public QuickResumeEntry? SelectedEntry => SelectedItem as QuickResumeEntry;

    public event EventHandler<QuickResumeItemInvokedEventArgs>? ItemInvoked;
    public event EventHandler<QuickResumeRemoveRequestedEventArgs>? RemoveRequested;
    public event EventHandler? SelectionChangedByUser;

    public void SetEntries(IEnumerable<QuickResumeEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var selectedId = SelectedEntry?.Id;
        var normalized = entries
            .Where(entry => entry is not null && !string.IsNullOrWhiteSpace(entry.Id))
            .GroupBy(entry => entry.Id, StringComparer.Ordinal)
            .Select(group => group.OrderByDescending(entry => entry.LastActive).First())
            .OrderByDescending(entry => entry.LastActive)
            .Take(MaximumItems)
            .ToArray();
        Items.Clear();
        foreach (var entry in normalized) Items.Add(entry);
        if (Items.Count > 0)
        {
            var preserved = selectedId is null ? -1 : Items.OfType<QuickResumeEntry>().ToList().FindIndex(entry => string.Equals(entry.Id, selectedId, StringComparison.Ordinal));
            SelectedIndex = preserved >= 0 ? preserved : 0;
        }
    }

    public bool InvokeSelected(QuickResumeInputDeviceKind inputDeviceKind = QuickResumeInputDeviceKind.Keyboard)
    {
        if (!IsResumeActionEnabled || SelectedEntry is not { CanResume: true } entry) return false;
        ItemInvoked?.Invoke(this, new QuickResumeItemInvokedEventArgs(entry, SelectedIndex, inputDeviceKind));
        SelectionChangedByUser?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool RemoveSelected()
    {
        if (!IsRemoveActionEnabled || SelectedEntry is not { } entry) return false;
        var args = new QuickResumeRemoveRequestedEventArgs(entry);
        RemoveRequested?.Invoke(this, args);
        if (args.Cancel) return false;
        var index = SelectedIndex;
        Items.Remove(entry);
        if (Items.Count > 0) SelectedIndex = Math.Min(index, Items.Count - 1);
        return true;
    }

    public bool MoveFocus(int delta)
    {
        if (Items.Count == 0 || delta == 0) return false;
        var current = SelectedIndex < 0 ? 0 : SelectedIndex;
        var next = current + Math.Sign(delta);
        if (IsWrapNavigationEnabled) next = (next % Items.Count + Items.Count) % Items.Count;
        if (next < 0 || next >= Items.Count) return false;
        SelectedIndex = next;
        ScrollIntoView(SelectedItem, ScrollIntoViewAlignment.Default);
        return true;
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        if (SelectedIndex >= Items.Count) SelectedIndex = Items.Count - 1;
    }

    private static void OnMaximumItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var picker = (QuickResumePicker)sender;
        var maximum = Math.Clamp((int)args.NewValue, 1, 100);
        if ((int)args.NewValue != maximum) picker.SetValue(MaximumItemsProperty, maximum);
        if (picker.Items.Count <= maximum) return;
        var keep = picker.Items.OfType<QuickResumeEntry>()
            .OrderByDescending(entry => entry.LastActive)
            .Take(maximum)
            .ToHashSet();
        for (var index = picker.Items.Count - 1; index >= 0; index--)
        {
            if (picker.Items[index] is QuickResumeEntry entry && !keep.Contains(entry)) picker.Items.RemoveAt(index);
        }
    }

    private void OnItemClick(object sender, ItemClickEventArgs args)
    {
        if (args.ClickedItem is QuickResumeEntry entry && IsResumeActionEnabled && entry.CanResume)
        {
            ItemInvoked?.Invoke(this, new QuickResumeItemInvokedEventArgs(entry, Items.IndexOf(entry), QuickResumeInputDeviceKind.Unknown));
            SelectionChangedByUser?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        var direction = args.Key switch
        {
            VirtualKey.Left or VirtualKey.GamepadDPadLeft => FlowDirection == FlowDirection.RightToLeft ? 1 : -1,
            VirtualKey.Right or VirtualKey.GamepadDPadRight => FlowDirection == FlowDirection.RightToLeft ? -1 : 1,
            _ => 0,
        };
        if (direction != 0)
        {
            args.Handled = MoveFocus(direction);
            return;
        }
        if (args.Key is VirtualKey.Enter or VirtualKey.Space or VirtualKey.GamepadA)
        {
            args.Handled = InvokeSelected(args.Key == VirtualKey.GamepadA ? QuickResumeInputDeviceKind.GameController : QuickResumeInputDeviceKind.Keyboard);
            return;
        }
        if (args.Key is VirtualKey.Delete or VirtualKey.GamepadX)
        {
            args.Handled = RemoveSelected();
        }
    }
}
