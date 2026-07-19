using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class HubPanoramaPage : Page
{
    public HubPanoramaPage()
    {
        InitializeComponent();
        Panorama.SetSections(new[]
        {
            new HubSection("home", "Home", new object[] { "Recent", "Pinned", "Recommended" }),
            new HubSection("media", "Media", new object[] { "Albums", "Playlists", "Now playing" }),
            new HubSection("people", "People", new object[] { "Favorites", "Messages", "Teams" }),
        });
        Panorama.SectionChanged += (_, args) => StatusText.Text = $"Section {args.Index}: {args.Current?.Header}";
        UpdateStatus();
    }

    private void OnPreviousClick(object sender, RoutedEventArgs e) => Panorama.Navigate(-1);
    private void OnNextClick(object sender, RoutedEventArgs e) => Panorama.Navigate(1);
    private void OnWrapChanged(object sender, RoutedEventArgs e) => Panorama.IsWrapNavigationEnabled = WrapToggle.IsChecked == true;
    private void UpdateStatus() => StatusText.Text = $"Section {Panorama.SelectedIndex}: {Panorama.SelectedSection?.Header ?? "none"}";
}
