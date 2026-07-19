using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class TabbedShellPage : Page
{
    private int _nextTabNumber = 4;

    public TabbedShellPage()
    {
        InitializeComponent();
        Shell.AddTab(CreateTab("home", "Home", "Home content"));
        Shell.AddTab(CreateTab("media", "Media", "Media content"));
        Shell.AddTab(CreateTab("settings", "Settings", "Settings content"));
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        Shell.Items.Clear();
        base.OnNavigatedFrom(e);
    }

    private void OnNewTabClick(object sender, RoutedEventArgs e)
    {
        var id = $"tab-{_nextTabNumber}";
        Shell.AddTab(CreateTab(id, $"Tab {_nextTabNumber}", $"Local deterministic content for {id}."));
        _nextTabNumber++;
        StatusText.Text = $"Added {id}.";
    }

    private void OnMoveLeftClick(object sender, RoutedEventArgs e)
    {
        var index = Shell.SelectedIndex;
        StatusText.Text = Shell.SelectedItem is { } item && Shell.MoveTab(item, index - 1)
            ? $"Moved {item.Id} left."
            : "The selected tab is already at the left edge.";
    }

    private void OnMoveRightClick(object sender, RoutedEventArgs e)
    {
        var index = Shell.SelectedIndex;
        StatusText.Text = Shell.SelectedItem is { } item && Shell.MoveTab(item, index + 1)
            ? $"Moved {item.Id} right."
            : "The selected tab is already at the right edge.";
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) =>
        StatusText.Text = Shell.CloseTab(Shell.SelectedItem) ? "Selected tab closed." : "The selected tab cannot be closed.";

    private void OnTearOutClick(object sender, RoutedEventArgs e) =>
        StatusText.Text = Shell.RequestTearOut() ? "Tear-out request accepted by the host." : "Tear-out request was not handled.";

    private void OnTabClosing(object? sender, TabbedShellTabClosingEventArgs e)
    {
        if (e.Item.IsPinned)
        {
            e.Cancel = true;
            StatusText.Text = $"Pinned tab {e.Item.Id} rejected close.";
        }
    }

    private void OnTabClosed(object? sender, TabbedShellTabClosedEventArgs e) => StatusText.Text = $"Closed {e.Item.Id} ({e.Reason}).";

    private void OnTabReordered(object? sender, TabbedShellTabReorderedEventArgs e) => StatusText.Text = $"Reordered {e.Item.Id}: {e.OldIndex} → {e.NewIndex}.";

    private void OnTearOutRequested(object? sender, TabbedShellTearOutRequestedEventArgs e)
    {
        e.Handled = true;
        StatusText.Text = $"Host received tear-out request for {e.Item.Id}; no window was created.";
    }

    private static TabbedShellItem CreateTab(string id, string header, string text) =>
        new(id, header, new Border
        {
            Padding = new Thickness(24),
            Child = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, FontSize = 20 }
        });
}
