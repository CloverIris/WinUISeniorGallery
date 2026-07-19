using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class OverlayMenuPage : Page
{
    public OverlayMenuPage()
    {
        InitializeComponent();
        var settings = new OverlayMenuItem("settings", "Settings");
        settings.Children.Add(new OverlayMenuItem("appearance", "Appearance"));
        settings.Children.Add(new OverlayMenuItem("privacy", "Privacy"));
        var playback = new OverlayMenuItem("playback", "Playback");
        playback.Children.Add(new OverlayMenuItem("speed", "Speed"));
        playback.Children.Add(new OverlayMenuItem("quality", "Quality"));
        Menu.Items.Add(settings);
        Menu.Items.Add(playback);
        Menu.Items.Add(new OverlayMenuItem("about", "About"));
        Menu.ItemInvoked += OnItemInvoked;
        Menu.NavigationChanged += (_, args) => StatusText.Text = args.Path.Count == 0 ? "Root menu" : $"Path: {string.Join(" / ", args.Path.Select(item => item.Header))}";
    }

    private void OnOpenClick(object sender, RoutedEventArgs e) { Menu.Modality = OverlayMenuModality.Modal; Menu.Open(); }
    private void OnNonModalClick(object sender, RoutedEventArgs e) { Menu.Modality = OverlayMenuModality.NonModal; Menu.Open(); }
    private void OnCloseClick(object sender, RoutedEventArgs e) => Menu.Close();
    private void OnItemInvoked(object? sender, OverlayMenuItemInvokedEventArgs e)
    {
        StatusText.Text = e.EnteredSubmenu ? $"Entered: {e.Item.Header}" : $"Invoked: {e.Item.Header}";
        if (!e.EnteredSubmenu) e.Handled = true;
    }
}
